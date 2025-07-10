
using Wc3_Combat_Game.IO;

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
            GameWindow = new DoubleBufferedPanel();
            DebugPanel = new Panel();
            TextBox_DebugOutput = new TextBox();
            DebugPathfinding = new CheckedListBox();
            DebugWaveChart = new DoubleBufferedPanel();
            DebugPanel.SuspendLayout();
            SuspendLayout();
            // 
            // GameWindow
            // 
            GameWindow.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            GameWindow.Location = new Point(12, 12);
            GameWindow.MaximumSize = new Size(1600, 1000);
            GameWindow.Name = "GameWindow";
            GameWindow.Size = new Size(800, 500);
            GameWindow.TabIndex = 0;
            GameWindow.Paint += GameWindow_Paint;
            // 
            // DebugPanel
            // 
            DebugPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            DebugPanel.Controls.Add(TextBox_DebugOutput);
            DebugPanel.Controls.Add(DebugPathfinding);
            DebugPanel.Location = new Point(818, 12);
            DebugPanel.Name = "DebugPanel";
            DebugPanel.Size = new Size(352, 400);
            DebugPanel.TabIndex = 1;
            // 
            // TextBox_DebugOutput
            // 
            TextBox_DebugOutput.Location = new Point(3, 321);
            TextBox_DebugOutput.Multiline = true;
            TextBox_DebugOutput.Name = "TextBox_DebugOutput";
            TextBox_DebugOutput.ReadOnly = true;
            TextBox_DebugOutput.Size = new Size(346, 70);
            TextBox_DebugOutput.TabIndex = 1;
            // 
            // DebugPathfinding
            // 
            DebugPathfinding.FormattingEnabled = true;
            DebugPathfinding.Location = new Point(3, 3);
            DebugPathfinding.Name = "DebugPathfinding";
            DebugPathfinding.Size = new Size(346, 312);
            DebugPathfinding.TabIndex = 0;
            DebugPathfinding.ItemCheck += DebugPathfinding_ItemCheck;
            // 
            // DebugWaveChart
            // 
            DebugWaveChart.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            DebugWaveChart.Location = new Point(821, 418);
            DebugWaveChart.Name = "DebugWaveChart";
            DebugWaveChart.Size = new Size(346, 100);
            DebugWaveChart.TabIndex = 2;
            DebugWaveChart.Paint += DebugWaveChart_Paint;
            // 
            // GameView
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1182, 524);
            Controls.Add(DebugWaveChart);
            Controls.Add(DebugPanel);
            Controls.Add(GameWindow);
            Name = "GameView";
            Text = "Wc3 combat Game";
            DebugPanel.ResumeLayout(false);
            DebugPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Panel DebugPanel;
        private CheckedListBox DebugPathfinding;
        private DoubleBufferedPanel GameWindow;
        private TextBox TextBox_DebugOutput;
        private Panel DebugWaveChart;
    }
}
