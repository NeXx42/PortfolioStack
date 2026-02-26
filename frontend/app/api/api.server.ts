"use server"

import type { Item, Link } from "@shared/types";

import { get } from "./api.shared";

export async function fetchFeaturedContent(): Promise<Item[]> {
    return (await get("content/featured"))!;
}

export async function fetchGame(gameId: string): Promise<Item> {
    return (await get(`content/${gameId}`))!;
}

export async function fetchLinks(): Promise<Link[]> {
    return (await get("content/links"))!;
}