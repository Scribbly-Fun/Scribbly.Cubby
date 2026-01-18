<script lang="ts" module>
	import type { Flags } from "$lib/api/types";


    export type FlagsProps = {
        flags: Flags
    };

</script>

<script lang="ts" >
    import Badge from "../badge/badge.svelte";
    
    let { flags }: { flags: FlagsProps } = $props();
</script>

<Badge>{flags.flags}</Badge>