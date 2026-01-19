<script lang="ts">
	import type { CacheEntry } from '$lib/api/types';
	import * as HoverCard from '$lib/components/ui/hover-card';
	import { Skeleton } from '$lib/components/ui/skeleton';

	import { Textarea } from '$lib/components/ui/textarea';

	// @ts-ignore
	import PointerIcon from '@tabler/icons-svelte/icons/hand-click';

	let { entry }: { entry: CacheEntry } = $props();

	let cacheDataPromise = $state<Promise<string> | undefined>(undefined);

	async function loadCacheValue() {
		const response = await fetch(`/api/caches?key=${encodeURIComponent(entry.key)}`);

		if (!response.ok) {
			throw new Error(`Failed to load cache value: ${response.statusText}`);
		}

		const result = await response.json();

		if (result.error) {
			throw new Error(result.error);
		}

		return result.data as string;
	}

	function handleOpenChange(open: boolean) {
		if (open) {
			cacheDataPromise = loadCacheValue();
		}
	}
</script>

<HoverCard.Root onOpenChange={handleOpenChange}>
	<HoverCard.Trigger>
		<div
			class="flex flex-row items-center gap-2 text-sm text-muted-foreground hover:cursor-pointer"
		>
			<PointerIcon class="h-4 w-4" />
			{entry.size} bytes
		</div>
	</HoverCard.Trigger>
	<HoverCard.Content class="w-[90vw]p-4 " side="top" align="start">
		{#await cacheDataPromise}
			<div class="w-full space-y-2">
				<Skeleton class="h-4 w-full" />
				<Skeleton class="h-4 w-full" />
				<Skeleton class="h-4 w-3/4" />
			</div>
		{:then data}
			<div class="flex flex-col">
				<h5 class="m-2 font-bold text-foreground">{entry.key.toUpperCase()}</h5>

				<Textarea value={data} disabled class="scrollbar max-h-40 scroll-auto" />
			</div>
		{:catch error}
			<div class="text-sm text-red-500">{error.message}</div>
		{/await}
	</HoverCard.Content>
</HoverCard.Root>
