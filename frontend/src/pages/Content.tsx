import { useParams } from "react-router-dom";
import { useGame } from "../hooks/useGame";

import "./Content.css"

import NotFound from "./NotFound";
import Navbar from "../components/navbar";
import { useEffect, useRef, useState } from "react";
import Footer from "../components/footer";
import type { ItemContent, ItemRelease } from "../types";
import { ProjectContentType } from "../enums";
import Content_Screenshots from "./Content/Content_Screenshots";
import Content_About from "./Content/Content_About";
import CommonButton from "../components/commonButton";
import Content_Releases from "./Content/Content_Releases";

export interface ContentElementProps {
    content: ItemContent
}

export default function Content() {
    const { slug } = useParams();

    if (slug === undefined)
        return <NotFound />

    const { content, error } = useGame(slug ?? "");
    const [latestRelease, setLatestRelease] = useState<ItemRelease | null>(null)

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

    useEffect(() => {
        document.title = `NeXx - ${content?.gameName ?? slug}`;

        console.log(content?.releases?.sort(r => r.date.getDate()))
        setLatestRelease(content?.releases?.sort(r => r.date.getDate())[0] ?? null)
    }, [content])


    const tryToDownloadLatestRelease = () => {
        const link = document.createElement("a");
        link.href = latestRelease?.downloads[0].link ?? "";

        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }

    const drawGet = () => {
        return (
            <div className={`Content_Get ${isGetSticky ? "Stuck" : ""}`}>
                <div className="Content_Get_Details">
                    <span>VERSION <b>{latestRelease?.version ?? "-"}</b></span>
                    <span>SIZE <b>{latestRelease?.size ?? "-"} MB</b></span>
                </div>

                <div className="Content_Get_Actions">
                    <label>{(content?.cost ?? 0) > 0 ? `£${content!.cost}` : ""}</label>
                    <CommonButton label={latestRelease !== null ? ((content?.cost ?? 0) > 0 ? "Purchase" : "Download") : "Unavailable"} onClick={tryToDownloadLatestRelease} />
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
            [ProjectContentType.Releases]: Content_Releases,
            [ProjectContentType.Requirements]: Content_About,
        }

        const Component = contentMap[element.type];

        return (
            <section className="Content_Section" key={contentKey}>
                <div className="Content_Section_Header">
                    <h2>{ProjectContentType[element.type]}</h2>
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
                        {content?.elements?.sort(a => a.order)?.map(drawElement)}
                    </div>
                </div>
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