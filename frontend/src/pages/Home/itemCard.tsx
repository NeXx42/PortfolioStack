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
    return (
        <div className={`Component_ItemCard ${isWide ? "Wide" : ""}`}>
            <div className="Component_ItemCard_Icon">
                <img src={itemData?.icon} />
            </div>

            <div className="Component_ItemCard_Content">
                <div className="Component_ItemCard_Info">
                    <h2>{itemData?.name}</h2>
                    <p>{itemData?.description}</p>
                </div>

                <div className="Component_ItemCard_Action">
                    <p>Free</p>
                    <a>Download</a>
                </div>
            </div>
        </div>
    )
}