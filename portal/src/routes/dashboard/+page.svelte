<script lang="ts">
	import ChartAreaInteractive from './components/reads-chart.svelte';
	import type { PageProps } from './$types';
	import CubbyOptions from './components/option-cards.svelte';

	let { data }: PageProps = $props();
</script>

<CubbyOptions cubby_options={data.cubby_options} />

<div class="px-4 lg:px-6">
	<ChartAreaInteractive />
</div>
