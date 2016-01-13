module.exports = function(http) {
    var io = require('socket.io')(http);

    var messagesCtrl = require('../controllers/messages');
    var peopleCtrl = require('../controllers/people');
    var Message = require('../models/message');

    console.log('starting chat');

    io.on('connection', function (socket) {
        var userId = socket.handshake.query.userId;
        var authToken = socket.handshake.query.authToken;

        peopleCtrl.checkToken(userId, authToken)
        .then(function () {
            console.log(userId + ' connected');
            socket.join(userId);
        
            socket.on('chat message', function (msg) {
                console.log('message: ' + JSON.stringify(msg));
                var message = new Message({
                    text: msg.text,
                    sender: userId,
                    receiver: msg.receiver
                });
                messagesCtrl.saveMessage(message)
                    .then(function(messageId) {
                        io.to(msg.receiver).emit('chat message', { id: messageId, sender: userId, text: msg.text });
                        io.to(userId).emit('message saved', { clientMessageId: msg.clientMessageId, messageId: messageId });
                    }, function(err) {
                        io.to(userId).emit('message error', { clientMessageId: msg.clientMessageId, error: err });
                    });
            });

            socket.on('message displayed', function(msgId) {
                messagesCtrl.setDisplayed(msgId)
                .then(function () {
                    console.log('message set as displayed: ' + msgId);
                    return messagesCtrl.findSender(msgId);
                })
                .then(function (msg) {
                    console.log('emmiting message '+msgId+' displayed to '  + msg.sender );
                    io.to(msg.sender).emit('message displayed', { messageId: msgId });
                })
                .catch(function(err) {
                    console.log('ERROR!!: failed set message as displayed: ' + msgId + ': ' + err);
                });
            });
        })
        .catch(function (error) { console.log(error); });
    });
};
