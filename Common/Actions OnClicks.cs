// Actions OnClick
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// Part of Forex Strategy Builder
// Website http://forexsb.com
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Actions : Controls
    /// </summary>
    public partial class Actions : Controls
    {
        /// <summary>
        /// Changes the Full Screen mode.
        /// </summary>
        protected override void MenuViewFullScreen_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            if (mi.Checked)
                FormState.Maximize(this);
            else
                FormState.Restore(this);

            return;
        }

        /// <summary>
        /// Opens the averaging parameters dialog.
        /// </summary>
        protected override void PnlAveraging_Click(object sender, EventArgs e)
        {
            EditStrategyProperties();

            return;
        }

        /// <summary>
        /// Opens the indicator parameters dialog.
        /// </summary>
        protected override void PnlSlot_MouseUp(object sender, MouseEventArgs e)
        {
            Panel panel = (Panel)sender;
            int   tag   = (int)panel.Tag;
            if (e.Button == MouseButtons.Left)
                EditSlot(tag);

            return;
        }

        /// <summary>
        /// Strategy panel menu items clicked
        /// </summary>
        protected override void SlotContextMenu_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            int tag = (int)mi.Tag;
            switch (mi.Name)
            {
                case "Edit":
                    EditSlot(tag);
                    break;
                case "Upwards":
                    MoveSlotUpwards(tag);
                    break;
                case "Downwards":
                    MoveSlotDownwards(tag);
                    break;
                case "Duplicate":
                    DuplicateSlot(tag);
                    break;
                case "Delete":
                    RemoveSlot(tag);
                    break;
                default:
                    break;
            }

            return;
        }

        /// <summary>
        /// Performs actions after the button add open filter was clicked.
        /// </summary>
        protected override void BtnAddOpenFilter_Click(object sender, EventArgs e)
        {
            AddOpenFilter();

            return;
        }

        /// <summary>
        /// Performs actions after the button add close filter was clicked.
        /// </summary>
        protected override void BtnAddCloseFilter_Click(object sender, EventArgs e)
        {
            AddCloseFilter();

            return;
        }

        /// <summary>
        /// Performs actions after selecting a new ComboBox item.
        /// Handler for: cbxMode, cbxSymbol, cbxPeriod, tscbInterpolationMethod
        /// </summary>
        protected override void SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isDiscardSelectedIndexChange)
                return;

            ToolStripComboBox cbx = (ToolStripComboBox)sender;

            if (cbx.Name == "tscbMode")
            {
                Configs.UseTickData = cbx.SelectedIndex == 1;
            }

            if (cbx.Name == "tscbInterpolationMethod")
            {
                Backtester.InterpolationMethod = (InterpolationMethod)(Enum.GetValues(typeof(InterpolationMethod)).GetValue(tscbInterpolationMethod.SelectedIndex));
            }

            if (cbx.Name == "tscbSymbol" || cbx.Name == "tscbPeriod")
            {
                if (LoadInstrument(false) == 0)
                {
                    Calculate(true);
                    PrepareScannerCompactMode();
                }
                else
                {
                    SetMarket(Data.Symbol, Data.Period);
                }
            }
            else
            {
                Calculate(false);
            }

            return;
        }

        /// <summary>
        /// Whether to express account in pips or in currency
        /// </summary>
        protected override void AccountShowInMoney_OnClick(object sender, EventArgs e)
        {
            if(((ToolStripMenuItem)sender).Name == "miAccountShowInMoney")
            {
                Configs.AccountInMoney       = true;
                miAccountShowInMoney.Checked = true;
                miAccountShowInPips.Checked  = false;
            }
            else if(((ToolStripMenuItem)sender).Name == "miAccountShowInPips")
            {
                Configs.AccountInMoney       = false;
                miAccountShowInMoney.Checked = false;
                miAccountShowInPips.Checked  = true;
            }

            Calculate(false);

            return;
        }

        /// <summary>
        /// Opens the account setting dialog
        /// </summary>
        protected override void MenuAccountSettings_OnClick(object sender, EventArgs e)
        {
            ShowAccountSettings();

            return;
        }

        /// <summary>
        /// Load a color scheme.
        /// </summary>
        protected override void MenuLoadColor_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            if (!mi.Checked)
            {
                Configs.ColorScheme = mi.Name;
            }
            foreach (ToolStripMenuItem tsmi in mi.Owner.Items)
            {
                tsmi.Checked = false;
            }
            mi.Checked = true;

            LoadColorScheme();

            return;
        }

        /// <summary>
        /// Performs actions corresponding on the menu item Load.
        /// </summary>
        protected override void MenuLoadData_OnClick(object sender, EventArgs e)
        {
            if (LoadInstrument(false) == 0)
                Calculate(true);

            PrepareScannerCompactMode();

            return;
        }

        /// <summary>
        /// Check the data.
        /// </summary>
        protected override void MenuCheckData_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            Configs.CheckData = tsmi.Checked;

            CheckLoadedData();

            return;
        }

        /// <summary>
        /// Refine the data
        /// </summary>
        protected override void MenuRefineData_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            if (tsmi.Name == "miCutBadData")
                Configs.CutBadData = tsmi.Checked;

            if (tsmi.Name == "miFillDataGaps")
                Configs.FillInDataGaps = tsmi.Checked;

            if (LoadInstrument(false) == 0)
                Calculate(true);

            return;
        }

        /// <summary>
        /// Data Horizon
        /// </summary>
        protected override void MenuDataHorizon_OnClick(object sender, EventArgs e)
        {
            DataHorizon();

            return;
        }

        /// <summary>
        /// Data Directory
        /// </summary>
        protected override void MenuDataDirectory_OnClick(object sender, EventArgs e)
        {
            DataDirectory dataDirectory = new DataDirectory();

            if (dataDirectory.ShowDialog() == DialogResult.OK)
            {
                string dataDirPath = dataDirectory.DataFolder;

                if (dataDirPath == "")
                    Data.OfflineDataDir = Data.DefaultOfflineDataDir;
                else if (dataDirPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    Data.OfflineDataDir = dataDirPath;
                else
                    Data.OfflineDataDir = dataDirPath + Path.DirectorySeparatorChar;

                if (LoadInstrument(false) == 0)
                {
                    Calculate(true);

                    // The new folder will be saved in the config file only when
                    // the data are loaded successfully
                    if (Data.OfflineDataDir == Data.DefaultOfflineDataDir)
                        Configs.DataDirectory = "";
                    else
                        Configs.DataDirectory = Data.OfflineDataDir;

                    PrepareScannerCompactMode();
                }
            }

            return;
        }

        /// <summary>
        /// Opens the strategy settings dialog.
        /// </summary>
        protected override void MenuStrategySettings_OnClick(object sender, EventArgs e)
        {
            UsePreviousBarValue_Change();

            return;
        }

        /// <summary>
        /// Autos can
        /// </summary>
        protected override void MenuStrategyAutoscan_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Configs.Autoscan = mi.Checked;
            Calculate(false);

            if (mi.Checked && !Data.IsIntrabarData)
            {
                PrepareScannerCompactMode();
            }

            return;
        }

        /// <summary>
        /// TradeUntillMC
        /// </summary>
        protected override void TradeUntilMC_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Configs.TradeUntilMarginCall = mi.Checked;
            Calculate(false);

            return;
        }

        /// <summary>
        /// AdditionalStats_OnClick
        /// </summary>
        protected override void AdditionalStats_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Configs.AdditionalStatistics = mi.Checked;
            Calculate(false);

            return;
        }

        /// <summary>
        /// Opens the strategy settings dialogue.
        /// </summary>
        protected override void MenuStrategyAUPBV_OnClick(object sender, EventArgs e)
        {
            UsePreviousBarValue_Change();

            return;
        }

        /// <summary>
        /// Export the strategy in BBCode format - ready to post in the forum
        /// </summary>
        protected override void MenuStrategyBBcode_OnClick(object sender, EventArgs e)
        {
            PublishStrategy();

            return;
        }

        /// <summary>
        /// Remove the corresponding indicator slot.
        /// </summary>
        protected override void BtnRemoveSlot_Click(object sender, EventArgs e)
        {
            int iSlot = (int)((Button)sender).Tag;

            RemoveSlot(iSlot);

            return;
        }

        /// <summary>
        /// Forces the calculation of the strategy.
        /// </summary>
        protected override void MenuAnalysisCalculate_OnClick(object sender, EventArgs e)
        {
            Calculate(true);

            return;
        }

        /// <summary>
        /// Forces the intrabar scanning of the strategy.
        /// </summary>
        protected override void MenuQuickScan_OnClick(object sender, EventArgs e)
        {
            Scan();

            return;
        }

        /// <summary>
        /// Performs a detailed back-test.
        /// </summary>
        protected override void MenuDetailedBacktest_OnClick(object sender, EventArgs e)
        {
            ShowScanner();

            return;
        }

        /// <summary>
        /// Loads the default strategy.
        /// </summary>
        protected override void MenuStrategyNew_OnClick(object sender, EventArgs e)
        {
            NewStrategy();

            return;
        }

        /// <summary>
        /// Opens the dialog form OpenFileDialog.
        /// </summary>
        protected override void MenuFileOpen_OnClick(object sender, EventArgs e)
        {
            OpenFile();

            return;
        }

        /// <summary>
        /// Saves the strategy.
        /// </summary>
        protected override void MenuFileSave_OnClick(object sender, EventArgs e)
        {
            SaveStrategy();

            return;
        }

        /// <summary>
        /// Opens the dialog form SaveFileDialog.
        /// </summary>
        protected override void MenuFileSaveAs_OnClick(object sender, EventArgs e)
        {
            SaveAsStrategy();

            return;
        }

        /// <summary>
        /// Undoes the strategy.
        /// </summary>
        protected override void MenuStrategyUndo_OnClick(object sender, EventArgs e)
        {
            UndoStrategy();

            return;
        }

        /// <summary>
        /// Loads the previously generated strategy
        /// </summary>
        protected override void MenuPrevHistory_OnClick(object sender, EventArgs e)
        {
            if (Data.GeneratorHistory.Count > 0 && Data.GenHistoryIndex > 0)
            {
                Data.GenHistoryIndex--;
                Data.Strategy = Data.GeneratorHistory[Data.GenHistoryIndex].Clone();
                RebuildStrategyLayout();
                Calculate(true);
            }

            return;
        }

        /// <summary>
        /// Loads the next generated strategy
        /// </summary>
        protected override void MenuNextHistory_OnClick(object sender, EventArgs e)
        {
            if (Data.GeneratorHistory.Count > 0 && Data.GenHistoryIndex < Data.GeneratorHistory.Count - 1)
            {
                Data.GenHistoryIndex++;
                Data.Strategy = Data.GeneratorHistory[Data.GenHistoryIndex].Clone();
                RebuildStrategyLayout();
                Calculate(true);
            }

            return;
        }

        /// <summary>
        /// Tools menu
        /// </summary>
        protected override void MenuTools_OnClick(object sender, EventArgs e)
        {
            string name = ((ToolStripMenuItem)sender).Name;

            switch (name)
            {
                case "Comparator":
                    ShowComparator();
                    break;
                case "Scanner":
                    ShowScanner();
                    break;
                case "Generator":
                    ShowGenerator();
                    break;
                case "Optimizer":
                    ShowOptimizer();
                    break;
                case "Bar Explorer":
                    ShowBarExplorer();
                    break;
                case "ProfitCalculator":
                    ShowProfitCalculator();
                    break;
                case "PivotPoints":
                    ShowPivotPoints();
                    break;
                case "FibonacciLevels":
                    ShowFibonacciLevels();
                    break;
                case "Charges":
                    EditTradingCharges();
                    break;
                case "miInstrumentEditor":
                    ShowInstrumentEditor();
                    break;
                case "Reset settings":
                    ResetSettings();
                    break;
                case "miNewTranslation":
                    MakeNewTranslation();
                    break;
                case "miEditTranslation":
                    EditTranslation();
                    break;
                case "miShowEnglishPhrases":
                    Language.ShowPhrases(1);
                    break;
                case "miShowAltPhrases":
                    Language.ShowPhrases(2);
                    break;
                case "miShowAllPhrases":
                    Language.ShowPhrases(3);
                    break;
                case "miOpenIndFolder":
                    try { System.Diagnostics.Process.Start(Data.SourceFolder); }
                    catch (System.Exception ex) { MessageBox.Show(ex.Message); }
                    break;
                case "miReloadInd":
                    Cursor = Cursors.WaitCursor;
                    ReloadCustomIndicators();
                    Cursor = Cursors.Default;
                    break;
                case "miExportAsCI":
                    Cursor = Cursors.WaitCursor;
                    Strategy_to_Indicator.ExportStrategyToIndicator();
                    ReloadCustomIndicators();
                    Cursor = Cursors.Default;
                    break;
                case "miCheckInd":
                    Custom_Indicators.TestCustomIndicators();
                    break;
                case "Calculator":
                    ShowCalculator();
                    break;
                case "miPlaySounds":
                    Configs.PlaySounds = !Configs.PlaySounds;
                    break;
                case "CommandConsole":
                    ShowCommandConsole();
                    break;
                case "miJForexImport":
                    JForexImport();
                    break;
                case "tsmiOverOptimization": // Analyzer
                    ShowAnalyzer("tsmiOverOptimization");
                    break;
                case "tsmiCumulativeStrategy": // Analyzer
                    ShowAnalyzer("tsmiCumulativeStrategy");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Tools button
        /// </summary>
        protected override void BtnTools_OnClick(object sender, EventArgs e)
        {
            string name = ((ToolStripButton)sender).Name;

            switch (name)
            {
                case "Analyzer":
                    ShowAnalyzer("tsmiOverOptimization");
                    break;
                case "Overview":
                    ShowOverview();
                    break;
                case "Comparator":
                    ShowComparator();
                    break;
                case "Scanner":
                    ShowScanner();
                    break;
                case "Generator":
                    ShowGenerator();
                    break;
                case "Optimizer":
                    ShowOptimizer();
                    break;
                case "Charges":
                    EditTradingCharges();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Reset settings
        /// </summary>
        void ResetSettings()
        {
            DialogResult result = MessageBox.Show(
                Language.T("Do you want to reset all settings?") + Environment.NewLine + Environment.NewLine +
                Language.T("Restart the program to activate the changes!"),
                Language.T("Reset Settings"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (result == DialogResult.OK)
                Configs.ResetParams();
        }

        /// <summary>
        /// Menu Journal
        /// </summary>
        protected override void MenuJournal_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem timItem = (ToolStripMenuItem)sender;

            if (timItem.Name == "miJournalByPos")
            {
                if (miJournalByPos.Checked)
                {
                    miJournalByPos.Checked  = false;
                    miJournalByBars.Checked = false;
                    Configs.ShowJournal     = false;
                    Configs.JournalByBars   = false;
                }
                else
                {
                    miJournalByPos.Checked  = true;
                    miJournalByBars.Checked = false;
                    Configs.ShowJournal     = true;
                    Configs.JournalByBars   = false;
                }
            }
            else if (timItem.Name == "miJournalByBars")
            {
                if (miJournalByBars.Checked)
                {
                    miJournalByPos.Checked  = false;
                    miJournalByBars.Checked = false;
                    Configs.ShowJournal     = false;
                    Configs.JournalByBars   = true;
                }
                else
                {
                    miJournalByPos.Checked  = false;
                    miJournalByBars.Checked = true;
                    Configs.ShowJournal     = true;
                    Configs.JournalByBars   = true;
                }
            }

            ResetJournal();

            return;
        }

        /// <summary>
        /// Starts the Analyzer.
        /// </summary>
        void ShowAnalyzer(string menuItem)
        {
            Dialogs.Analyzer.Analyzer analyzer = new Dialogs.Analyzer.Analyzer(menuItem);
            analyzer.SetParrentForm = this;
            analyzer.ShowDialog();

            return;
        }

        /// <summary>
        /// Starts the Calculator.
        /// </summary>
        void ShowCalculator()
        {
            Calculator calc = new Calculator();
            calc.Show();

            return;
        }

        /// <summary>
        /// Starts the Profit Calculator.
        /// </summary>
        void ShowProfitCalculator()
        {
            Profit_Calculator prCalc = new Profit_Calculator();
            prCalc.Show();

            return;
        }

        /// <summary>
        /// Starts the Pivot Points Calculator.
        /// </summary>
        void ShowPivotPoints()
        {
            Pivot_Points_Calculator ppCalc = new Pivot_Points_Calculator();
            ppCalc.Show();

            return;
        }

        /// <summary>
        /// Starts the Fibonacci Levels Calculator.
        /// </summary>
        void ShowFibonacciLevels()
        {
            Fibonacci_Levels_Calculator flCalc = new Fibonacci_Levels_Calculator();
            flCalc.Show();

            return;
        }

        /// <summary>
        /// Starts the Calculator.
        /// </summary>
        void ShowCommandConsole()
        {
            Command_Console commandConsole = new Command_Console();
            commandConsole.Show();

            return;
        }

        /// <summary>
        /// Makes new language file.
        /// </summary>
        void MakeNewTranslation()
        {
            New_Translation nt = new New_Translation();
            nt.Show();

            return;
        }

        /// <summary>
        /// Edit translation.
        /// </summary>
        void EditTranslation()
        {
            Edit_Translation et = new Edit_Translation();
            et.Show();

            return;
        }

        /// <summary>
        /// Starts JForexImport.
        /// </summary>
        void JForexImport()
        {
            JForex_Import jf = new JForex_Import();
            jf.ShowDialog();

            return;
        }

        /// <summary>
        /// Use logical groups menu item.
        /// </summary>
        protected override void MenuUseLogicalGroups_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;

            if (mi.Checked == true)
            {
                Configs.UseLogicalGroups = mi.Checked;
                RebuildStrategyLayout();
                return;
            }

            // Check if the current strategy uses logical groups
            bool usefroup = false;
            List<string> closegroup = new List<string>();
            foreach (IndicatorSlot slot in Data.Strategy.Slot)
            {
                if (slot.SlotType == SlotTypes.OpenFilter && slot.LogicalGroup != "A")
                    usefroup = true;

                if (slot.SlotType == SlotTypes.CloseFilter)
                {
                    if (closegroup.Contains(slot.LogicalGroup) || slot.LogicalGroup == "all")
                        usefroup = true;
                    else
                        closegroup.Add(slot.LogicalGroup);
                }
            }

            if (!usefroup)
            {
                Configs.UseLogicalGroups = false;
                RebuildStrategyLayout();
            }
            else
            {
                MessageBox.Show(
                    Language.T("The strategy requires logical groups.") + Environment.NewLine +
                    Language.T("\"Use Logical Groups\" option cannot be switched off."),
                    Language.T("Logical Groups"),
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Information);

                mi.Checked = true;
            }

            return;
        }

        /// <summary>
        /// Menu MenuOpeningLogicSlots_OnClick.
        /// </summary>
        protected override void MenuOpeningLogicSlots_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Configs.MAX_ENTRY_FILTERS = (int)mi.Tag;

            foreach (ToolStripMenuItem m in mi.Owner.Items)
                m.Checked = ((int)m.Tag == Configs.MAX_ENTRY_FILTERS);

            RebuildStrategyLayout();
            return;
        }

        /// <summary>
        /// Menu MenuClosingLogicSlots_OnClick.
        /// </summary>
        protected override void MenuClosingLogicSlots_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Configs.MAX_EXIT_FILTERS = (int)mi.Tag;

            foreach (ToolStripMenuItem m in mi.Owner.Items)
                m.Checked = ((int)m.Tag == Configs.MAX_EXIT_FILTERS);

            RebuildStrategyLayout();
            return;
        }

        /// <summary>
        /// Menu ShowPriceLine_OnClick.
        /// </summary>
        protected override void ShowPriceLine_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Configs.ShowPriceChartOnAccountChart = mi.Checked;

            smallBalanceChart.SetChartData();
            smallBalanceChart.InitChart();
            smallBalanceChart.Invalidate();

            return;
        }

        /// <summary>
        /// Sets the program's language.
        /// </summary>
        protected override void Language_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            if (!mi.Checked)
            {
                Configs.Language = mi.Name;
                Language.InitLanguages();

                MainMenuStrip.Items.Clear();
                InitializeMenu();

                statusStrip.Items.Clear();
                InitializeStatusBar();

                Calculate(false);
                RebuildStrategyLayout();
                infpnlMarketStatistics.Update(Data.MarketStatsParam, Data.MarketStatsValue, Data.MarketStatsFlag, Language.T("Market Statistics"));
                SetupJournal();
                pnlWorkspace.Invalidate(true);
                string messageText = Language.T("Restart the program to activate the changes!");
                MessageBox.Show(messageText, Language.T("Language Change"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            foreach (ToolStripMenuItem tsmi in mi.Owner.Items)
            {
                tsmi.Checked = false;
            }
            mi.Checked = true;

            return;
        }
    }
}
