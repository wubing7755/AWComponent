"use strict";

//#region 测试连通性

export function testConnection() {
    console.log("Test Connection");
};

//#endregion

//#region 文件上传
// single file upload
class FileHandler {
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

    getMetaData() {
        return {
            name: this._file.name,
            size: this._file.size,
            lastModified: new Date(this._file.lastModified).toISOString(),
            contentType: this._file.type,
        };
    }
}

// multi files upload
class MultiFileHandler {
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

    getMetaData() {
        return this._handlers.map(handler => handler.getMetaData());
    }
}

export function getFileMetaData(inputElement) {
    const files = Array.from(inputElement.files || []);

    if (files.length > 1) {
        const mFileHandler = new MultiFileHandler(files);
        return mFileHandler.getMetaData();
    }
    else {
        const sFileHandler = new FileHandler(files[0]);
        return sFileHandler.getMetaData();
    }
}

//#region 预览小文件内容

async function getFilesContent(chunkSize = 10 * 1024) {
    const promises = this._handlers.map(handler => handler.getFileContent(chunkSize));
    return await Promise.all(promises);
}

// 10KB 分块
async function getFileContent(chunkSize = 10 * 1024) {
    // 最大 500 KB
    const MAX_FILESIZE = 500 * 1024;
    if (this._file.size > MAX_FILESIZE) {
        return {
            isExLimit: true
        }
    }

    if (this._file.size <= chunkSize) {
        const base64 = await this._readChunk(this._file);
        return {
            isChunked: false,
            data: base64
        }
    } else {
        // 大文件返回分块信息
        const totalChunks = Math.ceil(this._file.size / chunkSize);
        const chunkPromises = [];

        // 并行读取所有分块
        for (let i = 0; i < totalChunks; i++) {
            const chunk = this._file.slice(i * chunkSize, (i + 1) * chunkSize);
            chunkPromises.push(
                this._readChunk(chunk).then(base64 => ({
                    index: i,
                    total: totalChunks,
                    data: base64,
                    size: chunk.size
                }))
            );
        }

        const chunks = await Promise.all(chunkPromises);
        return {
            isChunked: true,
            fileName: this._file.name,
            fileSize: this._file.size,
            chunks: chunks
        };
    }
}

function _readChunk(chunk) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();

        // onload 事件在读取操作完成时触发
        reader.onload = () => {
            const bytes = new Uint8Array(reader.result);
            let binaryString = '';

            for (const byte of bytes) {
                binaryString += String.fromCharCode(byte);
            }

            resolve(btoa(binaryString));
        };

        // onerror 事件在读取操作失败时触发
        reader.onerror = (e) => reject(e);
        // 开始读取文件
        reader.readAsArrayBuffer(chunk);

    });
}

//#endregion

//#region 并发分块上传

export async function uploadFile(inputElement, url, chunkSize) {
    const files = inputElement.files;
    if (!files || files.length === 0) {
        throw new Error("No files selected");
    }

    const file = files[0];
    await chunkFileUpLoad(file, url, chunkSize);
}

export async function uploadFiles(inputElement, url, chunkSize) {
    const files = inputElement.files;
    if (!files || files.length === 0) {
        throw new Error("No files selected");
    }

    for (const file of files) {
        await chunkFileUpLoad(file, url, chunkSize);
    }
}

// 文件分块
async function chunkFileUpLoad(file, url, chunkSize) {
    const totalChunks = Math.ceil(file.size / chunkSize);       // 文件分块数量 = 文件大小 / 分块大小
    const uploadPromises = [];                                  // 延迟执行的 Promise 任务队列

    for (let i = 0; i < totalChunks; i++) {
        const chunk = file.slice(i * chunkSize, (i + 1) * chunkSize);

        const formData = new FormData();
        formData.append("fileName", file.name);
        formData.append("chunkIndex", i);
        formData.append("totalChunks", totalChunks);
        formData.append("chunkData", chunk, `${file.name}.part${i}`);

        uploadPromises.push(() => retryableUpload(url, formData, 3));
    }

    // 限制最大并发
    await uploadPromisesLimit(uploadPromises, 3);
}

/**
 * 限制并发
 * @param {any} uploadPromises 由函数组成的数组
 * @param {any} limit
 */
async function uploadPromisesLimit(uploadPromises, limit) {
    let executing = [];   // 当前正在运行的Promise队列

    for (const promise of uploadPromises) {
        // 执行文件上传
        const p = promise()
            .finally(() => {
                executing = executing.filter(item => item !== p)
            })

        // 将活跃的Promise加入监控队列
        executing.push(p);

        // 阻塞直到有空闲槽位
        if (executing.length >= limit) {
            await Promise.race(executing);
        }
    }
}

// 重试上传
async function retryableUpload(url, formData, maxRetries) {
    for (let attempt = 0; attempt < maxRetries; attempt++) {
        try {
            const response = await fetch(url, {
                method: "POST",
                headers: { 'Accept': 'application/json' },
                body: formData
            });

            if (response.ok) {
                console.log(`HTTP Status: ${response.status} ${response.statusText}`);
            } else {
                // 尝试读取错误信息（如果是JSON）
                const errorBody = await response.json().catch(() => null);

                if (errorBody) {
                    console.error('Server error details:', errorBody);
                    throw new Error(`Server error: ${JSON.stringify(errorBody)}`);
                }

                throw new Error(`HTTP ${response.status} ${response.statusText}`);
            }

            return;
        } catch (err) {
            if (attempt === maxRetries - 1) throw err;
            await new Promise(resolve => setTimeout(resolve, 1000 * attempt));
        }
    }
}

//#endregion

//#endregion

//#region 文件下载
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

//#endregion

//#region SVG Elements

class SVGDragController {
    /**
     * 构造函数
     * @param {SVGElement} element 
     * @param {Object} dotNetObjRef 
     * @param {number} initialX 
     * @param {number} initialY 
     */
    constructor(element, dotNetObjRef, initialX, initialY) {
        if (!element || !(element instanceof SVGElement)) {
            throw new Error('Invalid SVG element provided');
        }

        this.element = element;
        this.dotNetObjRef = dotNetObjRef;
        this.svg = element.ownerSVGElement;

        // 拖拽状态
        this.state = {
            isSelected: false,
            isDragging: false,
            startOffset: { x: 0, y: 0 },
            position: { x: initialX, y: initialY }
        };

        // 绑定事件处理器，确保this上下文正确
        this.handleMouseDown = this.handleMouseDown.bind(this);
        this.handleMouseMove = this.handleMouseMove.bind(this);
        this.handleMouseUp = this.handleMouseUp.bind(this);
        this.handleDocumentClick = this.handleDocumentClick.bind(this);
    }

    /**
     * 初始化SVG元素拖拽功能
     * @param {SVGElement} element - 要添加拖拽功能的SVG元素
     * @param {Object} dotNetObjRef - Blazor组件引用
     * @param {number} initialX - 初始X坐标
     * @param {number} initialY - 初始Y坐标
     * @returns {Function} 清理函数，用于移除事件监听
     */
    initialize() {
        this.element.style.cursor = 'move';
        this.element.setAttribute('data-draggable', 'true');
        this.element.addEventListener('mousedown', this.handleMouseDown);
        this.svg.addEventListener('mousemove', this.handleMouseMove);
        this.svg.addEventListener('mouseup', this.handleMouseUp);
        document.addEventListener('click', this.handleDocumentClick);
    }

    /**
     * 清理事件监听
     */
    dispose() {
        this.element.removeEventListener('mousedown', this.handleMouseDown);
        this.svg.removeEventListener('mousemove', this.handleMouseMove);
        this.svg.removeEventListener('mouseup', this.handleMouseUp);
        document.removeEventListener('click', this.handleDocumentClick);

        this.element.style.cursor = '';
        this.element.removeAttribute('data-draggable');
    }

    /**
     * 鼠标按下事件处理
     * @param {MouseEvent} event 
     */
    async handleMouseDown(event) {
        // 只响应左键点击
        if (event.button !== 0) return;

        const svgPoint = this.getSVGPoint(event.clientX, event.clientY);

        this.state.isSelected = true;
        this.state.isDragging = true;
        this.state.startOffset = {
            x: svgPoint.x - this.state.position.x,
            y: svgPoint.y - this.state.position.y
        };

        await this.dotNetObjRef.invokeMethodAsync('SelectedElement');

        event.preventDefault();
        event.stopPropagation();
    }

    /**
     * 鼠标移动事件处理
     * @param {MouseEvent} event 
     */
    handleMouseMove(event) {
        if (!this.state.isDragging) return;

        const svgPoint = this.getSVGPoint(event.clientX, event.clientY);

        this.state.position = {
            x: svgPoint.x - this.state.startOffset.x,
            y: svgPoint.y - this.state.startOffset.y
        };

        this.notifyPositionUpdate();

        event.preventDefault();
        event.stopPropagation();
    }

    /**
     * 鼠标释放事件处理
     */
    async handleMouseUp() {
        this.state.isDragging = false;
    }

    async handleDocumentClick(event) {
        // 检查点击是否发生在当前控制器管理的元素或其子元素上
        const isClickOnSelfOrChild = this.element.contains(event.target) ||
            event.target === this.element;

        // 检查点击是否发生在SVG画布内（包括所有子元素）
        const isClickInSVG = this.svg.contains(event.target);

        // 如果点击在SVG画布内，在当前元素/子元素外，且当前元素/子元素是选中状态
        if ((isClickInSVG && !isClickOnSelfOrChild) && this.state.isSelected) {
            this.state.isSelected = false;
            await this.dotNetObjRef.invokeMethodAsync('UnSelectedElement');
        }
    }

    /**
     * 通知.NET位置更新
     */
    async notifyPositionUpdate() {
        try {
            await this.dotNetObjRef.invokeMethodAsync(
                'UpdatePosition',
                this.state.position.x,
                this.state.position.y
            );
        } catch (error) {
            console.error('Failed to notify position update:', error);
            // 可以考虑在这里添加重试逻辑或错误上报
        }
    }

    /**
     * 获取SVG坐标系中的点
     * @param {number} clientX 
     * @param {number} clientY 
     * @returns {SVGPoint}
     */
    getSVGPoint(clientX, clientY) {
        const point = this.svg.createSVGPoint();
        point.x = clientX;
        point.y = clientY;
        return point.matrixTransform(this.svg.getScreenCTM().inverse());
    }
}

const controllerInstances = new WeakMap();

export async function initializeDraggableSVGElement(inputElement, dotNetObjRef, x, y) {
    const controller = new SVGDragController(inputElement, dotNetObjRef, x, y);
    controllerInstances.set(inputElement, controller);
    controller.initialize();
}

export async function cleanUpDraggableSVGElement(inputElement) {
    if (!inputElement || !controllerInstances.has(inputElement)) {
        console.warn('No drag controller found for element:', inputElement);
        return;
    }

    const controller = controllerInstances.get(inputElement);
    controller.dispose();
    controllerInstances.delete(inputElement);
}

//#endregion
