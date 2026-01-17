import type { PageServerLoad } from './$types';
import { CUBBY_PROJECT_HTTP } from '$env/static/private';
import type { CubbyOptions } from '$lib/api/CubbyOptions';

export const load = (async () => {

    var response = await fetch(CUBBY_PROJECT_HTTP);
    
        console.log(`Portal Connected to Cubby @${CUBBY_PROJECT_HTTP} STATUS: ${response.status}`)
		if (response.status === 200) {
			const options = (await response.json()) as CubbyOptions[];
			return {
				cubby_options: options
			};
		}

    return {cubby_options: undefined};
}) satisfies PageServerLoad;