using Wc3_Combat_Game.IO;

namespace Wc3_Combat_Game.Components.Drawable
{
    internal interface IDrawable
    {
        // Draw, Draw debug, maybe prototype, or prototype is from it's owner.
        // Dunno what else.


        public abstract void Draw(Graphics g, IDrawContext context);
        //public abstract void DrawDebug(Graphics g, IDrawContext context);
    }
}
