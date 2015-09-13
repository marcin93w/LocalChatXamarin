module.exports = function(http) {
    var io = require('socket.io')(http);

    console.log('starting chat');

    io.on('connection', function (socket) {
        var userId = socket.handshake.query.userId;
        console.log(userId + ' connected');
        socket.join(userId);
        socket.on('chat message', function (msg) {
            console.log('message: ' + JSON.stringify(msg));
            io.to(msg.receiver).emit('chat message', {sender: userId, text: msg.text});
        });
    });
};
