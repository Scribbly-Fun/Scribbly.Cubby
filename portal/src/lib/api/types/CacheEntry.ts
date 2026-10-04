import type { EntryEncoding } from './EntryEncoding';
import type { EntryFlags } from './EntryFlags';
import type { SlidingDuration } from './SlidingDuration';

/**
 * A cache entry representation.
 */
export type CacheEntry = {
	key: string;
	flags: EntryFlags;
	encoding: EntryEncoding;
	size: number;
	expiration: string | undefined;
	sliding_duration: SlidingDuration | undefined;
};

/**
 * True when the entry has an expiration timestamp that is at or before now.
 * Expired entries can remain in the table until the next query or async cleanup.
 */
export function isEntryExpired(
	expiration: string | Date | undefined | null,
	now = Date.now()
): boolean {
	if (!expiration) {
		return false;
	}

	const time = expiration instanceof Date ? expiration.getTime() : Date.parse(expiration);
	return !Number.isNaN(time) && time <= now;
}
