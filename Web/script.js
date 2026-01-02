let scrollPos = 0;
let scrollSpeed = 1;   // pixels per frame
let scrolling = false;
let scrollInterval = null;
let bandLines = 3;   // default
let triggerFudge = 0;

// ----- Command model -----

let commands = [];          // global list of commands
let cleanedLines = [];      // logical lines that are actually displayed
let lineHeights = [];       // per-logical-line height in px
let lineOffsets = [];       // cumulative offset (top) for each logical line
let totalHeight = 0;        // total logical height
let baseLineHeight = 40;    // fallback; will be updated from CSS
let triggerOffset = 0;

// ----- Command triggering -----

function triggerCommand(cmd) {
    switch (cmd.command) {
        case "nextslide":
            window.chrome.webview.postMessage({ action: "nextSlide" });
            break;
        case "pause":
            window.chrome.webview.postMessage({ action: "pause", duration: cmd.duration });
            break;
        case "stop":
            window.chrome.webview.postMessage({ action: "stop" });
            break;
        // future commands:
        // case "speed+50":
        // case "speed-20":
        // case "goto 5":
        // case "color red":
    }
}

// Virtual line model: given a scroll offset, return logical line index
function getCurrentLogicalLine(scrollTop) {
    if (cleanedLines.length === 0) {
        return 0;
    }

    // Binary search over lineOffsets
    let low = 0;
    let high = lineOffsets.length - 1;

    while (low <= high) {
        const mid = (low + high) >> 1;
        const start = lineOffsets[mid];
        const end = (mid + 1 < lineOffsets.length) ? lineOffsets[mid + 1] : Number.POSITIVE_INFINITY;

        if (scrollTop < start) {
            high = mid - 1;
        } else if (scrollTop >= end) {
            low = mid + 1;
        } else {
            return mid;
        }
    }

    return 0;
}

function checkCommands() {
    const currentLine = getCurrentLogicalLine(scrollPos - triggerOffset - triggerFudge);

    for (let cmd of commands) {
        if (cmd.lineIndex === currentLine && !cmd.fired) {
            cmd.fired = true;
            triggerCommand(cmd);
        }
    }
}

// ----- Virtual line height model -----

function computeLogicalLineHeight(index) {
    const line = cleanedLines[index];
    let h = baseLineHeight;

    // Blank logical lines get smaller spacing
    if (line === "") {
        h = baseLineHeight * 0.5;
    }

    // Paragraph-style spacing: if previous logical line is blank, add extra space
    if (index > 0 && cleanedLines[index - 1] === "") {
        h = baseLineHeight * 1.5;
    }

    return h;
}

function buildVirtualLineModel() {
    const contentEl = document.getElementById("content");
    if (contentEl) {
        const computed = window.getComputedStyle(contentEl);
        const lh = parseFloat(computed.lineHeight);
        if (!isNaN(lh) && lh > 0) {
            baseLineHeight = lh;
        }
    }

    lineHeights = [];
    lineOffsets = [];
    totalHeight = 0;

    for (let i = 0; i < cleanedLines.length; i++) {
        const h = computeLogicalLineHeight(i);
        lineHeights.push(h);
        lineOffsets.push(totalHeight);
        totalHeight += h;
    }
}

// ----- Scrolling -----

function startScroll() {
    if (scrolling) return;
    scrolling = true;

    scrollInterval = setInterval(() => {
        scrollPos += scrollSpeed;

        const content = document.getElementById("content");
        content.style.transform = `translateY(${-scrollPos}px)`;

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

// ----- Notes loading and rendering -----

function loadNotes(text) {
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
            const cmdText = trimmed.substring(1).toLowerCase();

            const match = cmdText.match(/^pause\((\d+)\)$/);
            if (match) {
                commands.push({
                    lineIndex: logicalIndex,
                    command: "pause",
                    duration: parseInt(match[1], 10),
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

        // 3. Emit the line span
        output.push(
            `<span class="line" data-line="${logicalIndex}">${displayContent}</span>`
        );

        logicalIndex++;

        // 4. Emit the break element based on code
        if (code === "<P>") {
            output.push(`<div class="paragraph-break"></div>`);
        } else if (code === "<L>") {
            output.push(`<div class="line-break"></div>`);
        } else {
            // default break between normal lines
            output.push(`<div class="line-break"></div>`);
        }
    }

    // 5. Ensure final runway logical line
    cleanedLines.push("");
    output.push(
        `<span class="line" data-line="${logicalIndex}">&#8203;</span>`
    );
    logicalIndex++;

    // 6. Render
    el.innerHTML = output.join("\n");

    scrollPos = 0;
    el.style.transform = "translateY(0px)";

    // Rebuild the virtual line model now that cleanedLines is ready
    buildVirtualLineModel();

    // Set the highlight band based on the computed base line height
    setHighlightBand(bandLines);
}

function setHighlightBand(lines) {
    bandLines = lines;

    const band = document.getElementById("highlightBand");
    const content = document.getElementById("content");
    if (!band || !content) return;

    // Use the same baseLineHeight the virtual model uses
    const bandHeight = baseLineHeight * bandLines;

    band.style.height = bandHeight + "px";
    content.style.paddingTop = bandHeight + "px";

    triggerOffset = bandHeight * 1.5;
    triggerFudge = baseLineHeight * 2;   // adjust this number
}

// ----- WebView message handling -----

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
        setTimeout(() => {
            startScroll();
        }, msg.duration);
    }

    if (msg.action === "startScroll") {
        startScroll();
    }

    if (msg.action === "stopScroll") {
        stopScroll();
    }
});

// ----- Countdown and slideshow control -----

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

function startTeleprompter() {
    startCountdown(() => {
        refocusSlideshow();
        unpauseSlideshow();
        startScroll();
    });
}
