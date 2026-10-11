// Helpers of the FractalBlazor web site, shared by the server and WebAssembly hosts.
window.fbWebSite = {
    // Keeps a preference for a year.
    setCookie: (name, value) =>
        document.cookie = `${name}=${encodeURIComponent(value)}; path=/; max-age=31536000; samesite=lax`,

    getCookie: name => {
        const entry = document.cookie.split('; ').find(cookie => cookie.startsWith(name + '='));
        return entry ? decodeURIComponent(entry.substring(name.length + 1)) : null;
    }
};
