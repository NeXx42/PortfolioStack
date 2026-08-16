"use client"

import * as api from "@api/api.client"

import useRequest from "@/app/hooks/useRequest"
import { Project, ProjectContent, ProjectTag, releaseStatus } from "@/app/shared/types";
import { useParams, useRouter } from "next/navigation";
import React, { ReactNode, useEffect, useState } from "react";

import { ProjectContentType, ProjectType } from "@/app/shared/enums";
import Navbar from "@/app/shared/components/navbar";

import "./page.css"

import ItemCard from "@/app/components/itemCard";
import adminPage_Content_About from "./adminPage_Content_About";
import adminPage_Content_Screenshots from "./adminPage_Content_Screenshots";
import adminPage_Content_LauncherMetadata from "./adminPage_Content_LauncherMetadata";
import AdminPage_Releases from "./adminPage_Releases";

export interface ProjectAdminContent {
    projectId: string,
    contentId: number,

    content: ProjectContent
    saveCallback: (projectId: string, contentId: number, newContent: ProjectContent) => void;
}

const BlogEntryLookup: Record<ProjectContentType, React.ComponentType<ProjectAdminContent>> = {
    [ProjectContentType.Screenshots]: adminPage_Content_Screenshots,
    [ProjectContentType.About]: adminPage_Content_About,
    [ProjectContentType.Features]: adminPage_Content_About,
    [ProjectContentType.Releases]: adminPage_Content_About,
    [ProjectContentType.Requirements]: adminPage_Content_About,
    [ProjectContentType.LauncherMetadata]: adminPage_Content_LauncherMetadata,
}

export default function () {
    const router = useRouter();

    const params = useParams<{ id: string }>();

    const { data: originalData } = useRequest<Project>(api => api.admin_GetProject(params.id));
    const { data: tags } = useRequest<ProjectTag[]>(api => api.admin_GetTags());

    const [data, setData] = useState<Project | undefined>(undefined);
    const [editingBlogEntry, setEditingBlogEntry] = useState<ProjectContent | undefined>(undefined);
    const [editingReleases, setEditingReleases] = useState(false)

    useEffect(() => {
        setData(originalData);
    }, [originalData])

    // base content

    const updateProperty = <K extends keyof Project>(value: Project[K], prop: K) => {
        setData((prev) => {
            if (prev === undefined) return prev;
            return {
                ...prev,
                [prop]: value
            }
        })
    }

    const saveDetails = async () => {
        const form = new FormData();
        form.append("data", JSON.stringify(data));

        if (data?.icon?.startsWith("blob:")) {
            const response = await fetch(data.icon);
            const blob = await response.blob();

            const file = new File(
                [blob],
                "icon",
                { type: blob.type }
            );

            form.append("icon", file);
        }

        api.admin_SaveProject(form)
            .then(e => document.location.reload())
            .catch(e => alert(e.message));
    }

    // tags

    const addTag = () => {
        setData(prev => ({
            ...prev!,
            tags: [...(prev!.tags ?? []), tags![0]]
        }))
    }

    const removeTag = (index: number) => {
        setData(prev => ({
            ...prev!,
            tags: prev?.tags?.filter((_, i) => {
                return i !== index;
            })
        }))
    }

    const updateTag = (tagIndex: number, newTagIndex: number) => {
        setData(prev => ({
            ...prev!,
            tags: prev!.tags!.map((t, i) =>
                i === tagIndex ? tags![newTagIndex] : t
            )
        }))
    }

    // blogs

    const addBlogEntry = () => {
        setData((prev) => {
            if (prev === undefined) return prev;
            return {
                ...prev,
                elements: [...(prev.elements ?? []), {
                    id: Math.min(0, ...(prev.elements?.map(e => e.id) ?? [])) - 1,
                    type: ProjectContentType.About,
                    order: 0
                }]
            }
        });
    }

    const editBlogEntry = <K extends keyof ProjectContent>(id: number, value: ProjectContent[K], prop: K) => {
        setData((prev) => {
            if (prev === undefined) return prev;
            return {
                ...prev,
                elements: prev.elements?.map(e => {
                    if (e.id !== id) return e;
                    return {
                        ...e,
                        [prop]: value
                    }
                })
            }
        });
    }

    const drawBlogEdit = (): ReactNode => {
        const Component = BlogEntryLookup[editingBlogEntry!.type];

        return (
            <div className="admin_Project_Popup" onClick={() => setEditingBlogEntry(undefined)}>
                <div className="admin_Project_PopupContent" onClick={e => e.stopPropagation()}>
                    <Component projectId={data!.id} contentId={editingBlogEntry!.id!} content={editingBlogEntry!} saveCallback={saveContent} />
                </div>
            </div>
        )
    }

    const drawEditingReleases = (): ReactNode => {
        return (
            <div className="admin_Project_Popup" onClick={() => setEditingReleases(false)}>
                <div className="admin_Project_PopupContent" onClick={e => e.stopPropagation()}>
                    <AdminPage_Releases content={data!} />
                </div>
            </div>
        )
    }

    const saveContent = async (projectId: string, contentId: number, data: ProjectContent) => {
        const form = new FormData();

        form.append("contentId", contentId.toString());
        form.append("newData", JSON.stringify(data));

        for (const e of data.elements ?? []) {
            if (!e.value1?.startsWith("blob:"))
                continue;

            const response = await fetch(e.value1);
            const blob = await response.blob();

            const file = new File(
                [blob],
                e.value1,
                { type: blob.type }
            );

            form.append(e.value1, file);
        }

        api.admin_SaveContent(projectId, form)
            .then(e => document.location.reload())
            .catch(e => alert(e.message));
    }

    const handleChange = (e: any) => {
        const selectedFile = e.target.files[0];
        data!.icon = URL.createObjectURL(selectedFile)
    };

    if (data === undefined)
        return <>loading</>

    return (
        <>
            <Navbar />

            <div className="admin_Project">
                <div className="admin_Project_DetailsWrapper">
                    <form className="admin_Project_Details" >
                        <h1>Details</h1>
                        <div>
                            <a>Status</a>
                            <select value={data.status} onChange={e => updateProperty(Number.parseInt(e.target.value), "status")}>
                                {releaseStatus.map((m, i) => <option key={m} value={i}>{m}</option>)}
                            </select>
                        </div>
                        <div>
                            <a>Name</a>
                            <input type="text" value={data?.gameName} onChange={e => updateProperty(e.target.value, "gameName")} />
                        </div>
                        <div>
                            <a>Slug</a>
                            <input type="text" value={data?.slug} onChange={e => updateProperty(e.target.value, "slug")} />
                        </div>
                        <div>
                            <a>Icon</a>
                            <input type="file" accept="image/*" onChange={handleChange} />
                        </div>
                        <div>
                            <a>Description</a>
                            <textarea value={data?.shortDescription ?? ""} onChange={e => updateProperty(e.target.value, "shortDescription")} />
                        </div>
                        <div>
                            <a>Genre</a>
                            <input type="text" value={data.genre ?? ""} onChange={e => updateProperty(e.target.value, "genre")} />
                        </div>
                        <div>
                            <a>Type</a>
                            <select value={data?.type ?? ProjectType.Game} onChange={(e) => updateProperty(Number(e.target.value) as ProjectType, "type")} >
                                {Object.values(ProjectType)
                                    .filter((value) => typeof value === "number")
                                    .map((type) => (
                                        <option key={type} value={type}>
                                            {ProjectType[type]}
                                        </option>
                                    ))}
                            </select>
                        </div>

                        <div>
                            <a>Creation Date</a>
                            <input type="date" value={data?.dateCreated ? new Date(data.dateCreated * 1000).toISOString().split("T")[0] : ""} onChange={(e) => updateProperty(Math.floor(new Date(e.target.value).getTime() / 1000), "dateCreated")} />
                        </div>

                        <div>
                            <a>Tags</a>
                            <div className="admin_Project_Details_Tags">

                                {
                                    data?.tags?.map((dt, i) => (
                                        <div key={i}>
                                            <select value={tags?.findIndex(t => t.id === dt.id)} onChange={e => updateTag(i, Number.parseInt(e.target.value))}>
                                                {
                                                    tags?.map((tt, ti) => (
                                                        <option key={tt.id} value={ti}>{tt.name}</option>
                                                    ))
                                                }
                                            </select>
                                            <button type="button" onClick={() => removeTag(i)}>x</button>
                                        </div>
                                    ))
                                }

                                <button type="button" onClick={addTag}>+</button>
                            </div>
                        </div>

                        <button type="button" onClick={saveDetails}>Save</button>
                    </form>
                    <ItemCard itemData={data} />
                </div>

                <button onClick={() => setEditingReleases(true)}>Releases</button>

                <div>
                    <h1>Blog content</h1>
                    <table>
                        <thead>
                            <tr>
                                <th>Id</th>
                                <th>Order</th>
                                <th>Type</th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                data?.elements?.map(blog => (
                                    <tr key={blog.id}>
                                        <td>{blog.id}</td>
                                        <td>{blog.order}</td>
                                        <td>
                                            <select value={blog.type} onChange={(e) => editBlogEntry(blog.id, Number(e.target.value) as ProjectContentType, "type")} >
                                                {Object.values(ProjectContentType)
                                                    .filter((value) => typeof value === "number")
                                                    .map((type) => (
                                                        <option key={type} value={type}>
                                                            {ProjectContentType[type]}
                                                        </option>
                                                    ))}
                                            </select>
                                        </td>
                                        <td><button onClick={() => setEditingBlogEntry(blog)}>Edit</button></td>
                                    </tr>
                                ))
                            }
                        </tbody>
                    </table>
                    <button onClick={addBlogEntry}>Create</button>
                </div>

                {editingBlogEntry && drawBlogEdit()}
                {editingReleases && drawEditingReleases()}
            </div>
        </>
    )
}