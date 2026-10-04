<script lang="ts">
	import CacheTable from './table/cache-table.svelte';
	import type { PageData } from './$types';

	let { data }: { data: PageData } = $props();

	$effect(() => {
		console.log('Cache Entries:', data.entries);
	});
</script>

<div class="px-4 lg:px-6">
	<CacheTable data={data.entries} />
</div>
