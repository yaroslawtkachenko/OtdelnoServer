using GamesAPI;
using GamesAPI.DAO;
using GamesAPI.Model;
using GamesAPI.Sockets;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace GamesAPI
{
    public class GamesAPI
    {
        public static Dictionary<int, GameState> gamesData = new Dictionary<int, GameState>();
        public void AttachAPI(WebSocketServer server, IGameDAO gameDAO, string routePath)
        {
            server.AddWebSocketService(routePath, () => new GamesSocket(gameDAO, server));
        }
    }
}
