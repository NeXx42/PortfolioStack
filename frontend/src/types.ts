export interface User {
    userId: string,
    displayName: string
}

export interface Item {
    id: string,
    name: string,
    icon: string,
    description: string,
    price?: number,
    actionName: string
}