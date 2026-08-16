import Content_Screenshots from "@/app/[slug]/Content_Screenshots";

import { ProjectAdminContent } from "./page";
import { useState } from "react";

import "./adminPage_Content_Screenshots.css"
import { ProjectContentParam } from "@/app/shared/types";
import CommonButton from "@/app/shared/components/commonButton";

export default function (props: ProjectAdminContent) {
    const [data, setData] = useState(props.content)
    const [newIndex, setNewIndex] = useState(-1);

    const addScreenshot = () => {
        setNewIndex(prev => prev -= 1)

        setData(prev => ({
            ...prev,
            elements: [...(prev.elements ?? []), {
                id: newIndex,
                order: ((prev.elements ?? []).map(e => e.order).sort((a, b) => b - a)[0] ?? 0) + 1,
                value1: "",
                value2: "",
                value3: ""
            }]
        }))
    }

    const removeScreenshot = (id: number) => {
        setData(prev => ({
            ...prev,
            elements: (prev.elements ?? []).filter(e => e.id !== id)
        }))
    }

    const updateProp = <K extends keyof ProjectContentParam>(id: number, prop: K, value: ProjectContentParam[K]) => {
        setData(prev => ({
            ...prev,
            elements: (prev.elements ?? []).map(dat => {
                if (dat.id !== id) return dat;
                return {
                    ...dat,
                    [prop]: value
                }
            })
        }))
    }

    return (
        <div>
            <div className="admin_Project_Screenshots_Preview">
                <Content_Screenshots content={data} />
            </div>

            <div className="admin_Project_Screenshots">
                <table className="admin_Project_Screenshots_Table">
                    <thead>
                        <tr>
                            <th>Order</th>
                            <th>Name</th>
                            <th>icon</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody>

                        {data.elements?.map(d => <tr key={d.id}>
                            <td>{d.order}</td>
                            <td><input value={d.value1} onChange={e => updateProp(d.id, "value1", e.target.value)} /></td>
                            <td><input type="file" onChange={e => updateProp(d.id, "value1", URL.createObjectURL(e.target.files![0]))} /></td>
                            <td><button onClick={() => removeScreenshot(d.id)}>Remove</button></td>
                        </tr>)}
                    </tbody>
                </table>

                <button onClick={addScreenshot}>Add</button>
                <CommonButton label="Save" onClick={() => props.saveCallback(props.projectId, props.contentId, data)} />
            </div>
        </div>
    )
}