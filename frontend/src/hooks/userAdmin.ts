import { useEffect, useState } from "react";

import * as api from "../api/api"
import type { Item } from "../types";


interface HookReturn {
    loading: boolean

    slugs?: string[],

    saveItem: (item: Item) => Promise<void>,
    clearCache: () => Promise<void>
}


export function useAdmin(): HookReturn {
    const [slugs, setSlugs] = useState<string[] | undefined>(undefined);
    const [loading, setLoading] = useState<boolean>(false);

    useEffect(() => {
        setLoading(true);

        api.admin_Slugs()
            .then(setSlugs)
            //.catch((err: any) => setError(err.message))
            .finally(() => setLoading(false));
    }, [])

    const functionWrapper = async (inp: Promise<void>) => {
        setLoading(true);
        inp.finally(() => setLoading(false));
    }

    return {
        slugs,
        loading,

        saveItem: (i) => functionWrapper(api.admin_SaveItem(i)),
        clearCache: () => functionWrapper(api.admin_ClearCache())
    }
}