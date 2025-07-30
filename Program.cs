using System.Numerics;
using System.Reflection;
using System.Text;

using AssertUtils;

using nkast.Aether.Physics2D.Dynamics;

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




            Console.WriteLine("Type 'listclasses' to list all classes and properties in the assembly, or press Enter to start the game.");
            string? input = Console.ReadLine();
            input = input?.ToLower();
            switch(input)
            {
                case "listclasses":
                    ListClasses(); // Debugging utility to list all classes and properties in the assembly
                    return;

                default:
                    break;
            }


            // otherwise continue with the game

            game.CreateGameBoard();
            AssertUtil.NotNull(game.Board);

            game.CreateGameView();
            AssertUtil.NotNull(game.View);

            game.Board.InitMap(GetDefaultMap(), 32f);

            game.Board.InitWaves();

            game.Board.InitPlayer();
            AssertUtil.NotNull(game.Board.PlayerUnit);
            game.View.RegisterPlayer(game.Board.PlayerUnit);

            if(game.View != null)
            {


                game.StartGame();

                game.StartTimer();
                Application.Run(game.View);

                // When this finishes, GameOver/Victory. Probably need to handle here.
                // and, Allow restarting the game.
            }

            game.Board.Dispose(); // Cleanup.
            // Might need other cleanup later. Dunno.

        }
        public static void ListClasses()
        {
            var oString = new StringBuilder();
            string docPath = Path.Combine(AppContext.BaseDirectory, "Debug");
            Directory.CreateDirectory(docPath);

            var assembly = Assembly.GetExecutingAssembly();
            var allTypes = assembly.GetTypes()
                .Where(t => t.IsClass && t.IsPublic && t.Namespace != null)
                .OrderBy(t => t.Namespace)
                .ThenBy(t => t.Name);

            oString.AppendLine("--- Assembly Class Structure ---");

            foreach(var type in allTypes)
            {
                // Get the base type
                string baseType = type.BaseType?.Name ?? "Object";
                if(baseType == "Object" && type.IsValueType) baseType = "ValueType"; // For structs/enums

                // Get interfaces, filtering for those within the current assembly
                var projectInterfaces = type.GetInterfaces()
                                            .Where(i => i.Assembly == assembly)
                                            .Select(i => i.Name);
                string interfaces = projectInterfaces.Any() ? string.Join(", ", projectInterfaces) : "None (Custom)";
                if(!projectInterfaces.Any() && type.GetInterfaces().Any())
                {
                    interfaces = "None (Custom), plus external interfaces...";
                }


                oString.AppendLine($"\n## Namespace: {type.Namespace}");
                oString.AppendLine($"### Class: {type.Name}");
                oString.AppendLine($"  - Base Type: {baseType}");
                oString.AppendLine($"  - Interfaces: {interfaces}");

                // Constructors
                ConstructorInfo[] ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(c => !c.IsPrivate && !c.IsFamily)
                    .ToArray();
                if(ctors.Any())
                {
                    oString.AppendLine("  - Constructors:");
                    foreach(ConstructorInfo ctor in ctors)
                    {
                        string args = string.Join(", ", ctor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                        oString.AppendLine($"    - {type.Name}({args})");
                    }
                }

                // Properties
                PropertyInfo[] props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                    .Where(p => (p.GetMethod?.IsPrivate != true && p.GetMethod?.IsFamily != true) || (p.SetMethod?.IsPrivate != true && p.SetMethod?.IsFamily != true))
                    .DistinctBy(p => p.Name).ToArray();

                var projectProperties = props.Where(p => p.PropertyType.Assembly == assembly).ToList();
                var externalProperties = props.Where(p => p.PropertyType.Assembly != assembly).ToList();

                if(projectProperties.Any() || externalProperties.Any())
                {
                    oString.AppendLine("  - Properties:");
                    foreach(var prop in projectProperties)
                    {
                        oString.AppendLine($"    - {prop.Name}: {prop.PropertyType.Name}");
                    }
                    if(externalProperties.Any())
                    {
                        oString.AppendLine($"    - ... (plus {externalProperties.Count} properties with external types)");
                    }
                }

                // Methods
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsPrivate && !m.IsSpecialName && !m.IsFamily); // Excludes things like get_/set_ and private/protected methods

                if(methods.Any())
                {
                    oString.AppendLine("  - Methods:");
                    foreach(var method in methods)
                    {
                        string args = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                        string returnType = method.ReturnType.Name;
                        oString.AppendLine($"    - {method.Name}({args}): {returnType}");
                    }
                }
            }

            Console.Write(oString.ToString());

            try
            {
                var outputPath = Path.Combine(docPath, "ProgramStructure.txt");
                using var outputFile = new StreamWriter(outputPath);
                outputFile.Write(oString.ToString());
                Console.WriteLine($"\nOutput saved to: {outputPath}");
            }
            catch(IOException ex)
            {
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
            "#..SS..####..##..##..####..SS..#",
            "#..SS..#PP#..##..##..#PP#..SS..#",
            "#......#..#..........#..#......#",
            "#......#..#..........#..#......#",
            "#__#####__##__####__##__#####__#",
            "#..#P.._................_..P#..#",
            "#..#P.._................_..P#..#",
            "#__#####..##........##..#####__#",
            "#......#..#..........#..#......#",
            "#......_....__####__...._......#",
            "#..##.._...._......_...._..##..#",
            "#..##..#....#......#....#..##..#",
            "#......#....#..FF..#....#......#",
            "#......#....#..FF..#....#......#",
            "#..##..#....#......#....#..##..#",
            "#..##.._...._......_...._..##..#",
            "#......_....__####__...._......#",
            "#......#..#..........#..#......#",
            "#__#####..##........##..#####__#",
            "#..#P.._................_..P#..#",
            "#..#P.._................_..P#..#",
            "#__#####__##__####__##__#####__#",
            "#......#..#..........#..#......#",
            "#......#..#..##..##..#..#......#",
            "#..SS..#PP#..##..##..#PP#..SS..#",
            "#..SS..####..........####..SS..#",
            "#......_.._.........._.._......#",
            "#......_.._.........._.._......#",
            "################################"];
        }

        private static string[] GetDebugMap()
        {
            return [
            "################################",
            "#..............................#",
            "#__#####..##........##..#####__#",
            "#..#P.._................_..P#..#",
            "#..#P.._................_..P#..#",
            "#__#####__##__####__##__#####__#",
            "#.........#..........#..#......#",
            "#.........#..##..##..#..#......#",
            "#..SS..#PP#..##..##..#PP#..SS..#",
            "#..SS..####..........####..SS..#",
            "#......_.._.........._.._......#",
            "#......_.._.........._.._......#",
            "################################"];

        }
    }
}