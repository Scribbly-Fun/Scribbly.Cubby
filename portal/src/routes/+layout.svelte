<script lang="ts">
	import './layout.css';
	import { ModeWatcher } from "mode-watcher";
	import favicon from '$lib/assets/favicon.ico';
	
	import * as Sidebar from "$lib/components/ui/sidebar";
	import AppSidebar from "$lib/components/nav/app-sidebar.svelte";
	import SiteHeader from '$lib/components/nav/site-header.svelte';

	let { children } = $props();
</script>

<svelte:head><link rel="icon" href={favicon} /></svelte:head>
<ModeWatcher />
<Sidebar.Provider
	style="--sidebar-width: calc(var(--spacing) * 72); --header-height: calc(var(--spacing) * 12);"
>
	<AppSidebar variant="inset" />
	<Sidebar.Inset>
		<SiteHeader/>
		<div class="flex flex-1 flex-col">
			<div class="@container/main flex flex-1 flex-col gap-2">
				<div class="flex flex-col gap-4 py-4 md:gap-6 md:py-6">
					{@render children()}
				</div>
			</div>
		</div>
	</Sidebar.Inset>
</Sidebar.Provider>
