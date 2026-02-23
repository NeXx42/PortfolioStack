import { useEffect, useState } from "react";

import ItemCard from "./Home/itemCard";
import Navbar from "../components/navbar";

import { useContent } from "../hooks/useContent";

import { ProjectType } from "../enums";

import "./Home.css";
import Footer from "../components/footer";
import LinkCard from "./Home/linkCard";
import ItemCardSkeleton from "./Home/itemCardSkeleton";

export default function Home() {
    const [selectedTabIndex, setSelectedTabIndex] = useState(ProjectType.Game);
    const [tags, setTags] = useState<string[]>([])

    const {
        featured,
        loadingFeatured,

        links,
        loadingLinks,

        content,
        loadingContent,
        fetchContent
    } = useContent();

    const items = content[selectedTabIndex] ?? [];

    useEffect(() => {
        fetchContent(selectedTabIndex)
    }, [selectedTabIndex])

    useEffect(() => {
        if (loadingContent)
            return;

        const uniqueTags = new Set();
        setTags(items.flatMap(item => item.tags || [])
            .filter(tag => {
                if (uniqueTags.has(tag.name))
                    return false;

                uniqueTags.add(tag.name);
                return true;
            }).map(t => t.name));

    }, [items, loadingContent])

    useEffect(() => {
        document.title = "NeXx";
    })

    const skeletonDelay = 500;
    const [showContentSkeleton, setShowContentSkeleton] = useState(false);
    useEffect(() => {
        if (loadingContent) {
            const timeout = setTimeout(() => {
                setShowContentSkeleton(true);
            }, skeletonDelay)

            return () => clearTimeout(timeout);
        }

        setShowContentSkeleton(false);
    }, [loadingContent])

    return (<>
        <canvas id="StarContainer" />

        <div className="Home_Hero">
            <div className="Home_Hero_Orbs" />
            <div className="Home_Hero_Grid" />
            <div className="Home_Hero_Content">
                <div className="Home_Hero_Content_Person">
                    <h1 className="Home_Hero_Content_Name">NeXx42</h1>
                    <div className="Home_Hero_Content_Divider" />
                    <p className="Home_Hero_Content_Description">Game Developer · Software Developer</p>
                </div>

                <div className="Home_hero_Content_Featured">
                    {(loadingFeatured || featured.length == 0) ? (
                        <>
                            <ItemCardSkeleton isWide={true} />
                            <ItemCardSkeleton isWide={false} />
                            <ItemCardSkeleton isWide={false} />
                        </>
                    ) : (
                        featured.map((item, key) =>
                            (<ItemCard key={key} itemData={item} />)
                        )
                    )}
                </div>
            </div>

            <div className="Home_Hero_ScrollHint">
                <div>↓</div>
                <span>Scroll</span>
            </div>
        </div>

        <Navbar />

        <div className="Home_Projects">
            <div className="Home_Projects_Fitter">
                <div className="Home_Projects_Domains">
                    <div className="Home_SubTitleGroup Home_Projects_Domains_Titles">
                        <h2>Games, Tools, And Assets</h2>
                        <div />
                    </div>

                    <div className="Home_Projects_Domains_Selector">
                        <div style={{ transform: `translateX(calc(${selectedTabIndex * 100}% + (${selectedTabIndex * 5}px)))` }} />
                        <a onClick={() => setSelectedTabIndex(ProjectType.Game)}>Games</a>
                        <a onClick={() => setSelectedTabIndex(ProjectType.Software)}>Software</a>
                        <a onClick={() => setSelectedTabIndex(ProjectType.Asset)}>Assets</a>
                        <a onClick={() => setSelectedTabIndex(ProjectType.Project)}>Projects</a>
                    </div>
                </div>


                <div className="Home_Projects_Filters">
                    <div className="Home_Projects_Filters_Tags">
                        <p>Tags:</p>

                        <div>
                            {tags.map(t => (
                                <a key={t}>{t}</a>
                            ))}
                        </div>
                    </div>

                    <select className="Home_Projects_Filters_Direction">
                        <option>Sort: Date</option>
                        <option>Sort: Name</option>
                    </select>
                </div>

                <section id="projects" className="Home_Projects_Container">
                    {
                        showContentSkeleton ? (<>
                            <ItemCardSkeleton isWide={false} />
                            <ItemCardSkeleton isWide={false} />
                            <ItemCardSkeleton isWide={false} />
                        </>) :
                            (
                                items.length === 0 ? (
                                    <div className="Home_Projects_Container_Empty">
                                        <label>No Content</label>
                                    </div>
                                ) : (

                                    items.map(x => {
                                        return (
                                            <ItemCard key={x.slug} itemData={x} />
                                        )
                                    })
                                )

                            )

                    }
                </section>

                <div className="Home_SectionDivision" />

                <section id="links" className="Home_Links">
                    <div className="Home_SubTitleGroup Home_Links_Title">
                        <h2>Links</h2>
                        <div />
                    </div>

                    <div className="Home_Links_Container">
                        {loadingLinks ? (
                            <>
                                <LinkCard />
                                <LinkCard />
                                <LinkCard />
                            </>
                        ) : (links.map((l, i) => (
                            <LinkCard key={i} data={l} />
                        )))}
                    </div>
                </section>

                <div className="Home_SectionDivision" />

                <section id="details" className="Home_Details">
                    <div className="Home_Details_Profile">
                        <div className="Home_Details_Profile_Icon">
                            <img src="/Profile.png" />
                        </div>

                        <a>@NeXx42</a>
                    </div>

                    <div className="Home_Details_Text">
                        <p>
                            Software developer specializing in game development, with experience creating gameplay systems and modular tools for both single-player and multiplayer projects. Focused on building flexible and reusable systems that can support a variety of game mechanics.
                        </p>
                        <p>
                            Skilled in game networking, AI systems, and gameplay frameworks, with experience implementing features that interact with backend services and handle dynamic in-game data.
                        </p>
                        <p>
                            In addition to game development, experienced in full-stack software development and automation, creating components and workflows that improve maintainability and streamline development across projects.
                        </p>

                    </div>
                </section>
            </div>
        </div>


        <Footer />
    </>)
}