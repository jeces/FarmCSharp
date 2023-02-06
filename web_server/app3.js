// ��ġ�� ��� �ҷ�����
const express = require('express');
const socket = require('socket.io');
const http = require('http');

// express ��ü ����
const app = express();

// express http ���� ����
const server = http.createServer(app);

// ������ ������ socket.io�� ���ε�
const io = socket(server);

server.listen(4000, function() {
	console.log('Server On');
})

app.get('/', (req, res) => {
	res.send('send', 'send123');
});