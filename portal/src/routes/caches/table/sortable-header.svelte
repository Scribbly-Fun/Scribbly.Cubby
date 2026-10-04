<script lang="ts">
	import type { Column } from '@tanstack/table-core';
	import type { CacheEntry } from '$lib/api/types/CacheEntry';
	import { Button } from '$lib/components/ui/button';

	// @ts-ignore
	import ArrowUpIcon from '@tabler/icons-svelte/icons/arrow-up';
	// @ts-ignore
	import ArrowDownIcon from '@tabler/icons-svelte/icons/arrow-down';
	// @ts-ignore
	import SelectorIcon from '@tabler/icons-svelte/icons/selector';

	let {
		column,
		title
	}: {
		column: Column<CacheEntry, unknown>;
		title: string;
	} = $props();

	const sorted = $derived(column.getIsSorted());
</script>

<Button
	variant="ghost"
	size="sm"
	class="text-muted-foreground hover:text-foreground -ms-2 h-8 px-2 text-xs"
	aria-label={`Sort by ${title}`}
	onclick={() => column.toggleSorting(sorted === 'asc')}
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
