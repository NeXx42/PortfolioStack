import { useAuth } from "../hooks/useUser"
import CommonButton from "./commonButton";

import "./userPopupModal.css"

export default function UserPopupModal() {
    const { logout } = useAuth();

    return (<div className="Component_UserPopup">
        <CommonButton onClick={logout} label="Logout" />
    </div>)
}