using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wc3_Combat_Game.Util
{
    internal class FontUtils
    {
        public static Font FitFontToTile(Graphics g, string fontName, float tileSize, FontStyle style = FontStyle.Regular, GraphicsUnit graphicsUnit = GraphicsUnit.Pixel)
        {
            float fontSize = 1;
            Font testFont;
            SizeF size;

            do
            {
                testFont = new Font(fontName, fontSize, style, graphicsUnit);
                size = g.MeasureString("W", testFont);
                if (size.Width > tileSize || size.Height > tileSize)
                    break;
                fontSize += 0.5f;
                testFont.Dispose();
            } while (true);

            return new Font(fontName, fontSize - 0.5f, style, graphicsUnit);
        }
    }
}
