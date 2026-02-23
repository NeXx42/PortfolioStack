import { useEffect, useState } from "react";

import * as api from "../api/api"

import type { Item, Link } from "../types";
import { ProjectType } from "../enums";

interface HookReturn {
    featured: Item[];
    loadingFeatured: boolean,

    links: Link[],
    loadingLinks: boolean,

    content: Record<ProjectType, Item[]>,
    loadingContent: boolean,
    fetchContent: (type: ProjectType) => Promise<void>,
}


export function useContent(): HookReturn {
    const [featured, setFeatured] = useState<Item[]>([]);
    const [loadingFeatured, setLoadingFeatured] = useState(true);

    const [links, setLinks] = useState<Link[]>([]);
    const [loadingLinks, setLoadingLinks] = useState(true);

    useEffect(() => {
        setLoadingFeatured(true);
        setLoadingLinks(true);

        api.fetchFeaturedContent()
            .then(setFeatured)
            .finally(() => setLoadingFeatured(false));

        api.fetchLinks()
            .then(setLinks)
            .finally(() => setLoadingLinks(false));
    }, [])


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
        featured,
        loadingFeatured,

        links,
        loadingLinks,

        content,
        loadingContent,
        fetchContent
    }
}