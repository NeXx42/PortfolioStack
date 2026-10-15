"use client"

import "./userPopupModal.css"

import { useAuth } from "@hooks/useUser"
import { UserRoles } from "@shared/enums";
import CommonButton from "@shared/components/commonButton";

import { useRouter } from "next/navigation";

export default function UserPopupModal() {
    const { authenticatedUser, logout } = useAuth();
    const navigate = useRouter();

    const goToControlPanel = () => {
        navigate.push("/admin");
    }

    const onLogout = async () => {
        await logout();
        window.location.reload();
    }

    return (<div className="Component_UserPopup">
        {UserRoles[authenticatedUser?.role ?? 0] === "Admin" && (
            <CommonButton onClick={goToControlPanel} label="Admin" />
        )}

        <CommonButton onClick={onLogout} label="Logout" />
    </div>)
}