using AssertUtils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.IO.Load;
using Wc3_Combat_Game.IO.Load.GameSchema;
using Wc3_Combat_Game.Prototype;
using Wc3_Combat_Game.Prototypes;
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

            EnemySchema enemyData = new();

            string FilePath = Path.Combine(AppContext.BaseDirectory, "GameData\\EnemyData.json");

            enemyData = GameDataLoader.LoadSchema(FilePath) ?? throw new InvalidOperationException("Failed to load game schema.");
            
            // Ok now we have the game schema loader, now to make the prototypes.


            game.CreateGameBoard();
            AssertUtil.AssertNotNull(game.Board);
          
            // Setup wave units and counts Old
            game.Board.InitWaves();
            
            EntityPrototyper.InitEnemies(enemyData ?? throw new InvalidOperationException("enemyData not found in game schema."));
               

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