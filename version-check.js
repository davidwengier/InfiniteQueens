// Version check and auto-reload for updates
(function() {
    let currentVersion = null;
    
    // On page load, check if we just did a version update and bust CSS cache
    function bustCssCache() {
        const url = new URL(window.location.href);
        const versionBust = url.searchParams.get('_v');
        if (versionBust) {
            // We just reloaded for a new version - bust CSS cache
            const links = document.querySelectorAll('link[rel="stylesheet"]');
            links.forEach(link => {
                const href = link.getAttribute('href');
                if (href && !href.includes('?')) {
                    link.setAttribute('href', href + '?_v=' + versionBust);
                }
            });
            
            // Clean up the URL (remove _v parameter)
            url.searchParams.delete('_v');
            window.history.replaceState({}, '', url.toString());
        }
    }
    
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
                    // Clear localStorage version
                    localStorage.removeItem('appVersion');
                    
                    // Clear all caches and then reload
                    if ('caches' in window) {
                        const names = await caches.keys();
                        await Promise.all(names.map(name => caches.delete(name)));
                    }
                    
                    // Navigate with cache-busting parameter to force fresh load
                    const url = new URL(window.location.href);
                    url.searchParams.set('_v', Date.now());
                    window.location.href = url.toString();
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
    
    // Bust CSS cache if we just reloaded for a version update
    bustCssCache();
    
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
