import type { PageServerLoad } from './$types';
import { env } from '$env/dynamic/private';
import { error, redirect } from '@sveltejs/kit';
import {
	isCleanupMetricsRange,
	resolveCleanupMetricsWindow,
	type CleanupMetrics,
	type CleanupMetricsRange
} from '$lib/api/types';

const DEFAULT_RANGE: CleanupMetricsRange = '1h';

export const load = (async ({ url }) => {
	const cubbyUrl = env.CUBBY_HOST_URL as string | undefined;

	if (!cubbyUrl) {
		error(500, 'CUBBY_HOST_URL environment variable is not configured');
	}

	const requestedRange = url.searchParams.get('range');
	if (!isCleanupMetricsRange(requestedRange)) {
		const next = new URL(url);
		next.searchParams.set('range', DEFAULT_RANGE);
		redirect(303, `${next.pathname}${next.search}`);
	}

	const range = requestedRange;
	const { from, to } = resolveCleanupMetricsWindow(range);

	const metricsUrl = new URL(`${cubbyUrl}/cubby/portal/metrics`);
	metricsUrl.searchParams.set('from', from.toISOString());
	metricsUrl.searchParams.set('to', to.toISOString());

	const response = await fetch(metricsUrl);

	if (response.status !== 200) {
		error(response.status, `Failed to load cleanup metrics from Cubby at ${cubbyUrl}`);
	}

	const metrics = (await response.json()) as CleanupMetrics;

	return {
		cleanup_metrics: metrics,
		metrics_range: range
	};
}) satisfies PageServerLoad;
