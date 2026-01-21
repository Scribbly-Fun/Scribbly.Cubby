<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import * as Dialog from '$lib/components/ui/dialog/index.js';

	// @ts-ignore
	import AddIcon from '@tabler/icons-svelte/icons/tag-plus';
</script>

<Dialog.Root>
	<Dialog.Trigger><Button variant="outline"><AddIcon /> Create Entry</Button></Dialog.Trigger>
	<Dialog.Content>
		<Dialog.Header>
			<Dialog.Title>Create New Cache Entry</Dialog.Title>
			<Dialog.Description>
				Complete the required fields to create a new cache entry.
			</Dialog.Description>
		</Dialog.Header>
	</Dialog.Content>
</Dialog.Root>
