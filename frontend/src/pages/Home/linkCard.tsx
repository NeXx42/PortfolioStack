import type React from "react"
import type { Link } from "../../types"
import "./linkCard.css"

interface Props {
    data?: Link
}

export default function LinkCard(props: Props = {
    data: undefined
}) {
    console.log(props.data);
    return (<a className="Home_LinkCard" style={{ '--link-colour': (props.data?.customColour) } as React.CSSProperties} href={props.data?.url}>
        <img />
        <a>{props.data?.name}</a>
    </a>)
}