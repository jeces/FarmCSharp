// Server
const dgram = require('dgram');
const server = dgram.createSocket('udp4');
const port = 4000;
const host = '127.0.0.1';
var db_config = require(__dirname + '/config/server1.js');
var conn = db_config.init();

//한글인코딩
var iconv = require('iconv-lite').iconv;

var 

//multer 업로드 사용
const multer = require('multer');
const upload = multer({dest:'uploads/', limits: {fileSize: 5 * 1024 * 1024}});
 
// udp 패킷 데이터가 수신될때 callback 된다.
// message parameter를 통해 수신데이터가 전달된다.
var data = '';

//접속자를 저장할 딕셔너리
var dictObject = {}

server.bind(port);

server.on('message', (message, info) => {

  var sql = 'SELECT * FROM MEMBER_INFO'; 
  //데이터베이스 연결
  conn.query(sql, function (err, rows, fields) {
    //실패시
    if(err) { console.log('query is not excuted. select fail...\n' + err); }
    // Login
    else {
      data = 'Hi i`m Server 나는 서버';
      var dataE = iconv.encode(data, 'euc-kr');
      for(var i = 0; i < rows.length; ++i) {
        if(rows[i].MEMBER_ID == message.toString()) {
          console.log(info.address);
          dictObject[`${message.toString()}`] = info.address + "#" + info.port;
          LoginMember(message.toString());
          server.send(dataE, 0, dataE.length, info.port, info.address);
          
          console.log(`message: ${message.toString()}님이 ${dataE}`);
          console.log(`from address: ${info.address} port: ${info.port}`);
          break;
        }
      }
    }
  });
  
  //server.send(data, 0, data.length, port, info.address);
  //test
  if(message.toString() == "Connect program1" ) {
    console.log(`[${message.toString()}], ${info.port}, ${info.address}`);
    console.log(`come`);
    //server.send(data, 0, data.length, cport, cport);
  }

    //server.send(data, 0, data.length, port, info.address);
  //test
  if(message.toString() == "Connect program" ) {
    console.log(`[${message.toString()}], ${info.port}, ${info.address}`);
    console.log(`come`);
  }

  // 접속유저 확인[임시]
  var str = message.toString();
  if(str == "Check") {
     for(var key in dictObject) {
          console.log("key : " + key + ", value : " + dictObject[key]);
     }
  }
  
  // Logout
  var str = message.toString().split('#');
  if(str[0] == "Logout") {
    LogoutMember(str[1]);
  }

  //Chat 신청
  var str = message.toString().split('#');
  if((str[0] == "Chat")) {
    Chatting(str[1], str[2]);
  }

  //요청을 받은사람이 수락을 누르면
  var str = message.toString().split('#');
  var id = str[1];
  if(str[0] == 'accept') {
    console.log(`${str[0]}, ${str[1]}`);
    var cip;
    var cport;
    var str2;
    for(var key in dictObject) {
        if(key == id) {
            str2 =  dictObject[key].split('#');
            cip = str2[0];
            cport = str2[1];
            console.log(`why not come ${cip} ${cport}`);
            break;
        }
    }
    data = `acceptM#${str[2]}`;
    console.log(`accept complete, ${cip}, ${cport} where`);
    server.send(data, 0, data.length, cport, cip);
  }

  // 채챙보냄
  var str = message.toString().split('#');
  if(str[0] == 'ChatMessage') {
     var str2;
     for(var key in dictObject) {
        console.log(`${key}, ${str[1]}`);   
        if(key == str[1]) {
           console.log('come?');
           str2 = dictObject[key].split('#');
           cip = str2[0];
           cport = str2[1];
           console.log(`${cip}, ${cport}`); 
        }
     }
     data = str[2];
     console.log(`${cip}, ${cport}, ${data}`);
     server.send(data, 0, data.length, cport, cip);
  }
});
// 로그인
function LoginMember(id) {
    var sql = `UPDATE MEMBER_INFO SET MEMBER_LOGIN = 'O' WHERE MEMBER_ID = '${id}'`; 
    conn.query(sql, function (err, rows, fields) {
    //실패시
        if(err) { console.log('query is not excuted. select fail...\n' + err); }
        //성공시
        else {}
    });
}
// 로그아웃
function LogoutMember(id) {
    var sql = `UPDATE MEMBER_INFO SET MEMBER_LOGIN = 'X' WHERE MEMBER_ID = '${id}'`;
    conn.query(sql, function (err, rows, fields) {
        if(err) { console.log('query is not excuted. select fail...\n' + err); }
        else {}
    });
}
// 1:1 채팅 요청
function Chatting(rid, sid) {
    var cip;
    var cport;
    var str;
    for(var key in dictObject) {
        if(key == rid) {
            console.log('next');
            console.log(key == rid);
            str =  dictObject[key].split('#');
            cip = str[0];
            cport = str[1];
            console.log(dictObject[key]);
            break;
        }
    }
    console.log(`${cip}, ${cport}, ${str}`);
    sendToChat(sid, rid, cport, cip);
    console.log('chat complete');
}

function sendToChat(id, rid, port, ip) {
    data = `${id}`;
    console.log(`${data}, ${port}, ${ip}`);
    server.send(data, 0, data.length, port, ip);
}

server.on('listening', () => {
  const address = server.address();
  console.log(`listening ${address.address}:${address.port}`);
});

server.on('close', function() {
	console.log('Close event');
});
 
//udp 데이터를 수신받기 위한 ip와 port를 등록한다.
