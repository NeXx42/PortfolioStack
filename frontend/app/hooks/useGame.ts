import { useEffect, useState } from "react";

import * as api from "@api/api.client"

import type { Project } from "@shared/types";

interface HookReturn {
    content?: Project,
    loading: boolean,
    error?: string,
}


export function useGame(gameId?: string): HookReturn {
    const [content, setContent] = useState<Project | undefined>(undefined);
    const [error, setError] = useState<string | undefined>(undefined);

    const [loading, setLoadingFeatured] = useState(true);

    useEffect(() => {
        if (gameId === undefined)
            return;

        setLoadingFeatured(true);
        setError(undefined);

        api.fetchGame(gameId)
            .then((x: Project) => setContent(x))
            .catch((err: any) => setError(err.Message))
            .finally(() => setLoadingFeatured(false));
    }, [gameId])


    return {
        content,
        loading,
        error,
    }
}