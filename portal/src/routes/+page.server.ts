import type { PageServerLoad } from './$types';
import { redirect } from '@sveltejs/kit';
import { env } from '$env/dynamic/private';
import type { CubbyOptions } from '$lib/api/CubbyOptions';

export const load = (async () => {
    redirect(307, '/dashboard');
}) satisfies PageServerLoad;