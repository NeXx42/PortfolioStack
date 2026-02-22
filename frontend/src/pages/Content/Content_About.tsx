import type { ItemContent } from "../../types";

interface Props {
    content: ItemContent,
}

export default function Content_About(props: Props) {
    return (
        <>
            {props.content.elements?.map(e => <span>{e.value1}</span>)}
        </>
    )
}