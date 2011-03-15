// Strategy Properties Dialogue
// Last changed on 2011-02-12
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
    /// Account Settings
    /// </summary>
    public class Strategy_Properties : Form
    {
        Fancy_Panel pnlAveraging, pnlAmounts, pnlProtection;
        Small_Balance_Chart pnlSmallBalanceChart;

        Label lblSameDirAction, lblOppDirAction;
        Label lblMaxOpenLots, lblEntryLots, lblAddingLots, lblReducingLots;
        Label lblPercent1, lblPercent2, lblPercent3;

        ComboBox cbxSameDirAction, cbxOppDirAction;
        CheckBox chbPermaSL, chbPermaTP;
        RadioButton rbConstantUnits, rbVariableUnits;
        NUD nudPermaSL, nudPermaTP;
        ComboBox cbxPermaSLType, cbxPermaTPType;
        NUD nudMaxOpenLots, nudEntryLots, nudAddingLots, nudReducingLots;

        Button btnDefault, btnAccept, btnCancel;
        
        ToolTip toolTip = new ToolTip();

        Font font;
        Color colorText;

        int leftPanelsWidth;
        int rightPanelsWidth;

        public Strategy_Properties()
        {
            pnlAveraging  = new Fancy_Panel(Language.T("Handling of Additional Entry Signals"), LayoutColors.ColorSlotCaptionBackAveraging);
            pnlAmounts    = new Fancy_Panel(Language.T("Trading Size"), LayoutColors.ColorSlotCaptionBackAveraging);
            pnlProtection = new Fancy_Panel(Language.T("Permanent Protection"), LayoutColors.ColorSlotCaptionBackAveraging);
            pnlSmallBalanceChart = new Small_Balance_Chart();

            lblPercent1  = new Label();
            lblPercent2  = new Label();
            lblPercent3  = new Label();

            lblSameDirAction = new Label();
            lblOppDirAction  = new Label();

            cbxSameDirAction = new ComboBox();
            cbxOppDirAction  = new ComboBox();
            nudMaxOpenLots   = new NUD();
            rbConstantUnits  = new RadioButton();
            rbVariableUnits  = new RadioButton();
            nudEntryLots     = new NUD();
            nudAddingLots    = new NUD();
            nudReducingLots  = new NUD();
            lblMaxOpenLots   = new Label();
            lblEntryLots     = new Label();
            lblAddingLots    = new Label();
            lblReducingLots  = new Label();

            chbPermaSL = new CheckBox();
            cbxPermaSLType = new ComboBox();
            nudPermaSL = new NUD();
            chbPermaTP = new CheckBox();
            cbxPermaTPType = new ComboBox();
            nudPermaTP = new NUD();

            btnAccept  = new Button();
            btnDefault = new Button();
            btnCancel  = new Button();

            font      = this.Font;
            colorText = LayoutColors.ColorControlText;

            MaximizeBox     = false;
            MinimizeBox     = false;
            ShowInTaskbar   = false;
            Icon            = Data.Icon;
            BackColor       = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton    = btnAccept;
            Text            = Language.T("Strategy Properties");

            pnlAveraging.Parent  = this;
            pnlAmounts.Parent    = this;
            pnlProtection.Parent = this;
            pnlSmallBalanceChart.Parent = this;

            // Label Same dir action
            lblSameDirAction.Parent    = pnlAveraging;
            lblSameDirAction.ForeColor = colorText;
            lblSameDirAction.BackColor = Color.Transparent;
            lblSameDirAction.AutoSize  = true;
            lblSameDirAction.Text      = Language.T("Next same direction signal behavior");

            // Label Opposite dir action
            lblOppDirAction.Parent    = pnlAveraging;
            lblOppDirAction.ForeColor = colorText;
            lblOppDirAction.BackColor = Color.Transparent;
            lblOppDirAction.AutoSize  = true;
            lblOppDirAction.Text      = Language.T("Next opposite direction signal behavior");

            // ComboBox SameDirAction
            cbxSameDirAction.Parent        = pnlAveraging;
            cbxSameDirAction.Name          = "cbxSameDirAction";
            cbxSameDirAction.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxSameDirAction.Items.AddRange(new string[] { Language.T("Nothing"), Language.T("Winner"), Language.T("Add") });
            cbxSameDirAction.SelectedIndex = 0;
            toolTip.SetToolTip(cbxSameDirAction, 
                Language.T("Nothing - cancels the additional orders.") + Environment.NewLine +
                Language.T("Winner - adds to a winning position.")     + Environment.NewLine +
                Language.T("Add - adds to all positions.")); 

            // ComboBox OppDirAction
            cbxOppDirAction.Parent        = pnlAveraging;
            cbxOppDirAction.Name          = "cbxOppDirAction";
            cbxOppDirAction.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxOppDirAction.Items.AddRange(new string[] { Language.T("Nothing"), Language.T("Reduce"), Language.T("Close"), Language.T("Reverse") });
            cbxOppDirAction.SelectedIndex = 0;
            toolTip.SetToolTip(cbxOppDirAction,
                Language.T("Nothing - cancels the additional orders.") + Environment.NewLine +
                Language.T("Reduce - reduces or closes a position.")   + Environment.NewLine +
                Language.T("Close - closes the position.")             + Environment.NewLine +
                Language.T("Reverse - reverses the position.")); 

            // Label MaxOpen Lots
            lblMaxOpenLots.Parent    = pnlAmounts;
            lblMaxOpenLots.ForeColor = colorText;
            lblMaxOpenLots.BackColor = Color.Transparent;
            lblMaxOpenLots.AutoSize  = true;
            lblMaxOpenLots.Text      = Language.T("Maximum number of open lots");

            // NumericUpDown MaxOpen Lots
            nudMaxOpenLots.Parent    = pnlAmounts;
            nudMaxOpenLots.Name      = "nudMaxOpenLots";
            nudMaxOpenLots.BeginInit();
            nudMaxOpenLots.Minimum   = 0.01M;
            nudMaxOpenLots.Maximum   = 100;
            nudMaxOpenLots.Increment = 0.01M;
            nudMaxOpenLots.Value     = 20;
            nudMaxOpenLots.DecimalPlaces = 2;
            nudMaxOpenLots.TextAlign = HorizontalAlignment.Center;
            nudMaxOpenLots.EndInit();

            // Radio Button Constant Units
            rbConstantUnits.Parent    = pnlAmounts;
            rbConstantUnits.ForeColor = colorText;
            rbConstantUnits.BackColor = Color.Transparent;
            rbConstantUnits.Checked   = true;
            rbConstantUnits.AutoSize  = true;
            rbConstantUnits.Name      = "rbConstantUnits";
            rbConstantUnits.Text      = Language.T("Trade a constant number of lots");

            // Radio Button Variable Units
            rbVariableUnits.Parent    = pnlAmounts;
            rbVariableUnits.ForeColor = colorText;
            rbVariableUnits.BackColor = Color.Transparent;
            rbVariableUnits.Checked   = false;
            rbVariableUnits.AutoSize  = false;
            rbVariableUnits.Name      = "rbVariableUnits";
            rbVariableUnits.Text      = Language.T("Trade percent of your account. The percentage values show the part of the account equity used to cover the required margin.");

            // Label Entry Lots
            lblEntryLots.Parent    = pnlAmounts;
            lblEntryLots.ForeColor = colorText;
            lblEntryLots.BackColor = Color.Transparent;
            lblEntryLots.AutoSize  = true;
            lblEntryLots.Text      = Language.T("Number of entry lots for a new position");

            // NumericUpDown Entry Lots
            nudEntryLots.Parent    = pnlAmounts;
            nudEntryLots.Name      = "nudEntryLots";
            nudEntryLots.BeginInit();
            nudEntryLots.Minimum   = 0.01M;
            nudEntryLots.Maximum   = 100;
            nudEntryLots.Increment = 0.01M;
            nudEntryLots.Value     = 1;
            nudEntryLots.DecimalPlaces = 2;
            nudEntryLots.TextAlign = HorizontalAlignment.Center;
            nudEntryLots.EndInit();

            // Label Entry Lots %
            lblPercent1.Parent    = pnlAmounts;
            lblPercent1.ForeColor = colorText;
            lblPercent1.BackColor = Color.Transparent;

            // Label Adding Lots
            lblAddingLots.Parent    = pnlAmounts;
            lblAddingLots.ForeColor = colorText;
            lblAddingLots.BackColor = Color.Transparent;
            lblAddingLots.AutoSize  = true;
            lblAddingLots.Text      = Language.T("In case of addition - number of lots to add");

            // NumericUpDown Adding Lots
            nudAddingLots.Parent    = pnlAmounts;
            nudAddingLots.Name      = "nudAddingLots";
            nudAddingLots.BeginInit();
            nudAddingLots.Minimum   = 0.01M;
            nudAddingLots.Maximum   = 100;
            nudAddingLots.Increment = 0.01M;
            nudAddingLots.Value     = 1;
            nudAddingLots.DecimalPlaces = 2; 
            nudAddingLots.TextAlign = HorizontalAlignment.Center;
            nudAddingLots.EndInit();

            // Label Adding Lots %
            lblPercent2.Parent    = pnlAmounts;
            lblPercent2.ForeColor = colorText;
            lblPercent2.BackColor = Color.Transparent;

            // Label Reducing Lots
            lblReducingLots.Parent    = pnlAmounts;
            lblReducingLots.ForeColor = colorText;
            lblReducingLots.BackColor = Color.Transparent;
            lblReducingLots.AutoSize  = true;
            lblReducingLots.Text      = Language.T("In case of reduction - number of lots to close");

            // NumericUpDown Reducing Lots
            nudReducingLots.Parent    = pnlAmounts;
            nudReducingLots.Name      = "nudReducingLots";
            nudReducingLots.BeginInit();
            nudReducingLots.Minimum   = 0.01M;
            nudReducingLots.Maximum   = 100;
            nudReducingLots.Increment = 0.01m;
            nudReducingLots.DecimalPlaces = 2;
            nudReducingLots.Value     = 1;
            nudReducingLots.TextAlign = HorizontalAlignment.Center;
            nudReducingLots.EndInit();

            // Label Reducing Lots %
            lblPercent3.Parent    = pnlAmounts;
            lblPercent3.ForeColor = colorText;
            lblPercent3.BackColor = Color.Transparent;

            // CheckBox Permanent Stop Loss
            chbPermaSL.Name      = "chbPermaSL";
            chbPermaSL.Parent    = pnlProtection;
            chbPermaSL.ForeColor = colorText;
            chbPermaSL.BackColor = Color.Transparent;
            chbPermaSL.AutoCheck = true;
            chbPermaSL.AutoSize  = true;
            chbPermaSL.Checked   = false;
            chbPermaSL.Text      = Language.T("Permanent Stop Loss");

            // ComboBox cbxPermaSLType
            cbxPermaSLType.Parent = pnlProtection;
            cbxPermaSLType.Name   = "cbxPermaSLType";
            cbxPermaSLType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxPermaSLType.Items.AddRange(new string[] { Language.T("Relative"), Language.T("Absolute") });
            cbxPermaSLType.SelectedIndex = 0;

            // NumericUpDown Permanent S/L
            nudPermaSL.Name      = "nudPermaSL";
            nudPermaSL.Parent    = pnlProtection;
            nudPermaSL.BeginInit();
            nudPermaSL.Minimum   = 5;
            nudPermaSL.Maximum   = 5000;
            nudPermaSL.Increment = 1;
            nudPermaSL.Value     = (Data.InstrProperties.Digits == 5 || Data.InstrProperties.Digits == 3) ? 1000 : 100;
            nudPermaSL.TextAlign = HorizontalAlignment.Center;
            nudPermaSL.EndInit();

            // CheckBox Permanent Take Profit
            chbPermaTP.Name      = "chbPermaTP";
            chbPermaTP.Parent    = pnlProtection;
            chbPermaTP.ForeColor = colorText;
            chbPermaTP.BackColor = Color.Transparent;
            chbPermaTP.AutoCheck = true;
            chbPermaTP.AutoSize  = true;
            chbPermaTP.Checked   = false;
            chbPermaTP.Text      = Language.T("Permanent Take Profit");

            // ComboBox cbxPermaTPType
            cbxPermaTPType.Parent = pnlProtection;
            cbxPermaTPType.Name   = "cbxPermaTPType";
            cbxPermaTPType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxPermaTPType.Items.AddRange(new string[] { Language.T("Relative"), Language.T("Absolute") });
            cbxPermaTPType.SelectedIndex = 0;

            // NumericUpDown Permanent Take Profit
            nudPermaTP.Parent    = pnlProtection;
            nudPermaTP.Name      = "nudPermaTP";
            nudPermaTP.BeginInit();
            nudPermaTP.Minimum   = 5;
            nudPermaTP.Maximum   = 5000;
            nudPermaTP.Increment = 1;
            nudPermaTP.Value     = (Data.InstrProperties.Digits == 5 || Data.InstrProperties.Digits == 3) ? 1000 : 100;
            nudPermaTP.TextAlign = HorizontalAlignment.Center;
            nudPermaTP.EndInit();

            //Button Default
            btnDefault.Parent = this;
            btnDefault.Name   = "btnDefault";
            btnDefault.Text   = Language.T("Default");
            btnDefault.Click += new EventHandler(BtnDefault_Click);
            btnDefault.UseVisualStyleBackColor = true;

            //Button Cancel
            btnCancel.Parent       = this;
            btnCancel.Name         = "btnCancel";
            btnCancel.Text         = Language.T("Cancel");
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.UseVisualStyleBackColor = true;

            //Button Accept
            btnAccept.Parent       = this;
            btnAccept.Name         = "btnAccept";
            btnAccept.Text         = Language.T("Accept");
            btnAccept.DialogResult = DialogResult.OK;
            btnAccept.UseVisualStyleBackColor = true;

            return;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            SetFormSize();
            SetParams();
            btnAccept.Focus();

            return;
        }

        void SetFormSize()
        {
            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth  = (int)(Data.HorizontalDLU * 60);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int space        = btnHrzSpace;
            int textHeight   = Font.Height;
            int leftComboBxWith   = 80;
            int rightComboBxWith  = 95;
            int nudWidth          = 60;
            int lblPercentWidth   = 15;
            int border            = 2;

            leftPanelsWidth  = 3 * buttonWidth + 2 * btnHrzSpace;
            rightPanelsWidth = 3 * buttonWidth + 2 * btnHrzSpace;

            if (leftPanelsWidth < space + lblSameDirAction.Width + space + leftComboBxWith + space)
                leftPanelsWidth = space + lblSameDirAction.Width + space + leftComboBxWith + space;

            if (leftPanelsWidth < space + lblOppDirAction.Width + space + leftComboBxWith + space)
                leftPanelsWidth = space + lblOppDirAction.Width + space + leftComboBxWith + space;

            if (leftPanelsWidth < space + lblMaxOpenLots.Width + space + nudWidth + space)
                leftPanelsWidth = space + lblMaxOpenLots.Width + space + nudWidth + space;

            rbVariableUnits.Width = leftPanelsWidth - 2 * space;
            Graphics g = CreateGraphics();
            while (g.MeasureString(rbVariableUnits.Text, rbVariableUnits.Font, rbVariableUnits.Width - 10).Height > 3 * rbVariableUnits.Font.Height)
                rbVariableUnits.Width++;
            g.Dispose();
            if (leftPanelsWidth < space + rbVariableUnits.Width + space)
                leftPanelsWidth = space + rbVariableUnits.Width + space;

            if (leftPanelsWidth < space + lblEntryLots.Width + space + lblPercentWidth + nudWidth + space)
                leftPanelsWidth = space + lblEntryLots.Width + space + lblPercentWidth + nudWidth + space;

            if (leftPanelsWidth < space + lblAddingLots.Width + space + lblPercentWidth + nudWidth + space)
                leftPanelsWidth = space + lblAddingLots.Width + space + lblPercentWidth + nudWidth + space;

            if (leftPanelsWidth < space + lblReducingLots.Width + space + lblPercentWidth + nudWidth + space)
                leftPanelsWidth = space + lblReducingLots.Width + space + lblPercentWidth + nudWidth + space;

            int maxRightCheckBoxWidth = Math.Max(chbPermaSL.Width, chbPermaTP.Width);
            int requiredRightPanelWidth = border + space + maxRightCheckBoxWidth + space + rightComboBxWith + space + nudWidth + space + border;
            if (rightPanelsWidth < requiredRightPanelWidth)
                rightPanelsWidth = requiredRightPanelWidth;

            ClientSize = new Size(space + leftPanelsWidth + space + rightPanelsWidth + space, 360);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int buttonWidth  = (int)((rightPanelsWidth - 2 * btnHrzSpace) / 3);
            int space        = btnHrzSpace;
            int textHeight   = Font.Height;
            int border       = 2;
            int leftComboBxWith = 80;
            int rightComboBxWith = 95;
            int nudWidth     = 60;
            int lblPercentWidth = 15;

            // pnlAveraging
            pnlAveraging.Size = new Size(leftPanelsWidth, 84);
            pnlAveraging.Location = new Point(space, space);

            // pnlAmounts
            pnlAmounts.Size = new Size(leftPanelsWidth, 222);
            pnlAmounts.Location = new Point(space, pnlAveraging.Bottom + space);

            // pnlProtection
            pnlProtection.Size = new Size(rightPanelsWidth, 84);
            pnlProtection.Location = new Point(pnlAveraging.Right + space, pnlAveraging.Top);

            // Averaging
            int comboBxLeft = pnlAveraging.ClientSize.Width - leftComboBxWith - space - border;

            cbxSameDirAction.Width = leftComboBxWith;
            lblSameDirAction.Location = new Point(space, space + 25);
            cbxSameDirAction.Location = new Point(comboBxLeft, space + 21);

            cbxOppDirAction.Width = leftComboBxWith;
            lblOppDirAction.Location = new Point(space, buttonHeight + 2 * space + 23);
            cbxOppDirAction.Location = new Point(comboBxLeft, buttonHeight + 2 * space + 19);

            // Amounts
            int nudLeft = leftPanelsWidth - nudWidth - space - border;

            lblMaxOpenLots.Location = new Point(space, 0 * buttonHeight + space + 25);
            nudMaxOpenLots.Size = new Size(nudWidth, buttonHeight);
            nudMaxOpenLots.Location = new Point(nudLeft, 0 * buttonHeight + space + 22);

            rbConstantUnits.Location = new Point(space + 3, 55);
            rbVariableUnits.Location = new Point(space + 3, 79);
            rbVariableUnits.Size = new Size(leftPanelsWidth - 2 * space, 2 * buttonHeight);

            lblEntryLots.Location = new Point(btnHrzSpace, 139);
            nudEntryLots.Size = new Size(nudWidth, buttonHeight);
            nudEntryLots.Location = new Point(nudLeft, 137);
            lblPercent1.Width = lblPercentWidth;
            lblPercent1.Location = new Point(nudEntryLots.Left - lblPercentWidth, lblEntryLots.Top);

            lblAddingLots.Location = new Point(btnHrzSpace, 167);
            nudAddingLots.Size = new Size(nudWidth, buttonHeight);
            nudAddingLots.Location = new Point(nudLeft, 165);
            lblPercent2.Width = lblPercentWidth;
            lblPercent2.Location = new Point(nudAddingLots.Left - lblPercentWidth, lblAddingLots.Top);

            lblReducingLots.Location = new Point(btnHrzSpace, 195);
            nudReducingLots.Size = new Size(nudWidth, buttonHeight);
            nudReducingLots.Location = new Point(nudLeft, 193);
            lblPercent3.Width = lblPercentWidth;
            lblPercent3.Location = new Point(nudReducingLots.Left - lblPercentWidth, lblReducingLots.Top);

            nudLeft = rightPanelsWidth - nudWidth - btnHrzSpace - border;
            comboBxLeft = nudLeft - space - rightComboBxWith;

            // Permanent Stop Loss
            chbPermaSL.Location = new Point(border + space, 0 * buttonHeight + 1 * space + 24);
            cbxPermaSLType.Width = rightComboBxWith;
            cbxPermaSLType.Location = new Point(comboBxLeft, 0 * buttonHeight + 1 * space + 21);
            nudPermaSL.Size = new Size(nudWidth, buttonHeight);
            nudPermaSL.Location = new Point(nudLeft, 0 * buttonHeight + 1 * space + 22);

            // Permanent Take Profit
            chbPermaTP.Location = new Point(border + space, 1 * buttonHeight + 2 * space + 22);
            nudPermaTP.Size = new Size(nudWidth, buttonHeight);
            cbxPermaTPType.Width = rightComboBxWith;
            cbxPermaTPType.Location = new Point(comboBxLeft, 1 * buttonHeight + 2 * space + 19);
            nudPermaTP.Location = new Point(nudLeft, 1 * buttonHeight + 2 * space + 20);

            pnlSmallBalanceChart.Size = new Size(rightPanelsWidth, pnlAmounts.Height);
            pnlSmallBalanceChart.Location = new Point(pnlAveraging.Right + space, pnlProtection.Bottom + space);

            // Button Accept
            btnAccept.Size  = new Size(buttonWidth, buttonHeight);
            btnDefault.Size = new Size(buttonWidth, buttonHeight);
            btnCancel.Size  = new Size(buttonWidth, buttonHeight);
            int buttonTop = ClientSize.Height - buttonHeight - btnVertSpace;

            btnAccept.Location  = new Point(pnlProtection.Left, buttonTop);
            btnDefault.Location = new Point((pnlProtection.Left + pnlProtection.Right - buttonWidth) / 2, buttonTop);
            btnCancel.Location  = new Point(pnlProtection.Right - buttonWidth, buttonTop);

            return;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        void BtnDefault_Click(object sender, EventArgs e)
        {
            Data.Strategy.SameSignalAction = SameDirSignalAction.Nothing;

            if (Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName == "Close and Reverse")
                Data.Strategy.OppSignalAction = OppositeDirSignalAction.Reverse;
            else
                Data.Strategy.OppSignalAction = OppositeDirSignalAction.Nothing;

            Data.Strategy.UsePermanentSL  = false;
            Data.Strategy.PermanentSL     = (Data.InstrProperties.Digits == 5 || Data.InstrProperties.Digits == 3) ? 1000 : 100;
            Data.Strategy.PermanentSLType = PermanentProtectionType.Relative;
            Data.Strategy.UsePermanentTP  = false;
            Data.Strategy.PermanentTP     = (Data.InstrProperties.Digits == 5 || Data.InstrProperties.Digits == 3) ? 1000 : 100;
            Data.Strategy.PermanentTPType = PermanentProtectionType.Relative;
            Data.Strategy.UseAccountPercentEntry = false;
            Data.Strategy.MaxOpenLots     = 20;
            Data.Strategy.EntryLots       = 1;
            Data.Strategy.AddingLots      = 1;
            Data.Strategy.ReducingLots    = 1;

            SetParams();
            CalculateStrategy();
            UpdateChart();

            return;
        }

        void Param_Changed(object sender, EventArgs e)
        {
            nudPermaSL.Enabled = chbPermaSL.Checked;
            nudPermaTP.Enabled = chbPermaTP.Checked;
            cbxPermaSLType.Enabled = chbPermaSL.Checked;
            cbxPermaTPType.Enabled = chbPermaTP.Checked;

            if (!rbVariableUnits.Checked)
                nudEntryLots.Value = Math.Min(nudEntryLots.Value, nudMaxOpenLots.Value);

            Data.Strategy.SameSignalAction = (SameDirSignalAction)cbxSameDirAction.SelectedIndex;
            Data.Strategy.OppSignalAction  = (OppositeDirSignalAction)cbxOppDirAction.SelectedIndex;
            Data.Strategy.UseAccountPercentEntry = rbVariableUnits.Checked;
            Data.Strategy.MaxOpenLots      = (double)nudMaxOpenLots.Value;
            Data.Strategy.EntryLots        = (double)nudEntryLots.Value;
            Data.Strategy.AddingLots       = (double)nudAddingLots.Value;
            Data.Strategy.ReducingLots     = (double)nudReducingLots.Value;
            Data.Strategy.UsePermanentSL   = chbPermaSL.Checked;
            Data.Strategy.UsePermanentTP   = chbPermaTP.Checked;
            Data.Strategy.PermanentSLType  = (PermanentProtectionType)cbxPermaSLType.SelectedIndex;
            Data.Strategy.PermanentTPType  = (PermanentProtectionType)cbxPermaTPType.SelectedIndex;
            Data.Strategy.PermanentSL      = (int)nudPermaSL.Value;
            Data.Strategy.PermanentTP      = (int)nudPermaTP.Value;

            SetLabelPercent();
            CalculateStrategy();
            UpdateChart();

            return;
        }

        void SetParams()
        {
            RemoveParamEventHandlers();

            cbxSameDirAction.SelectedIndex = (int)Data.Strategy.SameSignalAction;
            cbxOppDirAction.SelectedIndex  = (int)Data.Strategy.OppSignalAction;
            cbxOppDirAction.Enabled = Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName != "Close and Reverse";

            rbConstantUnits.Checked = !Data.Strategy.UseAccountPercentEntry;
            rbVariableUnits.Checked = Data.Strategy.UseAccountPercentEntry;

            nudMaxOpenLots.Value  = (decimal)Data.Strategy.MaxOpenLots;

            if (!rbVariableUnits.Checked)
                nudEntryLots.Value = (decimal)Math.Min(Data.Strategy.EntryLots, Data.Strategy.MaxOpenLots);
            else
                nudEntryLots.Value = (decimal)Data.Strategy.EntryLots;

            nudAddingLots.Value = (decimal)Data.Strategy.AddingLots;
            nudReducingLots.Value = (decimal)Data.Strategy.ReducingLots;

            chbPermaSL.Checked = Data.Strategy.UsePermanentSL;
            nudPermaSL.Value   = Data.Strategy.PermanentSL;
            nudPermaSL.Enabled = Data.Strategy.UsePermanentSL;
            cbxPermaSLType.Enabled = Data.Strategy.UsePermanentSL;
            cbxPermaSLType.SelectedIndex = (int)Data.Strategy.PermanentSLType;

            chbPermaTP.Checked = Data.Strategy.UsePermanentTP;
            nudPermaTP.Value   = Data.Strategy.PermanentTP;
            nudPermaTP.Enabled = Data.Strategy.UsePermanentTP;
            cbxPermaTPType.Enabled = Data.Strategy.UsePermanentTP;
            cbxPermaTPType.SelectedIndex = (int)Data.Strategy.PermanentTPType;

            SetParamEventHandlers();
            SetLabelPercent();

            return;
        }

        void SetParamEventHandlers()
        {
            cbxSameDirAction.SelectedIndexChanged += new EventHandler(Param_Changed);
            cbxOppDirAction.SelectedIndexChanged  += new EventHandler(Param_Changed);
            rbConstantUnits.CheckedChanged        += new EventHandler(Param_Changed);
            rbVariableUnits.CheckedChanged        += new EventHandler(Param_Changed);
            nudMaxOpenLots.ValueChanged           += new EventHandler(Param_Changed);
            nudEntryLots.ValueChanged             += new EventHandler(Param_Changed);
            nudAddingLots.ValueChanged            += new EventHandler(Param_Changed);
            nudReducingLots.ValueChanged          += new EventHandler(Param_Changed);
            chbPermaSL.CheckedChanged             += new EventHandler(Param_Changed);
            cbxPermaSLType.SelectedIndexChanged   += new EventHandler(Param_Changed);
            nudPermaSL.ValueChanged               += new EventHandler(Param_Changed);
            chbPermaTP.CheckedChanged             += new EventHandler(Param_Changed);
            cbxPermaTPType.SelectedIndexChanged   += new EventHandler(Param_Changed);
            nudPermaTP.ValueChanged               += new EventHandler(Param_Changed);
        }

        void RemoveParamEventHandlers()
        {
            cbxSameDirAction.SelectedIndexChanged -= new EventHandler(Param_Changed);
            cbxOppDirAction.SelectedIndexChanged  -= new EventHandler(Param_Changed);
            rbConstantUnits.CheckedChanged        -= new EventHandler(Param_Changed);
            rbVariableUnits.CheckedChanged        -= new EventHandler(Param_Changed);
            nudMaxOpenLots.ValueChanged           -= new EventHandler(Param_Changed);
            nudEntryLots.ValueChanged             -= new EventHandler(Param_Changed);
            nudAddingLots.ValueChanged            -= new EventHandler(Param_Changed);
            nudReducingLots.ValueChanged          -= new EventHandler(Param_Changed);
            chbPermaSL.CheckedChanged             -= new EventHandler(Param_Changed);
            cbxPermaSLType.SelectedIndexChanged   -= new EventHandler(Param_Changed);
            nudPermaSL.ValueChanged               -= new EventHandler(Param_Changed);
            chbPermaTP.CheckedChanged             -= new EventHandler(Param_Changed);
            cbxPermaTPType.SelectedIndexChanged   -= new EventHandler(Param_Changed);
            nudPermaTP.ValueChanged               -= new EventHandler(Param_Changed);
        }

        void SetLabelPercent()
        {
            string text = Data.Strategy.UseAccountPercentEntry ? "%" : "";
            lblPercent1.Text = text;
            lblPercent2.Text = text;
            lblPercent3.Text = text;
        }

        void CalculateStrategy()
        {
            Backtester.Calculate();
            Backtester.CalculateAccountStats();
        }

        void UpdateChart()
        {
            pnlSmallBalanceChart.InitChart();
            pnlSmallBalanceChart.Invalidate();
        }
    }
}
