import { error } from '@sveltejs/kit';
import type { RequestHandler } from './$types';
import { getDocDocument } from '$lib/docs/documents';

const README_RAW_URL =
	'https://raw.githubusercontent.com/Scribbly-Fun/Scribbly.Cubby/main/README.md';
const README_ASSET_BASE =
	'https://raw.githubusercontent.com/Scribbly-Fun/Scribbly.Cubby/main/';
const MARKDOWN_API_URL = 'https://api.github.com/markdown';

function rewriteRelativeUrls(html: string): string {
	return html
		.replace(/(src|href)="(\.\/[^"]+)"/g, (_match, attr: string, path: string) => {
			return `${attr}="${README_ASSET_BASE}${path.slice(2)}"`;
		})
		.replace(/(src|href)="(?!https?:|mailto:|#|\/)([^"]+)"/g, (_match, attr: string, path: string) => {
			return `${attr}="${README_ASSET_BASE}${path}"`;
		});
}

function githubSlug(text: string): string {
	return text
		.trim()
		.toLowerCase()
		.replace(/[^\p{L}\p{N}\s-]/gu, '')
		.replace(/\s+/g, '-');
}

function addHeadingIds(html: string): string {
	const seen = new Map<string, number>();

	return html.replace(/<h([1-6])([^>]*)>([\s\S]*?)<\/h\1>/gi, (match, level: string, attrs: string, inner: string) => {
		if (/\sid\s*=/.test(attrs)) {
			return match;
		}

		const text = inner.replace(/<[^>]+>/g, '').trim();
		let slug = githubSlug(text);
		if (!slug) {
			return match;
		}

		const count = seen.get(slug) ?? 0;
		seen.set(slug, count + 1);
		if (count > 0) {
			slug = `${slug}-${count}`;
		}

		return `<h${level}${attrs} id="${slug}">${inner}</h${level}>`;
	});
}

function renderDocumentPage(title: string, bodyHtml: string, anchor: string): string {
	return `<!doctype html>
<html lang="en">
<head>
	<meta charset="utf-8" />
	<meta name="viewport" content="width=device-width, initial-scale=1" />
	<title>${title} Documentation</title>
	<style>
		:root {
			color-scheme: light dark;
			--bg: #ffffff;
			--fg: #1f2328;
			--muted: #59636e;
			--border: #d1d9e0;
			--code-bg: #f6f8fa;
			--link: #0969da;
		}
		@media (prefers-color-scheme: dark) {
			:root {
				--bg: #0d1117;
				--fg: #e6edf3;
				--muted: #9198a1;
				--border: #3d444d;
				--code-bg: #161b22;
				--link: #4493f8;
			}
		}
		* { box-sizing: border-box; }
		body {
			margin: 0;
			padding: 1.5rem;
			font: 16px/1.6 -apple-system, BlinkMacSystemFont, "Segoe UI", Helvetica, Arial, sans-serif;
			background: var(--bg);
			color: var(--fg);
		}
		.markdown-body { max-width: 56rem; margin: 0 auto; }
		.markdown-body h1,
		.markdown-body h2,
		.markdown-body h3,
		.markdown-body h4 {
			margin-top: 1.5em;
			margin-bottom: 0.5em;
			line-height: 1.25;
			border-bottom: 1px solid var(--border);
			padding-bottom: 0.3em;
		}
		.markdown-body h1 { border-bottom-width: 2px; }
		.markdown-body p { margin: 0.8em 0; }
		.markdown-body a { color: var(--link); }
		.markdown-body img { max-width: 100%; height: auto; }
		.markdown-body code {
			font-family: ui-monospace, SFMono-Regular, Menlo, Consolas, monospace;
			background: var(--code-bg);
			padding: 0.15em 0.35em;
			border-radius: 4px;
			font-size: 0.9em;
		}
		.markdown-body pre {
			background: var(--code-bg);
			border: 1px solid var(--border);
			border-radius: 8px;
			padding: 1rem;
			overflow: auto;
		}
		.markdown-body pre code {
			background: transparent;
			padding: 0;
		}
		.markdown-body blockquote {
			margin: 1em 0;
			padding: 0.25em 1em;
			color: var(--muted);
			border-left: 0.25em solid var(--border);
		}
		.markdown-body table {
			border-collapse: collapse;
			width: 100%;
			margin: 1em 0;
		}
		.markdown-body th,
		.markdown-body td {
			border: 1px solid var(--border);
			padding: 0.5rem 0.75rem;
		}
		.markdown-body hr {
			border: 0;
			border-top: 1px solid var(--border);
			margin: 1.5em 0;
		}
	</style>
</head>
<body>
	<article class="markdown-body">
		${bodyHtml}
	</article>
	<script>
		const anchor = ${JSON.stringify(anchor)};
		function scrollToAnchor() {
			if (!anchor) return;
			const el = document.getElementById(anchor) || document.querySelector('[name="' + anchor + '"]');
			if (el) el.scrollIntoView();
		}
		scrollToAnchor();
		window.addEventListener('load', scrollToAnchor);
	</script>
</body>
</html>`;
}

export const GET: RequestHandler = async ({ params, fetch }) => {
	const doc = getDocDocument(params.slug);
	if (!doc) {
		error(404, 'Documentation page not found');
	}

	const readmeResponse = await fetch(README_RAW_URL, {
		headers: {
			Accept: 'text/plain',
			'User-Agent': 'Scribbly.Cubby.Portal'
		}
	});

	if (!readmeResponse.ok) {
		error(502, 'Failed to load documentation source');
	}

	const markdown = await readmeResponse.text();

	const renderedResponse = await fetch(MARKDOWN_API_URL, {
		method: 'POST',
		headers: {
			Accept: 'application/vnd.github+json',
			'Content-Type': 'application/json',
			'User-Agent': 'Scribbly.Cubby.Portal',
			'X-GitHub-Api-Version': '2022-11-28'
		},
		body: JSON.stringify({
			text: markdown,
			mode: 'gfm',
			context: 'Scribbly-Fun/Scribbly.Cubby'
		})
	});

	if (!renderedResponse.ok) {
		error(502, 'Failed to render documentation');
	}

	const bodyHtml = addHeadingIds(rewriteRelativeUrls(await renderedResponse.text()));
	const html = renderDocumentPage(doc.name, bodyHtml, doc.anchor);

	return new Response(html, {
		headers: {
			'Content-Type': 'text/html; charset=utf-8',
			'Cache-Control': 'public, max-age=300'
		}
	});
};
