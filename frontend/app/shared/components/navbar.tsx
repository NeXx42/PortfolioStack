"use client"

import { useRouter } from "next/navigation";

import { useEffect, useRef, useState } from "react";
import { useAuth } from "@hooks/useUser"

import "./navbar.css"
import AuthenticationModal from "./authenticationModal";
import UserPopupModal from "./userPopupModal";

export default function Navbar() {
    const router = useRouter();

    const [authModal, setAuthModal] = useState(false);
    const popupRef = useRef<HTMLDivElement>(null);

    const { authenticatedUser: user } = useAuth();

    const selectPage = (fragment: string | undefined = undefined) => {
        router.push(fragment ? `/#${fragment}` : "/");
    }

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
                <div className="Component_Navbar_Alias">
                    <a onClick={() => selectPage()}>NEXX</a>
                </div>

                <div className="Component_Navbar_Right">
                    <div className="Component_Navbar_Links">
                        <a onClick={() => selectPage("projects")}>Shop</a>
                        <a onClick={() => selectPage("links")}>Links</a>
                        <a onClick={() => selectPage("about")}>About</a>
                    </div>
                    <div className="Component_Navbar_UserDetails">
                        <button onClick={() => setAuthModal(!authModal)} >{user?.displayName ?? "Login"}</button>
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