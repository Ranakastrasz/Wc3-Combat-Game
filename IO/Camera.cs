using System.Drawing.Drawing2D;
using System.Numerics;

using Wc3_Combat_Game.Entities.EntityTypes;

namespace Wc3_Combat_Game.IO
{
    public class Camera
    {
        public float LerpFactor = 10f; // Higher = faster following

        public Vector2 Position; // World-space top-left
        public float Zoom = 1f;  // Scale factor
        public Unit? FollowTarget { get; private set; }

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

        public void Update(float deltaTime)
        {
            if(FollowTarget != null)
            {
                Vector2 targetPosition = GetPositionCentered(FollowTarget.Position);

                Position = Vector2.Lerp(Position, targetPosition, Math.Clamp(LerpFactor * deltaTime, 0, 1));
            }
        }
        public void FollowUnit(Unit unit)
        {
            FollowTarget = unit;
        }

        public void SnapToUnit(Unit? unit = null)
        {
            Vector2 newPosition;
            if(unit == null)
            {
                if(FollowTarget == null) return;
                newPosition = FollowTarget.Position;
            }
            else
            {
                newPosition = unit.Position;
            }
            SetPositionCentered(newPosition);
        }
        public Vector2 GetPositionCentered(Vector2 position)
        {
            return new Vector2(
                    position.X - (Width / 2) / Zoom,
                    position.Y - (Height / 2) / Zoom
                );
        }
        public void SetPositionCentered(Vector2 position)
        {
            Position = GetPositionCentered(position);
        }

        public Vector2 WorldPointToScreenPoint(Vector2 worldPoint)
        {
            return (worldPoint - Position) * Zoom;
        }

        public Vector2 ScreenPointToWorldPoint(Vector2 screenPoint)
        {
            return (screenPoint / Zoom) + Position;
        }
    }
}
