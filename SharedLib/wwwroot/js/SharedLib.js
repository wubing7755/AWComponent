"use strict";

function TestConnection() {
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

/**
 * 通用浏览器文件下载工具
 * @param {string} filename - 下载的文件名
 * @param {string|Blob|Uint8Array} data - 文件数据（支持 Base64/Blob/二进制数组/文本）
 * @param {string} [mimeType] - 可选 MIME 类型（如未提供则自动推断）
 */
function downloadFile(filename, data, mimeType) {
    // 1. 自动推断 MIME 类型（如果未显式指定）
    if (!mimeType) {
        const ext = filename.split('.').pop().toLowerCase();
        mimeType = {
            'txt': 'text/plain',
            'xml': 'application/xml',
            'json': 'application/json',
            'csv': 'text/csv',
            'png': 'image/png',
            'jpg': 'image/jpeg',
            'pdf': 'application/pdf',
            'zip': 'application/zip'
        }[ext] || 'application/octet-stream';
    }

    // 2. 处理不同类型的数据输入
    let blob;
    if (data instanceof Blob) {
        blob = data;
    } else if (data instanceof Uint8Array) {
        blob = new Blob([data], { type: mimeType });
    } else if (typeof data === 'string' && data.startsWith('data:')) {
        // 已经是 DataURL 格式（如 Base64）
        const link = document.createElement('a');
        link.href = data;
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        return;
    } else {
        // 普通文本或 Base64（无前缀）
        blob = new Blob([data], { type: mimeType });
    }

    // 3. 创建下载链接并触发点击
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    link.style.display = 'none';
    document.body.appendChild(link);
    link.click();

    // 4. 清理内存
    setTimeout(() => {
        document.body.removeChild(link);
        URL.revokeObjectURL(url);
    }, 100);
}

window.downloadFile = downloadFile;

export { TestConnection, GetNativeFile, GetLocalFile, GetLocalFiles, downloadFile };