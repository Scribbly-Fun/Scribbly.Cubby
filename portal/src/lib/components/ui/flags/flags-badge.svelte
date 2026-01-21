<script lang="ts" module>
	import type { Flags } from '$lib/api/types';
	import { EntryFlags, FLAG_NAMES, hasFlag } from '$lib/api/types/EntryFlags';

	// @ts-ignore
	import CoffinIcon from '@tabler/icons-svelte/icons/coffin';

	function getActiveFlagNames(flags: Flags): string[] {
		return Object.values(EntryFlags)
			.filter((flag) => hasFlag(flags, flag as EntryFlags))
			.map((flag) => FLAG_NAMES[flag as EntryFlags]);
	}
</script>

<script lang="ts">
	import Badge from '../badge/badge.svelte';

	let { flags }: { flags: Flags } = $props();
	let activeFlagNames = $derived(getActiveFlagNames(flags));
</script>

<div class="flex w-full flex-wrap gap-2">
	{#each activeFlagNames as flagName (flagName)}
		{#if flagName === 'None'}
			<Badge variant="outline" class="opacity-50">{flagName}</Badge>
		{:else if flagName === 'Tombstone'}
			<Badge variant="destructive">
				<CoffinIcon /> {flagName}</Badge
			>
		{:else if flagName === 'Sliding'}
			<Badge variant="outline" class="bg-(--warning) text-white dark:bg-(--warning)"
				>{flagName}</Badge
			>
		{:else}
			<Badge>{flagName}</Badge>
		{/if}
	{/each}
</div>
