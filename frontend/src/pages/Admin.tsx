import { useEffect, useState } from "react";
import CommonButton from "../components/commonButton";
import Navbar from "../components/navbar";
import { UserRoles } from "../enums";
import { useAdmin } from "../hooks/userAdmin";
import { useAuth } from "../hooks/useUser";
import type { Item } from "../types";
import { useGame } from "../hooks/useGame";
import ContentEdit from "./Admin/ContentEdit";

export default function Admin() {
    const { authenticatedUser } = useAuth()

    if (authenticatedUser?.role !== UserRoles.Admin)
        return (<div>
            <Navbar />
            Unauthorised
        </div>)

    const { slugs, clearCache, saveItem } = useAdmin()

    const [selectedSlug, setSelectedSlug] = useState<string | undefined>(undefined)
    const [editingContent, setEditingContent] = useState<Item | undefined>(undefined)

    const { content } = useGame(selectedSlug);

    useEffect(() => {
        if (!selectedSlug) {
            setEditingContent(undefined);
            return;
        }

        setEditingContent(content);
    }, [selectedSlug, content]);

    return (
        <div>
            <Navbar />


            <div className="Admin_Controls">
                <CommonButton label="Clear Cache" onClick={() => clearCache()} />
            </div>

            <div className="Admin_Edit">
                <select onChange={(e) => setSelectedSlug(e.target.value)}>
                    {slugs?.map(x => (
                        <option key={x}>{x}</option>
                    ))}
                </select>
            </div>
            {editingContent !== undefined && (
                <ContentEdit item={editingContent} onSave={saveItem} />
            )}
        </div>
    )
}