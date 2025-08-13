// wwwroot/offline-sync.js

window.rewashOffline = {
    initialize: function (dotnetHelper) {
        // Expects dotnetHelper to be a DotNetObjectReference with a method 'TriggerSync'
        window.addEventListener('online', () => {
            console.log('Browser online - notifying Blazor to sync');
            try {
                dotnetHelper.invokeMethodAsync('TriggerSync');
            } catch (e) {
                console.warn('Failed to invoke .NET sync method', e);
            }
        });
    }
};
