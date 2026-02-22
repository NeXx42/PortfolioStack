import { useEffect, useState } from "react";

import * as api from "../api/api"

import type { Item, ItemContent } from "../types";

interface HookReturn {
    content?: Item,
    loading: boolean,
    error?: string,

    updatePage: (newPages?: ItemContent[], updates?: ItemContent[], deletions?: number[]) => Promise<void>,
}


export function useGame(gameId: string): HookReturn {
    const [content, setContent] = useState<Item | undefined>(undefined);
    const [error, setError] = useState<string | undefined>(undefined);

    const [loading, setLoadingFeatured] = useState(true);

    useEffect(() => {
        setLoadingFeatured(true);
        setError(undefined);

        api.fetchGame(gameId)
            .then((x: Item) => setContent(x))
            .catch((err: any) => setError(err.Message))
            .finally(() => setLoadingFeatured(false));
    }, [gameId])


    const updatePage = async (newPages?: ItemContent[], updates?: ItemContent[], deletions?: number[]) => {
        await api.updatePage(gameId, newPages, updates, deletions);
    }


    return {
        content,
        loading,
        error,

        updatePage
    }
}