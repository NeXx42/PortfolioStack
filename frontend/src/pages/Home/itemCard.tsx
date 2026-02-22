import { useNavigate } from "react-router-dom";
import type { Item } from "../../types"
import "./itemCard.css"

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
                    <p>Free</p>
                    <a>Download</a>
                </div>
            </div>
        </div>
    )
}