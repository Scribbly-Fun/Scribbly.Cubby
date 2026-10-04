import type { EntryEncoding } from './EntryEncoding';

/**
 * How a cache entry expires when inserted through Put.
 * Matches CacheEntryOptions.From: a duration plus the Sliding flag.
 */
export type ExpirationMode = 'Never' | 'Absolute' | 'Sliding';

export const EXPIRATION_MODES: readonly ExpirationMode[] = ['Never', 'Absolute', 'Sliding'];

export const EXPIRATION_LABELS: Record<ExpirationMode, string> = {
	Never: 'Never',
	Absolute: 'Absolute',
	Sliding: 'Sliding'
};

/**
 * Options that instruct how a cache entry behaves when entered.
 * Put maps these to CacheEntryOptions via x-cubby-encoding, x-cubby-flags, and x-cubby-expiry.
 */
export type CacheEntryOptions = {
	encoding: EntryEncoding;
	compressed: boolean;
	expiration: ExpirationMode;
	duration?: string;
};

const TIMESPAN_PATTERN = /^(?:(\d+)\.)?(\d+):(\d{2}):(\d{2})$/;

export function isExpirationMode(value: string): value is ExpirationMode {
	return (EXPIRATION_MODES as readonly string[]).includes(value);
}

export function isTimeSpan(value: string): boolean {
	return TIMESPAN_PATTERN.test(value.trim());
}

export function isPositiveTimeSpan(value: string): boolean {
	const match = TIMESPAN_PATTERN.exec(value.trim());
	if (!match) {
		return false;
	}

	const days = Number(match[1] ?? 0);
	const hours = Number(match[2]);
	const minutes = Number(match[3]);
	const seconds = Number(match[4]);

	return days + hours + minutes + seconds > 0;
}

/**
 * Flags string accepted by Put / CacheEntryFlags.ToFlagsString().
 */
export function toCubbyFlags(options: CacheEntryOptions): string {
	const sliding = options.expiration === 'Sliding';

	if (options.compressed && sliding) {
		return 'Compressed,Sliding';
	}

	if (options.compressed) {
		return 'Compressed';
	}

	if (sliding) {
		return 'Sliding';
	}

	return 'None';
}

/**
 * Duration header for Put. Omitted for Never so CacheEntryOptions.From uses Never().
 */
export function toCubbyExpiry(options: CacheEntryOptions): string | undefined {
	if (options.expiration === 'Never') {
		return undefined;
	}

	const duration = options.duration?.trim();
	return duration || undefined;
}
