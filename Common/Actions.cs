// Actions class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Actions : Controls
    /// </summary>
    public partial class Actions : Controls
    {
        bool isDiscardSelectedIndexChange = false;

        /// <summary>
        /// The starting point of the application
        /// </summary>
        [STAThreadAttribute]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            UpdateStatusLabel("Loading...");
            Data.Start();
            Instruments.LoadInstruments();
            Configs.LoadConfigs();
            Language.InitLanguages();
            LayoutColors.InitColorSchemes();
            Data.InitMarketStatistic();

            Data.InstrProperties = Instruments.InstrumentList[Data.Strategy.Symbol].Clone();

            Application.Run(new Actions());

            return;
        }

        /// <summary>
        /// The default constructor
        /// </summary>
        public Actions()
        {
            StartPosition     = FormStartPosition.CenterScreen;
            Size              = new Size(790, 590);
            MinimumSize       = new Size(500, 375);
            Icon              = Data.Icon;
            Text              = Data.ProgramName;
            FormClosing      += new FormClosingEventHandler(Actions_FormClosing);
            Application.Idle += new EventHandler(Application_Idle);

            // Load a data file
            UpdateStatusLabel("Loading historical data...");
            if (LoadInstrument(false) != 0)
            {
                LoadInstrument(true);
                string messageText = Language.T("Forex Strategy Builder cannot load a historical data file and is going to use integrated data!");
                MessageBox.Show(messageText, Language.T("Data File Loading"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            // Prepare custom indicators
            if (Configs.LoadCustomIndicators)
            {
                UpdateStatusLabel("Loading custom indicators...");
                Custom_Indicators.LoadCustomIndicators();
            }
            else
                Indicator_Store.CombineAllIndicators();

            // Load a strategy
            UpdateStatusLabel("Loading strategy...");
            string sStrategyPath = Data.StrategyPath;

            if (Configs.RememberLastStr && Configs.LastStrategy != "")
            {
                string sLastStrategy = Path.GetDirectoryName(Configs.LastStrategy);
                if (sLastStrategy != "")
                    sLastStrategy = Configs.LastStrategy;
                else
                {
                    string sPath = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);
                    sLastStrategy = Path.Combine(sPath, Configs.LastStrategy);
                }
                if (File.Exists(sLastStrategy))
                    sStrategyPath = sLastStrategy;
            }

            if (OpenStrategy(sStrategyPath) == 0)
            {
                AfterStrategyOpening(false);
            }

            Calculate(false);

            Check_Update liveContent = new Check_Update(Data.SystemDir, miLiveContent, miForex);

            Registrar registrar = new Registrar();
            registrar.Register();

            // Starting tips
            if (Configs.ShowStartingTip)
            {
                Starting_Tips startingTips = new Starting_Tips();
                startingTips.Show();
            }

            UpdateStatusLabel("Loading user interface...");

            return;
        }

        /// <summary>
        /// Application idle
        /// </summary>
        void Application_Idle(object sender, EventArgs e)
        {
            Application.Idle -= new EventHandler(Application_Idle);
            string sLockFile = GetLockFile();
            if (!string.IsNullOrEmpty(sLockFile))
                File.Delete(sLockFile);

            return;
        }

        /// <summary>
        /// Updates the splash screen label.
        /// </summary>
        static void UpdateStatusLabel(string comment)
        {
            try
            {
                TextWriter tw = new StreamWriter(GetLockFile(), false);
                tw.WriteLine(comment);
                tw.Close();
            }
            catch { }
        }

        /// <summary>
        /// The lockfile name will be passed automatically by Splash.exe as a
        /// command line arg -lockfile="c:\temp\C1679A85-A4FA-48a2-BF77-E74F73E08768.lock"
        /// </summary>
        /// <returns>Lock file path</returns>
        static string GetLockFile()
        {
            foreach (string arg in Environment.GetCommandLineArgs())
                if (arg.StartsWith("-lockfile="))
                    return arg.Replace("-lockfile=", String.Empty);

            return string.Empty;
        }

        /// <summary>
        /// Checks whether the strategy have been saved or not
        /// </summary>
        void Actions_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = WhetherSaveChangedStrategy();

            if (dialogResult == DialogResult.Yes)
                SaveStrategy();
            else if (dialogResult == DialogResult.Cancel)
                e.Cancel = true;

            // Remember the last used strategy
            if (Configs.RememberLastStr)
            {
                if (Data.LoadedSavedStrategy != "")
                {
                    string strategyPath = Path.GetDirectoryName(Data.LoadedSavedStrategy) + Path.DirectorySeparatorChar;
                    string defaultPath  = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);
                    if (strategyPath == defaultPath)
                        Data.LoadedSavedStrategy = Path.GetFileName(Data.LoadedSavedStrategy);
                }
                Configs.LastStrategy = Data.LoadedSavedStrategy;
            }

            Configs.SaveConfigs();
            Instruments.SaveInstruments();

            return;
        }

// ---------------------------------------------------------- //

        /// <summary>
        /// Edits the Strategy Properties Slot
        /// </summary>
        void EditStrategyProperties()
        {
            Data.IsStrategyReady = false;
            Data.StackStrategy.Push(Data.Strategy.Clone());

            Strategy_Properties strategyProperties = new Strategy_Properties();
            strategyProperties.ShowDialog();

            if (strategyProperties.DialogResult == DialogResult.OK)
            {
                this.Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
                Data.IsStrategyChanged = true;
                RebuildStrategyLayout();
                smallBalanceChart.InitChart();
                smallBalanceChart.Invalidate();
                SetupJournal();
                infpnlAccountStatistics.Update(Backtester.AccountStatsParam, Backtester.AccountStatsValue,
                                               Backtester.AccountStatsFlags, Language.T("Account Statistics"));
            }
            else
            {
                UndoStrategy();
            }

            Data.IsStrategyReady = true;

            return;
        }

        /// <summary>
        /// Edits the Strategy Slot
        /// </summary>
        void EditSlot(int slot)
        {
            Data.IsStrategyReady = false;

            SlotTypes slotType = Data.Strategy.Slot[slot].SlotType;
            bool isSlotExist = Data.Strategy.Slot[slot].IsDefined;
            if (isSlotExist)
                Data.StackStrategy.Push(Data.Strategy.Clone());

            Indicator_Dialog indicatorDialog = new Indicator_Dialog(slot, slotType, isSlotExist);
            indicatorDialog.ShowDialog();

            if (indicatorDialog.DialogResult == DialogResult.OK)
            {
                this.Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
                Data.IsStrategyChanged = true;
                smallIndicatorChart.InitChart();
                smallIndicatorChart.Invalidate();
                RebuildStrategyLayout();
                smallBalanceChart.InitChart();
                smallBalanceChart.Invalidate();
                SetupJournal();
                infpnlAccountStatistics.Update(Backtester.AccountStatsParam, Backtester.AccountStatsValue,
                                               Backtester.AccountStatsFlags, Language.T("Account Statistics"));
            }
            else
            {   // Cancel was pressed
                UndoStrategy();
            }

            Data.IsStrategyReady = true;

            return;
        }

        /// <summary>
        /// Moves a Slot Upwards
        /// </summary>
        void MoveSlotUpwards(int iSlotToMove)
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.MoveFilterUpwards(iSlotToMove);

            this.Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
            Data.IsStrategyChanged = true;
            RebuildStrategyLayout();
            Calculate(true);

            return;
        }

        /// <summary>
        /// Moves a Slot Downwards
        /// </summary>
        void MoveSlotDownwards(int iSlotToMove)
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.MoveFilterDownwards(iSlotToMove);

            this.Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
            Data.IsStrategyChanged = true;
            RebuildStrategyLayout();
            Calculate(true);

            return;
        }

        /// <summary>
        /// Duplicates a Slot
        /// </summary>
        void DuplicateSlot(int iSlotToDuplicate)
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.DuplicateFilter(iSlotToDuplicate);

            this.Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
            Data.IsStrategyChanged = true;
            RebuildStrategyLayout();
            Calculate(true);

            return;
        }

        /// <summary>
        /// Adds a new Open filter
        /// </summary>
        void AddOpenFilter()
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.AddOpenFilter();
            EditSlot(Data.Strategy.OpenFilters);

            return;
        }

        /// <summary>
        /// Adds a new Close filter
        /// </summary>
        void AddCloseFilter()
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.AddCloseFilter();
            EditSlot(Data.Strategy.Slots - 1);

            return;
        }

        /// <summary>
        /// Removes a strategy slot.
        /// </summary>
        /// <param name="iSlot">Slot to remove</param>
        void RemoveSlot(int iSlot)
        {
            this.Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
            Data.IsStrategyChanged = true;

            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.RemoveFilter(iSlot);
            RebuildStrategyLayout();

            Calculate(false);
        }

        /// <summary>
        /// Undoes the strategy
        /// </summary>
        void UndoStrategy()
        {
            if (Data.StackStrategy.Count <= 1)
            {
                this.Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + " - " + Data.ProgramName;
                Data.IsStrategyChanged = false;
            }

            if (Data.StackStrategy.Count > 0)
            {
                Data.Strategy = Data.StackStrategy.Pop();

                RebuildStrategyLayout();
                Calculate(true);
            }

            return;
        }

        /// <summary>
        /// Performs actions when upbv has been chaged
        /// </summary>
        void UsePreviousBarValue_Change()
        {
            if (miStrategyAUPBV.Checked == false)
            {
                // Confirmation Message
                string sMessageText = Language.T("Are you sure you want to control \"Use previous bar value\" manually?");
                DialogResult dialogResult = MessageBox.Show(sMessageText, Language.T("Use previous bar value"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {   // OK, we are sure
                    Data.AutoUsePrvBarValue = false;

                    foreach (IndicatorSlot indicatorSlot in Data.Strategy.Slot)
                        foreach (CheckParam checkParam in indicatorSlot.IndParam.CheckParam)
                            if (checkParam.Caption == "Use previous bar value")
                                checkParam.Enabled = true;
                }
                else
                {   // Not just now
                    miStrategyAUPBV.Checked = true;
                }
            }
            else
            {
                Data.AutoUsePrvBarValue = true;
                Data.Strategy.AdjustUsePreviousBarValue();
                RepaintStrategyLayout();
                Calculate(true);
            }

            return;
        }

        /// <summary>
        /// Ask for saving the changed strategy
        /// </summary>
        DialogResult WhetherSaveChangedStrategy()
        {
            DialogResult dr = DialogResult.No;
            if (Data.IsStrategyChanged)
            {
                string sMessageText = Language.T("Do you want to save the current strategy?") + Environment.NewLine + Data.StrategyName;
                dr = MessageBox.Show(sMessageText, Data.ProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            }

            return dr;
        }

        /// <summary>
        /// LoadInstrument
        /// </summary>
        int LoadInstrument(bool useResource)
        {
            string      symbol;
            DataPeriods dataPeriod;

            Cursor = Cursors.WaitCursor;

            //  Takes the instrument symbol and period
            symbol    = tscbSymbol.Text;
            dataPeriod = (DataPeriods)Enum.GetValues(typeof(DataPeriods)).GetValue(tscbPeriod.SelectedIndex);
            Instrument_Properties instrProperties = Instruments.InstrumentList[symbol].Clone();

            //  Makes an instance of class Instrument
            Instrument instrument = new Instrument(instrProperties, (int)dataPeriod);

            instrument.DataDir      = Data.OfflineDataDir;
            instrument.FormatDate   = DateFormat.Unknown;
            instrument.MaxBars      = Configs.MaxBars;
            instrument.StartYear    = Configs.StartYear;
            instrument.StartMonth   = Configs.StartMonth;
            instrument.StartDay     = Configs.StartDay;
            instrument.EndYear      = Configs.EndYear;
            instrument.EndMonth     = Configs.EndMonth;
            instrument.EndDay       = Configs.EndDay;
            instrument.UseStartDate = Configs.UseStartDate;
            instrument.UseEndDate   = Configs.UseEndDate;

            // Loads the data
            int iLoadDataResult = 0;

            if (useResource)
                iLoadDataResult = instrument.LoadResourceData();
            else
                iLoadDataResult = instrument.LoadData();

            if (instrument.Bars > 0 && iLoadDataResult == 0)
            {
                Data.InstrProperties = instrProperties.Clone();

                Data.Bars   = instrument.Bars;
                Data.Period = dataPeriod;
                Data.Update = instrument.Update;

                Data.Time   = new DateTime[Data.Bars];
                Data.Open   = new double[Data.Bars];
                Data.High   = new double[Data.Bars];
                Data.Low    = new double[Data.Bars];
                Data.Close  = new double[Data.Bars];
                Data.Volume = new int[Data.Bars];

                for (int bar = 0; bar < Data.Bars; bar++)
                {
                    Data.Open[bar]   = instrument.Open(bar);
                    Data.High[bar]   = instrument.High(bar);
                    Data.Low[bar]    = instrument.Low(bar);
                    Data.Close[bar]  = instrument.Close(bar);
                    Data.Time[bar]   = instrument.Time(bar);
                    Data.Volume[bar] = instrument.Volume(bar);
                }

                Data.MinPrice         = instrument.MinPrice;
                Data.MaxPrice         = instrument.MaxPrice;
                Data.DaysOff          = instrument.DaysOff;
                Data.AverageGap       = instrument.AverageGap;
                Data.MaxGap           = instrument.MaxGap;
                Data.AverageHighLow   = instrument.AverageHighLow;
                Data.MaxHighLow       = instrument.MaxHighLow;
                Data.AverageCloseOpen = instrument.AverageCloseOpen;
                Data.MaxCloseOpen     = instrument.MaxCloseOpen;
                Data.DataCut          = instrument.Cut;
                Data.IsIntrabarData   = false;
                Data.IsTickData       = false;
                Data.IsData           = true;
                Data.IsResult         = false;

                // Configs.SetAccountExchangeRate();

                CheckLoadedData();
                Data.GenerateMarketStats();
                infpnlMarketStatistics.Update(Data.MarketStatsParam, Data.MarketStatsValue,
                                              Data.MarketStatsFlag, Language.T("Market Statistics"));
                infpnlAccountStatistics.Update(Backtester.AccountStatsParam, Backtester.AccountStatsValue,
                                               Backtester.AccountStatsFlags, Language.T("Account Statistics"));
            }
            else if (iLoadDataResult == -1)
            {
                MessageBox.Show(Language.T("Error in the data file!"), Language.T("Data file loading"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor = Cursors.Default;
                return 1;
            }
            else
            {
                MessageBox.Show(Language.T("There is no data for") + " " + symbol + " " + Data.DataPeriodToString(dataPeriod) + " " +
                                Language.T("in folder") + " " + Data.OfflineDataDir + Environment.NewLine + Environment.NewLine +
                                Language.T("Check the offline data directory path (Menu Market -> Data Directory)"),
                                Language.T("Data File Loading"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Cursor = Cursors.Default;
                return 1;
            }

            Cursor = Cursors.Default;

            return 0;
        }

        /// <summary>
        /// Checks the loaded data
        /// </summary>
        void CheckLoadedData()
        {
            SetInstrumentDataStatusBar();

            if (!Configs.CheckData)
                return;

            string errorMessage = "";

            // Check for defective data
            int maxConsecutiveBars = 0;
            int maxConsecutiveBar  = 0;
            int consecutiveBars    = 0;
            int lastBar            = 0;
            for (int iBar = 0; iBar < Data.Bars; iBar++)
            {
                if (Data.Open[iBar] == Data.Close[iBar])
                {
                    if (lastBar == iBar - 1 || lastBar == 0)
                    {
                        consecutiveBars++;
                        lastBar = iBar;

                        if (consecutiveBars > maxConsecutiveBars)
                        {
                            maxConsecutiveBars = consecutiveBars;
                            maxConsecutiveBar = iBar;
                        }
                    }
                }
                else
                {
                    consecutiveBars = 0;
                }
            }

            if (maxConsecutiveBars > 10)
            {
                errorMessage += Language.T("Defective till bar number:") + " " + (maxConsecutiveBar + 1) + " - " +
                                 Data.Time[maxConsecutiveBar].ToString() + Environment.NewLine +
                                 Language.T("You can try to cut it using \"Data Horizon\".") + Environment.NewLine +
                                 Language.T("You can try also \"Cut Off Bad Data\".");
            }

            if (Data.Bars < 300)
            {
                errorMessage += Language.T("Contains less than 300 bars!") + Environment.NewLine +
                                 Language.T("Check your data file or the limits in \"Data Horizon\".");
            }

            if (Data.DaysOff > 5 && Data.Period != DataPeriods.week)
            {
                errorMessage += Language.T("Maximum days off") + " " + Data.DaysOff + Environment.NewLine +
                                 Language.T("The data is probably incomplete!") + Environment.NewLine +
                                 Language.T("You can try also \"Cut Off Bad Data\".");
            }

            if (errorMessage != "")
            {
                errorMessage = Language.T("Market") + " " + Data.Symbol + " " + Data.DataPeriodToString(Data.Period) + Environment.NewLine + errorMessage;
                MessageBox.Show(errorMessage, Language.T("Data File Loading"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            return;
        }

        /// <summary>
        /// Open a strategy file
        /// </summary>
        void OpenFile()
        {
            DialogResult dialogResult = WhetherSaveChangedStrategy();

            if (dialogResult == DialogResult.Yes)
                SaveStrategy();
            else if (dialogResult == DialogResult.Cancel)
                return;

            OpenFileDialog opendlg = new OpenFileDialog();

            opendlg.InitialDirectory = Data.StrategyDir;
            opendlg.Filter = Language.T("Strategy file") + " (*.xml)|*.xml";
            opendlg.Title  = Language.T("Open Strategy");

            if (opendlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    OpenStrategy(opendlg.FileName);
                    AfterStrategyOpening(true);
                    Calculate(false);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, Text);
                    return;
                }
            }

            return;
        }

        /// <summary>
        /// New Strategy
        /// </summary>
        void NewStrategy()
        {
            DialogResult dialogResult = WhetherSaveChangedStrategy();

            if (dialogResult == DialogResult.Yes)
                SaveStrategy();
            else if (dialogResult == DialogResult.Cancel)
                return;

            Data.StrategyDir = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);

            if (OpenStrategy(Path.Combine(Data.StrategyDir, "New.xml")) == 0)
            {
                AfterStrategyOpening(false);
                Calculate(false);
            }

            return;
        }

        /// <summary>
        ///Reloads the Custom Indicators.
        /// </summary>
        void ReloadCustomIndicators()
        {
            // Check if the strategy contains custom indicators
            bool strategyHasCustomIndicator = false;
            foreach (IndicatorSlot slot in Data.Strategy.Slot)
            {   // Searching the strategy slots for a custom indicator
                if (Indicator_Store.CustomIndicatorNames.Contains(slot.IndicatorName))
                {
                    strategyHasCustomIndicator = true;
                    break;
                }
            }

            if (strategyHasCustomIndicator)
            {   // Save the current strategy
                DialogResult dialogResult = WhetherSaveChangedStrategy();

                if (dialogResult == DialogResult.Yes)
                    SaveStrategy();
                else if (dialogResult == DialogResult.Cancel)
                    return;
            }

            // Reload all the custom indicators
            Custom_Indicators.LoadCustomIndicators();

            if (strategyHasCustomIndicator)
            {   // Load and calculate a new strategy
                Data.StrategyDir = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);

                if (OpenStrategy(Path.Combine(Data.StrategyDir, "New.xml")) == 0)
                {
                    AfterStrategyOpening(false);
                    Calculate(false);
                }
            }

            return;
        }

        /// <summary>
        /// Reads the strategy from a file.
        /// </summary>
        /// <param name="strategyName">The strategy name.</param>
        /// <returns>0 - success.</returns>
        int OpenStrategy(string strategyName)
        {
            try
            {
                if (File.Exists(strategyName) && Strategy.Load(strategyName))
                {   // Successfully opening
                    Data.Strategy.StrategyName = Path.GetFileNameWithoutExtension(strategyName);
                    Data.StrategyDir  = Path.GetDirectoryName(strategyName);
                    Data.StrategyName = Path.GetFileName(strategyName);
                }
                else
                {
                    Strategy.GenerateNew();
                    string sMessageText = Language.T("The strategy could not be loaded correctly!");
                    MessageBox.Show(sMessageText, Language.T("Strategy Loading"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Data.LoadedSavedStrategy = "";
                    this.Text = Data.ProgramName;
                }

                Data.SetStrategyIndicators();
                RebuildStrategyLayout();

                this.Text = Data.Strategy.StrategyName + " - " + Data.ProgramName;
                Data.IsStrategyChanged    = false;
                Data.LoadedSavedStrategy = Data.StrategyPath;

                Data.StackStrategy.Clear();
            }
            catch
            {
                Strategy.GenerateNew();
                string sMessageText = Language.T("The strategy could not be loaded correctly!");
                MessageBox.Show(sMessageText, Language.T("Strategy Loading"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Data.LoadedSavedStrategy = "";
                this.Text = Data.ProgramName;
                RebuildStrategyLayout();
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Save the current strategy
        /// </summary>
        int SaveStrategy()
        {
            if (Data.StrategyName == "New.xml")
            {
                SaveAsStrategy();
            }
            else
            {
                try
                {
                    Data.Strategy.Save(Data.StrategyPath);
                    this.Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + " - " + Data.ProgramName;
                    Data.IsStrategyChanged = false;
                    Data.LoadedSavedStrategy = Data.StrategyPath;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, Text);
                    return -1;
                }
            }

            return 0;
        }

        /// <summary>
        /// Save the current strategy
        /// </summary>
        void SaveAsStrategy()
        {
            //Creates a dialog form SaveFileDialog
            SaveFileDialog savedlg = new SaveFileDialog();

            savedlg.InitialDirectory = Data.StrategyDir;
            savedlg.FileName         = Path.GetFileName(Data.StrategyName);
            savedlg.AddExtension     = true;
            savedlg.Title            = Language.T("Save the Strategy As");
            savedlg.Filter           = Language.T("Strategy file") + " (*.xml)|*.xml";

            if (savedlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Data.StrategyName = Path.GetFileName(savedlg.FileName);
                    Data.StrategyDir  = Path.GetDirectoryName(savedlg.FileName);
                    Data.Strategy.Save(savedlg.FileName);
                    this.Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + " - " + Data.ProgramName;
                    Data.IsStrategyChanged = false;
                    Data.LoadedSavedStrategy = Data.StrategyPath;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, Text);
                    return;
                }
            }

            return;
        }

        /// <summary>
        /// Calculates the strategy.
        /// </summary>
        /// <param name="recalcIndicators">true - to recalculate all the indicators.</param>
        void Calculate(bool recalcIndicators)
        {
            bool isUPBVChanged = Data.Strategy.AdjustUsePreviousBarValue();

            // Calculates the indicators by slots if it's necessary
            if (recalcIndicators)
                foreach(IndicatorSlot indSlot in Data.Strategy.Slot)
                {
                    string    indicatorName = indSlot.IndicatorName;
                    SlotTypes slotType      = indSlot.SlotType;
                    Indicator indicator     = Indicator_Store.ConstructIndicator(indicatorName, slotType);

                    indicator.IndParam = indSlot.IndParam;

                    indicator.Calculate(slotType);

                    indSlot.IndicatorName  = indicator.IndicatorName;
                    indSlot.IndParam       = indicator.IndParam;
                    indSlot.Component      = indicator.Component;
                    indSlot.SeparatedChart = indicator.SeparatedChart;
                    indSlot.SpecValue      = indicator.SpecialValues;
                    indSlot.MinValue       = indicator.SeparatedChartMinValue;
                    indSlot.MaxValue       = indicator.SeparatedChartMaxValue;
                    indSlot.IsDefined      = true;
                }

            // Searches the indicators' components to determine the Data.FirstBar
            Data.FirstBar = Data.Strategy.SetFirstBar();

            // Logging
            Data.Log("Calculate the strategy");

            // Calculates the Backtest
            Backtester.Calculate();
            Backtester.CalculateAccountStats();

            Data.IsResult = true;
            if (isUPBVChanged) RebuildStrategyLayout();
            smallIndicatorChart.InitChart();
            smallIndicatorChart.Invalidate();
            smallBalanceChart.InitChart();
            smallBalanceChart.Invalidate();
            SetupJournal();
            infpnlAccountStatistics.Update(
                Backtester.AccountStatsParam,
                Backtester.AccountStatsValue,
                Backtester.AccountStatsFlags,
                Language.T("Account Statistics"));

            return;
        }

        /// <summary>
        /// Sets the market according the strategy
        /// </summary>
        void SetMarket(string symbol, DataPeriods dataPeriod)
        {
            Data.InstrProperties = Instruments.InstrumentList[symbol].Clone();
            Data.Period = dataPeriod;

            isDiscardSelectedIndexChange = true;
            tscbSymbol.SelectedIndex = tscbSymbol.Items.IndexOf(symbol);

            switch (dataPeriod)
            {
                case DataPeriods.min1:
                    tscbPeriod.SelectedIndex = 0;
                    break;
                case DataPeriods.min5:
                    tscbPeriod.SelectedIndex = 1;
                    break;
                case DataPeriods.min15:
                    tscbPeriod.SelectedIndex = 2;
                    break;
                case DataPeriods.min30:
                    tscbPeriod.SelectedIndex = 3;
                    break;
                case DataPeriods.hour1:
                    tscbPeriod.SelectedIndex = 4;
                    break;
                case DataPeriods.hour4:
                    tscbPeriod.SelectedIndex = 5;
                    break;
                case DataPeriods.day:
                    tscbPeriod.SelectedIndex = 6;
                    break;
                case DataPeriods.week:
                    tscbPeriod.SelectedIndex = 7;
                    break;
                default:
                    break;
            }

            isDiscardSelectedIndexChange = false;

            return;
        }

        /// <summary>
        /// Edit the Trading Charges
        /// </summary>
        void EditTradingCharges()
        {
            Trading_Charges tradingCharges = new Trading_Charges();
            tradingCharges.Spread      = Data.InstrProperties.Spread;
            tradingCharges.SwapLong    = Data.InstrProperties.SwapLong;
            tradingCharges.SwapShort   = Data.InstrProperties.SwapShort;
            tradingCharges.Commission  = Data.InstrProperties.Commission;
            tradingCharges.Slippage    = Data.InstrProperties.Slippage;
            tradingCharges.ShowDialog();

            if (tradingCharges.DialogResult == DialogResult.OK)
            {
                Data.InstrProperties.Spread     = tradingCharges.Spread;
                Data.InstrProperties.SwapLong   = tradingCharges.SwapLong;
                Data.InstrProperties.SwapShort  = tradingCharges.SwapShort;
                Data.InstrProperties.Commission = tradingCharges.Commission;
                Data.InstrProperties.Slippage   = tradingCharges.Slippage;

                Instruments.InstrumentList[Data.InstrProperties.Symbol] = Data.InstrProperties.Clone();

                Calculate(false);

                SetInstrumentDataStatusBar();
            }
            else if (tradingCharges.EditInstrument)
                ShowInstrumentEditor();

            return;
        }

        /// <summary>
        /// Check the needed market conditions
        /// </summary>
        /// <param name="isMessage">To show the mesage or not</param>
        void AfterStrategyOpening(bool isMessage)
        {
            if (Data.Strategy.Symbol != Data.Symbol || Data.Strategy.DataPeriod != Data.Period)
            {
                bool toReload = true;

                if (isMessage)
                {
                    DialogResult result;

                    result = MessageBox.Show(
                        Language.T("The loaded strategy has been designed for a different market!") +
                        Environment.NewLine + Environment.NewLine +
                        Data.Strategy.Symbol + " " + Data.DataPeriodToString(Data.Strategy.DataPeriod) +
                        Environment.NewLine + Environment.NewLine +
                        Language.T("Do you want to load this market data?"),
                        Data.Strategy.StrategyName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    toReload = (result == DialogResult.Yes);
                }

                if (toReload)
                {
                    if (!Instruments.InstrumentList.ContainsKey(Data.Strategy.Symbol))
                    {
                        MessageBox.Show(
                            Language.T("There is no information for this market!") +
                            Environment.NewLine + Environment.NewLine +
                            Data.Strategy.Symbol + " " + Data.DataPeriodToString(Data.Strategy.DataPeriod),
                            Data.Strategy.StrategyName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        return;
                    }

                    string      symbol     = Data.Symbol;
                    DataPeriods dataPeriod = Data.Period;

                    SetMarket(Data.Strategy.Symbol, Data.Strategy.DataPeriod);

                    if (LoadInstrument(false) == 0)
                    {
                        Calculate(true);
                        PrepareScannerCompactMode();
                    }
                    else
                    {
                        SetMarket(symbol, dataPeriod);
                    }
                }
            }
            else if (!Data.IsIntrabarData)
            {
                PrepareScannerCompactMode();
            }

            return;
        }

        /// <summary>
        /// Load intrabar data by using scanner.
        /// </summary>
        void PrepareScannerCompactMode()
        {
            if (Configs.Autoscan && (Data.Period != DataPeriods.min1 || Configs.UseTickData))
            {
                tscbSymbol.Enabled = false;
                tscbPeriod.Enabled = false;

                BackgroundWorker bgWorker = new BackgroundWorker();
                bgWorker.DoWork += new DoWorkEventHandler(LoadIntrabarData);
                bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(IntrabarDataLoaded);
                bgWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Starts scanner and loads intrabar data.
        /// </summary>
        void LoadIntrabarData(object sender, DoWorkEventArgs e)
        {

            Scanner scanner = new Scanner();
            scanner.CompactMode = true;
            scanner.ShowDialog();

            return;
        }

        /// <summary>
        /// The intrabar data is loaded. Refresh the program.
        /// </summary>
        void IntrabarDataLoaded(object sender, RunWorkerCompletedEventArgs e)
        {
            Calculate(true);

            tscbSymbol.Enabled = true;
            tscbPeriod.Enabled = true;

            return;
        }

        /// <summary>
        /// Generate BBCode for the forum
        /// </summary>
        void PublishStrategy()
        {
            Strategy_Publish publisher = new Strategy_Publish();
            publisher.Show();
        }

        /// <summary>
        /// Shows the Account Settings dialog.
        /// </summary>
        void ShowAccountSettings()
        {
            Account_Settings accountSettings = new Account_Settings();

            accountSettings.AccountCurrency = Configs.AccountCurrency;
            accountSettings.InitialAccount  = Configs.InitialAccount;
            accountSettings.Leverage        = Configs.Leverage;
            accountSettings.RateToUSD       = Data.InstrProperties.RateToUSD;
            accountSettings.RateToEUR       = Data.InstrProperties.RateToEUR;

            accountSettings.SetParams();

            if (accountSettings.ShowDialog() == DialogResult.OK)
            {
                Configs.AccountCurrency = accountSettings.AccountCurrency;
                Configs.InitialAccount  = accountSettings.InitialAccount;
                Configs.Leverage        = accountSettings.Leverage;

                Data.InstrProperties.RateToUSD = accountSettings.RateToUSD;
                Data.InstrProperties.RateToEUR = accountSettings.RateToEUR;

                Instruments.InstrumentList[Data.InstrProperties.Symbol] = Data.InstrProperties.Clone();

                Calculate(false);
            }

            return;
        }

        /// <summary>
        ///  Shows the scanner
        /// </summary>
        void ShowScanner()
        {
            Scanner scanner = new Scanner();
            scanner.ShowDialog();

            miStrategyAutoscan.Checked = Configs.Autoscan;

            infpnlAccountStatistics.Update(Backtester.AccountStatsParam, Backtester.AccountStatsValue,
                                           Backtester.AccountStatsFlags, Language.T("Account Statistics"));
            smallBalanceChart.InitChart();
            smallBalanceChart.Invalidate();
            SetupJournal();
        }

        /// <summary>
        /// Perform intrabar scanning
        /// </summary>
        void Scan()
        {
            if (!Data.IsIntrabarData)
                ShowScanner();
            else
                Backtester.Scan();

            infpnlAccountStatistics.Update(Backtester.AccountStatsParam, Backtester.AccountStatsValue,
                                           Backtester.AccountStatsFlags, Language.T("Account Statistics"));
            smallBalanceChart.InitChart();
            smallBalanceChart.Invalidate();
            SetupJournal();
        }

        /// <summary>
        ///  Starts the generator
        /// </summary>
        void ShowGenerator()
        {
            // Put the Strategy into the Undo Stack
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.IsStrategyReady = false;

            string sOrginalDescription = Data.Strategy.Description;

            Forex_Strategy_Builder.Dialogs.Generator.Generator generator = new Forex_Strategy_Builder.Dialogs.Generator.Generator();
            generator.SetParrentForm = this;
            generator.ShowDialog();

            if (generator.DialogResult == DialogResult.OK)
            {   // We accept the generated strategy
                Data.StrategyName = Data.Strategy.StrategyName + ".xml";
                Text = Data.Strategy.StrategyName + "* - " + Data.ProgramName;

                if (generator.IsStrategyModified)
                {
                    Data.Strategy.Description = (sOrginalDescription != string.Empty ?
                        sOrginalDescription + Environment.NewLine + Environment.NewLine +
                        "-----------" + Environment.NewLine + generator.GeneratedDescription :
                        generator.GeneratedDescription);
                }
                else
                {
                    Data.SetStrategyIndicators();
                    Data.Strategy.Description = generator.GeneratedDescription;
                }
                Data.IsStrategyChanged = true;
                RebuildStrategyLayout();
                Calculate(true);
            }
            else
            {   // When we cancel the Generating, we return the original strategy.
                UndoStrategy();
            }
            Data.IsStrategyReady = true;

            return;
        }

        /// <summary>
        ///  Starts the generator
        /// </summary>
        void ShowOverview()
        {
            Browser so = new Browser(Language.T("Strategy Overview"), Data.Strategy.GenerateHTMLOverview());
            so.Show();

            return;
        }

        /// <summary>
        /// Call the Optimizer
        /// </summary>
        void ShowOptimizer()
        {
            // Put the Strategy into the Undo Stack
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.IsStrategyReady = false;

            Optimizer optimizer = new Optimizer();
            optimizer.SetParrentForm = this;
            optimizer.ShowDialog();

            if (optimizer.DialogResult == DialogResult.OK)
            {   // We accept the optimized strategy
                this.Text = Path.GetFileNameWithoutExtension(Data.StrategyName) + "* - " + Data.ProgramName;
                Data.IsStrategyChanged = true;
                RepaintStrategyLayout();
                Calculate(true);
            }
            else
            {   // When we cancel the optimizing, we return the original strategy.
                UndoStrategy();
            }
            Data.IsStrategyReady = true;
        }

        /// <summary>
        ///  Show the method Comparator
        /// </summary>
        int ShowComparator()
        {
            // Save the original method to return it later
            InterpolationMethod interpMethodOriginal = Backtester.InterpolationMethod;

            Comparator mc = new Comparator();
            mc.ShowDialog();

            // Returns the original method
            Backtester.InterpolationMethod = interpMethodOriginal;
            Calculate(false);

            return 0;
        }

        /// <summary>
        ///  Shows the Bar Explorer tool.
        /// </summary>
        void ShowBarExplorer()
        {
            Bar_Explorer barExplorer = new Bar_Explorer(Data.FirstBar);
            barExplorer.ShowDialog();

            return;
        }

        /// <summary>
        ///  Sets the data starting parameters.
        /// </summary>
        void DataHorizon()
        {
            DateTime dtStart = new DateTime(Configs.StartYear, Configs.StartMonth, Configs.StartDay);
            DateTime dtEnd   = new DateTime(Configs.EndYear,   Configs.EndMonth,   Configs.EndDay);

            Data_Horizon horizon = new Data_Horizon(Configs.MaxBars, dtStart, dtEnd, Configs.UseStartDate, Configs.UseEndDate);
            horizon.ShowDialog();

            if (horizon.DialogResult == DialogResult.OK)
            {   // Applying the new settings
                Configs.MaxBars      = horizon.MaxBars;
                Configs.StartYear    = horizon.StartDate.Year;
                Configs.StartMonth   = horizon.StartDate.Month;
                Configs.StartDay     = horizon.StartDate.Day;
                Configs.EndYear      = horizon.EndDate.Year;
                Configs.EndMonth     = horizon.EndDate.Month;
                Configs.EndDay       = horizon.EndDate.Day;
                Configs.UseStartDate = horizon.UseStartDate;
                Configs.UseEndDate   = horizon.UseEndDate;

                if (LoadInstrument(false) == 0)
                {
                    Calculate(true);
                    PrepareScannerCompactMode();
                }
            }

            return;
        }

        /// <summary>
        ///  Shows the Instrument Editor dialog.
        /// </summary>
        void ShowInstrumentEditor()
        {
            Instrument_Editor instrEditor = new Instrument_Editor();
            instrEditor.ShowDialog();

            if (instrEditor.NeedReset)
            {
                isDiscardSelectedIndexChange = true;

                tscbSymbol.Items.Clear();
                tscbSymbol.Items.AddRange(Instruments.SymbolList);
                tscbSymbol.SelectedIndex = tscbSymbol.Items.IndexOf(Data.Symbol);

                isDiscardSelectedIndexChange = false;
            }

            Data.InstrProperties = Instruments.InstrumentList[Data.InstrProperties.Symbol].Clone();
            SetInstrumentDataStatusBar();
            Calculate(false);

            return;
        }

        /// <summary>
        /// Loads a color scheme.
        /// </summary>
        void LoadColorScheme()
        {
            string colorFile = Path.Combine(Data.ColorDir, Configs.ColorScheme + ".xml");

            if (File.Exists(colorFile))
            {
                LayoutColors.LoadColorScheme(colorFile);

                pnlWorkspace.BackColor = LayoutColors.ColorFormBack;
                infpnlAccountStatistics.SetColors();
                infpnlMarketStatistics.SetColors();
                smallIndicatorChart.InitChart();
                smallBalanceChart.InitChart();
                SetupJournal();
                pnlWorkspace.Invalidate(true);
            }

            return;
        }

        /// <summary>
        /// Sets the Status Bar Data Label
        /// </summary>
        void SetInstrumentDataStatusBar()
        {
            string swapUnit = "p";
            if (Data.InstrProperties.SwapType == Commission_Type.money)
                swapUnit = "m";
            else if (Data.InstrProperties.SwapType == Commission_Type.percents)
                swapUnit = "%";

            string commUnit = "p";
            if (Data.InstrProperties.CommissionType == Commission_Type.money)
                commUnit = "m";
            else if (Data.InstrProperties.CommissionType == Commission_Type.percents)
                commUnit = "%";

            ToolStripStatusLabelInstrument =
                Data.Symbol    + " "  +
                Data.PeriodString + " (" +
                Data.InstrProperties.Spread     + ", " +
                Data.InstrProperties.SwapLong.ToString("F2")   + swapUnit + ", " +
                Data.InstrProperties.SwapShort.ToString("F2")  + swapUnit + ", " +
                Data.InstrProperties.Commission.ToString("F2") + commUnit + ", " +
                Data.InstrProperties.Slippage + ")" +
                (Data.DataCut ? " - " + Language.T("Cut") : "") +
                (Configs.FillInDataGaps ?  " - " + Language.T("No Gaps") : "") +
                (Configs.CheckData  ? "" : " - " + Language.T("Unchecked"));

            return;
        }
    }
}
