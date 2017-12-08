using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersAPI.Model
{
    public class User
    {
        static int count = 0;

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Online { get; set; }
        public string SessionId { get; set; }

        public User()
        {

        }

        public User(User user)
        {
            Name = user.Name;
            Online = user.Online;
            SessionId = user.SessionId;
            Id = ++count;
        }

        public User(string name, bool online, string sessionId)
        {
            Name = name;
            Online = online;
            SessionId = sessionId;
            Id = ++count;
        }
    }
}
