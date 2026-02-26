"use client"

import type { Item, Link, User } from "@shared/types";
import type { ProjectType } from "@shared/enums";

import { post, get } from "./api.shared";

// -------------------- authentication

export async function login(email: string, password: string): Promise<User> {
    return (await post<User>("auth/login", { email, password }))!;
}

export async function signup(email: string, displayName: string, password: string, emailCode: number): Promise<User> {
    return (await post<User>("auth/signup", { email, displayName, password, emailVerification: emailCode }))!;
}

export async function getLoggedInUser(): Promise<User> {
    return (await get("auth/profile"))!;
}

export async function logout() {
    await post("auth/logout");
}

export async function auth_Email_Verify(emailAddress: string): Promise<void> {
    await post("auth/email/verification", { address: emailAddress });
}

export async function auth_Email_Confirm(emailAddress: string, code: number): Promise<boolean> {
    return (await post("auth/email/confirmation", { address: emailAddress, code: code }))!;
}

// -------------------- Content

export async function fetchContent(type: ProjectType): Promise<Item[]> {
    return (await get(`content?type=${type}`))!;
}

export async function fetchGame(gameId: string): Promise<Item> {
    return (await get(`content/${gameId}`))!;
}


// -------------------- admin


export async function admin_Slugs(): Promise<string[]> {
    return (await get("admin/slugs"))!;
}

export async function admin_ClearCache(): Promise<void> {
    await get("admin/clearCache");
}

export async function admin_SaveItem(item: Item): Promise<void> {
    await post("admin/save", item)
}

export async function admin_UploadImage(img: FormData): Promise<string> {
    return (await post("admin/upload", img))!;
}

export async function admin_GetImages(): Promise<string[]> {
    return (await get("admin/images"))!;
}