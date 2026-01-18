import type { ColumnDef } from '@tanstack/table-core';
import type { CacheEntry } from '$lib/api/types/CacheEntry';
import { renderComponent, renderSnippet } from '$lib/components/ui/data-table/render-helpers';
import FlagsBadge from '$lib/components/ui/flags/flags-badge.svelte';
import CacheTableActions from './cache-table-actions.svelte';
import { createRawSnippet } from 'svelte';

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
			const expirationCellSnippet = createRawSnippet<[{ expiration: string | undefined }]>(
				(getExpiration) => {
					return {
						render: () => `<div class=" font-medium">${getExpiration().expiration ?? 'Never'}</div>`
					};
				}
			);

			return renderSnippet(expirationCellSnippet, {
				expiration: row.original.expiration
			});
		}
	},
	{
		accessorKey: 'sliding',
		header: 'Sliding Duration',
		cell: ({ row }) => {
			const expirationCellSnippet = createRawSnippet<[{ sliding_duration: string | undefined }]>(
				(getDuration) => {
					return {
						render: () =>
							`<div class="font-medium">${getDuration().sliding_duration ?? '00:00:00'}</div>`
					};
				}
			);

			return renderSnippet(expirationCellSnippet, {
				sliding_duration: row.original.sliding_duration
			});
		}
	},
	{
		accessorKey: 'encoding',
		header: 'Encoding'
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
