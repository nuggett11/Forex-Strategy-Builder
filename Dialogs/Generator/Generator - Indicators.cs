// Ban Indicators
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Generator
{
    public class IndicatorsLayout : Panel
    {
        ToolStrip tsIndLayout;

        VScrollBar vScrollBar;
        FlowLayoutPanel flowLayoutIndicators;
        Panel layoutBase;
        ToolStripComboBox cbxIndicatorSlot;
        ToolStripButton tsbtnSelectAll;
        ToolStripButton tsbtnSelectNone;
        ToolStripButton tsbtnStatus;

        SlotTypes currentSlotType = SlotTypes.Open;

        List<string> bannedEntryIndicators       = new List<string>();
        List<string> bannedEntryFilterIndicators = new List<string>();
        List<string> bannedExitIndicators        = new List<string>();
        List<string> bannedExitFilterIndicators  = new List<string>();

        int margin = 3;
        bool isBlocked = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public IndicatorsLayout()
        {
            tsIndLayout = new ToolStrip();
            layoutBase = new Panel();
            flowLayoutIndicators = new FlowLayoutPanel();
            vScrollBar = new VScrollBar();
            cbxIndicatorSlot = new ToolStripComboBox();

            tsIndLayout.CanOverflow = false;

            cbxIndicatorSlot.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxIndicatorSlot.AutoSize = false;
            cbxIndicatorSlot.Items.AddRange(new string[] {
                Language.T("Opening Point of the Position"),
                Language.T("Opening Logic Condition"),
                Language.T("Closing Point of the Position"),
                Language.T("Closing Logic Condition")
            });
            cbxIndicatorSlot.SelectedIndex = 0;
            cbxIndicatorSlot.SelectedIndexChanged += new EventHandler(CbxIndicatorSlot_SelectedIndexChanged);

            tsbtnSelectAll = new ToolStripButton();
            tsbtnSelectAll.Name = "tsbtnSelectAll";
            tsbtnSelectAll.Click       += new EventHandler(Buttons_Click);
            tsbtnSelectAll.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtnSelectAll.Image        = Properties.Resources.optimizer_select_all;
            tsbtnSelectAll.ToolTipText  = Language.T("Allow all indicators.");
            tsbtnSelectAll.Alignment    = ToolStripItemAlignment.Right;

            tsbtnSelectNone = new ToolStripButton();
            tsbtnSelectNone.Name         = "tsbtnSelectNone";
            tsbtnSelectNone.Click       += new EventHandler(Buttons_Click);
            tsbtnSelectNone.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtnSelectNone.Image        = Properties.Resources.optimizer_select_none;
            tsbtnSelectNone.ToolTipText  = Language.T("Bann all indicators.");
            tsbtnSelectNone.Alignment    = ToolStripItemAlignment.Right;

            tsbtnStatus = new ToolStripButton();
            tsbtnStatus.Name      = "tsbtnStatus";
            tsbtnStatus.Text      = Language.T("banned");
            tsbtnStatus.Click    += new EventHandler(Buttons_Click);
            tsbtnStatus.Alignment = ToolStripItemAlignment.Right;

            tsIndLayout.Items.Add(cbxIndicatorSlot);
            tsIndLayout.Items.Add(tsbtnStatus);
            tsIndLayout.Items.Add(tsbtnSelectNone);
            tsIndLayout.Items.Add(tsbtnSelectAll);

            // Layout base
            layoutBase.Parent = this;
            layoutBase.Dock      = DockStyle.Fill;
            layoutBase.BackColor = LayoutColors.ColorControlBack;

            // Tool Strip Strategy
            tsIndLayout.Parent = this;
            tsIndLayout.Dock   = DockStyle.Top;

            // flowLayoutIndicators
            flowLayoutIndicators.Parent = layoutBase;
            flowLayoutIndicators.AutoScroll    = false;
            flowLayoutIndicators.AutoSize      = true;
            flowLayoutIndicators.FlowDirection = FlowDirection.TopDown;
            flowLayoutIndicators.BackColor     = LayoutColors.ColorControlBack;

            // VScrollBarStrategy
            vScrollBar.Parent  = layoutBase;
            vScrollBar.TabStop = true;
            vScrollBar.Scroll += new ScrollEventHandler(VScrollBar_Scroll);

            InitBannedIndicators();
            SetStatusButton();
            ArrangeIndicatorsSlots();
            vScrollBar.Select();
        }

        /// <summary>
        /// Reads config file record and arranges lists.
        /// </summary>
        void InitBannedIndicators()
        {
            string config = Configs.BannedIndicators;
            string NL = ";";
            if (config == "")
            {   // Preparing config string after reset.
                config  = "__OpenPoint__" + NL + "__OpenFilters__" + NL + "__ClosePoint__" + NL + "__CloseFilters__" + NL;
                Configs.BannedIndicators = config;
                return;
            }

            string [] banned = config.Split(new string[] { NL }, StringSplitOptions.RemoveEmptyEntries);
            SlotTypes indSlot = SlotTypes.NotDefined;
            foreach (string ind in banned)
            {
                if (ind == "__OpenPoint__")
                {
                    indSlot = SlotTypes.Open;
                    continue;
                }
                else if (ind == "__OpenFilters__")
                {
                    indSlot = SlotTypes.OpenFilter;
                    continue;
                }
                else if (ind == "__ClosePoint__")
                {
                    indSlot = SlotTypes.Close;
                    continue;
                }
                else if (ind == "__CloseFilters__")
                {
                    indSlot = SlotTypes.CloseFilter;
                    continue;
                }

                if (indSlot == SlotTypes.Open && ind != "")
                    if (!bannedEntryIndicators.Contains(ind))
                        bannedEntryIndicators.Add(ind);

                if (indSlot == SlotTypes.OpenFilter && ind != "")
                    if (!bannedEntryFilterIndicators.Contains(ind))
                        bannedEntryFilterIndicators.Add(ind);

                if (indSlot == SlotTypes.Close && ind != "")
                    if (!bannedExitIndicators.Contains(ind))
                        bannedExitIndicators.Add(ind);

                if (indSlot == SlotTypes.CloseFilter && ind != "")
                    if (!bannedExitFilterIndicators.Contains(ind))
                        bannedExitFilterIndicators.Add(ind);
            }
        }

        /// <summary>
        /// Checks if the indicator is in the bann list.
        /// </summary>
        public bool IsIndicatorBanned(SlotTypes slotType, string indicatorName)
        {
            bool bann = false;

            if (slotType == SlotTypes.Open)
                bann = bannedEntryIndicators.Contains(indicatorName);
            else if (slotType == SlotTypes.OpenFilter)
                bann = bannedEntryFilterIndicators.Contains(indicatorName);
            else if (slotType == SlotTypes.Close)
                bann = bannedExitIndicators.Contains(indicatorName);
            else if (slotType == SlotTypes.CloseFilter)
                bann = bannedExitFilterIndicators.Contains(indicatorName);

            return bann;
        }

        public void BlockIndikatorChange()
        {
            isBlocked = true;
            tsbtnSelectAll.Enabled  = false;
            tsbtnSelectNone.Enabled = false;
            ArrangeIndicatorsSlots();
        }

        public void UnblockIndikatorChange()
        {
            isBlocked = false;
            tsbtnSelectAll.Enabled  = true;
            tsbtnSelectNone.Enabled = true;
            ArrangeIndicatorsSlots();
        }

        /// <summary>
        /// Writes banned indicator in the config file.
        /// </summary>
        public void SetConfigFile()
        {
            string config = "";
            string NL = ";";

            config = "__OpenPoint__" + NL;
            foreach (string ind in bannedEntryIndicators)
                config += ind + NL;
            config += "__OpenFilters__" + NL;
            foreach (string ind in bannedEntryFilterIndicators)
                config += ind + NL;
            config += "__ClosePoint__" + NL;
            foreach (string ind in bannedExitIndicators)
                config += ind + NL;
            config += "__CloseFilters__" + NL;
            foreach (string ind in bannedExitFilterIndicators)
                config += ind + NL;

            Configs.BannedIndicators = config;
        }

        /// <summary>
        /// Rearanges layout.
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            cbxIndicatorSlot.Width = tsIndLayout.ClientSize.Width - tsbtnSelectAll.Width - tsbtnSelectNone.Width - tsbtnStatus.Width - 15;
            SetVerticalScrollBar();
        }

        /// <summary>
        /// Change of the slot type.
        /// </summary>
        void CbxIndicatorSlot_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentSlotType = (SlotTypes)(Enum.GetValues(typeof(SlotTypes)).GetValue(cbxIndicatorSlot.SelectedIndex + 1));
            ArrangeIndicatorsSlots();
            SetStatusButton();
            SetVerticalScrollBar();
            vScrollBar.Select();
        }

        /// <summary>
        /// Change of the indicator bann state.
        /// </summary>
        void ChbxIndicator_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chbxIndicator = (CheckBox)sender;
            bool isBanned = !chbxIndicator.Checked;
            string indicatorName = chbxIndicator.Text;

            if (currentSlotType == SlotTypes.Open)
            {
                if (isBanned)
                {
                    if (!bannedEntryIndicators.Contains(indicatorName))
                        bannedEntryIndicators.Add(indicatorName);
                }
                else
                {
                    if (bannedEntryIndicators.Contains(indicatorName))
                        bannedEntryIndicators.Remove(indicatorName);
                }
            }
            else if (currentSlotType == SlotTypes.OpenFilter)
            {
                if (isBanned)
                {
                    if (!bannedEntryFilterIndicators.Contains(indicatorName))
                        bannedEntryFilterIndicators.Add(indicatorName);
                }
                else
                {
                    if (bannedEntryFilterIndicators.Contains(indicatorName))
                        bannedEntryFilterIndicators.Remove(indicatorName);
                }
            }
            else if (currentSlotType == SlotTypes.Close)
            {
                if (isBanned)
                {
                    if (!bannedExitIndicators.Contains(indicatorName))
                        bannedExitIndicators.Add(indicatorName);
                }
                else
                {
                    if (bannedExitIndicators.Contains(indicatorName))
                        bannedExitIndicators.Remove(indicatorName);
                }
            }
            else if (currentSlotType == SlotTypes.CloseFilter)
            {
                if (isBanned)
                {
                    if (!bannedExitFilterIndicators.Contains(indicatorName))
                        bannedExitFilterIndicators.Add(indicatorName);
                }
                else
                {
                    if (bannedExitFilterIndicators.Contains(indicatorName))
                        bannedExitFilterIndicators.Remove(indicatorName);
                }
            }

            SetStatusButton();
            vScrollBar.Select();
        }

        /// <summary>
        /// Arranges the indicators in the layout.
        /// </summary>
        void ArrangeIndicatorsSlots()
        {
            List<string> currentIndicators = new List<string>();
            if (currentSlotType == SlotTypes.Open)
                currentIndicators = Indicator_Store.OpenPointIndicators;
            else if (currentSlotType == SlotTypes.OpenFilter)
                currentIndicators = Indicator_Store.OpenFilterIndicators;
            else if (currentSlotType == SlotTypes.Close)
                currentIndicators = Indicator_Store.ClosePointIndicators;
            else if (currentSlotType == SlotTypes.CloseFilter)
                currentIndicators = Indicator_Store.CloseFilterIndicators;

            flowLayoutIndicators.SuspendLayout();
            flowLayoutIndicators.Controls.Clear();
            flowLayoutIndicators.Height = 0;
            foreach (string indicatorName in currentIndicators)
            {
                CheckBox chbxIndicator = new CheckBox();
                chbxIndicator.AutoSize = true;
                chbxIndicator.Checked  = true;
                if (currentSlotType == SlotTypes.Open)
                    chbxIndicator.Checked = !bannedEntryIndicators.Contains(indicatorName);
                else if (currentSlotType == SlotTypes.OpenFilter)
                    chbxIndicator.Checked = !bannedEntryFilterIndicators.Contains(indicatorName);
                else if (currentSlotType == SlotTypes.Close)
                    chbxIndicator.Checked = !bannedExitIndicators.Contains(indicatorName);
                else if (currentSlotType == SlotTypes.CloseFilter)
                    chbxIndicator.Checked = !bannedExitFilterIndicators.Contains(indicatorName);
                chbxIndicator.Margin  = new Padding(margin, margin, 0, 0);
                chbxIndicator.Text    = indicatorName;
                chbxIndicator.Enabled = !isBlocked;
                chbxIndicator.CheckedChanged += new EventHandler(ChbxIndicator_CheckedChanged);
                flowLayoutIndicators.Controls.Add(chbxIndicator);
            }
            flowLayoutIndicators.ResumeLayout();
        }

        /// <summary>
        /// Shows, hides, sets the scrollbar.
        /// </summary>
        void SetVerticalScrollBar()
        {
            int width  = layoutBase.Width - vScrollBar.Width;
            int height = layoutBase.Height;
            int totalHeight = flowLayoutIndicators.Height;

            vScrollBar.Enabled     = true;
            vScrollBar.Visible     = true;
            vScrollBar.Value       = 0;
            vScrollBar.SmallChange = 30;
            vScrollBar.LargeChange = 60;
            vScrollBar.Maximum     = Math.Max(totalHeight - height + 60, 0);
            vScrollBar.Location    = new Point(width, 0);
            vScrollBar.Height      = height;
            vScrollBar.Cursor      = Cursors.Default;

            flowLayoutIndicators.Location = new Point(0, 0);

            return;
        }

        /// <summary>
        /// Sets the text of button Status.
        /// </summary>
        void SetStatusButton()
        {
            int banned = bannedEntryIndicators.Count + bannedEntryFilterIndicators.Count;
            banned += bannedExitIndicators.Count + bannedExitFilterIndicators.Count;
            tsbtnStatus.Text = banned.ToString() + " " + Language.T("banned");
            cbxIndicatorSlot.Width = tsIndLayout.ClientSize.Width - tsbtnSelectAll.Width - tsbtnSelectNone.Width - tsbtnStatus.Width - 15;
        }

        /// <summary>
        /// The Scrolling moves the flowLayout
        /// <summary>
        void VScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            VScrollBar vscroll = (VScrollBar)sender;
            flowLayoutIndicators.Location = new Point(0, -vscroll.Value);
        }
 
        /// <summary>
        /// ToolStrip Buttons click
        /// </summary>
        void Buttons_Click(object sender, EventArgs e)
        {
            ToolStripButton button = (ToolStripButton)sender;
            string name = button.Name;

            if (name == "tsbtnSelectAll")
            {
                if (currentSlotType == SlotTypes.Open)
                    bannedEntryIndicators.Clear();
                else if (currentSlotType == SlotTypes.OpenFilter)
                    bannedEntryFilterIndicators.Clear();
                else if (currentSlotType == SlotTypes.Close)
                    bannedExitIndicators.Clear();
                else if (currentSlotType == SlotTypes.CloseFilter)
                    bannedExitFilterIndicators.Clear();

                ArrangeIndicatorsSlots();
                SetStatusButton();
            }
            else if (name == "tsbtnSelectNone")
            {
                if (currentSlotType == SlotTypes.Open)
                {
                    bannedEntryIndicators.Clear();
                    bannedEntryIndicators.AddRange(Indicator_Store.OpenPointIndicators.GetRange(0, Indicator_Store.OpenPointIndicators.Count));
                }
                else if (currentSlotType == SlotTypes.OpenFilter){
                    bannedEntryFilterIndicators.Clear();
                    bannedEntryFilterIndicators.AddRange(Indicator_Store.OpenFilterIndicators.GetRange(0, Indicator_Store.OpenFilterIndicators.Count));
                }
                else if (currentSlotType == SlotTypes.Close)
                {
                    bannedExitIndicators.Clear();
                    bannedExitIndicators.AddRange(Indicator_Store.ClosePointIndicators.GetRange(0, Indicator_Store.ClosePointIndicators.Count));
                }
                else if (currentSlotType == SlotTypes.CloseFilter)
                {
                    bannedExitFilterIndicators.Clear();
                    bannedExitFilterIndicators.AddRange(Indicator_Store.CloseFilterIndicators.GetRange(0, Indicator_Store.CloseFilterIndicators.Count));
                }
                ArrangeIndicatorsSlots();
                SetStatusButton();
            }
            else if (name == "tsbtnStatus")
            {
                ShowStatus();
            }
        }

        /// <summary>
        /// Shows all banned indicators.
        /// </summary>
        void ShowStatus()
        {
            string text = "";

            if (bannedEntryIndicators.Count > 0)
            {
                text = "<h2>" + Language.T("Opening Point of the Position") + "</h2>";
                text += "<ul>";
                foreach (string ind in bannedEntryIndicators)
                    text += "<li>" + ind + "</li>";
                text += "</ul>";
            }

            if (bannedEntryFilterIndicators.Count > 0)
            {
                text += "<h2>" + Language.T("Opening Logic Condition") + "</h2>";
                text += "<ul>";
                foreach (string ind in bannedEntryFilterIndicators)
                    text += "<li>" + ind + "</li>";
                text += "</ul>";
            }

            if (bannedExitIndicators.Count > 0)
            {
                text += "<h2>" + Language.T("Closing Point of the Position") + "</h2>";
                text += "<ul>";
                foreach (string ind in bannedExitIndicators)
                    text += "<li>" + ind + "</li>";
                text += "</ul>";
            }

            if (bannedExitFilterIndicators.Count > 0)
            {
                text += "<h2>" + Language.T("Closing Logic Condition") + "</h2>";
                text += "<ul>";
                foreach (string ind in bannedExitFilterIndicators)
                    text += "<li>" + ind + "</li>";
                text += "</ul>";
            }

            Fancy_Message_Box msgbox = new Fancy_Message_Box(text, Language.T("Banned Indicators"));
            msgbox.Show();
        }
    }

}
