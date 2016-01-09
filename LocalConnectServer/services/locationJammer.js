(function(locationJammer) {
    var maxDisruptionStep = 5;

    locationJammer.generateDisruptionSettings = function(maxDisruption) {
        var length = Math.random() * maxDisruption;
        var angle = Math.random() * 2 * Math.PI;
        return {
            xDisruption: length * Math.sin(angle),
            yDisruption: length * Math.cos(angle)
        };
    }
    
    function calculateNewDisruption(currentDisruptionX, currentDisruptionY, maxDisruption) {
        var disruptionStepX = Math.random() * maxDisruptionStep * 2 - maxDisruptionStep;
        currentDisruptionX += disruptionStepX;

        if (currentDisruptionX < -maxDisruption || currentDisruptionX > maxDisruption) {
            currentDisruptionX -= 2 * disruptionStepX;
        }
        
        var disruptionStepY = Math.random() * maxDisruptionStep * 2 - maxDisruptionStep;
        currentDisruptionY += disruptionStepY;
        
        if (currentDisruptionY < -maxDisruption || currentDisruptionY > maxDisruption) {
            currentDisruptionY -= 2 * disruptionStepY;
        }

        return {
            xDisruption: currentDisruptionX,
            yDisruption: currentDisruptionY
        };
    }
    
    function disturbCoordinatesByMetres (location, disruptionSettings) {
        var R = 6371000;
        
        var radDisruptionY = disruptionSettings.yDisruption / R;
        var radDisruptionX = disruptionSettings.xDisruption / (R * Math.cos(Math.PI * location.Lat / 180));
        
        var disturbedLat = location.Lat + (radDisruptionY * 180 / Math.PI);
        var disturbedLon = location.Lon + (radDisruptionX * 180 / Math.PI);

        return { Lon: disturbedLon, Lat: disturbedLat };
    }

    locationJammer.calculateJammedLocationAndNewDisruptionSettings = function(location, maxDisruption, disruptionSettings) {
        var newDisruptionSettings = calculateNewDisruption(disruptionSettings.xDisruption, disruptionSettings.yDisruption, maxDisruption);
        return {
            location: disturbCoordinatesByMetres(location, newDisruptionSettings),
            newDisruptionSettings: newDisruptionSettings
        };
    }
       
})(module.exports);