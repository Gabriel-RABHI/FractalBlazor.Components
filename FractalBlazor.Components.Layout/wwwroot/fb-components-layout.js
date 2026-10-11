// FractalBlazor Layout interop. Every function lives under the single global
// `FractalBlazor` object, so the page global scope stays clean. The script is loaded
// once by a <script> tag: components call it without importing a module each.
window.FractalBlazor = window.FractalBlazor || {};

(function (fb) {

    //------ Visibility (FbShowWhenVisible) ------//

    /**
     * Reports to .NET when `element` enters or leaves the viewport.
     * Returns a handle whose `dispose()` stops the observation.
     */
    fb.observeVisibility = function (element, dotnet, keepObserving) {
        const observer = new IntersectionObserver(entries => {
            for (const entry of entries) {
                dotnet.invokeMethodAsync('OnVisibilityChanged', entry.isIntersecting);
                if (entry.isIntersecting && !keepObserving)
                    observer.disconnect();
            }
        });
        observer.observe(element);
        return { dispose: () => observer.disconnect() };
    };

    //------ Show when over (FbShowWhenOver, FbShowWhenOverPerformance) ------//

    const over = {
        mouseX: 0,
        mouseY: 0,
        elements: new Set(),
        performanceMode: false,
        didScroll: false,
        lastElementId: null,
        intervalId: null,
        scrollTarget: null,
        onMouseMove: null,
        onScroll: null,
        onScrollEnd: null
    };

    function show(element) {
        element.style.visibility = "inherit";
        element.style.opacity = 1;
    }

    function hide(element) {
        element.style.visibility = "collapse";
        element.style.opacity = 0;
    }

    fb.isOver = function (id, automatic) {
        over.lastElementId = id;
        if (automatic !== "AUTO" || (over.performanceMode && over.didScroll))
            return;

        const element = document.getElementById(id);
        if (!element) return;

        show(element);
        if (over.performanceMode)
            over.elements.add(id);
    };

    fb.isLeaving = function (id, automatic) {
        if (automatic !== "AUTO") return;

        const element = document.getElementById(id);
        if (!element) return;

        hide(element);
        if (over.performanceMode)
            over.elements.delete(id);
    };

    function isMouseOver(id) {
        const element = document.getElementById(id);
        if (!element) return false;

        const rect = element.getBoundingClientRect();
        return over.mouseX >= rect.left && over.mouseX <= rect.right
            && over.mouseY >= rect.top && over.mouseY <= rect.bottom;
    }

    /**
     * Performance mode: while scrolling, hovered elements are not shown; a periodic check
     * hides those the mouse has left without a mouseout event.
     */
    fb.startShowOnOver = function (parentId, verificationDelay) {
        fb.stopShowOnOver();

        over.performanceMode = true;
        over.onMouseMove = event => {
            over.mouseX = event.clientX;
            over.mouseY = event.clientY;
        };
        over.onScroll = () => over.didScroll = true;
        over.onScrollEnd = () => {
            const last = document.getElementById(over.lastElementId);
            if (last) {
                show(last);
                over.elements.add(over.lastElementId);
            }
            over.didScroll = false;
        };

        document.addEventListener('mousemove', over.onMouseMove);
        over.scrollTarget = document.getElementById(parentId) || window;
        over.scrollTarget.addEventListener('scroll', over.onScroll);
        over.scrollTarget.addEventListener('scrollend', over.onScrollEnd);

        over.intervalId = setInterval(() => {
            over.didScroll = false;
            over.elements.forEach(id => {
                if (!isMouseOver(id))
                    fb.isLeaving(id, "AUTO");
            });
        }, verificationDelay);
    };

    fb.stopShowOnOver = function () {
        clearInterval(over.intervalId);
        over.intervalId = null;
        over.elements.clear();
        over.performanceMode = false;

        if (over.onMouseMove)
            document.removeEventListener('mousemove', over.onMouseMove);
        if (over.scrollTarget) {
            over.scrollTarget.removeEventListener('scroll', over.onScroll);
            over.scrollTarget.removeEventListener('scrollend', over.onScrollEnd);
        }
        over.scrollTarget = over.onMouseMove = over.onScroll = over.onScrollEnd = null;
    };

    //------ Keyboard navigation ------//

    // Elements marked data-fb-nav-keys handle navigation keys in .NET (FbSelect): the
    // browser must not also scroll the page with them.
    const navigationKeys = ['ArrowUp', 'ArrowDown', 'Home', 'End', 'PageUp', 'PageDown', ' '];
    document.addEventListener('keydown', event => {
        if (navigationKeys.includes(event.key) && event.target.closest && event.target.closest('[data-fb-nav-keys]'))
            event.preventDefault();
    });

    //------ Dropdown positioning (FbDropDown) ------//

    const dropdowns = new Map();

    // The zone is fixed to the window, placed from the rectangle of its anchor: a scrolling
    // ancestor (overflow: auto) cannot clip it.
    function alignDropdown(containerId, dropdownId) {
        const container = document.getElementById(containerId);
        const dropdown = document.getElementById(dropdownId);
        if (!container || !dropdown) return;

        const rect = container.getBoundingClientRect();
        const viewportWidth = document.documentElement.clientWidth;
        const viewportHeight = document.documentElement.clientHeight;
        const dropdownHeight = dropdown.offsetHeight;
        const up = rect.bottom + dropdownHeight > viewportHeight
            && (rect.top > dropdownHeight || rect.top > viewportHeight - rect.bottom);

        dropdown.classList.add('fb-dropdown-fixed');
        dropdown.classList.toggle('fb-dropdown-open-up', up);
        dropdown.classList.toggle('fb-dropdown-open-down', !up);
        dropdown.style.minWidth = rect.width + 'px';
        dropdown.style.top = up ? '' : rect.bottom + 'px';
        dropdown.style.bottom = up ? (viewportHeight - rect.top) + 'px' : '';
        dropdown.style.left = Math.max(0, Math.min(rect.left, viewportWidth - dropdown.offsetWidth)) + 'px';
    }

    /**
     * Opens the dropdown downward, or upward when it does not fit below, and keeps it
     * aligned with its anchor while the page or any scrolling area scrolls, or the window
     * resizes.
     */
    fb.initDropdown = function (containerId, dropdownId) {
        let ticking = false;
        const align = () => {
            if (ticking) return;
            ticking = true;
            window.requestAnimationFrame(() => {
                alignDropdown(containerId, dropdownId);
                ticking = false;
            });
        };

        alignDropdown(containerId, dropdownId);
        window.addEventListener('scroll', align, true);
        window.addEventListener('resize', align);
        dropdowns.set(dropdownId, align);
    };

    fb.destroyDropdown = function (dropdownId) {
        const align = dropdowns.get(dropdownId);
        if (!align) return;

        window.removeEventListener('scroll', align, true);
        window.removeEventListener('resize', align);
        dropdowns.delete(dropdownId);
    };

})(window.FractalBlazor);
