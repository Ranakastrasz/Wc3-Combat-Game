using Wc3_Combat_Game.IO;

namespace Wc3_Combat_Game.Components.Drawable
{
    public abstract class DrawableComponent: IDrawable
    {
        protected Color _drawColor;

        public DrawableComponent(Color color)
        {
            _drawColor = color;
        }

        public abstract void Draw(Graphics g, IDrawContext context);
    }
}
