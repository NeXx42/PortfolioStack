import Content_About from "@/app/[slug]/Content_About";
import { ProjectAdminContent } from "./page";
import { useState } from "react";
import { ProjectContentParam } from "@/app/shared/types";
import CommonButton from "@/app/shared/components/commonButton";

export default function (props: ProjectAdminContent) {
    const [content, setContent] = useState(props.content);
    const [newId, setNewId] = useState(-1);

    const addRow = () => {
        setNewId(prev => prev--);
        setContent(prev => ({
            ...prev,
            elements: [...prev.elements ?? [], {
                id: newId,
                order: -newId,
                value1: "",
                value2: "",
                value3: "",
            }]
        }))
    }

    const removeRow = (pos: number) => {
        setContent(prev => ({
            ...prev,
            elements: prev?.elements?.filter((_, i) => i !== pos)
        }))
    }

    const updateProp = <K extends keyof ProjectContentParam>(id: number, prop: K, value: ProjectContentParam[K]) => {
        setContent(prev => ({
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
            <Content_About content={content} />

            <table>
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Order</th>
                        <th>Content</th>
                        <th>Controls</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        content.elements?.map((dat, i) => (
                            <tr key={dat.id}>
                                <td>{dat.id}</td>
                                <td><input type="number" value={dat.order} onChange={e => updateProp(dat.id, "order", Number(e.target.value))} /></td>
                                <td><textarea value={dat.value1} onChange={e => updateProp(dat.id, "value1", e.target.value)} /></td>
                                <td><button onClick={() => removeRow(i)}>Remove</button></td>
                            </tr>
                        ))
                    }
                    <tr>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td><button onClick={addRow}>Add</button></td>
                    </tr>
                </tbody>
            </table>

            <CommonButton label="Save" onClick={() => props.saveCallback(props.projectId, props.contentId, content)} />
        </div>
    )
}