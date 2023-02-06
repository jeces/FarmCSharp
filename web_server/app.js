var express = require('express');
var app = express();
var db_config = require(__dirname + '/config/server1.js');
var conn = db_config.init();
var bodyParser = require('body-parser');
var http = require('http');

db_config.connect(conn);

//html 템플릿 엔진 ejs 설정
app.set('views', __dirname + '/views');
app.set('view engine', 'ejs');
app.set('port',process.env.PORT || 4000);
//bodyparser
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({extended : false}));
/* 이걸 열면 웹이 안열림[url을 지정하지 않고 어떠한 경우라도 실행 됨] -> 바꿔야됨 GET, POST로 지정해줘서
app.use(function(req, res, next) {
    console.log('con');
    var approve ={'approve_id':'NO','approve_pw':'NO'};
    var approve2 ={'approve_id':'OK','approve_pw':'OK'};
    
    var paramId = req.body.id;
    var paramPassword = req.body.password;
    console.log('2) id : '+paramId+'  pw : '+paramPassword);

    var sql = 'SELECT * FROM ROOT';    

    //데이터베이스 연결
    conn.query(sql, function (err, rows, fields) {
        //실패시
        if(err) { console.log('query is not excuted. select fail...\n' + err); }
        //성공시
        else {
            for(var i = 0; i < rows.length; ++i) {
                if(rows[i].rootId == paramId) {
                    console.log(rows[i].rootId);
                    approve.approve_id = "OK";
                    if(rows[i].password == paramPassword) {
                        console.log(rows[i].password);
                        approve.approve_pw = "OK";
                        console.log(approve.approve_id);
                        console.log("Find");
                        res.send("ok");
                        break;
                    }
                }
                else {
                    console.log("Empty");
                }
            }
        }
    });
    /*
    //아이디 일치여부 flag json 데이터입니다.
    if(paramId == 'root') approve.approve_id = 'OK';
    if(paramPassword == 'ntek@123') approve.approve_pw = 'OK';
    
    console.log(approve.approve_id);
});
*/
let us = [
  {
    id: 1,
    name: 'alice'
  },
  {
    id: 2,
    name: 'bek'
  },
  {
    id: 3,
    name: 'chris'
  }
]

//html 웹서버에 get요청이 왔을때 index로 응답
app.get('/', function (req, res) {
    console.log('get');
    res.send('get send');
    //res.render('index.ejs', {data : 'Success'});
});

app.post('/', function (req, res) {
    console.log('post');
    res.send('post send');
});

//html 웹서버에 list요청이 왔을 때 데이터베이스 연결
app.get('/user', function (req, res) {
    var sql = 'SELECT * FROM ROOT';    
    
    //데이터베이스 연결
    conn.query(sql, function (err, rows, fields) {
        //실패시
        if(err) console.log('query is not excuted. select fail...\n' + err);
        //성공시 list.ejs 파일 실행
        else res.render('user.ejs', {list : rows});
    });
});
// html data 요청
app.get('/data', function (req, res) {
    var sql = 'SELECT * FROM LINE';
    conn.query(sql, function (err, rows, fields) {
        if(err) console.log('query is not excuted. select fail...\n' + err);
        else res.render('data.ejs', {list : rows});
    });
});

// 안드로이드 post
app.post('/pushData', function(req, res) {

    res.write("OK");
    var ch = "";
    //데이터 가져오기
    req.on('data', function(data) {
        //데이터를 json으로 파싱
        ch = json.parse(data);
    });
    req.on('end', function() {
        //파싱데이터 확인
        console.log("id : " + ch.rootId);
    });
});

var server = http.createServer(app).listen(app.get('port'),function(){
   console.log("익스프레스로 웹 서버를 실행함 : "+ app.get('port')); 
});

app.listen(3018, () => console.log('Server is running on port...'));