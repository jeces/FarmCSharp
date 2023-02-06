var express = require('express');
var app = express();
var db_config = require(__dirname + '/config/server1.js');
var conn = db_config.init();
var bodyParser = require('body-parser');
var http = require('http');

db_config.connect(conn);

//html ���ø� ���� ejs ����
app.set('views', __dirname + '/views');
app.set('view engine', 'ejs');
app.set('port',process.env.PORT || 4000);
//bodyparser
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({extended : false}));
/* �̰� ���� ���� �ȿ���[url�� �������� �ʰ� ��� ���� ���� ��] -> �ٲ�ߵ� GET, POST�� �������༭
app.use(function(req, res, next) {
    console.log('con');
    var approve ={'approve_id':'NO','approve_pw':'NO'};
    var approve2 ={'approve_id':'OK','approve_pw':'OK'};
    
    var paramId = req.body.id;
    var paramPassword = req.body.password;
    console.log('2) id : '+paramId+'  pw : '+paramPassword);

    var sql = 'SELECT * FROM ROOT';    

    //�����ͺ��̽� ����
    conn.query(sql, function (err, rows, fields) {
        //���н�
        if(err) { console.log('query is not excuted. select fail...\n' + err); }
        //������
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
    //���̵� ��ġ���� flag json �������Դϴ�.
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

//html �������� get��û�� ������ index�� ����
app.get('/', function (req, res) {
    console.log('get');
    res.send('get send');
    //res.render('index.ejs', {data : 'Success'});
});

app.post('/', function (req, res) {
    console.log('post');
    res.send('post send');
});

//html �������� list��û�� ���� �� �����ͺ��̽� ����
app.get('/user', function (req, res) {
    var sql = 'SELECT * FROM ROOT';    
    
    //�����ͺ��̽� ����
    conn.query(sql, function (err, rows, fields) {
        //���н�
        if(err) console.log('query is not excuted. select fail...\n' + err);
        //������ list.ejs ���� ����
        else res.render('user.ejs', {list : rows});
    });
});
// html data ��û
app.get('/data', function (req, res) {
    var sql = 'SELECT * FROM LINE';
    conn.query(sql, function (err, rows, fields) {
        if(err) console.log('query is not excuted. select fail...\n' + err);
        else res.render('data.ejs', {list : rows});
    });
});

// �ȵ���̵� post
app.post('/pushData', function(req, res) {

    res.write("OK");
    var ch = "";
    //������ ��������
    req.on('data', function(data) {
        //�����͸� json���� �Ľ�
        ch = json.parse(data);
    });
    req.on('end', function() {
        //�Ľ̵����� Ȯ��
        console.log("id : " + ch.rootId);
    });
});

var server = http.createServer(app).listen(app.get('port'),function(){
   console.log("�ͽ��������� �� ������ ������ : "+ app.get('port')); 
});

app.listen(3018, () => console.log('Server is running on port...'));