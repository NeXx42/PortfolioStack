"use client"

import type { Project, User, ProjectTag, ProjectContent, ProjectRelease } from "@shared/types";
import type { ProjectType } from "@shared/enums";

import { post, get, postForm, URL } from "./api.shared";

// -------------------- authentication

export async function login(email: string, password: string): Promise<User> {
    return (await post<User>("user/login", { email, password }))!;
}

export async function signup(email: string, displayName: string, password: string, emailCode: number): Promise<User> {
    return (await post<User>("user/signup", { email, displayName, password, emailVerification: emailCode }))!;
}

export async function getLoggedInUser(): Promise<User> {
    return (await get("user/profile"))!;
}

export async function logout() {
    await post("user/logout");
}

export async function auth_Email_Verify(emailAddress: string): Promise<void> {
    await post("user/email/verification", { address: emailAddress });
}

export async function auth_Email_Confirm(emailAddress: string, code: number): Promise<boolean> {
    return (await post("user/email/confirmation", { address: emailAddress, code: code }))!;
}

// -------------------- Content

export async function fetchContent(type: ProjectType): Promise<Project[]> {
    return (await get(`content?type=${type}`))!;
}

export async function fetchGame(gameId: string): Promise<Project> {
    return (await get(`content/${gameId}`))!;
}


// -------------------- admin

export async function admin_ClearCache(): Promise<void> {
    await get("admin/clearCache");
}

export async function admin_GetTags(): Promise<ProjectTag[]> {
    return (await get("admin/tags"))!;
}

export async function admin_SaveTags(tags: ProjectTag[]) {
    await post("admin/tags", tags);
}

// rewrite

export async function admin_GetProject(id: string): Promise<Project> {
    return (await get(`admin/${id}`))!;
}

export async function admin_CreateProject(): Promise<string> {
    return (await post("admin/project/create"))!;
}

export async function admin_GetProjects(): Promise<Project[]> {
    return (await get("admin/projects"))!
}

export async function admin_SaveProject(form: FormData): Promise<string> {
    return (await postForm("admin/project/save", form))!
}

export async function admin_SaveContent(projectId: string, form: FormData) {
    return (await postForm(`admin/project/${projectId}/save`, form))!
}

export async function admin_GetProjectReleases(projectId: string): Promise<ProjectRelease[]> {
    return (await get(`admin/project/${projectId}/releases`))!
}

export async function admin_SaveProjectRelease(projectId: string, release: ProjectRelease) {
    return (await post(`admin/project/${projectId}/release`, release))!
}

export async function admin_PrimeReleaseEngineUpload(projectId: string, releaseId: number, platform: string): Promise<string> {
    return (await post<string>(`admin/project/${projectId}/release/${releaseId}?platform=${platform}`))!
}