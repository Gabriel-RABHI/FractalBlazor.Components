// Demo-only helpers.
window.fbDemo = {
    // Keeps a preference for a year, readable by the server render.
    setCookie: (name, value) =>
        document.cookie = `${name}=${encodeURIComponent(value)}; path=/; max-age=31536000; samesite=lax`
};
