<script lang="ts">
	import { page } from '$app/state';
	import * as Sidebar from "$lib/components/ui/sidebar/index.js";
	// @ts-ignore
	import type { Icon } from "@tabler/icons-svelte";

	let { items }: { items: { title: string; url: string; icon?: Icon }[] } = $props();
</script>

<Sidebar.Group>
	<Sidebar.GroupContent class="flex flex-col gap-2">
		<Sidebar.GroupLabel>Cubby Cache</Sidebar.GroupLabel>
		<Sidebar.Menu>
			{#each items as item (item.title)}
				<Sidebar.MenuItem >
					<Sidebar.MenuButton 
						tooltipContent={item.title}
						class={page.url.pathname === item.url ? 'bg-accent' : ''}
					>
						
						{#snippet child({ props })}
						<a href={item.url} {...props}>
								{#if item.icon}
									<item.icon />
								{/if}
								<span>{item.title}</span>
							</a>
						{/snippet}
					</Sidebar.MenuButton>
				</Sidebar.MenuItem>
			{/each}
		</Sidebar.Menu>
	</Sidebar.GroupContent>
</Sidebar.Group>
