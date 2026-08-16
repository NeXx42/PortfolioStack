import type { ContentElementProps } from "./page";


export default function Content_About(props: ContentElementProps) {
    return (
        <>
            {props.content.elements?.sort((a, b) => a.id - b.id).map((e, i) => <p className="legibleText" key={i} dangerouslySetInnerHTML={{ __html: e.value1 }} />)}
        </>
    )
}