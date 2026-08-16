import type { MouseEventHandler } from "react";
import "./commonButton.css"

interface Props {
    onClick?: MouseEventHandler<HTMLElement>,
    label: string,
    type?: "submit" | "reset" | "button" | undefined,
}

export default function CommonButton(props: Props) {
    return (
        <button type={props.type ?? "button"} className="Common_Button" onClick={props.onClick}>{props.label}</button>
    );
}