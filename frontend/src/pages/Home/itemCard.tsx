import { useNavigate } from "react-router-dom";
import type { Item } from "../../types"
import "./itemCard.css"
import { ProjectType } from "../../enums";

interface Props {
    isWide?: boolean,
    itemData?: Item,
}

export default function ItemCard({
    isWide = false,
    itemData = undefined
}: Props) {
    const navigate = useNavigate();

    const navigateToPage = () => {
        navigate(`${itemData?.slug}/content`)
    };

    const getCost = (): string => {
        if (itemData?.type === ProjectType.Game)
            return itemData.cost == null ? "Free" : itemData.cost!.toString();

        return "";
    }

    const getAction = () => {
        if (itemData?.cost !== null)
            return "Purchase"

        switch (itemData?.type) {
            case ProjectType.Software:
                return "Download";
        }

        return "View";
    }

    return (
        <div onClick={navigateToPage} className={`Component_ItemCard ${isWide ? "Wide" : ""}`}>
            <div className="Component_ItemCard_Icon">
                <img src={itemData?.icon} />
            </div>

            <div className="Component_ItemCard_Content">
                <div className="Component_ItemCard_Info">
                    <h2>{itemData?.gameName}</h2>
                    <p>{itemData?.shortDescription}</p>
                </div>

                <div className="Component_ItemCard_Action">
                    <p>{getCost()}</p>
                    <a>{getAction()}</a>
                </div>
            </div>
        </div>
    )
}