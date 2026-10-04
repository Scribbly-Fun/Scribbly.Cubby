import type { SlidingDuration } from './SlidingDuration';

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
	points: CleanupMetricPoint[];
};
