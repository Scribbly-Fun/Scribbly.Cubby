<script lang="ts">
	// @ts-ignore
	import ChartBarIcon from '@tabler/icons-svelte/icons/chart-bar';
	// @ts-ignore
	import DatabaseIcon from '@tabler/icons-svelte/icons/database';
	// @ts-ignore
	import FileAiIcon from '@tabler/icons-svelte/icons/file-ai';
	// @ts-ignore
	import CacheIcon from '@tabler/icons-svelte/icons/device-sd-card';
	// @ts-ignore
	import LocksIcon from '@tabler/icons-svelte/icons/lock-code';
	// @ts-ignore
	import NavDocuments from './nav-documents.svelte';
	import NavMain from './nav-main.svelte';
	import NavUser from './nav-user.svelte';
	import * as Sidebar from '$lib/components/ui/sidebar/index.js';
	import type { ComponentProps } from 'svelte';

	const data = {
		user: {
			name: 'cubby',
			email: 'scribbly@scribbly.com'
		},
		cubbyFeatures: [
			{
				title: 'Dashboard',
				url: '/dashboard',
				icon: ChartBarIcon
			},
			{
				title: 'Caches',
				url: '/caches',
				icon: CacheIcon
			},
			{
				title: 'Locks',
				url: '/locks',
				icon: LocksIcon
			}
		],
		documentation: [
			{
				name: 'Server',
				url: 'https://github.com/Scribbly-Fun/Scribbly.Cubby?tab=readme-ov-file#cubby-host',
				icon: DatabaseIcon
			},
			{
				name: 'Client',
				url: 'https://github.com/Scribbly-Fun/Scribbly.Cubby?tab=readme-ov-file#cubby-client',
				icon: FileAiIcon
			}
		]
	};

	let { ...restProps }: ComponentProps<typeof Sidebar.Root> = $props();

	import banner from '$lib/assets/scribbly_banner.png';
</script>

<Sidebar.Root collapsible="offcanvas" {...restProps}>
	<Sidebar.Header>
		<Sidebar.Menu>
			<Sidebar.MenuItem>
				<Sidebar.MenuButton class="data-[slot=sidebar-menu-button]:p-1.5!">
					{#snippet child({ props })}
						<a href="##" {...props}>
							<img alt="Scribbly" src={banner} width="100" />
						</a>
					{/snippet}
				</Sidebar.MenuButton>
			</Sidebar.MenuItem>
		</Sidebar.Menu>
	</Sidebar.Header>
	<Sidebar.Content>
		<NavMain items={data.cubbyFeatures} />
		<NavDocuments items={data.documentation} />
	</Sidebar.Content>

	<Sidebar.Footer>
		<NavUser user={data.user} />
	</Sidebar.Footer>
</Sidebar.Root>
