using GamesAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesAPI.DAO
{
    public interface IGameDAO
    {
        List<Game> All { get; }
        void Delete(int id);
        void Add(Game game);

        void AddPlayer(Game game, string player);
        void RemovePlayer(Game game, string player);

        event Action PlayersUpdated;
        event Action GameDeleted;
    }
}
