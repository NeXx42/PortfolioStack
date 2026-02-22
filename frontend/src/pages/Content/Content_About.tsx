import type { ItemContent } from "../../types";

interface Props {
    content: ItemContent,
    isAdmin: boolean
}

export default function Content_About(props: Props) {


    return props.isAdmin ? (
        <textarea />
    ) : (
        props.content.elements?.map(e => <span>{e.value1}</span>)
    )
}