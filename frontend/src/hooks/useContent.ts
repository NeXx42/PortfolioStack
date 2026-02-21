import { useState } from "react";
import type { Item } from "../types";
import { ProjectType } from "../enums";

interface HookReturn {
    loading: boolean,

    games?: Item[],
    software?: Item[],
    assets?: Item[],
    projects?: Item[]

    fetchContent: (type: ProjectType) => Promise<void>
}


export function useContent(): HookReturn {
    const [games, setGames] = useState<Item[] | undefined>(undefined);
    const [software, setSoftware] = useState<Item[] | undefined>(undefined);
    const [assets, setAssets] = useState<Item[] | undefined>(undefined);
    const [projects, setProjects] = useState<Item[] | undefined>(undefined);

    const [loading, setLoading] = useState<boolean>(true);

    const fetchContent = async (type: ProjectType) => {
        setLoading(true);

        switch (type) {
            case ProjectType.Game:
                setGames([{ name: "yo", iconUrl: "test", price: 2, description: "test", actionName: "test2" }])
                break;

            case ProjectType.Asset:
                setAssets([
                    { name: "yo", iconUrl: "test", price: 2, description: "test", actionName: "test2" },
                    { name: "yo", iconUrl: "test", price: 2, description: "test", actionName: "test2" },
                ])
                break;

            case ProjectType.Project:
                setProjects([{ name: "yo", iconUrl: "test", price: 2, description: "test", actionName: "test2" }])
                break;

            case ProjectType.Software:
                setSoftware([{ name: "yo", iconUrl: "test", price: 2, description: "test", actionName: "test2" }])
                break;
        }

        setLoading(false);
    }

    return {
        loading,

        games,
        software,
        assets,
        projects,

        fetchContent
    }
}