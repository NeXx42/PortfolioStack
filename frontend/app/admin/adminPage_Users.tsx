import "./adminPage_Users.css"

import * as api from "@api/api.client"

import useRequest from "../hooks/useRequest";
import { useEffect, useState } from "react";
import { UserEntitlements, UserSummary } from "../shared/types";
import CommonButton from "../shared/components/commonButton";
import { EntitlementKey, UserRoles } from "../shared/enums";

interface EditingUserInfo {
    userInfo: UserSummary;
    selectedProjectId: string | undefined;
    userEntitlements: EditingUserEntitlements[];
}

interface EditingUserEntitlements {
    entitlementKey: string;
    userValue: string | null;
}

export default function () {
    const { data } = useRequest(api => api.admin_GetUsers());
    const { data: projects } = useRequest(api => api.admin_GetProjects());

    const [editing, setEditing] = useState<EditingUserInfo | undefined>(undefined);

    const startEditing = (usr: UserSummary) => {
        setEditing({
            userInfo: usr,
            selectedProjectId: undefined,
            userEntitlements: [],
        })
    }

    useEffect(() => {
        if (editing === undefined)
            return;

        setEditing(prev => ({ ...prev!, userEntitlements: [] }))

        if (editing.selectedProjectId) {
            api.admin_GetEntitlements(editing.selectedProjectId!)
                .then(res => {
                    api.admin_GetUserEntitlements(editing.selectedProjectId!, editing.userInfo.id)
                        .then(usrDat => {
                            setEditing(prev => ({
                                ...prev!,
                                userEntitlements: res.map(r => ({
                                    entitlementKey: r.key,
                                    userValue: usrDat?.filter(ud => ud.key === r.key)?.[0]?.value ?? null
                                }))
                            }));
                        })
                        .catch(e => alert(e.message))
                })
                .catch()
        }

    }, [editing?.selectedProjectId])

    const updateUser = <K extends keyof UserSummary>(prop: K, value: UserSummary[K]) => {
        setEditing(prev => {
            if (prev === undefined) return prev;
            return {
                ...prev,
                userInfo: {
                    ...prev.userInfo,
                    [prop]: value
                }
            }
        })
    }

    const saveUser = () => {
        if (editing === undefined)
            return;

        api.admin_UpdateUser(editing.userInfo)
            .then(_ => window.location.reload())
            .catch(e => alert(e.message));
    }

    const updateUserEntitlementValue = (key: string, to: string | null) => {
        if (!editing)
            return;

        setEditing(prev => ({
            ...prev!,
            userEntitlements: prev!.userEntitlements.map(e => {
                if (e.entitlementKey !== key) return e;
                return {
                    ...e,
                    userValue: to
                }
            })
        }))
    }

    const saveUserEntitlements = () => {
        if (!editing || !editing.selectedProjectId)
            return;

        const mappedEntitlements: UserEntitlements[] = editing.userEntitlements.filter(ue => ue.userValue).map(ue => ({
            key: ue.entitlementKey,
            value: ue.userValue!
        }));

        api.admin_UpdateUserEntitlements(editing.selectedProjectId, editing.userInfo.id, mappedEntitlements)
            .then(_ => window.location.reload())
            .catch(e => alert(e.message));
    }

    const drawEntitlementInput = (key: string, value: string | null) => {
        if (value === null)
            return <></>

        switch (key as EntitlementKey) {
            case "Early Access":
                return <input type="checkbox" checked={value === "1"} onChange={e => updateUserEntitlementValue(key, e.target.checked ? "1" : "0")}></input>

            default:
                return <input value={value} onChange={e => updateUserEntitlementValue(key, e.target.value)} />
        }
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
                            <td>{UserRoles[d.role ?? 0]}</td>
                            <td><button onClick={() => startEditing(d)}>Edit</button></td>
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
                            <input value={editing.userInfo.displayName} onChange={e => updateUser("displayName", e.target.value)}></input>
                        </header>
                        <header>
                            Email
                            <input value={editing.userInfo.email} onChange={e => updateUser("email", e.target.value)}></input>
                        </header>
                        <header>
                            Role
                            <select value={editing.userInfo.role} onChange={e => updateUser("role", Number(e.target.value))}>
                                {UserRoles.map((r, i) => <option key={r} value={i}>{r}</option>)}
                            </select>
                        </header>
                        <CommonButton label="Save" onClick={saveUser} />

                        <h1>Entitlements</h1>
                        <select value={editing.selectedProjectId} onChange={e => setEditing(prev => ({ ...prev!, selectedProjectId: e.target.value }))}>
                            <option value={""}>None</option>
                            {projects?.map(p => <option key={p.id} value={p.id}>{p.gameName}</option>)}
                        </select>

                        <table>
                            <thead>
                                <tr>
                                    <td>Key</td>
                                    <td>User's value</td>
                                </tr>
                            </thead>
                            <tbody>
                                {editing.userEntitlements.map(ue => (
                                    <tr key={ue.entitlementKey}>
                                        <td>{ue.entitlementKey}</td>
                                        <td>{drawEntitlementInput(ue.entitlementKey, ue.userValue)}</td>
                                        <td>
                                            <button onClick={() => updateUserEntitlementValue(ue.entitlementKey, ue.userValue === null ? "" : null)}>{ue.userValue === null ? "Add" : "Remove"}</button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                        <CommonButton label="Save" onClick={saveUserEntitlements} />

                    </div>
                </div>
            )
        }
    </div>)
}