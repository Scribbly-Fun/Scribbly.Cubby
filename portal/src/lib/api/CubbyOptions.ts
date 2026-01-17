/**
 * The options used when starting Cubby
 */
export type CubbyOptions = {
	transports: string;
	store: string;
	capacity: number;
	cores: number;
};

/**
 * Async cleanup options
 */
export type CleanupOptions = {
    strategy: number;
    delay: string;
}