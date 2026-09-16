// ~/site/js/weather.js
(function () {
    const widget = document.getElementById('weatherWidget');
    const tempEl = document.getElementById('weatherTemp');
    const conditionEl = document.getElementById('weatherCondition');
    const iconEl = document.getElementById('weatherIcon');
    const dateEl = document.getElementById('weatherDate');

    function renderWeather(data) {
        tempEl.textContent = `${Math.round(data.temperature)}°C`;
        conditionEl.textContent = data.condition;
        iconEl.src = data.iconUrl;
        dateEl.textContent = new Date(data.observedAt).toLocaleDateString(undefined, {
            weekday: 'short', day: 'numeric', month: 'short', year: 'numeric'
        });
        widget.style.display = 'flex';
    }

    function fetchWeather(lat, lng) {
        fetch(`/api/weather?lat=${lat}&lng=${lng}`)
            .then(res => {
                if (!res.ok) throw new Error('Weather fetch failed');
                return res.json();
            })
            .then(renderWeather)
            .catch(() => {
                // fail silently — widget just stays hidden, no user-facing error
            });
    }

    function init() {
        if (!('geolocation' in navigator)) return;

        navigator.geolocation.getCurrentPosition(
            (position) => {
                fetchWeather(position.coords.latitude, position.coords.longitude);
            },
            () => {
                // user denied or timed out — optional: fall back to a default location
                // fetchWeather(30.0444, 31.2357); // e.g. Cairo default
            },
            { timeout: 5000, maximumAge: 10 * 60 * 1000 } // reuse a cached browser position up to 10 min
        );
    }

    document.addEventListener('DOMContentLoaded', init);
})();