<script lang="ts" generics="TData, TValue">
	import { type ColumnDef, getCoreRowModel } from '@tanstack/table-core';
	import { createSvelteTable, FlexRender } from '$lib/components/ui/data-table/index.js';
	import * as Table from '$lib/components/ui/table/index.js';
	import Button from '$lib/components/ui/button/button.svelte';
	import { invalidateAll } from '$app/navigation';

	// @ts-ignore
	import RefreshIcon from '@tabler/icons-svelte/icons/refresh';
	// @ts-ignore
	import PlayIcon from '@tabler/icons-svelte/icons/player-play';
	// @ts-ignore
	import PauseIcon from '@tabler/icons-svelte/icons/player-pause';

	import CreateCacheDialog from './create-cache-dialog.svelte';

	type DataTableProps<TData, TValue> = {
		columns: ColumnDef<TData, TValue>[];
		data: TData[];
	};

	let { data, columns }: DataTableProps<TData, TValue> = $props();
	let playing = $state(false);

	const table = createSvelteTable({
		get data() {
			return data;
		},
		get columns() {
			return columns;
		},
		getCoreRowModel: getCoreRowModel()
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
	<CreateCacheDialog />
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
			variant="ghost"
			size="sm"
			class="hidden sm:flex dark:text-foreground"
			target="_blank"
			rel="noopener noreferrer"
		>
			<PlayIcon
				class={`h-[1.2rem] w-[1.2rem] transition-all! ${
					playing ? 'scale-0 rotate-90' : 'scale-100 rotate-0'
				} dark:${playing ? 'scale-100 dark:rotate-0' : 'scale-0 dark:-rotate-90'}`}
			/>
			<PauseIcon
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
