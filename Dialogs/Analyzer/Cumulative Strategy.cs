// Strategy Analyzer - Cumulative_Strategy
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Analyzer
{
    class Cumulative_Strategy : Fancy_Panel
    {
        Button btnAddStrategy;
        Button btnResetReport;
        Button btnOpenFolder;
        Button btnOpenReport;
        //string pathReportFile;

        public Cumulative_Strategy(string caption) : base(caption)
        {
            btnAddStrategy  = new Button();
            btnResetReport  = new Button();
            btnOpenFolder   = new Button();
            btnOpenReport   = new Button();

            // Button Add Strategy
            btnAddStrategy.Parent = this;
            btnAddStrategy.Text   = Language.T("Add Strategy");
            btnAddStrategy.Click += new EventHandler(BtnStart_Click);
            btnAddStrategy.UseVisualStyleBackColor = true;

            // Button Reset Report
            btnResetReport.Parent     = this;
            btnResetReport.Name       = "btnResetReport";
            btnResetReport.ImageAlign = ContentAlignment.MiddleCenter; 
            btnResetReport.Image      = Properties.Resources.close_button;
            btnResetReport.Click     += new EventHandler(ViewCharts_Click);
            btnResetReport.UseVisualStyleBackColor = true;

            // Button Open report folder
            btnOpenFolder.Parent     = this;
            btnOpenFolder.Name       = "btnOpenFolder";
            btnOpenFolder.Text       = Language.T("Open Folder");
            btnOpenFolder.ImageAlign = ContentAlignment.MiddleLeft;
            btnOpenFolder.Image      = Properties.Resources.folder_open;
            btnOpenFolder.Enabled    = false;
            btnOpenFolder.Click     += new EventHandler(OpenFolder_Click);
            btnOpenFolder.UseVisualStyleBackColor = true;

            // Button Open Report
            btnOpenReport.Parent     = this;
            btnOpenReport.Name       = "btnOpenReport";
            btnOpenReport.Text       = Language.T("Open Report");
            btnOpenReport.ImageAlign = ContentAlignment.MiddleLeft;
            btnOpenReport.Image      = Properties.Resources.export;
            btnOpenReport.Enabled    = false;
            btnOpenReport.Click     += new EventHandler(OpenReport_Click);
            btnOpenReport.UseVisualStyleBackColor = true;

            this.Resize += new EventHandler(PnlCumulativeStrategy_Resize);

        }

        void PnlCumulativeStrategy_Resize(object sender, EventArgs e)
        {
            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth  = (int)(Data.HorizontalDLU * 60);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int space  = btnHrzSpace;
            int border = 2;

            // Button Add Strategy.
            btnAddStrategy.Size     = new Size(buttonWidth, buttonHeight);
            btnAddStrategy.Location = new Point(border + btnHrzSpace, 18 + btnVertSpace);

            btnResetReport.Size     = new Size(buttonWidth / 2, buttonHeight);
            btnResetReport.Location = new Point(border + btnHrzSpace, btnAddStrategy.Bottom + btnVertSpace);

            int btnWideWidth = (int)((this.ClientSize.Width - 2 * border - 4 * btnHrzSpace) / 3);
            btnOpenFolder.Size = new Size(btnWideWidth, buttonHeight);
            btnOpenReport.Size = btnOpenFolder.Size;

            btnOpenFolder.Location = new Point(border + btnHrzSpace + btnWideWidth + btnHrzSpace, this.Height - buttonHeight - btnVertSpace);
            btnOpenReport.Location = new Point(btnOpenFolder.Right + btnHrzSpace, btnOpenFolder.Top);

        }


        /// <summary>
        /// Adds the strategy to the cumulative report.
        /// </summary>
        void BtnStart_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Opens charts screan.
        /// </summary>
        void ViewCharts_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Opens the report folder.
        /// </summary>
        void OpenFolder_Click(object sender, EventArgs e)
        {
            try { System.Diagnostics.Process.Start(Data.StrategyDir); }
            catch (System.Exception ex) { MessageBox.Show(ex.Message); }
        }

        /// <summary>
        /// Opens the report.
        /// </summary>
        void OpenReport_Click(object sender, EventArgs e)
        {
            //try { System.Diagnostics.Process.Start(pathReportFile); }
            //catch (System.Exception ex) { MessageBox.Show(ex.Message); }
        }

    }
}
