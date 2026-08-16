import * as api from "@api/api.client"

export default function () {
    const wrapFunction = (func: () => Promise<any>) => {
        func()
            .then(() => window.location.reload())
            .catch(e => alert(e.message));
    }

    return (
        <>
            <div className="Admin_Controls">
                <button onClick={() => wrapFunction(api.admin_ClearCache)} >Clear Cache</button>
            </div>
        </>
    )
}