function initSidebarDrag() {
    /*
     * 侧边栏拖拽
     */

    const sidebar = document.getElementById('draggable-sidebar');
    const resizeHandle = sidebar.querySelector('.resize-handle');
    const main = document.querySelector('main');
    const MIN_WIDTH = 210;
    const MAX_WIDTH = 400;

    if (!sidebar || !resizeHandle || !main) {
        console.error('Drag elements not found');
        return;
    }

    let isDragging = false;
    let startX = 0;
    let startWidth = 0;
    let translateX = 0;
    let newWidth = 0;

    const getCurrentWidth = () =>
        parseInt(window.getComputedStyle(sidebar).width, 10);

    const handleDown = function (e) {
        isDragging = true;
        startX = e.clientX;
        startWidth = getCurrentWidth();

        document.body.style.cursor = 'col-resize';
        document.body.style.userSelect = 'none';
        e.preventDefault();
    };

    const handleMove = function (e) {
        if (!isDragging) return;

        // 侧边栏宽度： 210 ~ 400 px
        newWidth = Math.max(MIN_WIDTH, Math.min(MAX_WIDTH, startWidth + e.clientX - startX));
        translateX = newWidth - startWidth;

        resizeHandle.style.transform = `translateX(${translateX}px)`;
    };

    const handleUp = function (e) {
        if (!isDragging) return;
        isDragging = false;

        newWidth = startWidth + translateX;

        requestAnimationFrame(() => {
            sidebar.style.width = `${newWidth}px`;
            main.style.marginLeft = `${newWidth}px`;
        });

        resizeHandle.style.transform = '';
        document.body.style.cursor = '';
        document.body.style.userSelect = '';
    };

    // 事件绑定
    resizeHandle.addEventListener('mousedown', handleDown);
    document.addEventListener('mousemove', handleMove, { passive: true });
    document.addEventListener('mouseup', handleUp);
    document.addEventListener('mouseleave', handleUp);

    // 清理函数
    return () => {
        resizeHandle.removeEventListener('mousedown', handleDown);
        document.removeEventListener('mousemove', handleMove);
        document.removeEventListener('mouseup', handleUp);
        document.removeEventListener('mouseleave', handleUp);
    }
}

window.initSidebarDrag = initSidebarDrag;