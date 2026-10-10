// FractalBlazor Layout interop. Every function lives under the single global
// `FractalBlazor` object, so the page global scope stays clean.
window.FractalBlazor = window.FractalBlazor || {};

/**
 * Reports to .NET when `element` enters or leaves the viewport.
 * Returns a handle whose `dispose()` stops the observation.
 */
window.FractalBlazor.observeVisibility = function (element, dotnet, keepObserving) {
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

let mouseX = 0, mouseY = 0;
const elementsFbOver = new Set();
let mouseMoveListener;
let onScrollListener;
let onScrollEndListener;
let intervalId;
let didScroll = false;
let performanceMode = false;
let lastElementId;

function FbIsOver(id, automatic) {

    lastElementId = id;

    if (automatic == "AUTO" && !performanceMode) {
        document.getElementById(id).style.visibility = "inherit";
        document.getElementById(id).style.opacity = 1;
    }
    else if (automatic == "AUTO" && performanceMode && !didScroll) {
        document.getElementById(id).style.visibility = "inherit";
        document.getElementById(id).style.opacity = 1;
        elementsFbOver.add(id);
    }
}
function FbIsLeaving(id, automatic) {
    if (automatic == "AUTO") {
        var element = document.getElementById(id);
        if (!element) return;

        element.style.visibility = "collapse";
        element.style.opacity = 0;

        if (performanceMode)
            elementsFbOver.delete(id);
    }
}
function isMouseOverElement(id) {
    var element = document.getElementById(id);
    if (!element) return false;

    var rect = element.getBoundingClientRect();

    return mouseX >= rect.left &&
        mouseX <= rect.right &&
        mouseY >= rect.top &&
        mouseY <= rect.bottom;
}
function ClearElementsInExcessOver(verificationDelay) {
    intervalId = setInterval(() => {
        didScroll = false;
        elementsFbOver.forEach(id => {
            if (!isMouseOverElement(id)) {
                FbIsLeaving(id, "AUTO");
            }
        });
    }, verificationDelay);
}

function runFbShowOnOverPerformance(parentId, verificationDelay) {

    performanceMode = true;
    elementsFbOver.clear();
    mouseMoveListener = function (event) {
        mouseX = event.clientX;
        mouseY = event.clientY;
    };
    onScrollListener = function () {
        didScroll = true;
    };
    onScrollEndListener = function () {
        var lastElement = document.getElementById(lastElementId);
        if (lastElement) {
            lastElement.style.visibility = "inherit";
            lastElement.style.opacity = 1;
            elementsFbOver.add(lastElementId);
        }

        didScroll = false;
    };

    document.addEventListener('mousemove', mouseMoveListener);

    var parentCpnt = document.getElementById(parentId);

    if (parentCpnt) {
        parentCpnt.addEventListener('scroll', onScrollListener);
        parentCpnt.addEventListener('scrollend', onScrollEndListener);
    }
    else {
        window.addEventListener('scroll', onScrollListener);
        window.addEventListener('scrollend', onScrollEndListener);
    }

    ClearElementsInExcessOver(verificationDelay);
}

function stopFbShowOnOverPerformance() {
    clearInterval(intervalId);
    elementsFbOver.clear();
    performanceMode = false;
    document.removeEventListener('mousemove', mouseMoveListener);
    document.removeEventListener('scroll', mouseMoveListener);
    document.removeEventListener('scrollend', mouseMoveListener);
}

//------ Dropdown Positioning & Tracking ------//

let activeDropdowns = new Map();

function fbAlignDropdown(containerId, dropdownId) {
    const container = document.getElementById(containerId);
    const dropdown = document.getElementById(dropdownId);
    if (!container || !dropdown) return;

    dropdown.classList.remove('fb-dropdown-open-up');
    dropdown.classList.add('fb-dropdown-open-down');

    const rect = container.getBoundingClientRect();
    const dropdownHeight = dropdown.offsetHeight;
    const viewportHeight = window.innerHeight;

    if (rect.bottom + dropdownHeight > viewportHeight) {
        if (rect.top > dropdownHeight || rect.top > viewportHeight - rect.bottom) {
            dropdown.classList.remove('fb-dropdown-open-down');
            dropdown.classList.add('fb-dropdown-open-up');
        }
    }
}

function fbInitDropdown(containerId, dropdownId) {
    let ticking = false;
    const align = () => {
        if (!ticking) {
            window.requestAnimationFrame(() => {
                fbAlignDropdown(containerId, dropdownId);
                ticking = false;
            });
            ticking = true;
        }
    };

    fbAlignDropdown(containerId, dropdownId);

    window.addEventListener('scroll', align, true);
    window.addEventListener('resize', align);

    activeDropdowns.set(dropdownId, { align, rawAlign: () => fbAlignDropdown(containerId, dropdownId) });
}

function fbDestroyDropdown(dropdownId) {
    const data = activeDropdowns.get(dropdownId);
    if (data) {
        window.removeEventListener('scroll', data.align, true);
        window.removeEventListener('resize', data.align);
        activeDropdowns.delete(dropdownId);
    }
}
