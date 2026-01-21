import type { CacheEntry } from '$lib/api/types/CacheEntry';
import { env } from '$env/dynamic/private';

let cachedEntries: CacheEntry[] = [];
let lastFetch = 0;
const POLL_INTERVAL = 5000; // 5 seconds

async function fetchCacheEntries(): Promise<CacheEntry[]> {
	const cubbyUrl = env.CUBBY_HOST_URL;

	if (!cubbyUrl) {
		console.error('CUBBY_HOST_URL is not configured');
		return [];
	}

	try {
		const response = await fetch(`${cubbyUrl}/cubby-caches`);
		if (response.ok) {
			const entries = (await response.json()) as CacheEntry[];
			cachedEntries = entries;
			lastFetch = Date.now();
			console.log(`[Polling] Updated cache entries: ${entries.length} items`);
			return entries;
		} else {
			console.error(`[Polling] Failed to fetch cache entries: ${response.status}`);
			return cachedEntries;
		}
	} catch (error) {
		console.error('[Polling] Error fetching cache entries:', error);
		return cachedEntries;
	}
}

export async function getCacheEntries(): Promise<CacheEntry[]> {
	const now = Date.now();
	
	// If we haven't fetched yet, or enough time has passed, fetch new data
	if (lastFetch === 0 || now - lastFetch > POLL_INTERVAL) {
		return await fetchCacheEntries();
	}
	
	return cachedEntries;
}

// Start polling in the background
let pollingInterval: NodeJS.Timeout | null = null;

export function startPolling(): void {
	if (pollingInterval) return;
	
	console.log('[Polling] Starting cache entries polling');
	fetchCacheEntries(); // Fetch immediately
	
	pollingInterval = setInterval(() => {
		fetchCacheEntries();
	}, POLL_INTERVAL);
}

export function stopPolling(): void {
	if (pollingInterval) {
		clearInterval(pollingInterval);
		pollingInterval = null;
		console.log('[Polling] Stopped cache entries polling');
	}
}
