using System.Diagnostics.CodeAnalysis;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            GameController game = new GameController();

            game.CreateGameBoard();
            AssertUtil.AssertNotNull(game.Board);

            game.CreateGameView();
            AssertUtil.AssertNotNull(game.View);

            game.Board.InitPlayer();
            AssertUtil.AssertNotNull(game.Board.PlayerUnit);

            game.View.RegisterPlayer(game.Board.PlayerUnit);

            game.StartGame();

            game.StartTimer();

            if (game.View != null)
            {
                Application.Run(game.View);
            }
        }
    }
}