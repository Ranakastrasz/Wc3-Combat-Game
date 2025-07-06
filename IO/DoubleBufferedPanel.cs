using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wc3_Combat_Game.IO
{
    /// <summary>
    /// Wrapper to workaround flickering issues with Panel controls by enabling double buffering.
    /// </summary>
    internal class DoubleBufferedPanel: System.Windows.Forms.Panel
    {
        public DoubleBufferedPanel()
        {
            // This sets the internal styles to enable double buffering.
            // UserPaint: Indicates the control will paint itself.
            // AllPaintingInWmPaint: Prevents flickering by ensuring all painting occurs in WM_PAINT.
            // OptimizedDoubleBuffer: Uses an off-screen buffer for painting.
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles(); // Apply the new styles
        }
    }
}
