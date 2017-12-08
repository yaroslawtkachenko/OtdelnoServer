using System;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;
using static UsersAPI.Actions.UserActions;
using NUnit.Framework;

namespace UsersTests
{
    [TestFixture]
    public class Tests
    {
        WebSocketServer server;
        UsersAPI.UsersAPI usersAPI = new UsersAPI.UsersAPI();

        [SetUp]
        public void StartServer()
        {
            server = new WebSocketServer("ws://localhost:8888/");
            usersAPI.AttachAPI(server, new UsersAPI.DAO.UserDAO(), "/Users");
            server.Start();
        }

        [TearDown]
        public void ServerStop()
        {
            server.Stop();
        }

        [Test]
        public void TestWebSocketWithServiceIsListening()
        {
            Assert.AreEqual(true, server.IsListening);
        }

        [Test]
        public void TestUsersUpdate()
        {
            var ws = new WebSocket("ws://localhost:8888/Users");

            string getMsg = "";
            ws.OnMessage += (sender, e) => { getMsg = e.Data; };

            ws.Connect();

            Thread.Sleep(1000);

            JToken token = JObject.Parse(getMsg);
            Assert.AreEqual((string)token.SelectToken("type"), UPDATE_USERS);
        }
    }
}
