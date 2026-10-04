<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input';
	import { Label } from '$lib/components/ui/label';
	import { Textarea } from '$lib/components/ui/textarea';
	import * as Select from '$lib/components/ui/select/index.js';
	import * as Tabs from '$lib/components/ui/tabs/index.js';
	import { Checkbox } from '$lib/components/ui/checkbox';
	import { Skeleton } from '$lib/components/ui/skeleton';
	import {
		ENTRY_ENCODINGS,
		ENCODING_LABELS,
		type EntryEncoding
	} from '$lib/api/types/EntryEncoding';
	import {
		EXPIRATION_MODES,
		EXPIRATION_LABELS,
		isPositiveTimeSpan,
		optionsFromEntry,
		valueSourceFromEncoding,
		type ExpirationMode
	} from '$lib/api/types/CacheEntryOptions';
	import type { CacheEntry } from '$lib/api/types/CacheEntry';
	import { invalidateAll } from '$app/navigation';
	import { deserialize } from '$app/forms';

	// @ts-ignore
	import AddIcon from '@tabler/icons-svelte/icons/tag-plus';

	type ValueSource = 'file' | 'json' | 'text';

	let {
		entry = undefined,
		showTrigger = true,
		open = $bindable(false)
	}: {
		entry?: CacheEntry;
		showTrigger?: boolean;
		open?: boolean;
	} = $props();

	let key = $state('');
	let encoding = $state<EntryEncoding>('Utf8String');
	let encodingTouched = $state(false);
	let compressed = $state(false);
	let expiration = $state<ExpirationMode>('Never');
	let duration = $state('00:05:00');
	let source = $state<ValueSource>('text');
	let textValue = $state('');
	let jsonValue = $state('');
	let rawValue = $state('');
	let files = $state<FileList | undefined>(undefined);
	let keepExistingValue = $state(false);
	let loadingValue = $state(false);
	let errorMessage = $state('');
	let submitting = $state(false);
	let loadedKey = $state<string | undefined>(undefined);

	const isEdit = $derived(!!entry);
	const selectedFile = $derived(files?.[0]);
	const encodingLabel = $derived(ENCODING_LABELS[encoding]);
	const expirationLabel = $derived(EXPIRATION_LABELS[expiration]);
	const showDuration = $derived(expiration !== 'Never');

	$effect(() => {
		if (encodingTouched) return;

		if (source === 'json') {
			encoding = 'Json';
		} else if (source === 'file') {
			encoding = 'None';
		} else {
			encoding = 'Utf8String';
		}
	});

	$effect(() => {
		if (!open) {
			loadedKey = undefined;
			return;
		}

		if (entry && loadedKey !== entry.key) {
			loadedKey = entry.key;
			void loadEntry(entry);
			return;
		}

		if (!entry && loadedKey !== '') {
			loadedKey = '';
			resetForm();
		}
	});

	function resetForm() {
		key = '';
		encoding = 'Utf8String';
		encodingTouched = false;
		compressed = false;
		expiration = 'Never';
		duration = '00:05:00';
		source = 'text';
		textValue = '';
		jsonValue = '';
		rawValue = '';
		files = undefined;
		keepExistingValue = false;
		loadingValue = false;
		errorMessage = '';
		submitting = false;
	}

	function handleOpenChange(nextOpen: boolean) {
		open = nextOpen;
		if (!nextOpen) {
			resetForm();
		}
	}

	async function loadEntry(current: CacheEntry) {
		resetForm();
		encodingTouched = true;
		key = current.key;

		const options = optionsFromEntry(current);
		encoding = options.encoding;
		compressed = options.compressed;
		expiration = options.expiration;
		duration = options.duration ?? '00:05:00';
		source = valueSourceFromEncoding(current.encoding);
		keepExistingValue = source === 'file';

		loadingValue = true;
		errorMessage = '';

		try {
			const params = new URLSearchParams({
				key: current.key,
				encoding: current.encoding
			});
			const response = await fetch(`/api/caches?${params.toString()}`);
			if (!response.ok) {
				throw new Error(`Failed to load cache value: ${response.statusText}`);
			}

			const result = (await response.json()) as { data?: string; error?: string };
			if (result.error || result.data === undefined) {
				throw new Error(result.error ?? 'Failed to load cache value.');
			}

			if (source === 'json') {
				jsonValue = formatJson(result.data);
			} else if (source === 'text') {
				textValue = result.data;
			} else {
				rawValue = result.data;
			}
		} catch (error) {
			errorMessage = error instanceof Error ? error.message : 'Failed to load cache value.';
		} finally {
			loadingValue = false;
		}
	}

	function formatJson(value: string): string {
		try {
			return JSON.stringify(JSON.parse(value), null, 2);
		} catch {
			return value;
		}
	}

	async function handleSubmit(event: Event) {
		event.preventDefault();
		errorMessage = '';

		if (!key.trim()) {
			errorMessage = 'Key is required.';
			return;
		}

		if (expiration !== 'Never' && !isPositiveTimeSpan(duration)) {
			errorMessage = 'Expiration duration must be a TimeSpan greater than zero, such as 00:05:00.';
			return;
		}

		const formData = new FormData();
		formData.append('key', key.trim());
		formData.append('encoding', encoding);
		formData.append('compressed', String(compressed));
		formData.append('expiration', expiration);
		formData.append('duration', duration);
		formData.append('source', source);

		if (source === 'file') {
			if (selectedFile) {
				formData.append('file', selectedFile);
			} else if (keepExistingValue) {
				formData.append('keepValue', 'true');
			} else {
				errorMessage = 'Choose a file to upload as raw bytes.';
				return;
			}
		} else if (source === 'json') {
			try {
				JSON.parse(jsonValue);
			} catch {
				errorMessage = 'Value must be valid JSON.';
				return;
			}
			formData.append('value', jsonValue);
		} else {
			formData.append('value', textValue);
		}

		submitting = true;

		try {
			const response = await fetch('?/create', {
				method: 'POST',
				body: formData
			});

			const result = deserialize(await response.text());
			if (result.type === 'failure') {
				errorMessage =
					(result.data as { message?: string } | undefined)?.message ??
					'Failed to save cache entry.';
				return;
			}

			if (result.type === 'error') {
				errorMessage = result.error?.message ?? 'Failed to save cache entry.';
				return;
			}

			open = false;
			resetForm();
			await invalidateAll();
		} catch (error) {
			errorMessage = error instanceof Error ? error.message : 'Failed to save cache entry.';
		} finally {
			submitting = false;
		}
	}
</script>

<Dialog.Root bind:open onOpenChange={handleOpenChange}>
	{#if showTrigger}
		<Dialog.Trigger>
			<Button variant="outline" type="button"><AddIcon /> Create Entry</Button>
		</Dialog.Trigger>
	{/if}
	<Dialog.Content class="max-h-[90vh] overflow-y-auto sm:max-w-xl">
		<form class="grid gap-4" onsubmit={handleSubmit}>
			<Dialog.Header>
				<Dialog.Title>{isEdit ? 'Edit Cache Entry' : 'Create New Cache Entry'}</Dialog.Title>
				<Dialog.Description>
					{isEdit
						? 'Update cache entry options and value. Raw byte values are shown as stored.'
						: 'Configure cache entry options, then provide a value as a file, JSON, or text.'}
				</Dialog.Description>
			</Dialog.Header>

			<div class="grid gap-2">
				<Label for="cache-entry-key">Key</Label>
				<Input
					id="cache-entry-key"
					name="key"
					placeholder="cache-key"
					autocomplete="off"
					bind:value={key}
					required
					disabled={isEdit}
				/>
			</div>

			<div class="grid gap-2">
				<Label for="cache-entry-encoding">Encoding</Label>
				<Select.Root
					type="single"
					value={encoding}
					onValueChange={(value) => {
						if (!value) return;
						encoding = value as EntryEncoding;
						encodingTouched = true;
					}}
				>
					<Select.Trigger id="cache-entry-encoding" class="w-full" aria-label="Select encoding">
						<span data-slot="select-value">{encodingLabel}</span>
					</Select.Trigger>
					<Select.Content>
						{#each ENTRY_ENCODINGS as option (option)}
							<Select.Item value={option}>{ENCODING_LABELS[option]}</Select.Item>
						{/each}
					</Select.Content>
				</Select.Root>
			</div>

			<div class="grid gap-2">
				<Label for="cache-entry-expiration">Expiration</Label>
				<Select.Root
					type="single"
					value={expiration}
					onValueChange={(value) => {
						if (!value) return;
						expiration = value as ExpirationMode;
						if (expiration !== 'Never' && !duration) {
							duration = '00:05:00';
						}
					}}
				>
					<Select.Trigger id="cache-entry-expiration" class="w-full" aria-label="Select expiration">
						<span data-slot="select-value">{expirationLabel}</span>
					</Select.Trigger>
					<Select.Content>
						{#each EXPIRATION_MODES as option (option)}
							<Select.Item value={option}>{EXPIRATION_LABELS[option]}</Select.Item>
						{/each}
					</Select.Content>
				</Select.Root>
				<p class="text-xs text-muted-foreground">
					Absolute expires after the duration. Sliding refreshes that duration on each read.
				</p>
			</div>

			{#if showDuration}
				<div class="grid gap-2">
					<Label for="cache-entry-duration">Duration</Label>
					<Input
						id="cache-entry-duration"
						name="duration"
						placeholder="00:05:00"
						autocomplete="off"
						bind:value={duration}
					/>
					<p class="text-xs text-muted-foreground">TimeSpan format, for example 00:05:00.</p>
				</div>
			{/if}

			<div class="flex items-center gap-2">
				<Checkbox id="cache-entry-compressed" bind:checked={compressed} />
				<Label for="cache-entry-compressed">Compressed</Label>
			</div>

			<div class="grid gap-2">
				<Label>Value</Label>
				{#if loadingValue}
					<div class="space-y-2 pt-2">
						<Skeleton class="h-4 w-full" />
						<Skeleton class="h-4 w-full" />
						<Skeleton class="h-24 w-full" />
					</div>
				{:else}
					<Tabs.Root bind:value={source}>
						<Tabs.List class="grid w-full grid-cols-3">
							<Tabs.Trigger value="file">File</Tabs.Trigger>
							<Tabs.Trigger value="json">JSON</Tabs.Trigger>
							<Tabs.Trigger value="text">Text</Tabs.Trigger>
						</Tabs.List>
						<Tabs.Content value="file" class="pt-3">
							{#if keepExistingValue}
								<Textarea
									value={rawValue}
									disabled
									class="scrollbar min-h-32 font-mono"
									aria-label="Stored raw value"
								/>
								<p class="mt-2 text-xs text-muted-foreground">
									Stored raw bytes. Choose a file only if you want to replace this value.
								</p>
							{/if}
							<Input id="cache-entry-file" name="file" type="file" bind:files class="mt-2" />
							{#if selectedFile}
								<p class="mt-2 text-xs text-muted-foreground">
									{selectedFile.name} · {selectedFile.size} bytes
								</p>
							{:else if !keepExistingValue}
								<p class="mt-2 text-xs text-muted-foreground">
									The file is stored as raw bytes. Use encoding to describe how clients should read
									it.
								</p>
							{/if}
						</Tabs.Content>
						<Tabs.Content value="json" class="pt-3">
							<Textarea
								id="cache-entry-json"
								name="value"
								class="min-h-32 font-mono"
								placeholder={'{\n  "hello": "cubby"\n}'}
								bind:value={jsonValue}
							/>
						</Tabs.Content>
						<Tabs.Content value="text" class="pt-3">
							<Textarea
								id="cache-entry-text"
								name="value"
								class="min-h-32"
								placeholder="Enter the cache value"
								bind:value={textValue}
							/>
						</Tabs.Content>
					</Tabs.Root>
				{/if}
			</div>

			{#if errorMessage}
				<p class="text-sm text-destructive">{errorMessage}</p>
			{/if}

			<Dialog.Footer>
				<Button type="button" variant="outline" onclick={() => (open = false)}>Cancel</Button>
				<Button type="submit" disabled={submitting || loadingValue}>
					{submitting ? 'Saving…' : isEdit ? 'Save Changes' : 'Create Entry'}
				</Button>
			</Dialog.Footer>
		</form>
	</Dialog.Content>
</Dialog.Root>
