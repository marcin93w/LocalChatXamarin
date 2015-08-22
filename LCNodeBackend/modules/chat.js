module.exports = function(http) {
    var io = require('socket.io')(http);

    console.log('starting chat');

    io.on('connection', function(socket) {
        console.log('a user connected');
        socket.on('chat message', function(msg) {
            console.log('message: ' + msg);
            io.emit('chat message', msg);
        });
    });
};
