// Account Settings class
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
    public class Account_Settings : Form
    {
        Fancy_Panel pnlBase;

        Label lblAccountCurrency;
        Label lblInitialAccount;
        Label lblLeverage;
        Label lblExchangeRate;
        Label lblExchangeRateInfo;

        ComboBox      cbxAccountCurrency;
        NumericUpDown nudInitialAccount;
        ComboBox      cbxLeverage;
        NumericUpDown nudExchangeRate;
        TextBox       tbxExchangeRate;

        Button  btnDefault;
        Button  btnAccept;
        Button  btnCancel;

        ToolTip toolTip = new ToolTip();

        Font   font;
        Color  colorText;

        string accountCurrency;
        int    initialAccount;
        int    leverage;
        double rateToUSD;
        double rateToEUR;

        /// <summary>
        /// Account Currency
        /// </summary>
        public string AccountCurrency
        {
            get { return accountCurrency; }
            set { accountCurrency = value; }
        }

        /// <summary>
        /// Initial Account
        /// </summary>
        public int InitialAccount
        {
            get { return initialAccount; }
            set { initialAccount = value; }
        }

        /// <summary>
        /// Leverage
        /// </summary>
        public int Leverage
        {
            get { return leverage; }
            set { leverage = value; }
        }

        /// <summary>
        /// Exchange Rate to USD
        /// </summary>
        public double RateToUSD
        {
            get { return rateToUSD; }
            set { rateToUSD = value; }
        }

        /// <summary>
        /// Exchange Rate to EUR
        /// </summary>
        public double RateToEUR
        {
            get { return rateToEUR; }
            set { rateToEUR = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Account_Settings()
        {
            pnlBase = new Fancy_Panel();

            lblAccountCurrency  = new Label();
            lblInitialAccount   = new Label();
            lblLeverage         = new Label();
            lblExchangeRate     = new Label();
            lblExchangeRateInfo = new Label();

            cbxAccountCurrency = new ComboBox();
            nudInitialAccount  = new NumericUpDown();
            cbxLeverage        = new ComboBox();
            nudExchangeRate    = new NumericUpDown();
            tbxExchangeRate    = new TextBox();

            btnDefault = new Button();
            btnCancel  = new Button();
            btnAccept  = new Button();

            font      = this.Font;
            colorText = LayoutColors.ColorControlText;

            MaximizeBox     = false;
            MinimizeBox     = false;
            ShowInTaskbar   = false;
            Icon            = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton    = btnAccept;
            Text            = Language.T("Account Settings");

            // pnlBase
            pnlBase.Parent = this;

            // Label Account Currency
            lblAccountCurrency.Parent    = pnlBase;
            lblAccountCurrency.ForeColor = colorText;
            lblAccountCurrency.BackColor = Color.Transparent;
            lblAccountCurrency.Text      = Language.T("Account currency");
            lblAccountCurrency.AutoSize  = true;

            // Label Initial Account
            lblInitialAccount.Parent    = pnlBase;
            lblInitialAccount.ForeColor = colorText;
            lblInitialAccount.BackColor = Color.Transparent;
            lblInitialAccount.Text      = Language.T("Initial account");
            lblInitialAccount.AutoSize  = true;

            // Label Leverage
            lblLeverage.Parent    = pnlBase;
            lblLeverage.ForeColor = colorText;
            lblLeverage.BackColor = Color.Transparent;
            lblLeverage.Text      = Language.T("Leverage");
            lblLeverage.AutoSize  = true;

            // Label Exchange Rate
            lblExchangeRate.Parent    = pnlBase;
            lblExchangeRate.ForeColor = colorText;
            lblExchangeRate.BackColor = Color.Transparent;
            lblExchangeRate.Text      = Language.T("Account exchange rate");
            lblExchangeRate.AutoSize  = true;

            // Label Exchange Rate Info
            lblExchangeRateInfo.Parent    = pnlBase;
            lblExchangeRateInfo.ForeColor = colorText;
            lblExchangeRateInfo.BackColor = Color.Transparent;
            lblExchangeRateInfo.Text      =
                Language.T("Forex Strategy Builder uses the account exchange rate to calculate the trading statistics in your account currency.") + " " +
                Language.T("When your account currency does not take part in the trading couple the account exchange rate is a fixed figure.");

            // ComboBox Account Currency
            cbxAccountCurrency.Parent = pnlBase;
            cbxAccountCurrency.Name   = "cbxAccountCurrency";
            cbxAccountCurrency.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxAccountCurrency.Items.AddRange(new string[] { "USD", "EUR" });
            cbxAccountCurrency.SelectedIndex = 0;

            // NumericUpDown Initial Account
            nudInitialAccount.Parent    = pnlBase;
            nudInitialAccount.Name      = "nudInitialAccount";
            nudInitialAccount.BeginInit();
            nudInitialAccount.Minimum   = 100;
            nudInitialAccount.Maximum   = 100000;
            nudInitialAccount.Increment = 1000;
            nudInitialAccount.Value     = initialAccount;
            nudInitialAccount.EndInit();

            // ComboBox Leverage
            cbxLeverage.Parent        = pnlBase;
            cbxLeverage.Name          = "cbxLeverage";
            cbxLeverage.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxLeverage.Items.AddRange(new string[] { "1/1", "1/10", "1/20", "1/30", "1/50", "1/100", "1/200", "1/300", "1/400", "1/500" });
            cbxLeverage.SelectedIndex = 5;

            // tbxExchangeRate
            tbxExchangeRate.Parent    = pnlBase;
            tbxExchangeRate.BackColor = LayoutColors.ColorControlBack;
            tbxExchangeRate.ForeColor = colorText;
            tbxExchangeRate.ReadOnly  = true;
            tbxExchangeRate.Visible   = false;
            tbxExchangeRate.Text      = Language.T("Deal price");

            // NumericUpDown Exchange Rate
            nudExchangeRate.BeginInit();
            nudExchangeRate.Parent        = pnlBase;
            nudExchangeRate.Name          = "nudExchangeRate";
            nudExchangeRate.Minimum       = 0;
            nudExchangeRate.Maximum       = 100000;
            nudExchangeRate.Increment     = 0.0001M;
            nudExchangeRate.DecimalPlaces = 4;
            nudExchangeRate.Value         = 1;
            nudExchangeRate.EndInit();

            //Button Default
            btnDefault.Parent = this;
            btnDefault.Name   = "Default";
            btnDefault.Text   = Language.T("Default");
            btnDefault.Click += new EventHandler(BtnDefault_Click);
            btnDefault.UseVisualStyleBackColor = true;

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
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            int buttonWidth = (int)(Data.HorizontalDLU * 60);
            int btnHrzSpace = (int)(Data.HorizontalDLU * 3);

            ClientSize = new Size(3 * buttonWidth + 4 * btnHrzSpace, 257);

            cbxAccountCurrency.SelectedIndexChanged += new EventHandler(ParamChanged);
            nudInitialAccount.ValueChanged          += new EventHandler(ParamChanged);
            cbxLeverage.SelectedIndexChanged        += new EventHandler(ParamChanged);
            nudExchangeRate.ValueChanged            += new EventHandler(ParamChanged);

            btnAccept.Focus();
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth  = (int)(Data.HorizontalDLU * 60);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int space        = btnHrzSpace;
            int textHeight   = Font.Height;
            int nudWidth     = buttonWidth - space - 1;
            int border       = 2;

            // pnlBase
            pnlBase.Size     = new Size(ClientSize.Width - 2 * space, ClientSize.Height - 2 * btnVertSpace - buttonHeight - space);
            pnlBase.Location = new Point(space, space);

            // Labels
            lblAccountCurrency.Location  = new Point(border + btnHrzSpace, 0 * buttonHeight + 1 * space + 8);
            lblInitialAccount.Location   = new Point(border + btnHrzSpace, 1 * buttonHeight + 2 * space + 6);
            lblLeverage.Location         = new Point(border + btnHrzSpace, 2 * buttonHeight + 3 * space + 8);
            lblExchangeRate.Location     = new Point(border + btnHrzSpace, 3 * buttonHeight + 4 * space + 7);
            lblExchangeRateInfo.Location = new Point(border + btnHrzSpace, 4 * buttonHeight + 5 * space + 8);
            lblExchangeRateInfo.Size     = new Size(pnlBase.ClientSize.Width - 2 * space - 2 * border, pnlBase.ClientSize.Height - border - space - lblExchangeRateInfo.Top);

            // Params
            int nudLeft = pnlBase.ClientSize.Width - nudWidth - btnHrzSpace - border;
            cbxAccountCurrency.Size     = new Size(nudWidth, buttonHeight);
            cbxAccountCurrency.Location = new Point(nudLeft, 0 * buttonHeight + 1 * space + 4);
            nudInitialAccount.Size      = new Size(nudWidth, buttonHeight);
            nudInitialAccount.Location  = new Point(nudLeft, 1 * buttonHeight + 2 * space + 4);
            cbxLeverage.Size            = new Size(nudWidth, buttonHeight);
            cbxLeverage.Location        = new Point(nudLeft, 2 * buttonHeight + 3 * space + 4);
            nudExchangeRate.Size        = new Size(nudWidth, buttonHeight);
            nudExchangeRate.Location    = new Point(nudLeft, 3 * buttonHeight + 4 * space + 4);
            tbxExchangeRate.Size        = new Size(nudWidth, buttonHeight);
            tbxExchangeRate.Location    = new Point(nudLeft, 3 * buttonHeight + 4 * space + 4);

            // Button Cancel
            btnCancel.Size     = new Size(buttonWidth, buttonHeight);
            btnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Default
            btnDefault.Size     = new Size(buttonWidth, buttonHeight);
            btnDefault.Location = new Point(btnCancel.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            btnAccept.Size     = new Size(buttonWidth, buttonHeight);
            btnAccept.Location = new Point(btnDefault.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

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
        /// Sets the controls' text
        /// </summary>
        public void SetParams()
        {
            // ComboBox Account Currency
            cbxAccountCurrency.SelectedItem = accountCurrency;

            // NumericUpDown Initial Account
            nudInitialAccount.Value = initialAccount;

            // ComboBox Leverage
            cbxLeverage.SelectedItem = "1/" + leverage.ToString();

            SetAcountExchangeRate();

            return;
        }

        /// <summary>
        /// Calculates the account exchange rate.
        /// </summary>
        void SetAcountExchangeRate()
        {
            lblExchangeRate.Text = Language.T("Account exchange rate");

            if (Data.InstrProperties.PriceIn == cbxAccountCurrency.Text)
            {
                tbxExchangeRate.Text    = Language.T("Not used");
                tbxExchangeRate.Visible = true;
                nudExchangeRate.Value   = 1;
                nudExchangeRate.Visible = false;
                if (cbxAccountCurrency.Text == "USD")
                    rateToUSD = 1;
                else if (cbxAccountCurrency.Text == "EUR")
                    rateToEUR = 1;
            }
            else if (Data.InstrProperties.InstrType == Instrumet_Type.Forex && Data.InstrProperties.Symbol.StartsWith(cbxAccountCurrency.Text))
            {
                tbxExchangeRate.Text    = Language.T("Deal price");
                tbxExchangeRate.Visible = true;
                nudExchangeRate.Value   = 0;
                nudExchangeRate.Visible = false;
                if (cbxAccountCurrency.Text == "USD")
                    rateToUSD = 0;
                else if (cbxAccountCurrency.Text == "EUR")
                    rateToEUR = 0;
            }
            else
            {
                lblExchangeRate.Text   += " " + cbxAccountCurrency.Text + Data.InstrProperties.PriceIn;
                tbxExchangeRate.Visible = false;
                if (cbxAccountCurrency.Text == "USD")
                    nudExchangeRate.Value = (decimal)rateToUSD;
                else if (cbxAccountCurrency.Text == "EUR")
                    nudExchangeRate.Value = (decimal)rateToEUR;
                nudExchangeRate.Visible = true;
            }

            return;
        }

        /// <summary>
        /// Sets the params values
        /// </summary>
        void ParamChanged(object sender, EventArgs e)
        {
            string name = ((Control)sender).Name;

            // ComboBox Account Currency
            if (name == "cbxAccountCurrency")
            {
                accountCurrency = cbxAccountCurrency.Text;
                SetAcountExchangeRate();
            }

            // NumericUpDown Initial Account
            if (name == "nudInitialAccount")
            {
                initialAccount = (int)nudInitialAccount.Value;
            }

            // ComboBox Leverage
            if (name == "cbxLeverage")
            {
                leverage = int.Parse(cbxLeverage.Text.Substring(2));
            }

            // NumericUpDown Exchange Rate
            if (name == "nudExchangeRate")
            {
                if (cbxAccountCurrency.Text == "USD")
                    rateToUSD = (double)nudExchangeRate.Value;
                else if (cbxAccountCurrency.Text == "EUR")
                    rateToEUR = (double)nudExchangeRate.Value;
            }

            return;
        }

        /// <summary>
        /// Button Default Click
        /// </summary>
        void BtnDefault_Click(object sender, EventArgs e)
        {
            accountCurrency = "USD";
            initialAccount  = 10000;
            leverage        = 100;

            SetParams();

            return;
        }
    }
}
