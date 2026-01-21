import type { SlidingDuration } from './SlidingDuration';

/**
 * The options used when starting Cubby
 */
export type CubbyOptions = {
	transports: string;
	store: string;
	capacity: number;
	cores: number;
	cleanup: CleanupOptions;
};

/**
 * Async cleanup options
 */
export type CleanupOptions = {
	strategy: AsyncStrategy;
	delay: SlidingDuration;
};

/**
 * The available async cleanup strategies
 */
export type AsyncStrategy = 'Disabled' | 'Hourly' | 'Random' | 'Aggressive' | 'Duration' | 'Manual';
