"use client"

import * as localApi from "@api/api.client"
import { useEffect, useState } from "react";

export default function <T>(req?: (api: typeof localApi) => Promise<T>) {
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | undefined>(undefined);
    const [data, setData] = useState<T | undefined>(undefined);

    useEffect(() => {
        if (req === undefined)
            return;

        sendRequest(req);
    }, [])

    async function sendRequest(req: (api: typeof localApi) => Promise<T>) {
        setLoading(true);

        try {
            setLoading(true);
            setError(undefined);

            const dat: T = await req(localApi);
            setData(dat);
        }
        catch (e: any) {
            setError(e.Message);
        }
        finally {
            setLoading(false);
        }
    }

    return {
        loading,
        error,
        data,

        sendRequest
    }
}