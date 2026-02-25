import type { ProjectContentType, ProjectType, UserRoles } from "./enums"

export interface User {
    userId: string,
    displayName: string
    role: UserRoles
}

export interface Item {
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

    elements?: ItemContent[],
    tags?: ItemTag[]
    releases: ItemRelease[]
}

export interface ItemContent {
    id: number,
    type: ProjectContentType,
    order: number,

    elements?: ItemContentParameter[]
}

export interface ItemContentParameter {
    id: number,
    order: number,

    value1: string,
    value2: string,
    value3: string,
}

export interface ItemTag {
    name: string,
    customColour: string,
}

export interface ItemRelease {
    id: number,
    version: string,
    size: string,
    date: Date,

    downloads: ItemReleaseDownload[]
}

export interface ItemReleaseDownload {
    id: number,
    link: string
}

export interface Link {
    name: string,
    url: string,

    customColour?: string,
    icon?: string
}