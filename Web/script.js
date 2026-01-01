let scrollPos = 0;
let scrollSpeed = 1;   // pixels per frame
let scrolling = false;
let scrollInterval = null;
let bandLines = 3;   // default

function triggerCommand(cmd) {
    switch (cmd.command) {
        case "nextslide":
            window.chrome.webview.postMessage({ action: "nextSlide" });
            break;
        case "pause":
            window.chrome.webview.postMessage({ action: "pause", duration:cmd.duration });
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

function checkCommands() {
    const content = document.getElementById("content");
    const lineHeight = parseFloat(window.getComputedStyle(content).lineHeight);

    const currentLine = getCurrentLogicalLine();

    for (let cmd of commands) {
        if (cmd.lineIndex === currentLine && !cmd.fired) {
            cmd.fired = true;
            triggerCommand(cmd);
        }
    }
}

function getCurrentLogicalLine() {
    const lines = document.querySelectorAll('.line');
    const triggerY = getTriggerY();

    for (let i = 0; i < lines.length; i++) {
        const rect = lines[i].getBoundingClientRect();
        const top = rect.top + window.scrollY;
        const bottom = rect.bottom + window.scrollY;

        if (top <= triggerY && bottom >= triggerY) {
            return i;
        }
    }

    return -1;
}

function getTriggerY() {
    const band = document.getElementById("highlightBand");
    const bandRect = band.getBoundingClientRect();

    const bandTop = bandRect.top + window.scrollY;
    const bandHeight = bandRect.height;

    return bandTop + (bandHeight / 2);
}


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
    document.getElementById("content").style.transform = "translateY(0px)";
}

function setSpeed(pxPerFrame) {
    scrollSpeed = pxPerFrame;
}

let commands = [];   // global list of commands

function loadNotes(text) {
    const el = document.getElementById("content");

    // Normalize line endings
    text = text
        .replace(/\r\n/g, "\n<P>")  // CRLF → paragraph
        .replace(/\v/g, "\n<L>")    // VT → line break
        .replace(/\r/g, "\n<P>");      // stray CR → normal line break

    const rawLines = text.split("\n");
    commands = [];

    let cleanedLines = [];

    for (let i = 0; i < rawLines.length; i++) {
        const original = rawLines[i];
        let trimmed = original.trim();


        // -----------------------------
        // 1. Dot-commands (skip display)
        // -----------------------------
        if (trimmed.startsWith(".")) {
            const cmd = trimmed.substring(1).toLowerCase();

            // pause(n)
            const match = cmd.match(/^pause\((\d+)\)$/);
            if (match) {
                commands.push({
                    lineIndex: cleanedLines.length,
                    command: "pause",
                    duration: parseInt(match[1], 10),
                    fired: false
                });
            } else {
                commands.push({
                    lineIndex: cleanedLines.length,
                    command: cmd,
                    fired: false
                });
            }
            continue;
        }

        // -----------------------------
        // 2. Determine line type
        // -----------------------------
        const code = trimmed.substring(0, 3);   // first 3 chars
        let content = trimmed

        let cssClass = "line";   // default

        if (code === "<P>") {
            cssClass = "paragraph";
            content = trimmed.length > 3 ? trimmed.slice(3) : "";
        } 
        else if (code === "<L>") {
            cssClass = "line";   // still a line, but no text
            content = trimmed.length > 3 ? trimmed.slice(3) : "";
        } 
        else {
            cssClass = "line";   // normal text line
        }
        
        // Future: dot-commands that create spacing blocks
        // Example: .b1, .b2, .b3
        // (We can wire this later when you’re ready)
        // if (someCondition) cssClass = "break1";

        // -----------------------------
        // 3. Build the span
        // -----------------------------
        content = content === "" ? "&#8203;" : content;

        const htmlLine =
            `<span class="${cssClass}" data-line="${cleanedLines.length}">` +
            `${content}</span>`;

        cleanedLines.push(htmlLine);
    }

    // -----------------------------
    // 4. Ensure final runway line
    // -----------------------------
    if (!cleanedLines[cleanedLines.length - 1].includes("&#8203;")) {
        cleanedLines.push(
            `<span class="line" data-line="${cleanedLines.length}">&#8203;</span>`
        );
    }

    // -----------------------------
    // 5. Render
    // -----------------------------
    el.innerHTML = cleanedLines.join("\n");

    scrollPos = 0;
    el.style.transform = "translateY(0px)";

    setHighlightBand(bandLines);
}

function setHighlightBand(lines) {
    bandLines = lines;

    // Estimate line height from computed style
    const content = document.getElementById("content");
    const lineHeight = parseFloat(window.getComputedStyle(content).lineHeight);

    const bandHeight = lineHeight * bandLines;

    const band = document.getElementById("highlightBand");
    band.style.height = bandHeight + "px";

    document.getElementById("content").style.paddingTop = bandHeight + "px";

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

function startCountdown(callback) {
    const overlay = document.getElementById("countdownOverlay");
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
