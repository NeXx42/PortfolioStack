import { useEffect, useState } from "react";
import { ProjectContentType } from "../../enums"
import type { Item, ItemContent, ItemContentParameter } from "../../types"

interface Props {
    item: Item,
    onSave: (item: Item) => Promise<void>
}

export default function ContentEdit(props: Props) {

    const [item, setItem] = useState<Item | undefined>();
    const [newContentId, setNewContentId] = useState(1);
    const [newParamId, setNewParamId] = useState(1);


    useEffect(() => {
        setItem(props.item);
    }, [props.item])

    // item

    const editItemProperty = (field: keyof Item, value: any) => {
        setItem((prev?: Item) => {
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

    const editItemContent = (contentId: number, field: keyof Item, value: any) => {
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

    const editContentParam = (contentId: number, paramId: number, field: keyof ItemContentParameter, value: any) => {
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


    // draw

    const saveChanges = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        await props.onSave(item!);
    }


    const drawElement = (element: ItemContent) => {
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
            </ol>
            <h1>Elements</h1>

            <ol>
                {item.elements?.map((x) => drawElement(x))}

                <button type="button" onClick={() => addItemContent()}>Add</button>
            </ol>

            <button type="submit">Save</button>
        </form>)}
    </div>)
}