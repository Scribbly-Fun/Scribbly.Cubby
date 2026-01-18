<script lang="ts">
	import EllipsisIcon from '@lucide/svelte/icons/ellipsis';
	import { Button } from '$lib/components/ui/button/index.js';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu/index.js';
	import type { CacheEntry } from '$lib/api/types';
	import { invalidateAll } from '$app/navigation';

	// @ts-ignore
	import TrashIcon from '@tabler/icons-svelte/icons/trash';

	// @ts-ignore
	import EditIcon from '@tabler/icons-svelte/icons/edit';

	// @ts-ignore
	import CoffinIcon from '@tabler/icons-svelte/icons/coffin';

	let { entry }: { entry: CacheEntry } = $props();

	async function handleEvict() {
		const formData = new FormData();
		formData.append('key', entry.key);

		const response = await fetch('?/evict', {
			method: 'POST',
			body: formData
		});

		if (response.ok) {
			console.log(`Entry ${entry.key} evicted successfully`);
			await invalidateAll();
		} else {
			console.error('Failed to evict entry');
		}
	}

	async function handleTombstone() {
		const formData = new FormData();
		formData.append('key', entry.key);

		const response = await fetch('?/tombstone', {
			method: 'POST',
			body: formData
		});

		if (response.ok) {
			console.log(`Entry ${entry.key} tombstoned successfully`);
			await invalidateAll();
		} else {
			console.error('Failed to tombstone entry');
		}
	}
</script>

<div class="flex flex-row justify-end">
	<DropdownMenu.Root>
		<DropdownMenu.Trigger>
			{#snippet child({ props })}
				<Button {...props} variant="ghost" size="icon" class="relative size-8 p-0">
					<span class="sr-only">Open menu</span>
					<EllipsisIcon />
				</Button>
			{/snippet}
		</DropdownMenu.Trigger>
		<DropdownMenu.Content>
			<DropdownMenu.Group>
				<DropdownMenu.Label>Modify</DropdownMenu.Label>
				<DropdownMenu.Item><TrashIcon />Edit</DropdownMenu.Item>
			</DropdownMenu.Group>
			<DropdownMenu.Group>
				<DropdownMenu.Label>Danger</DropdownMenu.Label>
				<DropdownMenu.Item variant="destructive" onclick={handleEvict}
					><TrashIcon />Evict</DropdownMenu.Item
				>
				<DropdownMenu.Item variant="destructive" onclick={handleTombstone}
					><CoffinIcon /> Tombstone</DropdownMenu.Item
				>
			</DropdownMenu.Group>
		</DropdownMenu.Content>
	</DropdownMenu.Root>
</div>
