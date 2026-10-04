import { readCacheValue } from '$lib/server/cacheEntryActions';
import { json, type RequestHandler } from '@sveltejs/kit';
import type { EntryEncoding } from '$lib/api/types/EntryEncoding';

export const GET: RequestHandler = async ({ url }) => {
	const key = url.searchParams.get('key');
	const encoding = (url.searchParams.get('encoding') ?? 'None') as EntryEncoding;

	if (!key) {
		return json({ error: 'Missing key parameter' }, { status: 400 });
	}

	try {
		const buffer = await readCacheValue(key);

		if (!buffer) {
			return json({ error: 'No data found' }, { status: 404 });
		}

		const data = decodeCacheValue(buffer, encoding);

		return json({ data });
	} catch (error) {
		const message = error instanceof Error ? error.message : 'Failed to read cache value';
		return json({ error: message }, { status: 500 });
	}
};

function decodeCacheValue(buffer: ArrayBuffer, encoding: EntryEncoding): string {
	if (encoding === 'Utf16String') {
		return new TextDecoder('utf-16le').decode(buffer);
	}

	return new TextDecoder().decode(buffer);
}
