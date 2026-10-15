"use client"

import "./admin.css";
import "./adminCommon.css";

import { useState } from "react";

import Navbar from "@shared/components/navbar";

import { UserRoles } from "@shared/enums";
import { useAuth } from "@hooks/useUser";

import AdminPage_Controls from "./adminPage_Controls"
import AdminPage_Content from "./adminPage_Content"

import AdminPage_Users from "./adminPage_Users";

export default function () {
    const { authenticatedUser } = useAuth();

    if (authenticatedUser?.role !== UserRoles.Admin)
        return (<div>
            <Navbar />
            Unauthorised
        </div>)

    const pages = [
        { content: <AdminPage_Controls />, name: "Controls" },
        { content: <AdminPage_Content />, name: "Content" },
        { content: <AdminPage_Users />, name: "Users" },
    ]

    const [currentPage, setCurrentPage] = useState(0);

    return (
        <div style={{ fontFamily: "sans-serif" }}>
            <Navbar />

            <div className="AdminPage_Main">

                <aside>
                    {
                        pages.map((cont, i) => <button key={i} onClick={() => setCurrentPage(i)}>{cont.name}</button>)
                    }
                </aside>

                <div>
                    {pages[currentPage].content}
                </div>
            </div>
        </div>
    )
}