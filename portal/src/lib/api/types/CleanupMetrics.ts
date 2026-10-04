import type { SlidingDuration } from './SlidingDuration';

/**
 * Dashboard timeline ranges for cleanup metrics.
 */
export type CleanupMetricsRange = '1h' | '3h' | '24h' | '3d';

/**
 * A single cleanup pass sample returned by the host metrics endpoint.
 */
export type CleanupMetricPoint = {
	timestamp: string;
	items_total: number;
	items_serviced: number;
	removed_tombstone: number;
	removed_expired: number;
	removed_sliding: number;
	skipped_due_to_writers: boolean;
	hit_deadline: boolean;
	duration_ms: number;
};

/**
 * Cleanup metrics payload used by the dashboard chart.
 */
export type CleanupMetrics = {
	resolution: string;
	sample_delay: SlidingDuration;
	capacity: number;
	from: string;
	to: string;
	points: CleanupMetricPoint[];
};

const RANGE_HOURS: Record<CleanupMetricsRange, number> = {
	'1h': 1,
	'3h': 3,
	'24h': 24,
	'3d': 72
};

export function isCleanupMetricsRange(value: string | null | undefined): value is CleanupMetricsRange {
	return value === '1h' || value === '3h' || value === '24h' || value === '3d';
}

export function resolveCleanupMetricsWindow(
	range: CleanupMetricsRange,
	now: Date = new Date()
): { from: Date; to: Date } {
	const to = now;
	const from = new Date(to.getTime() - RANGE_HOURS[range] * 60 * 60 * 1000);
	return { from, to };
}
