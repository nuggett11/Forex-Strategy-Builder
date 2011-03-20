// Workspace form.
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// This is the base application form.
    /// </summary>
    public class Workspace : Form
    {
        protected Panel pnlWorkspace;
        private   Panel pnlDataBase;
        private   Panel pnlJournalBase;
        private   Panel pnlMarketBase;
        private   Panel pnlStrategyBase;
        private   Panel pnlAccountBase;
        protected Panel pnlMarket;
        protected Panel pnlStrategy;
        protected Panel pnlAccount;
        protected Panel pnlJournal;
        protected ToolStrip tsMarket;
        protected ToolStrip tsStrategy;
        protected ToolStrip tsAccount;

        protected StatusStrip statusStrip;
        protected ToolTip toolTip;

        Splitter splitHoriz;

        protected int space = 4;

        /// <summary>
        /// The dafault constructor
        /// Sets the base panels
        /// </summary>
        public Workspace()
        {
            // Graphical measures
            Graphics g = CreateGraphics();
            SizeF sizeString   = g.MeasureString("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890", Font);
            Data.HorizontalDLU = (sizeString.Width / 62) / 4;
            Data.VerticalDLU   = sizeString.Height / 8;
            g.Dispose();

            toolTip = new ToolTip();

            MainMenuStrip   = new MenuStrip();
            pnlWorkspace    = new Panel();
            statusStrip     = new StatusStrip();

            pnlDataBase     = new Panel();

            pnlMarketBase   = new Panel();
            tsMarket        = new ToolStrip();
            pnlMarket       = new Panel();

            pnlStrategyBase = new Panel();
            tsStrategy      = new ToolStrip();
            pnlStrategy     = new Panel();

            pnlAccountBase  = new Panel();
            tsAccount       = new ToolStrip();
            pnlAccount      = new Panel();

            pnlJournalBase  = new Panel();
            pnlJournal      = new Panel();

            splitHoriz = new Splitter();
            Splitter splitVert1 = new Splitter();
            Splitter splitVert2 = new Splitter();

            // Panel Workspace
            pnlWorkspace.Parent    = this;
            pnlWorkspace.Dock      = DockStyle.Fill;
            pnlWorkspace.BackColor = LayoutColors.ColorFormBack;

            // Main menu
            MainMenuStrip.Parent = this;
            MainMenuStrip.Dock   = DockStyle.Top;

            // Status bar
            statusStrip.Parent = this;
            statusStrip.Dock   = DockStyle.Bottom;

            // Panel Journal Base
            pnlJournalBase.Parent  = pnlWorkspace;
            pnlJournalBase.Dock    = DockStyle.Fill;
            pnlJournalBase.Padding = new Padding(space, 0, space, space);

            // Horizontal splitter
            splitHoriz.Parent = pnlWorkspace;
            splitHoriz.Dock   = DockStyle.Top;
            splitHoriz.Height = space;

            // Panel Data Base
            pnlDataBase.Parent      = pnlWorkspace;
            pnlDataBase.Dock        = DockStyle.Top;
            pnlDataBase.MinimumSize = new Size(300, 200);

            // Panel Account Base
            pnlAccountBase.Parent      = pnlDataBase;
            pnlAccountBase.Dock        = DockStyle.Fill;
            pnlAccountBase.MinimumSize = new Size(100, 100);

            // Vertical splitter 1
            splitVert1.Parent = pnlDataBase;
            splitVert1.Dock   = DockStyle.Left;
            splitVert1.Width  = space;

            // Panel pnlStrategyBase
            pnlStrategyBase.Parent      = pnlDataBase;
            pnlStrategyBase.Dock        = DockStyle.Left;
            pnlStrategyBase.MinimumSize = new Size(100, 100);

            // Vertical splitter 2
            splitVert2.Parent = pnlDataBase;
            splitVert2.Dock   = DockStyle.Left;
            splitVert2.Width  = space;

            // Panel Market Base
            pnlMarketBase.Parent      = pnlDataBase;
            pnlMarketBase.Dock        = DockStyle.Left;
            pnlMarketBase.MinimumSize = new Size(100, 100);

            // Market panel
            pnlMarket.Parent  = pnlMarketBase;
            pnlMarket.Dock    = DockStyle.Fill;
            pnlMarket.Padding = new Padding(space, space, 0, 0);
            tsMarket.Parent   = pnlMarketBase;
            tsMarket.Dock     = DockStyle.Top;

            // Strategy panel
            pnlStrategy.Parent  = pnlStrategyBase;
            pnlStrategy.Dock    = DockStyle.Fill;
            pnlStrategy.Padding = new Padding(0, space, 0, 0);
            tsStrategy.Parent   = pnlStrategyBase;
            tsStrategy.Dock     = DockStyle.Top;

            // Account panel
            pnlAccount.Parent  = pnlAccountBase;
            pnlAccount.Dock    = DockStyle.Fill;
            pnlAccount.Padding = new Padding(0, space, space, 0);
            tsAccount.Parent   = pnlAccountBase;
            tsAccount.Dock     = DockStyle.Top;

            // Journal panel
            pnlJournal.Parent = pnlJournalBase;
            pnlJournal.Dock   = DockStyle.Fill;
        }

        /// <summary>
        /// Calculates the size of base panels
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            pnlJournalBase.Visible = Configs.ShowJournal;
            pnlDataBase.Height     = Configs.ShowJournal ? (int)(pnlWorkspace.ClientSize.Height * 0.630) : pnlWorkspace.ClientSize.Height - space;
            splitHoriz.Enabled     = Configs.ShowJournal;
            pnlMarketBase.Width    = pnlDataBase.ClientSize.Width / 3;
            pnlStrategyBase.Width  = pnlDataBase.ClientSize.Width / 3;

            return;
        }
    }
}
