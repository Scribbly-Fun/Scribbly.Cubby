<script lang="ts">
	import * as Chart from "$lib/components/ui/chart/index.js";
	import * as Card from "$lib/components/ui/card/index.js";
	import * as Select from "$lib/components/ui/select/index.js";
	import * as ToggleGroup from "$lib/components/ui/toggle-group/index.js";
	import { scaleUtc } from "d3-scale";
	import { Area, AreaChart } from "layerchart";
	import { curveNatural } from "d3-shape";
	import type { CleanupMetrics } from "$lib/api/types";

	let { metrics }: { metrics: CleanupMetrics } = $props();

	const chartData = $derived(
		metrics.points.map((point) => ({
			date: new Date(point.timestamp),
			items: point.items_total,
			serviced: point.items_serviced,
			removed:
				point.removed_tombstone + point.removed_expired + point.removed_sliding
		}))
	);

	let timeRange = $state("3h");

	const selectedLabel = $derived.by(() => {
		switch (timeRange) {
			case "3d":
				return "Last 3 Days";
			case "24h":
				return "Last 24 hours";
			case "3h":
				return "Last 3 Hours";
			default:
				return "Last 3 Hours";
		}
	});

	const filteredData = $derived(
		chartData.filter((item) => {
			const referenceDate = new Date();
			let hoursToSubtract = 3;
			if (timeRange === "24h") {
				hoursToSubtract = 24;
			} else if (timeRange === "3d") {
				hoursToSubtract = 72;
			}

			referenceDate.setHours(referenceDate.getHours() - hoursToSubtract);
			return item.date >= referenceDate;
		})
	);

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
				bind:value={timeRange}
				variant="outline"
				class="hidden *:data-[slot=toggle-group-item]:px-4! @[767px]/card:flex"
			>
				<ToggleGroup.Item value="3h">Last 3 Hours</ToggleGroup.Item>
				<ToggleGroup.Item value="24h">Last 24 hours</ToggleGroup.Item>
				<ToggleGroup.Item value="3d">Last 3 days</ToggleGroup.Item>
			</ToggleGroup.Root>
			<Select.Root type="single" bind:value={timeRange}>
				<Select.Trigger
					size="sm"
					class="flex w-40 **:data-[slot=select-value]:block **:data-[slot=select-value]:truncate @[767px]/card:hidden"
					aria-label="Select a value"
				>
					<span data-slot="select-value">
						{selectedLabel}
					</span>
				</Select.Trigger>
				<Select.Content class="rounded-xl">
					<Select.Item value="3h" class="rounded-lg">Last 3 Hours</Select.Item>
					<Select.Item value="24h" class="rounded-lg">Last 24 hours</Select.Item>
					<Select.Item value="3d" class="rounded-lg">Last 3 days</Select.Item>
				</Select.Content>
			</Select.Root>
		</Card.Action>
	</Card.Header>
	<Card.Content class="px-2 pt-4 sm:px-6 sm:pt-6">
		{#if filteredData.length === 0}
			<div
				class="text-muted-foreground flex h-62.5 items-center justify-center text-sm"
			>
				Waiting for the next cleanup sample…
			</div>
		{:else}
			<Chart.Container config={chartConfig} class="aspect-auto h-62.5 w-full">
				<AreaChart
					legend
					data={filteredData}
					x="date"
					xScale={scaleUtc()}
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
							ticks: timeRange === "3h" ? 6 : undefined,
							format: (v) => {
								return v.toLocaleString("en-US", {
									month: "short",
									day: "numeric",
									hour: "numeric",
									minute: "2-digit"
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
