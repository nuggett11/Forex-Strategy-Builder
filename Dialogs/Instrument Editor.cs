// Forex Strategy Builder - Instrument_Editor
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Instrument Editor
    /// </summary>
    public class Instrument_Editor : Form
    {
        Fancy_Panel pnlInstruments;
        Fancy_Panel pnlProperties;
        Fancy_Panel pnlAddInstrument;

        // Instruments' controls
        ListBox lbxInstruments;
        Button  btnUp;
        Button  btnDown;
        Button  btnDelete;

        // Properties' controls
        Label lblPropSymbol;
        Label lblPropType;
        Label lblPropComment;
        Label lblPropDigits;
        Label lblPropPoint;
        Label lblPropLots;
        Label lblPropSpread;
        Label lblPropSwap;
        Label lblPropCommission;
        Label lblPropSlippage;
        Label lblPropPriceIn;
        Label lblPropAccountIn;
        Label lblPropAccountRate;
        Label lblPropFileName;
        Label lblPropDataFiles;

        TextBox tbxPropSymbol;
        TextBox tbxPropType;
        TextBox tbxPropComment;
        TextBox tbxPropPoint;
        TextBox tbxPropSpread;
        TextBox tbxPropSlippage;
        TextBox tbxPropPriceIn;
        TextBox tbxPropAccountIn;
        TextBox tbxPropAccountRate;
        TextBox tbxPropFileName;

        ComboBox cbxPropSwap;
        ComboBox cbxPropCommission;
        ComboBox cbxPropCommScope;
        ComboBox cbxPropCommTime;

        NumericUpDown nudPropDigits;
        NumericUpDown nudPropLotSize;
        NumericUpDown nudPropSpread;
        NumericUpDown nudPropSwapLong;
        NumericUpDown nudPropSwapShort;
        NumericUpDown nudPropCommission;
        NumericUpDown nudPropSlippage;
        NumericUpDown nudPropAccountRate;

        Button  btnAccept;

        // Add an Instrument's controls
        Label    lblAddInstrSymbol;
        TextBox  tbxAddInstrSymbol;
        Label    lblAddInstrType;
        ComboBox cbxAddInstrType;
        Button   btnAddInstrAdd;

        Button  btnClose;

        ToolTip toolTip = new ToolTip();

        Font  font;
        Font  fontCaption;
        float captionHeight;
        Color colorText;
        bool  bNeedReset;

        Instrument_Properties instrPropSelectedInstrument;

        /// <summary>
        /// If a reset is necessary.
        /// </summary>
        public bool NeedReset { get { return bNeedReset; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public Instrument_Editor()
        {
            pnlInstruments   = new Fancy_Panel(Language.T("Instruments"));
            pnlProperties    = new Fancy_Panel(Language.T("Instrument Properties"));
            pnlAddInstrument = new Fancy_Panel(Language.T("Add an Instrument"));

            // Instruments' controls
            lbxInstruments = new ListBox();
            btnDelete      = new Button();
            btnUp          = new Button();
            btnDown        = new Button();

            // Properties' controls
            lblPropSymbol      = new Label();
            lblPropType        = new Label();
            lblPropComment     = new Label();
            lblPropDigits      = new Label();
            lblPropPoint       = new Label();
            lblPropLots        = new Label();
            lblPropSpread      = new Label();
            lblPropSwap        = new Label();
            lblPropCommission  = new Label();
            lblPropSlippage    = new Label();
            lblPropPriceIn     = new Label();
            lblPropAccountIn   = new Label();
            lblPropAccountRate = new Label();
            lblPropFileName    = new Label();
            lblPropDataFiles   = new Label();

            tbxPropSymbol      = new TextBox();
            tbxPropType        = new TextBox();
            tbxPropComment     = new TextBox();
            tbxPropPoint       = new TextBox();
            tbxPropSpread      = new TextBox();
            tbxPropSlippage    = new TextBox();
            tbxPropPriceIn     = new TextBox();
            tbxPropAccountIn   = new TextBox();
            tbxPropAccountRate = new TextBox();
            tbxPropFileName    = new TextBox();

            cbxPropSwap       = new ComboBox();
            cbxPropCommission = new ComboBox();
            cbxPropCommScope  = new ComboBox();
            cbxPropCommTime   = new ComboBox();

            nudPropDigits      = new NumericUpDown();
            nudPropLotSize     = new NumericUpDown();
            nudPropSpread      = new NumericUpDown();
            nudPropSwapLong    = new NumericUpDown();
            nudPropSwapShort   = new NumericUpDown();
            nudPropCommission  = new NumericUpDown();
            nudPropSlippage    = new NumericUpDown();
            nudPropAccountRate = new NumericUpDown();

            btnAccept = new Button();

            // Add an Instrument's controls
            lblAddInstrSymbol = new Label();
            lblAddInstrType   = new Label();
            tbxAddInstrSymbol = new TextBox();
            cbxAddInstrType   = new ComboBox();
            btnAddInstrAdd    = new Button();

            btnClose = new Button();

            font           = this.Font;
            fontCaption    = new Font(Font.FontFamily, 9);
            captionHeight = (float)Math.Max(fontCaption.Height, 18);
            colorText      = LayoutColors.ColorControlText;
            bNeedReset     = false;

            MaximizeBox     = false;
            MinimizeBox     = false;
            ShowInTaskbar   = false;
            Icon            = Data.Icon;
            BackColor       = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton    = btnAccept;
            Text            = Language.T("Instrument Editor");
            FormClosing    += new FormClosingEventHandler(Instrument_Editor_FormClosing);

            // pnlInstruments
            pnlInstruments.Parent = this;

            // pnlProperties
            pnlProperties.Parent = this;

            // pnlAddInstrument
            pnlAddInstrument.Parent = this;

            // lbxInstruments
            lbxInstruments.Parent = pnlInstruments;
            lbxInstruments.BackColor = LayoutColors.ColorControlBack;
            //lbxInstruments.BorderStyle = BorderStyle.None;
            lbxInstruments.ForeColor = colorText;
            lbxInstruments.Items.AddRange(Instruments.SymbolList);

            // Button UP
            btnUp.Parent = pnlInstruments;
            btnUp.Text   = Language.T("Up");
            btnUp.UseVisualStyleBackColor = true;
            btnUp.Click += new EventHandler(BtnUp_Click);

            // Button Down
            btnDown.Parent = pnlInstruments;
            btnDown.Text   = Language.T("Down");
            btnDown.UseVisualStyleBackColor = true;
            btnDown.Click += new EventHandler(BtnDown_Click);

            // Button Delete
            btnDelete.Parent = pnlInstruments;
            btnDelete.Text   = Language.T("Delete");
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += new EventHandler(BtnDelete_Click);

            // lblAddInstrSymbol
            lblAddInstrSymbol.Parent    = pnlAddInstrument;
            lblAddInstrSymbol.ForeColor = colorText;
            lblAddInstrSymbol.BackColor = Color.Transparent;
            lblAddInstrSymbol.AutoSize  = false;
            lblAddInstrSymbol.TextAlign = ContentAlignment.MiddleRight;
            lblAddInstrSymbol.Text      = Language.T("Symbol");

            // tbxAddInstrSymbol
            tbxAddInstrSymbol.Parent    = pnlAddInstrument;
            tbxAddInstrSymbol.ForeColor = colorText;

            // lblAddInstrType
            lblAddInstrType.Parent    = pnlAddInstrument;
            lblAddInstrType.ForeColor = colorText;
            lblAddInstrType.BackColor = Color.Transparent;
            lblAddInstrType.AutoSize  = false;
            lblAddInstrType.TextAlign = ContentAlignment.MiddleRight;
            lblAddInstrType.Text      = Language.T("Type");

            // cbxAddInstrType
            cbxAddInstrType.Parent        = pnlAddInstrument;
            cbxAddInstrType.Name          = "cbxAddInstrType";
            cbxAddInstrType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxAddInstrType.Items.AddRange(Enum.GetNames(typeof(Instrumet_Type)));
            cbxAddInstrType.SelectedIndex = 0;

            // btnAddInstrAdd
            btnAddInstrAdd.Parent = pnlAddInstrument;
            btnAddInstrAdd.Name   = "btnAddInstrAdd";
            btnAddInstrAdd.Text   = Language.T("Add");
            btnAddInstrAdd.UseVisualStyleBackColor = true;
            btnAddInstrAdd.Click += new EventHandler(BtnAddInstrAdd_Click);

            // pnlProperties
            lblPropSymbol.Parent    = pnlProperties;
            lblPropSymbol.ForeColor = colorText;
            lblPropSymbol.BackColor = Color.Transparent;
            lblPropSymbol.AutoSize  = false;
            lblPropSymbol.TextAlign = ContentAlignment.MiddleRight;
            lblPropSymbol.Text      = Language.T("Symbol");

            // lblPropType
            lblPropType.Parent    = pnlProperties;
            lblPropType.ForeColor = colorText;
            lblPropType.BackColor = Color.Transparent;
            lblPropType.AutoSize  = false;
            lblPropType.TextAlign = ContentAlignment.MiddleRight;
            lblPropType.Text      = Language.T("Type");

            // lblPropComment
            lblPropComment.Parent    = pnlProperties;
            lblPropComment.ForeColor = colorText;
            lblPropComment.BackColor = Color.Transparent;
            lblPropComment.AutoSize  = false;
            lblPropComment.TextAlign = ContentAlignment.MiddleRight;
            lblPropComment.Text      = Language.T("Comment");

            // lblPropDigits
            lblPropDigits.Parent    = pnlProperties;
            lblPropDigits.ForeColor = colorText;
            lblPropDigits.BackColor = Color.Transparent;
            lblPropDigits.AutoSize  = false;
            lblPropDigits.TextAlign = ContentAlignment.MiddleRight;
            lblPropDigits.Text      = Language.T("Digits");

            // lblPropPoint
            lblPropPoint.Parent    = pnlProperties;
            lblPropPoint.ForeColor = colorText;
            lblPropPoint.BackColor = Color.Transparent;
            lblPropPoint.AutoSize  = false;
            lblPropPoint.TextAlign = ContentAlignment.MiddleRight;
            lblPropPoint.Text      = Language.T("Point value");

            // lblPropLots
            lblPropLots.Parent    = pnlProperties;
            lblPropLots.ForeColor = colorText;
            lblPropLots.BackColor = Color.Transparent;
            lblPropLots.AutoSize  = false;
            lblPropLots.TextAlign = ContentAlignment.MiddleRight;
            lblPropLots.Text      = Language.T("Lot size");

            // lblPropSpread
            lblPropSpread.Parent    = pnlProperties;
            lblPropSpread.ForeColor = colorText;
            lblPropSpread.BackColor = Color.Transparent;
            lblPropSpread.AutoSize  = false;
            lblPropSpread.TextAlign = ContentAlignment.MiddleRight;
            lblPropSpread.Text      = Language.T("Spread in");

            // lblPropSwap
            lblPropSwap.Parent    = pnlProperties;
            lblPropSwap.ForeColor = colorText;
            lblPropSwap.BackColor = Color.Transparent;
            lblPropSwap.AutoSize  = false;
            lblPropSwap.TextAlign = ContentAlignment.MiddleRight;
            lblPropSwap.Text      = Language.T("Swap in");

            // lblPropCommission
            lblPropCommission.Parent    = pnlProperties;
            lblPropCommission.ForeColor = colorText;
            lblPropCommission.BackColor = Color.Transparent;
            lblPropCommission.AutoSize  = false;
            lblPropCommission.TextAlign = ContentAlignment.MiddleRight;
            lblPropCommission.Text      = Language.T("Commission in");

            // lblPropSlippage
            lblPropSlippage.Parent    = pnlProperties;
            lblPropSlippage.ForeColor = colorText;
            lblPropSlippage.BackColor = Color.Transparent;
            lblPropSlippage.AutoSize  = false;
            lblPropSlippage.TextAlign = ContentAlignment.MiddleRight;
            lblPropSlippage.Text      = Language.T("Slippage in");

            // lblPropPriceIn
            lblPropPriceIn.Parent    = pnlProperties;
            lblPropPriceIn.ForeColor = colorText;
            lblPropPriceIn.BackColor = Color.Transparent;
            lblPropPriceIn.AutoSize  = false;
            lblPropPriceIn.TextAlign = ContentAlignment.MiddleRight;
            lblPropPriceIn.Text      = Language.T("Price in");

            // lblPropAccountIn
            lblPropAccountIn.Parent    = pnlProperties;
            lblPropAccountIn.ForeColor = colorText;
            lblPropAccountIn.BackColor = Color.Transparent;
            lblPropAccountIn.AutoSize  = false;
            lblPropAccountIn.TextAlign = ContentAlignment.MiddleRight;
            lblPropAccountIn.Text      = Language.T("Account in");

            // lblPropAccountRate
            lblPropAccountRate.Parent    = pnlProperties;
            lblPropAccountRate.ForeColor = colorText;
            lblPropAccountRate.BackColor = Color.Transparent;
            lblPropAccountRate.AutoSize  = false;
            lblPropAccountRate.TextAlign = ContentAlignment.MiddleRight;
            lblPropAccountRate.Text      = Language.T("Account exchange rate");

            // lblPropFileName
            lblPropFileName.Parent    = pnlProperties;
            lblPropFileName.BackColor = Color.Transparent;
            lblPropFileName.ForeColor = colorText;
            lblPropFileName.AutoSize  = false;
            lblPropFileName.TextAlign = ContentAlignment.MiddleRight;
            lblPropFileName.Text      = Language.T("Base name of the data files");

            // lblPropDataFiles
            lblPropDataFiles.Parent    = pnlProperties;
            lblPropDataFiles.BackColor = Color.Transparent;
            lblPropDataFiles.ForeColor = colorText;
            lblPropDataFiles.AutoSize  = false;
            lblPropDataFiles.TextAlign = ContentAlignment.TopLeft;
            lblPropDataFiles.Text      = "";

            // tbxPropSymbol
            tbxPropSymbol.Parent    = pnlProperties;
            tbxPropSymbol.BackColor = LayoutColors.ColorControlBack;
            tbxPropSymbol.ForeColor = colorText;
            tbxPropSymbol.Enabled   = false;

            // tbxPropType
            tbxPropType.Parent    = pnlProperties;
            tbxPropType.BackColor = LayoutColors.ColorControlBack;
            tbxPropType.ForeColor = colorText;
            tbxPropType.Enabled   = false;

            // tbxPropComment
            tbxPropComment.Parent    = pnlProperties;
            tbxPropComment.BackColor = LayoutColors.ColorControlBack;
            tbxPropComment.ForeColor = colorText;

            // tbxPropPoint
            tbxPropPoint.Parent    = pnlProperties;
            tbxPropPoint.BackColor = LayoutColors.ColorControlBack;
            tbxPropPoint.ForeColor = colorText;
            tbxPropPoint.Enabled   = false;

            // tbxPropSpread
            tbxPropSpread.Parent    = pnlProperties;
            tbxPropSpread.BackColor = LayoutColors.ColorControlBack;
            tbxPropSpread.ForeColor = colorText;
            tbxPropSpread.Enabled   = false;
            tbxPropSpread.Text      = Language.T("pips");

            // tbxPropSlippage
            tbxPropSlippage.Parent    = pnlProperties;
            tbxPropSlippage.BackColor = LayoutColors.ColorControlBack;
            tbxPropSlippage.ForeColor = colorText;
            tbxPropSlippage.Enabled   = false;
            tbxPropSlippage.Text      = Language.T("pips");

            // tbxPropPriceIn
            tbxPropPriceIn.Parent       = pnlProperties;
            tbxPropPriceIn.BackColor    = LayoutColors.ColorControlBack;
            tbxPropPriceIn.ForeColor    = colorText;
            tbxPropPriceIn.TextChanged += new EventHandler(TbxPropPriceIn_TextChanged);

            // tbxPropAccountIn
            tbxPropAccountIn.Parent    = pnlProperties;
            tbxPropAccountIn.BackColor = LayoutColors.ColorControlBack;
            tbxPropAccountIn.ForeColor = colorText;
            tbxPropAccountIn.Enabled   = false;
            tbxPropAccountIn.Text      = Configs.AccountCurrency;

            // tbxPropAccountRate
            tbxPropAccountRate.Parent    = pnlProperties;
            tbxPropAccountRate.BackColor = LayoutColors.ColorControlBack;
            tbxPropAccountRate.ForeColor = colorText;
            tbxPropAccountRate.Enabled   = false;
            tbxPropAccountRate.Text      = "Deal price";

            // tbxPropFileName
            tbxPropFileName.Parent       = pnlProperties;
            tbxPropFileName.BackColor    = LayoutColors.ColorControlBack;
            tbxPropFileName.ForeColor    = colorText;
            tbxPropFileName.TextChanged += new EventHandler(TbxPropFileName_TextChanged);

            // cbxPropSwap
            cbxPropSwap.Parent        = pnlProperties;
            cbxPropSwap.Name          = "cbxPropSwap";
            cbxPropSwap.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxPropSwap.Items.AddRange(new string[] { Language.T("pips"), Language.T("percents"), Language.T("money") });
            cbxPropSwap.SelectedIndex = 0;

            // cbxPropCommission
            cbxPropCommission.Parent        = pnlProperties;
            cbxPropCommission.Name          = "cbxPropCommission";
            cbxPropCommission.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxPropCommission.Items.AddRange(new string[] { Language.T("pips"), Language.T("percents"), Language.T("money") });
            cbxPropCommission.SelectedIndex = 0;
            cbxPropCommission.SelectedIndexChanged += new EventHandler(CbxPropCommission_SelectedIndexChanged);

            // cbxPropCommScope
            cbxPropCommScope.Parent        = pnlProperties;
            cbxPropCommScope.Name          = "cbxPropCommScope";
            cbxPropCommScope.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxPropCommScope.Items.AddRange(new string[] { Language.T("per lot"), Language.T("per deal") });
            cbxPropCommScope.SelectedIndex = 0;

            // cbxPropCommTime
            cbxPropCommTime.Parent        = pnlProperties;
            cbxPropCommTime.Name          = "cbxPropCommTime";
            cbxPropCommTime.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxPropCommTime.Items.AddRange(new string[] { Language.T("at opening"), Language.T("at open/close")});
            cbxPropCommTime.SelectedIndex = 0;

            // NumericUpDown Digits
            nudPropDigits.BeginInit();
            nudPropDigits.Parent    = pnlProperties;
            nudPropDigits.Name      = "nudPropDigits";
            nudPropDigits.Minimum   = 0;
            nudPropDigits.Maximum   = 5;
            nudPropDigits.Increment = 1;
            nudPropDigits.Value     = 4;
            nudPropDigits.TextAlign = HorizontalAlignment.Center;
            nudPropDigits.ValueChanged += new EventHandler(NudPropDigits_ValueChanged);
            nudPropDigits.EndInit();

            // nudPropLotSize
            nudPropLotSize.BeginInit();
            nudPropLotSize.Parent    = pnlProperties;
            nudPropLotSize.Name      = "nudPropLotSize";
            nudPropLotSize.Minimum   = 0;
            nudPropLotSize.Maximum   = 100000;
            nudPropLotSize.Increment = 1;
            nudPropLotSize.Value     = 10000;
            nudPropLotSize.TextAlign = HorizontalAlignment.Center;
            nudPropLotSize.EndInit();

            // nudPropSpread
            nudPropSpread.BeginInit();
            nudPropSpread.Parent        = pnlProperties;
            nudPropSpread.Name          = "nudPropSpread";
            nudPropSpread.TextAlign     = HorizontalAlignment.Center;
            nudPropSpread.Minimum       = 0;
            nudPropSpread.Maximum       = 500;
            nudPropSpread.Increment     = 0.01M;
            nudPropSpread.DecimalPlaces = 2;
            nudPropSpread.Value         = 4;
            nudPropSpread.EndInit();
            toolTip.SetToolTip(nudPropSpread, Language.T("Difference between Bid and Ask prices."));

            // NumericUpDown Swap Long
            nudPropSwapLong.BeginInit();
            nudPropSwapLong.Parent        = pnlProperties;
            nudPropSwapLong.Name          = "nudPropSwapLong";
            nudPropSwapLong.TextAlign     = HorizontalAlignment.Center;
            nudPropSwapLong.Minimum       = -500;
            nudPropSwapLong.Maximum       = 500;
            nudPropSwapLong.Increment     = 0.01M;
            nudPropSwapLong.DecimalPlaces = 2;
            nudPropSwapLong.Value         = 1;
            nudPropSwapLong.EndInit();
            toolTip.SetToolTip(nudPropSwapLong, Language.T("Swap number for a long position rollover") + Environment.NewLine + Language.T("A positive value decreases your profit."));

            // NumericUpDown Swap Short
            nudPropSwapShort.BeginInit();
            nudPropSwapShort.Parent        = pnlProperties;
            nudPropSwapShort.Name          = "nudPropSwapShort";
            nudPropSwapShort.TextAlign     = HorizontalAlignment.Center;
            nudPropSwapShort.Minimum       = -500;
            nudPropSwapShort.Maximum       = 500;
            nudPropSwapShort.Increment     = 0.01M;
            nudPropSwapShort.DecimalPlaces = 2;
            nudPropSwapShort.Value         = -1;
            nudPropSwapShort.EndInit();
            toolTip.SetToolTip(nudPropSwapShort, Language.T("Swap number for a short position rollover") + Environment.NewLine + Language.T("A negative value decreases your profit."));

            // NumericUpDown nudPropCommission
            nudPropCommission.BeginInit();
            nudPropCommission.Parent        = pnlProperties;
            nudPropCommission.Name          = "nudPropCommission";
            nudPropCommission.TextAlign     = HorizontalAlignment.Center;
            nudPropCommission.Minimum       = -500;
            nudPropCommission.Maximum       = 500;
            nudPropCommission.Increment     = 0.01M;
            nudPropCommission.DecimalPlaces = 2;
            nudPropCommission.Value         = 0;
            nudPropCommission.EndInit();

            // NumericUpDown nudPropSlippage
            nudPropSlippage.BeginInit();
            nudPropSlippage.Parent        = pnlProperties;
            nudPropSlippage.Name          = "nudPropSlippage";
            nudPropSlippage.TextAlign     = HorizontalAlignment.Center;
            nudPropSlippage.Minimum       = 0;
            nudPropSlippage.Maximum       = 200;
            nudPropSlippage.Increment     = 1;
            nudPropSlippage.DecimalPlaces = 0;
            nudPropSlippage.Value         = 0;
            nudPropSlippage.EndInit();
            toolTip.SetToolTip(nudPropSlippage, Language.T("Number of pips you lose due to an inaccurate order execution."));

            // NumericUpDown nudPropAccountRate
            nudPropAccountRate.BeginInit();
            nudPropAccountRate.Parent        = pnlProperties;
            nudPropAccountRate.Name          = "nudPropAccountRate";
            nudPropAccountRate.TextAlign     = HorizontalAlignment.Center;
            nudPropAccountRate.Minimum       = 0;
            nudPropAccountRate.Maximum       = 100000;
            nudPropAccountRate.Increment     = 0.0001M;
            nudPropAccountRate.DecimalPlaces = 4;
            nudPropAccountRate.Value         = 1;
            nudPropAccountRate.ValueChanged += new EventHandler(nudPropAccountRate_ValueChanged);
            nudPropAccountRate.EndInit();

            // Button Accept
            btnAccept.Parent = pnlProperties;
            btnAccept.Name   = "btnAccept";
            btnAccept.Text   = Language.T("Accept");
            btnAccept.Click += new EventHandler(BtnAccept_Click);
            btnAccept.UseVisualStyleBackColor = true;

            //Button Close
            btnClose.Parent       = this;
            btnClose.Text         = Language.T("Close");
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.UseVisualStyleBackColor = true;
        }

        void nudPropAccountRate_ValueChanged(object sender, EventArgs e)
        {
            nudPropAccountRate.ForeColor = Color.Black;
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            int buttonWidth = (int)(Data.HorizontalDLU * 65);
            int btnHrzSpace = (int)(Data.HorizontalDLU * 3);

            ClientSize = new Size(6 * buttonWidth + 11 * btnHrzSpace + 4, 540);

            lbxInstruments.SelectedValueChanged += new EventHandler(LbxInstruments_SelectedValueChanged);
            lbxInstruments.SelectedIndex = lbxInstruments.Items.IndexOf(Data.Symbol);

            btnClose.Focus();
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth  = (int)(Data.HorizontalDLU * 65);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int space        = btnHrzSpace;
            int border       = 2;
            int textHeight   = Font.Height;

            // Button Close
            btnClose.Size     = new Size(buttonWidth, buttonHeight);
            btnClose.Location = new Point(ClientSize.Width - buttonWidth - 2 * space - 1, ClientSize.Height - buttonHeight - btnVertSpace);

            // pnlInstruments
            pnlInstruments.Size     = new Size(buttonWidth + 2 * space + 2, ClientSize.Height - 2 * space);
            pnlInstruments.Location = new Point(space, space);

            // pnlAddInstrument
            pnlAddInstrument.Size     = new Size(ClientSize.Width - 2 * space - pnlInstruments.Right, buttonHeight + 2 * space + (int)captionHeight + 2);
            pnlAddInstrument.Location = new Point(pnlInstruments.Right + space, btnClose.Top - btnVertSpace - pnlAddInstrument.Height);

            // pnlProperties
            pnlProperties.Size     = new Size(ClientSize.Width - 2 * space - pnlInstruments.Right, pnlAddInstrument.Top - 2 * space);
            pnlProperties.Location = new Point(pnlInstruments.Right + space, space);

            // Button Delete
            btnDelete.Size     = new Size(buttonWidth, buttonHeight);
            btnDelete.Location = new Point(btnHrzSpace, pnlInstruments.ClientSize.Height - buttonHeight - space);

            // Button Up
            btnUp.Size     = new Size((buttonWidth - space) / 2, buttonHeight);
            btnUp.Location = new Point(btnHrzSpace, btnDelete.Top - buttonHeight - space);

            // Button Down
            btnDown.Size     = new Size((buttonWidth - space) / 2, buttonHeight);
            btnDown.Location = new Point(btnUp.Right + btnHrzSpace, btnDelete.Top - buttonHeight - space);

            // lbxInstruments
            lbxInstruments.Size     = new Size(pnlInstruments.ClientSize.Width - 2 * btnHrzSpace - 2 * border, btnUp.Top - space - (int)captionHeight);
            lbxInstruments.Location = new Point(space + border, space + (int)captionHeight);

            // Properties' controls
            lblPropSymbol.Width      = buttonWidth;
            lblPropType.Width        = buttonWidth;
            lblPropComment.Width     = buttonWidth;
            lblPropDigits.Width      = buttonWidth;
            lblPropPoint.Width       = buttonWidth;
            lblPropLots.Width        = buttonWidth;
            lblPropSpread.Width      = buttonWidth;
            lblPropSwap.Width        = buttonWidth;
            lblPropCommission.Width  = buttonWidth;
            lblPropSlippage.Width    = buttonWidth;
            lblPropPriceIn.Width     = buttonWidth;
            lblPropAccountIn.Width   = buttonWidth;
            lblPropAccountRate.Width = 2 * buttonWidth + space;
            lblPropFileName.Width    = 2 * buttonWidth + space;
            lblPropDataFiles.Width   = 4 * buttonWidth + 3 * space;
            lblPropDataFiles.Height  = 2 * buttonWidth + 1 * space;
            lblPropSymbol.Location      = new Point(X(1), Y(1) + 1);
            lblPropType.Location        = new Point(X(3), Y(1) + 1);
            lblPropComment.Location     = new Point(X(1), Y(2) + 1);
            lblPropDigits.Location      = new Point(X(1), Y(3) + 1);
            lblPropPoint.Location       = new Point(X(3), Y(3) + 1);
            lblPropLots.Location        = new Point(X(1), Y(4) + 1);
            lblPropSpread.Location      = new Point(X(1), Y(5) + 1);
            lblPropSwap.Location        = new Point(X(1), Y(6) + 1);
            lblPropCommission.Location  = new Point(X(1), Y(7) + 1);
            lblPropSlippage.Location    = new Point(X(1), Y(8) + 1);
            lblPropPriceIn.Location     = new Point(X(1), Y(9) + 1);
            lblPropAccountIn.Location   = new Point(X(3), Y(9) + 1);
            lblPropAccountRate.Location = new Point(X(1), Y(10) + 1);
            lblPropFileName.Location    = new Point(X(1), Y(11) + 1);
            lblPropDataFiles.Location   = new Point(X(1), Y(12) + 1);

            tbxPropSymbol.Width      = buttonWidth;
            tbxPropType.Width        = buttonWidth;
            tbxPropComment.Width     = 3 * buttonWidth + 2 * space; ;
            tbxPropSpread.Width      = buttonWidth;
            tbxPropPriceIn.Width     = buttonWidth;
            tbxPropPoint.Width       = buttonWidth;
            tbxPropSlippage.Width    = buttonWidth;
            tbxPropAccountIn.Width   = buttonWidth;
            tbxPropAccountRate.Width = buttonWidth;
            tbxPropFileName.Width    = buttonWidth;
            tbxPropSymbol.Location      = new Point(X(2), Y(1) + 3);
            tbxPropType.Location        = new Point(X(4), Y(1) + 2);
            tbxPropComment.Location     = new Point(X(2), Y(2) + 3);
            tbxPropPoint.Location       = new Point(X(4), Y(3) + 3);
            tbxPropSpread.Location      = new Point(X(2), Y(5) + 2);
            tbxPropSlippage.Location    = new Point(X(2), Y(8) + 3);
            tbxPropPriceIn.Location     = new Point(X(2), Y(9) + 3);
            tbxPropAccountIn.Location   = new Point(X(4), Y(9) + 3);
            tbxPropAccountRate.Location = new Point(X(3), Y(10) + 3);
            tbxPropFileName.Location    = new Point(X(3), Y(11) + 3);

            cbxPropSwap.Width       = buttonWidth;
            cbxPropCommission.Width = buttonWidth;
            cbxPropCommScope.Width  = buttonWidth;
            cbxPropCommTime.Width   = buttonWidth;
            cbxPropSwap.Location       = new Point(X(2), Y(6) + 2);
            cbxPropCommission.Location = new Point(X(2), Y(7) + 2);
            cbxPropCommScope.Location  = new Point(X(3), Y(7) + 2);
            cbxPropCommTime.Location   = new Point(X(4), Y(7) + 2);

            nudPropDigits.Width      = buttonWidth;
            nudPropLotSize.Width     = buttonWidth;
            nudPropSpread.Width      = buttonWidth;
            nudPropSwapLong.Width    = buttonWidth;
            nudPropSwapShort.Width   = buttonWidth;
            nudPropCommission.Width  = buttonWidth;
            nudPropSlippage.Width    = buttonWidth;
            nudPropAccountRate.Width = buttonWidth;
            nudPropDigits.Location      = new Point(X(2), Y(3) + 4);
            nudPropLotSize.Location     = new Point(X(2), Y(4) + 4);
            nudPropSpread.Location      = new Point(X(3), Y(5) + 4);
            nudPropSwapLong.Location    = new Point(X(3), Y(6) + 4);
            nudPropSwapShort.Location   = new Point(X(4), Y(6) + 4);
            nudPropCommission.Location  = new Point(X(5), Y(7) + 4);
            nudPropSlippage.Location    = new Point(X(3), Y(8) + 4);
            nudPropAccountRate.Location = new Point(X(4), Y(10) + 4);

            // pnlAddInstrument's controls
            lblAddInstrSymbol.Width    = buttonWidth;
            lblAddInstrSymbol.Location = new Point(X(1), Y(1) + 1);
            tbxAddInstrSymbol.Width    = buttonWidth;
            tbxAddInstrSymbol.Location = new Point(X(2), Y(1) + 3);
            lblAddInstrType.Width      = buttonWidth;
            lblAddInstrType.Location   = new Point(X(3), Y(1) + 1);
            cbxAddInstrType.Width      = buttonWidth;
            cbxAddInstrType.Location   = new Point(X(4), Y(1) + 2);
            btnAddInstrAdd.Size        = new Size(buttonWidth, buttonHeight);
            btnAddInstrAdd.Location    = new Point(X(5), Y(1));

            // Button Accept
            btnAccept.Size     = new Size(buttonWidth, buttonHeight);
            btnAccept.Location = new Point(pnlProperties.ClientSize.Width - buttonWidth - space, pnlProperties.ClientSize.Height - buttonHeight - space);

            return;
        }

        /// <summary>
        /// Gives the horizontal position.
        /// </summary>
        int X(int i)
        {
            int buttonWidth = (int)(Data.HorizontalDLU * 65);
            int border      = (int)(Data.HorizontalDLU * 3);
            int iPosition    = i * border + (i - 1) * buttonWidth;

            return iPosition;
        }

        /// <summary>
        /// Gives the vertical position.
        /// </summary>
        int Y(int i)
        {
            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int border       = (int)(Data.HorizontalDLU * 3);
            int iPosition     = (int)captionHeight + i * border + (i - 1) * buttonHeight;

            return iPosition;
        }

        /// <summary>
        /// Validates the instrument properties
        /// </summary>
        bool ValidatePropertiesForm()
        {
            bool   isResult = true;
            string errorMessage = "";

            // Symbol
            if (!ValidateSymbol(tbxPropSymbol.Text, (Instrumet_Type)Enum.Parse(typeof(Instrumet_Type), tbxPropType.Text)))
                errorMessage += Environment.NewLine + Language.T("Wrong Symbol!");

            // Price In
            Regex regexPriceIn = new Regex(@"^[A-Z]{3}$");

            if (!regexPriceIn.IsMatch(tbxPropPriceIn.Text))
                errorMessage += Environment.NewLine + Language.T("Wrong Price!");

            // Commission
            if(cbxPropCommission.SelectedIndex == 1 && cbxPropCommScope.SelectedIndex != 1)
                errorMessage += Environment.NewLine + Language.T("Wrong commission settings!");


            // Base file name
            Regex regexFileName = new Regex(@"^[a-zA-Z\$\#][\w- ]*$");

            if (!regexFileName.IsMatch(tbxPropFileName.Text))
                errorMessage += Environment.NewLine + Language.T("Wrong Base name of the data files!");

            if(errorMessage != "")
            {
                isResult = false;
                MessageBox.Show(errorMessage, Language.T("Instrument Properties"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            return isResult;
        }

        /// <summary>
        /// Validates the instrument properties
        /// </summary>
        bool ValidateSymbol(string symbol, Instrumet_Type instrType)
        {
            bool isResult = false;

            if (instrType == Instrumet_Type.Forex)
            {
                Regex regex = new Regex(@"^[A-Z]{6}$");
                isResult = (regex.IsMatch(symbol) && symbol.Substring(0,3) != symbol.Substring(3,3));
            }
            else
            {
                Regex regex = new Regex(@"^[a-zA-Z\$\#]");
                isResult = regex.IsMatch(symbol);
            }

            return isResult;
        }

        /// <summary>
        /// Sets the properties form.
        /// </summary>
        void SetPropertiesForm()
        {
            tbxPropSymbol.Text              = instrPropSelectedInstrument.Symbol;
            tbxPropType.Text                = instrPropSelectedInstrument.InstrType.ToString();
            tbxPropComment.Text             = instrPropSelectedInstrument.Comment;
            tbxPropPoint.Text               = (1 / Math.Pow(10, (float)nudPropDigits.Value)).ToString("0.#####");
            tbxPropPriceIn.Text             = instrPropSelectedInstrument.PriceIn;
            tbxPropFileName.Text            = instrPropSelectedInstrument.BaseFileName;
            cbxPropSwap.SelectedIndex       = (int)instrPropSelectedInstrument.SwapType;
            cbxPropCommission.SelectedIndex = (int)instrPropSelectedInstrument.CommissionType;
            cbxPropCommScope.SelectedIndex  = (int)instrPropSelectedInstrument.CommissionScope;
            cbxPropCommTime.SelectedIndex   = (int)instrPropSelectedInstrument.CommissionTime;
            nudPropDigits.Value             = (decimal)instrPropSelectedInstrument.Digits;
            nudPropLotSize.Value            = (decimal)instrPropSelectedInstrument.LotSize;
            nudPropSpread.Value             = (decimal)instrPropSelectedInstrument.Spread;
            nudPropSwapLong.Value           = (decimal)instrPropSelectedInstrument.SwapLong;
            nudPropSwapShort.Value          = (decimal)instrPropSelectedInstrument.SwapShort;
            nudPropCommission.Value         = (decimal)instrPropSelectedInstrument.Commission;
            nudPropSlippage.Value           = (decimal)instrPropSelectedInstrument.Slippage;

            tbxPropPriceIn.Enabled  = instrPropSelectedInstrument.InstrType != Instrumet_Type.Forex;
            tbxPropFileName.Enabled = instrPropSelectedInstrument.InstrType != Instrumet_Type.Forex;

            SetAcountExchangeRate();

            return;
        }

        /// <summary>
        /// Sets the properties form.
        /// </summary>
        void SetSelectedInstrument()
        {
            instrPropSelectedInstrument.Symbol          = tbxPropSymbol.Text;
            instrPropSelectedInstrument.InstrType       = (Instrumet_Type)Enum.Parse(typeof(Instrumet_Type), tbxPropType.Text);
            instrPropSelectedInstrument.Comment         = tbxPropComment.Text;
            instrPropSelectedInstrument.PriceIn         = tbxPropPriceIn.Text;
            instrPropSelectedInstrument.BaseFileName    = tbxPropFileName.Text;
            instrPropSelectedInstrument.SwapType        = (Commission_Type)Enum.GetValues(typeof(Commission_Type)).GetValue(cbxPropSwap.SelectedIndex);
            instrPropSelectedInstrument.CommissionType  = (Commission_Type)Enum.GetValues(typeof(Commission_Type)).GetValue(cbxPropCommission.SelectedIndex);
            instrPropSelectedInstrument.CommissionScope = (Commission_Scope)Enum.GetValues(typeof(Commission_Scope)).GetValue(cbxPropCommScope.SelectedIndex);
            instrPropSelectedInstrument.CommissionTime  = (Commission_Time)Enum.GetValues(typeof(Commission_Time)).GetValue(cbxPropCommTime.SelectedIndex);
            instrPropSelectedInstrument.Digits     = (int)nudPropDigits.Value;
            instrPropSelectedInstrument.LotSize    = (int)nudPropLotSize.Value;
            instrPropSelectedInstrument.Spread     = (float)nudPropSpread.Value;
            instrPropSelectedInstrument.SwapLong   = (float)nudPropSwapLong.Value;
            instrPropSelectedInstrument.SwapShort  = (float)nudPropSwapShort.Value;
            instrPropSelectedInstrument.Commission = (float)nudPropCommission.Value;
            instrPropSelectedInstrument.Slippage   = (int)nudPropSlippage.Value;
            if (tbxPropAccountIn.Text == "USD")
                instrPropSelectedInstrument.RateToUSD = (float)nudPropAccountRate.Value;
            else
                instrPropSelectedInstrument.RateToEUR = (float)nudPropAccountRate.Value;

            return;
        }

        /// <summary>
        /// Calculates the account exchange rate.
        /// </summary>
        void SetAcountExchangeRate()
        {
            if (tbxPropPriceIn.Text == tbxPropAccountIn.Text)
            {
                tbxPropAccountRate.Text  = Language.T("Not used");
                nudPropAccountRate.Value = 1;
                nudPropAccountRate.Enabled = false;
                if (tbxPropAccountIn.Text == "USD")
                    instrPropSelectedInstrument.RateToUSD = 1;
                else if (tbxPropAccountIn.Text == "EUR")
                    instrPropSelectedInstrument.RateToEUR = 1;
            }
            else if (tbxPropType.Text == "Forex" && tbxPropSymbol.Text.StartsWith(tbxPropAccountIn.Text))
            {
                tbxPropAccountRate.Text  = Language.T("Deal price");
                nudPropAccountRate.Value = 0;
                nudPropAccountRate.Enabled = false;
                if (tbxPropAccountIn.Text == "USD")
                    instrPropSelectedInstrument.RateToUSD = 0;
                else if (tbxPropAccountIn.Text == "EUR")
                    instrPropSelectedInstrument.RateToEUR = 0;
            }
            else
            {
                tbxPropAccountRate.Text = tbxPropAccountIn.Text + tbxPropPriceIn.Text;
                if (tbxPropAccountIn.Text == "USD")
                    nudPropAccountRate.Value = (decimal)instrPropSelectedInstrument.RateToUSD;
                else if (tbxPropAccountIn.Text == "EUR")
                    nudPropAccountRate.Value = (decimal)instrPropSelectedInstrument.RateToEUR;
                nudPropAccountRate.Enabled = true;
                nudPropAccountRate.ForeColor = Color.Red;
            }

            return;
        }

        /// <summary>
        /// The lbxInstruments selected index changed
        /// </summary>
        void LbxInstruments_SelectedValueChanged(object sender, EventArgs e)
        {
            if (lbxInstruments.SelectedItem == null) return;

            instrPropSelectedInstrument = Instruments.InstrumentList[lbxInstruments.SelectedItem.ToString()].Clone();
            SetPropertiesForm();

            return;
        }

        /// <summary>
        /// Digit changed
        /// </summary>
        void NudPropDigits_ValueChanged(object sender, EventArgs e)
        {
            tbxPropPoint.Text = (1 / Math.Pow(10, (float)nudPropDigits.Value)).ToString("0.#####");

            return;
        }

        /// <summary>
        /// Checks the instrument currency.
        /// </summary>
        void TbxPropPriceIn_TextChanged(object sender, EventArgs e)
        {
            Regex regexPriceIn = new Regex(@"^[A-Z]{3}$");

            if (regexPriceIn.IsMatch(tbxPropPriceIn.Text))
            {
                tbxPropPriceIn.ForeColor = colorText;

                SetAcountExchangeRate();
            }
            else
            {
                tbxPropPriceIn.ForeColor = Color.Red;
            }

            return;
        }

        /// <summary>
        /// Checks the commission time
        /// </summary>
        void CbxPropCommission_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxPropCommission.SelectedIndex == 1)
            {
                cbxPropCommScope.SelectedIndex = 1;
                cbxPropCommScope.Enabled = false;
            }
            else
                cbxPropCommScope.Enabled = true;
        }

        /// <summary>
        /// Sets the data file names.
        /// </summary>
        void TbxPropFileName_TextChanged(object sender, EventArgs e)
        {
            string text = tbxPropFileName.Text;
            Regex regexFileName = new Regex(@"^[a-zA-Z\$\#][\w- ]*$");

            if (regexFileName.IsMatch(text))
            {
                tbxPropFileName.ForeColor = colorText;
                lblPropDataFiles.Text =
                    "   1 Minute    -  " + text + "1.csv;    1 Hour    -  " + text + "60.csv"   + Environment.NewLine +
                    "   5 Minutes  -  "  + text + "5.csv;    4 Hours  -  "  + text + "240.csv"  + Environment.NewLine +
                    " 15 Minutes  -  "   + text + "15.csv;  1 Day     -  "  + text + "1440.csv" + Environment.NewLine +
                    " 30 Minutes  -  "   + text + "30.csv;  1 Week  -  "    + text + "10080.csv";
            }
            else
            {
                tbxPropFileName.ForeColor = Color.Red;
                lblPropDataFiles.Text = "";
            }

            return;
        }

        /// <summary>
        /// BtnAccept Clicked.
        /// </summary>
        void BtnAccept_Click(object sender, EventArgs e)
        {
            if (ValidatePropertiesForm())
            {
                SetSelectedInstrument();
                if (Instruments.InstrumentList.ContainsKey(instrPropSelectedInstrument.Symbol))
                {   // The instrument exists. We change it.
                    Instruments.InstrumentList[instrPropSelectedInstrument.Symbol] = instrPropSelectedInstrument.Clone();
                }
                else
                {   // The instrument doesn't exist. We create it.
                    Instruments.InstrumentList.Add(instrPropSelectedInstrument.Symbol, instrPropSelectedInstrument.Clone());
                    lbxInstruments.Items.Add(instrPropSelectedInstrument.Symbol);
                    bNeedReset = true;
                }
            }

            return;
        }

        /// <summary>
        /// BtnAdd Clicked.
        /// </summary>
        void BtnAddInstrAdd_Click(object sender, EventArgs e)
        {
            if (ValidateSymbol(tbxAddInstrSymbol.Text, (Instrumet_Type)Enum.Parse(typeof(Instrumet_Type), cbxAddInstrType.Text)) &&
                !lbxInstruments.Items.Contains(tbxAddInstrSymbol.Text))
            {
                instrPropSelectedInstrument = new Instrument_Properties(tbxAddInstrSymbol.Text, (Instrumet_Type)Enum.Parse(typeof(Instrumet_Type), cbxAddInstrType.Text));
                SetPropertiesForm();
                SetSelectedInstrument();
            }
            else
            {
                MessageBox.Show(Language.T("Wrong Symbol!"), Language.T("Instrument Properties"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            return;
        }

        /// <summary>
        /// BtnDelete Clicked.
        /// </summary>
        void BtnDelete_Click(object sender, EventArgs e)
        {
            string symbol = lbxInstruments.SelectedItem.ToString();
            int    index  = lbxInstruments.SelectedIndex;

            if (symbol == "EURUSD" || symbol == "GBPUSD" ||
                symbol == "USDCHF" || symbol == "USDJPY" ||
                symbol == Data.Symbol)
            {
                MessageBox.Show(
                Language.T("You cannot delete this instrument!"),
                Language.T("Instrument Editor"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                return;
            }

            DialogResult dr = MessageBox.Show(
                Language.T("Do you want to delete the selected instrument?"),
                Language.T("Instrument Editor"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                Instruments.InstrumentList.Remove(symbol);
                lbxInstruments.Items.Remove(symbol);
                if (index > 0)
                    lbxInstruments.SelectedIndex = index - 1;
                else
                    lbxInstruments.SelectedIndex = index;

                bNeedReset = true;
            }

            return;
        }

        /// <summary>
        /// BtnUp Clicked.
        /// </summary>
        void BtnUp_Click(object sender, EventArgs e)
        {
            string symbol = lbxInstruments.SelectedItem.ToString();
            int    index  = lbxInstruments.SelectedIndex;

            if (index > 0)
            {
                lbxInstruments.Items.RemoveAt(index);
                lbxInstruments.Items.Insert(index - 1, symbol);
                lbxInstruments.SelectedIndex = index - 1;
                bNeedReset = true;
            }

            return;
        }

        /// <summary>
        /// BtnDown Clicked.
        /// </summary>
        void BtnDown_Click(object sender, EventArgs e)
        {
            string symbol = lbxInstruments.SelectedItem.ToString();
            int    index  = lbxInstruments.SelectedIndex;

            if (index < lbxInstruments.Items.Count - 1)
            {
                lbxInstruments.Items.RemoveAt(index);
                lbxInstruments.Items.Insert(index + 1, symbol);
                lbxInstruments.SelectedIndex = index + 1;
                bNeedReset = true;
            }

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
        /// Check whether the restart is necessary.
        /// </summary>
        void Instrument_Editor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bNeedReset)
            {
                Dictionary<String, Instrument_Properties> temp = new Dictionary<string, Instrument_Properties>(Instruments.InstrumentList.Count);

                foreach (KeyValuePair<String, Instrument_Properties> kvp in Instruments.InstrumentList)
                    temp.Add(kvp.Key, kvp.Value.Clone());

                Instruments.InstrumentList.Clear();

                foreach (string symbol in lbxInstruments.Items)
                    Instruments.InstrumentList.Add(symbol, temp[symbol].Clone());
            }

            return;
        }
    }
}
