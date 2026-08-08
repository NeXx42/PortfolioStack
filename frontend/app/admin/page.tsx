"use client"

import { useEffect, useRef, useState } from "react";

import { UserRoles } from "@shared/enums";
import type { Project } from "@shared/types";

import Navbar from "@shared/components/navbar";
import CommonButton from "@shared/components/commonButton";

import { useAuth } from "@hooks/useUser";

import AdminPage_Controls from "./adminPage_Controls"
import AdminPage_Content from "./adminPage_Content"

import "./admin.css"

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