var mongoose = require('mongoose');

var personSchema = new mongoose.Schema({
    firstname: String,
    surname: String,
    shortDescription: String,
    longDescription: String,
    user: {type: mongoose.Schema.Types.ObjectId, ref: 'User'}
});

module.exports = mongoose.model('Person', personSchema);