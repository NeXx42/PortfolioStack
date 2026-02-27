"use client"

import { useEffect, useRef, useState } from "react";

import { UserRoles } from "@shared/enums";
import type { Project } from "@shared/types";

import Navbar from "@shared/components/navbar";
import CommonButton from "@shared/components/commonButton";

import { useAdmin } from "@hooks/userAdmin";
import { useAuth } from "@hooks/useUser";
import { useGame } from "@hooks/useGame";

import Admin_Tags from "./Admin_Tags";
import Admin_Project from "./Admin_Project";

export default function () {
    const { authenticatedUser } = useAuth()

    if (authenticatedUser?.role !== UserRoles.Admin)
        return (<div>
            <Navbar />
            Unauthorised
        </div>)

    const {
        slugs,
        clearCache,
        saveItem,

        images,
        uploadImage,

        tags,
        saveTags,
    } = useAdmin()

    const [selectedSlug, setSelectedSlug] = useState<string | undefined>(undefined)
    const [editingContent, setEditingContent] = useState<Project | undefined>(undefined)

    const { content } = useGame(selectedSlug);

    useEffect(() => {
        if (!selectedSlug) {
            setEditingContent(undefined);
            return;
        }

        setEditingContent(content);
    }, [selectedSlug, content]);


    const [selectedImageUrl, setSelectedImageUrl] = useState<string | undefined>(undefined);
    const [uploadedImages, setUploadedImages] = useState<string[]>([])

    const fileRef = useRef<HTMLInputElement>(null);
    const upload = async () => {
        for (let i = 0; i < fileRef.current!.files!.length; i++) {
            const formData = new FormData();
            formData.append("file", fileRef.current!.files![i]);

            const res = await uploadImage(formData);
            setUploadedImages(prev => [...prev, res])
        }
    }

    return (
        <div style={{ fontFamily: "sans-serif" }}>
            <Navbar />

            <details className="images">
                <summary>Images</summary>
                <div style={{ display: "flex", justifyContent: "space-between" }}>

                    <ol>
                        {[...images ?? [], ...uploadedImages]?.map(x => (
                            <li key={x}>
                                <a key={x} onClick={() => setSelectedImageUrl(x)}>{x}</a>
                            </li>
                        ))}
                    </ol>

                    <img src={`/images/${selectedImageUrl}`} />
                </div>
                <input ref={fileRef} type="file" id="imageInput" multiple />
                <button onClick={upload}>Upload</button>
            </details>

            <details>
                <summary>Tags</summary>
                <Admin_Tags tags={tags} saveTags={saveTags} />
            </details >

            <details>
                <summary>Controls</summary>
                <div className="Admin_Controls">
                    <CommonButton label="Clear Cache" onClick={() => clearCache()} />
                </div>
            </details >

            <details>
                <summary>Project</summary>
                <div className="Admin_Edit">
                    <select onChange={(e) => setSelectedSlug(e.target.value)}>
                        {slugs?.map(x => (
                            <option key={x}>{x}</option>
                        ))}
                    </select>
                </div>

                {editingContent !== undefined && (
                    <Admin_Project item={editingContent} onSave={saveItem} tags={tags} />
                )}
            </details >
        </div>
    )
}