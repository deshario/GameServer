function datasetup() {

    this.version = '0.0.1';
    var db = null;
    var mysql = require('mysql');
    var config = {
        host : '127.0.0.1',
        user : 'root',
        passwoed : '',
        database : 'mydb',
    }

    this.connect = function (callback){

        db = mysql.createConnection(config);
        db.connect(function (err) {
            
            if (err) {
                console.error('error connecting myslq :' + err);
                return;
            }
            
            console.log('Connected as database ' + config.database);

            callback(err);
        });

    };
    
    this.addUser = function (user, color, callback) {

        db.query("INSERT INTO user ( `user_name`, `Position`, `Color`) VALUES (?,'null',?)",[user,color], function (err, data) {
            if (err) { console.error(err); }
            
            callback(err, data);
        });

    };

    this.loadallUser = function (callback) {

        var sql = 'select * from user';

        db.query(sql, function (err, data) {
            if (err) { console.error(err); }

            callback(err, data);
        });
    };


    this.searchUser = function (user, callback) {
        db.query('SELECT user_name FROM user WHERE user_name like ?','%' + user + '%', function(err, data) {

            if (err) { console.error(err); }

            callback(err, data);
        });

    };

    this.loadUser = function (user, callback) {

        db.query('SELECT * FROM user WHERE user_name = ?',[user], function (err, data) {

            if(err){ console.error(err);}

            callback(err, data);
        });

    };

    this.UpdatePosition = function (user, value, callback) {
        
        var dataUpdate = {

            Position : value,
        }
        
        db.query("UPDATE user set ? WHERE user_name = ? ",[dataUpdate,user], function (err, data) {
            
            if (err) { console.error(err); }
            
            callback(err, data);
        });

    };


}

module.exports = new datasetup;

