import type { EntryEncoding } from './EntryEncoding';
import type { CacheEntry } from './CacheEntry';
import { EntryFlags, hasFlag, isSliding } from './EntryFlags';

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

const TIMESPAN_PATTERN = /^(?:(\d+)\.)?(\d+):(\d{2}):(\d{2})(?:\.\d+)?$/;

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

export function normalizeTimeSpan(value: string | undefined): string | undefined {
	if (!value) {
		return undefined;
	}

	const match = TIMESPAN_PATTERN.exec(value.trim());
	if (!match) {
		return undefined;
	}

	const days = match[1];
	const hours = match[2].padStart(2, '0');
	const minutes = match[3];
	const seconds = match[4];

	return days ? `${days}.${hours}:${minutes}:${seconds}` : `${hours}:${minutes}:${seconds}`;
}

export function durationUntil(expirationIso: string, now = Date.now()): string | undefined {
	const end = Date.parse(expirationIso);
	if (Number.isNaN(end) || end <= now) {
		return undefined;
	}

	let totalSeconds = Math.floor((end - now) / 1000);
	const days = Math.floor(totalSeconds / 86400);
	totalSeconds %= 86400;
	const hours = Math.floor(totalSeconds / 3600);
	const minutes = Math.floor((totalSeconds % 3600) / 60);
	const seconds = totalSeconds % 60;
	const hms = `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;

	return days > 0 ? `${days}.${hms}` : hms;
}

export function optionsFromEntry(entry: CacheEntry): CacheEntryOptions {
	const compressed = hasFlag(entry.flags, EntryFlags.Comppressed);
	const slidingDuration = normalizeTimeSpan(entry.sliding_duration);

	if (isSliding(entry.flags)) {
		return {
			encoding: entry.encoding,
			compressed,
			expiration: 'Sliding',
			duration:
				slidingDuration && isPositiveTimeSpan(slidingDuration) ? slidingDuration : '00:05:00'
		};
	}

	if (entry.expiration) {
		const remaining = durationUntil(entry.expiration);
		if (remaining) {
			return {
				encoding: entry.encoding,
				compressed,
				expiration: 'Absolute',
				duration: remaining
			};
		}
	}

	return {
		encoding: entry.encoding,
		compressed,
		expiration: 'Never'
	};
}

export function valueSourceFromEncoding(encoding: EntryEncoding): 'file' | 'json' | 'text' {
	if (encoding === 'Json') {
		return 'json';
	}

	if (encoding === 'Utf8String' || encoding === 'Utf16String') {
		return 'text';
	}

	return 'file';
}
