// 설치된 모듈 불러오기
const express = require('express');
const socket = require('socket.io');
const http = require('http');

// express 객체 생성
const app = express();

// express http 서버 생성
const server = http.createServer(app);

// 생성된 서버를 socket.io에 바인딩
const io = socket(server);

server.listen(4000, function() {
	console.log('Server On');
})

app.get('/', (req, res) => {
	res.send('send', 'send123');
});