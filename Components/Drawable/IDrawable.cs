using Wc3_Combat_Game.IO;

namespace Wc3_Combat_Game.Components.Drawable
{
    internal interface IDrawable
    {
        protected Func<bool> GetVisible { get; }


        public abstract void Draw(Graphics g, IDrawContext context);
        //public abstract void DrawDebug(Graphics g, IDrawContext context);
    }
}
