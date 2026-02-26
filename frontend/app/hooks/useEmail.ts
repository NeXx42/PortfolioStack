import { useEffect, useState } from "react";

import { auth_Email_Confirm, auth_Email_Verify } from "@api/api.client"
import { Fascinate } from "next/font/google";

interface HookReturn {
    sentCode: boolean,
    loading: boolean,
    error?: string,

    sendValidation: (emailAddress: string) => void,
}


export function useEmail(): HookReturn {
    const [sentCode, setSentCode] = useState(false)

    const [error, setError] = useState<string | undefined>(undefined);
    const [loading, setLoading] = useState(false);

    const sendValidation = (emailAddress: string) => {
        setLoading(true);
        setError(undefined);
        setSentCode(false);

        auth_Email_Verify(emailAddress)
            .then(() => {
                setSentCode(true);
            })
            .catch((e) => {
                setError(e.message);
            })
            .finally(() => setLoading(false))
    }

    return {
        sentCode,
        loading,
        error,

        sendValidation
    }
}