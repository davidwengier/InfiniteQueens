// Version check and auto-reload for updates
(function() {
    let currentVersion = null;
    
    async function checkVersion() {
        try {
            // Add cache-busting parameter to ensure fresh fetch
            const response = await fetch('version.json?_=' + Date.now(), {
                cache: 'no-store',
                headers: {
                    'Cache-Control': 'no-cache'
                }
            });
            
            if (!response.ok) return;
            
            const data = await response.json();
            
            if (currentVersion === null) {
                // First load, just store the version
                currentVersion = data.version;
                localStorage.setItem('appVersion', data.version);
            } else if (currentVersion !== data.version) {
                // New version detected!
                console.log('New version detected:', data.version);
                
                // Show notification
                if (confirm('A new version is available! Click OK to reload and get the latest updates.')) {
                    // Clear all caches
                    if ('caches' in window) {
                        caches.keys().then(names => {
                            names.forEach(name => caches.delete(name));
                        });
                    }
                    
                    // Clear localStorage version
                    localStorage.removeItem('appVersion');
                    
                    // Force reload from server
                    window.location.reload(true);
                }
            }
        } catch (error) {
            console.log('Version check failed:', error);
        }
    }
    
    // Check version on page load
    const storedVersion = localStorage.getItem('appVersion');
    if (storedVersion) {
        currentVersion = storedVersion;
    }
    
    // Initial check after page loads
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => {
            setTimeout(checkVersion, 2000);
        });
    } else {
        setTimeout(checkVersion, 2000);
    }
    
    // Check for updates every 5 minutes
    setInterval(checkVersion, 5 * 60 * 1000);
    
    // Check when page becomes visible again (user switches back to tab)
    document.addEventListener('visibilitychange', () => {
        if (!document.hidden) {
            checkVersion();
        }
    });
})();
