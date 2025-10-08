

namespace Wc3_Combat_Game.Core.Commands
{
    public static class DebugCommandManager
    {
        private static Thread? _commandThread;
        private static GameController? _controller;
        private static volatile bool _running = false;

        public static void Start(GameController controller)
        {
            _controller = controller;
            _running = true;

            // Start a new thread for continuous console input
            _commandThread = new Thread(CommandLoop)
            {
                IsBackground = true, // Ensures thread dies when application exits
                Name = "ConsoleDebugThread"
            };
            _commandThread.Start();
            Console.WriteLine("\n[DEBUG] Console commands active. Type 'help' or 'pause'.");
        }

        private static void CommandLoop()
        {
            while(_running)
            {
                string? command = Console.ReadLine()?.Trim().ToLower();

                if(string.IsNullOrWhiteSpace(command)) continue;

                if(ExecuteCommand(command))
                {
                    // Command was executed successfully
                }
                else if(command != "help" && command != "exit")
                {
                    Console.WriteLine($"[DEBUG] Unknown command: '{command}'.");
                }
            }
        }

        private static bool ExecuteCommand(string command)
        {
            if(_controller == null) return false;

            if(command == "pause")
            {
                _controller.TogglePause();
                Console.WriteLine($"[DEBUG] Game state toggled: {_controller.CurrentState}");
                return true;
            }
            else if(command.StartsWith("setwave "))
            {
                // Example of a more complex command
                // You'd need a method on GameBoard or WaveManager to handle this.
                // Example: _controller.Board.WaveManager.SetCurrentWave(waveIndex);
                Console.WriteLine("[DEBUG] Wave command not yet fully implemented.");
                return true;
            }
            // Add more commands here (e.g., heal, godmode, printfps)

            return false;
        }

        public static void Stop()
        {
            _running = false;
        }
    }
}