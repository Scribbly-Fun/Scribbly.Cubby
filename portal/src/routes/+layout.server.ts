import type { LayoutServerLoad } from './$types';
import { env } from '$env/dynamic/private';
import { error } from '@sveltejs/kit';
import type { CubbyOptions } from '$lib/api/CubbyOptions';

export const load = (async () => {
     const cubbyUrl = env.CUBBY_HOST_URL;
     
     if (!cubbyUrl) {
          error(500, 'CUBBY_HOST_URL environment variable is not configured');
     }
     
     var response = await fetch(cubbyUrl);
        
            console.log(`Portal Connected to Cubby @${cubbyUrl} STATUS: ${response.status}`)
            if (response.status === 200) {
                const options = (await response.json()) as CubbyOptions;
                return {
                    cubby_options: options
                };
            }
    
        error(response.status, `Failed to connect to Cubby at ${cubbyUrl}`);
}) satisfies LayoutServerLoad;