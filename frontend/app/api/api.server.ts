"use server"

import type { Item, Link } from "@shared/types";

import * as api from "./api.shared";
import { SERVER_URL } from "@shared/config";

// -------------------- shared

async function get<T>(uri: string): Promise<T> {
    return (await api.get<T>(SERVER_URL, uri))!;
}

async function post<T>(uri: string, obj?: any): Promise<T> {
    return (await api.post<T>(SERVER_URL, uri, obj))!;
}


// -------------------- Content

export async function fetchFeaturedContent(): Promise<Item[]> {
    return await get("content/featured");
}

export async function fetchGame(gameId: string): Promise<Item> {
    return await get(`content/${gameId}`);
}

export async function fetchLinks(): Promise<Link[]> {
    return await get("content/links");
}