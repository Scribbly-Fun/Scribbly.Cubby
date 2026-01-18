import type { PageServerLoad } from './$types';
import { env } from '$env/dynamic/private';
import { error } from '@sveltejs/kit';
import type { CacheEntry } from '$lib/api/types/CacheEntry';

export const load = (async () => {

    const cubbyUrl = env.CUBBY_HOST_URL;
     
     if (!cubbyUrl) {
          error(500, 'CUBBY_HOST_URL environment variable is not configured');
     }
     
     var response = await fetch(`${cubbyUrl}/cubby-caches`);
        
            console.log(`Loaded Cache Entries @${cubbyUrl} STATUS: ${response.status}`)
            if (response.status === 200) {
                const entries = (await response.json()) as CacheEntry[];
                
                console.log(`Loaded Cache Entries @${cubbyUrl} STATUS: ${entries}`)
                return {
                    entries: entries
                };
            }
    
        error(response.status, `Failed to connect to Cubby at ${cubbyUrl}`);
}) satisfies PageServerLoad;