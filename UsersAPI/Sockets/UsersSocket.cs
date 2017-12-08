using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersAPI.Actions;
using UsersAPI.DAO;
using UsersAPI.Model;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace UsersAPI.Sockets
{
    class UsersSocket : WebSocketBehavior
    {
        static Random rng = new Random();
        IUserDAO userDAO;

        public UsersSocket(IUserDAO userDAO)
        {
            this.userDAO = userDAO;
        }

        protected override void OnOpen()
        {
            userDAO.Create(new User("Player " + rng.Next(1, 9999), true, Sessions.IDs.LastOrDefault()));
            BroadcastOnlineUsers();
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            JToken token = JObject.Parse(e.Data);
            string actionType = (string)token.SelectToken("type");

            if (actionType == UserActions.GET_ONLINE_USERS)
                SendOnlineUsers();

        }
        protected override void OnClose(CloseEventArgs e)
        {
            List<User> offlineUsers = new List<User>();
            foreach (User user in userDAO.All)
            {
                if (Sessions.IDs.Contains(user.SessionId) != true)
                {
                    offlineUsers.Add(user);
                    userDAO.SetUserStatus(user.Id, false);
                }
            }
            BroadcastOnlineUsers();
        }

        private void SendOnlineUsers()
        {
            Send(JsonConvert.SerializeObject(new { type = UserActions.UPDATE_USERS, data = userDAO.All.Where(p => p.Online) }));
        }
        private void BroadcastOnlineUsers()
        {
            Sessions.Broadcast(JsonConvert.SerializeObject(new { type = UserActions.UPDATE_USERS, data = userDAO.All.Where(p => p.Online) }));
        }
    }
}
