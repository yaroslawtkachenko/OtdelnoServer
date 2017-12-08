using GamesAPI.Actions;
using GamesAPI.DAO;
using GamesAPI.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace GamesAPI.Sockets
{
    class GamesSocket : WebSocketBehavior
    {
        IGameDAO gameDAO;
        WebSocketServer server;

        public GamesSocket(IGameDAO dao, WebSocketServer server)
        {
            this.server = server;
            gameDAO = dao;
            gameDAO.PlayersUpdated += OnPlayersUpdate;
            gameDAO.GameDeleted += OnGameDelete;
        }

        private void OnPlayersUpdate()
        {
            Broadcast(GameActions.UPDATE_GAMES, gameDAO.All);
        }

        private void OnGameDelete()
        {
            Broadcast(GameActions.UPDATE_GAMES, gameDAO.All);
        }

        private void OnGameRequest()
        {
            SendBack(GameActions.UPDATE_GAMES, gameDAO.All);
        }

        private void OnGameCreate(JToken game)
        {
            Game newGame = new Game(game.ToObject<Game>(), server.Port.ToString());
            gameDAO.Add(newGame);
            GamesAPI.gamesData.Add(newGame.Id, new GameState());
            server.AddWebSocketService("/Game/" + newGame.Id, () => new GameSocket(newGame, gameDAO));
            SendBack(GameActions.GAME_CREATED, newGame);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            JToken token = JObject.Parse(e.Data);
            string actionType = (string)token.SelectToken("type");

            if (actionType == GameActions.CREATE_GAME)
                OnGameCreate(token.SelectToken("data"));
            else if (actionType == GameActions.GET_GAMES)
                OnGameRequest();
        }

        private void Broadcast(string gameAction, object data)
        {
            Sessions.Broadcast(JsonConvert.SerializeObject(new { type = gameAction, data = data }));
        }

        private void SendBack(string gameAction, object data)
        {
            Send(JsonConvert.SerializeObject(new { type = gameAction, data = data }));
        }
    }
}

