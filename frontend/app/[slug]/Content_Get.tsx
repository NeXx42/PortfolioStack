"use client"

import { useEffect, useRef, useState } from "react";
import { ProjectRelease } from "../shared/types";
import Link from "next/link";
import CommonButton from "@shared/components/commonButton";

interface Props {
    release: ProjectRelease | undefined,
    cost?: number
}

export default function (props: Props) {
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

    return (
        <div ref={stickyPointRef} className="Content_GetDivider">
            <div className={`Content_Get ${isGetSticky ? "Stuck" : ""}`}>
                <div className="Content_Get_Details">
                    <span>VERSION <b>{props.release?.version ?? "-"}</b></span>
                    <span>SIZE <b>{props.release?.size ?? "-"} MB</b></span>
                </div>

                <div className="Content_Get_Actions">
                    <label>{(props.cost ?? 0) > 0 ? `£${props.cost}` : ""}</label>
                    <Link href={props.release?.downloads[0].link ?? ""}>
                        <CommonButton label={props.release !== null ? ((props.cost ?? 0) > 0 ? "Purchase" : "Download") : "Unavailable"} />
                    </Link>
                </div>
            </div>
        </div>
    )
}