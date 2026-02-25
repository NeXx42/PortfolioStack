"use client"

import { useEffect, useState } from "react";
import { ProjectType } from "@shared/enums";
import { useContent } from "@hooks/useContent";
import ItemCardSkeleton from "./itemCardSkeleton";
import ItemCard from "./itemCard";

export default function () {
    const [selectedTabIndex, setSelectedTabIndex] = useState(ProjectType.Game);
    const [tags, setTags] = useState<string[]>([])

    const {
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

    return (<section id="projects">
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

        <div className="Home_Projects_Container">
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
        </div>
    </section>)
}