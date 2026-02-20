import { useState } from "react";
import { useAuth } from "../hooks/useUser"

import "./navbar.css"
import AuthenticationModal from "./authenticationModal";

export default function Navbar() {
    const [authModal, setAuthModal] = useState(false);

    const { authenticatedUser: user, loading: userLoading } = useAuth();

    return (
        <>
            {
                authModal && (
                    <AuthenticationModal />
                )
            }
            <div className="Component_Navbar">
                <button onClick={() => setAuthModal(true)}>{user?.displayName ?? "Login"}</button>
            </div>
        </>
    )
}