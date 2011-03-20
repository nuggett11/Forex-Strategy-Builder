// Forex Strategy Builder - Strategy controls.
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Strategy field
    /// </summary>
    public partial class Controls : Menu_and_StatusBar
    {
        Strategy_Layout strategyLayout;
        ToolStripButton tsbtOverview;  // Opens the Overview
        ToolStripButton tsbtGenerator; // Opens the Generator
        ToolStripButton tsbtOptimizer; // Opens the Optimizer

        ToolStripButton tsbtStrategySize1;
        ToolStripButton tsbtStrategySize2;

        Button btnShowJournalByPos;
        Button btnShowJournalByBars;

        // Button Strategy Info
        // It opens the strategy description editor
        // This button has three icons:
        // 1. Gray i  - there is no any description
        // 2. Blue i  - there is a description to the strategy
        // 3. Blue i. - the description might be outdated
        ToolStripButton tsbtStrategyInfo;

        /// <summary>
        /// Initializes the strategy field
        /// </summary>
        void InitializeStrategy()
        {
            // Button Overview
            tsbtOverview = new ToolStripButton();
            tsbtOverview.Name      = "Overview";
            tsbtOverview.Text      = Language.T("Overview");
            tsbtOverview.Click    += new EventHandler(BtnTools_OnClick);
            tsbtOverview.ToolTipText = Language.T("See the strategy overview.");
            tsStrategy.Items.Add(tsbtOverview);

            // Button Generator
            tsbtGenerator = new ToolStripButton();
            tsbtGenerator.Name        = "Generator";
            tsbtGenerator.Text        = Language.T("Generator");
            tsbtGenerator.Click      += new EventHandler(BtnTools_OnClick);
            tsbtGenerator.ToolTipText = Language.T("Generate or improve a strategy.");
            tsStrategy.Items.Add(tsbtGenerator);

            // Button tsbtStrategySize1
            tsbtStrategySize1 = new ToolStripButton();
            tsbtStrategySize1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtStrategySize1.Image        = Properties.Resources.slot_size_max;
            tsbtStrategySize1.Tag          = 1;
            tsbtStrategySize1.Click       += new EventHandler(BtnSlotSize_Click);
            tsbtStrategySize1.ToolTipText  = Language.T("Show detailed info in the slots.");
            tsbtStrategySize1.Alignment    = ToolStripItemAlignment.Right;
            tsStrategy.Items.Add(tsbtStrategySize1);

            // Button tsbtStrategySize2
            tsbtStrategySize2 = new ToolStripButton();
            tsbtStrategySize2.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtStrategySize2.Image        = Properties.Resources.slot_size_min;
            tsbtStrategySize2.Tag          = 2;
            tsbtStrategySize2.Click       += new EventHandler(BtnSlotSize_Click);
            tsbtStrategySize2.ToolTipText  = Language.T("Show minimum info in the slots.");
            tsbtStrategySize2.Alignment    = ToolStripItemAlignment.Right;
            tsStrategy.Items.Add(tsbtStrategySize2);

            // Button tsbtStrategyInfo
            tsbtStrategyInfo              = new ToolStripButton();
            tsbtStrategyInfo.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtStrategyInfo.Image        = Properties.Resources.str_info_infook;
            tsbtStrategyInfo.Tag          = SlotSizeMinMidMax.min;
            tsbtStrategyInfo.Click       += new EventHandler(BtnStrategyDescription_Click);
            tsbtStrategyInfo.ToolTipText  = Language.T("Show the strategy description.");
            tsbtStrategyInfo.Alignment    = ToolStripItemAlignment.Right;
            tsStrategy.Items.Add(tsbtStrategyInfo);

            // Button Optimizer
            tsbtOptimizer = new ToolStripButton();
            tsbtOptimizer.Name        = "Optimizer";
            tsbtOptimizer.Text        = Language.T("Optimizer");
            tsbtOptimizer.Click      += new EventHandler(BtnTools_OnClick);
            tsbtOptimizer.ToolTipText = Language.T("Optimize the strategy parameters.");
            tsStrategy.Items.Add(tsbtOptimizer);

            // Creates strategyLayout
            strategyLayout        = new Strategy_Layout(Data.Strategy.Clone());
            strategyLayout.Parent = pnlStrategy;
            strategyLayout.btnAddOpenFilter.Click  += new EventHandler(BtnAddOpenFilter_Click);
            strategyLayout.btnAddCloseFilter.Click += new EventHandler(BtnAddCloseFilter_Click);

            btnShowJournalByPos = new Button();
            btnShowJournalByPos.Parent = pnlStrategy;
            btnShowJournalByPos.Text   = Language.T("Journal by Positions");
            btnShowJournalByPos.UseVisualStyleBackColor = true;
            btnShowJournalByPos.Click += new EventHandler(BtnShowJournalByPos_Click);

            btnShowJournalByBars = new Button();
            btnShowJournalByBars.Parent = pnlStrategy;
            btnShowJournalByBars.Text   = Language.T("Journal by Bars");
            btnShowJournalByBars.UseVisualStyleBackColor = true;
            btnShowJournalByBars.Click += new EventHandler(BtnShowJournalByBars_Click);

            pnlStrategy.Resize += new EventHandler(PnlStrategy_Resize);

            return;
        }

        /// <summary>
        /// Arranges the controls after resizing
        /// </summary>
        void PnlStrategy_Resize(object sender, EventArgs e)
        {
            if (Configs.ShowJournal)
            {
                btnShowJournalByPos.Visible  = false;
                btnShowJournalByBars.Visible = false;
                strategyLayout.Size     = new Size(pnlStrategy.ClientSize.Width, pnlStrategy.ClientSize.Height - space);
                strategyLayout.Location = new Point(0, space);
            }
            else
            {
                btnShowJournalByPos.Visible  = true;
                btnShowJournalByBars.Visible = true;
                btnShowJournalByPos.Width = btnShowJournalByBars.Width = (pnlStrategy.ClientSize.Width - space) / 2;
                btnShowJournalByPos.Location  = new Point(0, pnlStrategy.Height - btnShowJournalByPos.Height + 1);
                btnShowJournalByBars.Location = new Point(btnShowJournalByPos.Right + space, btnShowJournalByPos.Top);
                strategyLayout.Size     = new Size(pnlStrategy.ClientSize.Width, btnShowJournalByPos.Top - space - 2);
                strategyLayout.Location = new Point(0, space);
            }
        }

        /// <summary>
        /// Shows Journal by bars.
        /// </summary>
        void BtnShowJournalByBars_Click(object sender, EventArgs e)
        {
            Configs.ShowJournal   = true;
            Configs.JournalByBars = true;
            miJournalByPos.Checked  = !Configs.JournalByBars;
            miJournalByBars.Checked = Configs.JournalByBars;

            ResetJournal();
        }

        /// <summary>
        /// Shows Journal by positions.
        /// </summary>
        void BtnShowJournalByPos_Click(object sender, EventArgs e)
        {
            Configs.ShowJournal   = true;
            Configs.JournalByBars = false;
            miJournalByPos.Checked  = !Configs.JournalByBars;
            miJournalByBars.Checked = Configs.JournalByBars;

            ResetJournal();
        }

        /// <summary>
        /// Creates a new strategy layout using Data.Strategy
        /// </summary>
        protected void RebuildStrategyLayout()
        {
            strategyLayout.RebuildStrategyControls(Data.Strategy.Clone());
            strategyLayout.pnlProperties.Click += new EventHandler(PnlAveraging_Click);
            for (int iSlot = 0; iSlot < Data.Strategy.Slots; iSlot++)
            {
                ToolStripMenuItem miEdit = new ToolStripMenuItem();
                miEdit.Text   = Language.T("Edit") + "...";
                miEdit.Image  = Properties.Resources.edit;
                miEdit.Name   = "Edit";
                miEdit.Tag    = iSlot;
                miEdit.Click += new EventHandler(SlotContextMenu_Click);

                ToolStripMenuItem miUpwards = new ToolStripMenuItem();
                miUpwards.Text    = Language.T("Move Up");
                miUpwards.Image   = Properties.Resources.up_arrow;
                miUpwards.Name    = "Upwards";
                miUpwards.Tag     = iSlot;
                miUpwards.Click  += new EventHandler(SlotContextMenu_Click);
                miUpwards.Enabled = (iSlot > 1 && Data.Strategy.Slot[iSlot].SlotType == Data.Strategy.Slot[iSlot - 1].SlotType);

                ToolStripMenuItem miDownwards = new ToolStripMenuItem();
                miDownwards.Text    = Language.T("Move Down");
                miDownwards.Image   = Properties.Resources.down_arrow;
                miDownwards.Name    = "Downwards";
                miDownwards.Tag     = iSlot;
                miDownwards.Click  += new EventHandler(SlotContextMenu_Click);
                miDownwards.Enabled = (iSlot < Data.Strategy.Slots - 1 && Data.Strategy.Slot[iSlot].SlotType == Data.Strategy.Slot[iSlot + 1].SlotType);

                ToolStripMenuItem miDuplicate = new ToolStripMenuItem();
                miDuplicate.Text    = Language.T("Duplicate");
                miDuplicate.Image   = Properties.Resources.duplicate;
                miDuplicate.Name    = "Duplicate";
                miDuplicate.Tag     = iSlot;
                miDuplicate.Click  += new EventHandler(SlotContextMenu_Click);
                miDuplicate.Enabled = ( Data.Strategy.Slot[iSlot].SlotType == SlotTypes.OpenFilter  && Data.Strategy.OpenFilters  < Strategy.MaxOpenFilters ||
                                        Data.Strategy.Slot[iSlot].SlotType == SlotTypes.CloseFilter && Data.Strategy.CloseFilters < Strategy.MaxCloseFilters);

                ToolStripMenuItem miDelete = new ToolStripMenuItem();
                miDelete.Text    = Language.T("Delete");
                miDelete.Image   = Properties.Resources.close_button;
                miDelete.Name    = "Delete";
                miDelete.Tag     = iSlot;
                miDelete.Click  += new EventHandler(SlotContextMenu_Click);
                miDelete.Enabled = (Data.Strategy.Slot[iSlot].SlotType == SlotTypes.OpenFilter || Data.Strategy.Slot[iSlot].SlotType == SlotTypes.CloseFilter);

                strategyLayout.apnlSlot[iSlot].ContextMenuStrip = new ContextMenuStrip();
                strategyLayout.apnlSlot[iSlot].ContextMenuStrip.Items.AddRange(new ToolStripMenuItem[] { miEdit, miUpwards, miDownwards, miDuplicate, miDelete });

                strategyLayout.apnlSlot[iSlot].MouseClick += new MouseEventHandler(PnlSlot_MouseUp);
                if (iSlot != Data.Strategy.OpenSlot && iSlot != Data.Strategy.CloseSlot)
                    strategyLayout.abtnRemoveSlot[iSlot].Click += new EventHandler(BtnRemoveSlot_Click);
            }

            SetSrategyDescriptionButton();
        }

        /// <summary>
        /// Repaint the strategy slots without changing its kind and count
        /// </summary>
        protected void RepaintStrategyLayout()
        {
            strategyLayout.RepaintStrategyControls(Data.Strategy.Clone());
        }

        /// <summary>
        /// Rearange the strategy slots without changing its kind and count
        /// </summary>
        protected void RearangeStrategyLayout()
        {
            strategyLayout.RearangeStrategyControls();
        }

        /// <summary>
        /// Opens the averaging parameters dialog.
        /// </summary>
        protected virtual void PnlAveraging_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Click on a strategy slot
        /// </summary>
        protected virtual void PnlSlot_MouseUp(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Click on a strategy slot
        /// </summary>
        protected virtual void SlotContextMenu_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Performs actions after the button add open filter was clicked.
        /// </summary>
        protected virtual void BtnAddOpenFilter_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Performs actions after the button add close filter was clicked.
        /// </summary>
        protected virtual void BtnAddCloseFilter_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Removes the corresponding slot.
        /// </summary>
        protected virtual void BtnRemoveSlot_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Changes the slot size
        /// </summary>
        protected virtual void BtnSlotSize_Click(object sender, EventArgs e)
        {
            int iTag = (int)((ToolStripButton)sender).Tag;

            if (iTag == 1)
            {
                if (strategyLayout.SlotMinMidMax == SlotSizeMinMidMax.min ||
                    strategyLayout.SlotMinMidMax == SlotSizeMinMidMax.mid)
                {
                    tsbtStrategySize1.Image       = Properties.Resources.slot_size_mid;
                    tsbtStrategySize1.ToolTipText = Language.T("Show regular info in the slots.");
                    tsbtStrategySize2.Image       = Properties.Resources.slot_size_min;
                    tsbtStrategySize2.ToolTipText = Language.T("Show minimum info in the slots.");
                    strategyLayout.SlotMinMidMax  = SlotSizeMinMidMax.max;
                }
                else if (strategyLayout.SlotMinMidMax == SlotSizeMinMidMax.max)
                {
                    tsbtStrategySize1.Image       = Properties.Resources.slot_size_max;
                    tsbtStrategySize1.ToolTipText = Language.T("Show detailed info in the slots.");
                    tsbtStrategySize2.Image       = Properties.Resources.slot_size_min;
                    tsbtStrategySize2.ToolTipText = Language.T("Show minimum info in the slots.");
                    strategyLayout.SlotMinMidMax  = SlotSizeMinMidMax.mid;
                }
            }
            else
            {
                if (strategyLayout.SlotMinMidMax == SlotSizeMinMidMax.min)
                {
                    tsbtStrategySize1.Image       = Properties.Resources.slot_size_max;
                    tsbtStrategySize1.ToolTipText = Language.T("Show detailed info in the slots.");
                    tsbtStrategySize2.Image       = Properties.Resources.slot_size_min;
                    tsbtStrategySize2.ToolTipText = Language.T("Show minimum info in the slots.");
                    strategyLayout.SlotMinMidMax  = SlotSizeMinMidMax.mid;
                }
                else if (strategyLayout.SlotMinMidMax == SlotSizeMinMidMax.mid ||
                         strategyLayout.SlotMinMidMax == SlotSizeMinMidMax.max)
                {
                    tsbtStrategySize1.Image       = Properties.Resources.slot_size_max;
                    tsbtStrategySize1.ToolTipText = Language.T("Show detailed info in the slots.");
                    tsbtStrategySize2.Image       = Properties.Resources.slot_size_mid;
                    tsbtStrategySize2.ToolTipText = Language.T("Show regular info in the slots.");
                    strategyLayout.SlotMinMidMax  = SlotSizeMinMidMax.min;
                }
            }

            RearangeStrategyLayout();
        }

        /// <summary>
        /// View / edit the strategy description
        /// </summary>
        protected virtual void BtnStrategyDescription_Click(object sender, EventArgs e)
        {
            string oldInfo = Data.Strategy.Description;
            Strategy_Description si = new Strategy_Description();
            si.ShowDialog();
            if (oldInfo != Data.Strategy.Description)
            {
                Data.SetStrategyIndicators();
                SetSrategyDescriptionButton();
                this.Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
                Data.IsStrategyChanged = true;
            }
        }

        /// <summary>
        /// Sets the strategy description button icon
        /// </summary>
        void SetSrategyDescriptionButton()
        {
            if (Data.Strategy.Description == "")
                tsbtStrategyInfo.Image = Properties.Resources.str_info_noinfo;
            else
            {
                if (Data.IsStrDescriptionRelevant())
                    tsbtStrategyInfo.Image = Properties.Resources.str_info_infook;
                else
                    tsbtStrategyInfo.Image = Properties.Resources.str_info_warning;
            }
        }
    }
}
