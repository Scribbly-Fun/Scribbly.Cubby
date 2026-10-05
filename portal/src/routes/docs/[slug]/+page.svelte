<script lang="ts">
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();

	const embedSrc = $derived(`/docs/embed/${data.doc.slug}#${data.doc.anchor}`);
</script>

<div class="-my-4 flex min-h-[calc(100dvh-var(--header-height)-2rem)] flex-1 flex-col md:-my-6">
	<iframe
		title={`${data.doc.name} documentation`}
		src={embedSrc}
		class="h-full min-h-[calc(100dvh-var(--header-height)-2rem)] w-full flex-1 border-0 bg-background"
	></iframe>
</div>
