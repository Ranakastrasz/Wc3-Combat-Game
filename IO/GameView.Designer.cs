
namespace Wc3_Combat_Game
{
    partial class GameView
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                DisposeCustomResources();
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            GameWindow = new Panel();
            DebugPanel = new Panel();
            DebugPathfinding = new CheckedListBox();
            DebugPanel.SuspendLayout();
            SuspendLayout();
            // 
            // GameWindow
            // 
            GameWindow.Location = new Point(12, 12);
            GameWindow.Name = "GameWindow";
            GameWindow.Size = new Size(800, 500);
            GameWindow.TabIndex = 0;
            // 
            // DebugPanel
            // 
            DebugPanel.Controls.Add(DebugPathfinding);
            DebugPanel.Location = new Point(818, 12);
            DebugPanel.Name = "DebugPanel";
            DebugPanel.Size = new Size(250, 240);
            DebugPanel.TabIndex = 1;
            // 
            // DebugPathfinding
            // 
            DebugPathfinding.FormattingEnabled = true;
            DebugPathfinding.Location = new Point(3, 3);
            DebugPathfinding.Name = "DebugPathfinding";
            DebugPathfinding.Size = new Size(150, 114);
            DebugPathfinding.TabIndex = 0;
            // 
            // GameView
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1182, 524);
            Controls.Add(DebugPanel);
            Controls.Add(GameWindow);
            Name = "GameView";
            Text = "Wc3 combat Game";
            DebugPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel GameWindow;
        private Panel DebugPanel;
        private CheckedListBox DebugPathfinding;
    }
}
