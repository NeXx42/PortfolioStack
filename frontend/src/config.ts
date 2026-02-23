export const BASE_URL =
    import.meta.env.VITE_BASE_URL ?? window.__ENV__?.BASE_URL ?? "http://localhost:5064";

console.log(import.meta.env.VITE_DEBUG_SLOW_API)
export const DEBUG_SLOW_API: boolean = import.meta.env.VITE_DEBUG_SLOW_API ?? false;