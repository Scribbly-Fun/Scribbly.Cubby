<script lang="ts">
	import Badge from '../badge/badge.svelte';

	let { date }: { date: Date | undefined } = $props();
</script>

<div class="flex w-full flex-wrap gap-2">
	{#if date === undefined || date.getMilliseconds() === 0}
		<Badge variant="outline" class="opacity-50">N/A</Badge>
	{:else}
		<Badge variant="secondary">{date.toUTCString()}</Badge>
	{/if}
</div>
