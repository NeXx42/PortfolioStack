import * as api from "@api/api.client"

export default function () {


    return (
        <>
            <div className="Admin_Controls">
                <button onClick={() => api.admin_ClearCache()} >Clear Cache</button>
            </div>
        </>
    )
}