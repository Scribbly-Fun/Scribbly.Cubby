<script lang="ts">
	import ChartBarIcon from "@tabler/icons-svelte/icons/chart-bar";
	import DatabaseIcon from "@tabler/icons-svelte/icons/database";
	import FileAiIcon from "@tabler/icons-svelte/icons/file-ai";
	import CacheIcon from "@tabler/icons-svelte/icons/device-sd-card";
	import LocksIcon from "@tabler/icons-svelte/icons/lock-code";
	import SettingsIcon from "@tabler/icons-svelte/icons/settings";
	import NavDocuments from "./nav-documents.svelte";
	import NavMain from "./nav-main.svelte";
	import NavSecondary from "./nav-secondary.svelte";
	import NavUser from "./nav-user.svelte";
	import * as Sidebar from "$lib/components/ui/sidebar/index.js";
	import type { ComponentProps } from "svelte";

	const data = {
		user: {
			name: "shadcn",
			email: "m@example.com",
			avatar: "/avatars/shadcn.jpg",
		},
		cubbyFeatures: [
			{
				title: "Dashboard",
				url: "/dashboard",
				icon: ChartBarIcon,
			},
			{
				title: "Caches",
				url: "/caches",
				icon: CacheIcon,
			},
			{
				title: "Locks",
				url: "/locks",
				icon: LocksIcon,
			}
		],
		documentation: [
			{
				name: "Server",
				url: "https://github.com/Scribbly-Fun/Scribbly.Cubby?tab=readme-ov-file#cubby-host",
				icon: DatabaseIcon,
			},
			{
				name: "Client",
				url: "https://github.com/Scribbly-Fun/Scribbly.Cubby?tab=readme-ov-file#cubby-client",
				icon: FileAiIcon,
			}
		],
		navSecondary: [
			{
				title: "Settings",
				url: "/settings",
				icon: SettingsIcon,
			}
		],
	};

	let { ...restProps }: ComponentProps<typeof Sidebar.Root> = $props();

	import logo from '$lib/assets/scribbly_banner.png';
</script>

<Sidebar.Root collapsible="offcanvas" {...restProps}>
	<Sidebar.Header>
		<Sidebar.Menu>
			<Sidebar.MenuItem>
				<Sidebar.MenuButton class="data-[slot=sidebar-menu-button]:p-1.5!">
					{#snippet child({ props })}
						<a href="##" {...props}>
							<img alt="Scribbly" src={logo} width="100" />
						</a>
					{/snippet}
				</Sidebar.MenuButton>
			</Sidebar.MenuItem>
		</Sidebar.Menu>
	</Sidebar.Header>
	<Sidebar.Content>
		<NavMain items={data.cubbyFeatures} />
		<NavDocuments items={data.documentation} />
		<NavSecondary items={data.navSecondary} class="mt-auto" />
	</Sidebar.Content>
	<Sidebar.Footer>
		<NavUser user={data.user} />
	</Sidebar.Footer>
</Sidebar.Root>
