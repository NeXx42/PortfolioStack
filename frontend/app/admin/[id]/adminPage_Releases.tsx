import * as api from "@api/api.client"

import { Project, ProjectRelease, ProjectReleaseDownload, releaseStatus } from "@/app/shared/types"
import useRequest from "@/app/hooks/useRequest"

import { ReactNode, useState } from "react";

import "./adminPage_Releases.css"
import AdminPage_Release_GithubDerive from "./releases/adminPage_Release_GithubDerive";
import AdminPage_Release_Upload from "./releases/adminPage_Release_Upload";

type Popups = "None" | "Github derive" | "Uploading";

export default function ({ content }: { content: Project }) {
    const { data } = useRequest(a => a.admin_GetProjectReleases(content.id));

    const [selectedRelease, setSelectedRelease] = useState<ProjectRelease | undefined>(undefined)
    const [selectedReleasePlatform, setSelectedReleasePlatform] = useState(0)

    const [popup, setPopup] = useState<Popups>("None");

    const createRelease = () => {
        if (confirm("Create release?")) {
            const release: ProjectRelease = {
                versionId: -1,
                version: "new",
                patchNotes: "",
                downloads: []
            }

            api.admin_SaveProjectRelease(content.id, release)
                .then(_ => document.location.reload())
                .catch(e => alert(e.message));
        }
    }

    const updateRelease = <K extends keyof ProjectRelease>(prop: K, value: ProjectRelease[K]) => {
        setSelectedRelease(prev => ({
            ...prev!,
            [prop]: value
        }))
    }

    const updateReleaseDownload = <K extends keyof ProjectReleaseDownload>(downloadIndex: number, prop: K, value: ProjectReleaseDownload[K]) => {
        setSelectedRelease(prev => ({
            ...prev!,
            downloads: prev?.downloads.map((d, i) => {
                if (i !== downloadIndex) return d;
                return {
                    ...d,
                    [prop]: value
                }
            }) ?? []
        }))
    }

    const addRelease = () => {
        setSelectedRelease(prev => ({
            ...prev!,
            downloads: [...(prev?.downloads ?? []), {
                platform: "wins",

                downloadLink: "",
                releaseEngineManifestLink: "",

                entryPoint: "",
                size: 0,
            }]
        }));
    }

    const removeRelease = (index: number) => {
        setSelectedRelease(prev => ({
            ...prev!,
            downloads: prev?.downloads?.filter((_, i) => i !== index) ?? []
        }));
    }

    const saveRelease = () => {
        api.admin_SaveProjectRelease(content.id, selectedRelease!)
            .then(_ => document.location.reload())
            .catch(e => alert(e.message));
    }

    const drawPopup = (): ReactNode => {
        const wrap = (node: ReactNode) => {
            return (
                <div className="adminPage_Releases_Popup" onClick={() => setPopup("None")}>
                    <div className="adminPage_Releases_Popup_Content" onClick={e => e.stopPropagation()}>
                        {node}
                    </div>
                </div>
            )
        }

        switch (popup) {
            case "Github derive":
                return wrap(<AdminPage_Release_GithubDerive projectId={content.id} release={selectedRelease!} />)

            case "Uploading":
                return wrap(<AdminPage_Release_Upload projectId={content.id} releaseId={selectedRelease!.versionId} platform={selectedRelease?.downloads[selectedReleasePlatform].platform!} />)
        }

        return (<></>)
    }

    const drawRelease = () => {
        if (selectedRelease === undefined) return;

        return (
            <>
                <h1>{selectedRelease.version}</h1>

                <div>
                    <a>Version name</a>
                    <input value={selectedRelease.version} onChange={e => updateRelease("version", e.target.value)} />
                </div>

                <div>
                    <a>Status</a>
                    <select value={selectedRelease.status} onChange={e => updateRelease("status", Number.parseInt(e.target.value))}>
                        {releaseStatus.map((m, i) => <option key={m} value={i}>{m}</option>)}
                    </select>
                </div>

                <div>
                    <a>Patch notes</a>
                    <textarea value={selectedRelease.patchNotes} onChange={e => updateRelease("patchNotes", e.target.value)} />
                </div>

                <div>
                    <a>derive from github</a>
                    <button onClick={() => setPopup("Github derive")}>Derive</button>
                </div>

                <div className="adminPage_Releases_Content_Downloads">
                    {selectedRelease.downloads.map((d, i) => (
                        <div key={i}>
                            <button onClick={() => setSelectedReleasePlatform(i)}>{d.platform}</button>
                            <button onClick={() => removeRelease(i)}>x</button>
                        </div>
                    ))}
                    <button onClick={addRelease}>Add</button>
                </div>

                {drawDownload()}
                <button onClick={saveRelease}>Save</button>
            </>
        )
    }

    const drawDownload = () => {
        const download = selectedRelease?.downloads[selectedReleasePlatform];

        if (download === undefined)
            return (<></>);

        return (
            <div className="adminPage_Releases_Content_Download">
                <div>
                    <a>Platform</a>
                    <div>
                        <input type="text" value={download.platform} onChange={e => updateReleaseDownload(selectedReleasePlatform, "platform", e.target.value)} />
                        <select value={download.platform} onChange={e => updateReleaseDownload(selectedReleasePlatform, "platform", e.target.value)}>
                            <option value={"Windows"} label="Windows" />
                            <option value={"Linux"} label="Linux" />
                        </select>
                    </div>
                </div>

                <div>
                    <a>Entrypoint</a>
                    <input value={download.entryPoint} onChange={e => updateReleaseDownload(selectedReleasePlatform, "entryPoint", e.target.value)} />
                </div>

                <div>
                    <a>Download link</a>
                    <input value={download.downloadLink} onChange={e => updateReleaseDownload(selectedReleasePlatform, "downloadLink", e.target.value)} />
                </div>

                <div>
                    <a>Release engine link</a>
                    <input value={download.releaseEngineManifestLink} onChange={e => updateReleaseDownload(selectedReleasePlatform, "releaseEngineManifestLink", e.target.value)} />
                </div>
                <div>
                    <a>Upload to release engine</a>
                    <button onClick={() => setPopup("Uploading")}>Open</button>
                </div>

                <div>
                    <a>Size (bytes)</a>
                    <input value={download.size} onChange={e => updateReleaseDownload(selectedReleasePlatform, "size", Number.parseInt(e.target.value))} />
                </div>
            </div>
        )
    }


    return (
        <div className="adminPage_Releases">
            <div className="adminPage_Releases_Entries">
                <button onClick={createRelease}>Create</button>
                {
                    data?.map(d => (
                        <button key={d.versionId} onClick={() => setSelectedRelease(d)}>{d.version}</button>
                    ))
                }
            </div>

            <div className="adminPage_Releases_Content">
                {drawRelease()}
            </div>

            {drawPopup()}
        </div>
    )
}