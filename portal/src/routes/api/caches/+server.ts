import { readCacheValue } from '$lib/server/cacheEntryActions';
import { json, type RequestHandler } from '@sveltejs/kit';

export const GET: RequestHandler = async ({ url }) => {
	const key = url.searchParams.get('key');

	if (!key) {
		return json({ error: 'Missing key parameter' }, { status: 400 });
	}

	try {
		const buffer = await readCacheValue(key);

		if (!buffer) {
			return json({ error: 'No data found' }, { status: 404 });
		}

		// Decode the buffer to string
		const decoder = new TextDecoder();
		const data = decoder.decode(buffer);

		return json({ data });
	} catch (error) {
		const message = error instanceof Error ? error.message : 'Failed to read cache value';
		return json({ error: message }, { status: 500 });
	}
};
