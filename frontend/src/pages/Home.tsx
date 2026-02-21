import { useEffect, useRef, useState } from "react";

import ItemCard from "./Home/itemCard";
import Navbar from "../components/navbar";

import { useContent } from "../hooks/useContent";

import { ProjectType } from "../enums";
import type { Item } from "../types";

import "./Home.css";
import Footer from "../components/footer";
import LinkCard from "./Home/linkCard";

export default function Home() {
    const [selectedTabIndex, setSelectedTabIndex] = useState(ProjectType.Game);

    const {
        loading,

        games,
        software,
        assets,
        projects,

        fetchContent
    } = useContent();


    const getContent = (): Item[] => {
        switch (selectedTabIndex) {
            case ProjectType.Game: return games ?? [];
            case ProjectType.Software: return software ?? [];
            case ProjectType.Asset: return assets ?? [];
            case ProjectType.Project: return projects ?? [];
        }

        return [];
    }

    useEffect(() => {
        fetchContent(selectedTabIndex)
    }, [selectedTabIndex])


    return (<>
        <canvas id="StarContainer" />

        <div className="Home_Hero">
            <div className="Home_Hero_Orbs" />
            <div className="Home_Hero_Grid" />
            <div className="Home_Hero_Content">
                <div className="Home_Hero_Content_Person">
                    <div className="Home_Hero_Content_Crown">✦</div>
                    <h1 className="Home_Hero_Content_Name">NeXx42</h1>
                    <div className="Home_Hero_Content_Divider" />
                    <p className="Home_Hero_Content_Description">Game Dev · Software Crafter · Digital Artisan</p>
                </div>

                <div className="Home_hero_Content_Featured">
                    <ItemCard isWide={true} />
                    <ItemCard />
                    <ItemCard />
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
                            <a>All</a>
                            <a>RPG</a>
                            <a>Puzzle</a>
                            <a>Action</a>
                        </div>
                    </div>

                    <select className="Home_Projects_Filters_Direction">
                        <option>Sort: Date</option>
                        <option>Sort: Name</option>
                    </select>
                </div>

                <section id="projects" className="Home_Projects_Container">
                    {
                        loading ? (
                            <a>LOADING</a>
                        ) :
                            (

                                getContent().map((x, index) => {
                                    return (
                                        <ItemCard key={index} itemData={x} />
                                    )
                                })
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
                        <LinkCard />
                        <LinkCard />
                        <LinkCard />
                        <LinkCard />
                        <LinkCard />
                    </div>
                </section>

                <div className="Home_SectionDivision" />

                <section id="details" className="Home_Details">
                    <div>

                    </div>
                </section>
            </div>
        </div>


        <Footer />
    </>)
}