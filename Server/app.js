var io = require('socket.io')(1234);
var shortId = require('shortid');
var datasetup = require('./Database/dataSetup');

var clients = [];
var clientLookup = [];

console.log("Start Version " + datasetup.version);

datasetup.connect(function (err_connect) {

    datasetup.loadallUser(function (err, data) {
        
        if (data.length > 0) {
            console.log("Data : %j" , data);
        } else {
            console.log("Data is emty");
        }

    });

   

});

io.on('connection', function (socket) {
    
    

    var currentClient;
    
    socket.on('LoadMap', function () {

        console.log("LoadMap ");
        for (i = 0; i < clients.length; i++) {
            
            
            if (clients[i].data.name === currentClient.data.name) {

            } else {
                console.log(currentClient.data.name + " has LoadMap in : " + clients.length);

                socket.emit('swapAllPlayer', {
                    Name : clients[i].data.name,
                    Id : clients[i].id,
                    Position : clients[i].position,
                    Color : clients[i].color,
                })
            }
            
        }

    });
    
    socket.on('talk', function (data) {
        io.sockets.emit('talk', {
            name : currentClient.data.name,
            messe : data.message,
        });
    });

    socket.on('disconnect', function (){

        socket.broadcast.emit('Playerdisconnected', {
            
            name : currentClient.data.name,
        });
        socket.broadcast.emit('disconnected', currentClient);
        
        var index = clientLookup[currentClient.data.name];
        for (i = 0; i < clients.length; i++) {
            
            if (clients[i].data.name === currentClient.data.name) {
                console.log(clients[i].data.name + " has disconnect");
                io.sockets.emit('talk', {
                    name : currentClient.data.name,
                    messe : " has disconnect",
                });
                datasetup.UpdatePosition(currentClient.data.name, currentClient.position, function (err, dataP) {

                });
                clients.splice(i,1);
            }
            else {
                console.log(clients[i].data.name + " has in Map in");
            }
            

        }
        
    })
    
    socket.on('register', function (data){
        
        if (!data.name || !data.color) {

            socket.emit('registerunsuccess', {
                message : 'register unsuccess'
            });
        }
        else {

            datasetup.addUser(data.name, data.color, function (err, data) {
                if (err) { console.error(err); }
                else {
                    socket.emit('registersuccess', {
                        message : 'register success'
                    });
                }
            });
        }
    })

    socket.on('login', function (data) {
        
        datasetup.searchUser(data.name, function (err, dataS) {
            console.log("user Connected");
            if (err) { console.error(err); }
            else {
                if (dataS[0]) {
                    console.log(data.name);
                    datasetup.loadUser(data.name, function (err, datal) {
                        
                        currentClient = 
                        {
                            id: shortId.generate(),
                            data: data,
                            position: datal[0].Position,
                            color : datal[0].Color,
                        };
                        
                        socket.broadcast.emit('Playerconnected', {

                            name: currentClient.data.name,
                        });
                        console.log(currentClient.data.name + " has connected");
                        
                        clients.push(currentClient);
                        clientLookup[currentClient.id] = currentClient;
                        
                        socket.emit('loginSuccess', {
                            name : datal[0].user_name,
                            id : currentClient.id,
                            position : currentClient.position,
                            color : currentClient.color,
                        });

                        socket.broadcast.emit('swapPlayer', {
                            Name : currentClient.data.name,
                            Id : currentClient.id,
                            Position : currentClient.position,
                            Color : currentClient.color,
                        })

                    });
                }
                else {
                    console.error("Data not found");
                    socket.emit('loginUnsuccess', {
                        message : 'User not found'
                    });

                }
            }
        });

    });
    
    socket.on('UpdatePosition', function (data) { 

        datasetup.UpdatePosition(currentClient.data.name, currentClient.position, function (err, dataP) {

        });

    });

    socket.on('move', function (data){

        currentClient.position = data.position;
        console.log(currentClient.data.name + " move to " + currentClient.position);

        //datasetup.UpdatePosition(currentClient.data.name, currentClient.position, function (err, dataP) {

        //});

        socket.broadcast.emit('PlayerMove', {
            name : currentClient.data.name,
            Position : currentClient.position,
        });
    })
});