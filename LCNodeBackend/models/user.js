var mongoose = require('mongoose');

var userSchema = new mongoose.Schema({
    username: String,
    password: String,
    token: String
});

userSchema.methods.verifyPassword = function (password, callback) {
    callback(null, this.password === password);
};

userSchema.methods.verifyToken = function (token, callback) {
    callback(null, this.token === token);
};

userSchema.methods.generateNewToken = function () {
    this.token = this.username;
    return this.save();
};

module.exports = mongoose.model('User', userSchema);