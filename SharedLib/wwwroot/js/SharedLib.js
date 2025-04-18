"use strict";

function SayHello() {
    console.log("Test Connection");
};

function GetNativeFile(inputElement) {
    return inputElement.files[0];
}

/// 基于 Blazor WebAssembly 的 JavaScript 互操作 (JS interop) 文件读取 + .NET 异步流处理 

/**
 * upload a single file
 * @param {any} inputElement
 */
function GetLocalFile(inputElement) {
    if (inputElement.files.length == 0) return null;

    console.log("upload file ...");

    const file = inputElement.files[0];

    const fileObj = {
        getAttribute: (attr) => {
            const attributes = {
                name: file.name,
                lastModified: new Date(file.lastModified).toISOString(),
                size: file.size,
                contentType: file.type,
            }

            return attributes[attr] || "not find";
        },
        getAttributes: () => {
            return {
                name: file.name,
                lastModified: new Date(file.lastModified).toISOString(),
                size: file.size,
                contentType: file.type,
            }
        },
        getStreamReference: () => file
    };

    return fileObj;
}

/**
 * upload local files
 * @param {any} inputElement
 */
function GetLocalFiles(inputElement) {
    console.log("upload files ...");



}

class ChunkUploader {
    constructor(file, chunkSize = 5 * 1024 * 1024) {
        this.file = file;
        this.chunkSize = chunkSize;
        this.chunks = Math.ceil(file.size / chunkSize);
        this.uploaded = 0;
    }

    async upload() {
        for (let i = 0; i < this.chunks; i++) {
            const chunk = this.file.slice(
                i * this.chunkSize,
                Math.min((i + 1) * this.chunkSize, this.file.size)
            );

            const formData = new FormData();
            formData.append('chunk', chunk);
            formData.append('chunkIndex', i);
            formData.append('totalChunks', this.chunks);
            formData.append('fileName', this.file.name);

            await fetch('/api/upload/chunk', {
                method: 'POST',
                body: formData
            });

            this.uploaded++;
            updateProgress(this.uploaded / this.chunks * 100);
        }
    }
}


export { SayHello, GetNativeFile, GetLocalFile, GetLocalFiles };