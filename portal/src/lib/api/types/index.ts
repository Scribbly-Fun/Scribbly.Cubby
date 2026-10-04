import type { CacheEntry } from './CacheEntry';
import { isEntryExpired } from './CacheEntry';
import type { EntryFlags } from './EntryFlags';
import type { CubbyOptions } from './CubbyOptions';
import type { SlidingDuration } from './SlidingDuration';
import type { EntryEncoding } from './EntryEncoding';
import type { CacheEntryOptions, ExpirationMode } from './CacheEntryOptions';
import type { CleanupMetrics, CleanupMetricPoint, CleanupMetricsRange } from './CleanupMetrics';
import {
	isCleanupMetricsRange,
	resolveCleanupMetricsWindow
} from './CleanupMetrics';

export type {
	CacheEntry,
	EntryFlags,
	EntryEncoding,
	CacheEntryOptions,
	ExpirationMode,
	CubbyOptions,
	SlidingDuration,
	CleanupMetrics,
	CleanupMetricPoint,
	CleanupMetricsRange,
	//
	CacheEntry as Entry,
	EntryFlags as Flags,
	EntryEncoding as Encoding,
	CubbyOptions as Options,
	SlidingDuration as Duration
};

export { isEntryExpired, isCleanupMetricsRange, resolveCleanupMetricsWindow };
