import * as api from "@api/api.client"
import { CLIENT_UPLOAD_URL } from "@shared/config"

import { useRef, useState } from "react";

export default function () {
    const inputRef = useRef<HTMLInputElement>(null);

    const [status, setStatus] = useState<string | undefined>(undefined);
    const [uploadFile, setUploadFile] = useState<File | undefined>()

    const handleFileUpload = async () => {
        if (uploadFile === undefined)
            return;

        setStatus("uploading");

        const url = `${CLIENT_UPLOAD_URL}/api/Releases/Upload?fileName=${encodeURIComponent(uploadFile.name)}`;
        const response = await fetch(url, {
            method: "PUT",
            credentials: "include",
            headers: {
                "Content-Type": uploadFile.type || "application/octet-stream",
            },
            body: uploadFile,
        });

        if (!response.ok) {
            setStatus("failed");
        }
    }

    return (
        <>
            <h1>Upload file to server</h1>

            <a>Status - {status} </a>
            <a>Filename - {uploadFile?.name}</a>

            <div style={{ display: "flex", flexDirection: "column" }}>
                <div style={{ display: "flex", flexDirection: "row" }}>
                    <input type="file" multiple={false} onChange={e => setUploadFile(e.target.files![0])} />
                    <button onClick={handleFileUpload}>Upload</button>
                </div>
            </div>
        </>
    )
}