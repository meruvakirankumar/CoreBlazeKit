// CoreBlaze.Components — client-side helpers.
// Add to your host page:
//   <script src="_content/CoreBlaze.Components/coreblaze.js"></script>
// Works in Blazor Server, Blazor WebAssembly, and Blazor Web App (all render modes).
(function () {
    'use strict';
    window.CoreBlaze = window.CoreBlaze || {};

    // Triggers a browser download for the given base64-encoded content.
    // Used by DataGrid CSV / Excel / PDF export.
    window.CoreBlaze.download = function (filename, base64, mimeType) {
        mimeType = mimeType || 'text/csv;charset=utf-8';
        const link = document.createElement('a');
        link.href = 'data:' + mimeType + ';base64,' + base64;
        link.download = filename;
        link.style.display = 'none';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    };

    // Copies plain text to the clipboard. Falls back to a hidden textarea when
    // navigator.clipboard is unavailable (e.g. non-secure origins).
    window.CoreBlaze.copy = function (text) {
        if (navigator.clipboard && window.isSecureContext) {
            return navigator.clipboard.writeText(text);
        }
        const ta = document.createElement('textarea');
        ta.value = text;
        ta.style.position = 'fixed';
        ta.style.opacity = '0';
        document.body.appendChild(ta);
        ta.select();
        try { document.execCommand('copy'); } finally { document.body.removeChild(ta); }
        return Promise.resolve();
    };
})();
