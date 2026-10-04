<script lang="ts">
	import * as Chart from "$lib/components/ui/chart/index.js";
	import * as Card from "$lib/components/ui/card/index.js";
	import * as Select from "$lib/components/ui/select/index.js";
	import * as ToggleGroup from "$lib/components/ui/toggle-group/index.js";
	import { goto } from "$app/navigation";
	import { scaleUtc } from "d3-scale";
	import { Area, AreaChart } from "layerchart";
	import { curveNatural } from "d3-shape";
	import {
		isCleanupMetricsRange,
		type CleanupMetrics,
		type CleanupMetricsRange
	} from "$lib/api/types";

	let {
		metrics,
		range
	}: {
		metrics: CleanupMetrics;
		range: CleanupMetricsRange;
	} = $props();

	let selectedRange = $state<CleanupMetricsRange>(range);
	let navigating = $state(false);

	$effect(() => {
		selectedRange = range;
	});

	const chartData = $derived(
		metrics.points.map((point) => ({
			date: new Date(point.timestamp),
			items: point.items_total,
			serviced: point.items_serviced,
			removed:
				point.removed_tombstone + point.removed_expired + point.removed_sliding
		}))
	);

	const xDomain = $derived<[Date, Date]>([new Date(metrics.from), new Date(metrics.to)]);

	const selectedLabel = $derived.by(() => {
		switch (selectedRange) {
			case "1h":
				return "Last Hour";
			case "3h":
				return "Last 3 Hours";
			case "24h":
				return "Last 24 Hours";
			case "3d":
				return "Last 3 Days";
			default:
				return "Last Hour";
		}
	});

	async function selectRange(nextRange: string | undefined) {
		if (!isCleanupMetricsRange(nextRange) || nextRange === range || navigating) {
			selectedRange = range;
			return;
		}

		navigating = true;
		selectedRange = nextRange;

		try {
			await goto(`?range=${nextRange}`, {
				keepFocus: true,
				noScroll: true,
				invalidateAll: true,
				replaceState: true
			});
		} finally {
			navigating = false;
		}
	}

	const chartConfig = {
		items: { label: "Items", color: "var(--primary)" },
		serviced: { label: "Serviced", color: "var(--warning)" },
		removed: { label: "Removed", color: "var(--destructive)" }
	} satisfies Chart.ChartConfig;
</script>

<Card.Root class="@container/card">
	<Card.Header>
		<Card.Title>Async Cleanup</Card.Title>
		<Card.Description>
			<span class="hidden @[540px]/card:block">
				Resolution: {metrics.resolution}
			</span>
			<span class="@[540px]/card:hidden">{metrics.resolution}</span>
		</Card.Description>
		<Card.Action>
			<ToggleGroup.Root
				type="single"
				bind:value={selectedRange}
				onValueChange={selectRange}
				variant="outline"
				class="hidden *:data-[slot=toggle-group-item]:px-4! @[767px]/card:flex"
			>
				<ToggleGroup.Item value="1h">Last Hour</ToggleGroup.Item>
				<ToggleGroup.Item value="3h">Last 3 Hours</ToggleGroup.Item>
				<ToggleGroup.Item value="24h">Last 24 Hours</ToggleGroup.Item>
				<ToggleGroup.Item value="3d">Last 3 Days</ToggleGroup.Item>
			</ToggleGroup.Root>
			<Select.Root type="single" bind:value={selectedRange} onValueChange={selectRange}>
				<Select.Trigger
					size="sm"
					class="flex w-44 **:data-[slot=select-value]:block **:data-[slot=select-value]:truncate @[767px]/card:hidden"
					aria-label="Select a value"
				>
					<span data-slot="select-value">
						{selectedLabel}
					</span>
				</Select.Trigger>
				<Select.Content class="rounded-xl">
					<Select.Item value="1h" class="rounded-lg">Last Hour</Select.Item>
					<Select.Item value="3h" class="rounded-lg">Last 3 Hours</Select.Item>
					<Select.Item value="24h" class="rounded-lg">Last 24 Hours</Select.Item>
					<Select.Item value="3d" class="rounded-lg">Last 3 Days</Select.Item>
				</Select.Content>
			</Select.Root>
		</Card.Action>
	</Card.Header>
	<Card.Content class="px-2 pt-4 sm:px-6 sm:pt-6">
		{#if chartData.length === 0}
			<div
				class="text-muted-foreground flex h-62.5 items-center justify-center text-sm"
			>
				Waiting for the next cleanup sample…
			</div>
		{:else}
			<Chart.Container config={chartConfig} class="aspect-auto h-62.5 w-full">
				<AreaChart
					legend
					data={chartData}
					x="date"
					xScale={scaleUtc()}
					xDomain={xDomain}
					series={[
						{
							key: "items",
							label: "Items",
							color: chartConfig.items.color
						},
						{
							key: "serviced",
							label: "Serviced",
							color: chartConfig.serviced.color
						},
						{
							key: "removed",
							label: "Removed",
							color: chartConfig.removed.color
						}
					]}
					props={{
						area: {
							curve: curveNatural,
							"fill-opacity": 0.4,
							line: { class: "stroke-1" },
							motion: "tween"
						},
						xAxis: {
							ticks: selectedRange === "1h" || selectedRange === "3h" ? 6 : selectedRange === "24h" ? 8 : 6,
							format: (v: Date) => {
								if (selectedRange === "1h" || selectedRange === "3h") {
									return v.toLocaleTimeString("en-US", {
										hour: "numeric",
										minute: "2-digit"
									});
								}

								if (selectedRange === "24h") {
									return v.toLocaleString("en-US", {
										month: "short",
										day: "numeric",
										hour: "numeric"
									});
								}

								return v.toLocaleDateString("en-US", {
									month: "short",
									day: "numeric"
								});
							}
						},

						yAxis: { format: () => "" }
					}}
				>
					{#snippet marks({ series, getAreaProps })}
						<defs>
							<linearGradient id="fillItems" x1="0" y1="0" x2="0" y2="1">
								<stop
									offset="5%"
									stop-color="var(--color-items)"
									stop-opacity={1.0}
								/>
								<stop
									offset="95%"
									stop-color="var(--color-items)"
									stop-opacity={0.1}
								/>
							</linearGradient>
							<linearGradient id="fillServiced" x1="0" y1="0" x2="0" y2="1">
								<stop
									offset="5%"
									stop-color="var(--color-serviced)"
									stop-opacity={0.8}
								/>
								<stop
									offset="95%"
									stop-color="var(--color-serviced)"
									stop-opacity={0.1}
								/>
							</linearGradient>
							<linearGradient id="fillRemoved" x1="0" y1="0" x2="0" y2="1">
								<stop
									offset="5%"
									stop-color="var(--color-removed)"
									stop-opacity={0.8}
								/>
								<stop
									offset="95%"
									stop-color="var(--color-removed)"
									stop-opacity={0.1}
								/>
							</linearGradient>
						</defs>
						{#each series as s, i (s.key)}
							<Area
								{...getAreaProps(s, i)}
								fill={s.key === "items"
									? "url(#fillItems)"
									: s.key === "serviced"
										? "url(#fillServiced)"
										: "url(#fillRemoved)"}
							/>
						{/each}
					{/snippet}
					{#snippet tooltip()}
						<Chart.Tooltip
							labelFormatter={(v: Date) => {
								return v.toLocaleString("en-US", {
									month: "short",
									day: "numeric",
									hour: "numeric",
									minute: "2-digit"
								});
							}}
							indicator="line"
						/>
					{/snippet}
				</AreaChart>
			</Chart.Container>
		{/if}
	</Card.Content>
</Card.Root>
