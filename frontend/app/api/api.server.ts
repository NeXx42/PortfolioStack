"use server"

import type { Project, Link } from "@shared/types";

import { get } from "./api.shared";

export async function fetchFeaturedContent(): Promise<Project[]> {
    return (await get("content/featured", {
        revalidate: 10
    }))!;
}

export async function fetchGame(gameId: string): Promise<Project> {
    return (await get(`content/${gameId}`, {
        revalidate: 10
    }))!;
}

export async function fetchLinks(): Promise<Link[]> {
    return (await get("content/links", {
        revalidate: 10
    }))!;
}