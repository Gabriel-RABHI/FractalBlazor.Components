// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

function isInViewport(el) {
    var top = el.offsetTop;
    var left = el.offsetLeft;
    var width = el.offsetWidth;
    var height = el.offsetHeight;

    while (el.offsetParent) {
        el = el.offsetParent;
        top += el.offsetTop;
        left += el.offsetLeft;
    }

    return (
        top < (window.pageYOffset + window.innerHeight) &&
        left < (window.pageXOffset + window.innerWidth) &&
        (top + height) > window.pageYOffset &&
        (left + width) > window.pageXOffset
    );
}

function getAllElementsWithAttribute(attribute) {
    var matchingElements = [];
    var allElements = document.getElementsByTagName('*');
    for (var i = 0, n = allElements.length; i < n; i++)
        if (allElements[i].getAttribute(attribute) !== null)
            matchingElements.push(allElements[i]);
    return matchingElements;
}

window.setInterval(function () {
    // -------- Visibility
    var allVis = getAllElementsWithAttribute('data-fb-visibility-id');
    for (var i = 0, n = allVis.length; i < n; i++) {
        var el = allVis[i];
        if (isInViewport(el)) {
            var v = el.getAttribute('data-fb-visibility-id');
            UpdateVisibilityMessageCallerJS(v);
            var vreset = el.getAttribute('data-no-reset');
            if (vreset !== "true")
                el.removeAttribute('data-fb-visibility-id');
        }
    }
    OnWindowsResize();
}, 250);


function OnWindowsResize() {
    var allMaster = getAllElementsWithAttribute('data-fb-width-master-id');
    var allSlaves = getAllElementsWithAttribute('data-fb-width-slave-id');
    for (var i = 0, n = allMaster.length; i < n; i++) {
        var m = allMaster[i];
        var m_id = m.getAttribute('data-fb-width-master-id');
        for (var j = 0, sn = allSlaves.length; j < sn; j++) {
            var s = allSlaves[j];
            var s_id = s.getAttribute('data-fb-width-slave-id');
            if (m_id == s_id) {
                var w = m.clientWidth;
                s.style.width = w + "px";
            }
        }
    }
}

window.onresize = OnWindowsResize;


function UpdateVisibilityMessageCallerJS(cpntId) {
    DotNet.invokeMethodAsync('FractalBlazor.Components.Layout', 'VisibilityChangedMessageCaller', cpntId);
}


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

//------ AdaptativeDivHeight ------//

var fbAdaptativeDivsIntervals = {};

function fbAdaptDivHeight(targetId, additionalSpace = 0, paddingBottomRem = 0.0, inModal = false) {

    var element = document.getElementById(targetId);

    if (!element) return;

    var fontSizePx = parseFloat(getComputedStyle(document.documentElement).fontSize);
    var spaceAbove = element.getBoundingClientRect().top;

    if (inModal) {
        if (document.getElementsByClassName("mud-dialog-actions").length) {
            additionalSpace += document.getElementsByClassName("mud-dialog-actions")[0].offsetHeight;
        }
        paddingBottomRem = paddingBottomRem + 2.5;
    }

    var totalHeight = (spaceAbove + additionalSpace) / fontSizePx;

    var spaceAboveRem = (totalHeight + paddingBottomRem).toFixed(2);

    element.style.height = `calc(-${spaceAboveRem}rem + 100vh)`;
}

function runFbAdaptDivHeightInterval(targetId, additionalSpace = 0, paddingBottomRem = 0.0, inModal = false, intervalDelay = 0) {

    fbAdaptDivHeight(targetId, additionalSpace, paddingBottomRem, inModal);

    if (!fbAdaptativeDivsIntervals[targetId]) {
        fbAdaptativeDivsIntervals[targetId] = setInterval(() => {
            fbAdaptDivHeight(targetId, additionalSpace, paddingBottomRem, inModal);
        }, intervalDelay);
    }
}

function stopFbAdaptDivHeightInterval(targetId) {
    if (fbAdaptativeDivsIntervals[targetId]) {
        clearInterval(fbAdaptativeDivsIntervals[targetId]);
        delete fbAdaptativeDivsIntervals[targetId];
    }
}

function killFbAdaptDivHeightIntervals() {
    for (var targetId in fbAdaptativeDivsIntervals) {
        if (fbAdaptativeDivsIntervals.hasOwnProperty(targetId)) {
            clearInterval(fbAdaptativeDivsIntervals[targetId]);
        }
    }
    fbAdaptativeDivsIntervals = {};
}