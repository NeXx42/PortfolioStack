import { useNavigate } from "react-router-dom";
import { UserRoles } from "../enums";
import { useAuth } from "../hooks/useUser"
import CommonButton from "./commonButton";

import "./userPopupModal.css"

export default function UserPopupModal() {
    const { authenticatedUser, logout } = useAuth();
    const navigate = useNavigate();

    const goToControlPanel = () => {
        navigate("/admin");
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