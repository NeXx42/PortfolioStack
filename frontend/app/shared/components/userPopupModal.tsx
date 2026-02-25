"use client"

import { useAuth } from "@hooks/useUser"
import { UserRoles } from "@shared/enums";
import CommonButton from "@shared/components/commonButton";

import "./userPopupModal.css"

export default function UserPopupModal() {
    const { authenticatedUser, logout } = useAuth();
    //const navigate = useNavigate();

    const goToControlPanel = () => {
        //navigate("/admin");
    }

    const onLogout = async () => {
        await logout();
        window.location.reload();
    }

    return (<div className="Component_UserPopup">
        {authenticatedUser?.role === UserRoles.Admin && (
            <CommonButton onClick={goToControlPanel} label="Admin" />
        )}

        <CommonButton onClick={onLogout} label="Logout" />
    </div>)
}