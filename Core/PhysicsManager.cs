using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using nkast.Aether.Physics2D.Diagnostics;
using nkast.Aether.Physics2D.Dynamics;

using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Core
{
    public class PhysicsManager
    {
        public const Category Unit = Category.Cat1;
        public const Category Projectile = Category.Cat2;
        public const Category Player = Category.Cat3;
        public const Category Enemy = Category.Cat4;
        public const Category Terrain = Category.Cat5;
        public const Category Dead = Category.Cat6;

        public World _world;
        //private DebugView? _debugView;
        //private Form? _myDrawingPanel;

        // create the DebugView
        public PhysicsManager()
        {
            _world = new World(Vector2.Zero);
        }

        //public void RegisterForm(Form form)
        //{
        //    _myDrawingPanel = form;
//#pragma //warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly becaus//e of nullability attributes).
        //    _myDrawingPanel.Paint += MyDrawingPanel_Paint;
//#pragma //warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly becaus//e of nullability attributes).
        //                      // Initialize the debug view
        //    _debugView = new DebugView(_world);
        //    _debugView.AppendFlags(DebugViewFlags.Shape);
        //    _debugView.AppendFlags(DebugViewFlags.Joint);
        //    _debugView.AppendFlags(DebugViewFlags.ContactPoints);
        //    _debugView.AppendFlags(DebugViewFlags.AABB);
        //}

        public void Update(float deltaTime)
        {
            _world.Step(deltaTime);

            //_debugView = new DebugView(_world);

            //_myDrawingPanel?.Invalidate();
        }
        private void MyDrawingPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Apply transformations for scaling and panning (world to screen conversion)
            // This is crucial for matching physics coordinates to screen pixels.
            // For example, if 1 physics unit = 50 pixels, and (0,0) world is center of panel:
            float metersToPixels = 50f;
            float pixelsToMeters = 1f / metersToPixels;

            // Save original graphics state
            System.Drawing.Drawing2D.Matrix oldMatrix = g.Transform;
            g.ResetTransform();

            // Translate origin to center of panel for physics world (0,0)
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            //g.TranslateTransform(_myDrawingPanel.Width / 2f, _myDrawingPanel.Height / 2f);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                              // Flip Y-axis if physics uses Y-up and GDI+ uses Y-down
            g.ScaleTransform(metersToPixels, -metersToPixels);


            // Render your game elements here *using physics world coordinates*
            // For example, iterate your entities and draw their visual components.
            // This is where you would call your entity.Draw(g, context).

            // Then, draw the debug view
            // The DebugView needs to be configured to use these transformations.
            // It often has a SetProjection / SetView method, or you pass it the transform.
            // You'll need to ensure DebugView knows how to draw with GDI+.
            // This might involve creating a custom DebugView class that uses System.Drawing.Graphics.
            // If the official DebugView only supports XNA/MonoGame, you'd need to
            // manually reimplement parts of its drawing logic using Graphics paths/lines/fills.
            // _debugView.Render(); // Example if it knew how to draw directly.
            // Alternatively, you can iterate through the World's bodies and draw
            // their shapes yourself for debugging, using Graphics methods.

            // Restore original graphics state
            g.Transform = oldMatrix;
        }
    }
}