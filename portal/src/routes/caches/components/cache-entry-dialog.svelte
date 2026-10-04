<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import * as Dialog from '$lib/components/ui/dialog/index.js';
	import { Input } from '$lib/components/ui/input';
	import { Label } from '$lib/components/ui/label';
	import { Textarea } from '$lib/components/ui/textarea';
	import * as Select from '$lib/components/ui/select/index.js';
	import * as Tabs from '$lib/components/ui/tabs/index.js';
	import { Checkbox } from '$lib/components/ui/checkbox';
	import {
		ENTRY_ENCODINGS,
		ENCODING_LABELS,
		type EntryEncoding
	} from '$lib/api/types/EntryEncoding';
	import { invalidateAll } from '$app/navigation';
	import { deserialize } from '$app/forms';

	// @ts-ignore
	import AddIcon from '@tabler/icons-svelte/icons/tag-plus';

	type ValueSource = 'file' | 'json' | 'text';

	let open = $state(false);
	let key = $state('');
	let encoding = $state<EntryEncoding>('Utf8String');
	let encodingTouched = $state(false);
	let compressed = $state(false);
	let source = $state<ValueSource>('text');
	let textValue = $state('');
	let jsonValue = $state('');
	let files = $state<FileList | undefined>(undefined);
	let errorMessage = $state('');
	let submitting = $state(false);

	const selectedFile = $derived(files?.[0]);
	const encodingLabel = $derived(ENCODING_LABELS[encoding]);

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

	function resetForm() {
		key = '';
		encoding = 'Utf8String';
		encodingTouched = false;
		compressed = false;
		source = 'text';
		textValue = '';
		jsonValue = '';
		files = undefined;
		errorMessage = '';
		submitting = false;
	}

	function handleOpenChange(nextOpen: boolean) {
		open = nextOpen;
		if (!nextOpen) {
			resetForm();
		}
	}

	async function handleSubmit(event: Event) {
		event.preventDefault();
		errorMessage = '';

		if (!key.trim()) {
			errorMessage = 'Key is required.';
			return;
		}

		const formData = new FormData();
		formData.append('key', key.trim());
		formData.append('encoding', encoding);
		formData.append('compressed', String(compressed));
		formData.append('source', source);

		if (source === 'file') {
			if (!selectedFile) {
				errorMessage = 'Choose a file to upload as raw bytes.';
				return;
			}
			formData.append('file', selectedFile);
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
					'Failed to create cache entry.';
				return;
			}

			if (result.type === 'error') {
				errorMessage = result.error?.message ?? 'Failed to create cache entry.';
				return;
			}

			open = false;
			resetForm();
			await invalidateAll();
		} catch (error) {
			errorMessage = error instanceof Error ? error.message : 'Failed to create cache entry.';
		} finally {
			submitting = false;
		}
	}
</script>

<Dialog.Root bind:open onOpenChange={handleOpenChange}>
	<Dialog.Trigger>
		<Button variant="outline" type="button"><AddIcon /> Create Entry</Button>
	</Dialog.Trigger>
	<Dialog.Content class="sm:max-w-xl">
		<form class="grid gap-4" onsubmit={handleSubmit}>
			<Dialog.Header>
				<Dialog.Title>Create New Cache Entry</Dialog.Title>
				<Dialog.Description>
					Set a key, choose encoding and compression, then provide a value as a file, JSON, or text.
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

			<div class="flex items-center gap-2">
				<Checkbox id="cache-entry-compressed" bind:checked={compressed} />
				<Label for="cache-entry-compressed">Compress value (Brotli)</Label>
			</div>

			<div class="grid gap-2">
				<Label>Value</Label>
				<Tabs.Root bind:value={source}>
					<Tabs.List class="grid w-full grid-cols-3">
						<Tabs.Trigger value="file">File</Tabs.Trigger>
						<Tabs.Trigger value="json">JSON</Tabs.Trigger>
						<Tabs.Trigger value="text">Text</Tabs.Trigger>
					</Tabs.List>
					<Tabs.Content value="file" class="pt-3">
						<Input id="cache-entry-file" name="file" type="file" bind:files />
						{#if selectedFile}
							<p class="mt-2 text-xs text-muted-foreground">
								{selectedFile.name} · {selectedFile.size} bytes
							</p>
						{:else}
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
			</div>

			{#if errorMessage}
				<p class="text-sm text-destructive">{errorMessage}</p>
			{/if}

			<Dialog.Footer>
				<Button type="button" variant="outline" onclick={() => (open = false)}>Cancel</Button>
				<Button type="submit" disabled={submitting}>
					{submitting ? 'Creating…' : 'Create Entry'}
				</Button>
			</Dialog.Footer>
		</form>
	</Dialog.Content>
</Dialog.Root>
