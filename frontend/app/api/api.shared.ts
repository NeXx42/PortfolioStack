import { SERVER_URL, BASE_URL, DEBUG_SLOW_API } from "@shared/config";

const URL = typeof window === "undefined"
    ? SERVER_URL
    : BASE_URL;

export async function get<T>(uri: string): Promise<T | undefined> {
    const res = await fetch(`${URL}/api/${uri}`, {
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

    if (res.status === 204 || res.headers.get("content-length") === "0") {
        return undefined;
    }

    return res.json();
}

export async function post<T>(uri: string, obj?: any): Promise<T | undefined> {
    const res = await fetch(`${URL}/api/${uri}`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: obj ? JSON.stringify(obj) : "",
    });


    if (DEBUG_SLOW_API === true)
        await new Promise(res => setTimeout(res, 5000));

    if (!res.ok) {
        throw await handleException(res);
    }

    if (res.status === 204 || res.headers.get("content-length") === "0") {
        return undefined;
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
