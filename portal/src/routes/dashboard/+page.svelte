<script lang="ts">
	import ChartAreaInteractive from './components/reads-chart.svelte';
	import type { PageProps } from './$types';
	import CubbyOptions from './components/option-cards.svelte';
	import { invalidateAll } from '$app/navigation';

	const POLL_INTERVAL_MS = 30_000;

	let { data }: PageProps = $props();

	$effect(() => {
		const id = setInterval(() => {
			void invalidateAll();
		}, POLL_INTERVAL_MS);

		return () => clearInterval(id);
	});
</script>

<CubbyOptions cubby_options={data.cubby_options} />

<div class="px-4 lg:px-6">
	<ChartAreaInteractive metrics={data.cleanup_metrics} range={data.metrics_range} />
</div>
