import { useEffect, useState } from "react";

import * as api from "@api/api.client"

import type { Item, Link } from "@shared/types";
import { ProjectType } from "@shared/enums";

interface HookReturn {
    content: Record<ProjectType, Item[]>,
    loadingContent: boolean,
    fetchContent: (type: ProjectType) => Promise<void>,
}


export function useContent(): HookReturn {

    const [content, setContent] = useState<Record<ProjectType, Item[]>>(
        {} as Record<ProjectType, Item[]>
    );
    const [loadingContent, setLoadingContent] = useState<boolean>(true);

    const fetchContent = async (type: ProjectType) => {
        if (content[type])
            return

        setLoadingContent(true);
        const items: Item[] = await api.fetchContent(type);

        setContent(prev => ({
            ...prev,
            [type]: items
        }));

        setLoadingContent(false);
    }

    return {
        content,
        loadingContent,
        fetchContent
    }
}