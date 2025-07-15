using System.Numerics;

using Wc3_Combat_Game.Components.Drawable;
using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Util; // Assuming RectFFromCenter is here

namespace Wc3_Combat_Game.Entities.Components
{
    public class RectangleDrawable: DrawableComponent
    {
        private readonly Func<float> _getDiameter;

        public RectangleDrawable(Func<IDrawContext, Color> getColor, Func<Vector2> getPosition, Func<float> getDiameter, Func<bool> getVisible)
            : base(getColor, getPosition, getVisible)
        {
            _getDiameter = getDiameter;
        }

        public override void Draw(Graphics g, IDrawContext context)
        {
            //if(!context.DebugSettings.Get(DebugSetting.DrawEntities)) // Example of a draw setting
            //{
            //    return;
            //}

            if(!GetVisible()) return;

            Vector2 position = _getPosition();
            float diameter = _getDiameter();
            Vector2 boundingSize = new Vector2(diameter, diameter);
            RectangleF boundingBox = position.RectFFromCenter(boundingSize);

            var brush = context.DrawCache.GetSolidBrush(_getColor(context));
            g.FillRectangle(brush, boundingBox);
        }
    }
}