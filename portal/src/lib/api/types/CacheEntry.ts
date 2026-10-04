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
 * True when the entry's expiration instant is at or before the current UTC time.
 * Expired entries can remain in the table until the next query or async cleanup.
 */
export function isEntryExpired(
	expiration: string | Date | undefined | null,
	nowUtcMs = Date.now()
): boolean {
	if (!expiration) {
		return false;
	}

	const expirationUtcMs =
		expiration instanceof Date ? expiration.getTime() : Date.parse(expiration);
	return !Number.isNaN(expirationUtcMs) && expirationUtcMs <= nowUtcMs;
}
