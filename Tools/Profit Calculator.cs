// Profit Calculator
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    ///Profit Calculator
    /// </summary>
    public class Profit_Calculator : Form
    {
        Fancy_Panel pnlInput;
        Fancy_Panel pnlOutput;

        Label[] alblInputNames;
        Label[] alblOutputNames;
        Label[] alblOutputValues;

        Label         lblLotSize;
        ComboBox      cbxDirection;
        NumericUpDown nudLots;
        NumericUpDown nudEntryPrice;
        NumericUpDown nudExitPrice;
        NumericUpDown nudDays;

        Font  font;
        Color colorText;

        string symbol;
        System.Windows.Forms.Timer timer;

        /// <summary>
        /// Constructor
        /// </summary>
        public Profit_Calculator()
        {
            pnlInput  = new Fancy_Panel(Language.T("Input Values"));
            pnlOutput = new Fancy_Panel(Language.T("Output Values"));

            alblInputNames   = new Label[6];
            alblOutputNames  = new Label[8];
            alblOutputValues = new Label[8];

            lblLotSize    = new Label();
            cbxDirection  = new ComboBox();
            nudLots       = new NumericUpDown();
            nudEntryPrice = new NumericUpDown();
            nudExitPrice  = new NumericUpDown();
            nudDays       = new NumericUpDown();

            font      = this.Font;
            colorText = LayoutColors.ColorControlText;

            MaximizeBox     = false;
            MinimizeBox     = false;
            Icon            = Data.Icon;
            BackColor       = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Text            = Language.T("Profit Calculator");

            // Input
            pnlInput.Parent = this;

            // Output
            pnlOutput.Parent = this;

            // Input Names
            string[] asInputNames = new string[] {
                Data.InstrProperties.Symbol,
                Language.T("Direction"),
                Language.T("Number of lots"),
                Language.T("Entry price"),
                Language.T("Exit price"),
                Language.T("Days rollover"),
            };

            int number = 0;
            foreach (string name in asInputNames)
            {
                alblInputNames[number] = new Label();
                alblInputNames[number].Parent    = pnlInput;
                alblInputNames[number].ForeColor = colorText;
                alblInputNames[number].BackColor = Color.Transparent;
                alblInputNames[number].AutoSize  = true;
                alblInputNames[number].Text      = name;
                number++;
            }

            // Label Lot size
            lblLotSize.Parent    = pnlInput;
            lblLotSize.ForeColor = colorText;
            lblLotSize.BackColor = Color.Transparent;

            // ComboBox SameDirAction
            cbxDirection.Parent = pnlInput;
            cbxDirection.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxDirection.Items.AddRange(new string[] { Language.T("Long"), Language.T("Short")});
            cbxDirection.SelectedIndex = 0;

            // Lots
            nudLots.Parent = pnlInput;
            nudLots.BeginInit();
            nudLots.Minimum       = 0.01M;
            nudLots.Maximum       = 100;
            nudLots.Increment     = 0.01M;
            nudLots.DecimalPlaces = 2;
            nudLots.Value         = (decimal)Data.Strategy.EntryLots;
            nudLots.EndInit();

            // NumericUpDown Entry Price
            nudEntryPrice.Parent = pnlInput;

            // NumericUpDown Exit Price
            nudExitPrice.Parent = pnlInput;

            // NumericUpDown Reducing Lots
            nudDays.Parent = pnlInput;
            nudDays.BeginInit();
            nudDays.Minimum   = 0;
            nudDays.Maximum   = 1000;
            nudDays.Increment = 1;
            nudDays.Value     = 1;
            nudDays.EndInit();

            // Output Names
            string[] asOutputNames = new string[] {
                Language.T("Required margin"),
                Language.T("Gross profit"),
                Language.T("Spread"),
                Language.T("Entry commission"),
                Language.T("Exit commission"),
                Language.T("Rollover"),
                Language.T("Slippage"),
                Language.T("Net profit"),
            };

            number = 0;
            foreach (string name in asOutputNames)
            {
                alblOutputNames[number] = new Label();
                alblOutputNames[number].Parent    = pnlOutput;
                alblOutputNames[number].ForeColor = colorText;
                alblOutputNames[number].BackColor = Color.Transparent;
                alblOutputNames[number].AutoSize  = true;
                alblOutputNames[number].Text      = name;

                alblOutputValues[number] = new Label();
                alblOutputValues[number].Parent    = pnlOutput;
                alblOutputValues[number].ForeColor = colorText;
                alblOutputValues[number].BackColor = Color.Transparent;
                alblOutputValues[number].AutoSize  = true;

                number++;
            }

            alblOutputNames[number - 1].Font  = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
            alblOutputValues[number - 1].Font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);

            timer = new Timer();
            timer.Interval = 2000;
            timer.Tick    += new EventHandler(Timer_Tick);
            timer.Start();
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cbxDirection.SelectedIndexChanged += new EventHandler(ParamChanged);
            nudLots.ValueChanged       += new EventHandler(ParamChanged);
            nudEntryPrice.ValueChanged += new EventHandler(ParamChanged);
            nudExitPrice.ValueChanged  += new EventHandler(ParamChanged);
            nudDays.ValueChanged       += new EventHandler(ParamChanged);

            int buttonWidth = (int)(Data.HorizontalDLU * 60);
            int btnHrzSpace = (int)(Data.HorizontalDLU * 3);

            ClientSize = new Size(270, 405);

            InitParams();
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
            int border       = btnHrzSpace;
            int textHeight   = Font.Height;
            int width        = 100; // Right side controls

            // pnlInput
            pnlInput.Size = new Size(ClientSize.Width - 2 * border, 190);
            pnlInput.Location = new Point(border, border);

            int left = pnlInput.ClientSize.Width - width - btnHrzSpace - 1;

            lblLotSize.Width    = width;
            cbxDirection.Width  = width;
            nudLots.Width       = width;
            nudEntryPrice.Width = width;
            nudExitPrice.Width  = width;
            nudDays.Width       = width;

            int shift = 22;
            int vertSpace = 2;
            lblLotSize.Location    = new Point(left, 0 * buttonHeight + 1 * vertSpace + shift - 0);
            cbxDirection.Location  = new Point(left, 1 * buttonHeight + 2 * vertSpace + shift - 4);
            nudLots.Location       = new Point(left, 2 * buttonHeight + 3 * vertSpace + shift - 4);
            nudEntryPrice.Location = new Point(left, 3 * buttonHeight + 4 * vertSpace + shift - 4);
            nudExitPrice.Location  = new Point(left, 4 * buttonHeight + 5 * vertSpace + shift - 4);
            nudDays.Location       = new Point(left, 5 * buttonHeight + 6 * vertSpace + shift - 4);

            int numb = 0;
            foreach(Label lbl in alblInputNames)
            {
                lbl.Location = new Point(border, numb * buttonHeight + (numb + 1) * vertSpace + shift);
                numb++;
            }

            // pnlOutput
            pnlOutput.Size     = new Size(ClientSize.Width - 2 * border, 200);
            pnlOutput.Location = new Point(border, pnlInput.Bottom + border);

            shift = 24;
            vertSpace = -4;
            numb = 0;
            foreach(Label lbl in alblOutputNames)
            {
                lbl.Location = new Point(border, numb * (buttonHeight + vertSpace) + shift);
                numb++;
            }

            numb = 0;
            foreach(Label lbl in alblOutputValues)
            {
                lbl.Location = new Point(left, numb * (buttonHeight + vertSpace) + shift);
                numb++;
            }

            return;
        }

        /// <summary>
        /// Perform periodical action.
        /// </summary>
        void Timer_Tick(object sender, EventArgs e)
        {
            if (symbol != Data.InstrProperties.Symbol)
            {
                InitParams();
                InitParams();
            }

            return;
        }

        /// <summary>
        /// Sets the initial params.
        /// </summary>
        void InitParams()
        {
            symbol = Data.InstrProperties.Symbol;

            alblInputNames[0].Text = symbol;
            lblLotSize.Text = Data.InstrProperties.LotSize.ToString();

            // NumericUpDown Entry Price
            nudEntryPrice.BeginInit();
            nudEntryPrice.DecimalPlaces = Data.InstrProperties.Digits;
            nudEntryPrice.Minimum       = (decimal)(Data.MinPrice * 0.7);
            nudEntryPrice.Maximum       = (decimal)(Data.MaxPrice * 1.3);
            nudEntryPrice.Increment     = (decimal)Data.InstrProperties.Point;
            nudEntryPrice.Value         = (decimal)Data.Close[Data.Bars - 1];
            nudEntryPrice.EndInit();

            // NumericUpDown Exit Price
            nudExitPrice.BeginInit();
            nudExitPrice.DecimalPlaces = Data.InstrProperties.Digits;
            nudExitPrice.Minimum       = (decimal)(Data.MinPrice * 0.7);
            nudExitPrice.Maximum       = (decimal)(Data.MaxPrice * 1.3);
            nudExitPrice.Increment     = (decimal)Data.InstrProperties.Point;
            nudExitPrice.Value         = (decimal)(Data.Close[Data.Bars - 1] + 100 * Data.InstrProperties.Point);
            nudExitPrice.EndInit();

            Calculate();

            return;
        }

        /// <summary>
        /// Sets the params values
        /// </summary>
        void ParamChanged(object sender, EventArgs e)
        {
            Calculate();

            return;
        }

        /// <summary>
        /// Calculates the result
        /// </summary>
        void Calculate()
        {
            bool   isLong       = (cbxDirection.SelectedIndex == 0);
            PosDirection posDir = isLong ? PosDirection.Long : PosDirection.Short;
            int    lotSize      = Data.InstrProperties.LotSize;
            double lots         = (double)nudLots.Value;
            double entryPrice   = (double)nudEntryPrice.Value;
            double exitPrice    = (double)nudExitPrice.Value;
            int    daysRollover = (int)nudDays.Value;
            double point        = Data.InstrProperties.Point;
            string unit         = " " + Configs.AccountCurrency;
            double entryValue   = lots * lotSize * entryPrice;
            double exitValue    = lots * lotSize * exitPrice;

            // Required margin
            double requiredMargin = (lots * lotSize / Configs.Leverage) * (entryPrice / Backtester.AccountExchangeRate(entryPrice));
            alblOutputValues[0].Text = requiredMargin.ToString("F2") + unit;

            // Gross Profit
            double grossProfit = (isLong ? exitValue - entryValue : entryValue - exitValue) / Backtester.AccountExchangeRate(exitPrice);
            alblOutputValues[1].Text = grossProfit.ToString("F2") + unit;

            // Spread
            double spread = Data.InstrProperties.Spread * point * lots * lotSize / Backtester.AccountExchangeRate(exitPrice);
            alblOutputValues[2].Text = spread.ToString("F2") + unit;

            // Entry Commission
            double entryCommission = Backtester.CommissionInMoney(lots, entryPrice, false);
            alblOutputValues[3].Text = entryCommission.ToString("F2") + unit;

            // Exit Commission
            double exitCommission = Backtester.CommissionInMoney(lots, exitPrice, true);
            alblOutputValues[4].Text = exitCommission.ToString("F2") + unit;

            // Rollover
            double rollover = Backtester.RolloverInMoney(posDir, lots, daysRollover, exitPrice);
            alblOutputValues[5].Text = rollover.ToString("F2") + unit;

            // Slippage
            double slippage = Data.InstrProperties.Slippage * point * lots * lotSize / Backtester.AccountExchangeRate(exitPrice);
            alblOutputValues[6].Text = slippage.ToString("F2") + unit;

            // Net Profit
            double netProfit = grossProfit - entryCommission - exitCommission - rollover - slippage;
            alblOutputValues[7].Text = netProfit.ToString("F2") + unit;

            return;
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }
    }
}
