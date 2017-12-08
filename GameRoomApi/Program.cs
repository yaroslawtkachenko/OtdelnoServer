using System;
using System.Collections.Generic;
using System.Linq;
using GamesAPI;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;
namespace GameRoomApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var socket = new WebSocketServer("ws://localhost:8888/");
            GamesAPI.GamesAPI gamesAPI = new GamesAPI.GamesAPI();
            UsersAPI.UsersAPI usersAPI = new UsersAPI.UsersAPI();

            gamesAPI.AttachAPI(socket, new GamesAPI.DAO.GameDAO(), "/Games");
            usersAPI.AttachAPI(socket, new UsersAPI.DAO.UserDAO(), "/Users");

            socket.Start();

            Console.ReadKey();
        }
    }
}
