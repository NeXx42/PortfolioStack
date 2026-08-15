"use client"

import "./Content_Get.css"

import { useEffect, useInsertionEffect, useRef, useState } from "react";
import { ProjectRelease, ProjectReleaseDownload } from "../shared/types";
import Link from "next/link";
import CommonButton from "@shared/components/commonButton";

interface Props {
    release: ProjectRelease | undefined,
    cost?: number
}

export default function (props: Props) {
    const [isGetSticky, setGetSticky] = useState(false);
    const stickyPointRef = useRef<HTMLDivElement>(null);

    const [selectedDownload, setSelectedDownload] = useState<ProjectReleaseDownload | undefined>(undefined);

    const formatSize = (size: number, displayUnknown: boolean): string => {
        if (size === 0)
            return displayUnknown ? "-" : "";

        const units = ["B", "KB", "MB", "GB", "TB", "PB", "EB"];
        let curSize = size;
        let unit = 0;

        while (curSize >= 1024 && unit < units.length - 1) {
            curSize /= 1024;
            unit++;
        }

        return `${curSize.toFixed(1).replace(/\.0$/, "")} ${units[unit]}`;
    }

    useEffect(() => {
        if ((props.release?.downloads?.length ?? 0) > 0) {
            const ua = navigator.userAgent.toLowerCase();
            let os = null;

            if (ua.includes("windows"))
                os = "windows";
            else if (ua.includes("linux"))
                os = "linux";

            const selectedDownload =
                props.release!.downloads.find(download =>
                    os && download.platform.toLowerCase().includes(os)
                ) ?? props.release!.downloads[0];

            setSelectedDownload(selectedDownload);
        }

        const handleScroll = () => {
            if (stickyPointRef.current) {
                const stickyPoint = stickyPointRef.current.offsetTop + 65; // header bar 55 pxs?
                setGetSticky(window.scrollY > stickyPoint);
            }
        };
        window.addEventListener("scroll", handleScroll);
        return () => window.removeEventListener("scroll", handleScroll);
    }, [])


    const drawDownloadButton = () => {
        const hasDownload = selectedDownload?.downloadLink != undefined && selectedDownload?.downloadLink != "";

        return (
            <>
                <label>{(props.cost ?? 0) > 0 ? `£${props.cost}` : ""}</label>
                <Link href={selectedDownload?.downloadLink ?? ""}>
                    <CommonButton label={hasDownload ? ((props.cost ?? 0) > 0 ? "Purchase" : `Download`) : "Unavailable"} />
                </Link>
            </>
        )
    }

    const drawPlatformDetails = () => {
        const availablePlatforms: string[] = props?.release?.downloads.map(d => d.platform) ?? [];
        var platformSelector = <b>Unavailable</b>

        if (availablePlatforms.length == 1) {
            platformSelector = <b>{availablePlatforms[0]}</b>
        }
        else if (availablePlatforms.length > 1) {
            platformSelector = (
                <select className="Content_Get_Details_Select" value={selectedDownload?.platform} onChange={e => setSelectedDownload(props.release?.downloads.filter(d => d.platform === e.target.value)[0])}>
                    {availablePlatforms.map(p => <option key={p} value={p}>{p}</option>)}
                </select>
            )
        }

        return (
            <>
                <span>PLATFORM {platformSelector}</span>
                <span>VERSION <b>{props.release?.version ?? "-"}</b></span>
                <span>SIZE <b>{formatSize(selectedDownload?.size ?? 0, true)}</b></span>
            </>
        )
    }

    return (
        <div ref={stickyPointRef} className="Content_GetDivider">
            <div className={`Content_Get ${isGetSticky ? "Stuck" : ""}`}>
                <div className="Content_Get_Details">
                    {drawPlatformDetails()}
                </div>
                <div className="Content_Get_Actions">
                    {drawDownloadButton()}
                </div>
            </div>
        </div>
    )
}