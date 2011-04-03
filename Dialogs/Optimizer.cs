// Strategy Optimizer
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Provide links to the Parameter's fields
    /// </summary>
    public class Parameter
    {
        int    slotNumber;
        int    paramNumber;
        double bestValue;

        /// <summary>
        /// The number of the indicator slot
        /// </summary>
        public int SlotNumber
        {
            get { return slotNumber; }
            set { slotNumber = value; }
        }

        /// <summary>
        /// The number of NumericParam
        /// </summary>
        public int NumParam
        {
            get { return paramNumber; }
            set { paramNumber = value; }
        }

        /// <summary>
        /// The IndicatorParameters
        /// </summary>
        public IndicatorParam IP
        {
            get { return Data.Strategy.Slot[slotNumber].IndParam; }
        }

        /// <summary>
        /// The name of the parameter
        /// </summary>
        public string ParameterName
        {
            get { return Data.Strategy.Slot[slotNumber].IndParam.NumParam[paramNumber].Caption; }
        }

        /// <summary>
        /// The current value of the parameter
        /// </summary>
        public double Value
        {
            get { return Data.Strategy.Slot[slotNumber].IndParam.NumParam[paramNumber].Value; }
            set { Data.Strategy.Slot[slotNumber].IndParam.NumParam[paramNumber].Value = value; }
        }

        /// <summary>
        /// The minimum value of the parameter
        /// </summary>
        public double Minimum
        {
            get { return Data.Strategy.Slot[slotNumber].IndParam.NumParam[paramNumber].Min; }
        }

        /// <summary>
        /// The maximum value of the parameter
        /// </summary>
        public double Maximum
        {
            get { return Data.Strategy.Slot[slotNumber].IndParam.NumParam[paramNumber].Max; }
        }

        /// <summary>
        /// The number of significant digits
        /// </summary>
        public int Point
        {
            get { return Data.Strategy.Slot[slotNumber].IndParam.NumParam[paramNumber].Point; }
        }

        /// <summary>
        /// The best value of the parameter
        /// </summary>
        public double BestValue
        {
            get { return bestValue; }
            set { bestValue = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Parameter(int slotNumber, int paramNumber)
        {
            this.slotNumber  = slotNumber;
            this.paramNumber = paramNumber;
            bestValue = Value;
        }
    }

    /// <summary>
    /// The numbers of the parameters into the couple
    /// </summary>
    public struct CoupleOfParams
    {
        int  param1;
        int  param2;
        bool isPassed;

        public int Param1 { get { return param1; } set { param1 = value; } }
        public int Param2 { get { return param2; } set { param2 = value; } }
        public bool IsPassed { get { return isPassed; } set { isPassed = value; } }
    }

    /// <summary>
    ///  Optimizer ToolStrip Buttons
    /// </summary>
    public enum OptimizerButtons
    {
        SelectAll, SelectNone, SelectRandom,
        SetStep5, SetStep10, SetStep15,
        ShowParams, ShowLimitations, ShowSettings,
    }

    /// <summary>
    /// The Optimizer
    /// </summary>
    public class Optimizer : Form
    {
        ToolStrip tsOptimizerButtons;
        ToolStripButton[] aOptimizerButtons;

        Panel               pnlParamsBase;
        Panel               pnlCaptions;
        Panel               pnlParamsBase2;
        Panel               pnlParams;
        Fancy_Panel         pnlLimitations;
        Fancy_Panel         pnlSettings;

        CheckBox[]          achbxParameterName;
        Label []            alblParameterValue;
        NumericUpDown []    anudParameterMin;
        NumericUpDown []    anudParameterMax;
        NumericUpDown []    anudParameterStep;
        Small_Balance_Chart smallBalanceChart;
        Label []            alblIndicatorName;
        Label               lblNoParams;
        ProgressBar         progressBar;
        Button              btnOptimize;
        Button              btnAccept;
        Button              btnCancel;
        Parameter[]         aParameter;
        ToolTip             toolTip = new ToolTip();
        Random              random  = new Random();
        VScrollBar          scrollBar;
        BackgroundWorker    bgWorker;
        bool                isStartegyChanged = false;

        CheckBox      chbAmbiguousBars;
        NumericUpDown nudAmbiguousBars;
        CheckBox      chbMaxDrawdown;
        NumericUpDown nudMaxDrawdown;
        CheckBox      chbMinTrades;
        NumericUpDown nudMinTrades;
        CheckBox      chbMaxTrades;
        NumericUpDown nudMaxTrades;
        CheckBox      chbWinLossRatio;
        NumericUpDown nudWinLossRatio;
        CheckBox      chbEquityPercent;
        NumericUpDown nudEquityPercent;
        CheckBox      chbOOSPatternFilter;
        NumericUpDown nudOOSPatternPercent;
        CheckBox      chbSmoothBalanceLines;
        NumericUpDown nudSmoothBalancePercent;
        NumericUpDown nudSmoothBalanceCheckPoints;
        CheckBox      chbOptimizerWritesReport;

        CheckBox chbHideFSB;
        Button btnReset;

        int   parameters;   // Count of the NumericParameters
        int   indicators;   // Count of the Indicators
        bool  isOptimizing; // It is true when the optimizer is running

        int   checkedParams;   // Count of the checked parameters
        int[] aiChecked;       // An array of the checked parameters
        int   cycles;          // Count of the cycles
        int   computedCycles;  // Currently completed cycles
        int   progressPercent; // Reached progress in %

        // Out of Sample
        int  barOOS = Data.Bars - 1;
        bool isOOS  = false;
        CheckBox      chbOutOfSample;
        NumericUpDown nudOutOfSample;

        Font font;
        Font fontIndicator;
        Font fontParamValueBold;
        Font fontParamValueRegular;
        Color colorText;

        int border = 2;
        StringBuilder sbReport;

        Form formFSB;
        public Form SetParrentForm { set { formFSB = value; } }

        bool isReset = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public Optimizer()
        {
            pnlParamsBase     = new Panel();
            pnlParamsBase2    = new Panel();
            pnlCaptions       = new Panel();
            pnlParams         = new Panel();
            pnlLimitations    = new Fancy_Panel(Language.T("Limitations"));
            pnlSettings       = new Fancy_Panel(Language.T("Settings"));
            scrollBar         = new VScrollBar();
            smallBalanceChart = new Small_Balance_Chart();
            progressBar       = new ProgressBar();
            btnOptimize       = new Button();
            btnAccept         = new Button();
            btnCancel         = new Button();

            lblNoParams = new Label();

            font          = this.Font;
            fontIndicator = new Font(Font.FontFamily, 11);
            colorText     = LayoutColors.ColorControlText;

            MaximizeBox     = false;
            Icon            = Data.Icon;
            BackColor       = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton    = btnAccept;
            CancelButton    = btnCancel;
            Text            = Language.T("Optimizer");
            FormClosing    += new FormClosingEventHandler(Optimizer_FormClosing);

            // pnlParamsBase
            pnlParamsBase.Parent    = this;
            pnlParamsBase.BackColor = LayoutColors.ColorControlBack;
            pnlParamsBase.Paint    += new PaintEventHandler(PnlParamsBase_Paint);

            // pnlCaptions
            pnlCaptions.Parent    = pnlParamsBase;
            pnlCaptions.Dock      = DockStyle.Top;
            pnlCaptions.BackColor = LayoutColors.ColorCaptionBack;
            pnlCaptions.ForeColor = LayoutColors.ColorCaptionText;
            pnlCaptions.Paint    += new PaintEventHandler(PnlCaptions_Paint);

            // pnlParamsBase2
            pnlParamsBase2.Parent    = pnlParamsBase;
            pnlParamsBase2.BackColor = LayoutColors.ColorControlBack;
            pnlParamsBase2.Resize   += new EventHandler(PnlParamsBase2_Resize);

            // VScrollBar
            scrollBar.Parent        = pnlParamsBase2;
            scrollBar.Dock          = DockStyle.Right;
            scrollBar.TabStop       = true;
            scrollBar.ValueChanged += new EventHandler(ScrollBar_ValueChanged);
            scrollBar.MouseWheel   += new MouseEventHandler(ScrollBar_MouseWheel);

            // pnlParams
            pnlParams.Parent    = pnlParamsBase2;
            pnlParams.BackColor = LayoutColors.ColorControlBack;

            // lblNoParams
            lblNoParams.Parent   = pnlParams;
            lblNoParams.Text     = Language.T("There are no parameters suitable for optimization.");
            lblNoParams.AutoSize = true;
            lblNoParams.Visible  = false;

            // Panel Limitations
            pnlLimitations.Parent  = this;
            pnlLimitations.Visible = false;

            // Panel Settings
            pnlSettings.Parent  = this;
            pnlSettings.Visible = false;

            // smallBalanceChart
            smallBalanceChart.Parent    = this;
            smallBalanceChart.BackColor = LayoutColors.ColorControlBack;
            smallBalanceChart.SetChartData();

            // ProgressBar
            progressBar.Parent  = this;
            progressBar.Minimum = 1;
            progressBar.Maximum = 100;
            progressBar.Step    = 1;

            // Button Optimize
            btnOptimize.Parent   = this;
            btnOptimize.Name     = "btnOptimize";
            btnOptimize.Text     = Language.T("Optimize");
            btnOptimize.TabIndex = 0;
            btnOptimize.Click   += new EventHandler(BtnOptimize_Click);
            btnOptimize.UseVisualStyleBackColor = true;

            // Button Accept
            btnAccept.Parent       = this;
            btnAccept.Name         = "btnAccept";
            btnAccept.Text         = Language.T("Accept");
            btnAccept.TabIndex     = 1;
            btnAccept.Enabled      = false;
            btnAccept.DialogResult = DialogResult.OK;
            btnAccept.UseVisualStyleBackColor = true;

            // Button Cancel
            btnCancel.Parent       = this;
            btnCancel.Text         = Language.T("Cancel");
            btnCancel.TabIndex     = 2;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.UseVisualStyleBackColor = true;

            // BackGroundWorker
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress      = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork             += new DoWorkEventHandler(BgWorker_DoWork);
            bgWorker.ProgressChanged    += new ProgressChangedEventHandler(BgWorker_ProgressChanged);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BgWorker_RunWorkerCompleted);

            isOptimizing = false;
            SetupButtons();
            SetPanelLimitations();
            SetPanelSettings();
            LoadOptions();

            chbHideFSB.CheckedChanged += new EventHandler(HideFSB_Click);

            return;
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            formFSB.Visible = !chbHideFSB.Checked;

            SetIndicatorParams();

            Width  = 463;
            Height = 570;

            return;
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int buttonWidth  = (ClientSize.Width - 4 * btnHrzSpace) / 3;
            int space        = btnHrzSpace;
            int textHeight   = Font.Height;

            // Button Cancel
            btnCancel.Size     = new Size(buttonWidth, buttonHeight);
            btnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            btnAccept.Size     = new Size(buttonWidth, buttonHeight);
            btnAccept.Location = new Point(btnCancel.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Optimize
            btnOptimize.Size     = new Size(buttonWidth, buttonHeight);
            btnOptimize.Location = new Point(btnAccept.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // ProgressBar
            progressBar.Size     = new Size(ClientSize.Width - 2 * space, (int)(Data.VerticalDLU * 9));
            progressBar.Location = new Point(space, btnCancel.Top - progressBar.Height - btnVertSpace);

            // Panel Preview
            smallBalanceChart.Size     = new Size(ClientSize.Width - 2 * space, 200);
            smallBalanceChart.Location = new Point(space, progressBar.Top - space - smallBalanceChart.Height);

            // Panel Params Base
            pnlParamsBase.Size     = new Size(ClientSize.Width - 2 * space, smallBalanceChart.Top - 2 * space - tsOptimizerButtons.Bottom);
            pnlParamsBase.Location = new Point(space, tsOptimizerButtons.Bottom + space);

            // Panel Params Base 2
            pnlParamsBase2.Size = new Size(pnlParamsBase.Width - 2 * border, pnlParamsBase.Height - pnlCaptions.Height - border);
            pnlParamsBase2.Location = new Point(border, pnlCaptions.Height);

            // Panel Params
            pnlParams.Width = pnlParamsBase2.ClientSize.Width - scrollBar.Width;

            // No params
            lblNoParams.Location = new Point(5, 5);

            // Panel Limitations
            pnlLimitations.Size     = pnlParamsBase.Size;
            pnlLimitations.Location = pnlParamsBase.Location;

            int nudWidth = 55;

            // chbAmbiguousBars
            chbAmbiguousBars.Location = new Point(border + 5, 27);

            // nudAmbiguousBars
            nudAmbiguousBars.Width    = nudWidth;
            nudAmbiguousBars.Location = new Point(pnlLimitations.ClientSize.Width - nudWidth - border - 5, chbAmbiguousBars.Top - 1);

            // MaxDrawdown
            chbMaxDrawdown.Location = new Point(border + 5, chbAmbiguousBars.Bottom + border + 4);
            nudMaxDrawdown.Width    = nudWidth;
            nudMaxDrawdown.Location = new Point(nudAmbiguousBars.Left , chbMaxDrawdown.Top - 1);

            // MaxDrawdown %
            chbEquityPercent.Location = new Point(border + 5, nudMaxDrawdown.Bottom + border + 4);
            nudEquityPercent.Width    = nudWidth;
            nudEquityPercent.Location = new Point(nudAmbiguousBars.Left, chbEquityPercent.Top - 1);

            // MinTrades
            chbMinTrades.Location = new Point(border + 5, chbEquityPercent.Bottom + border + 4);
            nudMinTrades.Width    = nudWidth;
            nudMinTrades.Location = new Point(nudAmbiguousBars.Left, chbMinTrades.Top - 1);

            // MaxTrades
            chbMaxTrades.Location = new Point(border + 5, chbMinTrades.Bottom + border + 4);
            nudMaxTrades.Width    = nudWidth;
            nudMaxTrades.Location = new Point(nudAmbiguousBars.Left, chbMaxTrades.Top - 1);

            // WinLossRatios
            chbWinLossRatio.Location = new Point(border + 5, chbMaxTrades.Bottom + border + 4);
            nudWinLossRatio.Width    = nudWidth;
            nudWinLossRatio.Location = new Point(nudAmbiguousBars.Left, chbWinLossRatio.Top - 1);

            // OOS Pattern Filter
            chbOOSPatternFilter.Location = new Point(border + 5, chbWinLossRatio.Bottom + border + 4);
            nudOOSPatternPercent.Width    = nudWidth;
            nudOOSPatternPercent.Location = new Point(nudAmbiguousBars.Left, chbOOSPatternFilter.Top - 1);

            // Balance lines pattern
            chbSmoothBalanceLines.Location       = new Point(border + 5, chbOOSPatternFilter.Bottom + border + 4);
            nudSmoothBalancePercent.Width        = nudWidth;
            nudSmoothBalancePercent.Location     = new Point(nudAmbiguousBars.Left, chbSmoothBalanceLines.Top - 1);
            nudSmoothBalanceCheckPoints.Width    = nudWidth;
            nudSmoothBalanceCheckPoints.Location = new Point(nudSmoothBalancePercent.Left - nudWidth - border, chbSmoothBalanceLines.Top - 1);

            ////// Panel Settings
            pnlSettings.Size     = pnlParamsBase.Size;
            pnlSettings.Location = pnlParamsBase.Location;

            // Out Of Sample
            chbOutOfSample.Location = new Point(border + 5, 27);
            nudOutOfSample.Width    = nudWidth;
            nudOutOfSample.Location = new Point(pnlSettings.ClientSize.Width - nudWidth - border - 5, chbOutOfSample.Top - 1);

            // chbOptimizerWritesReport
            chbOptimizerWritesReport.Location = new Point(border + 5, chbOutOfSample.Bottom + border + 4);

            // Hide FSB when generator starts
            chbHideFSB.Location = new Point(border + 5, chbOptimizerWritesReport.Bottom + border + 4);

            // Button Reset
            btnReset.Width = pnlSettings.ClientSize.Width - 2 * (border + 5);
            btnReset.Location = new Point(border + 5, pnlSettings.Height - btnReset.Height - border - 2);

            // pnlCaptions
            pnlCaptions.Height = 20;
            pnlCaptions.Invalidate();

            return;
        }

        /// <summary>
        /// Check whether the strategy have been changed.
        /// </summary>
        void Optimizer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isReset)
                SaveOptions();

            if (isOptimizing)
            {   // Cancel the asynchronous operation.
                bgWorker.CancelAsync();
                e.Cancel = true;
            }
            else if (DialogResult == DialogResult.Cancel && isStartegyChanged)
            {
                DialogResult dr = MessageBox.Show(Language.T("Do you want to accept changes to the strategy?"),
                    Language.T("Optimizer"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (dr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (dr == DialogResult.Yes)
                {
                    DialogResult = DialogResult.OK;
                }
                else if (dr == DialogResult.No)
                {
                    DialogResult = DialogResult.Cancel;
                }
            }
            else if (DialogResult == DialogResult.OK && !isStartegyChanged)
            {
                DialogResult = DialogResult.Cancel;
            }

            formFSB.Visible = true;
        }

        /// <summary>
        /// Sets controls in panel Limitations
        /// </summary>
        void SetPanelLimitations()
        {
            chbAmbiguousBars = new CheckBox();
            chbAmbiguousBars.Parent    = pnlLimitations;
            chbAmbiguousBars.ForeColor = colorText;
            chbAmbiguousBars.BackColor = Color.Transparent;
            chbAmbiguousBars.Text      = Language.T("Maximum number of ambiguous bars");
            chbAmbiguousBars.Checked   = false;
            chbAmbiguousBars.AutoSize  = true;

            nudAmbiguousBars = new NumericUpDown();
            nudAmbiguousBars.Parent    = pnlLimitations;
            nudAmbiguousBars.TextAlign = HorizontalAlignment.Center;
            nudAmbiguousBars.BeginInit();
            nudAmbiguousBars.Minimum   = 0;
            nudAmbiguousBars.Maximum   = 100;
            nudAmbiguousBars.Increment = 1;
            nudAmbiguousBars.Value     = 10;
            nudAmbiguousBars.EndInit();

            chbMaxDrawdown = new CheckBox();
            chbMaxDrawdown.Parent    = pnlLimitations;
            chbMaxDrawdown.ForeColor = colorText;
            chbMaxDrawdown.BackColor = Color.Transparent;
            chbMaxDrawdown.Checked   = false;
            chbMaxDrawdown.Text      = Language.T("Maximum equity drawdown") + " [" + (Configs.AccountInMoney ? Configs.AccountCurrency + "]" : Language.T("pips") + "]");
            chbMaxDrawdown.AutoSize  = true;

            nudMaxDrawdown = new NumericUpDown();
            nudMaxDrawdown.Parent    = pnlLimitations;
            nudMaxDrawdown.TextAlign = HorizontalAlignment.Center;
            nudMaxDrawdown.BeginInit();
            nudMaxDrawdown.Minimum   = 0;
            nudMaxDrawdown.Maximum   = Configs.InitialAccount;
            nudMaxDrawdown.Increment = 10;
            nudMaxDrawdown.Value     = Configs.InitialAccount / 4;
            nudMaxDrawdown.EndInit();

            chbEquityPercent = new CheckBox();
            chbEquityPercent.Parent    = pnlLimitations;
            chbEquityPercent.ForeColor = colorText;
            chbEquityPercent.BackColor = Color.Transparent;
            chbEquityPercent.Text      = Language.T("Maximum equity drawdown") + " [% " + Configs.AccountCurrency + "]";
            chbEquityPercent.Checked   = false;
            chbEquityPercent.AutoSize  = true;

            nudEquityPercent = new NumericUpDown();
            nudEquityPercent.Parent    = pnlLimitations;
            nudEquityPercent.TextAlign = HorizontalAlignment.Center;
            nudEquityPercent.BeginInit();
            nudEquityPercent.Minimum   = 1;
            nudEquityPercent.Maximum   = 100;
            nudEquityPercent.Increment = 1;
            nudEquityPercent.Value     = 25;
            nudEquityPercent.EndInit();

            chbMinTrades = new CheckBox();
            chbMinTrades.Parent    = pnlLimitations;
            chbMinTrades.ForeColor = colorText;
            chbMinTrades.BackColor = Color.Transparent;
            chbMinTrades.Text      = Language.T("Minimum number of trades");
            chbMinTrades.Checked   = true;
            chbMinTrades.AutoSize  = true;

            nudMinTrades = new NumericUpDown();
            nudMinTrades.Parent    = pnlLimitations;
            nudMinTrades.TextAlign = HorizontalAlignment.Center;
            nudMinTrades.BeginInit();
            nudMinTrades.Minimum   = 10;
            nudMinTrades.Maximum   = 1000;
            nudMinTrades.Increment = 10;
            nudMinTrades.Value     = 100;
            nudMinTrades.EndInit();

            chbMaxTrades = new CheckBox();
            chbMaxTrades.Parent    = pnlLimitations;
            chbMaxTrades.ForeColor = colorText;
            chbMaxTrades.BackColor = Color.Transparent;
            chbMaxTrades.Text      = Language.T("Maximum number of trades");
            chbMaxTrades.Checked   = false;
            chbMaxTrades.AutoSize  = true;

            nudMaxTrades = new NumericUpDown();
            nudMaxTrades.Parent    = pnlLimitations;
            nudMaxTrades.TextAlign = HorizontalAlignment.Center;
            nudMaxTrades.BeginInit();
            nudMaxTrades.Minimum   = 10;
            nudMaxTrades.Maximum   = 10000;
            nudMaxTrades.Increment = 10;
            nudMaxTrades.Value     = 1000;
            nudMaxTrades.EndInit();

            chbWinLossRatio = new CheckBox();
            chbWinLossRatio.Parent    = pnlLimitations;
            chbWinLossRatio.ForeColor = colorText;
            chbWinLossRatio.BackColor = Color.Transparent;
            chbWinLossRatio.Text      = Language.T("Minimum win / loss trades ratio");
            chbWinLossRatio.Checked   = false;
            chbWinLossRatio.AutoSize  = true;

            nudWinLossRatio = new NumericUpDown();
            nudWinLossRatio.Parent    = pnlLimitations;
            nudWinLossRatio.TextAlign = HorizontalAlignment.Center;
            nudWinLossRatio.BeginInit();
            nudWinLossRatio.Minimum       = 0.10M;
            nudWinLossRatio.Maximum       = 1;
            nudWinLossRatio.Increment     = 0.01M;
            nudWinLossRatio.Value         = 0.30M;
            nudWinLossRatio.DecimalPlaces = 2;
            nudWinLossRatio.EndInit();

            chbOOSPatternFilter = new CheckBox();
            chbOOSPatternFilter.Parent    = pnlLimitations;
            chbOOSPatternFilter.ForeColor = colorText;
            chbOOSPatternFilter.BackColor = Color.Transparent;
            chbOOSPatternFilter.Text      = Language.T("Filter bad OOS performance");
            chbOOSPatternFilter.Checked   = false;
            chbOOSPatternFilter.AutoSize  = true;

            nudOOSPatternPercent = new NumericUpDown();
            nudOOSPatternPercent.Parent    = pnlLimitations;
            nudOOSPatternPercent.TextAlign = HorizontalAlignment.Center;
            nudOOSPatternPercent.BeginInit();
            nudOOSPatternPercent.Minimum = 1;
            nudOOSPatternPercent.Maximum = 50;
            nudOOSPatternPercent.Value   = 20;
            nudOOSPatternPercent.EndInit();
            toolTip.SetToolTip(nudOOSPatternPercent, Language.T("Deviation percent."));

            chbSmoothBalanceLines = new CheckBox();
            chbSmoothBalanceLines.Parent    = pnlLimitations;
            chbSmoothBalanceLines.ForeColor = colorText;
            chbSmoothBalanceLines.BackColor = Color.Transparent;
            chbSmoothBalanceLines.Text      = Language.T("Filter non-linear balance pattern");
            chbSmoothBalanceLines.Checked   = false;
            chbSmoothBalanceLines.AutoSize  = true;

            nudSmoothBalancePercent = new NumericUpDown();
            nudSmoothBalancePercent.Parent    = pnlLimitations;
            nudSmoothBalancePercent.TextAlign = HorizontalAlignment.Center;
            nudSmoothBalancePercent.BeginInit();
            nudSmoothBalancePercent.Minimum = 1;
            nudSmoothBalancePercent.Maximum = 50;
            nudSmoothBalancePercent.Value   = 20;
            nudSmoothBalancePercent.EndInit();
            toolTip.SetToolTip(nudSmoothBalancePercent, Language.T("Deviation percent."));

            nudSmoothBalanceCheckPoints = new NumericUpDown();
            nudSmoothBalanceCheckPoints.Parent    = pnlLimitations;
            nudSmoothBalanceCheckPoints.TextAlign = HorizontalAlignment.Center;
            nudSmoothBalanceCheckPoints.BeginInit();
            nudSmoothBalanceCheckPoints.Minimum = 1;
            nudSmoothBalanceCheckPoints.Maximum = 50;
            nudSmoothBalanceCheckPoints.Value   = 1;
            nudSmoothBalanceCheckPoints.EndInit();
            toolTip.SetToolTip(nudSmoothBalanceCheckPoints, Language.T("Check points count."));

            return;
        }

        /// <summary>
        /// Sets controls in panel Settings
        /// </summary>
        void SetPanelSettings()
        {
            chbOutOfSample = new CheckBox();
            chbOutOfSample.Parent    = pnlSettings;
            chbOutOfSample.ForeColor = colorText;
            chbOutOfSample.BackColor = Color.Transparent;
            chbOutOfSample.Text      = Language.T("Out of sample testing, percent of OOS bars");
            chbOutOfSample.Checked   = false;
            chbOutOfSample.AutoSize  = true;
            chbOutOfSample.CheckedChanged += new EventHandler(ChbOutOfSample_CheckedChanged);

            nudOutOfSample = new NumericUpDown();
            nudOutOfSample.Parent    = pnlSettings;
            nudOutOfSample.TextAlign = HorizontalAlignment.Center;
            nudOutOfSample.BeginInit();
            nudOutOfSample.Minimum   = 10;
            nudOutOfSample.Maximum   = 60;
            nudOutOfSample.Increment = 1;
            nudOutOfSample.Value     = 30;
            nudOutOfSample.EndInit();
            nudOutOfSample.ValueChanged += new EventHandler(NudOutOfSample_ValueChanged);

            chbOptimizerWritesReport = new CheckBox();
            chbOptimizerWritesReport.Parent    = pnlSettings;
            chbOptimizerWritesReport.ForeColor = colorText;
            chbOptimizerWritesReport.BackColor = Color.Transparent;
            chbOptimizerWritesReport.Text      = Language.T("Optimizer writes a report for each optimized strategy");
            chbOptimizerWritesReport.Checked   = false;
            chbOptimizerWritesReport.AutoSize  = true;

            chbHideFSB = new CheckBox();
            chbHideFSB.Parent    = pnlSettings;
            chbHideFSB.ForeColor = colorText;
            chbHideFSB.BackColor = Color.Transparent;
            chbHideFSB.Text      = Language.T("Hide FSB when Optimizer starts");
            chbHideFSB.Checked = true;
            chbHideFSB.AutoSize  = true;

            btnReset = new Button();
            btnReset.Parent = pnlSettings;
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Text = Language.T("Reset all parameters and settings");
            btnReset.Click += new EventHandler(BtnReset_Click);
        }

        /// <summary>
        /// Sets ups the chart's buttons.
        /// </summary>
        void SetupButtons()
        {
            tsOptimizerButtons = new ToolStrip();
            tsOptimizerButtons.Parent = this;

            aOptimizerButtons = new ToolStripButton[Enum.GetValues(typeof(OptimizerButtons)).Length];
            for (int i = 0; i < aOptimizerButtons.Length; i++)
            {
                aOptimizerButtons[i] = new ToolStripButton();
                aOptimizerButtons[i].Tag = (OptimizerButtons)i;
                aOptimizerButtons[i].Click += new EventHandler(Buttons_Click);
                tsOptimizerButtons.Items.Add(aOptimizerButtons[i]);
                if (i < 3)
                    aOptimizerButtons[i].DisplayStyle = ToolStripItemDisplayStyle.Image;
                else
                    aOptimizerButtons[i].DisplayStyle = ToolStripItemDisplayStyle.Text;
                if (i == 2 || i == 5)
                    tsOptimizerButtons.Items.Add(new ToolStripSeparator());
            }

            // Select All
            aOptimizerButtons[(int)OptimizerButtons.SelectAll].Image = Properties.Resources.optimizer_select_all;
            aOptimizerButtons[(int)OptimizerButtons.SelectAll].ToolTipText = Language.T("Select all parameters.");

            // Select None
            aOptimizerButtons[(int)OptimizerButtons.SelectNone].Image = Properties.Resources.optimizer_select_none;
            aOptimizerButtons[(int)OptimizerButtons.SelectNone].ToolTipText = Language.T("Select none of the parameters.");

            // Select Random
            aOptimizerButtons[(int)OptimizerButtons.SelectRandom].Image = Properties.Resources.optimizer_select_random;
            aOptimizerButtons[(int)OptimizerButtons.SelectRandom].ToolTipText = Language.T("Select a random number of parameters.");

            // Set step 5
            aOptimizerButtons[(int)OptimizerButtons.SetStep5].Text = "±5";
            aOptimizerButtons[(int)OptimizerButtons.SetStep5].ToolTipText = Language.T("Set Min / Max ± 5 steps.");

            // Set step 10
            aOptimizerButtons[(int)OptimizerButtons.SetStep10].Text = "±10";
            aOptimizerButtons[(int)OptimizerButtons.SetStep10].ToolTipText = Language.T("Set Min / Max ± 10 steps.");

            // Set step 15
            aOptimizerButtons[(int)OptimizerButtons.SetStep15].Text = "±15";
            aOptimizerButtons[(int)OptimizerButtons.SetStep15].ToolTipText = Language.T("Set Min / Max ± 15 steps.");

            // Show Params
            aOptimizerButtons[(int)OptimizerButtons.ShowParams].Text = Language.T("Parameters");
            aOptimizerButtons[(int)OptimizerButtons.ShowParams].ToolTipText = Language.T("Show indicator parameters.");
            aOptimizerButtons[(int)OptimizerButtons.ShowParams].Enabled = false;

            // Show Limitations
            aOptimizerButtons[(int)OptimizerButtons.ShowLimitations].Text =  Language.T("Limitations");
            aOptimizerButtons[(int)OptimizerButtons.ShowLimitations].ToolTipText = Language.T("Show strategy limitations.");

            // Show Settings
            aOptimizerButtons[(int)OptimizerButtons.ShowSettings].Text =  Language.T("Settings");
            aOptimizerButtons[(int)OptimizerButtons.ShowSettings].ToolTipText = Language.T("Show optimizer settings.");

            return;
        }

        /// <summary>
        /// Loads and parses the generator's options.
        /// </summary>
        void LoadOptions()
        {
            if (string.IsNullOrEmpty(Configs.OptimizerOptions))
                return;

            string[] options = Configs.OptimizerOptions.Split(';');
            int i = 0;

            try {
                chbOutOfSample.Checked            = bool.Parse(options[i++]);
                nudOutOfSample.Value              = int.Parse(options[i++]);
                chbAmbiguousBars.Checked          = bool.Parse(options[i++]);
                nudAmbiguousBars.Value            = int.Parse(options[i++]);
                chbMaxDrawdown.Checked            = bool.Parse(options[i++]);
                nudMaxDrawdown.Value              = int.Parse(options[i++]);
                chbMinTrades.Checked              = bool.Parse(options[i++]);
                nudMinTrades.Value                = int.Parse(options[i++]);
                chbMaxTrades.Checked              = bool.Parse(options[i++]);
                nudMaxTrades.Value                = int.Parse(options[i++]);
                chbWinLossRatio.Checked           = bool.Parse(options[i++]);
                nudWinLossRatio.Value             = int.Parse(options[i++]) / 100M;
                chbEquityPercent.Checked          = bool.Parse(options[i++]);
                nudEquityPercent.Value            = int.Parse(options[i++]);
                chbOOSPatternFilter.Checked       = bool.Parse(options[i++]);
                nudOOSPatternPercent.Value        = int.Parse(options[i++]);
                chbSmoothBalanceLines.Checked     = bool.Parse(options[i++]);
                nudSmoothBalancePercent.Value     = int.Parse(options[i++]);
                nudSmoothBalanceCheckPoints.Value = int.Parse(options[i++]);
                chbOptimizerWritesReport.Checked  = bool.Parse(options[i++]);
                chbHideFSB.Checked                = bool.Parse(options[i++]);
            }
            catch
            {
            }

            return;
        }

        /// <summary>
        /// Saves the generator's options.
        /// </summary>
        void SaveOptions()
        {
            string options =
            chbOutOfSample.Checked.ToString()                + ";" +
            nudOutOfSample.Value.ToString()                  + ";" +
            chbAmbiguousBars.Checked.ToString()              + ";" +
            nudAmbiguousBars.Value.ToString()                + ";" +
            chbMaxDrawdown.Checked.ToString()                + ";" +
            nudMaxDrawdown.Value.ToString()                  + ";" +
            chbMinTrades.Checked.ToString()                  + ";" +
            nudMinTrades.Value.ToString()                    + ";" +
            chbMaxTrades.Checked.ToString()                  + ";" +
            nudMaxTrades.Value.ToString()                    + ";" +
            chbWinLossRatio.Checked.ToString()               + ";" +
            ((int)(nudWinLossRatio.Value * 100M)).ToString() + ";" +
            chbEquityPercent.Checked.ToString()              + ";" +
            nudEquityPercent.Value.ToString()                + ";" +
            chbOOSPatternFilter.Checked.ToString()           + ";" +
            nudOOSPatternPercent.Value.ToString()            + ";" +
            chbSmoothBalanceLines.Checked.ToString()         + ";" +
            nudSmoothBalancePercent.Value.ToString()         + ";" +
            nudSmoothBalanceCheckPoints.Value.ToString()     + ";" +
            chbOptimizerWritesReport.Checked.ToString()      + ";" +
            chbHideFSB.Checked.ToString();

            Configs.OptimizerOptions = options;

            return;
        }

        /// <summary>
        /// Paints pnlCaptions
        /// </summary>
        void PnlCaptions_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            Panel pnl = (Panel)sender;
            Graphics g = e.Graphics;

            Data.GradientPaint(g, pnl.ClientRectangle, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            Brush brush = new SolidBrush(pnl.ForeColor);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;

            g.DrawString(Language.T("Parameter"), Font, brush, 22,  3);
            g.DrawString(Language.T("Value"),     Font, brush, 175, 3, stringFormat);
            g.DrawString(Language.T("Minimum"),   Font, brush, 210, 3);
            g.DrawString(Language.T("Maximum"),   Font, brush, 283, 3);
            g.DrawString(Language.T("Step"),      Font, brush, 370, 3);

            return;
        }

        /// <summary>
        /// Paints PnlParamsBase
        /// </summary>
        void PnlParamsBase_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = (Panel)sender;
            Graphics g = e.Graphics;
            Pen penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);

            g.DrawLine(penBorder, 1, 0, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - border + 1, 0, pnl.ClientSize.Width - border + 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - border + 1, pnl.ClientSize.Width, pnl.ClientSize.Height - border + 1);

            return;
        }

        /// <summary>
        /// Sets the parameters
        /// </summary>
        void SetIndicatorParams()
        {
            int checkedChBoxes = 0;

            // Counts the strategy's numeric parameters and Indicators
            indicators = 0;
            parameters = 0;
            int indicatorSlot = -1;
            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
                for (int numParam = 0; numParam < 6; numParam++)
                    if (Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled)
                    {
                        parameters++;
                        if (indicatorSlot != slot)
                        {
                            indicatorSlot = slot;
                            indicators++;
                        }
                    }

            aParameter         = new Parameter[parameters];
            achbxParameterName = new CheckBox[parameters];
            alblParameterValue = new Label[parameters];
            anudParameterMin   = new NumericUpDown[parameters];
            anudParameterMax   = new NumericUpDown[parameters];
            anudParameterStep  = new NumericUpDown[parameters];
            alblIndicatorName  = new Label[parameters];

            // Fill the lines
            indicatorSlot = -1;
            int param = 0;
            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                for (int numParam = 0; numParam < 6; numParam++)
                {
                    if (Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled)
                    {
                        if (indicatorSlot != slot)
                        {
                            indicatorSlot = slot;
                            alblIndicatorName[param] = new Label();
                            alblIndicatorName[param].Parent    = pnlParams;
                            alblIndicatorName[param].Text      = Data.Strategy.Slot[slot].IndicatorName;
                            alblIndicatorName[param].Font      = fontIndicator;
                            alblIndicatorName[param].ForeColor = LayoutColors.ColorSlotIndicatorText;
                            alblIndicatorName[param].BackColor = Color.Transparent;
                            alblIndicatorName[param].Height    = 18;
                            alblIndicatorName[param].Width     = 200;
                        }

                        aParameter[param] = new Parameter(slot, numParam);

                        achbxParameterName[param] = new CheckBox();
                        achbxParameterName[param].ForeColor = colorText;
                        achbxParameterName[param].BackColor = Color.Transparent;
                        achbxParameterName[param].Text      = aParameter[param].ParameterName;
                        achbxParameterName[param].CheckedChanged += new EventHandler(Optimizer_CheckedChanged);

                        // Auto checked
                        if (Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Caption == "Level")
                        {
                            if (Data.Strategy.Slot[slot].IndParam.ListParam[0].Text.Contains("Level"))
                            {
                                if (random.Next(100) > 20 && checkedChBoxes < 6)
                                {
                                    achbxParameterName[param].Checked = true;
                                    checkedChBoxes++;
                                }
                            }
                            else
                            {
                                achbxParameterName[param].Checked = false;
                            }
                        }
                        else
                        {
                            if (random.Next(100) > 40 && checkedChBoxes < 6)
                            {
                                achbxParameterName[param].Checked = true;
                                checkedChBoxes++;
                            }
                        }

                        int    point = aParameter[param].Point;
                        double value = aParameter[param].Value;
                        double min   = aParameter[param].Minimum;
                        double max   = aParameter[param].Maximum;
                        double step  = Math.Round(Math.Pow(10, -point), point);
                        string stringFormat = "{0:F" + point.ToString() + "}";

                        alblParameterValue[param] = new Label();
                        alblParameterValue[param].ForeColor = colorText;
                        alblParameterValue[param].BackColor = Color.Transparent;
                        alblParameterValue[param].Text      = string.Format(stringFormat, value);

                        fontParamValueRegular = alblParameterValue[param].Font;
                        fontParamValueBold    = new Font(fontParamValueRegular, FontStyle.Bold);

                        anudParameterMin[param] = new NumericUpDown();
                        anudParameterMin[param].BeginInit();
                        anudParameterMin[param].Minimum       = (decimal)min;
                        anudParameterMin[param].Maximum       = (decimal)max;
                        anudParameterMin[param].Value         = (decimal)Math.Round(Math.Max(value - 5 * step, min), point);
                        anudParameterMin[param].DecimalPlaces = point;
                        anudParameterMin[param].Increment     = (decimal)step;
                        anudParameterMin[param].EndInit();
                        toolTip.SetToolTip(anudParameterMin[param], Language.T("Minimum value."));

                        anudParameterMax[param] = new NumericUpDown();
                        anudParameterMax[param].BeginInit();
                        anudParameterMax[param].Minimum       = (decimal)min;
                        anudParameterMax[param].Maximum       = (decimal)max;
                        anudParameterMax[param].Value         = (decimal)Math.Round(Math.Min(value + 5 * step, max), point);
                        anudParameterMax[param].DecimalPlaces = point;
                        anudParameterMax[param].Increment     = (decimal)step;
                        anudParameterMax[param].EndInit();
                        toolTip.SetToolTip(anudParameterMax[param], Language.T("Maximum value."));

                        anudParameterStep[param] = new NumericUpDown();
                        anudParameterStep[param].BeginInit();
                        anudParameterStep[param].Minimum       = (decimal)step;
                        anudParameterStep[param].Maximum       = (decimal)Math.Max(step, Math.Abs(max - min));
                        anudParameterStep[param].Value         = (decimal)step;
                        anudParameterStep[param].DecimalPlaces = point;
                        anudParameterStep[param].Increment     = (decimal)step;
                        anudParameterStep[param].EndInit();
                        toolTip.SetToolTip(anudParameterStep[param], Language.T("Step of change."));


                        achbxParameterName[param].Parent    = pnlParams;
                        achbxParameterName[param].Width     = 140;
                        achbxParameterName[param].TextAlign = ContentAlignment.MiddleLeft;

                        alblParameterValue[param].Parent    = pnlParams;
                        alblParameterValue[param].Width     = 50;
                        alblParameterValue[param].TextAlign = ContentAlignment.MiddleCenter;

                        anudParameterMin[param].Parent    = pnlParams;
                        anudParameterMin[param].Width     = 70;
                        anudParameterMin[param].TextAlign = HorizontalAlignment.Center;

                        anudParameterMax[param].Parent    = pnlParams;
                        anudParameterMax[param].Width     = 70;
                        anudParameterMax[param].TextAlign = HorizontalAlignment.Center;

                        anudParameterStep[param].Parent    = pnlParams;
                        anudParameterStep[param].Width     = 70;
                        anudParameterStep[param].TextAlign = HorizontalAlignment.Center;

                        param++;
                    }
                }
            }

            int totalHeight = ArrangeControls();

            if (parameters == 0)
            {
                lblNoParams.Visible = true;
                pnlParams.Height = lblNoParams.Bottom;
            }
            else
            {
                pnlParams.Height = totalHeight;
            }

            btnOptimize.Focus();

            return;
        }

        /// <summary>
        /// Sets parameters' Min / Max values
        /// </summary>
        void SetParamsMinMax(int deltaStep)
        {
            for (int param = 0; param < parameters; param++)
            {
                int    point = aParameter[param].Point;
                double value = aParameter[param].Value;
                double min   = aParameter[param].Minimum;
                double max   = aParameter[param].Maximum;
                double step  = Math.Round(Math.Pow(10, -point), point);

                anudParameterMin[param].Value = (decimal)Math.Round(Math.Max(value - deltaStep * step, min), point);
                anudParameterMax[param].Value = (decimal)Math.Round(Math.Min(value + deltaStep * step, max), point);
            }
        }

        /// <summary>
        /// Select / unselect parameters
        /// </summary>
        void SelectParameters(OptimizerButtons button)
        {
            for (int param = 0; param < parameters; param++)
            {
                if (button == OptimizerButtons.SelectAll)
                    achbxParameterName[param].Checked = true;
                else if (button == OptimizerButtons.SelectNone)
                    achbxParameterName[param].Checked = false;
                else if (button == OptimizerButtons.SelectRandom)
                    achbxParameterName[param].Checked = random.Next(100) > 40;
            }
        }

        /// <summary>
        /// Check Box checked changed
        /// </summary>
        void Optimizer_CheckedChanged(object sender, EventArgs e)
        {
            btnOptimize.Focus();
        }

        /// <summary>
        /// Arranges the controls into the pnlParams
        /// </summary>
        void PnlParamsBase2_Resize(object sender, EventArgs e)
        {
            if (pnlParams.Height > pnlParamsBase2.Height)
            {
                scrollBar.Maximum     = pnlParams.Height - pnlParamsBase2.Height + 40;
                scrollBar.Value       = 0;
                scrollBar.SmallChange = 20;
                scrollBar.LargeChange = 40;
                scrollBar.Visible     = true;
            }
            else
            {
                scrollBar.Visible = false;
                scrollBar.Minimum = 0;
                scrollBar.Maximum = 0;
                scrollBar.Value   = 0;
            }

            pnlParams.Location = new Point(0, -scrollBar.Value);

            return;
        }

        /// <summary>
        /// Arranges the controls
        /// </summary>
        int ArrangeControls()
        {
            int vertMargin   = 3;
            int horizMargin  = 5;
            int vertPosition = vertMargin - scrollBar.Value;
            int totalHeight  = vertMargin;

            int slot = -1;
            for (int param = 0; param < parameters; param++)
            {
                if (slot != aParameter[param].SlotNumber)
                {
                    alblIndicatorName[param].Location = new Point(horizMargin, vertPosition);
                    slot = aParameter[param].SlotNumber;
                    vertPosition += alblIndicatorName[param].Height + vertMargin;
                    totalHeight  += alblIndicatorName[param].Height + vertMargin;
                }
                achbxParameterName[param].Location = new Point(horizMargin, vertPosition);
                alblParameterValue[param].Location = new Point(achbxParameterName[param].Right + horizMargin, vertPosition - 1);
                anudParameterMin[param].Location   = new Point(alblParameterValue[param].Right + horizMargin, vertPosition + 2);
                anudParameterMax[param].Location   = new Point(anudParameterMin[param].Right   + horizMargin, vertPosition + 2);
                anudParameterStep[param].Location  = new Point(anudParameterMax[param].Right   + horizMargin, vertPosition + 2);
                vertPosition += achbxParameterName[param].Height + vertMargin;
                totalHeight  += achbxParameterName[param].Height + vertMargin;
            }

            return totalHeight;
        }

        /// <summary>
        /// Invalidate the Panel Params
        /// <summary>
        void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            pnlParams.Location = new Point(0, -scrollBar.Value);
        }

        /// <summary>
        /// Shift the pnlParams viewpoint
        /// <summary>
        void ScrollBar_MouseWheel(object sender, MouseEventArgs e)
        {
            if (scrollBar.Visible)
            {
                int iNewValue = scrollBar.Value - e.Delta / 120;

                if (iNewValue < scrollBar.Minimum)
                    scrollBar.Value = scrollBar.Minimum;
                else if (iNewValue > scrollBar.Maximum)
                    scrollBar.Value = scrollBar.Maximum;
                else
                    scrollBar.Value = iNewValue;
            }
        }

        /// <summary>
        /// Optimize.
        /// </summary>
        void BtnOptimize_Click(object sender, EventArgs e)
        {
            if (isOptimizing)
            {   // Cancel the asynchronous operation.
                bgWorker.CancelAsync();
                return;
            }

            if (chbOptimizerWritesReport.Checked)
                InitReport();

            // Counts the checked params
            checkedParams = 0;
            for (int i = 0; i < parameters; i++)
                if (achbxParameterName[i].Checked)
                    checkedParams++;

            // If there are no checked returns
            if (checkedParams < 1)
            {
                System.Media.SystemSounds.Hand.Play();
                return;
            }

            // Contains the checked params only
            aiChecked = new int[checkedParams];
            int indexChecked = 0;
            for (int i = 0; i < parameters; i++)
                if (achbxParameterName[i].Checked)
                    aiChecked[indexChecked++] = i;

            SetNecessaryCycles();

            Cursor                 = Cursors.WaitCursor;
            progressBar.Value      = 1;
            progressPercent        = 0;
            computedCycles         = 0;
            isOptimizing           = true;
            btnCancel.Enabled      = false;
            btnAccept.Enabled      = false;
            btnOptimize.Text       = Language.T("Stop");

            for (int i = 0; i <= (int)OptimizerButtons.SetStep15; i++)
                aOptimizerButtons[i].Enabled = false;

            foreach (Control control in pnlParams.Controls)
                if (control.GetType() != typeof(Label))
                    control.Enabled = false;

            foreach (Control control in pnlLimitations.Controls)
                control.Enabled = false;

            foreach (Control control in pnlSettings.Controls)
                control.Enabled = false;

            // Start the bgWorker
            bgWorker.RunWorkerAsync();

            return;
        }

        /// <summary>
        /// Counts the necessary optimization cycles.
        /// </summary>
        void SetNecessaryCycles()
        {
            cycles = 0;
            for (int i = 0; i < checkedParams; i++)
            {
                double min  = (double)anudParameterMin[aiChecked[i]].Value;
                double max  = (double)anudParameterMax[aiChecked[i]].Value;
                double step = (double)anudParameterStep[aiChecked[i]].Value;

                for (double value = min; value <= max; value += step)
                    cycles += 1;
            }

            for (int i = 0; i < checkedParams - 1; i++)
            {
                for (int j = 0; j < checkedParams; j++)
                {
                    if (i < j)
                    {
                        double min1  = (double)anudParameterMin[aiChecked[i]].Value;
                        double max1  = (double)anudParameterMax[aiChecked[i]].Value;
                        double step1 = (double)anudParameterStep[aiChecked[i]].Value;
                        double min2  = (double)anudParameterMin[aiChecked[j]].Value;
                        double max2  = (double)anudParameterMax[aiChecked[j]].Value;
                        double step2 = (double)anudParameterStep[aiChecked[j]].Value;

                        for (double value1 = min1; value1 <= max1; value1 += step1)
                            for (double value2 = min2; value2 <= max2; value2 += step2)
                                cycles += 1;
                    }
                }
            }

            return;
        }

        /// <summary>
        /// Does the job
        /// </summary>
        void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // Optimize all Parameters
            OptimizeParams(worker, e);
        }

        /// <summary>
        /// This event handler updates the progress bar.
        /// </summary>
        void BgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// This event handler deals with the results of the background operation.
        /// </summary>
        void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Backtester.Calculate();
            Backtester.CalculateAccountStats();
            smallBalanceChart.SetChartData();
            smallBalanceChart.InitChart();
            smallBalanceChart.Invalidate();

            if (!e.Cancelled && Configs.PlaySounds)
                System.Media.SystemSounds.Exclamation.Play();

            isOptimizing           = false;
            btnCancel.Enabled      = true;
            btnAccept.Enabled      = true;
            btnOptimize.Text       = Language.T("Optimize");
            progressBar.Value      = 1;

            if(pnlParams.Visible == true)
                for (int i = 0; i <= (int)OptimizerButtons.SetStep15; i++)
                    aOptimizerButtons[i].Enabled = true;

            for (int i = 0; i < parameters; i++)
                alblParameterValue[i].Text = Math.Round(aParameter[i].BestValue, aParameter[i].Point).ToString();

            foreach (Control control in pnlParams.Controls)
                control.Enabled = true;

            foreach (Control control in pnlLimitations.Controls)
                control.Enabled = true;

            foreach (Control control in pnlSettings.Controls)
                control.Enabled = true;

            if (chbOptimizerWritesReport.Checked)
                SaveReport();

            Cursor = Cursors.Default;

            return;
        }

        /// <summary>
        /// Saves the Generator History
        /// </summary>
        void SetStrategyToGeneratorHistory()
        {
            Data.GeneratorHistory.Add(Data.Strategy.Clone());

            if (Data.GeneratorHistory.Count >= 110)
                Data.GeneratorHistory.RemoveRange(0, 10);

            Data.GenHistoryIndex = Data.GeneratorHistory.Count - 1;
        }

        /// <summary>
        /// Optimize all the checked parameters
        /// </summary>
        void OptimizeParams(BackgroundWorker worker, DoWorkEventArgs e)
        {
            int bestBalance = (isOOS ? Backtester.Balance(barOOS) : Backtester.NetBalance);

            // First Optimization Cycle
            for (int round = 0; round < checkedParams && isOptimizing; round++)
            {
                if (worker.CancellationPending) break;

                int param = aiChecked[round];

                double min  = (double)anudParameterMin[param].Value;
                double max  = (double)anudParameterMax[param].Value;
                double step = (double)anudParameterStep[param].Value;

                for (double value = min; value <= max; value += step)
                {
                    if (worker.CancellationPending) break;

                    aParameter[param].Value = value;
                    CalculateIndicator(aParameter[param].SlotNumber);
                    Backtester.Calculate();
                    Backtester.CalculateAccountStats();
                    if (chbOptimizerWritesReport.Checked)
                        FillInReport();

                    int balance = isOOS ? Backtester.Balance(barOOS) : Backtester.NetBalance;
                    if (balance > bestBalance && IsLimitationsFulfilled())
                    {
                        bestBalance = balance;
                        aParameter[param].BestValue = value;
                        ShowParamBestValue(param);
                        smallBalanceChart.SetChartData();
                        smallBalanceChart.InitChart();
                        smallBalanceChart.Invalidate();
                        isStartegyChanged = true;
                        SetStrategyToGeneratorHistory();
                    }

                    // Report progress as a percentage of the total task.
                    computedCycles++;
                    int percentComplete = 100 * computedCycles / cycles;
                    percentComplete = percentComplete > 100 ? 100 : percentComplete;
                    if (percentComplete > progressPercent)
                    {
                        progressPercent = percentComplete;
                        worker.ReportProgress(percentComplete);
                    }
                }

                aParameter[param].Value = aParameter[param].BestValue;
                CalculateIndicator(aParameter[param].SlotNumber);
                Backtester.Calculate();
                Backtester.CalculateAccountStats();
            }

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            if (checkedParams < 2)
                return;

            // Counts the necessary round
            int rounds= 0;
            for (int i = 0; i < checkedParams - 1; i++)
            for (int j = 0; j < checkedParams    ; j++)
                if (i < j) rounds++;

            CoupleOfParams[] aCP     = new CoupleOfParams[rounds];
            CoupleOfParams[] aCPTemp = new CoupleOfParams[rounds];

            rounds = 0;
            for (int i = 0; i < checkedParams - 1; i++)
            for (int j = 0; j < checkedParams    ; j++)
                if (i < j)
                {
                    aCPTemp[rounds].Param1 = aiChecked[i];
                    aCPTemp[rounds].Param2 = aiChecked[j];
                    aCPTemp[rounds].IsPassed = false;
                    rounds++;
                }

            // Shaking the params
            for (int round = 0; round < rounds; round++)
            {
                int couple = 0;
                do
                {
                    couple = random.Next(rounds);
                } while (aCPTemp[couple].IsPassed);
                aCPTemp[couple].IsPassed = true;
                aCP[round] = aCPTemp[couple];
            }

            // The Optimization Cycle
            for (int round = 0; round < rounds; round++)
            {
                if (worker.CancellationPending) break;

                int  param1 = aCP[round].Param1;
                int  param2 = aCP[round].Param2;
                bool isOneIndicator = (aParameter[param1].IP.IndicatorName == aParameter[param2].IP.IndicatorName);

                double min1  = (double)anudParameterMin[param1].Value;
                double max1  = (double)anudParameterMax[param1].Value;
                double step1 = (double)anudParameterStep[param1].Value;

                double min2  = (double)anudParameterMin[param2].Value;
                double max2  = (double)anudParameterMax[param2].Value;
                double step2 = (double)anudParameterStep[param2].Value;

                for (double value1 = min1; value1 <= max1; value1 += step1)
                {
                    if (worker.CancellationPending) break;

                    if (!isOneIndicator)
                    {
                        aParameter[param1].Value = value1;
                        CalculateIndicator(aParameter[param1].SlotNumber);
                    }

                    for (double value2 = min2; value2 <= max2; value2 += step2)
                    {
                        if (worker.CancellationPending) break;

                        if (isOneIndicator)
                        {
                            aParameter[param1].Value = value1;
                            aParameter[param2].Value = value2;
                            CalculateIndicator(aParameter[param1].SlotNumber);
                        }
                        else
                        {
                            aParameter[param2].Value = value2;
                            CalculateIndicator(aParameter[param2].SlotNumber);
                        }

                        // Calculates the Strategy
                        Backtester.Calculate();
                        Backtester.CalculateAccountStats();
                        if (chbOptimizerWritesReport.Checked)
                            FillInReport();

                        int balance = isOOS ? Backtester.Balance(barOOS) : Backtester.NetBalance;
                        if (balance > bestBalance && IsLimitationsFulfilled())
                        {
                            bestBalance = balance;
                            aParameter[param1].BestValue = value1;
                            aParameter[param2].BestValue = value2;
                            ShowParamBestValue(param1);
                            ShowParamBestValue(param2);
                            smallBalanceChart.SetChartData();
                            smallBalanceChart.InitChart();
                            smallBalanceChart.Invalidate();
                            isStartegyChanged = true;
                            SetStrategyToGeneratorHistory();
                        }

                        // Report progress as a percentage of the total task.
                        computedCycles++;
                        int percentComplete = 100 * computedCycles / cycles;
                        percentComplete = percentComplete > 100 ? 100 : percentComplete;
                        if (percentComplete > progressPercent)
                        {
                            progressPercent = percentComplete;
                            worker.ReportProgress(percentComplete);
                        }
                    }
                }

                aParameter[param1].Value = aParameter[param1].BestValue;
                aParameter[param2].Value = aParameter[param2].BestValue;

                if (isOneIndicator)
                {
                    CalculateIndicator(aParameter[param1].SlotNumber);
                }
                else
                {
                    CalculateIndicator(aParameter[param1].SlotNumber);
                    CalculateIndicator(aParameter[param2].SlotNumber);
                }

                Backtester.Calculate();
                Backtester.CalculateAccountStats();
            }

            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }

            return;
        }

        /// <summary>
        /// Calculates the indicator in the designated slot
        /// </summary>
        void CalculateIndicator(int slot)
        {
            IndicatorParam ip = Data.Strategy.Slot[slot].IndParam;

            Indicator indicator = Indicator_Store.ConstructIndicator(ip.IndicatorName, ip.SlotType);

            // List params
            for (int i = 0; i < 5; i++)
            {
                indicator.IndParam.ListParam[i].Index   = ip.ListParam[i].Index;
                indicator.IndParam.ListParam[i].Text    = ip.ListParam[i].Text;
                indicator.IndParam.ListParam[i].Enabled = ip.ListParam[i].Enabled;
            }

            // Numeric params
            for (int i = 0; i < 6; i++)
            {
                indicator.IndParam.NumParam[i].Value   = ip.NumParam[i].Value;
                indicator.IndParam.NumParam[i].Enabled = ip.NumParam[i].Enabled;
            }

            // Check params
            for (int i = 0; i < 2; i++)
            {
                indicator.IndParam.CheckParam[i].Checked = ip.CheckParam[i].Checked;
                indicator.IndParam.CheckParam[i].Enabled = ip.CheckParam[i].Enabled;
            }

            indicator.Calculate(ip.SlotType);

            // Sets Data.Strategy
            Data.Strategy.Slot[slot].IndicatorName  = indicator.IndicatorName;
            Data.Strategy.Slot[slot].IndParam       = indicator.IndParam;
            Data.Strategy.Slot[slot].Component      = indicator.Component;
            Data.Strategy.Slot[slot].SeparatedChart = indicator.SeparatedChart;
            Data.Strategy.Slot[slot].SpecValue      = indicator.SpecialValues;
            Data.Strategy.Slot[slot].MinValue       = indicator.SeparatedChartMinValue;
            Data.Strategy.Slot[slot].MaxValue       = indicator.SeparatedChartMaxValue;
            Data.Strategy.Slot[slot].IsDefined      = true;

            // Search the indicators' components to determine the Data.FirstBar
            Data.FirstBar = Data.Strategy.SetFirstBar();

            return;
        }

        /// <summary>
        /// Calculates the Limitations Criteria
        /// </summary>
        bool IsLimitationsFulfilled()
        {
            // Limitation Max Ambiguous Bars
            if (chbAmbiguousBars.Checked && Backtester.AmbiguousBars > nudAmbiguousBars.Value)
                return false;

            // Limitation Max Equity Drawdown
            double maxEquityDrawdown = Configs.AccountInMoney ? Backtester.MaxMoneyEquityDrawdown : Backtester.MaxEquityDrawdown;
            if (chbMaxDrawdown.Checked && maxEquityDrawdown > (double)nudMaxDrawdown.Value)
                return false;

            // Limitation Max Equity percent drawdown
            if (chbEquityPercent.Checked && Backtester.MoneyEquityPercentDrawdown > (double)nudEquityPercent.Value)
                return false;

            // Limitation Min Trades
            if (chbMinTrades.Checked && Backtester.ExecutedOrders < nudMinTrades.Value)
                return false;

            // Limitation Max Trades
            if (chbMaxTrades.Checked && Backtester.ExecutedOrders > nudMaxTrades.Value)
                return false;

            // Limitation Win / Loss ratio
            if (chbWinLossRatio.Checked && Backtester.WinLossRatio < (double)nudWinLossRatio.Value)
                return false;

            // OOS Pattern filter
            if (chbOOSPatternFilter.Checked && chbOutOfSample.Checked)
            {
                int netBalance = Backtester.NetBalance;
                int OOSbalance = Backtester.Balance(barOOS);
                float targetBalanceRatio = 1 + (int)nudOutOfSample.Value / 100.0F;
                int targetBalance = (int)(OOSbalance * targetBalanceRatio);
                int minBalance = (int)(targetBalance * (1 - nudOOSPatternPercent.Value / 100));
                if (netBalance < OOSbalance || netBalance < minBalance)
                    return false;
            }

            // Smooth Balance Line
            if (chbSmoothBalanceLines.Checked)
            {
                int checkPoints = (int)nudSmoothBalanceCheckPoints.Value;
                double maxPercentDeviation = (double)(nudSmoothBalancePercent.Value / 100);

                for (int i = 1; i <= checkPoints; i++)
                {
                    int firstBar = Backtester.FirstBar;
                    int bar = Backtester.FirstBar + i * (Data.Bars - firstBar) / (checkPoints + 1);
                    double netBalance = Backtester.NetMoneyBalance;
                    double startBalance = Backtester.MoneyBalance(firstBar);
                    double checkPointBalance = Backtester.MoneyBalance(bar);
                    double targetBalance = startBalance + i * (netBalance - startBalance) / (checkPoints + 1);
                    double minBalance = targetBalance * (1 - maxPercentDeviation);
                    double maxBalance = targetBalance * (1 + maxPercentDeviation);
                    if (checkPointBalance < minBalance || checkPointBalance > maxBalance)
                        return false;

                    if (Configs.AdditionalStatistics)
                    {
                        // Long balance line
                        netBalance = Backtester.NetLongMoneyBalance;
                        checkPointBalance = Backtester.LongMoneyBalance(bar);
                        startBalance = Backtester.LongMoneyBalance(firstBar);
                        targetBalance = startBalance + i * (netBalance - startBalance) / (checkPoints + 1);
                        minBalance = targetBalance * (1 - maxPercentDeviation);
                        maxBalance = targetBalance * (1 + maxPercentDeviation);
                        if (checkPointBalance < minBalance || checkPointBalance > maxBalance)
                            return false;

                        // Short balance line
                        netBalance = Backtester.NetShortMoneyBalance;
                        checkPointBalance = Backtester.ShortMoneyBalance(bar);
                        startBalance = Backtester.ShortMoneyBalance(firstBar);
                        targetBalance = startBalance + i * (netBalance - startBalance) / (checkPoints + 1);
                        minBalance = targetBalance * (1 - maxPercentDeviation);
                        maxBalance = targetBalance * (1 + maxPercentDeviation);
                        if (checkPointBalance < minBalance || checkPointBalance > maxBalance)
                            return false;
                    }
                }

            }

            return true;
        }

        /// <summary>
        /// Prepares the report string.
        /// </summary>
        void InitReport()
        {
            // Prepare report
            sbReport = new StringBuilder();
            sbReport.Append(
                "Net Balance"             + "," +
                "Max Drawdown"            + "," +
                "Gross Profit"            + "," +
                "Gross Loss"              + "," +
                "Executed Orders"         + "," +
                "Traded Lots"             + "," +
                "Time in Position"        + "," +
                "Sent Orders"             + "," +
                "Total Charged Spread"    + "," +
                "Total Charged Rollover"  + "," +
                "Win / Loss Ratio"        + "," +
                "Equity Percent Drawdown" + ",");

            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
                for (int numParam = 0; numParam < 6; numParam++)
                    if (Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled)
                        sbReport.Append(Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Caption + ",");

            sbReport.AppendLine();
        }

        /// <summary>
        ///  Fills a line to the report.
        /// </summary>
        void FillInReport()
        {
            sbReport.Append(
                Backtester.NetMoneyBalance.ToString("F2")            + "," +
                Backtester.MaxMoneyDrawdown.ToString("F2")           + "," +
                Backtester.GrossMoneyProfit.ToString("F2")           + "," +
                Backtester.GrossMoneyLoss.ToString("F2")             + "," +
                Backtester.ExecutedOrders.ToString()                 + "," +
                Backtester.TradedLots.ToString()                     + "," +
                Backtester.TimeInPosition.ToString()                 + "," +
                Backtester.SentOrders.ToString()                     + "," +
                Backtester.TotalChargedMoneySpread.ToString("F2")    + "," +
                Backtester.TotalChargedMoneyRollOver.ToString("F2")  + "," +
                Backtester.WinLossRatio.ToString("F2")               + "," +
                Backtester.MoneyEquityPercentDrawdown.ToString("F2") + ",");

            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
                for (int numParam = 0; numParam < 6; numParam++)
                    if (Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled)
                        sbReport.Append(Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Value.ToString() + ",");

            sbReport.AppendLine();
        }

        /// <summary>
        /// Saves the report in a file.
        /// </summary>
        void SaveReport()
        {
            string pathReport  = Data.StrategyPath.Replace(".xml", ".csv");
            string partilaPath = Data.StrategyPath.Replace(".xml", "");
            int reportIndex = 0;
            do
            {
                reportIndex++;
                pathReport = partilaPath + "-report-" + reportIndex.ToString() + ".csv";

            } while (File.Exists(pathReport));

            try
            {
                using (StreamWriter outfile = new StreamWriter(pathReport))
                {
                    outfile.Write(sbReport.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        delegate void ShowParamBestValueCallback(int param);
        /// <summary>
        /// Shows the best value of a parameter during optimization.
        /// </summary>
        void ShowParamBestValue(int param)
        {
            if (alblParameterValue[param].InvokeRequired)
            {
                BeginInvoke(new ShowParamBestValueCallback(ShowParamBestValue), new object[] { param });
            }
            else
            {
                string newText = Math.Round(aParameter[param].BestValue, aParameter[param].Point).ToString();
                if (alblParameterValue[param].Text == newText)
                    return;

                alblParameterValue[param].Text = newText;
                alblParameterValue[param].Font = fontParamValueBold;

                Timer timer = new Timer();
                timer.Interval = 1500;
                timer.Tag      = param;
                timer.Tick    += new EventHandler(Timer_Tick);
                timer.Start();
            }
        }

        /// <summary>
        /// Recovers the font of a parameter value label.
        /// </summary>
        void Timer_Tick(object sender, EventArgs e)
        {
            Timer timer = (Timer)sender;
            int parameter = (int)timer.Tag;

            alblParameterValue[parameter].Font = fontParamValueRegular;

            timer.Stop();
            timer.Dispose();
        }

        /// <summary>
        /// Out of Sample
        /// </summary>
        void NudOutOfSample_ValueChanged(object sender, EventArgs e)
        {
            isOOS = chbOutOfSample.Checked;
            barOOS = Data.Bars - Data.Bars * (int)nudOutOfSample.Value / 100 - 1;

            smallBalanceChart.OOSBar = barOOS;

            if (isOOS)
            {
                smallBalanceChart.SetChartData();
                smallBalanceChart.InitChart();
                smallBalanceChart.Invalidate();
            }
        }

        /// <summary>
        /// Out of Sample
        /// </summary>
        void ChbOutOfSample_CheckedChanged(object sender, EventArgs e)
        {
            isOOS = chbOutOfSample.Checked;
            barOOS = Data.Bars - Data.Bars * (int)nudOutOfSample.Value / 100 - 1;

            smallBalanceChart.OOS    = isOOS;
            smallBalanceChart.OOSBar = barOOS;

            smallBalanceChart.SetChartData();
            smallBalanceChart.InitChart();
            smallBalanceChart.Invalidate();
        }

        /// <summary>
        /// Changes chart's settings after a button click.
        /// </summary>
        void Buttons_Click(object sender, EventArgs e)
        {

            ToolStripButton  btn    = (ToolStripButton)sender;
            OptimizerButtons button = (OptimizerButtons)btn.Tag;

            switch (button)
            {
                case OptimizerButtons.ShowParams:
                    pnlParamsBase.Visible  = true;
                    pnlLimitations.Visible = false;
                    pnlSettings.Visible    = false;
                    if (isOptimizing == false)
                        for (int i = 0; i <= (int)OptimizerButtons.SetStep15; i++)
                            aOptimizerButtons[i].Enabled = true;
                    aOptimizerButtons[(int)OptimizerButtons.ShowParams].Enabled = false;
                    aOptimizerButtons[(int)OptimizerButtons.ShowLimitations].Enabled = true;
                    aOptimizerButtons[(int)OptimizerButtons.ShowSettings].Enabled = true;
                    break;
                case OptimizerButtons.ShowLimitations:
                    pnlParamsBase.Visible  = false;
                    pnlLimitations.Visible = true;
                    pnlSettings.Visible    = false;
                    for(int i = 0; i <= (int)OptimizerButtons.SetStep15; i++)
                        aOptimizerButtons[i].Enabled = false;
                    aOptimizerButtons[(int)OptimizerButtons.ShowParams].Enabled = true;
                    aOptimizerButtons[(int)OptimizerButtons.ShowLimitations].Enabled = false;
                    aOptimizerButtons[(int)OptimizerButtons.ShowSettings].Enabled = true;
                   break;
                case OptimizerButtons.ShowSettings:
                    pnlParamsBase.Visible  = false;
                    pnlLimitations.Visible = false;
                    pnlSettings.Visible    = true;
                    for(int i = 0; i <= (int)OptimizerButtons.SetStep15; i++)
                        aOptimizerButtons[i].Enabled = false;
                    aOptimizerButtons[(int)OptimizerButtons.ShowParams].Enabled = true;
                    aOptimizerButtons[(int)OptimizerButtons.ShowLimitations].Enabled = true;
                    aOptimizerButtons[(int)OptimizerButtons.ShowSettings].Enabled = false;
                    break;
                default:
                    break;
            }

            if (isOptimizing)
                return;

            switch (button)
            {
                case OptimizerButtons.SelectAll:
                    SelectParameters(OptimizerButtons.SelectAll);
                    break;
                case OptimizerButtons.SelectNone:
                    SelectParameters(OptimizerButtons.SelectNone);
                    break;
                case OptimizerButtons.SelectRandom:
                    SelectParameters(OptimizerButtons.SelectRandom);
                    break;
                case OptimizerButtons.SetStep5:
                    SetParamsMinMax(5);
                    break;
                case OptimizerButtons.SetStep10:
                    SetParamsMinMax(10);
                    break;
                case OptimizerButtons.SetStep15:
                    SetParamsMinMax(15);
                    break;
                default:
                    break;
            }

            return;
        }

        /// <summary>
        /// Toggles FSB visibility.
        /// </summary>
        void HideFSB_Click(object sender, EventArgs e)
        {
            formFSB.Visible = !chbHideFSB.Checked;
        }

        /// <summary>
        /// Resets Generator
        /// </summary>
        void BtnReset_Click(object sender, EventArgs e)
        {
            Configs.OptimizerOptions = "";
            isReset = true;
            Close();
        }
   }
}
