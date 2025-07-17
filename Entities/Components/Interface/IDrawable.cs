using Wc3_Combat_Game.Core.Context;

namespace Wc3_Combat_Game.Entities.Components.Interface
{
    public interface IDrawable
    {
        public abstract void Draw(Graphics g, IDrawContext context);
    }
}
