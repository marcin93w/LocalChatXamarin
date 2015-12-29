var mongoose = require('mongoose');

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
    callback(null, this.password === password);
};

userSchema.methods.verifyToken = function (token, callback) {
    callback(null, this.token === token);
};

userSchema.methods.generateNewToken = function () {
    this.token = this.username || this.facebookId;
    return this.save();
};

module.exports = mongoose.model('User', userSchema);