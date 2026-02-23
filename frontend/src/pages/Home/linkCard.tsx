import type React from "react"
import type { Link } from "../../types"
import "./linkCard.css"

interface Props {
    data?: Link
}

export default function LinkCard(props: Props = {
    data: undefined
}) {
    return (<a className="Home_LinkCard" style={{ '--link-colour': (props.data?.customColour) } as React.CSSProperties} href={props.data?.url}>
        <img />
        <h3>{props.data?.name}</h3>
    </a>)
}