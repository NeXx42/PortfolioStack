import { useEffect, useState } from "react";

import * as api from "@api/api.client"
import type { Project, ProjectTag } from "@shared/types";


interface HookReturn {
    loading: boolean

    slugs?: string[],

    saveItem: (item: Project) => Promise<void>,
    clearCache: () => Promise<void>

    images?: string[],
    uploadImage: (img: FormData) => Promise<string>,

    tags?: ProjectTag[]
    saveTags: (tags: ProjectTag[]) => Promise<void>
}


export function useAdmin(): HookReturn {
    const [images, setImages] = useState<string[] | undefined>(undefined);
    const [tags, setTags] = useState<ProjectTag[] | undefined>(undefined);


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

        api.admin_GetTags()
            .then(setTags);
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

        tags,
        saveTags: async (tags: ProjectTag[]) => await api.admin_SaveTags(tags)
    }
}