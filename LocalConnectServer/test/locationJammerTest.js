var assert = require('assert');
var locationJammer = require('../services/locationJammer');

function checkMultipleTimes (number, test) {
    for (var i = 0; i < number; i++) {
        test();
    }
}

describe('locationJammer', function () {
    it('should generate disruption settings within given radius', function () {
        checkMultipleTimes(100, function() {
            var radius = 5;
            var settings = locationJammer.generateDisruptionSettings(radius);
            assert(
                Math.sqrt(
                    settings.xDisruption * settings.xDisruption + 
                    settings.yDisruption * settings.yDisruption) <= radius,
            'given radius: ' + radius + 
            ', returned values: x(' + settings.xDisruption + ')' + ' y('+ settings.yDisruption +')');
        });
    });

    it('should calculate correct location', function () {
        var location = { Lon: 20, Lat: 50 };
        var tolerance = 0.00018;
        var settings = { xDisruption: 200, yDisruption: 300 };
        var jammedLocation = locationJammer
            .calculateJammedLocationAndNewDisruptionSettings(location, 600, settings).location;
        assert(Math.abs(jammedLocation.Lon - 20.002877) < tolerance, 'Lon: ' + jammedLocation.Lon);
        assert(Math.abs(jammedLocation.Lat - 50.002698) < tolerance, 'Lat: ' + jammedLocation.Lat);
    });

    it('should change location settings correctly', function () {
        var location = { Lon: 20, Lat: 50 };
        var maxChange = 5;
        var settings = { xDisruption: 200, yDisruption: 300 };
        checkMultipleTimes(100, function() {
            var newSettings = locationJammer
                .calculateJammedLocationAndNewDisruptionSettings(location, 600, settings).newDisruptionSettings;
            assert(Math.abs(settings.xDisruption - newSettings.xDisruption) < maxChange);
            assert(Math.abs(settings.xDisruption - newSettings.xDisruption) < maxChange);
            settings = newSettings;
        });
    });
});