let bandLines = 3;          // default number of logical lines in highlight band
let bandHeightPx = 0;       // virtual height of the highlight band
let bandCenterOffsetPx = 0; // fine-tune offset from center of band (slider-driven)
let baseLineHeight = 40;    // fallback; updated from CSS
let scrollSpeed = 1;        // pixels per frame
let scrollInterval = null;
let scrolling = false;
let line = 0;
let bandGeometry = null;
let geometry = null;
let manualScrollPos = 0;
function loadNotes(text) {
    let dataParent = "";
    manualScrollPos = 0;
    const el = document.getElementById("content");
    if (!el) return;

    // Normalize line endings into markers
    text = text
        .replace(/\r\n/g, "\n<P>")  // CRLF → insert paragraph marker
        .replace(/\v/g, "\n<L>")    // VT → line marker
        .replace(/\r/g, "\n<P>");   // stray CR → paragraph marker

    const rawLines = text.split("\n");

    commands = [];
    cleanedLines = [];

    let output = [];          // final HTML elements
    let logicalIndex = 0;     // index into cleanedLines / logical lines

    for (let i = 0; i < rawLines.length; i++) {
        const original = rawLines[i];

        let code = "";
        let content = original;

        // Detect and strip leading markers <P> or <L>
        if (content.startsWith("<P>")) {
            code = "<P>";
            content = content.substring(3);
        } else if (content.startsWith("<L>")) {
            code = "<L>";
            content = content.substring(3);
        }

        let trimmed = content.trim();

        // 1. Dot-commands (skip display, attach to current logicalIndex)
        if (trimmed.startsWith(".")) {
            // Special case: enable commands
            // obs_enable
            if (trimmed.startsWith(".obs_enable")) {
                handleEnableCommand(trimmed);
                // Do NOT add to commands list
                continue;
            }

            const cmdText = trimmed.substring(1).toLowerCase();
            const match = trimmed.match(/^\.([a-zA-Z0-9_]+)(?:\((.*)\))?$/);
            if (match) {
                const cmd = match[1];
                const argRaw = match[2] || "";

                // Split on commas, trim whitespace, strip quotes
                const args = argRaw
                    .split(',')
                    .map(a => a.trim().replace(/^["']|["']$/g, ""))
                    .filter(a => a.length > 0);

                commands.push({
                    lineIndex: logicalIndex,
                    command: cmd,
                    args: args,
                    fired: false
                });
            } else {
                commands.push({
                    lineIndex: logicalIndex,
                    command: cmdText,
                    fired: false
                });
            }
            continue;
        }

        // 2. Determine visible line content
        let displayContent = trimmed;

        if (displayContent === "") {
            // zero-width space for true blank lines
            displayContent = "&#8203;";
        }

        // Record logical line
        cleanedLines.push(trimmed);  // keep the trimmed text (possibly "")

        // 3. Emit the line div
        output.push(`<div class="line" data-line="${logicalIndex}">`);
        output.push(displayContent);

        // 4. Emit the break element based on code
        if (code === "<P>") {
            output.push(`<div class="paragraph-break" data-parent="${logicalIndex}"></div>`);
        } else if (code === "<L>") {
            output.push(`<div class="line-break" data-parent="${logicalIndex}"></div>`);
        } else {
            // default break between normal lines
            output.push(`<div class="line-break" data-parent="${logicalIndex}"></div>`);
        }

        //3a. Close the line div
        output.push(`</div>`);

        logicalIndex++;

    }

    // 5. Ensure final runway logical line
    cleanedLines.push("");
    output.push(`<span class="line" data-line="${logicalIndex}">&#8203;</span>`);

    logicalIndex++;

    // 6. Render
    el.innerHTML = output.join("\n");

    scrollPos = 0;
    el.style.transform = "translateY(0px)";
    line = 0;

    // Rebuild the virtual line model now that cleanedLines is ready
    setHighlightBand(bandLines);            // *** still here, but now sets bandHeightPx
    const geometry = computeLineGeometry();
    const bandGeometry = computeBandGeometry();
}

function handleEnableCommand(trimmed) {

    if (!trimmed.startsWith("."))
        return false;

    // Split into command + optional argument
    const match = trimmed.match(/^\.(\w+_enable)(?:\((.*)\))?$/);

    cmd = match[1];
    let arg = match[2];
    if (arg != undefined) {
        arg = arg.trim();
        if (arg.length === 0)
            arg = null;
    } else {
        arg = null;
    }   

    window.chrome.webview.postMessage({
        action: cmd,
        argument: arg && arg.length > 0 ? arg : null
    });
    return true;

}

/**
 * Displays a highlight band in the teleprompter window (WebView)
 * @param {any} lines
 * @returns
 */
function setHighlightBand(lines) {

    bandLines = lines;

    const band = document.getElementById("highlightBand");
    const content = document.getElementById("content");
    if (!band || !content) return;

    // Use the same baseLineHeight the virtual model uses
    bandHeightPx = baseLineHeight * bandLines;  // *** CHANGED: store as global

    band.style.height = bandHeightPx + "px";
    content.style.paddingTop = bandHeightPx + "px";

    // Default: trigger in the center of the band
    bandCenterOffsetPx = 0;  // *** NEW: reset fine-tune when band size changes
}

/**
 * Computes the rendered line geometry for the teleprompter content.
 * Returns an object containing:
 *  - lineBottoms[]: cumulative bottom positions of each visual line
 *  - lineHeights[]: height of each visual line
 *  - totalHeight: total rendered height of the content
 */
function computeLineGeometry() {
    const blocks = document.querySelectorAll("#content .line");

    geometry = [];

    blocks.forEach((el, blockIndex) => {
        const type = el.classList.contains("line")
            ? "line"
            : el.classList.contains("line-break")
                ? "line-break"
                : "paragraph-break";

        // Logical line index:
        // - .line uses data-line
        // - breaks use data-parent
        const logicalLine = el.dataset.line ?? el.dataset.parent;

        let top, bottom;

        if (type === "line") {
            // Lines may wrap → multiple rects
            const rects = el.getClientRects();
            if (rects.length === 0) return; // Should never happen, but safe

            top = rects[0].top;
            bottom = rects[rects.length - 1].bottom;
        } else {
            // Breaks never wrap → one rect
            const rect = el.getBoundingClientRect();
            top = rect.top;
            bottom = rect.bottom;
        }

        geometry.push({
            blockIndex,
            type,
            logicalLine: Number(logicalLine),
            top,
            bottom,
            height: bottom - top
        });
    });

    line = 0;
}

/**
 * Gets the band position and height directly from the DOM 
 * @returns
 */
function computeBandGeometry() {
    const band = document.getElementById("highlightBand");
    const bandRect = band.getBoundingClientRect();
    const triggerPoint = document.getElementById("triggerPoint");
    const triggerPointRect = triggerPoint.getBoundingClientRect();

    // Convert band position into container-relative coordinates
    const bandTop = bandRect.top;
    const bandBottom = bandTop + bandRect.height;
    const bandCenter = bandTop + bandRect.height / 2;

    const triggerPointPos = triggerPointRect.top + triggerPointRect.height / 2;

    bandGeometry = {
        bandTop,
        bandBottom,
        bandCenter,
        triggerPointPos,
        bandHeight: bandRect.height
    };
}

/**
 * Starts a countdown timer and displays 3..2..1 in the view window
 * @param {any} callback
 * @returns
 */
function startCountdown(callback) {
    const overlay = document.getElementById("countdownOverlay");
    if (!overlay) {
        if (callback) callback();
        return;
    }

    const numbers = ["3", "2", "1"];
    let index = 0;

    overlay.style.opacity = 1; // fade in

    function showNext() {
        if (index < numbers.length) {
            overlay.textContent = numbers[index];
            index++;
            setTimeout(showNext, 1000);
        } else {
            overlay.style.opacity = 0; // fade out
            setTimeout(() => {
                overlay.textContent = "";
                if (callback) callback();
            }, 400);
        }
    }

    showNext();
}

function refocusSlideshow() {
    window.chrome.webview.postMessage({ action: "refocusSlideshow" });
}

function unpauseSlideshow() {
    window.chrome.webview.postMessage({ action: "unpauseSlideshow" });
}

function setSpeed(pxPerFrame) {
    scrollSpeed = pxPerFrame;
}

function startTeleprompter() {
    startCountdown(() => {
        resetCommands();
        refocusSlideshow();
        unpauseSlideshow();
        const geometry = computeLineGeometry();
        const bandGeometry = computeBandGeometry();
        line = 0;
        startScroll();
    });
}
function scrollByWheel(delta) {
    const scaled = delta.delta * 0.05
    manualScrollPos -= scaled;

    const content = document.getElementById("content");
    if (content) {
        content.style.transform = `translateY(${-manualScrollPos}px)`;
    }
}
function checkCommands() {
    for (let cmd of commands) {
        if (cmd.lineIndex === line && !cmd.fired) {
            cmd.fired = true;
            window.chrome.webview.postMessage({ action: cmd.command, args: cmd.args });
        }
    }
}

function resetCommands() {
    for (let cmd of commands) {
        cmd.fired = false;
    }
}

window.chrome.webview.addEventListener('message', event => {
    let msg;

    try {
        msg = JSON.parse(event.data);
    } catch (e) {
        console.error("Invalid JSON from C#:", event.data);
        return;
    }

    if (msg.action === "pause") {
        pauseScroll();
        if (msg.duration > 0) {
            setTimeout(() => {
                startScroll();
            }, msg.duration);
        }
    }

    if (msg.action === "startScroll") {
        startScroll();
    }

    if (msg.action === "stopScroll") {
        stopScroll();
    }
});

// ----- Scrolling -----
function startScroll() {
    if (scrolling) return;

    scrolling = true;
    window.chrome.webview.postMessage({ action: "scrollStarted" });

    const bodyStyles = window.getComputedStyle(document.body);
    const paddingTop = parseFloat(bodyStyles.paddingTop);
    contentOffset = bandHeightPx + paddingTop;
    triggerPoint = bandGeometry.triggerPointPos; 

    scrollInterval = setInterval(() => {
        scrollPos += scrollSpeed;

        const content = document.getElementById("content");
        if (content) {
            // We treat scrollPos as the single source of truth for virtual scroll,
            // and we just apply it to the DOM transform.
            content.style.transform = `translateY(${-scrollPos}px)`;
        }

        if (line < geometry.length && geometry[line].bottom - triggerPoint - scrollPos <= 0) {
            line = line + 1;
        }

        checkCommands();

    }, 16);
}

function pauseScroll() {
    scrolling = false;
    clearInterval(scrollInterval);
}

function stopScroll() {
    scrolling = false;
    clearInterval(scrollInterval);
    scrollPos = 0;
    const content = document.getElementById("content");
    if (content) {
        content.style.transform = "translateY(0px)";
    }
}

function setSpeed(pxPerFrame) {
    scrollSpeed = pxPerFrame;
}
