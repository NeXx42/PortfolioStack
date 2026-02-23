import type { Item, ItemContent, User } from "../types";
import { BASE_URL } from "../config";
import type { ProjectType } from "../enums";

// -------------------- shared

async function get<T>(uri: string): Promise<T> {
    const res = await fetch(`${BASE_URL}/api/${uri}`, {
        method: "GET",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
    });

    if (!res.ok)
        throw new Error("err");

    return res.json();
}

async function post<T>(uri: string, obj?: any): Promise<T> {
    const res = await fetch(`${BASE_URL}/api/${uri}`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: obj ? JSON.stringify(obj) : "",
    });

    if (!res.ok)
        throw new Error("err");

    return res.json();
}


// -------------------- authentication

export async function login(email: string, password: string): Promise<User> {
    return await post<User>("auth/login", { email, password });
}

export async function signup(email: string, displayName: string, password: string): Promise<User> {
    return await post<User>("auth/signup", { email, displayName, password });
}

export async function getLoggedInUser(): Promise<User> {
    return get("auth/profile");
}

export async function logout() {
    await post("auth/logout");
}


// -------------------- Content

export async function fetchContent(type: ProjectType): Promise<Item[]> {
    return await get(`content?type=${type}`);
}

export async function fetchFeaturedContent(): Promise<Item[]> {
    return await get("content/featured");
}

export async function fetchGame(gameId: string): Promise<Item> {
    return await get(`content/${gameId}`);
}

// remove me
export async function updatePage(slug: string, newPages?: ItemContent[], updates?: ItemContent[], deletions?: number[]) {
    const res = await fetch(`${BASE_URL}/api/content/${slug}`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ newPages, updates, deletions }),
    });

    if (!res.ok)
        throw new Error("Failed to update game");
}

export async function fetchLinks() {
    return await get("content/links");
}


// -------------------- admin


export async function admin_Slugs(): Promise<string[]> {
    return await get("admin/slugs");
}

export async function admin_ClearCache(): Promise<void> {
    await get("admin/clearCache");
}

export async function admin_SaveItem(item: Item): Promise<void> {
    await post("admin/save", item)
}