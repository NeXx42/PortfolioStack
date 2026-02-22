export { };

declare global {
    interface Window {
        __ENV__?: {
            BASE_URL?: string;
            [key: string]: any; // optional: allows more env vars
        };
    }
}