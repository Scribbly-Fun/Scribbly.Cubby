<script lang="ts">
	import type { SortingState } from '@tanstack/table-core';
	import { Button } from '$lib/components/ui/button';

	// @ts-ignore
	import ArrowUpIcon from '@tabler/icons-svelte/icons/arrow-up';
	// @ts-ignore
	import ArrowDownIcon from '@tabler/icons-svelte/icons/arrow-down';
	// @ts-ignore
	import SelectorIcon from '@tabler/icons-svelte/icons/selector';

	let {
		columnId,
		title,
		sorting,
		toggleSort
	}: {
		columnId: string;
		title: string;
		sorting: SortingState;
		toggleSort: (columnId: string) => void;
	} = $props();

	const sorted = $derived(
		sorting[0]?.id === columnId ? (sorting[0].desc ? 'desc' : 'asc') : false
	);
</script>

<Button
	variant="ghost"
	size="sm"
	class="text-muted-foreground hover:text-foreground -ms-2 h-8 px-2 text-xs"
	aria-label={`Sort by ${title}`}
	onclick={() => toggleSort(columnId)}
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
