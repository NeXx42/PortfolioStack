import { useParams } from "react-router-dom";
import { useGame } from "../hooks/useGame";

import "./Content.css"

import NotFound from "./NotFound";
import Navbar from "../components/navbar";
import { useEffect, useRef, useState } from "react";
import Footer from "../components/footer";
import type { ItemContent, ItemContentParameter } from "../types";
import { ProjectContentType, UserRoles } from "../enums";
import Content_Screenshots from "./Content/Content_Screenshots";
import { useAuth } from "../hooks/useUser";
import Content_About from "./Content/Content_About";
import CommonButton from "../components/commonButton";

export interface ContentElementProps {
    content: ItemContent
}

export default function Content() {
    const { slug } = useParams();

    if (slug === undefined)
        return <NotFound />

    const { content, error, updatePage } = useGame(slug ?? "");
    const { authenticatedUser } = useAuth()

    // Admin

    const [addPageType, setAddPageType] = useState(ProjectContentType.Screenshots);
    const [editElement, setEditElement] = useState<ItemContent | undefined>(undefined);

    const [newPages, setNewPages] = useState<ItemContent[]>([])
    const [deletedContent, setDeletedContent] = useState<number[]>([])
    const [elementModifications, setElementModifications] = useState<Record<number, ItemContent>>({})


    const addPage = () => {
        setNewPages(prev => {
            const newId: number = Math.min(Math.min(...(prev.map(e => e.id) ?? [0])), 0) - 1

            return [...prev, {
                id: newId,
                type: addPageType,
                order: Math.max(...newPages.map(i => i.order), ...(content?.elements ?? []).map(i => (i.order ?? 0))) + 1
            }]
        });
    }

    const startEdit = (content: ItemContent) => {
        setEditElement(elementModifications[content.id] ?? content);
    }

    const editElementParam = (id: number, field: keyof ItemContentParameter, value: any) => {
        setEditElement(prev => {
            if (prev === undefined)
                return;

            return {
                ...prev, elements: [...(prev.elements ?? [])].map(x => {
                    if (x.id != id)
                        return x;

                    return {
                        ...x,
                        [field]: value
                    }
                })
            }
        })
    }

    const removeElementParam = (id: number) => {
        setEditElement(prev => {
            if (prev === undefined)
                return;
            return { ...prev, elements: prev.elements?.filter(x => x.id !== id) }
        })
    }

    const addElementParam = () => {
        setEditElement(prev => {
            if (prev === undefined)
                return;

            const min: number = Math.min(Math.min(...(prev.elements?.map(e => e.id) ?? [0])), 0) - 1
            const maxOrder: number = Math.max(...(prev.elements?.map(e => e.order) ?? [0])) + 1;

            return {
                ...prev,
                elements: [...(prev.elements ?? []), {
                    id: min,
                    order: maxOrder,
                    value1: "",
                    value2: "",
                    value3: "",
                }]
            }
        })
    }

    const saveEditElement = () => {
        if (editElement === undefined)
            return;

        setElementModifications(prev => {
            return {
                ...prev,
                [editElement.id]: editElement
            }
        });

        setEditElement(undefined);
    }

    const deleteElement = (element: ItemContent) => {
        if (element.id >= 0) {
            setDeletedContent(prev => [...prev, element.id]);
        }
        else {
            setNewPages(prev => prev.filter(p => p.id !== element.id));
        }

        setElementModifications(prev => {
            const copy = { ...prev };
            delete copy[element.id];
            return copy;
        });
    }

    const saveModifications = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        const toCreate: ItemContent[] = newPages.map(p => {
            return elementModifications[p.id] ?? p
        });

        try {
            await updatePage(toCreate, Object.values(elementModifications).filter(p => p.id >= 0), deletedContent);
            window.location.reload();
        }
        catch {
            alert("Failed to save");
        }
    }

    // end of admin


    const isAdmin = authenticatedUser?.role === UserRoles.Admin;

    const pageContent = [
        ...newPages,
        ...(content?.elements ?? [])
            .filter(p => !deletedContent.includes(p.id))
            .map(e => elementModifications[e.id] ?? e)
    ].sort(a => a.order);

    const [isGetSticky, setGetSticky] = useState(false);
    const stickyPointRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const handleScroll = () => {
            if (stickyPointRef.current) {
                const stickyPoint = stickyPointRef.current.offsetTop + 65; // header bar 55 pxs?
                setGetSticky(window.scrollY > stickyPoint);
            }
        };

        window.addEventListener("scroll", handleScroll);
        return () => window.removeEventListener("scroll", handleScroll);
    }, [])


    const drawGet = () => {
        return (
            <div className={`Content_Get ${isGetSticky ? "Stuck" : ""}`}>
                <div className="Content_Get_Details">
                    <span>VERSION <b>{content?.version ?? "0.0.0"}</b></span>
                    <span>SIZE <b>Unavailable</b></span>
                </div>

                <div className="Content_Get_Actions">
                    <label>{content?.cost}</label>
                    <CommonButton label={(content?.cost ?? 0) >= 0 ? "Download" : "Purchase"} onClick={() => { }} />
                </div>
            </div>
        )
    }

    const drawTitle = () => {
        return (
            <section className="Content_Title">
                <div className="Content_Title_Tags">
                    {content?.tags?.map(t => (<a key={t.name}>{t.name}</a>))}
                </div>
                <h1>{content?.gameName}</h1>
                <div className="Content_Title_Tagline">
                    <span>By Nexx42</span>
                    <div />
                    <span>Released {content?.gameName}</span>
                    <div />
                    <span>Updated {content?.gameName}</span>
                </div>
            </section>
        )
    }


    const drawElement = (element: ItemContent, index: number) => {
        const contentKey: string = `Content_${element.type}_${index}`;
        const contentMap: Record<ProjectContentType, React.ComponentType<ContentElementProps>> = {
            [ProjectContentType.Screenshots]: Content_Screenshots,
            [ProjectContentType.About]: Content_About,
            [ProjectContentType.Features]: Content_About,
            [ProjectContentType.Releases]: Content_About,
            [ProjectContentType.Requirements]: Content_About,
        }

        const Component = contentMap[element.type];

        return (
            <section className="Content_Section" key={contentKey}>
                <div className="Content_Section_Header">
                    <h2>{ProjectContentType[element.type]}</h2>
                    {isAdmin && (
                        <div>
                            <button onClick={() => startEdit(element)}>Edit</button>
                            <button>Move Up</button>
                            <button>Move Down</button>
                            <button onClick={() => deleteElement(element)}>Delete</button>
                        </div>
                    )}
                </div>
                {<Component content={element} />}
            </section>
        )
    }

    const drawPage = () => {
        return (
            <div className="Content">

                <div className="Content_ContentFitter">
                    <ol className="Content_BreadCrumbs">
                        <li><a>Home</a></li>
                        <li><a>Projects</a></li>
                        <li><a>Games</a></li>
                        <li><a>{content?.gameName}</a></li>
                    </ol>

                    <div className="Content_Hero">
                        <img src={content?.icon} />

                        <div className="Content_Hero_Shine" />
                        <div className="Content_Hero_Glow" />
                        <div className="Content_Hero_Vignette" />
                    </div>

                    <div ref={stickyPointRef} className="Content_GetDivider">
                        {drawGet()}
                    </div>

                    <div className="Content_Main">
                        {drawTitle()}
                        {pageContent?.map(drawElement)}

                        {isAdmin && (
                            <div>
                                <select
                                    value={addPageType}
                                    onChange={(e) => setAddPageType(Number(e.target.value) as ProjectContentType)}
                                >
                                    {Object.entries(ProjectContentType)
                                        .filter(([_, value]) => typeof value === "number")
                                        .map(([key, value]) => (
                                            <option key={value} value={value}>
                                                {key}
                                            </option>
                                        ))}
                                </select>

                                <button onClick={addPage}>Add Content</button>

                                <form onSubmit={saveModifications}>
                                    <button type="submit">Save Page</button>
                                </form>
                            </div>
                        )}
                    </div>
                </div>

                {editElement && (
                    <div className="Content_Admin_Edit">
                        <button onClick={() => setEditElement(undefined)}>Close</button>
                        <button onClick={() => saveEditElement()}>Save</button>

                        <ol>
                            {editElement.elements?.map((x, i) => (
                                <li key={i}>
                                    <div>
                                        <label>{x.id}</label>
                                        <input type="number" onChange={e => editElementParam(x.id, "order", Number.parseInt(e.target.value))} value={x.order} />
                                        <input onChange={e => editElementParam(x.id, "value1", e.target.value)} value={x.value1} />
                                        <input onChange={e => editElementParam(x.id, "value2", e.target.value)} value={x.value2} />
                                        <input onChange={e => editElementParam(x.id, "value3", e.target.value)} value={x.value3} />
                                        <button onClick={() => removeElementParam(x.id)}>Remove</button>
                                    </div>
                                </li>
                            ))}
                        </ol>

                        <button onClick={addElementParam}>Add</button>
                    </div>
                )}
            </div>
        )
    }

    const drawError = () => {
        return (
            <a>{error}</a>
        );
    }

    return (
        <>
            <Navbar />

            {error === undefined ? (
                drawPage()
            ) :
                drawError()
            }

            <Footer />
        </>

    )
}