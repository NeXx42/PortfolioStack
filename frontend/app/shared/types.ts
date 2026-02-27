import type { ProjectContentType, ProjectType, UserRoles } from "./enums"

export interface User {
    userId: string,
    displayName: string
    role: UserRoles
}

export interface Project {
    id: string,
    gameName: string,
    slug: string,
    icon: string,
    shortDescription: string,

    cost?: number,
    version?: string,

    dateCreated?: Date,
    dateUpdate?: Date,

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
    id: number,
    version: string,
    size: string,
    date: Date,

    downloads: ProjectReleaseDownload[]
}

export interface ProjectReleaseDownload {
    id: number,
    link: string
}




export interface Link {
    name: string,
    url: string,

    customColour?: string,
    icon?: string
}