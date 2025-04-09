function initSidebarDrag() {
    const sidebar = document.getElementById('draggable-sidebar');
    const resizeHandle = sidebar.querySelector('.resize-handle');
    let isDragging = false;
    let startX = 0;
    let startWidth = 0;

    resizeHandle.addEventListener('mousedown', function(e) {
        isDragging = true;
        startX = e.clientX;
        startWidth = parseInt(document.querySelector('.sidebar').style.width || 250);
        document.body.style.cursor = 'col-resize';
        document.body.style.userSelect = 'none';
        e.preventDefault();
    });

    document.addEventListener('mousemove', function(e) {
        if (!isDragging) return;
        
        const newWidth = Math.max(200, Math.min(400, startWidth + e.clientX - startX));

        requestAnimationFrame(() => {
            document.querySelector('.sidebar').style.width = `${newWidth}px`;
            document.querySelector('main').style.marginLeft = `${newWidth}px`;
        });
    });

    document.addEventListener('mouseup', function() {
        isDragging = false;
        document.body.style.cursor = '';
        document.body.style.userSelect = '';
    });
}

window.initSidebarDrag = initSidebarDrag;