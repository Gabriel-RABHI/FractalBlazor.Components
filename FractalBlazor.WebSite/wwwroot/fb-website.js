// Helpers of the FractalBlazor web site, shared by the server and WebAssembly hosts.
window.fbWebSite = {
    // Keeps a preference for a year.
    setCookie: (name, value) =>
        document.cookie = `${name}=${encodeURIComponent(value)}; path=/; max-age=31536000; samesite=lax`,

    getCookie: name => {
        const entry = document.cookie.split('; ').find(cookie => cookie.startsWith(name + '='));
        return entry ? decodeURIComponent(entry.substring(name.length + 1)) : null;
    },

    // Keeps a text in this browser; a browser that refuses storage is ignored.
    saveText: (key, text) => {
        try { localStorage.setItem(key, text); } catch { }
    },

    loadText: key => {
        try { return localStorage.getItem(key); } catch { return null; }
    },

    // Puts a text in the system clipboard; a browser that refuses it is ignored.
    copyText: text => navigator.clipboard ? navigator.clipboard.writeText(text).catch(() => { }) : undefined
};
