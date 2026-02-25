import "./itemCard.css"
import "./itemCardSkeleton.css"


export default function ItemCardSkeleton({
    isWide = false,
}: { isWide: boolean }) {
    return (
        <div className={`Component_ItemCard Skeleton ${isWide ? "Wide" : ""}`}>
            <div className="Component_ItemCard_Icon Skeleton">

            </div>

            <div className="Component_ItemCard_Content Skeleton">
                <div className="Component_ItemCard_Info Skeleton">
                    <h2></h2>
                    <p></p>
                </div>

                <div className="Component_ItemCard_Action Skeleton">
                    <p></p>
                    <a></a>
                </div>
            </div>
        </div>
    )
}