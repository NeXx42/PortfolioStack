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
                <img src="https://nexx42.github.io/PortfolioSite/Content/8d41e4f3-961b-4497-86dd-768335c887dd/Icon.png" />
            </div>

            <div className="Component_ItemCard_Content">
                <div className="Component_ItemCard_Info">
                    <h2>Cool Game</h2>
                    <p>description</p>
                </div>

                <div className="Component_ItemCard_Action">
                    <p>Free</p>
                    <a>Download</a>
                </div>
            </div>
        </div>
    )
}