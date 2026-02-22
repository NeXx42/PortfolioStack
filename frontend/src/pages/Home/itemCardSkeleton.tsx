import "./itemCard.css"
import "./itemCardSkeleton.css"


export default function ItemCardSkeleton({
    isWide = false,
}: { isWide: boolean }) {
    return (
        <div className={`Component_ItemCard ${isWide ? "Wide" : ""}`}>
            <div className="Component_ItemCard_Icon">
                <img />
            </div>

            <div className="Component_ItemCard_Content">
                <div className="Component_ItemCard_Info">
                    <h2></h2>
                    <p></p>
                </div>

                <div className="Component_ItemCard_Action">
                    <p>Free</p>
                    <a>Download</a>
                </div>
            </div>
        </div>
    )
}