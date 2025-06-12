using AssertUtils;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using Wc3_Combat_Game.Core;

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

            //EnemySchema enemyData = new();

            //string FilePath = Path.Combine(AppContext.BaseDirectory, "GameData\\EnemyData.json");

            //enemyData = GameDataLoader.LoadSchema(FilePath) ?? throw new InvalidOperationException("Failed to load game schema.");
            
            // Ok now we have the game schema loader, now to make the prototypes.


            game.CreateGameBoard();
            AssertUtil.AssertNotNull(game.Board);
          
            // Setup wave units and counts Old
            game.Board.InitWaves();
            
            //EntityPrototyper.InitEnemies(enemyData ?? throw new InvalidOperationException("enemyData not found in game schema."));
               

            game.CreateGameView();
            AssertUtil.AssertNotNull(game.View);

            game.Board.InitPlayer();
            AssertUtil.AssertNotNull(game.Board.PlayerUnit);
            game.View.RegisterPlayer(game.Board.PlayerUnit);

            game.Board.InitMap(GetDefaultMap(), 32f);

            game.StartGame();

            game.StartTimer();

            Console.WriteLine("Type 'listclasses' to list all classes and properties in the assembly, or press Enter to start the game.");
            string? input = Console.ReadLine();
            if (input?.ToLower() == "listclasses")
            {
                ListClasses(); // Debugging utility to list all classes and properties in the assembly
                return;
            }
            // otherwise continue with the game
            if(game.View != null)
            {
                Application.Run(game.View);
            }
        }
        public static void ListClasses()
        {
            // Build a string for all classes and their properties in the assembly.
            var oString = new StringBuilder();
            // Write to a file in the Debug folder
            string docPath = Path.Combine(AppContext.BaseDirectory, "Debug");
            Directory.CreateDirectory(docPath); // Ensure the directory exists

            var assembly = Assembly.GetExecutingAssembly(); // This is the assembly of the current project
            var allTypes = assembly.GetTypes() // Get all types in the assembly
                .Where(t => t.IsClass && t.IsPublic && t.Namespace != null) // Filter for public classes with a namespace
                .OrderBy(t => t.Namespace) // Order by namespace
                .ThenBy(t => t.Name); // Then by class name

            // Write header
            oString.AppendLine("Namespace\tClass\tBaseType\tInterfaces\tProperty\tPropertyType");

            foreach (var type in allTypes)
            {
                // Get the base type and interfaces
                string baseType = type.BaseType?.Name ?? "";
                string interfaces = string.Join(",", type.GetInterfaces().Select(i => i.FullName ?? i.Name));


                // Get properties of the class, excluding duplicates by name
                
                ConstructorInfo[] ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(c => !c.IsPrivate && !c.IsFamily)
                    .ToArray();
                foreach (ConstructorInfo ctor in ctors)
                {
                    string args = string.Join(", ", ctor.GetParameters()
                    .Select(p => $"{p.ParameterType.Name} {p.Name}"));
                    oString.AppendLine($"{type.Namespace}\t{type.Name}\t{baseType}\t{interfaces}\tConstructor\t({args})");
                }


                PropertyInfo[] props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                    .Where(p => p.GetMethod?.IsPrivate != true && p.GetMethod?.IsFamily != true)
                    .DistinctBy(p => p.Name).ToArray();
                

                if (props.Length == 0)
                {// If there are no properties, just print the class information

                    oString.AppendLine($"{type.Namespace}\t{type.Name}\t{baseType}\t{interfaces}\t\t");
                }
                else
                {
                    foreach (var prop in props)
                    { // For each property, check if it is part of the project assembly
                        bool isProjectType = prop.PropertyType.Assembly == assembly;
                        if (!isProjectType)
                        {
                            // Skip properties that are not part of the project assembly
                            continue;
                        }
                        // Append the class, base type, interfaces, property name, and property type to the string
                        oString.AppendLine($"{type.Namespace}\t{type.Name}\t{baseType}\t{interfaces}\t{prop.Name}\t{prop.PropertyType}");
                    }
                }
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsPrivate && !m.IsSpecialName); // Excludes things like get_/set_

                foreach(var method in methods)
                {
                    string args = string.Join(", ", method.GetParameters()
                .Select(p => $"{p.ParameterType.Name} {p.Name}"));
                    string returnType = method.ReturnType.Name;
                    oString.AppendLine($"{type.Namespace}\t{type.Name}\tMethod\t{type.BaseType?.Name ?? ""}\t{interfaces}\t{method.Name}\t{returnType} ({args})");
                }
            }
            Console.Write(oString.ToString());

            try
            { // Write the output to a file in the Debug folder
                var outputPath = Path.Combine(docPath, "ProgramStructure.txt");
                using var outputFile = new StreamWriter(outputPath);
                outputFile.Write(oString.ToString());
            }
            catch (IOException ex)
            { // Handle any IO exceptions that may occur
                Console.Error.WriteLine($"Error writing file: {ex.Message}");
            }
        }
        private static string[] GetDefaultMap()
        {
            // This is an overly complex map for testing purposes.
            // It has a lot of walls, portals, and shops, and is close to a real game map.

            return [
            "################################",
            "#......_.._.........._.._......#",
            "#......_.._.........._.._......#",
            "#..SS..####..........####..SS..#",
            "#..SS..#PP#..........#PP#..SS..#",
            "#......#..#..........#..#......#",
            "#......#..#..........#..#......#",
            "#__#####__##__####__##__#####__#",
            "#..#P.._................_..P#..#",
            "#..#P.._................_..P#..#",
            "#__#####................#####__#",
            "#......#................#......#",
            "#......_....__####__...._......#",
            "#......_...._......_...._......#",
            "#......#....#......#....#......#",
            "#......#....#......#....#......#",
            "#......#....#......#....#......#",
            "#......#....#......#....#......#",
            "#......_...._......_...._......#",
            "#......_....__####__...._......#",
            "#......#................#......#",
            "#__#####................#####__#",
            "#..#P.._................_..P#..#",
            "#..#P.._................_..P#..#",
            "#__#####__##__####__##__#####__#",
            "#......#..#..........#..#......#",
            "#......#..#..........#..#......#",
            "#..SS..#PP#..........#PP#..SS..#",
            "#..SS..####..........####..SS..#",
            "#......_.._.........._.._......#",
            "#......_.._.........._.._......#",
            "################################"];
        }
    }
}