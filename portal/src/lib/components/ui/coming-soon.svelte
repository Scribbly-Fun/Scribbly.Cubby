<script lang="ts">
	import { Button } from '$lib/components/ui/button/index.js';
	// @ts-ignore
	import GithubIcon from '@tabler/icons-svelte/icons/brand-github';

	let {
		title = 'Coming Soon',
		description = 'This feature is not available yet.',
		issueUrl = undefined,
		issueLabel = 'Track progress on GitHub'
	}: {
		title?: string;
		description?: string;
		issueUrl?: string;
		issueLabel?: string;
	} = $props();
</script>

<div class="flex flex-1 flex-col items-center justify-center gap-4 px-4 py-16 text-center">
	<h2 class="text-2xl font-semibold tracking-tight">{title}</h2>
	<p class="max-w-md text-muted-foreground">{description}</p>
	{#if issueUrl}
		<Button
			href={issueUrl}
			variant="outline"
			target="_blank"
			rel="noopener noreferrer"
		>
			<GithubIcon />
			{issueLabel}
		</Button>
	{/if}
</div>
