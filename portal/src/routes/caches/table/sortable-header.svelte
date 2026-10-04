<script lang="ts">
	import type { Column, Table } from '@tanstack/table-core';
	import type { CacheEntry } from '$lib/api/types/CacheEntry';
	import type { CacheTableMeta } from './columns';
	import { Button } from '$lib/components/ui/button';

	// @ts-ignore
	import ArrowUpIcon from '@tabler/icons-svelte/icons/arrow-up';
	// @ts-ignore
	import ArrowDownIcon from '@tabler/icons-svelte/icons/arrow-down';
	// @ts-ignore
	import SelectorIcon from '@tabler/icons-svelte/icons/selector';

	let {
		column,
		table,
		title
	}: {
		column: Column<CacheEntry, unknown>;
		table: Table<CacheEntry>;
		title: string;
	} = $props();

	const meta = $derived((table.options.meta ?? {}) as CacheTableMeta);
	const sorted = $derived(
		meta.sorting?.[0]?.id === column.id ? (meta.sorting[0].desc ? 'desc' : 'asc') : false
	);
</script>

<Button
	variant="ghost"
	size="sm"
	class="text-muted-foreground hover:text-foreground -ms-2 h-8 px-2 text-xs"
	aria-label={`Sort by ${title}`}
	onclick={() => meta.toggleSort?.(column.id)}
>
	{title}
	{#if sorted === 'asc'}
		<ArrowUpIcon />
	{:else if sorted === 'desc'}
		<ArrowDownIcon />
	{:else}
		<SelectorIcon />
	{/if}
</Button>
