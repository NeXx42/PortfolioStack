import type { User } from "../types";
import { BASE_URL } from "../config";

export async function login(email: string, password: string): Promise<User> {
    const res = await fetch(`${BASE_URL}/api/auth/login`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ email, password }),
    });

    if (!res.ok)
        throw new Error("Login failed");

    return res.json();
}

export async function signup(email: string, displayName: string, password: string): Promise<User> {
    const res = await fetch(`${BASE_URL}/api/auth/signup`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ email, displayName, password }),
    });

    if (!res.ok)
        throw new Error("Signup failed");

    return res.json();
}

export async function getLoggedInUser(): Promise<User> {
    const res = await fetch(`${BASE_URL}/api/auth/profile`, {
        credentials: "include"
    });

    if (!res.ok)
        throw new Error("Failed to fetch user");

    return res.json();
}

export async function logout() {
    const res = await fetch(`${BASE_URL}/api/auth/logout`, {
        method: "POST",
        credentials: "include"
    });

    if (!res.ok)
        throw new Error("Failed to logout");
}