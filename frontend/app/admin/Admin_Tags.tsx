"use client"

import { useEffect, useState } from "react";
import { ProjectTag } from "../shared/types";

interface Props {
    tags: ProjectTag[] | undefined,
    saveTags: (tags: ProjectTag[]) => Promise<void>
}

export default function (props: Props) {

    const [newTagCount, setNewTagCount] = useState<number>(1);
    const [tags, setTags] = useState<ProjectTag[] | undefined>();

    useEffect(() => {
        setTags(props.tags);
    }, [props.tags])

    const addTag = () => {
        setNewTagCount(prev => prev + 1);

        setTags(prev => {
            if (prev === undefined) return;

            return [...prev, {
                id: -newTagCount,
                name: "",
                customColour: ""
            }]
        })
    }

    const updateTag = (id: number, value: string) => {
        setTags(prev => {
            if (prev === undefined) return;

            return prev.map(x => {
                if (x.id !== id) return x;
                return {
                    ...x,
                    name: value
                }
            })
        })
    }

    const saveTags = () => {
        if (tags === undefined) return;
        props.saveTags(tags!);
    }

    return (
        <div style={{ display: "flex", padding: "25px" }}>
            {tags?.map(x => (
                <input type="text" onChange={(e) => updateTag(x.id, e.target.value)} key={x.id} value={x.name} />
            ))}

            <button onClick={addTag}>Add</button>
            <button onClick={saveTags}>Save</button>
        </div>
    )
}