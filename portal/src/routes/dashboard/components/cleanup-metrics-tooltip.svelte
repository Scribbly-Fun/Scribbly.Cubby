<script lang="ts">
	import { getPayloadConfigFromPayload, useChart } from "$lib/components/ui/chart/chart-utils.js";
	import { getTooltipContext, Tooltip as TooltipPrimitive } from "layerchart";

	let {
		labelFormatter
	}: {
		labelFormatter?: (value: Date) => string;
	} = $props();

	const chart = useChart();
	const tooltipCtx = getTooltipContext();

	const point = $derived(
		tooltipCtx.payload?.[0]?.payload as
			| { date?: Date; durationMs?: number }
			| undefined
	);

	const formattedLabel = $derived.by(() => {
		const date = point?.date;
		if (!(date instanceof Date)) {
			return null;
		}

		return labelFormatter?.(date) ?? date.toLocaleString();
	});

	const durationLabel = $derived.by(() => {
		const durationMs = point?.durationMs;
		if (typeof durationMs !== "number" || Number.isNaN(durationMs)) {
			return null;
		}

		if (durationMs < 1) {
			return `${durationMs.toFixed(3)} ms`;
		}

		if (durationMs < 1000) {
			return `${durationMs.toFixed(2)} ms`;
		}

		return `${(durationMs / 1000).toFixed(2)} s`;
	});
</script>

<TooltipPrimitive.Root variant="none">
	<div
		class="border-border/50 bg-background grid min-w-[9rem] items-start gap-1.5 rounded-lg border px-2.5 py-1.5 text-xs shadow-xl"
	>
		{#if formattedLabel}
			<div class="font-medium">{formattedLabel}</div>
		{/if}
		<div class="grid gap-1.5">
			{#each tooltipCtx.payload as item, i (item.key + i)}
				{@const key = `${item.key || item.name || "value"}`}
				{@const itemConfig = getPayloadConfigFromPayload(chart.config, item, key)}
				{@const indicatorColor = item.payload?.color || item.color}
				<div class="[&>svg]:text-muted-foreground flex w-full flex-wrap items-stretch gap-2 [&>svg]:size-2.5">
					<div
						style="--color-bg: {indicatorColor}; --color-border: {indicatorColor};"
						class="h-full w-1 shrink-0 rounded-[2px] border-(--color-border) bg-(--color-bg)"
					></div>
					<div class="flex flex-1 shrink-0 items-center justify-between leading-none">
						<span class="text-muted-foreground">
							{itemConfig?.label || item.name}
						</span>
						{#if item.value !== undefined}
							<span class="text-foreground font-mono font-medium tabular-nums">
								{item.value.toLocaleString()}
							</span>
						{/if}
					</div>
				</div>
			{/each}
			{#if durationLabel}
				<div class="border-border/50 mt-0.5 flex w-full items-center justify-between border-t pt-1.5 leading-none">
					<span class="text-muted-foreground">Duration</span>
					<span class="text-foreground font-mono font-medium tabular-nums">
						{durationLabel}
					</span>
				</div>
			{/if}
		</div>
	</div>
</TooltipPrimitive.Root>
