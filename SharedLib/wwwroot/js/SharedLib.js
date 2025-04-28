"use strict";

// Test connection
export function testConnection() {
    console.log("Test Connection");
};

// single file upload
export class FileHandler {
    constructor(file) {
        this._file = file;
    }

    get filename() {
        return this._file.name;
    }

    get size() {
        return this._file.size;
    }

    get lastModified() {
        return this._file.lastModified;
    }

    get contentType() {
        return this._file.contentType;
    }

    getAttributes() {
        return {
            filename: this._file.name,
            size: this._file.size,
            lastModified: new Date(this._file.lastModified).toISOString(),
            contentType: this._file.type
        };
    }

    getStreamReference() {
        return this._file;
    }
}

// multi files upload
export class MultiFileHandler {
    constructor(files) {
        this._handlers = files.map(file => new FileHandler(file));
    }

    get count() {
        return this._handlers.length;
    }

    get totalSize() {
        return this._handlers.reduce((sum, handler) => sum + handler.size, 0);
    }

    getHandler(index) {
        return this._handlers[index] || null;
    }

    getAllAttributes() {
        return this._handlers.map(handler => handler.getAttributes());
    }

    getAllStreamReferences() {
        return this._handlers.map(handler => handler.getStreamReference());
    }
}

export function createFileHandler(inputElement) {
    const files = Array.from(inputElement.files);

    if (file.length > 1) {
        return new MultiFileHandler(files);
    }
    else {
        return new FileHandler(files[0]);
    }
}

export function getFileAttributes(instance) {
    if (instance instanceof MultiFileHandler) {
        return instance.getAllAttributes();
    }
    else {
        return instance.getAttributes();
    }
}

export function getFileStreamReference(instance) {
    if (instance instanceof MultiFileHandler) {
        return instance.getAllStreamReferences();
    }
    else {
        instance.getStreamReference();
    }
}

export function downloadFile(filename, data, mimeType) {
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
