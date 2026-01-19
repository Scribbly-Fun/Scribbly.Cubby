export enum EntryFlags {
	None = 0,
	Comppressed = 1 << 0,
	Sliding = 1 << 1,
	Tombstone = 1 << 2
}

export const FLAG_NAMES: Record<EntryFlags, string> = {
	[EntryFlags.None]: 'None',
	[EntryFlags.Comppressed]: 'Compressed',
	[EntryFlags.Sliding]: 'Sliding',
	[EntryFlags.Tombstone]: 'Tombstone'
};

export function hasFlag(flags: EntryFlags, flagToCheck: EntryFlags): boolean {
	return (flags & flagToCheck) === flagToCheck;
}

export function addFlag(flags: EntryFlags, flagToAdd: EntryFlags): EntryFlags {
	return flags | flagToAdd;
}

export function removeFlag(flags: EntryFlags, flagToRemove: EntryFlags): EntryFlags {
	return flags & ~flagToRemove;
}

export function isTombstone(flags: EntryFlags): boolean {
	return hasFlag(flags, EntryFlags.Tombstone);
}

export function isSliding(flags: EntryFlags): boolean {
	return hasFlag(flags, EntryFlags.Sliding);
}
