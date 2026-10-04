import type { PageServerLoad } from './$types';
import { env } from '$env/dynamic/private';
import { error } from '@sveltejs/kit';
import type { CleanupMetrics } from '$lib/api/types';

export const load = (async () => {
	const cubbyUrl = env.CUBBY_HOST_URL as string | undefined;

	if (!cubbyUrl) {
		error(500, 'CUBBY_HOST_URL environment variable is not configured');
	}

	const response = await fetch(`${cubbyUrl}/cubby/portal/metrics`);

	if (response.status !== 200) {
		error(response.status, `Failed to load cleanup metrics from Cubby at ${cubbyUrl}`);
	}

	const metrics = (await response.json()) as CleanupMetrics;

	return {
		cleanup_metrics: metrics
	};
}) satisfies PageServerLoad;
