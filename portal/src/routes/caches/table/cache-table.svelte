<script lang="ts" generics="TData, TValue">
	import {
		type ColumnDef,
		type SortingState,
		getCoreRowModel,
		getSortedRowModel
	} from '@tanstack/table-core';
	import { createSvelteTable, FlexRender } from '$lib/components/ui/data-table/index.js';
	import * as Table from '$lib/components/ui/table/index.js';
	import Button from '$lib/components/ui/button/button.svelte';
	import { Input } from '$lib/components/ui/input';
	import { invalidateAll } from '$app/navigation';

	// @ts-ignore
	import RefreshIcon from '@tabler/icons-svelte/icons/refresh';
	// @ts-ignore
	import PlayIcon from '@tabler/icons-svelte/icons/player-play';
	// @ts-ignore
	import PauseIcon from '@tabler/icons-svelte/icons/player-pause';
	// @ts-ignore
	import SearchIcon from '@tabler/icons-svelte/icons/search';

	import CreateCacheDialog from '../components/cache-entry-dialog.svelte';

	const POLL_INTERVAL_MS = 2000;

	type DataTableProps<TData, TValue> = {
		columns: ColumnDef<TData, TValue>[];
		data: TData[];
	};

	let { data, columns }: DataTableProps<TData, TValue> = $props();

	let playing = $state(false);
	let keyFilter = $state('');
	let sorting = $state<SortingState>([]);

	const filteredData = $derived.by(() => {
		const query = keyFilter.trim().toLowerCase();
		if (!query) {
			return data;
		}

		return data.filter((row) => {
			if (!row || typeof row !== 'object' || !('key' in row)) {
				return false;
			}

			return String(row.key ?? '')
				.toLowerCase()
				.includes(query);
		});
	});

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

	const table = createSvelteTable({
		get data() {
			return filteredData;
		},
		get columns() {
			return columns;
		},
		getCoreRowModel: getCoreRowModel(),
		getSortedRowModel: getSortedRowModel(),
		get state() {
			return {
				sorting
			};
		},
		onSortingChange: (updater) => {
			sorting = typeof updater === 'function' ? updater(sorting) : updater;
		}
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
			{#each table.getHeaderGroups() as headerGroup (headerGroup.id)}
				<Table.Row>
					{#each headerGroup.headers as header (header.id)}
						<Table.Head colspan={header.colSpan}>
							{#if !header.isPlaceholder}
								<FlexRender
									content={header.column.columnDef.header}
									context={header.getContext()}
								/>
							{/if}
						</Table.Head>
					{/each}
				</Table.Row>
			{/each}
		</Table.Header>
		<Table.Body>
			{#each table.getRowModel().rows as row (row.id)}
				<Table.Row data-state={row.getIsSelected() && 'selected'}>
					{#each row.getVisibleCells() as cell (cell.id)}
						<Table.Cell>
							<FlexRender content={cell.column.columnDef.cell} context={cell.getContext()} />
						</Table.Cell>
					{/each}
				</Table.Row>
			{:else}
				<Table.Row>
					<Table.Cell colspan={columns.length} class="h-24 text-center">No results.</Table.Cell>
				</Table.Row>
			{/each}
		</Table.Body>
	</Table.Root>
</div>
