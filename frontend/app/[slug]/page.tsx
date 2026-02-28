import "./page.css"

import { fetchGame } from "@api/api.server";

import Navbar from "@shared/components/navbar";
import Footer from "@shared/components/footer";
import CommonButton from "@shared/components/commonButton";

import { Project } from "@shared/types";
import { ProjectContentType } from "@shared/enums";
import type { ProjectContent, ProjectRelease } from "@shared/types";

import Content_About from "./Content_About";
import Content_Releases from "./Content_Releases";
import Content_Screenshots from "./Content_Screenshots";
import Content_Get from "./Content_Get";

import { Metadata } from "next";
import { cache } from "react";

export interface ContentElementProps {
    content: ProjectContent
}

const getProject = cache(async (slug: string) => {
    return fetchGame(slug);
})

export async function generateMetadata({ params }: { params: { slug: string } }): Promise<Metadata> {
    const { slug } = await params;

    try {
        const content = await getProject(slug);

        return {
            title: `${content?.gameName ?? ""} - NeXx`
        };
    } catch {
        return { title: "Project" };
    }
}

export default async function ({ params }: { params: { slug: string } }) {
    const { slug } = await params;

    var content: Project | undefined;

    try {
        content = await getProject(slug);
    }
    catch { }

    const latestRelease: ProjectRelease | undefined = content?.releases?.sort(r => r.date.getDate())[0] ?? undefined;

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


    const drawElement = (element: ProjectContent, index: number) => {
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


    return (
        <>
            <Navbar />

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

                    <Content_Get release={latestRelease} cost={content?.cost} />

                    <div className="Content_Main">
                        {drawTitle()}
                        {content?.elements?.sort(a => a.order)?.map(drawElement)}
                    </div>
                </div>
            </div>

            <Footer />
        </>

    )
}