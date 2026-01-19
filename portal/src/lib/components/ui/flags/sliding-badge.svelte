<script lang="ts">
	import type { SlidingDuration } from '$lib/api/types';
	import Badge from '../badge/badge.svelte';

	let { duration }: { duration: SlidingDuration | undefined } = $props();
</script>

<div class="flex w-full flex-wrap gap-2">
	{#if duration === undefined || duration === '00:00:00'}
		<Badge variant="outline" class="opacity-50">N/A</Badge>
	{:else}
		<Badge class="bg-blue-500 text-white dark:bg-blue-600">{duration}</Badge>
	{/if}
</div>
