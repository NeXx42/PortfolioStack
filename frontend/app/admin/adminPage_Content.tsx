"use client"

import * as api from "@api/api.client"

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { ProjectTag } from "@shared/types";

import useRequest from "../hooks/useRequest";

import "./adminPage_Content.css";

export default function () {
    const router = useRouter();


    const { data: projectData } = useRequest(api => api.admin_GetProjects());
    const { data: tagData } = useRequest(api => api.admin_GetTags());

    const [uniqueId, setUniqueId] = useState(-1);
    const [tags, setTags] = useState<ProjectTag[]>([]);

    useEffect(() => {
        setTags(tagData ?? []);
    }, [tagData])

    const getUniqueId = () => {
        setUniqueId(prev => prev - 1);
        return uniqueId;
    }

    function createProject() {
        api.admin_CreateProject().then(r => router.push(`admin/${r}`))
    }

    const createTag = () => {
        setTags(prev => [
            ...prev,
            {
                id: getUniqueId(),
                name: "new",
                customColour: ""
            }
        ])
    }

    const updateTag = <K extends keyof ProjectTag>(id: number, value: ProjectTag[K], prop: K) => {
        setTags(prev => prev.map(p => {
            if (p.id !== id) return p;
            return {
                ...p,
                [prop]: value
            }
        }))
    }

    const saveTags = () => {
        api.admin_SaveTags(tags)
            .then(_ => window.document.location.reload())
            .catch(e => alert(e.message));
    }

    return (
        <div className="adminPage_Content">
            <h2>Tags</h2>
            <table>
                <thead>
                    <tr>
                        <th><p>Id</p></th>
                        <th><p>Name</p></th>
                        <th><p>Control</p></th>
                    </tr>
                </thead>
                <tbody>
                    {
                        tags?.map(t =>
                            <tr key={t.id}>
                                <td><p>{t.id}</p></td>
                                <td><input value={t.name} onChange={e => updateTag(t.id, e.target.value, "name")} /></td>
                                <td><button onClick={() => router.push(`admin/${t.id}`)}>Remove</button></td>
                            </tr>
                        )
                    }
                    <tr>
                        <td></td>
                        <td></td>
                        <td>
                            <button onClick={createTag}>Create</button>
                            <button onClick={saveTags}>Save</button>
                        </td>
                    </tr>
                </tbody>
            </table>

            <h2>Content</h2>
            <table>
                <thead>
                    <tr>
                        <th><p>Id</p></th>
                        <th><p>Name</p></th>
                        <th><p>Status</p></th>
                        <th><p>Control</p></th>
                    </tr>
                </thead>
                <tbody>
                    {
                        projectData?.map(p =>
                            <tr key={p.id}>
                                <td><p>{p.id}</p></td>
                                <td><p>{p.slug}</p></td>
                                <td><p>active</p></td>
                                <td><button onClick={() => router.push(`admin/${p.id}`)}>Edit</button></td>
                            </tr>
                        )
                    }
                    <tr>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td><button onClick={createProject}>Create</button></td>
                    </tr>
                </tbody>
            </table>

        </div>
    )
}
/*
function Modal({ slug }: { slug: string | undefined }) {
    api.fetchGame();

    const [item, setItem] = useState<Project | undefined>();

    const [newContentId, setNewContentId] = useState(1);
    const [newParamId, setNewParamId] = useState(1);
    const [newTagId, setNewTagId] = useState(1);

    useEffect(() => {
        setItem(props.item);
    }, [slug])

    // item

    const editItemProperty = (field: keyof Project, value: any) => {
        setItem((prev?: Project) => {
            if (prev === undefined) return;
            return {
                ...prev,
                [field]: value
            }
        })
    }

    // content

    const addItemContent = () => {
        const newId = newContentId + 1;
        setNewContentId(newId)

        setItem(prev => {
            if (prev === undefined) return;

            return {
                ...prev,
                elements: [...(prev.elements ?? []), {
                    type: ProjectContentType.About,
                    id: -newId,
                    order: 0,
                }]
            }
        })
    }

    const editItemContent = (contentId: number, field: keyof Project, value: any) => {
        setItem(prev => {
            if (prev === undefined) return;
            return {
                ...prev,
                elements: prev.elements?.map(x => {
                    if (x.id !== contentId) return x;
                    return {
                        ...x,
                        [field]: value
                    }
                }) ?? []
            }
        })
    }

    const removeItemContent = (id: number) => {
        setItem(prev => {
            if (prev === undefined) return;
            return {
                ...prev,
                elements: prev.elements?.filter(x => x.id !== id) ?? []
            }
        })
    }

    // params

    const addContentParam = (contentId: number) => {
        const newId = newParamId + 1;
        setNewParamId(newId)

        setItem(prev => {
            if (prev === undefined) return;
            return {
                ...prev,
                elements: prev.elements?.map(x => {
                    if (x.id !== contentId) return x;

                    return {
                        ...x,
                        elements: [...(x.elements ?? []), {
                            id: -newId,
                            order: 0,
                            value1: "",
                            value2: "",
                            value3: "",
                        }]
                    }
                })
            }
        });
    }

    const editContentParam = (contentId: number, paramId: number, field: keyof ProjectContentParam, value: any) => {
        setItem(prev => {
            if (prev === undefined) return;
            return {
                ...prev,
                elements: prev.elements?.map(x => {
                    if (x.id !== contentId) return x;
                    return {
                        ...x,
                        elements: x.elements?.map(p => {
                            if (p.id !== paramId) return p;
                            return {
                                ...p,
                                [field]: value
                            }
                        }) ?? []
                    }
                }) ?? []
            }
        })
    }

    const removeContentParam = (contentId: number, paramId: number) => {
        setItem(prev => {
            if (prev === undefined) return;
            return {
                ...prev,
                elements: prev.elements?.map(x => {
                    if (x.id !== contentId) return x;
                    return {
                        ...x,
                        elements: x.elements?.filter(x => x.id !== paramId) ?? []
                    }
                })
            }
        });
    }

    // tags

    const addProjectTag = () => {
        setNewTagId(prev => prev + 1);

        setItem(prev => {
            if (prev === undefined) return;
            return {
                ...prev,
                tags: [...(prev.tags ?? []), {
                    id: 1,
                    name: "",
                    customColour: ""
                }]
            }
        })
    }

    const removeProjectTag = (index: number) => {
        setItem(prev => {
            if (prev === undefined) return;
            return {
                ...prev,
                tags: prev.tags?.filter((_, i) => i !== index)
            }
        })
    }

    const updateProjectTag = (value: number, i: number) => {
        setItem(prev => {
            if (prev === undefined) return;
            return {
                ...prev,
                tags: prev.tags?.map((t, index) => {
                    if (index !== i) return t;
                    return {
                        ...t,
                        id: value
                    }
                })
            }
        })
    }

    // draw

    const saveChanges = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        await props.onSave(item!);
    }


    const drawElement = (element: ProjectContent) => {
        return (
            <div style={{ marginTop: "55px" }} key={element.id}>
                <div>
                    <select onChange={e => editItemContent(element.id, "type", e.target.value)} value={element.type}>
                        {Object.entries(ProjectContentType)
                            .filter(([_, value]) => typeof value === "number")
                            .map(([key, value]) => (
                                <option key={value} value={value}>
                                    {key}
                                </option>
                            ))}
                    </select>
                    <button type="button" onClick={() => removeItemContent(element.id)}>Remove</button>
                </div>
                <h3>Args</h3>
                <ol>
                    {element.elements?.map(x => (<li style={{ display: "flex", width: "100%", }}>
                        <input style={{ width: "50px" }} type="number" onChange={e => editContentParam(element.id, x.id, "order", Number.parseInt(e.target.value))} value={x.order} />
                        <input style={{ flex: "3" }} onChange={e => editContentParam(element.id, x.id, "value1", e.target.value)} value={x.value1} />
                        <input style={{ flex: "1" }} onChange={e => editContentParam(element.id, x.id, "value2", e.target.value)} value={x.value2} />
                        <input style={{ flex: "1" }} onChange={e => editContentParam(element.id, x.id, "value3", e.target.value)} value={x.value3} />
                        <button type="button" onClick={() => removeContentParam(element.id, x.id)}>Remove</button>
                    </li>))}
                    <li><button type="button" onClick={() => addContentParam(element.id)}>Create</button></li>
                </ol>
            </div>
        )
    }

    return (<div style={{ margin: "25px" }}>
        {item && (<form onSubmit={saveChanges}>
            <h1>Details</h1>
            <ol >
                <li style={{ display: "flex" }}>
                    <label style={{ width: "200px" }}>GameName</label>
                    <input style={{ flex: "3" }} onChange={e => editItemProperty("gameName", e.target.value)} value={item.gameName} />
                </li>
                <li style={{ display: "flex" }}>
                    <label style={{ width: "200px" }}>Short Description</label>
                    <textarea style={{ flex: "3" }} onChange={e => editItemProperty("shortDescription", e.target.value)} value={item.shortDescription} />
                </li>
                <li style={{ display: "flex" }}>
                    <label style={{ width: "200px" }}>Icon</label>
                    <input style={{ flex: "3" }} onChange={e => editItemProperty("icon", e.target.value)} value={item.icon} />
                </li>
                <li style={{ display: "flex" }}>
                    <label style={{ width: "200px" }}>Price</label>
                    <input style={{ flex: "3" }} onChange={e => editItemProperty("cost", e.target.value)} value={item.cost} />
                </li>
                <li style={{ display: "flex" }}>
                    <label style={{ width: "200px" }}>Version</label>
                    <input style={{ flex: "3" }} onChange={e => editItemProperty("version", e.target.value)} value={item.version} />
                </li>
                <li style={{ display: "flex" }}>
                    <label style={{ width: "200px" }}>Created Date</label>
                    <input style={{ flex: "3" }} onChange={e => editItemProperty("dateCreated", e.target.value)} type="date" value={item.dateCreated?.toString()} />
                </li>
                <li style={{ display: "flex" }}>
                    <label style={{ width: "200px" }}>Update date</label>
                    <input style={{ flex: "3" }} onChange={e => editItemProperty("dateUpdate", e.target.value)} type="date" value={item.dateUpdate?.toString()} />
                </li>

                <li style={{ display: "flex" }}>
                    <label style={{ width: "200px" }}>Tags</label>
                    <ol style={{ flex: "3" }}>
                        {item.tags?.map((t, tagIndex) => (
                            <li key={tagIndex}>

                                <select value={t.id} onChange={(e) => updateProjectTag(Number.parseInt(e.target.value), tagIndex)}>
                                    {props.tags?.map(t => (
                                        <option key={t.id} value={t.id}>{t.name}</option>
                                    ))}
                                </select>

                                <button onClick={() => removeProjectTag(t.id)}>Remove</button>
                            </li>
                        ))}
                        <li><button onClick={addProjectTag}>Add</button></li>
                    </ol>
                </li>
            </ol>
            <h1>Releases</h1>

            <Admin_Project_Release project={item} setProject={setItem} />

            <h1>Elements</h1>

            <ol>
                {item.elements?.map((x) => drawElement(x))}

                <button type="button" onClick={() => addItemContent()}>Add</button>
            </ol>

            <button type="submit">Save</button>
        </form>)}
    </div>)

    return (
        <div className="adminPage_Content_ModalInner">

        </div>
    )
}
    */