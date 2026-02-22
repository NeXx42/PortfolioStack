import type { ItemContentParameter } from "../../types";
import type { ContentElementProps } from "../Content";

import "./Content_Screenshots.css"


export default function (props: ContentElementProps) {
    if ((props.content.elements?.length ?? 0) === 0)
        return (<>Not Images</>);

    const drawImage = (icon: ItemContentParameter, key: number) => {
        return (<div className="Content_Screenshot_Icon" key={key}>
            <img src={icon.value1} />
        </div>)
    }

    return (<div className="Content_Screenshot_Container">
        {drawImage(props.content.elements![0], 0)}
        <div className="Content_Screenshots_SubImageContainer">
            {props.content.elements?.slice(1).map(drawImage)}
        </div>
    </div>);
}