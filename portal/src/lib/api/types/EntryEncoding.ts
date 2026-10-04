/**
 * The encoding types available for entries.
 */
export type EntryEncoding = 'None' | 'Utf8String' | 'Utf16String' | 'Json' | 'MessagePack';

export const ENTRY_ENCODINGS: readonly EntryEncoding[] = [
	'None',
	'Utf8String',
	'Utf16String',
	'Json',
	'MessagePack'
];

export const ENCODING_LABELS: Record<EntryEncoding, string> = {
	None: 'None (raw bytes)',
	Utf8String: 'UTF-8 string',
	Utf16String: 'UTF-16 string',
	Json: 'JSON',
	MessagePack: 'MessagePack'
};
