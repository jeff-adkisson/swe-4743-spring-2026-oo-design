import mermaid from "https://cdn.jsdelivr.net/npm/mermaid@11.14.0/dist/mermaid.esm.min.mjs";

const headCustomScript = document.querySelector('script[src*="head-custom.js"]');
const homeUrl = headCustomScript?.dataset.homeUrl || "/";

mermaid.initialize({
  startOnLoad: false,
  theme: "default",
  flowchart: { useMaxWidth: false },
  themeVariables: {
    background: "#ffffff",
    primaryColor: "#ffffff",
    primaryTextColor: "#000000",
    lineColor: "#000000",
    textColor: "#000000",
  },
});

// Builds a reusable control link used by Mermaid and table UI affordances.
function createControlLink(text, className, ariaLabel) {
  const link = document.createElement("a");
  link.href = "#";
  link.className = `head-custom-control ${className}`;
  link.textContent = text;
  link.setAttribute("aria-label", ariaLabel);
  return link;
}

// Copies text with the Clipboard API when available, with a textarea fallback.
async function copyTextToClipboard(text) {
  if (navigator.clipboard?.writeText) {
    await navigator.clipboard.writeText(text);
    return;
  }

  const textarea = document.createElement("textarea");
  textarea.value = text;
  textarea.style.position = "fixed";
  textarea.style.left = "-9999px";
  document.body.appendChild(textarea);
  textarea.select();
  document.execCommand("copy");
  document.body.removeChild(textarea);
}

// Serializes an inline SVG and opens the result in a separate browser tab.
function openSvgInNewTab(svg) {
  const clone = svg.cloneNode(true);

  if (!clone.getAttribute("xmlns")) {
    clone.setAttribute("xmlns", "http://www.w3.org/2000/svg");
  }

  const markup = new XMLSerializer().serializeToString(clone);
  const blob = new Blob([markup], { type: "image/svg+xml;charset=utf-8" });
  const blobUrl = URL.createObjectURL(blob);
  const newTab = window.open(blobUrl, "_blank", "noopener,noreferrer");

  if (!newTab) {
    URL.revokeObjectURL(blobUrl);
    return;
  }

  setTimeout(() => URL.revokeObjectURL(blobUrl), 60_000);
}

// Opens either an image source URL or an inline SVG in a new tab.
function openMediaInNewTab(element) {
  if (element instanceof HTMLImageElement) {
    const sourceUrl = element.currentSrc || element.src;

    if (sourceUrl) {
      window.open(sourceUrl, "_blank", "noopener,noreferrer");
    }

    return;
  }

  if (element instanceof SVGElement) {
    openSvgInNewTab(element);
  }
}

// Finds or creates a standard control row under a custom content block.
function ensureControlsContainer(parent) {
  const existingControls = Array.from(parent.children).find((child) =>
    child.classList?.contains("head-custom-controls"),
  );

  if (existingControls) {
    return existingControls;
  }

  const controls = document.createElement("div");
  controls.className = "head-custom-controls";
  parent.appendChild(controls);
  return controls;
}

// Converts GitHub-style markdown alert blockquotes into styled local preview alerts.
function transformGitHubAlerts() {
  const alertPattern = /^\s*\[!(NOTE|TIP|IMPORTANT|WARNING|CAUTION)\]\s*/i;
  const blockquotes = document.querySelectorAll("blockquote");

  blockquotes.forEach((blockquote) => {
    if (blockquote.dataset.githubAlertInitialized === "true") {
      return;
    }

    const firstParagraph = blockquote.querySelector(":scope > p");

    if (!firstParagraph) {
      return;
    }

    const markerMatch = firstParagraph.textContent?.match(alertPattern);

    if (!markerMatch) {
      return;
    }

    const alertType = markerMatch[1].toLowerCase();
    firstParagraph.innerHTML = firstParagraph.innerHTML.replace(alertPattern, "").trimStart();

    const title = document.createElement("strong");
    title.className = "gh-alert-title";
    title.textContent = markerMatch[1].charAt(0).toUpperCase() + markerMatch[1].slice(1).toLowerCase();

    firstParagraph.prepend(title);
    blockquote.classList.add("gh-alert", `gh-alert-${alertType}`);
    blockquote.dataset.githubAlertInitialized = "true";
  });
}

// Toggles squeezed table state and records whether it was automatic or manual.
function setTableSqueezed(wrapper, squeezed, mode = "") {
  wrapper.classList.toggle("is-squeezed", squeezed);
  wrapper.dataset.squeezeMode = squeezed ? mode : "";
}

// Creates the explicit squeeze control for wide tables.
function createSqueezeButton(wrapper, table) {
  const button = createControlLink(
    "Squeeze",
    "table-squeeze",
    "Squeeze table to page width",
  );

  button.addEventListener("click", (event) => {
    event.preventDefault();
    setTableSqueezed(wrapper, true, "manual");
    updateTableSqueezeButtons();
  });

  return button;
}

// Finds or creates the floating control container for a scrollable table.
function ensureTableControls(wrapper, table) {
  const existingControls = wrapper.querySelector(":scope > .table-scroll-controls");

  if (existingControls) {
    return existingControls;
  }

  const controls = document.createElement("div");
  controls.className = "table-scroll-controls";
  controls.appendChild(createSqueezeButton(wrapper, table));
  wrapper.appendChild(controls);
  return controls;
}

// Allows table headers to trigger squeeze when that table currently supports it.
function enableTableHeaderSqueeze(table) {
  const headerCells = table.querySelectorAll("th");

  headerCells.forEach((headerCell) => {
    if (headerCell.dataset.squeezeClickInitialized === "true") {
      return;
    }

    headerCell.addEventListener("click", (event) => {
      if (event.target.closest("a, button, input, select, textarea, label")) {
        return;
      }

      const wrapper = headerCell.closest(".table-scroll");

      if (!wrapper?.classList.contains("has-table-controls")) {
        return;
      }

      setTableSqueezed(wrapper, true, "manual");
      updateTableSqueezeButtons();
    });

    headerCell.dataset.squeezeClickInitialized = "true";
  });
}

// Refreshes squeeze state and control visibility for all wrapped tables.
function updateTableSqueezeButtons() {
  const wrappers = document.querySelectorAll(".table-scroll");

  wrappers.forEach((wrapper) => {
    const table = wrapper.querySelector(":scope > table");

    if (!table) {
      return;
    }

    const controls = ensureTableControls(wrapper, table);
    const naturalWidth = Number(wrapper.dataset.naturalWidth || table.scrollWidth);
    const shouldAutoSqueeze = naturalWidth > 960 && naturalWidth < 1200;

    if (wrapper.dataset.squeezeMode !== "manual") {
      setTableSqueezed(wrapper, shouldAutoSqueeze, shouldAutoSqueeze ? "auto" : "");
    }

    const hasHorizontalScroll = wrapper.scrollWidth > wrapper.clientWidth + 1;

    controls.style.display = hasHorizontalScroll ? "block" : "none";
    wrapper.classList.toggle("has-table-controls", hasHorizontalScroll);

    table.querySelectorAll("th").forEach((headerCell) => {
      if (hasHorizontalScroll) {
        headerCell.setAttribute("title", "Click to squeeze table to page width");
      } else {
        headerCell.removeAttribute("title");
      }
    });
  });
}

// Wraps rendered tables so they can scroll horizontally and host table controls.
function wrapTablesForHorizontalScroll() {
  const tables = document.querySelectorAll("table");

  tables.forEach((table) => {
    if (table.parentElement?.classList.contains("table-scroll")) {
      return;
    }

    const wrapper = document.createElement("div");
    wrapper.className = "table-scroll";
    table.replaceWith(wrapper);
    wrapper.appendChild(table);
    wrapper.dataset.naturalWidth = String(Math.ceil(table.scrollWidth));
    enableTableHeaderSqueeze(table);
  });

  updateTableSqueezeButtons();
}

// Makes the theme header navigate home while preserving explicit interactive targets.
function enableHeaderHomeNavigation() {
  const header = document.querySelector("#header_wrap header");

  if (!header || header.dataset.homeNavigationInitialized === "true") {
    return;
  }

  header.addEventListener("click", (event) => {
    if (event.target.closest("#forkme_banner, a, button, input, select, textarea, label")) {
      return;
    }

    window.location.href = homeUrl;
  });

  header.dataset.homeNavigationInitialized = "true";
}

// Finds or creates the floating button used to return to the top of the page.
function ensureScrollTopButton() {
  const existingButton = document.querySelector(".scroll-top-button");

  if (existingButton) {
    return existingButton;
  }

  const button = document.createElement("button");
  button.type = "button";
  button.className = "scroll-top-button";
  button.textContent = "Top";
  button.setAttribute("aria-label", "Return to top of page");
  button.addEventListener("click", () => {
    window.scrollTo({ top: 0, behavior: "smooth" });
  });
  document.body.appendChild(button);
  return button;
}

// Shows the top button only when the heading is out of view on larger screens.
function enableScrollTopButton() {
  const button = ensureScrollTopButton();
  const heading =
    document.querySelector("#project_title") ||
    document.querySelector("#main_content > h1") ||
    document.querySelector("h1");

  if (!heading || button.dataset.scrollSpyInitialized === "true") {
    return;
  }

  const viewportQuery = window.matchMedia("(min-width: 800px)");

  const updateVisibility = (headingVisible) => {
    const shouldShow = viewportQuery.matches && !headingVisible;
    button.classList.toggle("is-visible", shouldShow);
  };

  if ("IntersectionObserver" in window) {
    const observer = new IntersectionObserver(([entry]) => {
      updateVisibility(entry.isIntersecting);
    });

    observer.observe(heading);
  } else {
    const onScroll = () => {
      const rect = heading.getBoundingClientRect();
      const headingVisible = rect.bottom > 0 && rect.top < window.innerHeight;
      updateVisibility(headingVisible);
    };

    window.addEventListener("scroll", onScroll, { passive: true });
    onScroll();
  }

  viewportQuery.addEventListener("change", () => {
    const rect = heading.getBoundingClientRect();
    const headingVisible = rect.bottom > 0 && rect.top < window.innerHeight;
    updateVisibility(headingVisible);
  });

  button.dataset.scrollSpyInitialized = "true";
}

// Makes standalone images and SVGs clickable so they open in a new tab.
function enableMediaClickToExpand() {
  const mediaElements = document.querySelectorAll("img, svg");

  mediaElements.forEach((element) => {
    if (element.dataset.clickToOpenInitialized === "true") {
      return;
    }

    if (element.closest("a, button")) {
      return;
    }

    element.classList.add("clickable-media");
    element.setAttribute("title", "Open in a new tab");

    if (element instanceof HTMLImageElement) {
      element.setAttribute("aria-label", element.alt || "Open image in a new tab");
    }

    element.addEventListener("click", () => {
      openMediaInNewTab(element);
    });

    element.dataset.clickToOpenInitialized = "true";
  });
}

// Replaces fenced Mermaid code blocks with rendered diagrams and copy controls.
async function renderMermaidFromCodeBlocks() {
  const blocks = document.querySelectorAll("pre > code.language-mermaid");

  blocks.forEach((code) => {
    const pre = code.parentElement;

    if (!pre) {
      return;
    }

    const wrap = document.createElement("div");
    wrap.className = "mermaid-wrap";

    const container = document.createElement("div");
    container.className = "mermaid";

    const mermaidSource = (code.textContent || "").trimEnd();
    container.textContent = mermaidSource;
    wrap.appendChild(container);

    const controls = ensureControlsContainer(wrap);

    const copy = createControlLink(
      "Copy Mermaid",
      "mermaid-copy",
      "Copy Mermaid code to clipboard",
    );
    copy.dataset.mermaidSource = mermaidSource;

    copy.addEventListener("click", async (event) => {
      event.preventDefault();

      try {
        await copyTextToClipboard(copy.dataset.mermaidSource || "");

        const oldText = copy.textContent;
        copy.textContent = "Copied!";
        setTimeout(() => {
          copy.textContent = oldText;
        }, 1200);
      } catch {
        const oldText = copy.textContent;
        copy.textContent = "Copy failed";
        setTimeout(() => {
          copy.textContent = oldText;
        }, 1400);
      }
    });

    controls.appendChild(copy);

    pre.replaceWith(wrap);
  });

  await mermaid.run({ querySelector: ".mermaid" });
}

// Applies all local preview enhancements after the page content is ready.
async function initializeHeadCustom() {
  transformGitHubAlerts();
  await renderMermaidFromCodeBlocks();
  wrapTablesForHorizontalScroll();
  enableHeaderHomeNavigation();
  enableScrollTopButton();
  enableMediaClickToExpand();

  window.addEventListener("resize", updateTableSqueezeButtons);
}

if (document.readyState === "loading") {
  document.addEventListener("DOMContentLoaded", () => {
    initializeHeadCustom();
  });
} else {
  initializeHeadCustom();
}
