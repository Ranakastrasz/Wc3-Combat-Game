//using System.IO;
//using System.Numerics; // Using System.Numerics.Vector2 for Aether.Physics2D
//using System.Xml.Linq;
//
//using nkast.Aether.Physics2D.Collision;
//using nkast.Aether.Physics2D.Dynamics;
//using nkast.Aether.Physics2D.Dynamics.Contacts;
//
//using Timer = System.Windows.Forms.Timer;
//
//public class CollisionDebugger
//{
//    private XDocument _logDoc;
//    private XElement _collisionsRoot;
//    private string _logFilePath;
//    private int _simulationStep = 0;
//
//    public CollisionDebugger(string logFilePath)
//    {
//        _logFilePath = logFilePath;
//        _logDoc = new XDocument(new XElement("CollisionLog"));
//        _collisionsRoot = _logDoc.Root; // The root element where we'll add collision entries
//    }
//
//    // Call this at the start of each simulation step
//    public void BeginSimulationStep()
//    {
//        _simulationStep++;
//    }
//
//    // Attach this method to Fixture.OnCollision
//    public bool OnFixtureCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
//    {
//        // Log collision start
//        LogCollisionEvent("CollisionStart", fixtureA, fixtureB, contact);
//        // Return true to allow the collision to proceed, false to ignore it
//        return true;
//    }
//
//    // Attach this method to Fixture.OnSeparation
//    public void OnFixtureSeparation(Fixture fixtureA, Fixture fixtureB, Contact contact)
//    {
//        // Log collision end
//        LogCollisionEvent("CollisionEnd", fixtureA, fixtureB, contact);
//    }
//
//    private void LogCollisionEvent(string eventType, Fixture fixtureA, Fixture fixtureB, Contact contact)
//    {
//        WorldManifold manifold;
//        contact.GetWorldManifold(out manifold);
//
//        // Get contact points (can be one or two)
//        XElement contactPoints = new XElement("ContactPoints");
//        for(int i = 0; i < manifold.Points.Count; i++)
//        {
//            Vector2 point = manifold.Points[i];
//            contactPoints.Add(new XElement("Point",
//                new XAttribute("X", point.X),
//                new XAttribute("Y", point.Y)));
//        }
//
//        XElement entry = new XElement("CollisionEvent",
//            new XAttribute("Step", _simulationStep),
//            new XAttribute("Type", eventType),
//            new XElement("FixtureA",
//                new XAttribute("Name", fixtureA.Tag?.ToString() ?? "Unnamed"),
//                //new XAttribute("BodyId", fixtureA.Body.BodyId),
//                new XAttribute("ShapeType", fixtureA.Shape.ShapeType.ToString())),
//            new XElement("FixtureB",
//                new XAttribute("Name", fixtureB.Tag?.ToString() ?? "Unnamed"),
//                //new XAttribute("BodyId", fixtureB.Body.BodyId),
//                new XAttribute("ShapeType", fixtureB.Shape.ShapeType.ToString())),
//            new XElement("Normal",
//                new XAttribute("X", manifold.Normal.X),
//                new XAttribute("Y", manifold.Normal.Y)),
//            contactPoints,
//            new XElement("IsSensor",
//                new XAttribute("FixtureA", fixtureA.IsSensor),
//                new XAttribute("FixtureB", fixtureB.IsSensor))
//            // You can add more attributes here, like restitution, friction, categories etc.
//            // For example:
//            // new XElement("CollisionCategories",
//            //     new XAttribute("FixtureA", fixtureA.CollisionCategories.ToString()),
//            //     new XAttribute("FixtureB", fixtureB.CollisionCategories.ToString()))
//        );
//
//        _collisionsRoot.Add(entry);
//    }
//
//    // Call this when your application exits or when you want to save the log
//    public void SaveLog()
//    {
//        _logDoc.Save(_logFilePath);
//        Console.WriteLine($"Collision log saved to: {_logFilePath}");
//    }
//}
//
//// Example Usage in your Windows Forms app:
//public partial class MyPhysicsForm: Form
//{
//    private World _world;
//    private CollisionDebugger _debugger;
//    private System.Windows.Forms.Timer _gameLoopTimer; // Or whatever your update mechanism is
//
//    public MyPhysicsForm()
//    {
//        InitializeComponent();
//
//        _world = new World(new Vector2(0, -9.8f)); // Your physics world
//        _debugger = new CollisionDebugger("collision_log.xml");
//
//        // Example: Create some bodies and fixtures
//        Body ground = _world.CreateBody();
//        ground.CreateEdge(new Vector2(-10, 0), new Vector2(10, 0));
//        ground.Tag = "Ground"; // Assign UserData for easier identification in log
//
//        Body box = _world.CreateBody(new Vector2(0, 5), 0, BodyType.Dynamic);
//        Fixture boxFixture = box.CreateRectangle(1, 1, 1, Vector2.Zero);
//        boxFixture.Tag = "FallingBox"; // Assign UserData
//
//        // Attach the debugger's event handler to ALL fixtures you care about
//        // You can iterate through all fixtures or attach as you create them
//        boxFixture.OnCollision += _debugger.OnFixtureCollision;
//        boxFixture.OnSeparation += _debugger.OnFixtureSeparation;
//        ground.FixtureList[0].OnCollision += _debugger.OnFixtureCollision;
//        ground.FixtureList[0].OnSeparation += _debugger.OnFixtureSeparation;
//
//
//        // Setup your game loop/timer
//        _gameLoopTimer = new Timer();
//        _gameLoopTimer.Interval = 16; // Approx 60 FPS
//        _gameLoopTimer.Tick += GameLoop_Tick;
//        _gameLoopTimer.Start();
//
//        // Ensure log is saved on form close
//        this.FormClosed += (s, e) => _debugger.SaveLog();
//    }
//
//    private void GameLoop_Tick(object sender, EventArgs e)
//    {
//        _debugger.BeginSimulationStep();
//        _world.Step(Math.Min((float)_gameLoopTimer.Interval / 1000f, (1f / 30f))); // Max step 30 FPS
//        // Your other drawing/update logic here
//    }
//}