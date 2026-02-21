import { useEffect, useRef, useState } from "react";
import { useAuth } from "../hooks/useUser"

import "./navbar.css"
import AuthenticationModal from "./authenticationModal";
import UserPopupModal from "./userPopupModal";
import CommonButton from "./commonButton";

export default function Navbar() {
    const [authModal, setAuthModal] = useState(false);
    const popupRef = useRef<HTMLDivElement>(null);

    const { authenticatedUser: user, loading: userLoading } = useAuth();

    useEffect(() => {
        function handleClickOutside(event: any) {
            if (popupRef.current && !popupRef.current.contains(event.target)) {
                setAuthModal(false);
            }
        }

        if (authModal) {
            document.addEventListener("mousedown", handleClickOutside);
        } else {
            document.removeEventListener("mousedown", handleClickOutside);
        }

        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, [authModal]);

    return (
        <>
            {
                authModal && user === undefined && (<AuthenticationModal onExit={() => setAuthModal(false)} />)
            }
            <div className="Component_Navbar">
                <div className="Component_Navbar_Login">
                    <a>NEXX</a>
                </div>

                <div className="Component_Navbar_Right">
                    <div className="Component_Navbar_Links">
                        <a>Shop</a>
                        <a>Links</a>
                        <a>About</a>
                    </div>
                    <div className="Component_Navbar_UserDetails">
                        <CommonButton label={user?.displayName ?? "Login"} onClick={() => setAuthModal(!authModal)} />
                        {authModal && user !== undefined && (
                            <div ref={popupRef}>
                                <UserPopupModal />
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </>
    )
}