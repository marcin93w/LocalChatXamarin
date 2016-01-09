var mongoose = require('mongoose');
var bcrypt = require('bcryptjs');
var hat = require('hat');
var SHA256 = require("crypto-js/sha256");
var q = require('q');
var config = require('../config');

var userSchema = new mongoose.Schema({
    username: String,
    password: String,
    facebookId: String,
    token: String,
    firstname: String,
    surname: String,
    shortDescription: String,
    longDescription: String,
    avatar: String,
    location: {
        type: [Number],  // [<longitude>, <latitude>]
        index: '2d'      // create the geospatial index
    },
    jammedLocation: {
        type: [Number],
        index: '2d'
    },
    settings: {
        locationDisruption: Number,
        peopleDisplayCount: Number
    },
    locationJammerSettings: {
        xDisruption: Number,
        yDisruption: Number
    }
});

userSchema.methods.verifyPassword = function (password, callback) {
    bcrypt.compare(password, this.password, callback);
};

userSchema.methods.verifyToken = function (token, callback) {
    callback(null, this.token === SHA256(token).toString());
};

userSchema.methods.generateNewToken = function () {
    var defer = q.defer();
    var token = hat();
    this.token = SHA256(token);
    this.save(function (err) {
        if (err) defer.reject(err);
        defer.resolve(token);
    });
    
    return defer.promise;
};

userSchema.methods.savePassword = function (password) {
    var defer = q.defer();
    var user = this;
    bcrypt.hash(password, 8, function (err, hash) {
        if (err) defer.reject(err);
        user.password = hash;
        user.save(function(err) {
            if (err) defer.reject(err);
            defer.resolve();
        });
    });

    return defer.promise;
}

module.exports = mongoose.model('User', userSchema);