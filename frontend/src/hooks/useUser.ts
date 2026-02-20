import { useEffect, useState } from "react";
import * as api from "../api/api";

import type { User } from "../types";

interface HookReturn {
    authenticatedUser: User | undefined,
    loading: boolean,
    error: string | undefined,
    login: (email: string, password: string) => Promise<void>,
    signup: (email: string, displayName: string, password: string) => Promise<void>,
}

export function useAuth(): HookReturn {
    const [user, setUser] = useState<User | undefined>(undefined);

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(undefined);

    useEffect(() => {
        api.getLoggedInUser()
            .then(u => setUser(u))
            .catch(e => setError(e.message))
            .finally(() => setLoading(false));
    }, []);

    async function login(email: string, password: string) {
        setLoading(true);

        try {
            const u = await api.login(email, password);
            setUser(u);
        }
        catch (err: any) {
            setError(err.message || "login failed");
        }
        finally {
            setLoading(false);
        }
    }

    async function signup(email: string, displayName: string, password: string) {
        setLoading(true);

        try {
            const u = await api.signup(email, displayName, password);
            setUser(u);
        }
        catch (err: any) {
            setError(err.message || "login failed");
        }
        finally {
            setLoading(false);
        }
    }


    return {
        authenticatedUser: user,
        loading: loading,
        error: error,
        login,
        signup
    }
}