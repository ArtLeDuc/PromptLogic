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
    output.push(`<span class="line" data-line="${logicalIndex}">&#8203;</span>`);

    logicalIndex++;

    // 6. Render
    el.innerHTML = output.join("\n");

    scrollPos = 0;
    el.style.transform = "translateY(0px)";

    // Rebuild the virtual line model now that cleanedLines is ready
//    buildVirtualLineModel();                // *** still here
//    setHighlightBand(bandLines);            // *** still here, but now sets bandHeightPx
}
