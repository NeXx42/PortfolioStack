export enum ProjectType {
    Game,
    Software,
    Asset,
    Project
}

export enum ProjectContentType {
    Screenshots = 0,
    About = 1,
    Features = 2,
    Requirements = 3,
    Releases = 4,
    LauncherMetadata = 5,
}


export type UserRole = "None" | "Admin";
export const UserRoles: UserRole[] = ["None", "Admin"]

export type EntitlementKey = "Owned" | "Early Access";
export const EntitlementKeys: EntitlementKey[] = ["Owned", "Early Access"]

export type EntitlementType = "Default" | "AutoGrantedGuid" | "HardwareKeyed"
export const EntitlementTypes: EntitlementType[] = ["Default", "AutoGrantedGuid", "HardwareKeyed"]