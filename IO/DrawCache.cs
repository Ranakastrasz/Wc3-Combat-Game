using System;
using System.Collections.Generic;
using System.Drawing; // Make sure this is included for Color and SolidBrush
using System.Linq; // For .Values, though not strictly needed for this loop
using System.Text;
using System.Threading.Tasks;

namespace Wc3_Combat_Game.IO
{
    public class DrawCache: IDisposable
    {
        private Dictionary<Color, SolidBrush> _solidBrushes = new Dictionary<Color, SolidBrush>();
        private bool disposedValue; // Flag to track if Dispose has been called

        // This method is fine internally, but we'll call it conditionally
        private void DisposeInternal() // Renamed for clarity, not strictly necessary
        {
            foreach(var brush in _solidBrushes.Values)
            {
                brush.Dispose();
            }
            _solidBrushes.Clear();
        }

        public SolidBrush GetOrCreateBrush(Color color)
        {
            // Optional: You could throw an ObjectDisposedException here
            // if you want to prevent use after disposal.
            // if (disposedValue) throw new ObjectDisposedException(nameof(DrawCache));

            if(!_solidBrushes.TryGetValue(color, out SolidBrush? brush))
            {
                brush = new SolidBrush(color);
                _solidBrushes.Add(color, brush);
            }
            return brush;
        }

        public void Dispose()
        {
            // Implement the dispose pattern directly here for simplicity,
            // or use the common protected virtual Dispose(bool disposing) pattern
            // for more complex scenarios involving inheritance and unmanaged resources.

            if(!disposedValue) // Only run disposal logic if not already disposed
            {
                DisposeInternal(); // Call your actual disposal logic

                disposedValue = true; // Mark as disposed
                GC.SuppressFinalize(this); // Tell the GC that finalization is not needed
                                           // (even if you don't have a finalizer, this is good practice for IDisposable)
            }
        }
    }
}