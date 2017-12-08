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
using static GamesAPI.GamesAPI;

namespace GamesAPI.Sockets
{
    class GameSocket : WebSocketBehavior
    {
        IGameDAO gameDAO;

        public Game Game { get; set; }

        public List<int[]> winnerIndexes = new List<int[]> { new int[] { 0,1,2}, new int[] { 3, 4, 5 }, new int[] { 6, 7, 8 },
                                                            new int[] { 0,3,6},new int[] { 1,4,7},new int[] { 2,5,8},
                                                            new int[] { 0,4,8},new int[] { 2,4,6} };

        public GameSocket(Game game, IGameDAO gameDAO)
        {
            this.Game = game;
            this.gameDAO = gameDAO;
        }

        protected override void OnOpen()
        {
            gameDAO.AddPlayer(Game, "player");
            Sessions.Broadcast(JsonConvert.SerializeObject(new { type = GameActions.UPDATE_GAME_PLAYERS, data = Game.CurrentPlayers }));
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            JToken token = JObject.Parse(e.Data);
            string actionType = (string)token.SelectToken("type");

            if (actionType == "setupRequest")
            {
                Send(JsonConvert.SerializeObject(new
                {
                    type = "setup",
                    playerData = new
                    {
                        whosTurn = gamesData[Game.Id].players[gamesData[Game.Id].whosNext],
                        playerID = gamesData[Game.Id].players[gamesData[Game.Id].numPlayers++],
                        board = gamesData[Game.Id].board
                    }
                }));
            }
            else if (actionType == "move")
            {
                PlayersMove((string)token.SelectToken("playerID"), (int)token.SelectToken("cellID"));
                UpdateClientState();
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            gameDAO.RemovePlayer(Game, "player");

            if (Game.CurrentPlayers == 0)
                gameDAO.Delete(Game.Id);
            else
                Sessions.Broadcast(JsonConvert.SerializeObject(new { type = GameActions.UPDATE_GAME_PLAYERS, data = Game.CurrentPlayers }));
        }

        public void PlayersMove(string player, int cellID)
        {
            if (player == gamesData[Game.Id].players[gamesData[Game.Id].whosNext])
            {
                gamesData[Game.Id].board[cellID] = (player == "red") ? 1 : -1;

                foreach (var indexes in winnerIndexes)
                {
                    if (gamesData[Game.Id].board[indexes[0]] * gamesData[Game.Id].board[indexes[1]] * gamesData[Game.Id].board[indexes[2]] != 0)
                    {
                        if (gamesData[Game.Id].board[indexes[0]] == gamesData[Game.Id].board[indexes[1]] && gamesData[Game.Id].board[indexes[2]] == gamesData[Game.Id].board[indexes[1]])
                        {
                            gamesData[Game.Id].winnerPresent = true;
                            Sessions.Broadcast(JsonConvert.SerializeObject(new
                            {
                                type = "winner",
                                winner = (gamesData[Game.Id].board[indexes[0]] == 1) ? "Red Player Won!" : "Blue Player Won!"
                            }));
                        }
                    }
                }

                if (gamesData[Game.Id].winnerPresent == false && gamesData[Game.Id].board.Contains(0) == false)
                {
                    gamesData[Game.Id].winnerPresent = true;
                    Sessions.Broadcast(JsonConvert.SerializeObject(new
                    {
                        type = "winner",
                        winner = "Draw!"
                    }));
                }

                if (gamesData[Game.Id].whosNext >= 1)
                    gamesData[Game.Id].whosNext = 0;
                else
                    gamesData[Game.Id].whosNext += 1;
            }
        }

        public void ResetBoard()
        {
            gamesData[Game.Id].whosNext = 0;
            gamesData[Game.Id].board = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        }

        public void UpdateClientState()
        {
            if (gamesData[Game.Id].winnerPresent != true)
            {
                Sessions.Broadcast(JsonConvert.SerializeObject(new
                {
                    type = "update",
                    whosTurn = gamesData[Game.Id].players[gamesData[Game.Id].whosNext],
                    board = gamesData[Game.Id].board
                }));
            }
        }
    }
}