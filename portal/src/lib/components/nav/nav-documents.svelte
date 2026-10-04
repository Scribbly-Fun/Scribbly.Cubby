<script lang="ts">
	// @ts-ignore
	import CopyIcon from '@tabler/icons-svelte/icons/copy';
	// @ts-ignore
	import DotsIcon from '@tabler/icons-svelte/icons/dots';
	// @ts-ignore
	import ExternalLinkIcon from '@tabler/icons-svelte/icons/external-link';
	// @ts-ignore
	import type { Icon } from '@tabler/icons-svelte';

	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import * as Sidebar from '$lib/components/ui/sidebar/index.js';

	let { items }: { items: { name: string; url: string; icon: Icon }[] } = $props();

	const sidebar = Sidebar.useSidebar();

	async function copyLink(url: string) {
		try {
			await navigator.clipboard.writeText(url);
		} catch {
			// Fallback for environments without clipboard permissions.
			const input = document.createElement('input');
			input.value = url;
			document.body.appendChild(input);
			input.select();
			document.execCommand('copy');
			document.body.removeChild(input);
		}
	}

	function openInNewTab(url: string) {
		window.open(url, '_blank', 'noopener,noreferrer');
	}
</script>

<Sidebar.Group class="group-data-[collapsible=icon]:hidden">
	<Sidebar.GroupLabel>Documentation</Sidebar.GroupLabel>
	<Sidebar.Menu>
		{#each items as item (item.name)}
			<Sidebar.MenuItem>
				<Sidebar.MenuButton>
					{#snippet child({ props })}
						<a {...props} href={item.url}>
							<item.icon />
							<span>{item.name}</span>
						</a>
					{/snippet}
				</Sidebar.MenuButton>
				<DropdownMenu.Root>
					<DropdownMenu.Trigger>
						{#snippet child({ props })}
							<Sidebar.MenuAction
								{...props}
								showOnHover
								class="rounded-sm data-[state=open]:bg-accent"
							>
								<DotsIcon />
								<span class="sr-only">More</span>
							</Sidebar.MenuAction>
						{/snippet}
					</DropdownMenu.Trigger>
					<DropdownMenu.Content
						class="w-28 rounded-lg"
						side={sidebar.isMobile ? 'bottom' : 'right'}
						align={sidebar.isMobile ? 'end' : 'start'}
					>
						<DropdownMenu.Item
							onclick={() => {
								void copyLink(item.url);
							}}
						>
							<CopyIcon />
							<span>Copy</span>
						</DropdownMenu.Item>
						<DropdownMenu.Item
							onclick={() => {
								openInNewTab(item.url);
							}}
						>
							<ExternalLinkIcon />
							<span>Open</span>
						</DropdownMenu.Item>
					</DropdownMenu.Content>
				</DropdownMenu.Root>
			</Sidebar.MenuItem>
		{/each}
	</Sidebar.Menu>
</Sidebar.Group>
