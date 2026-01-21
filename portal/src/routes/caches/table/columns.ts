import type { ColumnDef } from '@tanstack/table-core';
import type { CacheEntry } from '$lib/api/types/CacheEntry';
import { renderComponent, renderSnippet } from '$lib/components/ui/data-table/render-helpers';
import FlagsBadge from '$lib/components/ui/flags/flags-badge.svelte';
import CacheTableActions from './cache-table-actions.svelte';
import { createRawSnippet } from 'svelte';
import SlidingBadge from '$lib/components/ui/flags/sliding-badge.svelte';
import ExpirationBadge from '$lib/components/ui/flags/expiration-badge.svelte';
import EncodingBadge from '$lib/components/ui/flags/encoding-badge.svelte';
import CacheEntryDisplay from '../components/cache-entry-display.svelte';

function renderHeader(title: string) {
	const amountHeaderSnippet = createRawSnippet(() => ({
		render: () => `<div class="text-muted-foreground text-xs ">${title}</div>`
	}));
	return renderSnippet(amountHeaderSnippet);
}
export const columns: ColumnDef<CacheEntry>[] = [
	{
		accessorKey: 'key',
		header: () => renderHeader('Key')
	},
	{
		accessorKey: 'flags',
		header: () => renderHeader('Flags'),
		cell: ({ row }) => renderComponent(FlagsBadge, { flags: row.original.flags })
	},
	{
		accessorKey: 'expiration',
		header: () => renderHeader('Expiration'),
		cell: ({ row }) => {
			return renderComponent(ExpirationBadge, {
				date: row.original.expiration ? new Date(row.original.expiration) : undefined
			});
		}
	},
	{
		accessorKey: 'sliding',
		header: () => renderHeader('Sliding Duration'),
		cell: ({ row }) => {
			return renderComponent(SlidingBadge, { duration: row.original.sliding_duration });
		}
	},
	{
		accessorKey: 'encoding',
		header: () => renderHeader('Encoding'),
		cell: ({ row }) => {
			return renderComponent(EncodingBadge, {
				encoding: row.original.encoding
			});
		}
	},
	{
		accessorKey: 'size',
		header: () => renderHeader('Size'),
		cell: ({ row }) => {
			return renderComponent(CacheEntryDisplay, { entry: row.original });
		}
	},
	{
		id: 'actions',
		cell: ({ row }) => {
			return renderComponent(CacheTableActions, { entry: row.original });
		}
	}
];
