import { error } from '@sveltejs/kit';
import type { PageLoad } from './$types';
import { getDocDocument } from '$lib/docs/documents';

export const load: PageLoad = ({ params }) => {
	const doc = getDocDocument(params.slug);
	if (!doc) {
		error(404, 'Documentation page not found');
	}

	return { doc };
};
