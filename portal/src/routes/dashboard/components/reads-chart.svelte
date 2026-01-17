<script lang="ts">
	import * as Chart from "$lib/components/ui/chart/index.js";
	import * as Card from "$lib/components/ui/card/index.js";
	import * as Select from "$lib/components/ui/select/index.js";
	import * as ToggleGroup from "$lib/components/ui/toggle-group/index.js";
	import { scaleUtc } from "d3-scale";
	import { Area, AreaChart } from "layerchart";
	import { curveNatural } from "d3-shape";

	const chartData = [
		{ date: new Date("2024-04-01"), reads: 222, writes: 150 },
		{ date: new Date("2024-04-02"), reads: 97, writes: 180 },
		{ date: new Date("2024-04-03"), reads: 167, writes: 120 },
		{ date: new Date("2024-04-04"), reads: 242, writes: 260 },
		{ date: new Date("2024-04-05"), reads: 373, writes: 290 },
		{ date: new Date("2024-04-06"), reads: 301, writes: 340 },
		{ date: new Date("2024-04-07"), reads: 245, writes: 180 },
		{ date: new Date("2024-04-08"), reads: 409, writes: 320 },
		{ date: new Date("2024-04-09"), reads: 59, writes: 110 },
		{ date: new Date("2024-04-10"), reads: 261, writes: 190 },
		{ date: new Date("2024-04-11"), reads: 327, writes: 350 },
		{ date: new Date("2024-04-12"), reads: 292, writes: 210 },
		{ date: new Date("2024-04-13"), reads: 342, writes: 380 },
		{ date: new Date("2024-04-14"), reads: 137, writes: 220 },
		{ date: new Date("2024-04-15"), reads: 120, writes: 170 },
		{ date: new Date("2024-04-16"), reads: 138, writes: 190 },
		{ date: new Date("2024-04-17"), reads: 446, writes: 360 },
		{ date: new Date("2024-04-18"), reads: 364, writes: 410 },
		{ date: new Date("2024-04-19"), reads: 243, writes: 180 },
		{ date: new Date("2024-04-20"), reads: 89, writes: 150 },
		{ date: new Date("2024-04-21"), reads: 137, writes: 200 },
		{ date: new Date("2024-04-22"), reads: 224, writes: 170 },
		{ date: new Date("2024-04-23"), reads: 138, writes: 230 },
		{ date: new Date("2024-04-24"), reads: 387, writes: 290 },
		{ date: new Date("2024-04-25"), reads: 215, writes: 250 },
		{ date: new Date("2024-04-26"), reads: 75, writes: 130 },
		{ date: new Date("2024-04-27"), reads: 383, writes: 420 },
		{ date: new Date("2024-04-28"), reads: 122, writes: 180 },
		{ date: new Date("2024-04-29"), reads: 315, writes: 240 },
		{ date: new Date("2024-04-30"), reads: 454, writes: 380 },
		{ date: new Date("2024-05-01"), reads: 165, writes: 220 },
		{ date: new Date("2024-05-02"), reads: 293, writes: 310 },
		{ date: new Date("2024-05-03"), reads: 247, writes: 190 },
		{ date: new Date("2024-05-04"), reads: 385, writes: 420 },
		{ date: new Date("2024-05-05"), reads: 481, writes: 390 },
		{ date: new Date("2024-05-06"), reads: 498, writes: 520 },
		{ date: new Date("2024-05-07"), reads: 388, writes: 300 },
		{ date: new Date("2024-05-08"), reads: 149, writes: 210 },
		{ date: new Date("2024-05-09"), reads: 227, writes: 180 },
		{ date: new Date("2024-05-10"), reads: 293, writes: 330 },
		{ date: new Date("2024-05-11"), reads: 335, writes: 270 },
		{ date: new Date("2024-05-12"), reads: 197, writes: 240 },
		{ date: new Date("2024-05-13"), reads: 197, writes: 160 },
		{ date: new Date("2024-05-14"), reads: 448, writes: 490 },
		{ date: new Date("2024-05-15"), reads: 473, writes: 380 },
		{ date: new Date("2024-05-16"), reads: 338, writes: 400 },
		{ date: new Date("2024-05-17"), reads: 499, writes: 420 },
		{ date: new Date("2024-05-18"), reads: 315, writes: 350 },
		{ date: new Date("2024-05-19"), reads: 235, writes: 180 },
		{ date: new Date("2024-05-20"), reads: 177, writes: 230 },
		{ date: new Date("2024-05-21"), reads: 82, writes: 140 },
		{ date: new Date("2024-05-22"), reads: 81, writes: 120 },
		{ date: new Date("2024-05-23"), reads: 252, writes: 290 },
		{ date: new Date("2024-05-24"), reads: 294, writes: 220 },
		{ date: new Date("2024-05-25"), reads: 201, writes: 250 },
		{ date: new Date("2024-05-26"), reads: 213, writes: 170 },
		{ date: new Date("2024-05-27"), reads: 420, writes: 460 },
		{ date: new Date("2024-05-28"), reads: 233, writes: 190 },
		{ date: new Date("2024-05-29"), reads: 78, writes: 130 },
		{ date: new Date("2024-05-30"), reads: 340, writes: 280 },
		{ date: new Date("2024-05-31"), reads: 178, writes: 230 },
		{ date: new Date("2024-06-01"), reads: 178, writes: 200 },
		{ date: new Date("2024-06-02"), reads: 470, writes: 410 },
		{ date: new Date("2024-06-03"), reads: 103, writes: 160 },
		{ date: new Date("2024-06-04"), reads: 439, writes: 380 },
		{ date: new Date("2024-06-05"), reads: 88, writes: 140 },
		{ date: new Date("2024-06-06"), reads: 294, writes: 250 },
		{ date: new Date("2024-06-07"), reads: 323, writes: 370 },
		{ date: new Date("2024-06-08"), reads: 385, writes: 320 },
		{ date: new Date("2024-06-09"), reads: 438, writes: 480 },
		{ date: new Date("2024-06-10"), reads: 155, writes: 200 },
		{ date: new Date("2024-06-11"), reads: 92, writes: 150 },
		{ date: new Date("2024-06-12"), reads: 492, writes: 420 },
		{ date: new Date("2024-06-13"), reads: 81, writes: 130 },
		{ date: new Date("2024-06-14"), reads: 426, writes: 380 },
		{ date: new Date("2024-06-15"), reads: 307, writes: 350 },
		{ date: new Date("2024-06-16"), reads: 371, writes: 310 },
		{ date: new Date("2024-06-17"), reads: 475, writes: 520 },
		{ date: new Date("2024-06-18"), reads: 107, writes: 170 },
		{ date: new Date("2024-06-19"), reads: 341, writes: 290 },
		{ date: new Date("2024-06-20"), reads: 408, writes: 450 },
		{ date: new Date("2024-06-21"), reads: 169, writes: 210 },
		{ date: new Date("2024-06-22"), reads: 317, writes: 270 },
		{ date: new Date("2024-06-23"), reads: 480, writes: 530 },
		{ date: new Date("2024-06-24"), reads: 132, writes: 180 },
		{ date: new Date("2024-06-25"), reads: 141, writes: 190 },
		{ date: new Date("2024-06-26"), reads: 434, writes: 380 },
		{ date: new Date("2024-06-27"), reads: 448, writes: 490 },
		{ date: new Date("2024-06-28"), reads: 149, writes: 200 },
		{ date: new Date("2024-06-29"), reads: 103, writes: 160 },
		{ date: new Date("2024-06-30"), reads: 446, writes: 400 },
	];

	let timeRange = $state("3d");

	const selectedLabel = $derived.by(() => {
		switch (timeRange) {
			case "3d":
				return "Last 3 Days";
			case "1d":
				return "Last 24 hours";
			case "7d":
				return "Last 3 Hours";
			default:
				return "Last 3 days";
		}
	});

	const filteredData = $derived(
		chartData.filter((item) => {
			// eslint-disable-next-line svelte/prefer-svelte-reactivity
			const referenceDate = new Date("2024-06-30");
			let daysToSubtract = 90;
			if (timeRange === "1d") {
				daysToSubtract = 30;
			} else if (timeRange === "7d") {
				daysToSubtract = 7;
			}

			referenceDate.setDate(referenceDate.getDate() - daysToSubtract);
			return item.date >= referenceDate;
		})
	);

	const chartConfig = {
		reads: { label: "Reads", color: "var(--primary)" },
		writes: { label: "Writes", color: "var(--warning)" },
	} satisfies Chart.ChartConfig;
</script>

<Card.Root class="@container/card">
	<Card.Header>
		<Card.Title>Reads VS Writes</Card.Title>
		<Card.Description>
			<span class="hidden @[540px]/card:block"> Total for the last 3 days </span>
			<span class="@[540px]/card:hidden">Last 3 days</span>
		</Card.Description>
		<Card.Action>
			<ToggleGroup.Root
				type="single"
				bind:value={timeRange}
				variant="outline"
				class="hidden *:data-[slot=toggle-group-item]:px-4! @[767px]/card:flex"
			>
				<ToggleGroup.Item value="3d">Last 3 days</ToggleGroup.Item>
				<ToggleGroup.Item value="1d">Last 24 hours</ToggleGroup.Item>
				<ToggleGroup.Item value="7d">Last 3 Hours</ToggleGroup.Item>
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
					<Select.Item value="3d" class="rounded-lg">Last 3 days</Select.Item>
					<Select.Item value="1d" class="rounded-lg">Last 24 hours</Select.Item>
					<Select.Item value="7d" class="rounded-lg">Last 3 Hours</Select.Item>
				</Select.Content>
			</Select.Root>
		</Card.Action>
	</Card.Header>
	<Card.Content class="px-2 pt-4 sm:px-6 sm:pt-6">
		<Chart.Container config={chartConfig} class="aspect-auto h-62.5 w-full">
			<AreaChart
				legend
				data={filteredData}
				x="date"
				xScale={scaleUtc()}
				series={[
					{
						key: "reads",
						label: "Reads",
						color: chartConfig.reads.color,
					},
					{
						key: "writes",
						label: "Writes",
						color: chartConfig.writes.color,
					},
				]}
				seriesLayout="stack"
				props={{
					area: {
						curve: curveNatural,
						"fill-opacity": 0.4,
						line: { class: "stroke-1" },
						motion: "tween",
					},
					xAxis: {
						ticks: timeRange === "7d" ? 7 : undefined,
						format: (v) => {
							return v.toLocaleDateString("en-US", {
								month: "short",
								day: "numeric",
							});
						},
					},

					yAxis: { format: () => "" },
				}}
			>
				{#snippet marks({ series, getAreaProps })}
					<defs>
						<linearGradient id="fillDesktop" x1="0" y1="0" x2="0" y2="1">
							<stop
								offset="5%"
								stop-color="var(--color-reads)"
								stop-opacity={1.0}
							/>
							<stop
								offset="95%"
								stop-color="var(--color-reads)"
								stop-opacity={0.1}
							/>
						</linearGradient>
						<linearGradient id="fillMobile" x1="0" y1="0" x2="0" y2="1">
							<stop offset="5%" stop-color="var(--color-writes)" stop-opacity={0.8} />
							<stop
								offset="95%"
								stop-color="var(--color-writes)"
								stop-opacity={0.1}
							/>
						</linearGradient>
					</defs>
					{#each series as s, i (s.key)}
						<Area
							{...getAreaProps(s, i)}
							fill={s.key === "reads" ? "url(#fillDesktop)" : "url(#fillMobile)"}
						/>
					{/each}
				{/snippet}
				{#snippet tooltip()}
					<Chart.Tooltip
						labelFormatter={(v: Date) => {
							return v.toLocaleDateString("en-US", {
								month: "short",
								day: "numeric",
							});
						}}
						indicator="line"
					/>
				{/snippet}
			</AreaChart>
		</Chart.Container>
	</Card.Content>
</Card.Root>
