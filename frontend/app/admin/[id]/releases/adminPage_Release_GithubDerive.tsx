import * as api from "@api/api.client"

import { ProjectRelease, ProjectReleaseDownload } from "@/app/shared/types";
import { useState } from "react";

export default function ({ projectId, release }: { projectId: string, release: ProjectRelease }) {
    const [githubUrl, setGithubUrl] = useState("");
    const [content, setContent] = useState<ProjectReleaseDownload[]>([]);

    const updateContent = (id: number, to: string) => {
        setContent(prev => prev.map((p, i) => {
            if (i !== id) return p;
            return {
                ...p,
                platform: to
            }
        }))
    }

    const removeContent = (id: number) => {
        setContent(prev => prev.filter((_, i) => i !== id))
    }

    const work = () => {
        const url = githubUrl.replace("https://github.com", "https://api.github.com/repos").replace("releases/tag", "releases/tags");

        fetch(url).then(r => r.json()).then(j => {
            setContent(j.assets.map((a: any) => ({
                platform: a.name,
                downloadLink: a.browser_download_url,
                size: a.size
            })))
        });
    }

    const save = () => {
        api.admin_SaveProjectRelease(projectId, {
            ...release,
            downloads: content
        })
            .then(_ => document.location.reload())
            .catch(e => alert(e.message))
    }

    return (<>
        <div style={{ display: "grid", gridTemplateColumns: "100px auto" }}>
            <a>Github url</a>
            <input value={githubUrl} onChange={e => setGithubUrl(e.target.value)} />
        </div>

        <button onClick={work}>Work</button>

        <div>
            {content.map((c, i) => (
                <div key={i}>
                    <input value={c.platform} onChange={e => updateContent(i, e.target.value)} />
                    <button onClick={() => removeContent(i)}>x</button>
                </div>
            ))}
        </div>

        <button onClick={save}>Save</button>
    </>)
}