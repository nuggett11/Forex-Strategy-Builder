// Strategy Generator - GUI
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Generator
{
    /// <summary>
    /// Strategy Generator
    /// </summary>
    public partial class Generator : Form
    {
        ToolStrip           tsStrategy;
        ToolStrip           tsGenerator;
        Strategy_Layout     strategyLayout;
        Fancy_Panel         pnlCommon;
        Fancy_Panel         pnlLimitations;
        Fancy_Panel         pnlSettings;
        Fancy_Panel         pnlTop10;
        Fancy_Panel         pnlIndicators;
        Small_Balance_Chart smallBalanceChart;
        Info_Panel          infpnlAccountStatistics;
        ProgressBar         progressBar;
        Button              btnGenerate;
        Button              btnAccept;
        Button              btnCancel;
        ToolTip             toolTip = new ToolTip();
        Random              random  = new Random();
        BackgroundWorker    bgWorker;

        ToolStripButton tsbtLockAll;
        ToolStripButton tsbtUnlockAll;
        ToolStripButton tsbtLinkAll;
        ToolStripButton tsbtOverview;
        ToolStripButton tsbtStrategyInfo;
        ToolStripButton tsbtStrategySize1;
        ToolStripButton tsbtStrategySize2;

        ToolStripButton tsbtShowOptions;
        ToolStripButton tsbtShowLimitations;
        ToolStripButton tsbtShowSettings;
        ToolStripButton tsbtShowTop10;
        ToolStripButton tsbtShowIndicators;

        Form   formFSB;
        public Form SetParrentForm { set { formFSB = value; } }

        Font  font;
        Color colorText;

        CheckBox      chbGenerateNewStrategy;
        CheckBox      chbPreservPermSL;
        CheckBox      chbPreservPermTP;
        CheckBox      chbInitialOptimisation;
        CheckBox      chbMaxOpeningLogicSlots;
        NumericUpDown nudMaxOpeningLogicSlots;
        CheckBox      chbMaxClosingLogicSlots;
        NumericUpDown nudMaxClosingLogicSlots;
        CheckBox      chbOutOfSample;
        NumericUpDown nudOutOfSample;
        Label         lblCalcStrInfo;        
        Label         lblCalcStrNumb;
        NumericUpDown nudWorkingMinutes;
        Label         lblWorkingMinutes;

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

        CheckBox      chbUseDefaultIndicatorValues;
        CheckBox      chbHideFSB;
        Button        btnReset;

        double buttonWidthMultiplier = 1; // It's used in OnResize().

        Top10Layout top10Layout;
        IndicatorsLayout indicatorsLayout;

        bool isReset = false;

        /// <summary>
        /// Gets the strategy description
        /// </summary>
        public string GeneratedDescription { get { return generatedDescription; } }

        /// <summary>
        /// Whether the strategy was modified or entirely generated
        /// </summary>
        public bool IsStrategyModified { get { return isStrategyModified; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public Generator()
        {
            strategyBest       = Data.Strategy.Clone();
            bestBalance        = isOOS ? Backtester.Balance(barOOS) : Backtester.NetBalance;
            isGenerating       = false;
            isStartegyChanged  = false;
            indicatorBlackList = new List<string>();

            font      = this.Font;
            colorText = LayoutColors.ColorControlText;

            tsStrategy         = new ToolStrip();
            tsGenerator        = new ToolStrip();
            strategyLayout     = new Strategy_Layout(strategyBest);
            pnlCommon          = new Fancy_Panel(Language.T("Common"));
            pnlLimitations     = new Fancy_Panel(Language.T("Limitations"));
            pnlSettings        = new Fancy_Panel(Language.T("Settings"));
            pnlTop10           = new Fancy_Panel(Language.T("Top 10"));
            pnlIndicators      = new Fancy_Panel(Language.T("Indicators"));
            smallBalanceChart  = new Small_Balance_Chart();
            infpnlAccountStatistics = new Info_Panel();
            progressBar        = new ProgressBar();
            lblCalcStrInfo     = new Label();
            lblCalcStrNumb     = new Label();
            btnAccept          = new Button();
            btnGenerate        = new Button();
            btnCancel          = new Button();
            chbGenerateNewStrategy = new CheckBox();
            chbPreservPermSL   = new CheckBox();
            chbPreservPermTP   = new CheckBox();
            chbInitialOptimisation = new CheckBox();
            nudWorkingMinutes  = new NumericUpDown();
            lblWorkingMinutes  = new Label();

            MaximizeBox     = false;
            Icon            = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            BackColor       = LayoutColors.ColorFormBack;
            AcceptButton    = btnGenerate;
            Text            = Language.T("Strategy Generator") + " - " + Data.Symbol + " " + Data.PeriodString + ", " + Data.Bars.ToString() + " " + Language.T("bars");
            FormClosing    += new FormClosingEventHandler(Generator_FormClosing);

            // Tool Strip Strategy
            tsStrategy.Parent   = this;
            tsStrategy.Dock     = DockStyle.None;
            tsStrategy.AutoSize = false;

            // Tool Strip Generator
            tsGenerator.Parent   = this;
            tsGenerator.Dock     = DockStyle.None;
            tsGenerator.AutoSize = false;

            // Creates a Strategy Layout
            strategyLayout.Parent                = this;
            strategyLayout.ShowAddSlotButtons    = false;
            strategyLayout.ShowRemoveSlotButtons = false;
            strategyLayout.ShowPadlockImg        = true;
            strategyLayout.SlotPropertiesTipText = Language.T("Lock or unlock the slot.");
            strategyLayout.SlotToolTipText       = Language.T("Lock, link, or unlock the slot.");

            pnlCommon.Parent       = this;
            pnlLimitations.Parent  = this;
            pnlSettings.Parent     = this;
            pnlTop10.Parent        = this;
            pnlIndicators.Parent   = this;

            // smallBalanceChart
            smallBalanceChart.Parent    = this;
            smallBalanceChart.BackColor = LayoutColors.ColorControlBack;
            smallBalanceChart.Visible   = true;
            smallBalanceChart.Cursor    = Cursors.Hand;
            smallBalanceChart.Click    += new EventHandler(AccountAutput_Click);
            smallBalanceChart.DoubleClick += new EventHandler(AccountAutput_Click);
            toolTip.SetToolTip(smallBalanceChart, Language.T("Show account statistics."));
            smallBalanceChart.SetChartData();

            // Info Panel Account Statistics
            infpnlAccountStatistics.Parent  = this;
            infpnlAccountStatistics.Visible = false;
            infpnlAccountStatistics.Cursor  = Cursors.Hand;
            infpnlAccountStatistics.Click  += new EventHandler(AccountAutput_Click);
            infpnlAccountStatistics.DoubleClick += new EventHandler(AccountAutput_Click);
            toolTip.SetToolTip(infpnlAccountStatistics, Language.T("Show account chart."));

            // ProgressBar
            progressBar.Parent  = this;
            progressBar.Minimum = 1;
            progressBar.Maximum = 100;
            progressBar.Step    = 1;

            //Button Generate
            btnGenerate.Parent = this;
            btnGenerate.Name   = "Generate";
            btnGenerate.Text   = Language.T("Generate");
            btnGenerate.Click += new EventHandler(BtnGenerate_Click);
            btnGenerate.UseVisualStyleBackColor = true;

            //Button Accept
            btnAccept.Parent  = this;
            btnAccept.Name    = "Accept";
            btnAccept.Text    = Language.T("Accept");
            btnAccept.Enabled = false;
            btnAccept.DialogResult = DialogResult.OK;
            btnAccept.UseVisualStyleBackColor = true;

            //Button Cancel
            btnCancel.Parent = this;
            btnCancel.Text   = Language.T("Cancel");
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.UseVisualStyleBackColor = true;

            // BackgroundWorker
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress      = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork             += new DoWorkEventHandler(BgWorker_DoWork);
            bgWorker.ProgressChanged    += new ProgressChangedEventHandler(BgWorker_ProgressChanged);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BgWorker_RunWorkerCompleted);

            SetButtonsStrategy();
            SetButtonsGenerator();
            SetPanelCommon();
            SetPanelLimitations();
            SetPanelSettings();
            SetPanelTop10();
            SetPanelIndicators();
            LoadOptions();
            SetSrategyDescriptionButton();

            chbHideFSB.CheckedChanged += new EventHandler(HideFSB_Click);

            return;
        }

        /// <summary>
        /// Loads and parses the generator's options.
        /// </summary>
        void LoadOptions()
        {
            if (string.IsNullOrEmpty(Configs.GeneratorOptions))
                return;

            string[] options = Configs.GeneratorOptions.Split(';');
            int i = 0;
            try {
                chbGenerateNewStrategy.Checked  = bool.Parse(options[i++]);
                chbPreservPermSL.Checked        = bool.Parse(options[i++]);
                chbPreservPermTP.Checked        = bool.Parse(options[i++]);
                chbInitialOptimisation.Checked  = bool.Parse(options[i++]);
                chbMaxOpeningLogicSlots.Checked = bool.Parse(options[i++]);
                nudMaxOpeningLogicSlots.Value   = Math.Min(int.Parse(options[i++]), Strategy.MaxOpenFilters);
                chbMaxClosingLogicSlots.Checked = bool.Parse(options[i++]);
                nudMaxClosingLogicSlots.Value   = Math.Min(int.Parse(options[i++]), Strategy.MaxCloseFilters);
                chbOutOfSample.Checked          = bool.Parse(options[i++]);
                nudOutOfSample.Value            = int.Parse(options[i++]);
                nudWorkingMinutes.Value         = int.Parse(options[i++]);
                chbAmbiguousBars.Checked        = bool.Parse(options[i++]);
                nudAmbiguousBars.Value          = int.Parse(options[i++]);
                chbMaxDrawdown.Checked          = bool.Parse(options[i++]);
                nudMaxDrawdown.Value            = int.Parse(options[i++]);
                chbMinTrades.Checked            = bool.Parse(options[i++]);
                nudMinTrades.Value              = int.Parse(options[i++]);
                chbMaxTrades.Checked            = bool.Parse(options[i++]);
                nudMaxTrades.Value              = int.Parse(options[i++]);
                chbWinLossRatio.Checked         = bool.Parse(options[i++]);
                nudWinLossRatio.Value           = int.Parse(options[i++]) / 100M;
                chbEquityPercent.Checked        = bool.Parse(options[i++]);
                nudEquityPercent.Value          = int.Parse(options[i++]);
                chbOOSPatternFilter.Checked     = bool.Parse(options[i++]);
                nudOOSPatternPercent.Value      = int.Parse(options[i++]);
                chbSmoothBalanceLines.Checked   = bool.Parse(options[i++]);
                nudSmoothBalancePercent.Value   = int.Parse(options[i++]);
                nudSmoothBalanceCheckPoints.Value = int.Parse(options[i++]);
                chbUseDefaultIndicatorValues.Checked = bool.Parse(options[i++]);
                chbHideFSB.Checked              = bool.Parse(options[i++]);
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
            chbGenerateNewStrategy.Checked.ToString()        + ";" +
            chbPreservPermSL.Checked.ToString()              + ";" +
            chbPreservPermTP.Checked.ToString()              + ";" +
            chbInitialOptimisation.Checked.ToString()        + ";" +
            chbMaxOpeningLogicSlots.Checked.ToString()       + ";" +
            nudMaxOpeningLogicSlots.Value.ToString()         + ";" +
            chbMaxClosingLogicSlots.Checked.ToString()       + ";" +
            nudMaxClosingLogicSlots.Value.ToString()         + ";" +
            chbOutOfSample.Checked.ToString()                + ";" +
            nudOutOfSample.Value.ToString()                  + ";" +
            nudWorkingMinutes.Value.ToString()               + ";" +
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
            chbUseDefaultIndicatorValues.Checked.ToString()  + ";" +
            chbHideFSB.Checked.ToString();

            Configs.GeneratorOptions = options;

            return;
        }

        /// <summary>
        /// Sets controls in panel Common
        /// </summary>
        void SetPanelCommon()
        {
            // chbGenerateNewStrategy
            chbGenerateNewStrategy.Parent    = pnlCommon;
            chbGenerateNewStrategy.Text      = Language.T("Generate a new strategy at every start");
            chbGenerateNewStrategy.AutoSize  = true;
            chbGenerateNewStrategy.Checked   = true;
            chbGenerateNewStrategy.ForeColor = LayoutColors.ColorControlText;
            chbGenerateNewStrategy.BackColor = Color.Transparent;

            // chbPreservPermSL
            chbPreservPermSL.Parent    = pnlCommon;
            chbPreservPermSL.Text      = Language.T("Do not change the Permanent Stop Loss");
            chbPreservPermSL.AutoSize  = true;
            chbPreservPermSL.Checked   = true;
            chbPreservPermSL.ForeColor = LayoutColors.ColorControlText;
            chbPreservPermSL.BackColor = Color.Transparent;

            // chbPreservPermTP
            chbPreservPermTP.Parent    = pnlCommon;
            chbPreservPermTP.Text      = Language.T("Do not change the Permanent Take Profit");
            chbPreservPermTP.AutoSize  = true;
            chbPreservPermTP.Checked   = true;
            chbPreservPermTP.ForeColor = LayoutColors.ColorControlText;
            chbPreservPermTP.BackColor = Color.Transparent;

            // chbPseudoOpt
            chbInitialOptimisation.Parent    = pnlCommon;
            chbInitialOptimisation.Text      = Language.T("Perform an initial optimization");
            chbInitialOptimisation.AutoSize  = true;
            chbInitialOptimisation.Checked   = true;
            chbInitialOptimisation.ForeColor = LayoutColors.ColorControlText;
            chbInitialOptimisation.BackColor = Color.Transparent;

            chbMaxOpeningLogicSlots = new CheckBox();
            chbMaxOpeningLogicSlots.Parent    = pnlCommon;
            chbMaxOpeningLogicSlots.ForeColor = colorText;
            chbMaxOpeningLogicSlots.BackColor = Color.Transparent;
            chbMaxOpeningLogicSlots.Text      = Language.T("Maximum number of opening logic slots");
            chbMaxOpeningLogicSlots.Checked   = true;
            chbMaxOpeningLogicSlots.AutoSize  = true;

            nudMaxOpeningLogicSlots = new NumericUpDown();
            nudMaxOpeningLogicSlots.Parent    = pnlCommon;
            nudMaxOpeningLogicSlots.TextAlign = HorizontalAlignment.Center;
            nudMaxOpeningLogicSlots.BeginInit();
            nudMaxOpeningLogicSlots.Minimum   = 0;
            nudMaxOpeningLogicSlots.Maximum   = Strategy.MaxOpenFilters;
            nudMaxOpeningLogicSlots.Increment = 1;
            nudMaxOpeningLogicSlots.Value     = 2;
            nudMaxOpeningLogicSlots.EndInit();

            chbMaxClosingLogicSlots = new CheckBox();
            chbMaxClosingLogicSlots.Parent    = pnlCommon;
            chbMaxClosingLogicSlots.ForeColor = colorText;
            chbMaxClosingLogicSlots.BackColor = Color.Transparent;
            chbMaxClosingLogicSlots.Text      = Language.T("Maximum number of closing logic slots");
            chbMaxClosingLogicSlots.Checked   = true;
            chbMaxClosingLogicSlots.AutoSize  = true;

            nudMaxClosingLogicSlots = new NumericUpDown();
            nudMaxClosingLogicSlots.Parent    = pnlCommon;
            nudMaxClosingLogicSlots.TextAlign = HorizontalAlignment.Center;
            nudMaxClosingLogicSlots.BeginInit();
            nudMaxClosingLogicSlots.Minimum   = 0;
            nudMaxClosingLogicSlots.Maximum   = Strategy.MaxCloseFilters;
            nudMaxClosingLogicSlots.Increment = 1;
            nudMaxClosingLogicSlots.Value     = 1;
            nudMaxClosingLogicSlots.EndInit();

            //lblNumUpDown
            lblWorkingMinutes.Parent    = pnlCommon;
            lblWorkingMinutes.ForeColor = LayoutColors.ColorControlText;
            lblWorkingMinutes.BackColor = Color.Transparent;
            lblWorkingMinutes.Text      = Language.T("Working time");
            lblWorkingMinutes.AutoSize  = true;
            lblWorkingMinutes.TextAlign = ContentAlignment.MiddleRight;

            // numUpDownWorkingTime
            nudWorkingMinutes.Parent    = pnlCommon;
            nudWorkingMinutes.Value     = 5;
            nudWorkingMinutes.Minimum   = 0;
            nudWorkingMinutes.Maximum   = 10000;
            nudWorkingMinutes.TextAlign = HorizontalAlignment.Center;
            toolTip.SetToolTip(nudWorkingMinutes, Language.T("Set the number of minutes for the Generator to work.") +
                Environment.NewLine + "0 - " + Language.T("No limits").ToLower() + ".");

            // Label Calculated Strategies Caption
            lblCalcStrInfo.Parent    = pnlCommon;
            lblCalcStrInfo.AutoSize  = true;
            lblCalcStrInfo.ForeColor = LayoutColors.ColorControlText;
            lblCalcStrInfo.BackColor = Color.Transparent;
            lblCalcStrInfo.Text      = Language.T("Calculations");

            // Label Calculated Strategies Number
            lblCalcStrNumb.Parent      = pnlCommon;
            lblCalcStrNumb.BorderStyle = BorderStyle.FixedSingle;
            lblCalcStrNumb.ForeColor   = LayoutColors.ColorControlText;
            lblCalcStrNumb.BackColor   = LayoutColors.ColorControlBack;
            lblCalcStrNumb.TextAlign   = ContentAlignment.MiddleCenter;
            lblCalcStrNumb.Text        = "0";
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
            chbAmbiguousBars.Checked   = true;
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
            chbMaxDrawdown.Text      = Language.T("Maximum equity drawdown") + " [" + (Configs.AccountInMoney ? Configs.AccountCurrency + "]" : Language.T("pips") + "]");
            chbMaxDrawdown.Checked   = false;
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
            chbEquityPercent.Checked   = true;
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

            chbUseDefaultIndicatorValues = new CheckBox();
            chbUseDefaultIndicatorValues.Parent    = pnlSettings;
            chbUseDefaultIndicatorValues.ForeColor = colorText;
            chbUseDefaultIndicatorValues.BackColor = Color.Transparent;
            chbUseDefaultIndicatorValues.Text      = Language.T("Only use default numeric indicator values");
            chbUseDefaultIndicatorValues.Checked   = false;
            chbUseDefaultIndicatorValues.AutoSize  = true;

            chbHideFSB = new CheckBox();
            chbHideFSB.Parent    = pnlSettings;
            chbHideFSB.ForeColor = colorText;
            chbHideFSB.BackColor = Color.Transparent;
            chbHideFSB.Text      = Language.T("Hide FSB when Generator starts");
            chbHideFSB.Checked   = true;
            chbHideFSB.AutoSize  = true;
            chbHideFSB.Cursor    = Cursors.Default;

            btnReset = new Button();
            btnReset.Parent = pnlSettings;
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Text = Language.T("Reset all parameters and settings");
            btnReset.Click += new EventHandler(BtnReset_Click);
        }

        /// <summary>
        /// Sets controls in panel Top 10
        /// </summary>
        void SetPanelTop10()
        {
            top10Layout = new Top10Layout(10);
            top10Layout.Parent = pnlTop10;
        }

        /// <summary>
        /// Sets controls in panel Indicators
        /// </summary>
        void SetPanelIndicators()
        {
            indicatorsLayout = new IndicatorsLayout();
            indicatorsLayout.Parent = pnlIndicators;
        }

        /// <summary>
        /// Sets tool strip buttons
        /// </summary>
        void SetButtonsStrategy()
        {
            tsbtLockAll = new ToolStripButton();
            tsbtLockAll.Name = "tsbtLockAll";
            tsbtLockAll.Image = Properties.Resources.padlock_img;
            tsbtLockAll.Click += new EventHandler(ChangeSlotStatus);
            tsbtLockAll.ToolTipText = Language.T("Lock all slots.");
            tsStrategy.Items.Add(tsbtLockAll);

            tsbtUnlockAll = new ToolStripButton();
            tsbtUnlockAll.Name = "tsbtUnlockAll";
            tsbtUnlockAll.Image = Properties.Resources.open_padlock_img;
            tsbtUnlockAll.Click += new EventHandler(ChangeSlotStatus);
            tsbtUnlockAll.ToolTipText = Language.T("Unlock all slots.");
            tsStrategy.Items.Add(tsbtUnlockAll);

            tsbtLinkAll = new ToolStripButton();
            tsbtLinkAll.Name = "tsbtLinkAll";
            tsbtLinkAll.Image = Properties.Resources.linked;
            tsbtLinkAll.Click += new EventHandler(ChangeSlotStatus);
            tsbtLinkAll.ToolTipText = Language.T("Link all slots.");
            tsStrategy.Items.Add(tsbtLinkAll);

            tsStrategy.Items.Add(new ToolStripSeparator());

            // Button Overview
            tsbtOverview = new ToolStripButton();
            tsbtOverview.Name = "Overview";
            tsbtOverview.Text = Language.T("Overview");
            tsbtOverview.Click += new EventHandler(Show_Overview);
            tsbtOverview.ToolTipText = Language.T("See the strategy overview.");
            tsStrategy.Items.Add(tsbtOverview);

            // Button tsbtStrategySize1
            tsbtStrategySize1 = new ToolStripButton();
            tsbtStrategySize1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtStrategySize1.Image = Properties.Resources.slot_size_max;
            tsbtStrategySize1.Tag = 1;
            tsbtStrategySize1.Click += new EventHandler(BtnSlotSize_Click);
            tsbtStrategySize1.ToolTipText = Language.T("Show detailed info in the slots.");
            tsbtStrategySize1.Alignment = ToolStripItemAlignment.Right;
            tsStrategy.Items.Add(tsbtStrategySize1);

            // Button tsbtStrategySize2
            tsbtStrategySize2 = new ToolStripButton();
            tsbtStrategySize2.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtStrategySize2.Image = Properties.Resources.slot_size_min;
            tsbtStrategySize2.Tag = 2;
            tsbtStrategySize2.Click += new EventHandler(BtnSlotSize_Click);
            tsbtStrategySize2.ToolTipText = Language.T("Show minimum info in the slots.");
            tsbtStrategySize2.Alignment = ToolStripItemAlignment.Right;
            tsStrategy.Items.Add(tsbtStrategySize2);

            // Button tsbtStrategyInfo
            tsbtStrategyInfo = new ToolStripButton();
            tsbtStrategyInfo.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtStrategyInfo.Image = Properties.Resources.str_info_infook;
            tsbtStrategyInfo.Click += new EventHandler(BtnStrategyDescription_Click);
            tsbtStrategyInfo.ToolTipText = Language.T("Show the strategy description.");
            tsbtStrategyInfo.Alignment = ToolStripItemAlignment.Right;
            tsStrategy.Items.Add(tsbtStrategyInfo);
        }

        /// <summary>
        /// Sets tool strip buttons
        /// </summary>
        void SetButtonsGenerator()
        {
            // Button Options
            tsbtShowOptions = new ToolStripButton();
            tsbtShowOptions.Name = "tsbtShowOptions";
            tsbtShowOptions.Text = Language.T("Common");
            tsbtShowOptions.Click += new EventHandler(ChangeGeneratorPanel);
            tsbtShowOptions.Enabled = false;
            tsGenerator.Items.Add(tsbtShowOptions);

            // Button Limitations
            tsbtShowLimitations = new ToolStripButton();
            tsbtShowLimitations.Name = "tsbtShowLimitations";
            tsbtShowLimitations.Text = Language.T("Limitations");
            tsbtShowLimitations.Click += new EventHandler(ChangeGeneratorPanel);
            tsGenerator.Items.Add(tsbtShowLimitations);

            // Button Settings
            tsbtShowSettings = new ToolStripButton();
            tsbtShowSettings.Name = "tsbtShowSettings";
            tsbtShowSettings.Text = Language.T("Settings");
            tsbtShowSettings.Click += new EventHandler(ChangeGeneratorPanel);
            tsGenerator.Items.Add(tsbtShowSettings);

            // Button Top10
            tsbtShowTop10 = new ToolStripButton();
            tsbtShowTop10.Name = "tsbtShowTop10";
            tsbtShowTop10.Text = Language.T("Top 10");
            tsbtShowTop10.Click += new EventHandler(ChangeGeneratorPanel);
            tsGenerator.Items.Add(tsbtShowTop10);

            // Button Indicators
            tsbtShowIndicators = new ToolStripButton();
            tsbtShowIndicators.Name = "tsbtIndicators";
            tsbtShowIndicators.Text = Language.T("Indicators");
            tsbtShowIndicators.Click += new EventHandler(ChangeGeneratorPanel);
            tsGenerator.Items.Add(tsbtShowIndicators);
        }

        /// <summary>
        /// Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            formFSB.Visible = !chbHideFSB.Checked;

            // Find correct size
            int maxCheckBoxWidth = 250;
            foreach (Control control in pnlLimitations.Controls)
            {
                if (maxCheckBoxWidth < control.Width)
                    maxCheckBoxWidth = control.Width;
            }
            foreach (Control control in pnlCommon.Controls)
            {
                if (maxCheckBoxWidth < control.Width)
                    maxCheckBoxWidth = control.Width;
            }

            int buttonWidth = (int)(Data.HorizontalDLU * 60);
            int btnHrzSpace = (int)(Data.HorizontalDLU * 3);
            int nudWidth    = 55;
            pnlLimitations.Width = 3 * buttonWidth + 2 * btnHrzSpace;
            int iBorderWidth = (pnlLimitations.Width - pnlLimitations.ClientSize.Width) / 2;

            if (maxCheckBoxWidth + 3 * btnHrzSpace + nudWidth + 4 > pnlLimitations.ClientSize.Width)
                buttonWidthMultiplier = ((maxCheckBoxWidth + nudWidth + 3 * btnHrzSpace + 2 * iBorderWidth + 4) / 3.0) / buttonWidth;

            ClientSize = new Size(2 * ((int)(3 * buttonWidth * buttonWidthMultiplier) + 2 * btnHrzSpace) + 3 * btnHrzSpace, 528);

            OnResize(e);

            RebuildStrategyLayout(strategyBest);
            RefreshAccountStatisticas();
            Top10AddStrategy();

            return;
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int buttonHeight   = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth    = (int)(Data.HorizontalDLU * 60 * buttonWidthMultiplier);
            int btnVertSpace   = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace    = (int)(Data.HorizontalDLU * 3);
            int border         = btnHrzSpace;
            int textHeight     = Font.Height;
            int rightSideWidth = 3 * buttonWidth + 2 * btnHrzSpace;
            int rightSideLocation = ClientSize.Width - rightSideWidth - btnHrzSpace;
            int leftSideWidth  = ClientSize.Width - 3 * buttonWidth - 5 * btnHrzSpace;
            int nudWidth       = 55;
            int optionsHeight  = 228;

            //Button Cancel
            btnCancel.Size     = new Size(buttonWidth, buttonHeight);
            btnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            //Button Accept
            btnAccept.Size     = new Size(buttonWidth, buttonHeight);
            btnAccept.Location = new Point(btnCancel.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            //Button Generate
            btnGenerate.Size     = new Size(buttonWidth, buttonHeight);
            btnGenerate.Location = new Point(btnAccept.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Progress Bar
            progressBar.Size     = new Size(ClientSize.Width - leftSideWidth - 3 * border, (int)(Data.VerticalDLU * 9));
            progressBar.Location = new Point(leftSideWidth + 2 * border, btnAccept.Top - progressBar.Height - btnVertSpace);

            // Tool Strip Strategy
            tsStrategy.Width = leftSideWidth + border;
            tsStrategy.Location = Point.Empty;

            // Tool Strip Strategy
            tsGenerator.Width = ClientSize.Width - leftSideWidth - border;
            tsGenerator.Location = new Point(tsStrategy.Right + border, 0);

            // Panel Common
            pnlCommon.Size = new Size(rightSideWidth, optionsHeight);
            pnlCommon.Location = new Point(rightSideLocation, tsStrategy.Bottom + border);

            // Panel pnlLimitations
            pnlLimitations.Size = new Size(rightSideWidth, optionsHeight);
            pnlLimitations.Location = new Point(rightSideLocation, tsStrategy.Bottom + border);

            // Panel Settings
            pnlSettings.Size = new Size(rightSideWidth, optionsHeight);
            pnlSettings.Location = new Point(rightSideLocation, tsStrategy.Bottom + border);

            // Panel Top 10
            pnlTop10.Size = new Size(rightSideWidth, optionsHeight);
            pnlTop10.Location = new Point(rightSideLocation, tsStrategy.Bottom + border);

            // Panel Indicators
            pnlIndicators.Size = new Size(rightSideWidth, optionsHeight);
            pnlIndicators.Location = new Point(rightSideLocation, tsStrategy.Bottom + border);

            // Panel StrategyLayout
            strategyLayout.Size = new Size(leftSideWidth, ClientSize.Height - border - tsStrategy.Bottom - border);
            strategyLayout.Location = new Point(border, tsStrategy.Bottom + border);

            // Panel Balance Chart
            smallBalanceChart.Size     = new Size(ClientSize.Width - leftSideWidth - 3 * border, progressBar.Top - 3 * border - pnlCommon.Bottom);
            smallBalanceChart.Location = new Point(strategyLayout.Right + border, pnlCommon.Bottom + border);

            // Account Statistics
            infpnlAccountStatistics.Size = new Size(ClientSize.Width - leftSideWidth - 3 * border, progressBar.Top - 3 * border - pnlCommon.Bottom);
            infpnlAccountStatistics.Location = new Point(strategyLayout.Right + border, pnlCommon.Bottom + border);

            //chbGenerateNewStrategy
            chbGenerateNewStrategy.Location = new Point(border + 2, 26);

            //chbPreservPermSL
            chbPreservPermSL.Location = new Point(border + 2, chbGenerateNewStrategy.Bottom + border + 4);

            //chbPreservPermTP
            chbPreservPermTP.Location = new Point(border + 2, chbPreservPermSL.Bottom + border + 4);

            // chbPseudoOpt
            chbInitialOptimisation.Location = new Point(border + 2, chbPreservPermTP.Bottom + border + 4);

            // chbMaxOpeningLogicSlots
            chbMaxOpeningLogicSlots.Location = new Point(border + 2, chbInitialOptimisation.Bottom + border + 4);

            // nudMaxOpeningLogicSlots
            nudMaxOpeningLogicSlots.Width    = nudWidth;
            nudMaxOpeningLogicSlots.Location = new Point(nudAmbiguousBars.Left, chbMaxOpeningLogicSlots.Top - 1);

            // chbMaxClosingLogicSlots
            chbMaxClosingLogicSlots.Location = new Point(border + 2, chbMaxOpeningLogicSlots.Bottom + border + 4);

            // nudMaxClosingLogicSlots
            nudMaxClosingLogicSlots.Width    = nudWidth;
            nudMaxClosingLogicSlots.Location = new Point(nudAmbiguousBars.Left, chbMaxClosingLogicSlots.Top - 1);

            // Labels Strategy Calculations
            lblCalcStrInfo.Location = new Point(border - 1, pnlCommon.Height - nudMaxOpeningLogicSlots.Height - border);
            lblCalcStrNumb.Size     = new Size(nudWidth, nudMaxOpeningLogicSlots.Height - 1);
            lblCalcStrNumb.Location = new Point(lblCalcStrInfo.Right + border, lblCalcStrInfo.Top - 3);

            //Working Minutes
            nudWorkingMinutes.Width    = nudWidth;
            nudWorkingMinutes.Location = new Point(nudMaxOpeningLogicSlots.Right - nudWidth, lblCalcStrInfo.Top - 2);
            lblWorkingMinutes.Location = new Point(nudWorkingMinutes.Left - lblWorkingMinutes.Width - 3, lblCalcStrInfo.Top);

            // chbAmbiguousBars
            chbAmbiguousBars.Location = new Point(border + 2, 25);

            // nudAmbiguousBars
            nudAmbiguousBars.Width    = nudWidth;
            nudAmbiguousBars.Location = new Point(pnlLimitations.ClientSize.Width - nudWidth - border - 2, chbAmbiguousBars.Top - 1);

            // MaxDrawdown
            chbMaxDrawdown.Location = new Point(border + 2, chbAmbiguousBars.Bottom + border + 4);
            nudMaxDrawdown.Width    = nudWidth;
            nudMaxDrawdown.Location = new Point(nudAmbiguousBars.Left , chbMaxDrawdown.Top - 1);

            // MaxDrawdown %
            chbEquityPercent.Location = new Point(border + 2, nudMaxDrawdown.Bottom + border + 4);
            nudEquityPercent.Width    = nudWidth;
            nudEquityPercent.Location = new Point(nudAmbiguousBars.Left, chbEquityPercent.Top - 1);

            // MinTrades
            chbMinTrades.Location = new Point(border + 2, chbEquityPercent.Bottom + border + 4);
            nudMinTrades.Width    = nudWidth;
            nudMinTrades.Location = new Point(nudAmbiguousBars.Left, chbMinTrades.Top - 1);

            // MaxTrades
            chbMaxTrades.Location = new Point(border + 2, chbMinTrades.Bottom + border + 4);
            nudMaxTrades.Width    = nudWidth;
            nudMaxTrades.Location = new Point(nudAmbiguousBars.Left, chbMaxTrades.Top - 1);

            // WinLossRatios
            chbWinLossRatio.Location = new Point(border + 2, chbMaxTrades.Bottom + border + 4);
            nudWinLossRatio.Width    = nudWidth;
            nudWinLossRatio.Location = new Point(nudAmbiguousBars.Left, chbWinLossRatio.Top - 1);

            // OOS Pattern Filter
            chbOOSPatternFilter.Location = new Point(border + 2, chbWinLossRatio.Bottom + border + 4);
            nudOOSPatternPercent.Width = nudWidth;
            nudOOSPatternPercent.Location = new Point(nudAmbiguousBars.Left, chbOOSPatternFilter.Top - 1);

            // Balance lines pattern
            chbSmoothBalanceLines.Location = new Point(border + 2, chbOOSPatternFilter.Bottom + border + 4);
            nudSmoothBalancePercent.Width = nudWidth;
            nudSmoothBalancePercent.Location = new Point(nudAmbiguousBars.Left, chbSmoothBalanceLines.Top - 1);
            nudSmoothBalanceCheckPoints.Width = nudWidth;
            nudSmoothBalanceCheckPoints.Location = new Point(nudSmoothBalancePercent.Left - nudWidth - border, chbSmoothBalanceLines.Top - 1);

            // chbOutOfSample
            chbOutOfSample.Location = new Point(border + 2, 25);
            nudOutOfSample.Width = nudWidth;
            nudOutOfSample.Location = new Point(nudAmbiguousBars.Left, chbOutOfSample.Top - 1);

            // Use default indicator values
            chbUseDefaultIndicatorValues.Location = new Point(border + 2, chbOutOfSample.Bottom + border + 4);

            // Hide FSB when generator starts
            chbHideFSB.Location = new Point(border + 2, chbUseDefaultIndicatorValues.Bottom + border + 4);

            // Button Reset
            btnReset.Width = pnlSettings.ClientSize.Width - 2 * (border + 2);
            btnReset.Location = new Point(border + 2, pnlSettings.Height - btnReset.Height - border - 2);

            // Top 10 Layout
            top10Layout.Size = new Size(pnlTop10.Width - 2 * 2, pnlTop10.Height - (int)pnlTop10.CaptionHeight - 2);
            top10Layout.Location = new Point(2, (int)pnlTop10.CaptionHeight);

            // Indicators Layout
            indicatorsLayout.Size = new Size(pnlIndicators.Width - 2 * 2, pnlIndicators.Height - (int)pnlIndicators.CaptionHeight - 2);
            indicatorsLayout.Location = new Point(2, (int)pnlIndicators.CaptionHeight);
            return;
        }

        /// <summary>
        /// Check whether the strategy have been changed
        /// </summary>
        void Generator_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isReset)
                SaveOptions();

            if (isGenerating)
            {   // Cancel the asynchronous operation.
                bgWorker.CancelAsync();
                e.Cancel = true;
                return;
            }
            else if (DialogResult == DialogResult.Cancel && isStartegyChanged)
            {
                DialogResult dr = MessageBox.Show(Language.T("Do you want to accept the generated strategy?"),
                            Data.ProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (dr == DialogResult.Cancel)
                { 
                    e.Cancel = true;
                    return;
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

            if (!isReset)
                indicatorsLayout.SetConfigFile();

            Data.Strategy = ClearStrategySlotsStatus(Data.Strategy);
            formFSB.Visible = true;

            return;
        }

        delegate void DelegateRefreshBalanceChart();
        delegate void DelegateRefreshAccountStatisticas();
        delegate void DelegateRebuildStrategyLayout(Strategy strategy);
        delegate void SetCyclesCallback(string text);
        delegate void DelegateReportIndicatorError(string text, string caption);

        /// <summary>
        /// Refreshes the balance chart
        /// </summary>
        void RefreshSmallBalanceChart()
        {
            if (smallBalanceChart.InvokeRequired)
            {
                Invoke(new DelegateRefreshBalanceChart(RefreshSmallBalanceChart), new object[] { });
            }
            else
            {
                smallBalanceChart.SetChartData();
                smallBalanceChart.InitChart();
                smallBalanceChart.Invalidate();
            }

            return;
        }

        /// <summary>
        /// Refreshes the AccountStatistics
        /// </summary>
        void RefreshAccountStatisticas()
        {
            if (infpnlAccountStatistics.InvokeRequired)
            {
                Invoke(new DelegateRefreshAccountStatisticas(RefreshAccountStatisticas), new object[] { });
            }
            else
            {
                infpnlAccountStatistics.Update(
                    Backtester.AccountStatsParam,
                    Backtester.AccountStatsValue,
                    Backtester.AccountStatsFlags,
                    Language.T("Account Statistics"));
            }

            return;
        }

        /// <summary>
        /// Creates a new strategy layout according to the given strategy.
        /// </summary>
        void RebuildStrategyLayout(Strategy strategy)
        {
            if (strategyLayout.InvokeRequired)
            {
                Invoke(new DelegateRebuildStrategyLayout(RebuildStrategyLayout), new object[] { strategy });
            }
            else
            {
                strategyLayout.RebuildStrategyControls(strategy);
                strategyLayout.pnlProperties.Click       += new EventHandler(PnlProperties_Click);
                strategyLayout.pnlProperties.DoubleClick += new EventHandler(PnlProperties_Click);
                for (int slot = 0; slot < strategy.Slots; slot++)
                {
                    strategyLayout.apnlSlot[slot].Click       += new EventHandler(PnlSlot_Click);
                    strategyLayout.apnlSlot[slot].DoubleClick += new EventHandler(PnlSlot_Click);
                }
            }

            return;
        }

        /// <summary>
        /// Sets the lblCalcStrNumb.Text
        /// </summary>
        void SetLabelCyclesText(string text)
        {
            if (lblCalcStrNumb.InvokeRequired)
            {
                BeginInvoke(new SetCyclesCallback(SetLabelCyclesText), new object[] { text });
            }
            else
            {
                lblCalcStrNumb.Text = text;
            }
        }

        /// <summary>
        /// Composes an informative error message. It presumes that the reason for the error is a custom indicator. Ohhh!!
        /// </summary>
        string GenerateCalculationErrorMessage(string exceptionMessage)
        {
            string text = "<h1>Error: " + exceptionMessage + "</h1>";
            string customIndicators = "";
            int    customIndCount   = 0;

            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                string indName = Data.Strategy.Slot[slot].IndicatorName;
                Indicator indicator = Indicator_Store.ConstructIndicator(indName, Data.Strategy.Slot[slot].SlotType);
                if (indicator.CustomIndicator)
                {
                    customIndCount++;
                    indicatorBlackList.Add(indName);
                    customIndicators += "<li>" + Data.Strategy.Slot[slot].IndicatorName + "</li>" + Environment.NewLine;
                }
            }

            if (customIndCount > 0)
            {
                string plural = (customIndCount > 1 ? "s" : "");

                text +=
                    "<p>" +
                        "An error occurred when calculating the strategy." + " " +
                        "The error can be a result of the following custom indicator" + plural + ":" +
                    "</p>" +
                    "<ul>" +
                        customIndicators +
                    "</ul>" +
                    "<p>" +
                        "Please report this error to the author of the indicator" + plural + "!<br />" +
                        "You may remove this indicator" + plural + " from the Custom Indicators folder." +
                    "</p>";
            }
            else
            {
                text +=
                    "<p>" +
                        "Please report this error in the support forum!" +
                    "</p>";

            }

            return text;
        }

        /// <summary>
        /// Report Indicator Error
        /// </summary>
        void ReportIndicatorError(string text, string caption)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new DelegateReportIndicatorError(ReportIndicatorError), new object[] { text, caption });
            }
            else
            {
                Fancy_Message_Box msgBox = new Fancy_Message_Box(text, caption);
                msgBox.BoxWidth  = 450;
                msgBox.BoxHeight = 250;
                msgBox.Show();
            }
        }

        /// <summary>
        /// Out of Sample
        /// </summary>
        void NudOutOfSample_ValueChanged(object sender, EventArgs e)
        {
            SetOOS();
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
            SetOOS();

            smallBalanceChart.OOS    = isOOS;
            smallBalanceChart.OOSBar = barOOS;

            smallBalanceChart.SetChartData();
            smallBalanceChart.InitChart();
            smallBalanceChart.Invalidate();
        }

        /// <summary>
        /// Out of Sample
        /// </summary>
        void SetOOS()
        {
            isOOS  = chbOutOfSample.Checked;
            barOOS = Data.Bars - Data.Bars * (int)nudOutOfSample.Value / 100 - 1;
            targetBalanceRatio = 1 + (int)nudOutOfSample.Value / 100.0F;
        }

        /// <summary>
        /// Generates a description
        /// </summary>
        string GenerateDescription()
        {
            // Description
            if (lockedEntryFilters == 0  && lockedExitFilters == 0 &&
                lockedEntrySlot == null  && lockedExitSlot == null &&
                strategyBest.PropertiesStatus == StrategySlotStatus.Open)
            {
                isStrategyModified = false;
                generatedDescription = Language.T("Automatically generated on") + " ";
            }
            else
            {
                isStrategyModified = true;
                generatedDescription = Language.T("Modified by the strategy generator on") + " ";
            }

            generatedDescription += DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + ".";

            if (isOOS)
            {
                generatedDescription += Environment.NewLine + Language.T("Out of sample testing, percent of OOS bars") + ": " + nudOutOfSample.Value.ToString() + "%";
                generatedDescription += Environment.NewLine + Language.T("Balance") + ": " +
                    (Configs.AccountInMoney ? Backtester.MoneyBalance(barOOS).ToString("F2") + " " + Configs.AccountCurrency : Backtester.Balance(barOOS).ToString() + " " + Language.T("pips"));
                generatedDescription += " (" + Data.Time[barOOS].ToShortDateString() + " " + Data.Time[barOOS].ToShortTimeString() + "  " + Language.T("Bar") + ": " + barOOS.ToString() + ")";
            }

            return generatedDescription;
        }

        /// <summary>
        /// Toggles FSB visibility.
        /// </summary>
        void HideFSB_Click(object sender, EventArgs e)
        {
            formFSB.Visible = !chbHideFSB.Checked;
        }

        /// <summary>
        /// Toggles panels.
        /// </summary>
        void ChangeGeneratorPanel(object sender, EventArgs e)
        {
            ToolStripButton button = (ToolStripButton)sender;
            string name = button.Name;

            if (name == "tsbtShowOptions")
            {
                pnlCommon.Visible = true;
                pnlLimitations.Visible = false;
                pnlSettings.Visible = false;
                pnlTop10.Visible = false;
                pnlIndicators.Visible = false;

                tsbtShowOptions.Enabled = false;
                tsbtShowLimitations.Enabled = true;
                tsbtShowSettings.Enabled = true;
                tsbtShowTop10.Enabled = true;
                tsbtShowIndicators.Enabled = true;
            }
            else if (name == "tsbtShowLimitations")
            {
                pnlCommon.Visible = false;
                pnlLimitations.Visible = true;
                pnlSettings.Visible = false;
                pnlTop10.Visible = false;
                pnlIndicators.Visible = false;

                tsbtShowOptions.Enabled = true;
                tsbtShowLimitations.Enabled = false;
                tsbtShowSettings.Enabled = true;
                tsbtShowTop10.Enabled = true;
                tsbtShowIndicators.Enabled = true;
            }
            else if (name == "tsbtShowSettings")
            {
                pnlCommon.Visible = false;
                pnlLimitations.Visible = false;
                pnlSettings.Visible = true;
                pnlTop10.Visible = false;
                pnlIndicators.Visible = false;

                tsbtShowOptions.Enabled = true;
                tsbtShowLimitations.Enabled = true;
                tsbtShowSettings.Enabled = false;
                tsbtShowTop10.Enabled = true;
                tsbtShowIndicators.Enabled = true;
            }
            else if (name == "tsbtShowTop10")
            {
                pnlCommon.Visible = false;
                pnlLimitations.Visible = false;
                pnlSettings.Visible = false;
                pnlTop10.Visible = true;
                pnlIndicators.Visible = false;

                tsbtShowOptions.Enabled = true;
                tsbtShowLimitations.Enabled = true;
                tsbtShowSettings.Enabled = true;
                tsbtShowTop10.Enabled = false;
                tsbtShowIndicators.Enabled = true;
            }
            else if (name == "tsbtIndicators")
            {
                pnlCommon.Visible = false;
                pnlLimitations.Visible = false;
                pnlSettings.Visible = false;
                pnlTop10.Visible = false;
                pnlIndicators.Visible = true;

                tsbtShowOptions.Enabled = true;
                tsbtShowLimitations.Enabled = true;
                tsbtShowSettings.Enabled = true;
                tsbtShowTop10.Enabled = true;
                tsbtShowIndicators.Enabled = false;
            }
        }

        /// <summary>
        /// Shows strategy overview.
        /// </summary>
        protected virtual void Show_Overview(object sender, EventArgs e)
        {
            if (generatedDescription != string.Empty)
                Data.Strategy.Description = generatedDescription;

            Browser so = new Browser(Language.T("Strategy Overview"), Data.Strategy.GenerateHTMLOverview());
            so.Show();

            return;
        }

        /// <summary>
        /// Lock, unlock, link all slots.
        /// </summary>
        protected virtual void ChangeSlotStatus(object sender, EventArgs e)
        {
            ToolStripButton button = (ToolStripButton)sender;
            string name = button.Name;

            if (name == "tsbtLockAll")
            {
                strategyBest.PropertiesStatus = StrategySlotStatus.Locked;
                for (int slot = 0; slot < strategyBest.Slots; slot++)
                    strategyBest.Slot[slot].SlotStatus = StrategySlotStatus.Locked;
            }
            else if (name == "tsbtUnlockAll")
            {
                strategyBest.PropertiesStatus = StrategySlotStatus.Open;
                for (int slot = 0; slot < strategyBest.Slots; slot++)
                    strategyBest.Slot[slot].SlotStatus = StrategySlotStatus.Open;
            }
            else if (name == "tsbtLinkAll")
            {
                strategyBest.PropertiesStatus = StrategySlotStatus.Open;
                for (int slot = 0; slot < strategyBest.Slots; slot++)
                    strategyBest.Slot[slot].SlotStatus = StrategySlotStatus.Linked;
            }

            strategyLayout.RepaintStrategyControls(strategyBest);
        }

        /// <summary>
        /// Lock, link, or unlock the strategy slot.
        /// </summary>
        void PnlSlot_Click(object sender, EventArgs e)
        {
            if (isGenerating)
                return;

            int slot = (int)((Panel)sender).Tag;

            if (strategyBest.Slot[slot].SlotStatus == StrategySlotStatus.Open)
                strategyBest.Slot[slot].SlotStatus = StrategySlotStatus.Locked;
            else if (strategyBest.Slot[slot].SlotStatus == StrategySlotStatus.Locked)
                strategyBest.Slot[slot].SlotStatus = StrategySlotStatus.Linked;
            else if (strategyBest.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                strategyBest.Slot[slot].SlotStatus = StrategySlotStatus.Open;

            strategyLayout.RepaintStrategyControls(strategyBest);

            return;
        }

        /// <summary>
        /// Lock, link, or unlock the strategy properties slot.
        /// </summary>
        void PnlProperties_Click(object sender, EventArgs e)
        {
            if (isGenerating)
                return;

            if (strategyBest.PropertiesStatus == StrategySlotStatus.Open)
                strategyBest.PropertiesStatus = StrategySlotStatus.Locked;
            else if (strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
                strategyBest.PropertiesStatus = StrategySlotStatus.Open;

            strategyLayout.RepaintStrategyControls(strategyBest);

            return;
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

            strategyLayout.RearangeStrategyControls();
        }

        /// <summary>
        /// Vew / edit the strategy description
        /// </summary>
        protected virtual void BtnStrategyDescription_Click(object sender, EventArgs e)
        {
            if (generatedDescription != string.Empty)
                Data.Strategy.Description = generatedDescription;
            string sOldInfo = Data.Strategy.Description;
            Strategy_Description si = new Strategy_Description();
            si.ShowDialog();
            generatedDescription = Data.Strategy.Description;
        }

        /// <summary>
        /// Sets the strategy description button icon
        /// </summary>
        void SetSrategyDescriptionButton()
        {
            if (generatedDescription != string.Empty)
                Data.Strategy.Description = generatedDescription;

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

        /// <summary>
        /// Clears the slots status of the given strategy.
        /// </summary>
        Strategy ClearStrategySlotsStatus(Strategy strategy)
        {
            Strategy tempStrategy = strategy.Clone();
            tempStrategy.PropertiesStatus = StrategySlotStatus.Open;
            foreach (IndicatorSlot slot in tempStrategy.Slot)
                slot.SlotStatus = StrategySlotStatus.Open;

            return tempStrategy;
        }

        /// <summary>
        /// Saves the Generator History
        /// </summary>
        void AddStrategyToGeneratorHistory(string description)
        {
            Strategy strategy = ClearStrategySlotsStatus(strategyBest);
            Data.GeneratorHistory.Add(strategy);
            Data.GeneratorHistory[Data.GeneratorHistory.Count - 1].Description = description;

            if (Data.GeneratorHistory.Count >= 110)
                Data.GeneratorHistory.RemoveRange(0, 10);

            Data.GenHistoryIndex = Data.GeneratorHistory.Count - 1;
        }

        /// <summary>
        /// Updates the last strategy in Generator History
        /// </summary>
        void UpdateStrategyInGeneratorHistory(string description)
        {
            if (Data.GeneratorHistory.Count == 0)
                return;

            Strategy strategy = ClearStrategySlotsStatus(strategyBest);
            Data.GeneratorHistory[Data.GeneratorHistory.Count - 1] = strategy;
            Data.GeneratorHistory[Data.GeneratorHistory.Count - 1].Description = description;
        }

        delegate void DelegateTop10AddStrategy();
        /// <summary>
        /// Adds a strategy to Top 10 list.
        /// </summary>
        void Top10AddStrategy()
        {
            if (top10Layout.InvokeRequired)
            {
                Invoke(new DelegateTop10AddStrategy(Top10AddStrategy), new object[] { });
            }
            else
            {
                Top10Slot top10Slot = new Top10Slot();
                top10Slot.Width  = 290;
                top10Slot.Height = 65;
                top10Slot.InitSlot();
                top10Slot.Click       += new EventHandler(Top10Slot_Click);
                top10Slot.DoubleClick += new EventHandler(Top10Slot_Click);
                Top10StrategyInfo top10StrategyInfo = new Top10StrategyInfo();
                top10StrategyInfo.Balance     = Configs.AccountInMoney ? (int)Math.Round(Backtester.NetMoneyBalance) : Backtester.NetBalance;
                top10StrategyInfo.Top10Slot   = top10Slot;
                top10StrategyInfo.TheStrategy = Data.Strategy.Clone();
                top10Layout.AddStrategyInfo(top10StrategyInfo);
            }
        }

        /// <summary>
        /// Loads a strategy from the clicked Top 10 slot.
        /// </summary>
        void Top10Slot_Click(object sender, EventArgs e)
        {
            if (isGenerating)
                return;

            Top10Slot top10Slot = (Top10Slot)sender;
            
            if (top10Slot.IsSelected)
                return;

            top10Layout.ClearSelectionOfSelectedSlot();

            top10Slot.IsSelected = true;
            top10Slot.Invalidate();

            Data.Strategy = top10Layout.GetStrategy(top10Slot.Balance);
            bestBalance = 0;
            CalculateTheResult(true);
        }

        /// <summary>
        /// Toggles the account chart and statistics.
        /// </summary>
        void AccountAutput_Click(object sender, EventArgs e)
        {
            bool isChartVisible = smallBalanceChart.Visible;
            smallBalanceChart.Visible = !isChartVisible;
            infpnlAccountStatistics.Visible = isChartVisible;
        }

        /// <summary>
        /// Resets Generator
        /// </summary>
        void BtnReset_Click(object sender, EventArgs e)
        {
            Configs.GeneratorOptions = "";
            Configs.BannedIndicators = "";
            isReset = true;
            Close();
        }
    }
}
