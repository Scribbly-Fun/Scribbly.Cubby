<script lang="ts">
	import Badge from '../badge/badge.svelte';
	import { isEntryExpired } from '$lib/api/types/CacheEntry';
	import { currentTime } from '$lib/hooks/now.svelte';

	// @ts-ignore
	import ClockIcon from '@tabler/icons-svelte/icons/clock-x';

	let { date }: { date: Date | undefined } = $props();

	const hasExpiration = $derived(!!date && !Number.isNaN(date.getTime()));
	const expired = $derived(hasExpiration && isEntryExpired(date, currentTime()));
	const display = $derived(
		hasExpiration
			? date?.toLocaleString(undefined, {
					dateStyle: 'medium',
					timeStyle: 'short'
				})
			: undefined
	);
</script>

<div class="flex w-full flex-wrap gap-2">
	{#if !hasExpiration}
		<Badge variant="outline" class="opacity-50">N/A</Badge>
	{:else if expired}
		<Badge variant="destructive">
			<ClockIcon />
			{display}
		</Badge>
	{:else}
		<Badge variant="secondary">{display}</Badge>
	{/if}
</div>
