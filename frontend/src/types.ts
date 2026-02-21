export interface User {
    userId: string,
    displayName: string
}

export interface Item {
    name: string,
    iconUrl: string,
    description: string,
    price?: number,
    actionName: string
}