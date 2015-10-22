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
    avatar: String
});

module.exports = mongoose.model('Person', personSchema);