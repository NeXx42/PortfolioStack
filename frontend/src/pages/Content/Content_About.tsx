import type { ContentElementProps } from "../Content";


export default function Content_About(props: ContentElementProps) {
    return (
        <>
            {props.content.elements?.map(e => <span>{e.value1}</span>)}
        </>
    )
}