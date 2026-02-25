import type { MouseEventHandler } from "react";
import "./commonButton.css"

interface Props {
    onClick?: MouseEventHandler<HTMLElement>,
    label: string
}

export default function CommonButton(props: Props) {


    return (
        <button className="Common_Button" onClick={props.onClick}>{props.label}</button>
    );
}