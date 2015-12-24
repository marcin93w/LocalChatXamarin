var mongoose = require('mongoose');

var personSchema = new mongoose.Schema({
    firstname: String,
    surname: String,
    shortDescription: String,
    longDescription: String,
    user: { type: mongoose.Schema.Types.ObjectId, ref: 'User' },
    location: {
        type: [Number],  // [<longitude>, <latitude>]
        index: '2d'      // create the geospatial index
    },
    avatar: String,
    locationDisruption: Number,
    jammedLocation: {
        type: [Number],  // [<longitude>, <latitude>]
        index: '2d'
    },
    locationJammerSettings: {
        xDisruption: Number,
        yDisruption: Number
    }
});

module.exports = mongoose.model('Person', personSchema);