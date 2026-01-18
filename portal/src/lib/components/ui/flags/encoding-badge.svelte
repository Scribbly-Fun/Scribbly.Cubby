<script lang="ts">
	import type { Encoding } from '$lib/api/types';
	import Badge from '../badge/badge.svelte';

	// @ts-ignore
	import JsonIcon from '@tabler/icons-svelte/icons/json';

	// @ts-ignore
	import BinaryIcon from '@tabler/icons-svelte/icons/binary';

	// @ts-ignore
	import TextIcon from '@tabler/icons-svelte/icons/abc';

	let { encoding }: { encoding: Encoding } = $props();
</script>

<div class="flex w-full flex-wrap gap-2">
	{#if encoding === 'None'}
		<Badge><BinaryIcon />{encoding}</Badge>
	{:else if encoding === 'Utf8String' || encoding === 'Utf16String'}
		<Badge>
			<TextIcon /> {encoding}</Badge
		>
	{:else if encoding === 'Json'}
		<Badge>
			<JsonIcon /> {encoding}</Badge
		>
	{:else if encoding === 'MessagePack'}
		<Badge>{encoding}</Badge>
	{:else}
		<Badge>{encoding}</Badge>
	{/if}
</div>
