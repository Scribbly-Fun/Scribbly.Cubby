import { env } from '$env/dynamic/private';
import type { CacheEntryOptions } from '$lib/api/types/CacheEntryOptions';
import { toCubbyExpiry, toCubbyFlags } from '$lib/api/types/CacheEntryOptions';
import type { EntryEncoding } from '$lib/api/types/EntryEncoding';

const ENCODINGS: readonly EntryEncoding[] = [
	'None',
	'Utf8String',
	'Utf16String',
	'Json',
	'MessagePack'
];

export function isEntryEncoding(value: string): value is EntryEncoding {
	return (ENCODINGS as readonly string[]).includes(value);
}

export function encodeTextValue(text: string, encoding: EntryEncoding): Uint8Array {
	if (encoding === 'Utf16String') {
		const bytes = new Uint8Array(text.length * 2);
		for (let i = 0; i < text.length; i++) {
			const code = text.charCodeAt(i);
			bytes[i * 2] = code & 0xff;
			bytes[i * 2 + 1] = code >> 8;
		}
		return bytes;
	}

	return new TextEncoder().encode(text);
}

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

export type CreateCacheEntryInput = {
	key: string;
	options: CacheEntryOptions;
	value: Uint8Array;
};

/**
 * Inserts or updates a cache entry through the existing HTTP Put endpoint.
 * Entry options are sent as the headers Put already maps to CacheEntryOptions.
 * @returns True when the host created or updated the entry
 */
export async function createEntry(input: CreateCacheEntryInput): Promise<boolean> {
	const cubbyUrl = env.CUBBY_HOST_URL;

	if (!cubbyUrl) {
		console.error('CUBBY_HOST_URL is not configured');
		return false;
	}

	const params = new URLSearchParams({
		key: input.key
	});

	const body = new ArrayBuffer(input.value.byteLength);
	new Uint8Array(body).set(input.value);

	const headers: Record<string, string> = {
		'Content-Type': 'application/octet-stream',
		'x-cubby-encoding': input.options.encoding,
		'x-cubby-flags': toCubbyFlags(input.options)
	};

	const expiry = toCubbyExpiry(input.options);
	if (expiry) {
		headers['x-cubby-expiry'] = expiry;
	}

	try {
		const response = await fetch(`${cubbyUrl}/cubby?${params.toString()}`, {
			method: 'PUT',
			headers,
			body
		});

		console.warn('Executed Create Entry', response.status);
		return response.ok;
	} catch (error) {
		console.error(`Error creating entry: ${input.key}`, error);
		return false;
	}
}
