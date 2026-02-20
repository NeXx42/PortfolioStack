import { useState } from "react";
import { useAuth } from "../hooks/useUser"

import "./navbar.css"
import AuthenticationModal from "./authenticationModal";
import UserPopupModal from "./userPopupModal";

export default function Navbar() {
    const [authModal, setAuthModal] = useState(false);

    const { authenticatedUser: user, loading: userLoading } = useAuth();

    return (
        <>
            {
                authModal && (
                    user === undefined ? (<AuthenticationModal />) :
                        (<UserPopupModal />)
                )
            }
            <div className="Component_Navbar">
                <button onClick={() => setAuthModal(!authModal)}>{user?.displayName ?? "Login"}</button>
            </div>
        </>
    )
}