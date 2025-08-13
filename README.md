# RewashPlus

## Key design decisions (short)
Frontend: Blazor WebAssembly (C#), because you requested C# skill alignment and a single-language stack.

Offline storage: Blazored.LocalStorage for simplicity and reliability for prototype. For heavier usage we can switch to IndexedDB via a library.

Sync model: "Queue of pending bookings" stored in local storage. When online, the app iterates the queue and POSTs to an API endpoint (you can replace it later with Firebase or your own server). If the POST succeeds, the booking is marked synced and moved to a syncedBookings collection.

Service worker: Use the default PWA service worker from Blazor template for asset caching + a small published worker for production.

Online detection: small JS file (offline-sync.js) listens for online events and notifies Blazor via JS interop to trigger sync.
