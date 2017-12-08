using GamesAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesAPI.DAO
{
    public class GameDAO : IGameDAO
    {
        public List<Game> All => games;

        List<Game> games = new List<Game>();

        public event Action PlayersUpdated;
        public event Action GameDeleted;

        public void Add(Game game)
        {
            games.Add(game);
        }

        public void Delete(int id)
        {
            games.RemoveAll(g => g.Id == id);
            GameDeleted();
        }

        public void RemovePlayer(Game game, string player)
        {
            games.Find(g => g.Id == game.Id).CurrentPlayers--;
            PlayersUpdated();
        }
        public void AddPlayer(Game game, string player)
        {
            games.Find(g => g.Id == game.Id).CurrentPlayers++;
            PlayersUpdated();
        }
    }
}
