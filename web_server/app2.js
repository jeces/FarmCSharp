var net = require('net')
var server = net.createServer(function(socket) {
	console.log('Client connect');
	socket.write('Welcome to Socket Server');

	socket.on('data', function(data) {
		console.log('client send : ',
		data.toString());
	});
});

server.on('listening', function() {
	console.log('Server is listening');
});

server.listen(4000);