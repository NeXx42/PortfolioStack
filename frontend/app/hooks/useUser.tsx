"use client"

import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import * as api from "@api/api.client";

import type { User } from "@shared/types";


type AuthContextType = {
    authenticatedUser: User | undefined,
    loading: boolean,
    error: string | undefined,
    login: (email: string, password: string) => Promise<boolean>,
    signup: (email: string, displayName: string, password: string, emailCode: number) => Promise<boolean>,
    logout: () => Promise<void>,
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<User | undefined>(undefined);

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(undefined);

    useEffect(() => {
        api.getLoggedInUser()
            .then(u => setUser(u))
            .finally(() => setLoading(false));
    }, []);

    async function login(email: string, password: string) {
        setError(undefined);
        setLoading(true);

        try {
            const u = await api.login(email, password);
            setUser(u);

            return true;
        }
        catch (err: any) {
            setError(err.message || "login failed");
        }
        finally {
            setLoading(false);
            return false;
        }
    }

    async function signup(email: string, displayName: string, password: string, emailCode: number) {
        setError(undefined);
        setLoading(true);

        try {
            const u = await api.signup(email, displayName, password, emailCode);
            setUser(u);

            return true;
        }
        catch (err: any) {
            setError(err.message || "login failed");
        }
        finally {
            setLoading(false);
            return false;
        }
    }

    async function logout() {
        setError(undefined);
        setLoading(true);

        try {
            await api.logout();
        }
        catch (err: any) {
            setError(err.Message);
        }
        finally {
            setLoading(false);
        }
    }

    return (
        <AuthContext.Provider
            value={{
                authenticatedUser: user,
                loading,
                error,
                login,
                signup,
                logout,
            }}>
            {children}
        </AuthContext.Provider>
    )
}

export function useAuth(): AuthContextType {
    const ctx = useContext(AuthContext);

    if (!ctx)
        throw new Error("useAuth must be inside AuthProvider");

    return ctx;
}