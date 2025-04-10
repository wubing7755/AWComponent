// 使用 ES6 模块导出方式
export const ScriptsInOne = (() => {
    // 保存原始初始化函数引用
    let _sidebarDrag;
    let _themeToggle;

    return {
        initialize: function () {
            console.log('Initializing scripts...');

            // 挂载侧边栏拖拽
            if (typeof initSidebarDrag === 'function') {
                console.log('Initializing sidebar drag');
                _sidebarDrag = window.initSidebarDrag;
                _sidebarDrag();
            }

            // 挂载主题切换
            if (typeof initThemeToggle === 'function') {
                console.log('Initializing theme toggle');
                _themeToggle = window.initThemeToggle;
                _themeToggle();
            }
        },

        cleanup: function () {
            console.log('Cleaning up scripts...');

            // 执行侧边栏清理
            if (typeof _sidebarDragCleanup === 'function') {

            }

            // 执行主题切换清理
            if (typeof _themeToggleCleanup === 'function') {

            }
        }
    };
})();

// 仍然保持全局访问（可选）
window.ScriptsInOne = ScriptsInOne;
