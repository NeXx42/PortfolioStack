import Content_About from "@/app/[slug]/Content_About";
import { ProjectAdminContent } from "./page";

export default function (props: ProjectAdminContent) {
    return <Content_About content={props.content} />
}