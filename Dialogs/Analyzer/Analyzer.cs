// Strategy Analyzer class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Analyzer
{
    public class Analyzer : Form
    {
        Button btnClose;
        ToolStrip tsMainMenu;
        Dialogs.Analyzer.Options pnlOptions;
        Dialogs.Analyzer.OverOptimization pnlOverOptimization;
        Dialogs.Analyzer.Cumulative_Strategy pnlCumulativeStrategy;

        public Form SetParrentForm { set { pnlOptions.SetParrentForm = value; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public Analyzer(string menuItem)
        {
            Text            = Language.T("Strategy Analyzer");
            MaximizeBox     = false;
            Icon            = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton    = btnClose;
            FormClosing    += new FormClosingEventHandler(Actions_FormClosing);

            // Button Close
            btnClose = new Button();
            btnClose.Parent = this;
            btnClose.Text   = Language.T("Close");
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.UseVisualStyleBackColor = true;

            SetupMenuBar();
            SetPanelOptions();
            SetPanelOverOptimization();
            SetPanelCumulativeStrategy();

            ToolStripMenuItem tsMenuItem = new ToolStripMenuItem();
            tsMenuItem.Name = menuItem;
            MainMenu_Click(tsMenuItem, EventArgs.Empty);
        }

        /// <summary>
        /// Sets items in the Main Menu.
        /// </summary>
        void SetupMenuBar()
        {
            tsMainMenu = new ToolStrip();
            tsMainMenu.Parent = this;

            ToolStripMenuItem tsmiAnalysis = new ToolStripMenuItem();
            tsmiAnalysis.Text = Language.T("Analysis");
            tsMainMenu.Items.Add(tsmiAnalysis);

            ToolStripMenuItem tsmiOverOptimization = new ToolStripMenuItem();
            tsmiOverOptimization.Text   = Language.T("Over-optimization Report");
            tsmiOverOptimization.Name   = "tsmiOverOptimization";
            tsmiOverOptimization.Image  = Properties.Resources.overoptimization_chart;
            tsmiOverOptimization.Click += new EventHandler(MainMenu_Click);
            tsmiAnalysis.DropDownItems.Add(tsmiOverOptimization);

            ToolStripMenuItem tsmiCumulativeStrategy = new ToolStripMenuItem();
            tsmiCumulativeStrategy.Text   = Language.T("Cumulative Strategy");
            tsmiCumulativeStrategy.Name   = "tsmiCumulativeStrategy";
            tsmiCumulativeStrategy.Image = Properties.Resources.cumulative_str;
            tsmiCumulativeStrategy.Click += new EventHandler(MainMenu_Click);
            tsmiAnalysis.DropDownItems.Add(tsmiCumulativeStrategy);

            ToolStripMenuItem tsmiTools = new ToolStripMenuItem();
            tsmiTools.Text = Language.T("Tools");
            tsMainMenu.Items.Add(tsmiTools);

            ToolStripMenuItem tsmiOptions = new ToolStripMenuItem();
            tsmiOptions.Text   = Language.T("Options");
            tsmiOptions.Name   = "tsmiOptions";
            tsmiOptions.Image  = Properties.Resources.tools;
            tsmiOptions.Click += new EventHandler(MainMenu_Click);
            tsmiTools.DropDownItems.Add(tsmiOptions);
        }

        void MainMenu_Click(object sender, EventArgs e)
        {
            if (IsSomethingRunning())
            {
                System.Media.SystemSounds.Hand.Play();
                return;
            }

            ToolStripMenuItem button = (ToolStripMenuItem)sender;
            string name = button.Name;

            switch (name)
            {
                case "tsmiOptions":
                    pnlOverOptimization.Visible = false;
                    pnlCumulativeStrategy.Visible = false;
                    pnlOptions.Visible = true;
                    break;
                case "tsmiOverOptimization":
                    pnlOptions.Visible = false;
                    pnlCumulativeStrategy.Visible = false;
                    pnlOverOptimization.Visible = true;
                    break;
                case "tsmiCumulativeStrategy":
                    pnlOptions.Visible = false;
                    pnlOverOptimization.Visible = false;
                    pnlCumulativeStrategy.Visible = true;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            pnlOptions.SetFSBVisiability();

            ClientSize = new Size(500, 400);

            return;
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth  = (int)(Data.HorizontalDLU * 60);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int space        = btnHrzSpace;

            // Button Close
            btnClose.Size = new Size(buttonWidth, buttonHeight);
            btnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // pnlOptions
            pnlOptions.Size = new Size(ClientSize.Width - 2 * space, btnClose.Top - space - btnVertSpace - tsMainMenu.Bottom);
            pnlOptions.Location = new Point(space, tsMainMenu.Bottom + space);

            // pnlOverOptimization
            pnlOverOptimization.Size = pnlOptions.Size;
            pnlOverOptimization.Location = pnlOptions.Location;

            // pnlCumulativeStrategy
            pnlCumulativeStrategy.Size = pnlOptions.Size;
            pnlCumulativeStrategy.Location = pnlOptions.Location;
        }

        /// <summary>
        /// It shows if some proces is running.
        /// </summary>
        /// <returns></returns>
        bool IsSomethingRunning()
        {
            bool isRunning = false;

            if (pnlOverOptimization.IsRunning)
                isRunning = true;

            return isRunning;
        }

        /// <summary>
        /// Analyzer closes
        /// </summary>
        void Actions_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsSomethingRunning())
            {
                System.Media.SystemSounds.Hand.Play();
                e.Cancel = true;
            }
            else
            {
                pnlOptions.ShowFSB();
            }
        }

        void SetPanelOptions()
        {
            pnlOptions = new Dialogs.Analyzer.Options(Language.T("Options"));
            pnlOptions.Parent  = this;
            pnlOptions.Visible = false;
        }

        void SetPanelOverOptimization()
        {
            pnlOverOptimization = new Dialogs.Analyzer.OverOptimization(Language.T("Over-optimization Report"));
            pnlOverOptimization.Parent  = this;
            pnlOverOptimization.Visible = true;
        }

        void SetPanelCumulativeStrategy()
        {
            pnlCumulativeStrategy = new Dialogs.Analyzer.Cumulative_Strategy(Language.T("Cumulative Strategy"));
            pnlCumulativeStrategy.Parent = this;
            pnlCumulativeStrategy.Visible = false;
        }
   }
}
