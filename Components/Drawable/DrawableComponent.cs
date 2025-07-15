using System.Numerics;

using Wc3_Combat_Game.IO;

namespace Wc3_Combat_Game.Components.Drawable
{
    public abstract class DrawableComponent: IDrawable
    {
        protected Func<IDrawContext,Color> _getColor;
        protected Func<Vector2> _getPosition;
        protected Func<bool> _getVisible; // New: Store the visibility func

        public DrawableComponent(Func<IDrawContext, Color> getColor, Func<Vector2> getPosition, Func<bool> getVisible)
        {
            _getColor = getColor;
            _getPosition = getPosition;
            _getVisible = getVisible;
        }

        public Func<bool> GetVisible => _getVisible;

        public abstract void Draw(Graphics g, IDrawContext context);

    }
}
