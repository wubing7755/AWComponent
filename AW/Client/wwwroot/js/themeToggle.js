function initThemeToggle() {
    /*
     * 主题切换：Dark/Light
     * 方法一：在localStorage中存储用户选择
     * 方法二：跟随浏览器主题
     */

    try {
        const themeToggle = document.querySelector('.theme-toggle');
        const root = document.documentElement;

        // 从localStorage读取保存的主题
        const savedTheme = localStorage.getItem('theme') || 'system';
        if (savedTheme !== 'system') {
            root.dataset.theme = savedTheme;
        }

        themeToggle.addEventListener('click', () => {
            const currentTheme = root.dataset.theme || 'system';
            const newTheme = currentTheme === 'dark' ? 'light' : 'dark';

            root.dataset.theme = newTheme;
            localStorage.setItem('theme', newTheme);

            // 同步系统主题变化
            if (newTheme === 'system') {
                root.removeAttribute('data-theme');
            }
        });

        // 监听系统主题变化
        const colorSchemeMedia = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';

        colorSchemeMedia.addEventListener('change', (e) => {
            if (!localStorage.getItem('theme')) {
                root.dataset.theme = e.matches ? 'dark' : 'light';
            }
        });
    } catch (error) {
        console.error('Failed to initialize theme toggle:', error);
    }
}

window.initThemeToggle = initThemeToggle;


        
//        /**
// * 初始化主题切换功能
// * 支持手动切换主题(dark/light)和跟随系统主题
// */
//function initThemeToggle() {
//    try {
//        const themeToggle = document.querySelector('.theme-toggle');
//        const root = document.documentElement;

//        if (!themeToggle) {
//            console.warn('Theme toggle button not found');
//            return;
//        }

//        // 有效的主题值
//        const VALID_THEMES = ['light', 'dark', 'system'];

//        /**
//         * 设置主题
//         * @param {string} theme - 要设置的主题 ('light', 'dark' 或 'system')
//         */
//        const setTheme = (theme) => {
//            if (!VALID_THEMES.includes(theme)) {
//                console.error(`Invalid theme: ${theme}`);
//                return;
//            }

//            if (theme === 'system') {
//                root.removeAttribute('data-theme');
//                localStorage.removeItem('theme');
//                applySystemTheme();
//            } else {
//                root.dataset.theme = theme;
//                localStorage.setItem('theme', theme);
//            }
//        };

//        /**
//         * 应用系统主题
//         */
//        const applySystemTheme = () => {
//            const isDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
//            root.dataset.theme = isDark ? 'dark' : 'light';
//        };

//        /**
//         * 获取当前主题
//         * @returns {string} 当前主题
//         */
//        const getCurrentTheme = () => {
//            return root.dataset.theme || 'system';
//        };

//        /**
//         * 切换主题
//         */
//        const toggleTheme = () => {
//            const currentTheme = getCurrentTheme();
//            const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
//            setTheme(newTheme);
//        };

//        // 初始化主题
//        const savedTheme = localStorage.getItem('theme');
//        if (savedTheme && VALID_THEMES.includes(savedTheme)) {
//            setTheme(savedTheme);
//        } else {
//            setTheme('system');
//        }

//        // 添加点击事件监听
//        themeToggle.addEventListener('click', toggleTheme);

//        // 监听系统主题变化
//        const colorSchemeMedia = window.matchMedia('(prefers-color-scheme: dark)');
//        colorSchemeMedia.addEventListener('change', (e) => {
//            if (!localStorage.getItem('theme')) {
//                applySystemTheme();
//            }
//        });

//    } catch (error) {
//        console.error('Failed to initialize theme toggle:', error);
//    }
//}

//// 暴露初始化函数
//window.initThemeToggle = initThemeToggle;
