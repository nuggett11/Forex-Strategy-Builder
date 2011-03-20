// Forex Strategy Builder - Trading Charges
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
    /// Market Settings
    /// </summary>
    public class Trading_Charges : Form
    {
        Fancy_Panel pnlBase;

        Label lblSpread;
        Label lblSwapLong;
        Label lblSwapShort;
        Label lblCommission;
        Label lblSlippage;

        NumericUpDown nudSpread;
        NumericUpDown nudSwapLong;
        NumericUpDown nudSwapShort;
        NumericUpDown nudCommission;
        NumericUpDown nudSlippage;

        Button btnEditInstrument;
        Button btnAccept;
        Button btnCancel;

        ToolTip toolTip = new ToolTip();

        Font  font;
        Color colorText;

        bool editInstrument = false;

        /// <summary>
        /// Spread
        /// </summary>
        public double Spread
        {
            get { return (double)nudSpread.Value; }
            set { nudSpread.Value = (decimal)value; }
        }

        /// <summary>
        /// Swap Long
        /// </summary>
        public double SwapLong
        {
            get { return (double)nudSwapLong.Value; }
            set { nudSwapLong.Value = (decimal)value; }
        }

        /// <summary>
        /// Swap Short
        /// </summary>
        public double SwapShort
        {
            get { return (double)nudSwapShort.Value; }
            set { nudSwapShort.Value = (decimal)value; }
        }

        /// <summary>
        /// Commission
        /// </summary>
        public double Commission
        {
            get { return (double)nudCommission.Value; }
            set { nudCommission.Value = (decimal)value; }
        }

        /// <summary>
        /// Slippage
        /// </summary>
        public int Slippage
        {
            get { return (int)nudSlippage.Value; }
            set { nudSlippage.Value = value; }
        }

        /// <summary>
        /// Whether to edit the instrument
        /// </summary>
        public bool EditInstrument
        {
            get { return editInstrument; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Trading_Charges()
        {
            pnlBase = new Fancy_Panel();

            lblSpread     = new Label();
            lblSwapLong   = new Label();
            lblSwapShort  = new Label();
            lblCommission = new Label();
            lblSlippage   = new Label();

            nudSpread     = new NumericUpDown();
            nudSwapLong   = new NumericUpDown();
            nudSwapShort  = new NumericUpDown();
            nudCommission = new NumericUpDown();
            nudSlippage   = new NumericUpDown();

            btnEditInstrument = new Button();
            btnAccept = new Button();
            btnCancel = new Button();

            font      = this.Font;
            colorText = LayoutColors.ColorControlText;

            MaximizeBox     = false;
            MinimizeBox     = false;
            ShowInTaskbar   = false;
            Icon            = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton    = btnAccept;
            Text            = Language.T("Trading Charges") + " - " + Data.Symbol;

            // pnlBase
            pnlBase.Parent = this;

            // Label Spread
            lblSpread.Parent    = pnlBase;
            lblSpread.ForeColor = colorText;
            lblSpread.BackColor = Color.Transparent;
            lblSpread.AutoSize  = true;
            lblSpread.Text      = Language.T("Spread") + " [" + Language.T("pips") + "]";

            // Label Swap Long
            lblSwapLong.Parent    = pnlBase;
            lblSwapLong.ForeColor = colorText;
            lblSwapLong.BackColor = Color.Transparent;
            lblSwapLong.AutoSize  = true;
            lblSwapLong.Text      = Language.T("Swap number for a long position rollover") + " [" +
                (Data.InstrProperties.SwapType == Commission_Type.money ?
                Data.InstrProperties.PriceIn :
                Language.T(Data.InstrProperties.SwapType.ToString())) + "]" + Environment.NewLine +
                "(" +  Language.T("A positive value decreases your profit.") +")";

            // Label Swap Short
            lblSwapShort.Parent    = pnlBase;
            lblSwapShort.ForeColor = colorText;
            lblSwapShort.BackColor = Color.Transparent;
            lblSwapShort.AutoSize  = true;
            lblSwapShort.Text      = Language.T("Swap number for a short position rollover") + " [" +
                (Data.InstrProperties.SwapType == Commission_Type.money ?
                Data.InstrProperties.PriceIn :
                Language.T(Data.InstrProperties.SwapType.ToString())) + "]" + Environment.NewLine +
                "(" + Language.T("A negative value decreases your profit.") + ")";

            // Label Commission
            lblCommission.Parent    = pnlBase;
            lblCommission.ForeColor = colorText;
            lblCommission.BackColor = Color.Transparent;
            lblCommission.AutoSize  = true;
            lblCommission.Text = Language.T("Commission in") + " " +
                Data.InstrProperties.CommissionTypeToString  + " " +
                Data.InstrProperties.CommissionScopeToString + " " +
                Data.InstrProperties.CommissionTimeToString  +
                (Data.InstrProperties.CommissionType == Commission_Type.money ? " [" + Data.InstrProperties.PriceIn + "]" : "");

            // Label Slippage
            lblSlippage.Parent    = pnlBase;
            lblSlippage.ForeColor = colorText;
            lblSlippage.BackColor = Color.Transparent;
            lblSlippage.AutoSize  = true;
            lblSlippage.Text      = Language.T("Slippage") + " [" + Language.T("pips") + "]";

            // NumericUpDown Spread
            nudSpread.BeginInit();
            nudSpread.Parent        = pnlBase;
            nudSpread.Name          = Language.T("Spread");
            nudSpread.TextAlign     = HorizontalAlignment.Center;
            nudSpread.Minimum       = 0;
            nudSpread.Maximum       = 500;
            nudSpread.Increment     = 0.01M;
            nudSpread.DecimalPlaces = 2;
            nudSpread.Value         = 4;
            nudSpread.EndInit();
            toolTip.SetToolTip(nudSpread, Language.T("Difference between Bid and Ask prices."));

            // NumericUpDown Swap Long
            nudSwapLong.BeginInit();
            nudSwapLong.Parent        = pnlBase;
            nudSwapLong.Name          = "SwapLong";
            nudSwapLong.TextAlign     = HorizontalAlignment.Center;
            nudSwapLong.Minimum       = -500;
            nudSwapLong.Maximum       = 500;
            nudSwapLong.Increment     = 0.01M;
            nudSwapLong.DecimalPlaces = 2;
            nudSwapLong.Value         = 1;
            nudSwapLong.EndInit();
            toolTip.SetToolTip(nudSwapLong, Language.T("A position changes its average price with the selected number during a rollover."));

            // NumericUpDown Swap Short
            nudSwapShort.BeginInit();
            nudSwapShort.Parent        = pnlBase;
            nudSwapShort.Name          = "SwapShort";
            nudSwapShort.TextAlign     = HorizontalAlignment.Center;
            nudSwapShort.Minimum       = -500;
            nudSwapShort.Maximum       = 500;
            nudSwapShort.Increment     = 0.01M;
            nudSwapShort.DecimalPlaces = 2;
            nudSwapShort.Value         = -1;
            nudSwapShort.EndInit();
            toolTip.SetToolTip(nudSwapShort, Language.T("A position changes its average price with the selected number during a rollover."));

            // NumericUpDown Commission
            nudCommission.BeginInit();
            nudCommission.Parent        = pnlBase;
            nudCommission.Name          = Language.T("Commission");
            nudCommission.TextAlign     = HorizontalAlignment.Center;
            nudCommission.Minimum       = -500;
            nudCommission.Maximum       = 500;
            nudCommission.Increment     = 0.01M;
            nudCommission.DecimalPlaces = 2;
            nudCommission.Value         = 0;
            nudCommission.EndInit();

            // NumericUpDown Slippage
            nudSlippage.BeginInit();
            nudSlippage.Parent    = pnlBase;
            nudSlippage.Name      = "Slippage";
            nudSlippage.TextAlign = HorizontalAlignment.Center;
            nudSlippage.Minimum   = 0;
            nudSlippage.Maximum   = 200;
            nudSlippage.Increment = 1;
            nudSlippage.Value     = 0;
            nudSlippage.EndInit();
            toolTip.SetToolTip(nudSlippage, Language.T("Number of pips you lose due to an inaccurate order execution."));

            //Button btnEditInstrument
            btnEditInstrument.Parent = this;
            btnEditInstrument.Name   = "EditInstrument";
            btnEditInstrument.Text   = Language.T("More");
            btnEditInstrument.Click += new EventHandler(BtnEditInstrument_Click);
            btnEditInstrument.UseVisualStyleBackColor = true;

            //Button Cancel
            btnCancel.Parent       = this;
            btnCancel.Text         = Language.T("Cancel");
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.UseVisualStyleBackColor = true;

            //Button Accept
            btnAccept.Parent       = this;
            btnAccept.Name         = "Accept";
            btnAccept.Text         = Language.T("Accept");
            btnAccept.DialogResult = DialogResult.OK;
            btnAccept.UseVisualStyleBackColor = true;

            return;
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ClientSize = new Size(350, 208);

            btnAccept.Focus();

            return;
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
            int border       = 2;
            int textHeight   = Font.Height;
            int nudWidth     = 70;

            // pnlBase
            pnlBase.Size     = new Size(ClientSize.Width - 2 * space, ClientSize.Height - 2 * btnVertSpace - buttonHeight - space);
            pnlBase.Location = new Point(space, space);

            // Labels
            lblSpread.Location     = new Point(btnHrzSpace + border, 0 * buttonHeight + 1 * space + 8);
            lblSwapLong.Location   = new Point(btnHrzSpace + border, 1 * buttonHeight + 2 * space + 2);
            lblSwapShort.Location  = new Point(btnHrzSpace + border, 2 * buttonHeight + 3 * space + 2);
            lblCommission.Location = new Point(btnHrzSpace + border, 3 * buttonHeight + 4 * space + 8);
            lblSlippage.Location   = new Point(btnHrzSpace + border, 4 * buttonHeight + 5 * space + 8);

            // NUD Params
            int nudLeft = pnlBase.ClientSize.Width - nudWidth - btnHrzSpace - border;
            nudSpread.Size          = new Size(nudWidth, buttonHeight);
            nudSpread.Location      = new Point(nudLeft, 0 * buttonHeight + 1 * space + 6);
            nudSwapLong.Size        = new Size(nudWidth, buttonHeight);
            nudSwapLong.Location    = new Point(nudLeft, 1 * buttonHeight + 2 * space + 6);
            nudSwapShort.Size       = new Size(nudWidth, buttonHeight);
            nudSwapShort.Location   = new Point(nudLeft, 2 * buttonHeight + 3 * space + 6);
            nudCommission.Size      = new Size(nudWidth, buttonHeight);
            nudCommission.Location  = new Point(nudLeft, 3 * buttonHeight + 4 * space + 6);
            nudSlippage.Size        = new Size(nudWidth, buttonHeight);
            nudSlippage.Location    = new Point(nudLeft, 4 * buttonHeight + 5 * space + 6);

            // Button btnEditInstrument
            btnEditInstrument.Size     = new Size(buttonWidth, buttonHeight);
            btnEditInstrument.Location = new Point(btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Cancel
            btnCancel.Size     = new Size(buttonWidth, buttonHeight);
            btnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            btnAccept.Size     = new Size(buttonWidth, buttonHeight);
            btnAccept.Location = new Point(btnCancel.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Default
            //btnDefault.Size     = new Size(buttonWidth, buttonHeight);
            //btnDefault.Location = new Point(btnAccept.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Resize if necessary
            int iMaxLblRight = lblSpread.Right;
            if (lblSwapLong.Right   > iMaxLblRight) iMaxLblRight = lblSwapLong.Right;
            if (lblSwapShort.Right  > iMaxLblRight) iMaxLblRight = lblSwapShort.Right;
            if (lblCommission.Right > iMaxLblRight) iMaxLblRight = lblCommission.Right;
            if (lblSlippage.Right   > iMaxLblRight) iMaxLblRight = lblSlippage.Right;

            if (nudLeft - iMaxLblRight < btnVertSpace)
                Width += btnVertSpace - nudLeft + iMaxLblRight;

            return;
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);

            return;
        }

        /// <summary>
        /// Button Default Click
        /// <summary>
        void BtnDefault_Click(object sender, EventArgs e)
        {
            Instrument_Properties ip = new Instrument_Properties(Data.InstrProperties.Symbol, Data.InstrProperties.InstrType);

            nudSpread.Value     = (decimal)ip.Spread;
            nudSwapLong.Value   = (decimal)ip.SwapLong;
            nudSwapShort.Value  = (decimal)ip.SwapShort;
            nudSlippage.Value   = (decimal)ip.Slippage;
            nudCommission.Value = (decimal)ip.Commission;

            return;
        }

        /// <summary>
        /// Shows Instrument Editor
        /// </summary>
        void BtnEditInstrument_Click(object sender, EventArgs e)
        {
            editInstrument = true;
            this.Close();

            return;
        }

    }
}
