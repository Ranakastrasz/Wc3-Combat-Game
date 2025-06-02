using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Wc3_Combat_Game.IO
{
    public class Camera
    {
        public float LerpFactor = 10f; // Higher = faster following

        public Vector2 Position; // World-space top-left
        public float Zoom = 1f;  // Scale factor

        public RectangleF Viewport => new RectangleF(Position.X, Position.Y, Width / Zoom, Height / Zoom);

        public float Width { get; set; }
        public float Height { get; set; }

        public Matrix GetTransform()
        {
            var transform = new Matrix();
            transform.Scale(Zoom, Zoom); // Zoom
            transform.Translate(-Position.X, -Position.Y); // Scroll
            return transform;
        }

        public void UpdateFollow(Vector2 target, float deltaTime)
        {
            Vector2 targetPosition = new Vector2(
                target.X - (Width / 2) / Zoom,
                target.Y - (Height / 2) / Zoom
            );

            Position = Vector2.Lerp(Position, targetPosition, Math.Clamp(LerpFactor * deltaTime, 0, 1));
        }
        public void CenterOn(Vector2 worldPos)
        {
            Position = new Vector2(
                worldPos.X - (Width / 2) / Zoom,
                worldPos.Y - (Height / 2) / Zoom
            );
        }
    }
}
