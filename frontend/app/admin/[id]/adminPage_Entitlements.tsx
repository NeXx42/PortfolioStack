import "./adminPage_Entitlements.css"

import * as api from "@api/api.client"

import useRequest from "@/app/hooks/useRequest"
import CommonButton from "@/app/shared/components/commonButton";
import { ProjectEntitlement } from "@/app/shared/types";
import { useEffect, useState } from "react";
import { EntitlementKeys, EntitlementTypes } from "@/app/shared/enums";

export default function ({ id }: { id: string }) {
    const { data, loading, error } = useRequest(a => a.admin_GetEntitlements(id));
    const [modifiedEntitlements, setModifiedEntitlements] = useState<ProjectEntitlement[]>([]);

    useEffect(() => {
        setModifiedEntitlements(data ?? []);
    }, [data])

    if (loading)
        return <>Loading...</>

    if (error) {
        if (error === "Request failed with status 404") {
            const registerEntitlements = () => {
                api.admin_UseEntitlements(id)
                    .then(_ => window.location.reload())
                    .catch(e => alert(e.message));
            }

            return <CommonButton label="Use entitlements" onClick={() => registerEntitlements()} />
        }

        return <>{error}</>
    }

    const drawRow = (entitlement: ProjectEntitlement, index: number) => {
        return (<tr key={index}>
            <td>
                <select value={entitlement.key} onChange={e => updateEntitlement(index, "key", e.target.value)}>
                    {EntitlementKeys.map(k => <option key={k} value={k}>{k}</option>)}
                </select>
            </td>
            <td>
                <select value={entitlement.type} onChange={e => updateEntitlement(index, "type", Number(e.target.value))}>
                    {EntitlementTypes.map((k, i) => <option key={k} value={i}>{k}</option>)}
                </select>
            </td>
            <td>
                <input value={entitlement.value} onChange={e => updateEntitlement(index, "value", e.target.value)}></input>
            </td>
            <td>
                <button onClick={() => removeEntitlement(index)}>Remove</button>
            </td>
        </tr>)
    }

    const addEntitlement = () => {
        setModifiedEntitlements(prev => [...prev, {
            key: EntitlementKeys[0],
            type: 0
        }])
    }

    const removeEntitlement = (id: number) => {
        setModifiedEntitlements(prev => prev.filter((_, i) => i !== id));
    }

    const updateEntitlement = <K extends keyof ProjectEntitlement>(id: number, prop: K, value: ProjectEntitlement[K]) => {
        setModifiedEntitlements(prev => prev.map((p, i) => {
            if (i !== id) return p;
            return {
                ...p,
                [prop]: value
            }
        }))
    }

    const save = () => {
        api.admin_SaveEntitlements(id, modifiedEntitlements)
            .then(_ => window.location.reload())
            .catch(e => alert(e.message));
    }

    return (<>
        <table>
            <thead>
                <tr>
                    <th>Key</th>
                    <th>Type</th>
                    <th>Default Value</th>
                </tr>
            </thead>
            <tbody>
                {
                    modifiedEntitlements.map(drawRow) ?? (<></>)
                }
            </tbody>
        </table>

        <CommonButton label="Add" onClick={() => addEntitlement()} />
        <CommonButton label="Save" onClick={() => save()} />
    </>)
}