import { useAdmin } from "../hooks/userAdmin";
import CommonButton from "../shared/components/commonButton";

export default function () {
    const {
        clearCache,
    } = useAdmin()

    return (
        <>
            <div className="Admin_Controls">
                <button onClick={() => clearCache()} >Clear Cache</button>
            </div>
        </>
    )
}