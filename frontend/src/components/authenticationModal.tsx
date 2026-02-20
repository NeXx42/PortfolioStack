import { useState } from "react"
import "./authenticationModal.css"
import { useAuth } from "../hooks/useUser";

export default function AuthenticationModal() {
    const [email, setEmail] = useState<string>("");
    const [displayName, setDisplayName] = useState<string>("");
    const [password, setPassword] = useState<string>("");

    const [loginTab, setLoginTab] = useState(true);
    const { login, signup, loading } = useAuth();

    const handleLogin = async () => {
        if (email === "" || password === "") {
            return;
        }

        await login(email, password);
    };

    const handleSignup = async () => {
        if (email === "" || password === "" || displayName == "") {
            return;
        }

        await signup(email, displayName, password);
    };

    return (
        <div className="Component_AuthenticationModal">
            <button className="Component_AuthenticationModal_close">✕</button>

            <div className="Component_AuthenticationModal_title">Welcome back</div>
            <p className="Component_AuthenticationModal_sub">Sign in or create a new account</p>

            <div className="Component_AuthenticationModal_tabs">
                <button className="Component_AuthenticationModal_tab active" onClick={() => setLoginTab(true)}>Sign in</button>
                <button className="Component_AuthenticationModal_tab" onClick={() => setLoginTab(false)}>Sign up</button>
            </div>

            {
                loginTab ? (
                    <div id="Component_AuthenticationModal_signin">
                        <div className="ig"><label>Email</label><input type="email" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="you@example.com" /></div>
                        <div className="ig"><label>Password</label><input type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="••••••••" /></div>

                        <button className="btn-modal" onClick={handleLogin}>Sign in</button>
                        <div className="modal-foot"><a href="#">Forgot password?</a></div>
                    </div>
                ) : (
                    <div id="Component_AuthenticationModal_signup">
                        <div className="ig"><label>Name</label><input type="text" value={displayName} onChange={(e) => setDisplayName(e.target.value)} placeholder="Your name" /></div>
                        <div className="ig"><label>Email</label><input type="email" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="you@example.com" /></div>
                        <div className="ig"><label>Password</label><input type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Choose a password" /></div>

                        <button className="btn-modal" onClick={handleSignup}>Create account</button>
                        <div className="modal-foot">No spam. Just update emails when things ship.</div>
                    </div>
                )
            }
        </div>
    )
}