namespace Wc3_Combat_Game.IO.WindowView
{
    /// <summary>
    /// Wrapper to workaround flickering issues with Panel controls by enabling double buffering.
    /// </summary>
    public class DoubleBufferedPanel: Panel
    {
        public DoubleBufferedPanel()
        {
            // This sets the public styles to enable double buffering.
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
