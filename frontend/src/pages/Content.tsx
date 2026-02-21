import { useParams } from "react-router-dom";
import { useGame } from "../hooks/useGame";

import "./Content.css"

import NotFound from "./NotFound";
import Navbar from "../components/navbar";
import type { ReactNode } from "react";
import Footer from "../components/footer";

export default function Content() {
    const { guid } = useParams();

    if (guid === undefined)
        return <NotFound />

    const { content, loading, error } = useGame(guid ?? "");

    const wrapSection = (sectionName: string, children: ReactNode) => {
        return (
            <section className="Content_Section">
                <h2>{sectionName}</h2>
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

                </div>
                <h1>{content?.name}</h1>

            </section>
        )
    }

    const drawScreenshots = () => {
        return wrapSection("Screenshots", (
            <></>
        ));
    }

    const drawReleases = () => {
        return (
            <section>

            </section>
        )
    }

    const drawAbout = () => {
        return wrapSection("About", (
            <p> Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur lobortis egestas magna ut lacinia. Donec quis scelerisque nibh. Fusce tincidunt ultricies turpis. Aliquam in dictum risus, vitae volutpat nibh. Curabitur sed eros eleifend, placerat justo vel, dignissim libero. Integer vestibulum sit amet dolor quis rutrum. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Suspendisse eget mauris et massa pellentesque hendrerit a a risus. Pellentesque dapibus ultrices sem ac fringilla. Suspendisse potenti. Praesent quis mi sit amet ante vehicula consequat. Nullam lacinia elit non feugiat congue. Nunc vehicula cursus quam a porta. Sed consequat ipsum eu commodo iaculis.
                <br />
                <br />
                Morbi vitae placerat lacus. Nullam fringilla ante justo, ac bibendum nibh tincidunt sed. Donec ornare euismod diam, vitae maximus sapien malesuada quis. Sed eget magna suscipit, pulvinar massa nec, laoreet est. Interdum et malesuada fames ac ante ipsum primis in faucibus. Etiam volutpat leo odio, non facilisis arcu ornare a. Nulla accumsan sem in nisi lacinia laoreet. Vivamus vehicula dolor eget justo elementum, sed faucibus est mollis. Nam lacus quam, pharetra eget dignissim ut, semper sit amet sapien. Integer at ipsum in nulla placerat hendrerit vitae fringilla nisl.
                <br />
                <br />
                Ut consequat turpis nulla, et auctor felis dapibus in. Donec non lacus ipsum. Vestibulum ullamcorper sed libero vel pulvinar. Sed pulvinar diam quis scelerisque vulputate. Aenean interdum a odio ac finibus. Vestibulum ex sapien, pellentesque rhoncus interdum sagittis, dignissim vel massa. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; In erat est, pretium quis porta eu, convallis sed nisl. Quisque erat arcu, eleifend sit amet risus a, consequat laoreet odio. In nec nibh ac lorem tincidunt auctor. In metus libero, ullamcorper convallis felis non, convallis tempus nisi. Maecenas vitae vulputate erat. Curabitur aliquam rutrum interdum. Fusce at sagittis metus. Pellentesque nec magna est. Vestibulum venenatis consectetur mauris, quis dictum lacus sodales in. </p>
        ));
    }

    const drawFeatures = () => {
        return wrapSection("Features", (
            <></>
        ));
    }

    const drawRequirements = () => {
        return wrapSection("Requirements", (
            <></>
        ));
    }



    const drawPage = () => {
        return (
            <div className="Content">
                <ol className="Content_BreadCrumbs">
                    <li><a>Home</a></li>
                    <li><a>Projects</a></li>
                    <li><a>Games</a></li>
                    <li><a>{content?.name}</a></li>
                </ol>

                <div className="Content_ContentFitter">
                    <div className="Content_Hero">
                        <img src={content?.icon} />

                        <div className="Content_Hero_Shine" />
                        <div className="Content_Hero_Glow" />
                        <div className="Content_Hero_Vignette" />
                    </div>

                    <div className="Content_Main">
                        {drawGet()}
                        {drawTitle()}
                        {drawScreenshots()}
                        {drawReleases()}
                        {drawAbout()}
                        {drawRequirements()}
                        {drawFeatures()}
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