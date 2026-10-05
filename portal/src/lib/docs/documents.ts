export type DocDocument = {
	slug: string;
	name: string;
	externalUrl: string;
	anchor: string;
};

export const DOC_DOCUMENTS: DocDocument[] = [
	{
		slug: 'server',
		name: 'Server',
		externalUrl: 'https://github.com/Scribbly-Fun/Scribbly.Cubby?tab=readme-ov-file#cubby-host',
		anchor: 'cubby-host'
	},
	{
		slug: 'client',
		name: 'Client',
		externalUrl: 'https://github.com/Scribbly-Fun/Scribbly.Cubby?tab=readme-ov-file#cubby-client',
		anchor: 'cubby-client'
	}
];

export function getDocDocument(slug: string): DocDocument | undefined {
	return DOC_DOCUMENTS.find((doc) => doc.slug === slug);
}
