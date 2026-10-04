import adapter from '@sveltejs/adapter-node';
import { vitePreprocess } from '@sveltejs/vite-plugin-svelte';

/** @type {import('@sveltejs/kit').Config} */
const config = {
	// Consult https://svelte.dev/docs/kit/integrations
	// for more information about preprocessors
	preprocess: vitePreprocess(),

	kit: {
		// adapter-auto only supports some environments, see https://svelte.dev/docs/kit/adapter-auto for a list.
		// If your environment is not supported, or you settled on a specific environment, switch out the adapter.
		// See https://svelte.dev/docs/kit/adapters for more information about adapters.
		adapter: adapter({
			out: 'build'
		}),
		csrf: {
			// adapter-node rebuilds request.url as https://<host> unless ORIGIN or
			// x-forwarded-proto is set. Aspire/Docker serve this admin UI over HTTP
			// on a published host port, so browser Origin (http://localhost:<hostPort>)
			// never matches and form actions return 403 Cross-site POST form submissions.
			trustedOrigins: ['*']
		}
	}
};

export default config;
