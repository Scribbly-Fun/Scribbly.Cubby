import type { EntryFlags } from './EntryFlags';

/**
 * A cache entry representation.
 */
export type CacheEntry = {
	key: string;
	flags: EntryFlags;
	encoding: string;
	size: number;
	expiration: string | undefined;
	sliding: string | undefined;
};
