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

    //------ Width (FbWidthSensor) ------//

    /**
     * Reports to .NET the width category of the box that contains `element`, each time it
     * changes, and only then. `thresholds` are the lower bounds, in pixels, of the categories
     * above the first one. Returns a handle whose `dispose()` stops the observation.
     */
    fb.observeWidth = function (element, dotnet, thresholds) {
        // A parent displayed as contents has no box: measure the first one above it.
        let box = element.parentElement;
        while (box && getComputedStyle(box).display === 'contents')
            box = box.parentElement;
        if (!box)
            return { dispose: () => { } };

        let category = -1;
        const observer = new ResizeObserver(entries => {
            const entry = entries[0];
            const width = entry.borderBoxSize?.[0]?.inlineSize ?? entry.contentRect.width;
            let next = 0;
            while (next < thresholds.length && width >= thresholds[next])
                next++;
            if (next !== category) {
                category = next;
                dotnet.invokeMethodAsync('OnWidthClassChanged', category);
            }
        });
        observer.observe(box);
        return { dispose: () => observer.disconnect() };
    };

    //------ Modal drawer (FbDrawer) ------//

    // Listeners of each open drawer, removed before the state closes it.
    const drawers = new WeakMap();

    function isOutside(dialog, event) {
        const rect = dialog.getBoundingClientRect();
        return event.clientX < rect.left || event.clientX > rect.right
            || event.clientY < rect.top || event.clientY > rect.bottom;
    }

    /**
     * Shows `dialog` as a modal: the page becomes inert and the focus stays inside. Escape,
     * a click on the backdrop and any close request of the browser call `OnDismiss` on
     * `dotnet`, and the dialog stays open: the state decides, then calls `closeDrawer`.
     */
    fb.showDrawer = function (dialog, dotnet) {
        if (!dialog || drawers.has(dialog)) return;

        // A click on the backdrop targets the dialog, outside its box. The press must start
        // there too: selecting text inside and releasing outside is not a dismissal.
        let pressedOutside = false;
        const dismiss = () => dotnet.invokeMethodAsync('OnDismiss');
        const listeners = {
            cancel: event => {
                event.preventDefault();
                dismiss();
            },
            pointerdown: event => {
                pressedOutside = event.target === dialog && isOutside(dialog, event);
            },
            click: event => {
                if (pressedOutside && event.target === dialog && isOutside(dialog, event))
                    dismiss();
                pressedOutside = false;
            },
            // Closed by the browser despite the cancelled request: the state must follow.
            close: dismiss
        };
        for (const [type, listener] of Object.entries(listeners))
            dialog.addEventListener(type, listener);
        drawers.set(dialog, listeners);

        if (!dialog.open)
            dialog.showModal();
    };

    /**
     * Closes `dialog`, which gives the focus back to where it was, and resolves once its
     * exit transition is over, so that its content can go.
     */
    fb.closeDrawer = function (dialog) {
        if (!dialog) return Promise.resolve();

        const listeners = drawers.get(dialog);
        if (listeners) {
            for (const [type, listener] of Object.entries(listeners))
                dialog.removeEventListener(type, listener);
            drawers.delete(dialog);
        }
        if (!dialog.open) return Promise.resolve();

        dialog.close();
        const transitions = dialog.getAnimations({ subtree: true })
            .filter(animation => animation.effect && animation.effect.target === dialog)
            .map(animation => animation.finished.catch(() => { }));
        const timeout = new Promise(resolve => setTimeout(resolve, 500));
        return Promise.race([Promise.all(transitions), timeout]);
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

    // Elements marked data-fb-nav-keys handle navigation keys in .NET (FbSelect, FbTabs, the
    // thumbs of a slider): the browser must not also scroll the page with them.
    const navigationKeys = ['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight', 'Home', 'End', 'PageUp', 'PageDown', ' '];
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

    //------ Slider (FbSlider, FbRangeSlider) ------//

    /**
     * Follows the drags of a slider. A press on the track takes the nearest thumb, a press on
     * a thumb takes it; while the pointer moves, the thumb is placed at once on the rail
     * (--fb-slider-drag-from or -to), rounded to the step and kept on its side of the other
     * thumb, and .NET receives the value with `OnSlide`. Once the release is handled, the
     * rail lets the position the state rendered show again. The scale hides the labels that
     * would overlap, again whenever the slider is resized. Returns a handle whose `dispose()`
     * stops following.
     */
    fb.initSlider = function (element, dotnet) {
        if (!element) return { dispose: () => { } };

        let drag = null;
        const number = name => parseFloat(element.dataset[name]);
        const thumbs = () => element.querySelectorAll('.fb-slider-thumb');
        const rail = () => element.querySelector('.fb-slider-rail');

        function decimals(step) {
            const text = String(step);
            const point = text.indexOf('.');
            return point < 0 ? 0 : Math.min(text.length - point - 1, 10);
        }

        function valueAt(event) {
            const box = rail().getBoundingClientRect();
            const vertical = element.classList.contains('fb-slider-vertical');
            const length = vertical ? box.height : box.width;
            let ratio = length > 0
                ? (vertical ? box.bottom - event.clientY : event.clientX - box.left) / length
                : 0;
            ratio = Math.min(1, Math.max(0, ratio));

            const min = number('min'), max = number('max'), step = number('step');
            let value = min + ratio * (max - min);
            if (step > 0)
                value = min + Math.round((value - min) / step) * step;
            value = +value.toFixed(decimals(step));
            return Math.min(Math.max(value, Math.min(min, max)), Math.max(min, max));
        }

        // A thumb of a range stays on its side of the other one.
        function bounded(index, value) {
            const all = thumbs();
            if (all.length < 2) return value;
            const other = parseFloat(all[1 - index].getAttribute('aria-valuenow'));
            return index === 0 ? Math.min(value, other) : Math.max(value, other);
        }

        function place(index, value) {
            const min = number('min'), max = number('max');
            const percent = max > min ? (value - min) / (max - min) * 100 : 0;
            const thumb = thumbs()[index];
            rail().style.setProperty('--fb-slider-drag-' + thumb.dataset.position, percent + '%');
        }

        // The thumb a press takes: the one pressed, or the nearest. Two thumbs on one value
        // wait for the first move: towards the start, the first one goes; otherwise the second.
        function chosen(event, value) {
            const all = thumbs();
            if (all.length < 2) return 0;
            const start = parseFloat(all[0].getAttribute('aria-valuenow'));
            const end = parseFloat(all[1].getAttribute('aria-valuenow'));
            if (start === end) return value === start ? null : value < start ? 0 : 1;
            const own = event.target.closest('.fb-slider-thumb');
            if (own) return [...all].indexOf(own);
            return Math.abs(value - start) <= Math.abs(value - end) ? 0 : 1;
        }

        function take(index) {
            drag.index = index;
            const thumb = thumbs()[index];
            thumb.classList.add('fb-slider-active');
            thumb.focus({ preventScroll: true });
        }

        function move(event) {
            if (!drag || event.pointerId !== drag.pointerId) return;
            const at = valueAt(event);
            if (drag.index === null) {
                if (at === drag.overlap) return;
                take(at < drag.overlap ? 0 : 1);
            }
            const value = bounded(drag.index, at);
            if (value === drag.value) return;
            drag.value = value;
            place(drag.index, value);
            dotnet.invokeMethodAsync('OnSlide', drag.index, value, false);
        }

        function down(event) {
            if (event.button !== 0 || drag || element.classList.contains('fb-slider-disabled')) return;
            const at = valueAt(event);
            const index = chosen(event, at);
            drag = { index: null, pointerId: event.pointerId, value: null, overlap: at };
            try {
                element.setPointerCapture(event.pointerId);
            } catch {
                // A pointer the browser no longer tracks: the drag follows the element only.
            }
            element.classList.add('fb-slider-sliding');
            if (index !== null) take(index);
            else thumbs()[1].focus({ preventScroll: true });
            event.preventDefault();
            move(event);
        }

        function up(event) {
            if (!drag || event.pointerId !== drag.pointerId) return;
            const { index, value } = drag;
            drag = null;
            element.classList.remove('fb-slider-sliding');
            if (index === null) return;
            thumbs()[index]?.classList.remove('fb-slider-active');
            const release = () => {
                const position = thumbs()[index]?.dataset.position;
                if (position && !drag) rail().style.removeProperty('--fb-slider-drag-' + position);
            };
            if (value === null) return release();
            dotnet.invokeMethodAsync('OnSlide', index, value, true).then(release, release);
        }

        // Whether two labels of the scale, the first one lower, leave a gap between them.
        function apart(lower, higher, vertical) {
            const gap = 6;
            return vertical ? lower.top >= higher.bottom + gap : higher.left >= lower.right + gap;
        }

        // The scale keeps one label in two, three... when they would overlap: the first label
        // always, the last one in place of the one before it. Labels keep their layout while
        // hidden, so they can be measured again when the slider grows.
        function fit() {
            const labels = [...element.querySelectorAll('.fb-slider-tick > .fb-slider-label')];
            if (labels.length < 2) return;
            const vertical = element.classList.contains('fb-slider-vertical');
            const boxes = labels.map(label => label.getBoundingClientRect());
            const last = labels.length - 1;
            const kept = stride => {
                const indexes = [];
                for (let i = 0; i <= last; i += stride) indexes.push(i);
                if (indexes[indexes.length - 1] !== last) {
                    if (indexes.length > 1) indexes.pop();
                    indexes.push(last);
                }
                return indexes;
            };
            let shown = [0];
            for (let stride = 1; stride <= last; stride++) {
                const indexes = kept(stride);
                if (indexes.every((index, n) => n === 0 || apart(boxes[indexes[n - 1]], boxes[index], vertical))) {
                    shown = indexes;
                    break;
                }
            }
            labels.forEach((label, i) => label.style.visibility = shown.includes(i) ? '' : 'hidden');
        }

        let fitting = 0;
        const refit = () => {
            cancelAnimationFrame(fitting);
            fitting = requestAnimationFrame(fit);
        };
        const resized = new ResizeObserver(refit);
        resized.observe(element);
        const changed = new MutationObserver(mutations => {
            const parent = node => node.nodeType === Node.ELEMENT_NODE ? node : node.parentElement;
            if (mutations.some(m => parent(m.target)?.closest('.fb-slider-tick') || m.target.classList?.contains('fb-slider-rail')))
                refit();
        });
        changed.observe(element, { childList: true, subtree: true, characterData: true });
        document.fonts?.ready.then(refit);

        element.addEventListener('pointerdown', down);
        element.addEventListener('pointermove', move);
        element.addEventListener('pointerup', up);
        element.addEventListener('pointercancel', up);
        return {
            dispose: () => {
                cancelAnimationFrame(fitting);
                resized.disconnect();
                changed.disconnect();
                element.removeEventListener('pointerdown', down);
                element.removeEventListener('pointermove', move);
                element.removeEventListener('pointerup', up);
                element.removeEventListener('pointercancel', up);
            }
        };
    };

    //------ Drag and drop (FbDraggable, FbDraggableHandle, FbDraggableReceiver) ------//

    // Delegated listeners: no component calls the script, and no state lives here but the
    // draggable armed by the current press.
    const dragData = 'application/x-fractalblazor';
    let armed = null;

    function elementOf(node) {
        return node && node.nodeType === Node.ELEMENT_NODE ? node : node && node.parentElement;
    }

    function disarm() {
        if (!armed) return;
        armed.setAttribute('draggable', 'false');
        armed.classList.remove('fb-dragging');
        armed = null;
    }

    // A draggable is draggable only while a press on one of its handles lasts: the rest of
    // its content keeps its own behavior. A handle moves the nearest draggable around it.
    document.addEventListener('pointerdown', event => {
        disarm();
        const handle = elementOf(event.target)?.closest('.fb-drag-handle');
        const draggable = handle?.closest('.fb-draggable');
        if (!draggable) return;

        draggable.setAttribute('draggable', 'true');
        armed = draggable;
    }, true);

    // A press that does not become a drag ends here; a drag ends with dragend.
    document.addEventListener('pointerup', disarm, true);
    document.addEventListener('dragend', disarm, true);

    // Only the armed draggable starts a drag of the library, from itself or from the content
    // of one of its handles (an image there drags the item). An image, a link or a text
    // selection dragged from the rest of a draggable stays a drag of the browser, and never
    // reaches the components. The dragged item fades once the browser has its image.
    window.addEventListener('dragstart', event => {
        const element = elementOf(event.target);
        if (!element?.closest('.fb-draggable')) return;
        const owner = element === armed ? armed : element.closest('.fb-drag-handle')?.closest('.fb-draggable');
        if (!armed || owner !== armed) {
            event.stopPropagation();
            return;
        }

        event.dataTransfer.effectAllowed = 'move';
        event.dataTransfer.setData(dragData, '');
        const dragged = armed;

        // A drag fires no mouseout: what the mouse showed on the item (FbShowWhenOver) would
        // stay shown once it lands elsewhere.
        dragged.querySelectorAll('.fb-swo-auto').forEach(hide);
        requestAnimationFrame(() => {
            if (dragged === armed) dragged.classList.add('fb-dragging');
        });
    }, true);

    // A zone lets the dragged item in, unless its preview refused it there (fb-drop-refused):
    // the browser then shows a refusal and fires no drop.
    function enterZone(event) {
        const zone = elementOf(event.target)?.closest('.fb-drop-zone');
        if (!zone || zone.classList.contains('fb-drop-refused')) return;

        event.preventDefault();
        event.dataTransfer.dropEffect = 'move';
    }

    document.addEventListener('dragenter', enterZone);
    document.addEventListener('dragover', enterZone);

    // The drop belongs to the components: the browser must not open what was dropped.
    document.addEventListener('drop', event => {
        if (elementOf(event.target)?.closest('.fb-drop-zone'))
            event.preventDefault();
    });

})(window.FractalBlazor);
