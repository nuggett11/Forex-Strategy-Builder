// Forex Strategy Builder - Journal contrlos.
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Controls Journal: Menu_and_StatusBar
    /// </summary>
    public partial class Controls : Menu_and_StatusBar
    {
        protected Panel pnlJournalRight; // Parent panel for Order and Position panels 
        protected Journal_Positions pnlJournalByPositions;
        protected Journal_Bars pnlJournalByBars;
        protected Journal_Ord  pnlJournalOrder;
        protected Journal_Pos  pnlJournalPosition;
        protected Splitter splJournal;
        protected int selectedBar; // The bar number represented by the selected row

        /// <summary>
        /// Initialise the controls in panel pnlJournal.
        /// </summary>
        void InitializeJournal()
        {
            // pnlJournalRight
            pnlJournalRight = new Panel();
            pnlJournalRight.Parent = pnlJournal;
            pnlJournalRight.Dock   = DockStyle.Fill;

            // pnlJournalOrder
            pnlJournalOrder = new Journal_Ord();
            pnlJournalOrder.Parent = pnlJournalRight;
            pnlJournalOrder.Dock   = DockStyle.Fill;
            pnlJournalOrder.Cursor = Cursors.Hand;
            pnlJournalOrder.Click += new EventHandler(PnlJournal_Click);
            pnlJournalOrder.BtnRemoveJournal.Click += new EventHandler(BtnRemoveJournal_Click);
            pnlJournalOrder.BtnToggleJournal.Click += new EventHandler(BtnToggleJournal_Click);
            toolTip.SetToolTip(pnlJournalOrder, Language.T("Click to view Bar Explorer."));

            Splitter splitter1 = new Splitter();
            splitter1.Parent   = pnlJournalRight;
            splitter1.Dock     = DockStyle.Bottom;
            splitter1.Height   = space;

            // pnlJournalPosition
            pnlJournalPosition = new Journal_Pos();
            pnlJournalPosition.Parent = pnlJournalRight;
            pnlJournalPosition.Dock   = DockStyle.Bottom;
            pnlJournalPosition.Cursor = Cursors.Hand;
            pnlJournalPosition.Click += new EventHandler(PnlJournal_Click);
            toolTip.SetToolTip(pnlJournalPosition, Language.T("Click to view Bar Explorer."));

            splJournal        = new Splitter();
            splJournal.Parent = pnlJournal;
            splJournal.Dock   = DockStyle.Left;
            splJournal.Width  = space;

            // pnlJournalData
            pnlJournalByBars = new Journal_Bars();
            pnlJournalByBars.Name   = "pnlJournalData";
            pnlJournalByBars.Parent = pnlJournal;
            pnlJournalByBars.Dock   = DockStyle.Left;
            pnlJournalByBars.SelectedBarChange += new EventHandler(PnlJournal_SelectedBarChange);
            pnlJournalByBars.MouseDoubleClick  += new MouseEventHandler(PnlJournal_MouseDoubleClick);
            toolTip.SetToolTip(pnlJournalByBars, Language.T("Click to select a bar.") + Environment.NewLine + Language.T("Double click to view Bar Explorer."));

            // pnlJournalByPositions
            pnlJournalByPositions = new Journal_Positions();
            pnlJournalByPositions.Name   = "pnlJournalByPositions";
            pnlJournalByPositions.Parent = pnlJournal;
            pnlJournalByPositions.Dock   = DockStyle.Fill;
            pnlJournalByPositions.SelectedBarChange += new EventHandler(PnlJournal_SelectedBarChange);
            pnlJournalByPositions.MouseDoubleClick  += new MouseEventHandler(PnlJournal_MouseDoubleClick);
            pnlJournalByPositions.BtnRemoveJournal.Click += new EventHandler(BtnRemoveJournal_Click);
            pnlJournalByPositions.BtnToggleJournal.Click += new EventHandler(BtnToggleJournal_Click);
            toolTip.SetToolTip(pnlJournalByPositions, Language.T("Click to select a bar.") + Environment.NewLine + Language.T("Double click to view Bar Explorer."));

            pnlJournal.Resize        += new EventHandler(PnlJournal_Resize);
            pnlJournalRight.Visible   = Configs.JournalByBars;
            pnlJournalByBars.Visible  = Configs.JournalByBars;
            splJournal.Visible        = Configs.JournalByBars;
            pnlJournalByPositions.Visible = !Configs.JournalByBars;

            return;
        }

        /// <summary>
        /// Sets the journal data
        /// </summary>
        protected void SetupJournal()
        {
            if (Configs.ShowJournal)
            {
                if (Configs.JournalByBars)
                {
                    pnlJournalByBars.SetUpJournal();
                    pnlJournalByBars.Invalidate();
                    selectedBar = pnlJournalByBars.SelectedBar;
                    pnlJournalOrder.SelectedBar = selectedBar;
                    pnlJournalOrder.SetUpJournal();
                    pnlJournalOrder.Invalidate();
                    pnlJournalPosition.SelectedBar = selectedBar;
                    pnlJournalPosition.SetUpJournal();
                    pnlJournalPosition.Invalidate();
                }
                else
                {
                    pnlJournalByPositions.SetUpJournal();
                    pnlJournalByPositions.Invalidate();
                    selectedBar = pnlJournalByBars.SelectedBar;
                }
            }

            return;
        }

        /// <summary>
        /// Arranges the controls after resizing
        /// </summary>
        void PnlJournal_Resize(object sender, EventArgs e)
        {
            if (Configs.ShowJournal && Configs.JournalByBars)
            {
                pnlJournalByBars.Width    = 2 * this.ClientSize.Width / 3;
                pnlJournalPosition.Height = (pnlJournal.ClientSize.Height - space) / 2;
            }

            return;
        }

        /// <summary>
        /// Sets the selected bar number
        /// </summary>
        void PnlJournal_SelectedBarChange(object sender, EventArgs e)
        {
            Panel pnl = (Panel)sender;
            if (pnl.Name == "pnlJournalData")
            {
                selectedBar = pnlJournalByBars.SelectedBar;
                pnlJournalOrder.SelectedBar = selectedBar;
                pnlJournalOrder.SetUpJournal();
                pnlJournalOrder.Invalidate();
                pnlJournalPosition.SelectedBar = selectedBar;
                pnlJournalPosition.SetUpJournal();
                pnlJournalPosition.Invalidate();
            }
            else if (pnl.Name == "pnlJournalByPositions")
            {
                selectedBar = pnlJournalByPositions.SelectedBar;
            }

            return;
        }

        /// <summary>
        /// Shows the Bar Explorer
        /// </summary>
        void PnlJournal_Click(object sender, EventArgs e)
        {
            Bar_Explorer barExplorer = new Bar_Explorer(selectedBar);
            barExplorer.ShowDialog();

            return;
        }

        /// <summary>
        /// Shows the Bar Explorer
        /// </summary>
        void PnlJournal_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Bar_Explorer barExplorer = new Bar_Explorer(selectedBar);
            barExplorer.ShowDialog();

            return;
        }

        /// <summary>
        /// Removes the journal
        /// </summary>
        void BtnRemoveJournal_Click(object sender, EventArgs e)
        {
            miJournalByPos.Checked  = false;
            miJournalByBars.Checked = false;

            Configs.ShowJournal     = false;
            OnResize(EventArgs.Empty);

            return;
        }

        /// <summary>
        /// Toggles the journal view
        /// </summary>
        void BtnToggleJournal_Click(object sender, EventArgs e)
        {
            Configs.JournalByBars   = !Configs.JournalByBars;
            miJournalByPos.Checked  = !Configs.JournalByBars;
            miJournalByBars.Checked = Configs.JournalByBars;

            ResetJournal();

            return;
        }

        /// <summary>
        /// Reset the jounal leyout
        /// </summary>
        protected void ResetJournal()
        {
            SetupJournal();

            pnlJournalRight.Visible  = Configs.JournalByBars;
            splJournal.Visible       = Configs.JournalByBars;
            pnlJournalByBars.Visible = Configs.JournalByBars;
            pnlJournalByPositions.Visible = !Configs.JournalByBars;

            if (Configs.ShowJournal && Configs.JournalByBars)
            {
                pnlJournalByBars.Width    = 2 * ClientSize.Width / 3;
                pnlJournalPosition.Height = pnlJournal.ClientSize.Height / 2;
            }

            OnResize(EventArgs.Empty);

            return;
        }
    }
}