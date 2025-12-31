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
        .replace(/\r\n/g, "\n")
        .replace(/\r/g, "\n")
        .replace(/\v/g, "\n");

    const lines = text.split("\n");
    commands = [];

    let cleanedLines = [];

    for (let i = 0; i < lines.length; i++) {
        const line = lines[i].trim();

        if (line.startsWith(".")) {
            const cmd = line.substring(1).toLowerCase();

            // Check for pause(n)
            const match = cmd.match(/^pause\((\d+)\)$/);

            if (match) {
                commands.push({
                    lineIndex: cleanedLines.length,
                    command: "pause",
                    duration: parseInt(match[1], 10),
                    fired: false
                });
            }else{
                // Store command with its line index
                commands.push({
                    lineIndex: cleanedLines.length,
                    command: line.substring(1).toLowerCase(),
                    fired: false
                });
            }
            continue; // do NOT include this line in visible text
        }

        // Wrap the logical line in a span
        const content = line === "" ? "&#8203;" : line;
        const htmlLine = `<span class="line" data-line="${cleanedLines.length}">${content}</span>`;

        cleanedLines.push(htmlLine);
    }

    // Ensure final runway line exists
    if (!cleanedLines[cleanedLines.length - 1].includes("&#8203;")) {
        cleanedLines.push(
            `<span class="line" data-line="${cleanedLines.length}">&#8203;</span>`
        );
    }

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
