import "./adminPage_Users.css"

import * as api from "@api/api.client"

import useRequest from "../hooks/useRequest";
import { useState } from "react";
import { UserSummary } from "../shared/types";
import CommonButton from "../shared/components/commonButton";

const roleNames: string[] = ["Default", "Admin"]

export default function () {
    const { data } = useRequest(api => api.admin_GetUsers());

    const [editing, setEditing] = useState<UserSummary | undefined>(undefined);

    const updateUser = <K extends keyof UserSummary>(prop: K, value: UserSummary[K]) => {
        setEditing(prev => {
            if (prev === undefined) return prev;
            return {
                ...prev,
                [prop]: value
            }
        })
    }

    const saveUser = () => {
        if (editing === undefined)
            return;

        api.admin_UpdateUser(editing)
            .then(_ => window.location.reload())
            .catch(e => alert(e.message));
    }

    return (<div className="adminPage_Content">
        <table>
            <thead>
                <tr>
                    <th>
                        Id
                    </th>
                    <th>
                        Name
                    </th>
                    <th>
                        Email
                    </th>
                    <th>
                        Role
                    </th>
                </tr>
            </thead>
            <tbody>
                {
                    data?.map(d => (
                        <tr key={d.id}>
                            <td>{d.id}</td>
                            <td>{d.displayName}</td>
                            <td>{d.email}</td>
                            <td>{roleNames[d.role ?? 0]}</td>
                            <td><button onClick={() => setEditing({ ...d })}>Edit</button></td>
                        </tr>
                    ))
                }
            </tbody>
        </table>
        {
            editing && (
                <div className="adminPage_Users_EditContainer" onClick={() => setEditing(undefined)}>
                    <div className="adminPage_Users_Edit" onClick={e => e.stopPropagation()}>
                        <h1>User Management</h1>
                        <header>
                            Display name
                            <input value={editing.displayName} onChange={e => updateUser("displayName", e.target.value)}></input>
                        </header>
                        <header>
                            Email
                            <input value={editing.email} onChange={e => updateUser("email", e.target.value)}></input>
                        </header>
                        <header>
                            Role
                            <select value={editing.role} onChange={e => updateUser("role", Number(e.target.value))}>
                                {roleNames.map((r, i) => <option key={r} value={i}>{r}</option>)}
                            </select>
                        </header>
                        <CommonButton label="Save" onClick={saveUser} />
                    </div>
                </div>
            )
        }
    </div>)
}