import type { PageServerLoad, Actions } from './$types';
import { env } from '$env/dynamic/private';
import { error, fail } from '@sveltejs/kit';
import type { CacheEntry } from '$lib/api/types/CacheEntry';
import { evictEntry, tombstoneEntry } from '$lib/server/cacheEntryActions';

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
	}
} satisfies Actions;
