import type { ColumnDef } from "@tanstack/table-core";
import type CacheEntry from '$lib/api/CacheEntry';
 
export const columns: ColumnDef<CacheEntry>[] = [
 {
  accessorKey: "key",
  header: "Key",
 },
 {
  accessorKey: "flags",
  header: "Flags",
 },
 {
  accessorKey: "encoding",
  header: "Encoding",
 },
 {
  accessorKey: "size",
  header: "Size",
 },
];