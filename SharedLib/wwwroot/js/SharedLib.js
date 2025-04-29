"use strict";

// Test connection
export function testConnection() {
    console.log("Test Connection");
};

/* ------------------------ FILE UPLOAD ------------------------ */
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

    getFileInfo() {
        return {
            name: this._file.name,
            size: this._file.size,
            lastModified: new Date(this._file.lastModified).toISOString(),
            contentType: this._file.type
        };
    }

    async getFileContent() {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();

            reader.onload = () => {
                const base64 = btoa(
                    String.fromCharCode(...new Uint8Array(reader.result))
                );
                resolve(base64);
            };

            reader.onerror = (e) => reject(e);
            reader.readAsArrayBuffer(this._file);
        });
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

    getFilesInfo() {
        return this._handlers.map(handler => handler.getFileInfo());
    }

    async getFilesContent() {
        const promises = this._handlers.map(handler => handler.getFileContent());
        return await Promise.all(promises);
    }
}

export function createFileHandler(inputElement) {
    const files = Array.from(inputElement.files || []);

    if (files.length > 1) {
        return new MultiFileHandler(files);
    }
    else {
        return new FileHandler(files[0]);
    }
}

export function getFilesInfo(instance) {
    if (instance instanceof MultiFileHandler) {
        return instance.getFilesInfo();
    }
    else {
        return instance.getFileInfo();
    }
}

export function getFilesData(instance) {
    if (instance instanceof MultiFileHandler) {
        return instance.getFilesData();
    }
    else {
        return instance.getFileData();
    }
}

export function getFilesContent(instance) {
    if (instance instanceof MultiFileHandler) {
        return instance.getFilesContent();
    }
    else {
        return instance.getFileContent();
    }
}

/* ------------------------ FILE UPLOAD ------------------------ */

/* ------------------------ FILE DOWNLOAD ------------------------ */
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
/* ------------------------ FILE DOWNLOAD ------------------------ */
