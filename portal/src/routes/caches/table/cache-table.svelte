<script lang="ts">
	import type { SortingState } from '@tanstack/table-core';
	import * as Table from '$lib/components/ui/table/index.js';
	import Button from '$lib/components/ui/button/button.svelte';
	import { Input } from '$lib/components/ui/input';
	import { invalidateAll } from '$app/navigation';
	import type { CacheEntry } from '$lib/api/types/CacheEntry';
	import { compareCacheEntries } from './columns';
	import SortableHeader from './sortable-header.svelte';
	import FlagsBadge from '$lib/components/ui/flags/flags-badge.svelte';
	import ExpirationBadge from '$lib/components/ui/flags/expiration-badge.svelte';
	import SlidingBadge from '$lib/components/ui/flags/sliding-badge.svelte';
	import EncodingBadge from '$lib/components/ui/flags/encoding-badge.svelte';
	import CacheEntryDisplay from '../components/cache-entry-display.svelte';
	import CacheTableActions from './cache-table-actions.svelte';
	import CreateCacheDialog from '../components/cache-entry-dialog.svelte';

	// @ts-ignore
	import RefreshIcon from '@tabler/icons-svelte/icons/refresh';
	// @ts-ignore
	import PlayIcon from '@tabler/icons-svelte/icons/player-play';
	// @ts-ignore
	import PauseIcon from '@tabler/icons-svelte/icons/player-pause';
	// @ts-ignore
	import SearchIcon from '@tabler/icons-svelte/icons/search';

	const POLL_INTERVAL_MS = 2000;

	let { data }: { data: CacheEntry[] } = $props();

	let playing = $state(false);
	let keyFilter = $state('');
	let sorting = $state<SortingState>([]);

	const displayData = $derived.by(() => {
		const query = keyFilter.trim().toLowerCase();
		const rows = query
			? data.filter((entry) => entry.key.toLowerCase().includes(query))
			: data;

		const current = sorting[0];
		if (!current) {
			return rows;
		}

		return [...rows].sort((left, right) => {
			const result = compareCacheEntries(left, right, current.id);
			return current.desc ? -result : result;
		});
	});

	function toggleSort(columnId: string) {
		const current = sorting[0];
		if (current?.id === columnId && !current.desc) {
			sorting = [{ id: columnId, desc: true }];
			return;
		}

		if (current?.id === columnId && current.desc) {
			sorting = [];
			return;
		}

		sorting = [{ id: columnId, desc: false }];
	}

	$effect(() => {
		if (!playing) return;

		console.log('Starting polling');

		const id = setInterval(() => {
			invalidateAll();
		}, POLL_INTERVAL_MS);

		return () => {
			console.log('Stopping polling');
			clearInterval(id);
		};
	});

	function toggleAutopolling() {
		console.log('Toggling autopolling...');
		playing = !playing;
	}

	function refresh() {
		console.log('refreshing...');
		invalidateAll();
	}
</script>

<div class="mb-4 flex flex-row items-center justify-between gap-4">
	<div class="flex min-w-0 flex-row items-center gap-2">
		<CreateCacheDialog />
		<div class="relative">
			<SearchIcon
				class="text-muted-foreground pointer-events-none absolute top-1/2 left-2.5 size-4 -translate-y-1/2"
			/>
			<Input
				id="cache-key-filter"
				type="search"
				placeholder="Filter by key"
				aria-label="Filter by key"
				class="h-9 w-56 ps-8"
				bind:value={keyFilter}
			/>
		</div>
	</div>
	<div class="flex flex-row gap-2 rounded-md border bg-card p-2 shadow-sm">
		<Button
			onclick={refresh}
			variant="ghost"
			size="sm"
			class="hidden sm:flex dark:text-foreground"
			target="_blank"
			rel="noopener noreferrer"
		>
			<RefreshIcon />
		</Button>
		<Button
			onclick={toggleAutopolling}
			variant={playing ? 'destructive' : 'ghost'}
			size="sm"
			class="hidden sm:flex dark:text-foreground"
			target="_blank"
			rel="noopener noreferrer"
		>
			<PauseIcon
				class={`h-[1.2rem] w-[1.2rem] transition-all! ${
					playing ? 'scale-0 rotate-90' : 'scale-100 rotate-0'
				} dark:${playing ? 'scale-100 dark:rotate-0' : 'scale-0 dark:-rotate-90'}`}
			/>
			<PlayIcon
				class={`absolute h-[1.2rem] w-[1.2rem] transition-all! ${
					playing ? 'scale-100 rotate-0' : 'scale-0 -rotate-90'
				} dark:${playing ? 'scale-0 dark:rotate-90' : 'scale-100 dark:rotate-0'}`}
			/>
		</Button>
	</div>
</div>

<div class="rounded-md border bg-card shadow-sm">
	<Table.Root>
		<Table.Header>
			<Table.Row>
				<Table.Head>
					<SortableHeader columnId="key" title="Key" {sorting} {toggleSort} />
				</Table.Head>
				<Table.Head>
					<SortableHeader columnId="flags" title="Flags" {sorting} {toggleSort} />
				</Table.Head>
				<Table.Head>
					<SortableHeader columnId="expiration" title="Expiration" {sorting} {toggleSort} />
				</Table.Head>
				<Table.Head>
					<SortableHeader
						columnId="sliding"
						title="Sliding Duration"
						{sorting}
						{toggleSort}
					/>
				</Table.Head>
				<Table.Head>
					<SortableHeader columnId="encoding" title="Encoding" {sorting} {toggleSort} />
				</Table.Head>
				<Table.Head>
					<SortableHeader columnId="size" title="Size" {sorting} {toggleSort} />
				</Table.Head>
				<Table.Head></Table.Head>
			</Table.Row>
		</Table.Header>
		<Table.Body>
			{#each displayData as entry (entry.key)}
				<Table.Row>
					<Table.Cell>{entry.key}</Table.Cell>
					<Table.Cell>
						<FlagsBadge flags={entry.flags} />
					</Table.Cell>
					<Table.Cell>
						<ExpirationBadge
							date={entry.expiration ? new Date(entry.expiration) : undefined}
						/>
					</Table.Cell>
					<Table.Cell>
						<SlidingBadge duration={entry.sliding_duration} />
					</Table.Cell>
					<Table.Cell>
						<EncodingBadge encoding={entry.encoding} />
					</Table.Cell>
					<Table.Cell>
						<CacheEntryDisplay {entry} />
					</Table.Cell>
					<Table.Cell>
						<CacheTableActions {entry} />
					</Table.Cell>
				</Table.Row>
			{:else}
				<Table.Row>
					<Table.Cell colspan={7} class="h-24 text-center">No results.</Table.Cell>
				</Table.Row>
			{/each}
		</Table.Body>
	</Table.Root>
</div>
