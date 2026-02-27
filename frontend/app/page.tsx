import { fetchLinks, fetchFeaturedContent } from "@api/api.server"

import ItemCard from "./components/itemCard";
import Navbar from "@shared/components/navbar";

import { ProjectType } from "@shared/enums";

import "./Home.css";
import Footer from "@shared/components/footer";
import LinkCard from "./components//linkCard";
import ItemCardSkeleton from "./components/itemCardSkeleton";
import CardList from "./components/cardList";
import { Project, Link } from "./shared/types";
import StarContainer from "./shared/components/starContainer";

export default async function () {

  var links: Link[] | null = null;
  var content: Project[] | null = null;

  try {
    links = await fetchLinks();
    content = await fetchFeaturedContent();
  }
  catch {

  }

  return (<>
    <StarContainer />

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
          {content !== undefined && ([1, 0, 2].map(x =>
            (<ItemCard key={x} itemData={content![x]} />)
          ))}
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

        <CardList />

        <div className="Home_SectionDivision" />

        <section id="links" className="Home_Links">
          <div className="Home_SubTitleGroup Home_Links_Title">
            <h2>Links</h2>
            <div />
          </div>

          <div className="Home_Links_Container">
            {links?.map((l, i) => (
              <LinkCard key={i} data={l} />
            ))}
          </div>
        </section>

        <div className="Home_SectionDivision" />

        <section id="about" className="Home_Details">
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