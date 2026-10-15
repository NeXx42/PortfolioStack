import type { ProjectContentType, ProjectType, UserRoles } from "./enums"

export const releaseStatus = ["Published", "Prerelease", "Unpublished"];
export type ReleaseStatus = typeof releaseStatus[number];

export interface User {
    userId: string,
    displayName: string
    role: number
}

export interface Project {
    id: string,
    gameName: string,
    slug: string,

    icon: string,
    shortDescription: string,
    genre: string,

    dateCreated?: number,
    dateUpdated?: number,

    status: number,
    type: ProjectType,

    elements?: ProjectContent[],
    tags?: ProjectTag[]
    releases: ProjectRelease[]
}

export interface ProjectContent {
    id: number,
    type: ProjectContentType,
    order: number,

    elements?: ProjectContentParam[]
}

export interface ProjectContentParam {
    id: number,
    order: number,

    value1: string,
    value2: string,
    value3: string,
}

export interface ProjectTag {
    id: number,
    name: string,
    customColour: string,
}

export interface ProjectRelease {
    versionId: number,
    version: string,

    status?: number,
    patchNotes?: string,

    downloads: ProjectReleaseDownload[]
}

export interface ProjectReleaseDownload {
    platform: string,

    downloadLink: string
    releaseEngineManifestLink?: string

    entryPoint?: string
    size: number
}




export interface Link {
    name: string,
    url: string,

    customColour?: string,
    icon?: string
}

export interface ServerFile {
    id: string,
    fileName: string,
    size: number
}

export interface UserSummary {
    id: string;
    displayName: string;
    email: string;
    role: number;
}

export interface ProjectEntitlement {
    key: string;
    value?: string;
    type: number;
}

export interface UserEntitlements {
    key: string;
    value: string | undefined;
}