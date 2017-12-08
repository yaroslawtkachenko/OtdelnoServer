namespace GamesAPI
{
    public class GameState
    {
        public string[] players = new string[] { "red", "blue" };
        public int whosNext = 0;
        public int numPlayers = 0;
        public bool winnerPresent = false;
        public int[] board = new int[] {
                0, 0, 0,
                0, 0, 0,
                0, 0, 0
                };
    }
}