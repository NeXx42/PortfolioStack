import { useAuth } from "../hooks/useUser"
import "./userPopupModal.css"

export default function UserPopupModal() {
    const { logout } = useAuth();

    return (<>
        <button onClick={logout}>Logout</button>
    </>)
}