import { env } from '$env/dynamic/private';

/**
 * Evicts a cache entry from the cache
 * @param key The key of the entry to evict
 * @returns True when the entry was removed
 */
export async function evictEntry(key: string): Promise<boolean> {
	const cubbyUrl = env.CUBBY_HOST_URL;

	if (!cubbyUrl) {
		console.error('CUBBY_HOST_URL is not configured');
		return false;
	}

	try {
		const response = await fetch(`${cubbyUrl}/cubby/portal/caches/evict?key=${key}`, {
			method: 'DELETE'
		});
		console.warn('Executed Eviction', response);
		return response.ok;
	} catch (error) {
		console.error(`Error evicting entry: ${key}`, error);
		return false;
	}
}

/**
 * Marks a cache entry for deletion.  Once marked the entry will be removed on the next query or cleanup
 * @param key The key of the entry to evict
 * @returns True when the entry was tombstoned
 */
export async function tombstoneEntry(key: string): Promise<boolean> {
	const cubbyUrl = env.CUBBY_HOST_URL;

	if (!cubbyUrl) {
		console.error('CUBBY_HOST_URL is not configured');
		return false;
	}

	try {
		const response = await fetch(`${cubbyUrl}/cubby/portal/caches/tombstone?key=${key}`, {
			method: 'DELETE'
		});
		console.warn('Executed Tombstone', response);
		return response.ok;
	} catch (error) {
		console.error(`Error tombstoning entry: ${key}`, error);
		return false;
	}
}

/**
 * Reads the raw value of a cache entry
 * @param key The key of the entry to evict
 * @returns The data in the buffer or undefined if not found
 */
export async function readCacheValue(key: string): Promise<ArrayBuffer | undefined> {
	const cubbyUrl = env.CUBBY_HOST_URL;

	if (!cubbyUrl) {
		console.error('CUBBY_HOST_URL is not configured');
		return undefined;
	}

	try {
		const response = await fetch(`${cubbyUrl}/cubby/portal/caches/value?key=${key}`, {
			method: 'GET'
		});

		if (response.status === 200) {
			return await response.arrayBuffer();
		}

		return undefined;
	} catch (error) {
		console.error(`Error tombstoning entry: ${key}`, error);

		return undefined;
	}
}
