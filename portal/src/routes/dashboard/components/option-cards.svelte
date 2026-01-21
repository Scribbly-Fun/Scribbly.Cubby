<script lang="ts">
	// @ts-ignore
	import StoreIcon from "@tabler/icons-svelte/icons/backpack";
	// @ts-ignore
	import CpuIcon from "@tabler/icons-svelte/icons/cpu";
	// @ts-ignore
	import CapacityIcon from "@tabler/icons-svelte/icons/shopping-cart";
	// @ts-ignore
	import StrategyIcon from "@tabler/icons-svelte/icons/chess-king";

	import * as Card from "$lib/components/ui/card/index.js";
	
	import type { CubbyOptions } from "$lib/api/types/index.js";

	let { cubby_options }: { cubby_options: CubbyOptions | undefined } = $props()
</script>

<div
	class="*:data-[slot=card]:from-primary/5 *:data-[slot=card]:to-card dark:*:data-[slot=card]:bg-card grid grid-cols-1 gap-4 px-4 *:data-[slot=card]:bg-linear-to-t *:data-[slot=card]:shadow-xs lg:px-6 @xl/main:grid-cols-2 @5xl/main:grid-cols-4"
>
	<Card.Root class="@container/card">
		<Card.Header>
			<Card.Description>CUBBY STORE</Card.Description>
			<Card.Title class="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
				{cubby_options?.store || 'N/A'}
			</Card.Title>
			<Card.Action>
				<StoreIcon />
			</Card.Action>
		</Card.Header>
		<Card.Footer class="flex-col items-start gap-1.5 text-sm">
			<div class="line-clamp-1 flex gap-2 font-medium">
				Using the {cubby_options?.store} storage</div>
			<div class="text-muted-foreground">Set the CUBBY__STORE environment variable</div>
		</Card.Footer>
	</Card.Root>
	
	<Card.Root class="@container/card">
		<Card.Header>
			<Card.Description>CPU CORES</Card.Description>
			<Card.Title class="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
				{cubby_options?.cores || 'N/A'}
			</Card.Title>
			<Card.Action>
				<CpuIcon />
			</Card.Action>
		</Card.Header>
		<Card.Footer class="flex-col items-start gap-1.5 text-sm">
			<div class="line-clamp-1 flex gap-2 font-medium">
				Currently configured with specific CPU cores</div>
			<div class="text-muted-foreground">Set the CUBBY__CORES environment variable</div>
		</Card.Footer>
	</Card.Root>

	<Card.Root class="@container/card">
		<Card.Header>
			<Card.Description>Capactity</Card.Description>
			<Card.Title class="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
				{cubby_options?.capacity || 'N/A'}
			</Card.Title>
			<Card.Action>
				<CapacityIcon />
			</Card.Action>
		</Card.Header>
		<Card.Footer class="flex-col items-start gap-1.5 text-sm">
			<div class="line-clamp-1 flex gap-2 font-medium">
				{cubby_options?.capacity || 'N/A'} total capacity
			</div>
			<div class="text-muted-foreground">set the CUBBY__CAPACITY environment variable
			</div>
		</Card.Footer>
	</Card.Root>

	<Card.Root class="@container/card">
		<Card.Header>
			<Card.Description>Strategy</Card.Description>
			<Card.Title class="text-2xl font-semibold tabular-nums @[250px]/card:text-3xl">
				{cubby_options?.cleanup?.strategy || 'N/A'}
			</Card.Title>
			<Card.Action>
				<StrategyIcon />
			</Card.Action>
		</Card.Header>
		<Card.Footer class="flex-col items-start gap-1.5 text-sm">
			<div class="line-clamp-1 flex gap-2 font-medium">
				Async cleanup strategy {cubby_options?.cleanup?.strategy || 'N/A'}
			</div>
			<div class="text-muted-foreground">Delay {cubby_options?.cleanup?.delay || 'N/A'}</div>
		</Card.Footer>
	</Card.Root>
</div>
