using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersAPI.Model;

namespace UsersAPI.DAO
{
    public class UserDAO : IUserDAO
    {
        List<User> users = new List<User>();

        public List<User> All => users;

        public void Create(User receivedUser)
        {
            users.Add(receivedUser);
        }

        public void SetUserStatus(int id, bool online)
        {
            users.Find(u => u.Id == id).Online = online;
        }
    }
}
