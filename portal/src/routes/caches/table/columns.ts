import type { ColumnDef } from '@tanstack/table-core';
import type { CacheEntry } from '$lib/api/types/CacheEntry';
import { renderComponent, renderSnippet } from '$lib/components/ui/data-table/render-helpers';
import FlagsBadge from '$lib/components/ui/flags/flags-badge.svelte';
import CacheTableActions from './cache-table-actions.svelte';
import { createRawSnippet } from 'svelte';
import SlidingBadge from '$lib/components/ui/flags/sliding-badge.svelte';
import ExpirationBadge from '$lib/components/ui/flags/expiration-badge.svelte';
import EncodingBadge from '$lib/components/ui/flags/encoding-badge.svelte';

export const columns: ColumnDef<CacheEntry>[] = [
	{
		accessorKey: 'key',
		header: 'Key'
	},
	{
		accessorKey: 'flags',
		header: 'Flags',
		cell: ({ row }) => renderComponent(FlagsBadge, { flags: row.original.flags })
	},
	{
		accessorKey: 'expiration',
		header: 'Expiration',
		cell: ({ row }) => {
			return renderComponent(ExpirationBadge, {
				date: row.original.expiration ? new Date(row.original.expiration) : undefined
			});
		}
	},
	{
		accessorKey: 'sliding',
		header: 'Sliding Duration',
		cell: ({ row }) => {
			return renderComponent(SlidingBadge, { duration: row.original.sliding_duration });
		}
	},
	{
		accessorKey: 'encoding',
		header: 'Encoding',
		cell: ({ row }) => {
			return renderComponent(EncodingBadge, {
				encoding: row.original.encoding
			});
		}
	},
	{
		accessorKey: 'size',
		header: 'Size'
	},
	{
		id: 'actions',

		cell: ({ row }) => {
			return renderComponent(CacheTableActions, { entry: row.original });
		}
	}
];
