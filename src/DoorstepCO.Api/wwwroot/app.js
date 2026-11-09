"use strict";

const issueTexts = {
    Empty: () => "La dirección está vacía.",
    Unparseable: () => "No se reconoce como una dirección urbana (tipo de vía, número, cruce y placa).",
    RuralAddress: () => "Parece una dirección rural (vereda, finca, km…) y no sigue la nomenclatura urbana.",
    UnrecognizedText: (message) => {
        const detail = message.match(/"(.*)"$/);
        return detail
            ? `Se ignoró un texto que no se reconoce: «${detail[1]}».`
            : "Se ignoró un texto que no se reconoce.";
    },
    ZeroNumber: () => "La vía, el cruce o la placa es 0.",
    PlateNumberOutOfRange: () => "El número de placa es demasiado alto.",
};

const severeIssues = new Set(["Empty", "Unparseable", "RuralAddress"]);

const batchErrorTexts = {
    EmptyFile: "El archivo está vacío.",
    MissingAddressColumn: "El archivo no tiene una columna DIRECCION en la primera fila.",
    UnreadableFile: "No se pudo abrir el archivo. ¿Es un Excel .xlsx válido?",
    UnsupportedFileType: "Solo se aceptan archivos .csv y .xlsx.",
    FileTooLarge: "El archivo supera el tamaño máximo de 10 MB.",
};

const roadTypeLabels = {
    Calle: "Calle",
    Carrera: "Carrera",
    Diagonal: "Diagonal",
    Transversal: "Transversal",
    Avenida: "Avenida",
    AvenidaCalle: "Avenida calle",
    AvenidaCarrera: "Avenida carrera",
};

const complementLabels = {
    Apartamento: "Apartamento",
    Torre: "Torre",
    Interior: "Interior",
    Bloque: "Bloque",
    Casa: "Casa",
    Manzana: "Manzana",
    Lote: "Lote",
    Conjunto: "Conjunto",
    ConjuntoResidencial: "Conjunto residencial",
};

const genericError = "Algo salió mal. Intenta de nuevo en unos segundos.";

const $ = (id) => document.getElementById(id);

function show(element, text) {
    element.textContent = text;
    element.hidden = false;
}

function hide(...elements) {
    elements.forEach((element) => { element.hidden = true; });
}

/* Single address */

const singleForm = $("single-form");
const addressInput = $("address");

singleForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    hide($("single-error"));
    try {
        const response = await fetch("normalize", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ address: addressInput.value }),
        });
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }
        renderResult(await response.json());
    } catch {
        hide($("single-result"));
        show($("single-error"), genericError);
    }
});

document.querySelectorAll("[data-example]").forEach((chip) => {
    chip.addEventListener("click", () => {
        addressInput.value = chip.dataset.example;
        singleForm.requestSubmit();
    });
});

$("copy").addEventListener("click", async (event) => {
    const button = event.currentTarget;
    await navigator.clipboard.writeText($("normalized").textContent);
    button.textContent = "Copiada";
    setTimeout(() => { button.textContent = "Copiar"; }, 1500);
});

function renderResult(result) {
    const normalized = $("normalized");
    normalized.textContent = result.normalized ?? "No se pudo normalizar";
    normalized.classList.toggle("empty", result.normalized === null);
    $("copy").hidden = result.normalized === null;

    const percent = Math.round(result.confidence * 100);
    const bar = $("confidence-bar");
    $("confidence-value").textContent = `${percent} %`;
    bar.style.width = `${percent}%`;
    bar.style.background = percent >= 80 ? "var(--good)" : percent >= 50 ? "var(--warn)" : "var(--bad)";

    const issues = $("issues");
    issues.replaceChildren(...result.issues.map((issue) => {
        const item = document.createElement("li");
        const text = issueTexts[issue.code];
        item.textContent = text ? text(issue.message) : issue.message;
        item.classList.toggle("severe", severeIssues.has(issue.code));
        return item;
    }));
    if (result.issues.length === 0) {
        const item = document.createElement("li");
        item.className = "ok";
        item.textContent = "Sin problemas detectados.";
        issues.append(item);
    }

    $("components").replaceChildren(...componentsOf(result.address).map(([label, value]) => {
        const wrapper = document.createElement("div");
        const term = document.createElement("dt");
        const description = document.createElement("dd");
        term.textContent = label;
        description.textContent = value;
        wrapper.append(term, description);
        return wrapper;
    }));

    $("single-result").hidden = false;
}

function componentsOf(address) {
    if (!address) {
        return [];
    }

    return [
        ["Tipo de vía", roadTypeLabels[address.roadType] ?? address.roadType],
        ["Vía", formatRoad(address.road)],
        ["Cruce", formatRoad(address.cross)],
        ["Placa", String(address.plateNumber)],
        ...address.complements.map((complement) => [
            complementLabels[complement.type] ?? complement.type,
            complement.value,
        ]),
    ];
}

function formatRoad(road) {
    const parts = [`${road.number}${road.letter ?? ""}`];
    if (road.isBis) {
        parts.push(road.bisLetter ? `BIS ${road.bisLetter}` : "BIS");
    }
    if (road.quadrant) {
        parts.push(road.quadrant.toUpperCase());
    }
    return parts.join(" ");
}

/* Batch file */

const dropzone = $("dropzone");
const fileInput = $("file");
const processButton = $("process");
let selectedFile = null;
let downloadUrl = null;

function selectFile(file) {
    selectedFile = file ?? null;
    hide($("batch-status"), $("batch-error"));
    dropzone.classList.toggle("has-file", selectedFile !== null);
    processButton.disabled = selectedFile === null;
    const text = $("dropzone-text");
    if (selectedFile) {
        text.textContent = `${selectedFile.name} · ${formatSize(selectedFile.size)}`;
    } else {
        text.innerHTML = "<strong>Arrastra tu archivo aquí</strong> o haz clic para elegirlo";
    }
}

function formatSize(bytes) {
    return bytes < 1024 * 1024
        ? `${Math.max(1, Math.round(bytes / 1024))} KB`
        : `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

fileInput.addEventListener("change", () => selectFile(fileInput.files[0]));

["dragenter", "dragover"].forEach((type) => dropzone.addEventListener(type, (event) => {
    event.preventDefault();
    dropzone.classList.add("dragging");
}));

["dragleave", "drop"].forEach((type) => dropzone.addEventListener(type, (event) => {
    event.preventDefault();
    dropzone.classList.remove("dragging");
}));

dropzone.addEventListener("drop", (event) => selectFile(event.dataTransfer.files[0]));

processButton.addEventListener("click", async () => {
    if (!selectedFile) {
        return;
    }

    hide($("batch-status"), $("batch-error"));
    processButton.disabled = true;
    processButton.textContent = "Procesando…";
    try {
        const form = new FormData();
        form.append("file", selectedFile);
        const response = await fetch("normalize/batch", { method: "POST", body: form });
        if (!response.ok) {
            const problem = await response.json().catch(() => null);
            show($("batch-error"), batchErrorTexts[problem?.code] ?? genericError);
            return;
        }

        const blob = await response.blob();
        const fileName = fileNameOf(response) ?? "normalizada";
        if (downloadUrl) {
            URL.revokeObjectURL(downloadUrl);
        }
        downloadUrl = URL.createObjectURL(blob);
        download(downloadUrl, fileName);

        const status = $("batch-status");
        const link = document.createElement("a");
        link.href = downloadUrl;
        link.download = fileName;
        link.textContent = fileName;
        status.replaceChildren("Listo. Se descargó ", link, ".");
        status.hidden = false;
    } catch {
        show($("batch-error"), genericError);
    } finally {
        processButton.disabled = selectedFile === null;
        processButton.textContent = "Procesar archivo";
    }
});

function fileNameOf(response) {
    const disposition = response.headers.get("Content-Disposition") ?? "";
    const encoded = disposition.match(/filename\*=UTF-8''([^;]+)/i);
    if (encoded) {
        return decodeURIComponent(encoded[1]);
    }
    const plain = disposition.match(/filename="?([^";]+)"?/i);
    return plain ? plain[1] : null;
}

function download(url, fileName) {
    const link = document.createElement("a");
    link.href = url;
    link.download = fileName;
    document.body.append(link);
    link.click();
    link.remove();
}
