PORT = 52300;
const io = require('socket.io')(process.env.PORT || PORT);

/*------------------------*/
/*---- Custom Classes ----*/
/*------------------------*/

const Player = require("./Classes/Player.js");

const players = [];
const sockets = [];

console.log(`Server has started on http://localhost:${PORT}`);

io.on('connection', function (socket) {
    console.log("Connection made!");

    const player = new Player();
    const thisPlayerID = player.id;

    players[thisPlayerID] = player;          //Saving reference to current player.
    console.log(players);
    console.log("players^^");
    sockets[thisPlayerID] = socket;         //Saving reference to current socket.

    //Tell the client that this is our id for the server.
    socket.emit("register", {id: thisPlayerID});
    socket.emit("spawn", player);       //Tell this that it has spawned.
    socket.broadcast.emit("spawn", player);     //Tell others that this has spawned.

    //Tell this about other players.
    for(const playerID in players){
        if(playerID != thisPlayerID){
            socket.emit("spawn", players[playerID]);
        }
    }

    socket.on("updatePosition", function(pUnityData) {
        player.position.x = pUnityData.position.x;
        player.position.y = pUnityData.position.y;
        player.position.z = pUnityData.position.z;

        // const rData = {
        //     position = {
        //         x = player.position.x,
        //         y = player.position.y,
        //         z = player.position.z
        //     }
        // }

        console.log(`player, ${player.id}, position is...`);
        console.log(player.position);
        console.log("...................................")

        socket.broadcast.emit("updatePosition", player);
    });

    socket.on("disconnect", function() {
        console.log(`Player, ${player.id}, has disconnected`);
        delete players[thisPlayerID];
        delete sockets[thisPlayerID];
        socket.broadcast.emit("disconnected", player);
    });
});