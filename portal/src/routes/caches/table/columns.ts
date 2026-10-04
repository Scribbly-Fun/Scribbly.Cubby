import type { ColumnDef } from '@tanstack/table-core';
import type { CacheEntry } from '$lib/api/types/CacheEntry';
import type { SlidingDuration } from '$lib/api/types/SlidingDuration';
import { renderComponent, renderSnippet } from '$lib/components/ui/data-table/render-helpers';
import FlagsBadge from '$lib/components/ui/flags/flags-badge.svelte';
import CacheTableActions from './cache-table-actions.svelte';
import { createRawSnippet } from 'svelte';
import SlidingBadge from '$lib/components/ui/flags/sliding-badge.svelte';
import ExpirationBadge from '$lib/components/ui/flags/expiration-badge.svelte';
import EncodingBadge from '$lib/components/ui/flags/encoding-badge.svelte';
import CacheEntryDisplay from '../components/cache-entry-display.svelte';
import SortableHeader from './sortable-header.svelte';

function renderHeader(title: string) {
	const amountHeaderSnippet = createRawSnippet(() => ({
		render: () => `<div class="text-muted-foreground text-xs ">${title}</div>`
	}));
	return renderSnippet(amountHeaderSnippet);
}

function expirationSortValue(expiration: string | undefined): number {
	if (!expiration) {
		return Number.POSITIVE_INFINITY;
	}

	const time = Date.parse(expiration);
	return Number.isNaN(time) ? Number.POSITIVE_INFINITY : time;
}

function slidingSortValue(duration: SlidingDuration | undefined): number {
	if (!duration) {
		return Number.NEGATIVE_INFINITY;
	}

	const match = /^(?:(\d+)\.)?(\d+):(\d{2}):(\d{2})(?:\.\d+)?$/.exec(duration.trim());
	if (!match) {
		return Number.NEGATIVE_INFINITY;
	}

	const days = Number(match[1] ?? 0);
	const hours = Number(match[2]);
	const minutes = Number(match[3]);
	const seconds = Number(match[4]);
	return days * 86400 + hours * 3600 + minutes * 60 + seconds;
}

function getSortValue(row: CacheEntry, columnId: string): string | number {
	switch (columnId) {
		case 'key':
			return row.key;
		case 'flags':
			return row.flags;
		case 'expiration':
			return expirationSortValue(row.expiration);
		case 'sliding':
			return slidingSortValue(row.sliding_duration);
		case 'encoding':
			return row.encoding;
		case 'size':
			return row.size;
		default:
			return '';
	}
}

export function compareCacheEntries(a: CacheEntry, b: CacheEntry, columnId: string): number {
	const left = getSortValue(a, columnId);
	const right = getSortValue(b, columnId);

	if (typeof left === 'number' && typeof right === 'number') {
		return left - right;
	}

	return String(left).localeCompare(String(right));
}

export type CacheTableMeta = {
	sorting: import('@tanstack/table-core').SortingState;
	toggleSort: (columnId: string) => void;
};

export const columns: ColumnDef<CacheEntry>[] = [
	{
		accessorKey: 'key',
		header: ({ column, table }) =>
			renderComponent(SortableHeader, { column, table, title: 'Key' })
	},
	{
		accessorKey: 'flags',
		header: ({ column, table }) =>
			renderComponent(SortableHeader, { column, table, title: 'Flags' }),
		cell: ({ row }) => renderComponent(FlagsBadge, { flags: row.original.flags })
	},
	{
		id: 'expiration',
		accessorFn: (row) => expirationSortValue(row.expiration),
		header: ({ column, table }) =>
			renderComponent(SortableHeader, { column, table, title: 'Expiration' }),
		cell: ({ row }) => {
			return renderComponent(ExpirationBadge, {
				date: row.original.expiration ? new Date(row.original.expiration) : undefined
			});
		}
	},
	{
		id: 'sliding',
		accessorFn: (row) => slidingSortValue(row.sliding_duration),
		header: ({ column, table }) =>
			renderComponent(SortableHeader, { column, table, title: 'Sliding Duration' }),
		cell: ({ row }) => {
			return renderComponent(SlidingBadge, { duration: row.original.sliding_duration });
		}
	},
	{
		accessorKey: 'encoding',
		header: ({ column, table }) =>
			renderComponent(SortableHeader, { column, table, title: 'Encoding' }),
		cell: ({ row }) => {
			return renderComponent(EncodingBadge, {
				encoding: row.original.encoding
			});
		}
	},
	{
		accessorKey: 'size',
		header: ({ column, table }) =>
			renderComponent(SortableHeader, { column, table, title: 'Size' }),
		cell: ({ row }) => {
			return renderComponent(CacheEntryDisplay, { entry: row.original });
		}
	},
	{
		id: 'actions',
		enableSorting: false,
		header: () => renderHeader(''),
		cell: ({ row }) => {
			return renderComponent(CacheTableActions, { entry: row.original });
		}
	}
];
