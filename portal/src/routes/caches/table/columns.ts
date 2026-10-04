import type { CacheEntry } from '$lib/api/types/CacheEntry';
import type { SlidingDuration } from '$lib/api/types/SlidingDuration';

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
