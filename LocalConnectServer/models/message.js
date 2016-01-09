var mongoose = require('mongoose');

var messageSchema = new mongoose.Schema({
    text: String,
    sender: { type: mongoose.Schema.Types.ObjectId, ref: 'Person' },
    receiver: { type: mongoose.Schema.Types.ObjectId, ref: 'Person' },
    status: { type: Number, default: 1 }, // 1: saved, 2: delivered, 3: displayed 
    dateTime: { type : Date, default: Date.now }
});

module.exports = mongoose.model('Message', messageSchema);