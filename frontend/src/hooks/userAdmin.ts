import { useEffect, useState } from "react";

import * as api from "../api/api"
import type { Item } from "../types";


interface HookReturn {
    loading: boolean

    slugs?: string[],

    saveItem: (item: Item) => Promise<void>,
    clearCache: () => Promise<void>

    images?: string[],
    uploadImage: (img: FormData) => Promise<string>
}


export function useAdmin(): HookReturn {
    const [images, setImages] = useState<string[] | undefined>(undefined);
    const [slugs, setSlugs] = useState<string[] | undefined>(undefined);
    const [loading, setLoading] = useState<boolean>(false);

    useEffect(() => {
        setLoading(true);

        api.admin_Slugs()
            .then(setSlugs)
            //.catch((err: any) => setError(err.message))
            .finally(() => setLoading(false));

        api.admin_GetImages()
            .then(setImages);
    }, [])

    const functionWrapper = async (inp: Promise<void>) => {
        setLoading(true);
        inp.finally(() => setLoading(false));
    }

    return {
        slugs,
        loading,

        saveItem: (i) => functionWrapper(api.admin_SaveItem(i)),
        clearCache: () => functionWrapper(api.admin_ClearCache()),

        images,
        uploadImage: async (img: FormData) => await api.admin_UploadImage(img),
    }
}