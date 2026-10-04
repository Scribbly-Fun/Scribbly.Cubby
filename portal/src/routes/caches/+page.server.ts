import type { PageServerLoad, Actions } from './$types';
import { env } from '$env/dynamic/private';
import { error, fail } from '@sveltejs/kit';
import type { CacheEntry } from '$lib/api/types/CacheEntry';
import {
	createEntry,
	encodeTextValue,
	evictEntry,
	isEntryEncoding,
	readCacheValue,
	tombstoneEntry
} from '$lib/server/cacheEntryActions';
import { isExpirationMode, isPositiveTimeSpan } from '$lib/api/types/CacheEntryOptions';

export const load = (async () => {
	const cubbyUrl = env.CUBBY_HOST_URL;

	if (!cubbyUrl) {
		error(500, 'CUBBY_HOST_URL environment variable is not configured');
	}

	var response = await fetch(`${cubbyUrl}/cubby/portal/caches`);

	console.log(`Loaded Cache Entries @${cubbyUrl} STATUS: ${response.status}`);
	if (response.status === 200) {
		const entries = (await response.json()) as CacheEntry[];

		console.log(`Loaded Cache Entries @${cubbyUrl} STATUS: ${entries}`);
		return {
			entries: entries
		};
	}

	error(response.status, `Failed to connect to Cubby at ${cubbyUrl}`);
}) satisfies PageServerLoad;

export const actions = {
	evict: async ({ request }) => {
		const data = await request.formData();
		const key = data.get('key') as string;

		if (!key) {
			return fail(400, { message: 'Key is required' });
		}

		const success = await evictEntry(key);
		if (!success) {
			return fail(500, { message: `Failed to evict entry: ${key}` });
		}

		return { success: true, message: `Entry ${key} evicted` };
	},

	tombstone: async ({ request }) => {
		const data = await request.formData();
		const key = data.get('key') as string;

		if (!key) {
			return fail(400, { message: 'Key is required' });
		}

		const success = await tombstoneEntry(key);
		if (!success) {
			return fail(500, { message: `Failed to tombstone entry: ${key}` });
		}

		return { success: true, message: `Entry ${key} tombstoned` };
	},

	create: async ({ request }) => {
		const data = await request.formData();
		const key = (data.get('key') as string | null)?.trim() ?? '';
		const encodingValue = (data.get('encoding') as string | null) ?? 'None';
		const source = (data.get('source') as string | null) ?? 'text';
		const expirationValue = (data.get('expiration') as string | null) ?? 'Never';
		const duration = ((data.get('duration') as string | null) ?? '').trim();
		const compressed =
			data.get('compressed') === 'true' ||
			data.get('compressed') === 'on' ||
			data.get('compressed') === '1';
		const keepValue = data.get('keepValue') === 'true';

		if (!key) {
			return fail(400, { message: 'Key is required' });
		}

		if (!isEntryEncoding(encodingValue)) {
			return fail(400, { message: 'A valid encoding is required' });
		}

		if (!isExpirationMode(expirationValue)) {
			return fail(400, { message: 'A valid expiration is required' });
		}

		if (expirationValue !== 'Never' && !isPositiveTimeSpan(duration)) {
			return fail(400, {
				message: 'Expiration duration must be a TimeSpan greater than zero, such as 00:05:00'
			});
		}

		let value: Uint8Array;

		if (source === 'file') {
			const file = data.get('file');
			if (file instanceof File && file.size > 0) {
				value = new Uint8Array(await file.arrayBuffer());
			} else if (keepValue) {
				const existing = await readCacheValue(key);
				if (!existing) {
					return fail(404, { message: `Existing value was not found for ${key}` });
				}
				value = new Uint8Array(existing);
			} else {
				return fail(400, { message: 'A file is required' });
			}
		} else {
			const text = (data.get('value') as string | null) ?? '';

			if (source === 'json') {
				try {
					JSON.parse(text);
				} catch {
					return fail(400, { message: 'Value must be valid JSON' });
				}
			}

			value = encodeTextValue(text, encodingValue);
		}

		const success = await createEntry({
			key,
			options: {
				encoding: encodingValue,
				compressed,
				expiration: expirationValue,
				duration: expirationValue === 'Never' ? undefined : duration
			},
			value
		});

		if (!success) {
			return fail(500, { message: `Failed to save entry: ${key}` });
		}

		return { success: true, message: `Entry ${key} saved` };
	}
} satisfies Actions;
