import { useParams } from "react-router-dom";
import { useGame } from "../hooks/useGame";

import "./Content.css"

import NotFound from "./NotFound";
import Navbar from "../components/navbar";
import type { ReactNode } from "react";
import Footer from "../components/footer";
import type { ItemContent } from "../types";
import { ProjectContentType, UserRoles } from "../enums";
import Content_Screenshots from "./Content/Content_Screenshots";
import { useAuth } from "../hooks/useUser";

export default function Content() {
    const { slug } = useParams();

    if (slug === undefined)
        return <NotFound />

    const { content, loading, error } = useGame(slug ?? "");
    const { authenticatedUser } = useAuth()

    const wrapSection = (sectionName: string, children: ReactNode) => {
        return (
            <section className="Content_Section" key={sectionName}>
                <div className="Content_Section_Header">
                    <h2>{sectionName}</h2>
                    {authenticatedUser?.role === UserRoles.Admin && (
                        <div>
                            <button>Move Up</button>
                            <button>Move Down</button>
                            <button>Delete</button>
                        </div>
                    )}
                </div>
                {children}
            </section>
        )
    }


    const drawGet = () => {
        return (
            <div className="Content_Get stuck">

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
        let child: ReactNode | undefined;
        console.log(element);
        switch (element.type) {
            case ProjectContentType.Screenshots:
                child = <Content_Screenshots key={contentKey} content={element} />;
                break;
        }

        return wrapSection(element.type.toString(), child);
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

                    <div className="Content_Main">
                        {drawGet()}
                        {drawTitle()}
                        {content?.elements?.map(drawElement)}
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