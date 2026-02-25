"use client"

import React, { useState } from "react"
import "./authenticationModal.css"
import { useAuth } from "@hooks/useUser";
import { createPortal } from "react-dom";
import CommonButton from "./commonButton";

interface Props {
    onExit: () => void
}

export default function AuthenticationModal(props: Props) {
    const [email, setEmail] = useState<string>("");
    const [displayName, setDisplayName] = useState<string>("");
    const [password, setPassword] = useState<string>("");

    const [loginTab, setLoginTab] = useState(true);
    const { login, signup, error } = useAuth();

    const handleLogin = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        if (await login(email, password))
            props.onExit();
    };

    const handleSignup = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        if (await signup(email, displayName, password))
            props.onExit();
    };

    return createPortal(
        <div className="ModalContainer" onClick={() => props.onExit()}>
            <div className="Component_AuthenticationModal" onClick={e => e.stopPropagation()}>
                <button className="Component_AuthenticationModal_close" onClick={() => props.onExit()}>✕</button>

                <div className="Component_AuthenticationModal_title">Login</div>
                <p className="Component_AuthenticationModal_sub">Sign in or create a new account</p>

                <div className="Component_AuthenticationModal_tabs">
                    <button className={`Component_AuthenticationModal_tab ${loginTab ? "active" : ""}`} onClick={() => setLoginTab(true)}>Sign in</button>
                    <button className={`Component_AuthenticationModal_tab ${!loginTab ? "active" : ""}`} onClick={() => setLoginTab(false)}>Sign up</button>
                </div>

                {
                    loginTab ? (
                        <form onSubmit={handleLogin} id="Component_AuthenticationModal_signin" className="Component_AuthenticationModal_Form">
                            <div className="ig">
                                <label>Email</label>
                                <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="you@example.com" required />
                            </div>
                            <div className="ig">
                                <label>Password</label>
                                <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="••••••••" required />
                            </div>

                            <label>{error}</label>
                            <CommonButton label="Login" />
                        </form>
                    ) : (
                        <form onSubmit={handleSignup} id="Component_AuthenticationModal_signup" className="Component_AuthenticationModal_Form">
                            <div className="ig">
                                <label>Name</label>
                                <input type="text" value={displayName} onChange={(e) => setDisplayName(e.target.value)} placeholder="Your name" required />
                            </div>
                            <div className="ig">
                                <label>Email</label>
                                <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="you@example.com" required />
                            </div>
                            <div className="ig">
                                <label>Password</label>
                                <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Choose a password" required />
                            </div>

                            <label>{error}</label>
                            <CommonButton label="Signup" />
                        </form>
                    )
                }
            </div>
        </div>
        , document.body)
}