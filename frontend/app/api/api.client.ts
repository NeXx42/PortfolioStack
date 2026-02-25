"use client"

import type { Item, Link, User } from "@shared/types";
import type { ProjectType } from "@shared/enums";

import { BASE_URL, DEBUG_SLOW_API } from "@shared/config";

// -------------------- shared

async function get<T>(uri: string): Promise<T> {
    const res = await fetch(`${BASE_URL}/api/${uri}`, {
        method: "GET",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
    });

    if (DEBUG_SLOW_API === true)
        await new Promise(res => setTimeout(res, 5000));

    if (!res.ok) {
        throw await handleException(res);
    }

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

    if (!res.ok) {
        throw await handleException(res);
    }

    return res.json();
}

async function handleException(res: Response): Promise<Error> {
    try {
        const errorData = await res.json();
        return new Error(errorData.message || JSON.stringify(errorData));
    } catch (_) {
        return new Error(`Request failed with status ${res.status}`)
    }
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

export async function fetchLinks(): Promise<Link[]> {
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

export async function admin_UploadImage(img: FormData): Promise<string> {
    const res = await fetch(`${BASE_URL}/api/admin/upload`, {
        method: "POST",
        body: img,
        credentials: "include",
    });

    if (!res.ok) {
        throw await handleException(res);
    }

    return await res.json();
}

export async function admin_GetImages(): Promise<string[]> {
    return await get("admin/images");
}