using System.Collections.Generic;
using WebSocketSharp.Server;

namespace GamesAPI.Model
{
    public class Game
    {
        static int count = 0;
        
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string WebSocketUrl { get; set; }
        public int MaxPlayers { get; set; }
        public int CurrentPlayers { get; set; }       

        public Game()
        {

        }

        public Game(Game game, string port)
        {
            Title = game.Title;
            ImageUrl = game.ImageUrl;
            MaxPlayers = game.MaxPlayers;
            CurrentPlayers = game.CurrentPlayers;
            Id = ++count;
            WebSocketUrl = "ws://localhost:"+ port + "/Game/" + Id;
        }
    }
}
