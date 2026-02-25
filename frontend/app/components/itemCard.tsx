"use client"
import { useRouter } from "next/navigation";

import type { Item } from "@shared/types"
import { ProjectType } from "@shared/enums";

import "./itemCard.css"

interface Props {
    isWide?: boolean,
    itemData?: Item,
}


export default function ItemCard({
    isWide = false,
    itemData = undefined
}: Props) {
    const navigate = useRouter();

    const navigateToPage = () => {
        navigate.push(`${itemData?.slug}`)
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