import type { ItemContentParameter } from "../../types";
import type { ContentElementProps } from "../Content";

import "./Content_Screenshots.css"


export default function (props: ContentElementProps) {
    if ((props.content.elements?.length ?? 0) === 0)
        return (<>Not Images</>);

    const elementsOrdered = props.content.elements!.slice().sort((a, b) => (a.order ?? 0) - (b.order ?? 0));

    const drawImage = (icon: ItemContentParameter, key: number) => {
        return (<div className={`Content_Screenshot_Icon ${icon.value2}`} key={key}>
            <a>⤢ expand</a>
            <img src={icon.value1} />
        </div>)
    }

    return (<div className="Content_Screenshot_Container">
        {drawImage(elementsOrdered[0], 0)}
        <div className="Content_Screenshots_SubImageContainer">
            {elementsOrdered.slice(1).map(drawImage)}
        </div>
    </div>);
}