
namespace Wc3_Combat_Game.IO
{
    // Define a struct for Pen keys
    public readonly struct PenKey: IEquatable<PenKey>
    {
        public readonly Color Color;
        public readonly float Width;

        public PenKey(Color color, float width)
        {
            Color = color;
            Width = width;
        }

        public override bool Equals(object? obj) => obj is PenKey other && Equals(other);
        public bool Equals(PenKey other) => Color.Equals(other.Color) && Width.Equals(other.Width);

        public override int GetHashCode() => HashCode.Combine(Color, Width);
    }

    // Define a struct for Font keys
    public readonly struct FontKey: IEquatable<FontKey>
    {
        public readonly string Family;
        public readonly float EmSize;
        public readonly FontStyle Style; // Include FontStyle in the key

        public FontKey(string family, float emSize, FontStyle style)
        {
            Family = family;
            EmSize = emSize;
            Style = style;
        }

        public override bool Equals(object? obj) => obj is FontKey other && Equals(other);
        public bool Equals(FontKey other) =>
            Family.Equals(other.Family, StringComparison.OrdinalIgnoreCase) && // Case-insensitive family name
            Math.Abs(EmSize - other.EmSize) < 0.001f && // Use epsilon for float comparison
            Style == other.Style;

        public override int GetHashCode() => HashCode.Combine(Family.GetHashCode(StringComparison.OrdinalIgnoreCase), EmSize, Style);
    }

    public class DrawCache: IDisposable
    {
        private Dictionary<Color, SolidBrush> _solidBrushes = new Dictionary<Color, SolidBrush>();
        private Dictionary<PenKey, Pen> _pens = new();
        private Dictionary<FontKey, Font> _fonts = new();
        private bool disposedValue; // Flag to track if Dispose has been called)

        private readonly object _lock = new object();

        public SolidBrush GetSolidBrush(Color key)
        {
            lock(_lock)
            {
                if(!_solidBrushes.TryGetValue(key, out SolidBrush? brush))
                {
                    brush = new SolidBrush(key);
                    _solidBrushes.Add(key, brush);
                }
                return brush;
            }
        }

        public Pen GetPen(Color color, float width = 1)
        {
            PenKey key = new PenKey(color, width); // Struct allocation (on stack/inline)
            lock(_lock)
            {
                if(!_pens.TryGetValue(key, out Pen? pen))
                {
                    pen = new Pen(color, width);
                    _pens.Add(key, pen);
                }
                return pen;
            }
        }

        public Font GetFont(string family, float emSize, FontStyle style = FontStyle.Regular)
        {
            FontKey key = new FontKey(family, emSize, style); // Struct allocation (on stack/inline)
            lock(_lock)
            {
                if(!_fonts.TryGetValue(key, out Font? font))
                {
                    font = new Font(family, emSize, style);
                    _fonts.Add(key, font);
                }
                return font;
            }
        }

        private void DisposeInternal()
        {
            lock(_lock)
            {
                foreach(var brush in _solidBrushes.Values)
                {
                    brush.Dispose();
                }
                _solidBrushes.Clear();

                foreach(var pen in _pens.Values)
                {
                    pen.Dispose();
                }
                _pens.Clear();

                foreach(var font in _fonts.Values)
                {
                    font.Dispose();
                }
                _fonts.Clear();
            }
        }

        public void Dispose()
        {
            if(!disposedValue)
            {
                DisposeInternal();
                disposedValue = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}