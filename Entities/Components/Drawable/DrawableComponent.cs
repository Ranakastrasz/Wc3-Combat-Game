using System.Numerics;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Interface;

namespace Wc3_Combat_Game.Entities.Components.Drawable
{
    public abstract class DrawableComponent: IDrawable
    {
        protected Func<bool> _getVisible { get; }
        protected Func<IDrawContext,Color> _getColor;
        protected Func<Vector2> _getPosition;

        public DrawableComponent(Func<IDrawContext, Color> getColor, Func<Vector2> getPosition, Func<bool> getVisible)
        {
            _getColor = getColor;
            _getPosition = getPosition;
            _getVisible = getVisible;
        }

        public abstract void Draw(Graphics g, IDrawContext context);

    }
}
