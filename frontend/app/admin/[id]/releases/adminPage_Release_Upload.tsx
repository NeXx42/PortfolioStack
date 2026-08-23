import * as api from "@api/api.client"
import { CLIENT_UPLOAD_URL } from "@shared/config"

import { useRef, useState } from "react";

type DirectoryInputProps =
    React.InputHTMLAttributes<HTMLInputElement> & {
        webkitdirectory?: string;
    };

const inputProps: DirectoryInputProps = {
    type: "file",
    webkitdirectory: "",
    multiple: true,
};

export default function ({ projectId, releaseId, platform }: { projectId: string, releaseId: number, platform: string }) {
    const inputRef = useRef<HTMLInputElement>(null);

    const [uploadFiles, setUploadFiles] = useState<File[]>([])
    const [uploadPercentage, setUploadPercentage] = useState<number | undefined>(undefined);

    const handleFileUpload = async () => {
        setUploadPercentage(0);

        const sessionId: string = await api.releases_PrimeReleaseEngineUpload(projectId, releaseId, platform);

        for (var i = 0; i < uploadFiles.length; i++) {
            const relativePath = uploadFiles[i].webkitRelativePath.split("/").slice(1).join("/");
            const url = `${CLIENT_UPLOAD_URL}/api/Releases/${sessionId}/Upload?relativePath=${encodeURIComponent(relativePath)}`;

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

        await api.releases_CompleteReleaseEngine(sessionId);
        setUploadPercentage(100);
    }

    return (
        <>
            <h1>{platform}</h1>

            <div style={{ display: "flex", flexDirection: "column" }}>
                <div style={{ display: "flex", flexDirection: "row" }}>
                    <input type="range" min={0} max={100} value={uploadPercentage ?? 0} readOnly={true} />
                    <a>{uploadPercentage ?? 0}%</a>
                </div>

                <div style={{ display: "flex", flexDirection: "row" }}>
                    <input
                        {...inputProps}
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