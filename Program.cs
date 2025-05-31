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

            //game.View = new GameView(game);
            game.CreateSession();
            
            game.SyncDrawables();


            game.StartGame();

            game.StartTimer();
            Application.Run(game.View);
        }
    }
}