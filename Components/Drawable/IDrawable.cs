using Wc3_Combat_Game.IO;

namespace Wc3_Combat_Game.Components.Drawable
{
    public interface IDrawable
    {
        public abstract void Draw(Graphics g, IDrawContext context);
    }
}
