module.exports = function(http) {
    var io = require('socket.io')(http);

    var messagesCtrl = require('../controllers/messages');
    var Message = require('../models/message');

    console.log('starting chat');

    io.on('connection', function (socket) {
        var personId = socket.handshake.query.personId;
        console.log(personId + ' connected');
        socket.join(personId);
        
        socket.on('chat message', function (msg) {
            console.log('message: ' + JSON.stringify(msg));
            var message = new Message({
                text: msg.text,
                sender: personId,
                receiver: msg.receiver
            });
            messagesCtrl.saveMessage(message)
                .then(function(messageId) {
                    io.to(msg.receiver).emit('chat message', { sender: personId, text: msg.text });
                    io.to(personId).emit('message saved', { clientMessageId: msg.clientMessageId, messageId: messageId });
                }, function(err) {
                    io.to(personId).emit('message error', { clientMessageId: msg.clientMessageId, error: err });
                });
        });

        socket.on('message displayed', function(msgId) {
            messagesCtrl.setDisplayed(msgId)
            .then(function () {
                console.log('message set as displayed: ' + msgId);
                //TODO send that info to sender
            })
            .catch(function(err) {
                console.log('ERROR!!: failed set message as displayed: ' + msgId + ': ' + err);
            });
        });
    });
};
