import * as api from "@api/api.client"
import { URL } from "@api/api.shared"

import { useState } from "react";

export default function ({ projectId, releaseId, platform }: { projectId: string, releaseId: number, platform: string }) {
    const [uploadFiles, setUploadFiles] = useState<File[]>([])
    const [uploadPercentage, setUploadPercentage] = useState<number | undefined>(undefined);


    const handleFileUpload = async () => {
        setUploadPercentage(0);

        const uri: string = await api.admin_PrimeReleaseEngineUpload(projectId, releaseId, platform);
        console.log(uri);

        for (var i = 0; i < uploadFiles.length; i++) {
            const relativePath = uploadFiles[i].webkitRelativePath.split("/").slice(1).join("/");
            const url = `${URL}/${uri}&relativePath=${encodeURIComponent(relativePath)}`;

            await new Promise(resolve => setTimeout(resolve, 10));

            const response = await fetch(url, {
                method: "PUT",
                credentials: "include",
                headers: {
                    "Content-Type": uploadFiles[i].type || "application/octet-stream",
                },
                body: uploadFiles[i],
            });

            if (!response.ok) {
                throw new Error(`Failed to upload ${relativePath}`);
            }

            setUploadPercentage((i / uploadFiles.length) * 100);
        }

        setUploadPercentage(100);
    }

    return (
        <>
            <h1>{platform}</h1>

            <div style={{ display: "flex", flexDirection: "column" }}>
                <div style={{ display: "flex", flexDirection: "row" }}>
                    <input type="range" min={0} max={100} value={uploadPercentage} onChange={_ => { }} />
                    <a>{uploadPercentage ?? 0}%</a>
                </div>

                <div style={{ display: "flex", flexDirection: "row" }}>
                    <input
                        type="file"
                        webkitdirectory=""
                        directory=""
                        multiple
                        onChange={e => setUploadFiles(Array.from(e.target.files ?? []))}
                    />
                    <button onClick={handleFileUpload}>Upload</button>
                </div>
            </div>

            <div style={{ display: "flex", flexDirection: "column" }}>
                {uploadFiles.map((f, i) => <a key={i}>{f.webkitRelativePath.split("/").slice(1).join("/")}</a>)}
            </div>

        </>
    )
}