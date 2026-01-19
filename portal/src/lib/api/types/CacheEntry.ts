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
