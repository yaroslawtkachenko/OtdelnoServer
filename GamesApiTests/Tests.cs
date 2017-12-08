using System;
using System.Collections.Generic;
using System.Threading;
using GamesAPI.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;
using static GamesAPI.Actions.GameActions;
using NUnit.Framework;

namespace GamesApiTests
{
    [TestFixture]
    public class Tests
    {
        WebSocketServer server;
        GamesAPI.GamesAPI gamesAPI = new GamesAPI.GamesAPI();

        [SetUp]
        public void StartServer()
        {
            server = new WebSocketServer("ws://localhost:8888/");
            gamesAPI.AttachAPI(server, new GamesAPI.DAO.GameDAO(), "/Games");
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
        public void TestGamesUpdate()
        {
            var ws = new WebSocket("ws://localhost:8888/Games");

            string getMsg = "";
            ws.OnMessage += (sender, e) => { getMsg = e.Data; };

            ws.Connect();

            ws.Send(JsonConvert.SerializeObject(new { type = GET_GAMES }));

            Thread.Sleep(1000);

            JToken token = JObject.Parse(getMsg);
            Assert.AreEqual((string)token.SelectToken("type"), UPDATE_GAMES);
        }

        [Test]
        public void TestGameCreate()
        {
            var ws = new WebSocket("ws://localhost:8888/Games");

            string getMsg = "";
            ws.OnMessage += (sender, e) => getMsg = e.Data;

            ws.Connect();

            ws.Send(JsonConvert.SerializeObject(new { type = CREATE_GAME, data = new { Title = "GameTitle"} }));

            Thread.Sleep(1000);

            JToken token = JObject.Parse(getMsg);
            Assert.AreEqual((string)token.SelectToken("type"), GAME_CREATED);
        }
    }
}
