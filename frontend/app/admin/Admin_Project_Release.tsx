import { Dispatch, SetStateAction, useState } from "react"
import { Project, ProjectRelease, ProjectReleaseDownload } from "../shared/types"

interface props {
    project: Project | undefined,
    setProject: Dispatch<SetStateAction<Project | undefined>>
}

export default function (props: props) {
    const [downloadCount, setNewDownloadCount] = useState(1);
    const [releaseCount, setNewReleaseCount] = useState(1);

    const addRelease = function () {
        setNewReleaseCount(releaseCount + 1);
        props.setProject(prev => {
            if (prev === undefined) return;

            const res: ProjectRelease[] = [...prev.releases, {
                id: -releaseCount,
                downloads: [],
                size: "",
                date: new Date(),
                version: "",
            }]

            return {
                ...prev,
                releases: res
            }
        })
    }

    const updateRelease = function (releaseId: number, key: keyof ProjectRelease, value: any) {
        props.setProject(prev => {
            if (prev === undefined) return;
            return {
                ...prev,
                releases: prev.releases.map(x => {
                    if (x.id !== releaseId) return x;
                    return {
                        ...x,
                        [key]: value
                    }
                })
            }
        })
    }


    const addDownload = function (releaseId: number) {
        setNewDownloadCount(downloadCount + 1);
        props.setProject(prev => {
            if (prev === undefined) return;
            return {
                ...prev,
                releases: prev.releases.map(r => {
                    if (r.id !== releaseId) return r;
                    return {
                        ...r,
                        downloads: [...r.downloads, {
                            id: -downloadCount,
                            link: ""
                        }]
                    }
                })
            }
        })
    }


    const updateDownload = function (releaseId: number, downloadId: number, key: keyof ProjectReleaseDownload, value: any) {
        props.setProject(prev => {
            if (prev === undefined) return;
            return {
                ...prev,
                releases: prev.releases.map(x => {
                    if (x.id !== releaseId) return x;
                    return {
                        ...x,
                        downloads: x.downloads.map(d => {
                            if (d.id !== downloadId) return d;
                            return {
                                ...d,
                                [key]: value
                            }
                        })
                    }
                })
            }
        })
    }



    return (
        <ol>
            {props.project?.releases.map((x, i) => (

                <li key={i}>
                    <div>
                        <input type="text" onChange={e => updateRelease(x.id, "version", e.target.value)} value={x.version} />
                        <input type="text" onChange={e => updateRelease(x.id, "size", e.target.value)} value={x.size} />
                        <input type="date" onChange={e => updateRelease(x.id, "date", e.target.value)} value={x.date.toString()} />

                        <ol>
                            {x.downloads.map((d, di) => (
                                <li key={di}>
                                    <input type="text" onChange={e => updateDownload(x.id, d.id, "link", e.target.value)} value={d.link} />
                                </li>
                            ))}
                            <button type="button" onClick={() => addDownload(x.id)}>Add Download</button>
                        </ol>
                    </div>
                </li>

            ))}
            <button type="button" onClick={addRelease}>Add Release</button>
        </ol>
    )
}