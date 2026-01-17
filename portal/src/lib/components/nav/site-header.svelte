<script lang="ts">
	import { toggleMode } from 'mode-watcher';
	import GithubIcon from '@tabler/icons-svelte/icons/brand-github';
	import DarkMode from '@tabler/icons-svelte/icons/brightness';
	import LightMode from '@tabler/icons-svelte/icons/brightness-up';
	import { Button } from '$lib/components/ui/button/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import * as Sidebar from '$lib/components/ui/sidebar/index.js';
</script>

<header
	class="flex h-(--header-height) shrink-0 items-center gap-2 border-b transition-[width,height] ease-linear group-has-data-[collapsible=icon]/sidebar-wrapper:h-(--header-height)"
>
	<div class="flex w-full items-center gap-1 px-4 lg:gap-2 lg:px-6">
		<Sidebar.Trigger class="-ms-1" />
		<Separator orientation="vertical" class="mx-2 data-[orientation=vertical]:h-4" />
		<h1 class="text-base font-medium">Documentation</h1>
		<div class="ms-auto flex items-center gap-2">
			<Button
				onclick={toggleMode}
				variant="ghost"
				size="sm"
				class="hidden sm:flex dark:text-foreground"
				target="_blank"
				rel="noopener noreferrer"
			>
				<LightMode
					class="h-[1.2rem] w-[1.2rem] scale-100 rotate-0 transition-all! dark:scale-0 dark:-rotate-90"
				/>
				<DarkMode
					class="absolute h-[1.2rem] w-[1.2rem] scale-0 rotate-90 transition-all! dark:scale-100 dark:rotate-0"
				/>
			</Button>
			<Button
				href="https://github.com/Scribbly-Fun/Scribbly.Cubby"
				variant="ghost"
				size="sm"
				class="hidden sm:flex dark:text-foreground"
				target="_blank"
				rel="noopener noreferrer"
			>
				<GithubIcon />
			</Button>
		</div>
	</div>
</header>
