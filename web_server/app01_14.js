// Server
const dgram = require('dgram');
const server = dgram.createSocket('udp4');
const port = 4000;
const host = '127.0.0.1';
var db_config = require(__dirname + '/config/server1.js');
var conn = db_config.init();
var client = [];

//한글인코딩, 파일
var iconv = require('iconv-lite');
var fs = require('fs');

// utf8인코딩
const utf8 = require('utf8');

// udp 패킷 데이터가 수신될때 callback 된다.
// message parameter를 통해 수신데이터가 전달된다.
var data = '';

//접속자를 저장할 딕셔너리
var dictObject = {}

// udp 서버가 초기화 되고 데이터를 수신받을 준비가 되었을때 callback 된다.
server.on('listening', () => {
  const address = server.address();
  console.log(`listening ${address.address}:${address.port}.. 요청을 받아들일 준비 완료`);
});
// udp 데이터를 수신받기 위한 ip와 port를 등록한다.
server.bind(port);

// udp 패킷 데이터가 수신될때 callback 된다.
// message parameter를 통해 수신데이터가 전달된다.
server.on('message', (message, info) => {
  // 이거 수정해야됨
  
  // 현재 접속유저 업데이트 계속
  var sql = 'SELECT * FROM MEMBER_INFO'; 
  console.log(`안드로이드 연결 성공${info.address} ${info.port}`);
  // 안드로이드 연결 로그인[중복, 비밀번호, 아이디, 승인검사]
  var str = message.toString().split('#');
  console.log(str);
  if(str[0] == 'android') {
        
        var data = r
        server.send(data, 0, data.length, info.port, info.address);
  }

  // 로그인 이미지
  var str = message.toString().split('#');
  if(str[0] == 'LoginImage') {
    
    var imagefile = 'login.png';
    fs.readFile(imagefile, function(err, data) {
        //console.log(data);
        server.send(data, 0, data.length, info.port, info.address);
        console.log(`${info.port}`);
    });
  }
  
  // Data 이미지
  var str00 = message.toString().split('#');
  if(str00[0] == 'DataImage') {
        var imagefile1 = 'data.jpg';
        fs.readFile(imagefile1, function(err, data) {
            console.log(data);
            //server.send(data, 0, data.length, info.port, info.address);
            
            for(var key in dictObject) {
                
                if(key == str00[1]) {
                    
                    console.log(`dataimage `);
                    str2 =  dictObject[key].split('#');
                    cip = str2[0];
                    cport = str2[1];
                    console.log(data);
                    console.log(`${cip}, ${cport}, ${info.port}`);
                    server.send(data, 0, data.length, cport, cip);
                    return;
                }
            }
        });
  }
  
  // 채팅종료 시 남아있는 receive 처리를 위한 코드
  var str0 = message.toString().split('#');
  if(str0[0] == 'emptyReceive') {
        var dataD = 'clean#';
        //server.send(dataD, 0, dataD.length, info.port, info.address);
        for(var key in dictObject) {
            console.log(`뭐지? : ${str0}`);
            var str2 =  dictObject[key].split('#');
            if(key == str0[1]) {
                continue;
            }
            console.log("123");
            cip = str2[0];
            cport = str2[1];
            var wa = str2[2]
            if(wa == 'Android') {
                continue
            }
            server.send(dataD, 0, dataD.length, cport, cip);
        }
  }
  
  // 로그인 시
  var str1 = message.toString().split('#');
  if(str1[0] == 'LoginCheck') {
        var sql = 'SELECT MEMBER_ID, MEMBER_PW, MEMBER_APPROVAL FROM MEMBER_INFO';
        var pw = '';
        conn.query(sql, function (err, rows, fields) {
            if(err) { console.log('query is not excuted. select fail...\n' + err); }  
            else {
                for(var i = 0; i < rows.length; ++i) {
                    if(rows[i].MEMBER_ID == str1[1]) {
                        if(rows[i].MEMBER_APPROVAL == 0) {
                            var dataC = 'notApproval';
                            server.send(dataC, 0, dataC.length, info.port, info.address);
                            return;
                        }
                        pw = rows[i].MEMBER_PW;
                        //중복접속검사

                        for(var key in dictObject) {
                            console.log(`검사 : ${key}, ${str1[1]}`);
                            if(key == str1[1]) {
                                dataD = 'loginDupli';
                                server.send(dataD, 0, dataD.length, info.port, info.address);
                                return;
                            }
                        }
                        successCheck(str1[1], pw, str1[2], info.port, info.address, str1[3]);
                        return;


                        /*
                        var dataD = '';
                        console.log(`검사 : ${loginCheks(str[1])}`);
                        if(loginCheks(str[1]) == false) {
                             dataD = 'loginDupli';
                             server.send(dataD, 0, dataD.length, info.port, info.address);
                             return;
                        }else {
                            successCheck(str[1], pw, str[2], info.port, info.address);
                            return;
                        }
                        */
                    }
                }
                var dataN = 'NoId';
                server.send(dataN, 0, dataN.length, info.port, info.address);
                return;
            }
        });
  }
  // 로그아웃 시
  var str2 = message.toString().split('#');
  if(str2[0] == "Logout") {
    console.log(`logtout`)
    LogoutMember(str2[1]);
  }
  // 회원가입
  var str3 = message.toString().split('#');
  if(str3[0] == "JoinCheck") {
    var sql = `INSERT INTO MEMBER_INFO VALUES('${str3[1]}', HEX(AES_ENCRYPT('${str3[2]}', 'member')), '${str3[3]}', '${str3[4]}', '${str3[5]}', NOW(), 0, 'X')`;
    conn.query(sql, function(err, rows, fields) {
        if(err) { console.log('query is not excuted. select fail...\n' + err); }
        else {
            var dataJ = 'successJoin';
            server.send(dataJ, 0, dataJ.length, info.port, info.address);
            console.log("complete");
        }
    });
  }
  // 중복검사
  var str4 = message.toString().split('#');
  if(str4[0] == "duplicat") {
    var sql = `SELECT COUNT(*) FROM MEMBER_INFO WHERE MEMBER_ID = '${str4[1]}'`;
    conn.query(sql, function(err, rows, fields) {
        if(err) { console.log('query is not excuted. select fail...\n' + err); }
        else {
            if(rows[0]['COUNT(*)'] == '0') {
                dataE = 'notDuplicate';
                server.send(dataE, 0, dataE.length, info.port, info.address);
            } else {
                dataE = 'Duplicat';
                server.send(dataE, 0, dataE.length, info.port, info.address);
            }
        }
    });
  }
  // 현재 접속 유저
  var str5 = message.toString().split('#');
  if(str5[0] == "currentUser") {
    var sql = `SELECT MEMBER_ID, MEMBER_NAME, MEMBER_PHONE, MEMBER_LOGIN FROM (SELECT MEMBER_ID, MEMBER_NAME, MEMBER_PHONE, MEMBER_LOGIN FROM MEMBER_INFO WHERE MEMBER_LOGIN = 'A' OR MEMBER_LOGIN = 'W') A`
    conn.query(sql, function(err, rows, fields) {
        if(err) { console.log('query is not excuted. select fail...\n' + err); }
        else {
            console.log(`크기 : ${JSON.stringify(rows).length}`)
            console.log(`보냄 ${JSON.stringify(rows)}`)
            fs.writeFileSync('text.json', JSON.stringify(rows), 'utf-8');
            var filename = 'text.json';
            fs.readFile(filename, function(err, data) {         // 비동기처리
                data = `currUse#${data}`
                for(var key in dictObject) {
                    if(str5[1].trim() == "Android") {   // 안드로이드 시
                        // 바이트 크기
                        var strByteLength = function(s,b,i,c){
                            for(b=i=0;c=s.charCodeAt(i++);b+=c>>11?3:c>>7?2:1);
                            return b
                        }
                        // 안드로이드는 자신한테 보낼 필요 없거나 안드로이드 현재유저를 볼때
                        if(key == str5[3]) {
                            // 자기자신한테만 뿌릴 때[안드로이드로 접속유저를 확인할 때만 2개의 Send]
                            console.log(str5[2].trim() == "AndroidMe")
                            console.log(str5[2].trim())
                            if(str5[2] == "AndroidMe") {
                                console.log("여기옴?")
                                str2 =  dictObject[key].split('#')
                                cip = str2[0]
                                cport = str2[1]
                                console.log(strByteLength(`${data}`) + " Bytes");
                                console.log(`타입 : ${typeof(strByteLength(`${data}`))}`)
                                server.send(strByteLength(`${data}`).toString(), 0, strByteLength(`${data}`).toString().length, cport, cip)
                                console.log(`완료 ${strByteLength(`${data}`).toString()}`)
                                server.send(data, 0, data.length, cport, cip)
                                console.log(`2개 완료`)
                                break
                            }
                            console.log("여기옴?2")
                            continue
                        }
                        console.log("여기옴?4")
                        // 나머지 전부에게는 그냥 보내주면 됨
                        str2 =  dictObject[key].split('#');
                        cip = str2[0]
                        cport = str2[1]
                        var wa = str2[2]
                        if(wa == 'Android') {
                            continue
                        }
                        server.send(data, 0, data.length, cport, cip)


                    } else {                    // Window 시
                        // 일단 안드로이드한테는 안보내야됨[현재상태 띄우는게 아님]
                        str2 =  dictObject[key].split('#')
                        cip = str2[0]
                        cport = str2[1]
                        var wa = str2[2]
                        if(wa == 'Android') {
                            continue
                        }
                        // 바이트 크기
                        var strByteLength = function(s,b,i,c){
                            for(b=i=0;c=s.charCodeAt(i++);b+=c>>11?3:c>>7?2:1);
                            return b
                        }
                        // 나머지 전부한테
                        str2 =  dictObject[key].split('#');
                        cip = str2[0];
                        cport = str2[1];
                        and = str2[2]
                        server.send(data, 0, data.length, cport, cip);
                    }
                }
            });
            console.log("현재접속유저 끝");
        }
    });
  }

  //Chat 신청
  var str6 = message.toString().split('#');

  if((str6[0] == "Chat")) {
    Chatting(str6[1], str6[2]);
  }

  //요청을 받은사람이 수락을 누르면
  var str7 = message.toString().split('#');
  var id = str7[1];
  if(str7[0] == 'accept') {
    //console.log(`${str7[0]}, ${str7[1]}`);
    var cip;
    var cport;
    var str2;
    for(var key in dictObject) {
        console.log(`몇번 돔? ${key}, ${id}`);
        if(key == id) {
            str2 =  dictObject[key].split('#');
            cip = str2[0];
            cport = str2[1];
            //console.log(`why not come ${cip} ${cport}`);
            break;
        }
    }
    data = `acceptM#${str7[2]}`;
    console.log(`accept complete, ${cip}, ${cport} where`);
    server.send(data, 0, data.length, cport, cip);
    //server.send(data, 0, data.length, cport, cip);
    console.log(`보냄`);
  }
  // 채챙보냄
  var str8 = message.toString().split('#');
  if(str8[0] == 'ChatMessage') {
     var str2;
     for(var key in dictObject) {
        console.log(`${key}, ${str8[1]}`);   
        if(key == str8[1]) {
           //console.log('come?');
           str2 = dictObject[key].split('#');
           cip = str2[0];
           cport = str2[1];
           //console.log(`${cip}, ${cport}`); 
        }
     }
     data = `receiveMessage#${str8[2]}`;
     dataE = iconv.encode(data, 'utf-8');
     //console.log(`타입 : ${typeof(data)}`);
     console.log(`${cip}, ${cport}, ${data}`);
     server.send(dataE, 0, dataE.length, cport, cip); // 2개의 스레드때문에 2번 받아야 하나씩 받아져서 처리됨
  }

  // 안드로이드 온도
  var str9 = message.toString().split('#');
  if(str9[0] == 'temp') {
        var sql = `SELECT TEMP_LINE, TEMP_TEMP, TEMP_DATE, TEMP_CONTROL FROM TEMP_INFO`;
        conn.query(sql, function(err, rows, fields) {
            if(err) { console.log('query is not excuted. select fail...\n' + err); }
            else {
                fs.writeFileSync('temp.json', JSON.stringify(rows), 'utf-8');
                var filename = 'temp.json';
                fs.readFile(filename, function(err, data) {
                    var datas = `temp#${data}`.toString('utf8')

                    // 바이트 크기
                    var strByteLength = function(s,b,i,c){
                        for(b=i=0;c=s.charCodeAt(i++);b+=c>>11?3:c>>7?2:1);
                        return b
                    }         
                    console.log(strByteLength(`${datas}`) + " Bytes");
                    //console.log(strByteLength(`한글`) + " Bytes");
                    console.log(`타입 : ${typeof(strByteLength(`${data}`))}`)
                    server.send(strByteLength(`${datas}`).toString(), 0, strByteLength(`${datas}`).toString().length, info.port, info.address)
                    //server.send(t, 0, t.length, info.port, info.address)
                    console.log(`완료 ${strByteLength(`${datas}`).toString()}`)
                    server.send(datas, 0, strByteLength(`${datas}`), info.port, info.address)
                    console.log(`2개 완료`)                       
                    console.log(datas)
                });
            }
        });
  }

  // 안드로이드 습도
  var str10 = message.toString().split('#');
  if(str10[0] == 'humi') {
        var sql = `SELECT HUMI_LINE, HUMI_HUMI, HUMI_DATE, HUMI_CONTROL FROM HUMI_INFO`;
        conn.query(sql, function(err, rows, fields) {
            if(err) { console.log('query is not excuted. select fail...\n' + err); }
            else {
                fs.writeFileSync('humi.json', JSON.stringify(rows), 'utf-8');
                var filename = 'humi.json';
                fs.readFile(filename, function(err, data) {
                    var datas = `humi#${data}`

                    // 바이트 크기
                    var strByteLength = function(s,b,i,c){
                        for(b=i=0;c=s.charCodeAt(i++);b+=c>>11?3:c>>7?2:1);
                        return b
                    }         
                    console.log(strByteLength(`${datas}`) + " Bytes");
                    console.log(`타입 : ${typeof(strByteLength(`${data}`))}`)
                    server.send(strByteLength(`${datas}`).toString(), 0, strByteLength(`${datas}`).toString().length, info.port, info.address)
                    console.log(`완료 ${strByteLength(`${datas}`).toString()}`)
                    server.send(datas, 0, strByteLength(`${datas}`), info.port, info.address)
                    console.log(`2개 완료`)                       
                    console.log(datas)
                });
            }
        });
  }

  // 안드로이드 토양
  var str11 = message.toString().split('#');
  if(str11[0] == 'soil') {
        var sql = `SELECT * FROM LAND_INFO`;
        conn.query(sql, function(err, rows, fields) {
            if(err) { console.log('query is not excuted. select fail...\n' + err); }
            else {
                fs.writeFileSync('land.json', JSON.stringify(rows), 'utf-8');
                var filename = 'land.json';
                fs.readFile(filename, function(err, data) {
                    var datas = `land#${data}`

                    // 바이트 크기
                    var strByteLength = function(s,b,i,c){
                        for(b=i=0;c=s.charCodeAt(i++);b+=c>>11?3:c>>7?2:1);
                        return b
                    }         
                    console.log(strByteLength(`${datas}`) + " Bytes");
                    console.log(`타입 : ${typeof(strByteLength(`${data}`))}`)
                    server.send(strByteLength(`${datas}`).toString(), 0, strByteLength(`${datas}`).toString().length, info.port, info.address)
                    console.log(`완료 ${strByteLength(`${datas}`).toString()}`)
                    server.send(datas, 0, strByteLength(`${datas}`), info.port, info.address)
                    console.log(`2개 완료`)                       
                    console.log(datas)
                });
            }
        });
  }

  //안드로이드 CO2
  var str12 = message.toString().split('#');
  if(str12[0] == 'co2') {
        var sql = `SELECT * FROM CO2_INFO`;
        conn.query(sql, function(err, rows, fields) {
            if(err) { console.log('query is not excuted. select fail...\n' + err); }
            else {
                fs.writeFileSync('co2.json', JSON.stringify(rows), 'utf-8');
                var filename = 'co2.json';
                fs.readFile(filename, function(err, data) {
                    var datas = `co2#${data}`

                    // 바이트 크기
                    var strByteLength = function(s,b,i,c){
                        for(b=i=0;c=s.charCodeAt(i++);b+=c>>11?3:c>>7?2:1);
                        return b
                    }         
                    console.log(strByteLength(`${datas}`) + " Bytes");
                    console.log(`타입 : ${typeof(strByteLength(`${data}`))}`)
                    server.send(strByteLength(`${datas}`).toString(), 0, strByteLength(`${datas}`).toString().length, info.port, info.address)
                    console.log(`완료 ${strByteLength(`${datas}`).toString()}`)
                    server.send(datas, 0, strByteLength(`${datas}`), info.port, info.address)
                    console.log(`2개 완료`)
                    console.log(datas)
                });
            }
        });
  }

  // 안드로이드 양액
  var str13 = message.toString().split('#');
  if(str13[0] == 'nutri') {
        var sql = `SELECT * FROM NUTRI_INFO`;
        conn.query(sql, function(err, rows, fields) {
            if(err) { console.log('query is not excuted. select fail...\n' + err); }
            else {
                fs.writeFileSync('nutri.json', JSON.stringify(rows), 'utf-8');
                var filename = 'nutri.json';
                fs.readFile(filename, function(err, data) {
                    var datas = `nutri#${data}`

                    // 바이트 크기
                    var strByteLength = function(s,b,i,c){
                        for(b=i=0;c=s.charCodeAt(i++);b+=c>>11?3:c>>7?2:1);
                        return b
                    }         
                    console.log(strByteLength(`${datas}`) + " Bytes");
                    console.log(`타입 : ${typeof(strByteLength(`${data}`))}`)
                    server.send(strByteLength(`${datas}`).toString(), 0, strByteLength(`${datas}`).toString().length, info.port, info.address)
                    console.log(`완료 ${strByteLength(`${datas}`).toString()}`)
                    server.send(datas, 0, strByteLength(`${datas}`), info.port, info.address)
                    console.log(`2개 완료`)                       
                    console.log(datas)
                });
            }
        });
  }

  // 안드로이드 게시판 리스트
  var str14 = message.toString().split('#')
  if(str14[0] == 'board') {
        var sql = `SELECT * FROM BOARD_INFO`;
        conn.query(sql, function(err, rows, fields) {
            if(err) { console.log('query is not excuted. select fail...\n' + err); }
            else {
                fs.writeFileSync('board.json', JSON.stringify(rows), 'utf-8');
                var filename = 'board.json';
                fs.readFile(filename, function(err, data) {
                    var datas = `board#${data}`

                    // 바이트 크기
                    var strByteLength = function(s,b,i,c){
                        for(b=i=0;c=s.charCodeAt(i++);b+=c>>11?3:c>>7?2:1);
                        return b
                    }         
                    console.log(strByteLength(`${datas}`) + " Bytes");
                    console.log(`타입 : ${typeof(strByteLength(`${data}`))}`)
                    server.send(strByteLength(`${datas}`).toString(), 0, strByteLength(`${datas}`).toString().length, info.port, info.address)
                    console.log(`완료 ${strByteLength(`${datas}`).toString()}`)
                    server.send(datas, 0, strByteLength(`${datas}`), info.port, info.address)
                    console.log(`2개 완료`)                       
                    console.log(datas)
                });
            }
        });
  }

  // 안드로이드 게시판 삭제
  var str15 = message.toString().split('#')
  if(str15[0] == 'boardDe') {
        console.log(str15[3])
        var sql = `DELETE FROM BOARD_INFO WHERE BOARD_IDX = ${str15[3]}`
        conn.query(sql, function(err, rows, fields) {
            if(err) { console.log('query is not excuted. select fail...\n' + err); }
            else {

            }
        });
  }

});

// --------------------function-------------------------
// 로그인 체크
function successCheck(id, hexpw, pw, cport, cip, aw) {
    var dataE = '';
    var sql = `SELECT COUNT(*) WHERE CAST(AES_DECRYPT(UNHEX('${hexpw}'), 'member') AS CHAR(127) CHARACTER SET UTF8) = '${pw}'`;
    conn.query(sql, function(err, rows, fields) {
        if(err) { console.log('query is not excuted. select fail...\n' + err); }
        else {
            console.log(`1. ${rows[0]['COUNT(*)']}`);
            if(rows[0]['COUNT(*)'] == '0') {
                dataE = 'notPassword';
                server.send(dataE, 0, dataE.length, cport, cip);
            } else {
                dataE = 'collectPassword#';
                if(aw == 'Android') {
                    LoginMemberA(id);
                } else {
                    LoginMemberW(id);
                }   
                dictObject[`${id}`] = cip + "#" + cport + "#" + aw;
                server.send(dataE, 0, dataE.length, cport, cip);
            }
        }
    });
}

// 로그인 중복검사
function loginCheks(id) {
    var sql = `SELECT MEMBER_LOGIN FROM MEMBER_INFO WHERE MEMBER_ID = '${id}'`;
    conn.query(sql, function (err, rows, fields) {
        if(err) { console.log('query is not excuted. select fail...\n' + err); }
        else {
            var re = 2;
            if(rows[0].MEMBER_LOGIN == 'W' || rows[0].MEMBER_LOGIN == 'A') {
                re = 0;
                console.log(`로그인중복 ${rows[0].MEMBER_LOGIN}`);
                return re;
            }else {
                console.log(`로그인중복 ${rows[0].MEMBER_LOGIN}`);
                re = 1;
                return re;
            }
        }
    });
}

// 로그인 상태 'W'
function LoginMemberW(id) {
    var sql = `UPDATE MEMBER_INFO SET MEMBER_LOGIN = 'W' WHERE MEMBER_ID = '${id}'`; 
    conn.query(sql, function (err, rows, fields) {
        if(err) { console.log('query is not excuted. select fail...\n' + err); }
        else {}
    });
}
// 로그인 상태 'A'
function LoginMemberA(id) {
    var sql = `UPDATE MEMBER_INFO SET MEMBER_LOGIN = 'A' WHERE MEMBER_ID = '${id}'`; 
    conn.query(sql, function (err, rows, fields) {
        if(err) { console.log('query is not excuted. select fail...\n' + err); }
        else {}
    });
}

// 로그아웃 상태 'X'
function LogoutMember(id) {
    var sql = `UPDATE MEMBER_INFO SET MEMBER_LOGIN = 'X' WHERE MEMBER_ID = '${id}'`;
    conn.query(sql, function (err, rows, fields) {
        if(err) { console.log('query is not excuted. select fail...\n' + err); }
        else {
            var filename = 'text.json';
            fs.readFile(filename, function(err, data) {
                //server.send(data, 0, data.length, info.port, info.address);
                data = `currUse#${data}`
                console.log('제거 전에');
                for(var key in dictObject) {
                    console.log(`제거전 : ${dictObject[key]}`);
                }
                for(var key in dictObject) {
                    str2 =  dictObject[key].split('#');
                    cip = str2[0];
                    cport = str2[1];
                    data = 'currUse';
                    delete dictObject[`${id}`];
                    //server.send(data, 0, data.length, cport, cip);
                    console.log("제거 완료");
                }
                console.log('제거 후에');
                for(var key in dictObject) {
                    console.log(`제거 후 : ${dictObject[key]}`);
                }
            });
            
        }
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
            //console.log(key == rid);
            str =  dictObject[key].split('#');
            cip = str[0];
            cport = str[1];
            //console.log(dictObject[key]);
            break;
        }
    }
    //console.log(`${cip}, ${cport}, ${str}`);
    sendToChat(sid, rid, cport, cip);
}

// 위에 채팅 요청
function sendToChat(id, rid, port, ip) {
    data = `acc#${id}`;
    //server.send(data, 0, data.length, port, ip);
    server.send(data, 0, data.length, port, ip);
    console.log(`${data}, ${port}, ${ip}`);
}

server.on('close', function() {
	console.log('Close event');
});
 
//udp 데이터를 수신받기 위한 ip와 port를 등록한다.