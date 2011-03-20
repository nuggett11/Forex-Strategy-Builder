// Menu and StatusBar
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// Part of Forex Strategy Builder
// Website http://forexsb.com
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Provides the StatusBar and MainMenu
    /// </summary>
    public class Menu_and_StatusBar : Workspace
    {
        ToolStripStatusLabel sbpInstrument;
        ToolStripStatusLabel sbpChartInfo;
        ToolStripStatusLabel sbpDate;
        ToolStripStatusLabel sbpTime;
        protected ToolStripMenuItem miJournalByBars;
        protected ToolStripMenuItem miJournalByPos;
        protected ToolStripMenuItem miAccountShowInMoney;
        protected ToolStripMenuItem miAccountShowInPips;
        protected ToolStripMenuItem miStrategyAUPBV;
        protected ToolStripMenuItem miForex;
        protected ToolStripMenuItem miLiveContent;
        protected ToolStripMenuItem miStrategyAutoscan;

        /// <summary>
        /// Gets or sets the instrument info on the status bar
        /// </summary>
        protected string ToolStripStatusLabelInstrument
        {
            get { return sbpInstrument.Text; }
            set { sbpInstrument.Text = value; }
        }

        /// <summary>
        /// Gets or sets the dynamic info for the instrument chart on the status bar
        /// </summary>
        protected string ToolStripStatusLabelChartInfo
        {
            get { return sbpChartInfo.Text; }
            set { sbpChartInfo.Text = value; }
        }

        /// <summary>
        /// The default constructor
        /// </summary>
        public Menu_and_StatusBar()
        {
            InitializeMenu();
            InitializeStatusBar();
        }

        /// <summary>
        /// Sets the Main Menu.
        /// </summary>
        protected void InitializeMenu()
        {
            // File
            ToolStripMenuItem miFile = new ToolStripMenuItem(Language.T("File"));

            ToolStripMenuItem miNew = new ToolStripMenuItem();
            miNew.Text         = Language.T("New");
            miNew.Image        = Properties.Resources.new_startegy;
            miNew.ShortcutKeys = Keys.Control | Keys.N;
            miNew.ToolTipText  = Language.T("Open the default strategy \"New.xml\".");
            miNew.Click       += new EventHandler(MenuStrategyNew_OnClick);
            miFile.DropDownItems.Add(miNew);

            ToolStripMenuItem miOpen = new ToolStripMenuItem();
            miOpen.Text         = Language.T("Open...");
            miOpen.Image        = Properties.Resources.open;
            miOpen.ShortcutKeys = Keys.Control | Keys.O;
            miOpen.ToolTipText  = Language.T("Open a strategy.");
            miOpen.Click       += new EventHandler(MenuFileOpen_OnClick);
            miFile.DropDownItems.Add(miOpen);

            ToolStripMenuItem miSave = new ToolStripMenuItem();
            miSave.Text         = Language.T("Save");
            miSave.Image        = Properties.Resources.save;
            miSave.ShortcutKeys = Keys.Control | Keys.S;
            miSave.ToolTipText  = Language.T("Save the strategy.");
            miSave.Click       += new EventHandler(MenuFileSave_OnClick);
            miFile.DropDownItems.Add(miSave);

            ToolStripMenuItem miSaveAs = new ToolStripMenuItem();
            miSaveAs.Text        = Language.T("Save As") + "...";
            miSaveAs.Image       = Properties.Resources.save_as;
            miSaveAs.ToolTipText = Language.T("Save a copy of the strategy.");
            miSaveAs.Click      += new EventHandler(MenuFileSaveAs_OnClick);
            miFile.DropDownItems.Add(miSaveAs);

            miFile.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miClose = new ToolStripMenuItem();
            miClose.Text         = Language.T("Exit");
            miClose.Image        = Properties.Resources.exit;
            miClose.ToolTipText  = Language.T("Close the program.");
            miClose.ShortcutKeys = Keys.Control | Keys.X;
            miClose.Click       += new EventHandler(MenuFileCloseOnClick);
            miFile.DropDownItems.Add(miClose);

            // Edit
            ToolStripMenuItem miEdit = new ToolStripMenuItem(Language.T("Edit"));

            ToolStripMenuItem miStrategyUndo = new ToolStripMenuItem();
            miStrategyUndo.Text         = Language.T("Undo");
            miStrategyUndo.Image        = Properties.Resources.undo;
            miStrategyUndo.ToolTipText  = Language.T("Undo the last change in the strategy.");
            miStrategyUndo.ShortcutKeys = Keys.Control | Keys.Z;
            miStrategyUndo.Click       += new EventHandler(MenuStrategyUndo_OnClick);
            miEdit.DropDownItems.Add(miStrategyUndo);

            miEdit.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miPrevGenHistory = new ToolStripMenuItem();
            miPrevGenHistory.Text         = Language.T("Previous Generated Strategy");
            miPrevGenHistory.Image        = Properties.Resources.prev_gen;
            miPrevGenHistory.ShortcutKeys = Keys.Control | Keys.H;
            miPrevGenHistory.Click       += new EventHandler(MenuPrevHistory_OnClick);
            miEdit.DropDownItems.Add(miPrevGenHistory);

            ToolStripMenuItem miNextGenHistory = new ToolStripMenuItem();
            miNextGenHistory.Text         = Language.T("Next Generated Strategy");
            miNextGenHistory.Image        = Properties.Resources.next_gen;
            miNextGenHistory.ShortcutKeys = Keys.Control | Keys.J;
            miNextGenHistory.Click       += new EventHandler(MenuNextHistory_OnClick);
            miEdit.DropDownItems.Add(miNextGenHistory);

            //View
            ToolStripMenuItem miView = new ToolStripMenuItem(Language.T("View"));

            ToolStripMenuItem miLanguage = new ToolStripMenuItem();
            miLanguage.Text = "Language";
            miLanguage.Image = Properties.Resources.lang;
            for (int i = 0; i < Language.LanguageList.Length; i++)
            {
                ToolStripMenuItem miLang = new ToolStripMenuItem();
                miLang.Text    = Language.LanguageList[i];
                miLang.Name    = Language.LanguageList[i];
                miLang.Checked = miLang.Name == Configs.Language;
                miLang.Click  += new EventHandler(Language_Click);
                miLanguage.DropDownItems.Add(miLang);
            }

            miView.DropDownItems.Add(miLanguage);

            ToolStripMenuItem miLanguageTools = new ToolStripMenuItem();
            miLanguageTools.Text  = Language.T("Language Tools");
            miLanguageTools.Image = Properties.Resources.lang_tools;

            ToolStripMenuItem miNewTranslation = new ToolStripMenuItem();
            miNewTranslation.Name   = "miNewTranslation";
            miNewTranslation.Text   = Language.T("Make New Translation") + "...";
            miNewTranslation.Image  = Properties.Resources.new_translation;
            miNewTranslation.Click += new EventHandler(MenuTools_OnClick);
            miLanguageTools.DropDownItems.Add(miNewTranslation);

            ToolStripMenuItem miEditTranslation = new ToolStripMenuItem();
            miEditTranslation.Name   = "miEditTranslation";
            miEditTranslation.Text   = Language.T("Edit Current Translation") + "...";
            miEditTranslation.Image  = Properties.Resources.edit_translation;
            miEditTranslation.Click += new EventHandler(MenuTools_OnClick);
            miLanguageTools.DropDownItems.Add(miEditTranslation);

            miLanguageTools.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miShowEnglishPhrases = new ToolStripMenuItem();
            miShowEnglishPhrases.Name   = "miShowEnglishPhrases";
            miShowEnglishPhrases.Text   = Language.T("Show English Phrases") + "...";
            miShowEnglishPhrases.Image  = Properties.Resources.view_translation;
            miShowEnglishPhrases.Click += new EventHandler(MenuTools_OnClick);
            miLanguageTools.DropDownItems.Add(miShowEnglishPhrases);

            ToolStripMenuItem miShowAltPhrases = new ToolStripMenuItem();
            miShowAltPhrases.Name   = "miShowAltPhrases";
            miShowAltPhrases.Text   = Language.T("Show Translated Phrases") + "...";
            miShowAltPhrases.Image  = Properties.Resources.view_translation;
            miShowAltPhrases.Click += new EventHandler(MenuTools_OnClick);
            miLanguageTools.DropDownItems.Add(miShowAltPhrases);

            ToolStripMenuItem miShowBothPhrases = new ToolStripMenuItem();
            miShowBothPhrases.Name   = "miShowAllPhrases";
            miShowBothPhrases.Text   = Language.T("Show All Phrases") + "...";
            miShowBothPhrases.Image  = Properties.Resources.view_translation;
            miShowBothPhrases.Click += new EventHandler(MenuTools_OnClick);
            miLanguageTools.DropDownItems.Add(miShowBothPhrases);

            miView.DropDownItems.Add(miLanguageTools);

            miView.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miShowPriceChart = new ToolStripMenuItem();
            miShowPriceChart.Text         = Language.T("Indicator Chart") + "...";
            miShowPriceChart.ToolTipText  = Language.T("Show the full Indicator Chart.");
            miShowPriceChart.ShortcutKeys = Keys.F2;
            miShowPriceChart.Image        = Properties.Resources.bar_chart;
            miShowPriceChart.Click       += new EventHandler(ShowPriceChart_OnClick);
            miView.DropDownItems.Add(miShowPriceChart);

            ToolStripMenuItem miShowAccountChart = new ToolStripMenuItem();
            miShowAccountChart.Text         = Language.T("Account Chart") + "...";
            miShowAccountChart.ToolTipText  = Language.T("Show the full Account Chart.");
            miShowAccountChart.Image        = Properties.Resources.balance_chart;
            miShowAccountChart.ShortcutKeys = Keys.F3;
            miShowAccountChart.Click       += new EventHandler(ShowAccountChart_OnClick);
            miView.DropDownItems.Add(miShowAccountChart);

            miView.DropDownItems.Add(new ToolStripSeparator());

            miJournalByPos = new ToolStripMenuItem();
            miJournalByPos.Name    = "miJournalByPos";
            miJournalByPos.Text    = Language.T("Journal by Positions");
            miJournalByPos.Checked = Configs.ShowJournal && !Configs.JournalByBars;
            miJournalByPos.Click  += new EventHandler(MenuJournal_OnClick);
            miView.DropDownItems.Add(miJournalByPos);

            miJournalByBars = new ToolStripMenuItem();
            miJournalByBars.Name    = "miJournalByBars";
            miJournalByBars.Text    = Language.T("Journal by Bars");
            miJournalByBars.Checked = Configs.ShowJournal && Configs.JournalByBars;
            miJournalByBars.Click  += new EventHandler(MenuJournal_OnClick);
            miView.DropDownItems.Add(miJournalByBars);

            miView.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miFullScreen = new ToolStripMenuItem();
            miFullScreen.Text         = Language.T("Full Screen");
            miFullScreen.Name         = "miFullScreen";
            miFullScreen.Checked      = false;
            miFullScreen.CheckOnClick = true;
            miFullScreen.ShortcutKeys = Keys.Alt | Keys.Enter;
            miFullScreen.Click       += new EventHandler(MenuViewFullScreen_OnClick);
            miView.DropDownItems.Add(miFullScreen);

            ToolStripMenuItem miLoadColor = new ToolStripMenuItem();
            miLoadColor.Text = Language.T("Color Scheme");
            miLoadColor.Image = Properties.Resources.palette;
            for (int i = 0; i < LayoutColors.ColorSchemeList.Length; i++)
            {
                ToolStripMenuItem miColor = new ToolStripMenuItem();
                miColor.Text    = LayoutColors.ColorSchemeList[i];
                miColor.Name    = LayoutColors.ColorSchemeList[i];
                miColor.Checked = miColor.Name == Configs.ColorScheme;
                miColor.Click  += new EventHandler(MenuLoadColor_OnClick);
                miLoadColor.DropDownItems.Add(miColor);
            }

            miView.DropDownItems.Add(miLoadColor);

            ToolStripMenuItem miGradientView = new ToolStripMenuItem();
            miGradientView.Text         = Language.T("Gradient View");
            miGradientView.Name         = "miGradientView";
            miGradientView.Checked      = Configs.GradientView;
            miGradientView.CheckOnClick = true;
            miGradientView.Click       += new EventHandler(MenuGradientView_OnClick);
            miView.DropDownItems.Add(miGradientView);

            // Account
            ToolStripMenuItem miAccount = new ToolStripMenuItem(Language.T("Account"));

            miAccountShowInMoney = new ToolStripMenuItem();
            miAccountShowInMoney.Name        = "miAccountShowInMoney";
            miAccountShowInMoney.Text        = Language.T("Information in Currency");
            miAccountShowInMoney.ToolTipText = Language.T("Display the account and the statistics in currency.");
            miAccountShowInMoney.Checked     = Configs.AccountInMoney;
            miAccountShowInMoney.Click      += new EventHandler(AccountShowInMoney_OnClick);
            miAccount.DropDownItems.Add(miAccountShowInMoney);

            miAccountShowInPips = new ToolStripMenuItem();
            miAccountShowInPips.Name        = "miAccountShowInPips";
            miAccountShowInPips.Text        = Language.T("Information in Pips");
            miAccountShowInPips.ToolTipText = Language.T("Display the account and the statistics in pips.");
            miAccountShowInPips.Checked     = !Configs.AccountInMoney;
            miAccountShowInPips.Click      += new EventHandler(AccountShowInMoney_OnClick);
            miAccount.DropDownItems.Add(miAccountShowInPips);

            miAccount.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miAccountSettings = new ToolStripMenuItem();
            miAccountSettings.Text        = Language.T("Account Settings") + "...";
            miAccountSettings.Image       = Properties.Resources.account_sett;
            miAccountSettings.ToolTipText = Language.T("Set the account parameters.");
            miAccountSettings.Click      += new EventHandler(MenuAccountSettings_OnClick);
            miAccount.DropDownItems.Add(miAccountSettings);

            // Market
            ToolStripMenuItem miMarket = new ToolStripMenuItem(Language.T("Market"));

            ToolStripMenuItem miReLoadData = new ToolStripMenuItem();
            miReLoadData.Text         = Language.T("Reload");
            miReLoadData.Image        = Properties.Resources.reload_data;
            miReLoadData.ToolTipText  = Language.T("Reload the market data.");
            miReLoadData.ShortcutKeys = Keys.Control | Keys.L;
            miReLoadData.Click       += new EventHandler(MenuLoadData_OnClick);
            miMarket.DropDownItems.Add(miReLoadData);

            miMarket.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miCharges = new ToolStripMenuItem();
            miCharges.Name        = "Charges";
            miCharges.Text        = Language.T("Charges") + "...";
            miCharges.ToolTipText = Language.T("Spread, Swap numbers, Slippage.");
            miCharges.Image       = Properties.Resources.charges;
            miCharges.Click      += new EventHandler(MenuTools_OnClick);
            miMarket.DropDownItems.Add(miCharges);

            ToolStripMenuItem miDataHorizon = new ToolStripMenuItem();
            miDataHorizon.Text        = Language.T("Data Horizon") + "...";
            miDataHorizon.Image       = Properties.Resources.data_horizon;
            miDataHorizon.ToolTipText = Language.T("Limit the number of data bars and the starting date.");
            miDataHorizon.Click      += new EventHandler(MenuDataHorizon_OnClick);
            miMarket.DropDownItems.Add(miDataHorizon);

            ToolStripMenuItem miDataDirectory = new ToolStripMenuItem();
            miDataDirectory.Text        = Language.T("Data Directory") + "...";
            miDataDirectory.Image       = Properties.Resources.data_directory;
            miDataDirectory.ToolTipText = Language.T("Change the current offline data directory.");
            miDataDirectory.Click      += new EventHandler(MenuDataDirectory_OnClick);
            miMarket.DropDownItems.Add(miDataDirectory);

            ToolStripMenuItem miInstrumentEditor = new ToolStripMenuItem();
            miInstrumentEditor.Name        = "miInstrumentEditor";
            miInstrumentEditor.Text        = Language.T("Edit Instruments") + "...";
            miInstrumentEditor.Image       = Properties.Resources.instr_edit;
            miInstrumentEditor.ToolTipText = Language.T("Add, edit, or delete instruments.");
            miInstrumentEditor.Click      += new EventHandler(MenuTools_OnClick);
            miMarket.DropDownItems.Add(miInstrumentEditor);

            miMarket.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miCheckData = new ToolStripMenuItem();
            miCheckData.Text         = Language.T("Check the Data");
            miCheckData.ToolTipText  = Language.T("Check the data during loading.");
            miCheckData.CheckOnClick = true;
            miCheckData.Checked      = Configs.CheckData;
            miCheckData.Click       += new EventHandler(MenuCheckData_OnClick);
            miMarket.DropDownItems.Add(miCheckData);

            ToolStripMenuItem miCutBadData = new ToolStripMenuItem();
            miCutBadData.Name         = "miCutBadData";
            miCutBadData.Text         = Language.T("Cut Off Bad Data");
            miCutBadData.CheckOnClick = true;
            miCutBadData.Checked      = Configs.CutBadData;
            miCutBadData.Click       += new EventHandler(MenuRefineData_OnClick);
            miMarket.DropDownItems.Add(miCutBadData);

            ToolStripMenuItem miFillDataGaps = new ToolStripMenuItem();
            miFillDataGaps.Name         = "miFillDataGaps";
            miFillDataGaps.Text         = Language.T("Fill In Data Gaps");
            miFillDataGaps.CheckOnClick = true;
            miFillDataGaps.Checked      = Configs.FillInDataGaps;
            miFillDataGaps.Click       += new EventHandler(MenuRefineData_OnClick);
            miMarket.DropDownItems.Add(miFillDataGaps);

            miMarket.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miDownload = new ToolStripMenuItem();
            miDownload.Text        = Language.T("Download Forex Rates") + "...";
            miDownload.Image       = Properties.Resources.download_data;
            miDownload.Tag         = "http://forexsb.com/wiki/fsb/rates";
            miDownload.ToolTipText = Language.T("Download historical data from the program's website.");
            miDownload.Click      += new EventHandler(MenuHelpContentsOnClick);
            miMarket.DropDownItems.Add(miDownload);

            miMarket.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miJForexImport = new ToolStripMenuItem();
            miJForexImport.Name   = "miJForexImport";
            miJForexImport.Text   = Language.T("Import JForex Data Files") + "...";
            miJForexImport.Image  = Properties.Resources.jforex;
            miJForexImport.Click += new EventHandler(MenuTools_OnClick);
            miMarket.DropDownItems.Add(miJForexImport);

            // Strategy
            ToolStripMenuItem miStrategy = new ToolStripMenuItem(Language.T("Strategy"));

            ToolStripMenuItem miStrategyOverview = new ToolStripMenuItem();
            miStrategyOverview.Text         = Language.T("Overview") + "...";
            miStrategyOverview.Image        = Properties.Resources.overview;
            miStrategyOverview.ToolTipText  = Language.T("See the strategy overview.");
            miStrategyOverview.ShortcutKeys = Keys.F4;
            miStrategyOverview.Click       += new EventHandler(MenuStrategyOverview_OnClick);
            miStrategy.DropDownItems.Add(miStrategyOverview);

            ToolStripMenuItem miCalculate = new ToolStripMenuItem();
            miCalculate.Text         = Language.T("Recalculate");
            miCalculate.Image        = Properties.Resources.recalculate;
            miCalculate.ToolTipText  = Language.T("Recalculate the strategy.");
            miCalculate.ShortcutKeys = Keys.F5;
            miCalculate.Click       += new EventHandler(MenuAnalysisCalculate_OnClick);
            miStrategy.DropDownItems.Add(miCalculate);

            ToolStripMenuItem miQuickScan = new ToolStripMenuItem();
            miQuickScan.Text         = Language.T("Quick Scan");
            miQuickScan.ToolTipText  = Language.T("Perform quick intrabar scanning.");
            miQuickScan.Image        = Properties.Resources.fast_scan;
            miQuickScan.ShortcutKeys = Keys.F6;
            miQuickScan.Click       += new EventHandler(MenuQuickScan_OnClick);
            miStrategy.DropDownItems.Add(miQuickScan);

            miStrategy.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miStrategyPublish = new ToolStripMenuItem();
            miStrategyPublish.Text        = Language.T("Publish") + "...";
            miStrategyPublish.Image       = Properties.Resources.publish_strategy;
            miStrategyPublish.ToolTipText = Language.T("Publish the strategy in the program's forum.");
            miStrategyPublish.Click      += new EventHandler(MenuStrategyBBcode_OnClick);
            miStrategy.DropDownItems.Add(miStrategyPublish);

            miStrategy.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miUseLogicalGroups = new ToolStripMenuItem();
            miUseLogicalGroups.Text         = Language.T("Use Logical Groups");
            miUseLogicalGroups.ToolTipText  = Language.T("Groups add AND and OR logic interaction of the indicators.");
            miUseLogicalGroups.Checked      = Configs.UseLogicalGroups;
            miUseLogicalGroups.CheckOnClick = true;
            miUseLogicalGroups.Click       += new EventHandler(MenuUseLogicalGroups_OnClick);
            miStrategy.DropDownItems.Add(miUseLogicalGroups);

            ToolStripMenuItem miOpeningLogicConditions = new ToolStripMenuItem();
            miOpeningLogicConditions.Text  = Language.T("Max number of Opening Logic Conditions");
            miOpeningLogicConditions.Image = Properties.Resources.numb_gr;
            miStrategy.DropDownItems.Add(miOpeningLogicConditions);

            for (int i = 2; i < 9; i++)
            {
                ToolStripMenuItem miOpeningLogicSlots = new ToolStripMenuItem();
                miOpeningLogicSlots.Text    = i.ToString();
                miOpeningLogicSlots.Tag     = i;
                miOpeningLogicSlots.Checked = (Configs.MAX_ENTRY_FILTERS == i);
                miOpeningLogicSlots.Click  += new EventHandler(MenuOpeningLogicSlots_OnClick);
                miOpeningLogicConditions.DropDownItems.Add(miOpeningLogicSlots);
            }

            ToolStripMenuItem miClosingLogicConditions = new ToolStripMenuItem();
            miClosingLogicConditions.Text = Language.T("Max number of Closing Logic Conditions");
            miClosingLogicConditions.Image = Properties.Resources.numb_br;
            miStrategy.DropDownItems.Add(miClosingLogicConditions);

            for (int i = 2; i < 9; i++)
            {
                ToolStripMenuItem miClosingLogicSlots = new ToolStripMenuItem();
                miClosingLogicSlots.Text    = i.ToString();
                miClosingLogicSlots.Tag     = i;
                miClosingLogicSlots.Checked = (Configs.MAX_EXIT_FILTERS == i);
                miClosingLogicSlots.Click  += new EventHandler(MenuClosingLogicSlots_OnClick);
                miClosingLogicConditions.DropDownItems.Add(miClosingLogicSlots);
            }

            miStrategy.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miStrategyRemember = new ToolStripMenuItem();
            miStrategyRemember.Text         = Language.T("Remember the Last Strategy");
            miStrategyRemember.ToolTipText  = Language.T("Load the last used strategy at startup.");
            miStrategyRemember.Checked      = Configs.RememberLastStr;
            miStrategyRemember.CheckOnClick = true;
            miStrategyRemember.Click       += new EventHandler(MenuStrategyRemember_OnClick);
            miStrategy.DropDownItems.Add(miStrategyRemember);

            miStrategyAUPBV = new ToolStripMenuItem();
            miStrategyAUPBV.Text         = Language.T("Auto Control of \"Use previous bar value\"");
            miStrategyAUPBV.ToolTipText  = Language.T("Provides automatic setting of the indicators' check box \"Use previous bar value\".");
            miStrategyAUPBV.Checked      = true;
            miStrategyAUPBV.CheckOnClick = true;
            miStrategyAUPBV.Click       += new EventHandler(MenuStrategyAUPBV_OnClick);
            miStrategy.DropDownItems.Add(miStrategyAUPBV);

            // Export
            ToolStripMenuItem miExport = new ToolStripMenuItem(Language.T("Export"));

            ToolStripMenuItem miExpDataOnly = new ToolStripMenuItem();
            miExpDataOnly.Name        = "dataOnly";
            miExpDataOnly.Image       = Properties.Resources.export;
            miExpDataOnly.Text        = Language.T("Market Data") + "...";
            miExpDataOnly.ToolTipText = Language.T("Export market data as a spreadsheet.");
            miExpDataOnly.Click      += new EventHandler(Export_OnClick);
            miExport.DropDownItems.Add(miExpDataOnly);

            ToolStripMenuItem miExpCSVData = new ToolStripMenuItem();
            miExpCSVData.Name        = "CSVData";
            miExpCSVData.Image       = Properties.Resources.export;
            miExpCSVData.Text        = Language.T("Data File") + "...";
            miExpCSVData.ToolTipText = Language.T("Export market data as a CSV file.");
            miExpCSVData.Click      += new EventHandler(Export_OnClick);
            miExport.DropDownItems.Add(miExpCSVData);

            ToolStripMenuItem miExpIndicators = new ToolStripMenuItem();
            miExpIndicators.Name        = "indicators";
            miExpIndicators.Text        = Language.T("Indicators") + "...";
            miExpIndicators.Image       = Properties.Resources.export;
            miExpIndicators.ToolTipText = Language.T("Export market data and indicators as a spreadsheet.");
            miExpIndicators.Click      += new EventHandler(Export_OnClick);
            miExport.DropDownItems.Add(miExpIndicators);

            ToolStripMenuItem miExpBarSummary = new ToolStripMenuItem();
            miExpBarSummary.Name        = "summary";
            miExpBarSummary.Text        = Language.T("Bar Summary") + "...";
            miExpBarSummary.Image       = Properties.Resources.export;
            miExpBarSummary.ToolTipText = Language.T("Export the transactions summary by bars as a spreadsheet.");
            miExpBarSummary.Click      += new EventHandler(Export_OnClick);
            miExport.DropDownItems.Add(miExpBarSummary);

            ToolStripMenuItem miExpPositions = new ToolStripMenuItem();
            miExpPositions.Name        = "positions";
            miExpPositions.Text        = Language.T("Positions") + "...";
            miExpPositions.ToolTipText = Language.T("Export positions in pips as a spreadsheet.");
            miExpPositions.Image       = Properties.Resources.export;
            miExpPositions.Click      += new EventHandler(Export_OnClick);
            miExport.DropDownItems.Add(miExpPositions);

            ToolStripMenuItem miExpMoneyPositions = new ToolStripMenuItem();
            miExpMoneyPositions.Name        = "positionInMoney";
            miExpMoneyPositions.Text        = Language.T("Positions in Currency") + "...";
            miExpMoneyPositions.Image       = Properties.Resources.export;
            miExpMoneyPositions.ToolTipText = Language.T("Export positions in currency as a spreadsheet.");
            miExpMoneyPositions.Click      += new EventHandler(Export_OnClick);
            miExport.DropDownItems.Add(miExpMoneyPositions);

            // Testing
            ToolStripMenuItem miTesting = new ToolStripMenuItem(Language.T("Testing"));

            miStrategyAutoscan = new ToolStripMenuItem();
            miStrategyAutoscan.Text         = Language.T("Automatic Scan");
            miStrategyAutoscan.ToolTipText  = Language.T("Scan the strategy using all available intrabar data.") + Environment.NewLine +  Language.T("Use the scanner to load the data.");
            miStrategyAutoscan.Checked      = Configs.Autoscan;
            miStrategyAutoscan.CheckOnClick = true;
            miStrategyAutoscan.Click       += new EventHandler(MenuStrategyAutoscan_OnClick);
            miTesting.DropDownItems.Add(miStrategyAutoscan);

            ToolStripMenuItem miTradeUntilMC = new ToolStripMenuItem();
            miTradeUntilMC.Name         = "miTradeUntilMC";
            miTradeUntilMC.Text         = Language.T("Trade until a Margin Call");
            miTradeUntilMC.Checked      = Configs.TradeUntilMarginCall;
            miTradeUntilMC.CheckOnClick = true;
            miTradeUntilMC.ToolTipText  = Language.T("Close an open position after a Margin Call.") + Environment.NewLine + Language.T("Do not open a new position when the Free Margin is insufficient.");
            miTradeUntilMC.Click       += new EventHandler(TradeUntilMC_OnClick);
            miTesting.DropDownItems.Add(miTradeUntilMC);

            ToolStripMenuItem miAdditionalStats = new ToolStripMenuItem();
            miAdditionalStats.Name         = "miAdditionalStats";
            miAdditionalStats.Text         = Language.T("Additional Statistics");
            miAdditionalStats.Checked      = Configs.AdditionalStatistics;
            miAdditionalStats.CheckOnClick = true;
            miAdditionalStats.ToolTipText  = Language.T("Show long/short balance lines in the chart and more statistics in the overview.");
            miAdditionalStats.Click       += new EventHandler(AdditionalStats_OnClick);
            miTesting.DropDownItems.Add(miAdditionalStats);

            ToolStripMenuItem miShowClosePrice = new ToolStripMenuItem();
            miShowClosePrice.Name         = "miShowClosePrice";
            miShowClosePrice.Text         = Language.T("Show Price Line on Account Chart");
            miShowClosePrice.Checked      = Configs.ShowPriceChartOnAccountChart;
            miShowClosePrice.CheckOnClick = true;
            miShowClosePrice.Click       += new EventHandler(ShowPriceLine_OnClick);
            miTesting.DropDownItems.Add(miShowClosePrice);

            // Analysis
            ToolStripMenuItem miAnalysis = new ToolStripMenuItem(Language.T("Analysis"));

            ToolStripMenuItem tsmiOverOptimization = new ToolStripMenuItem();
            tsmiOverOptimization.Text   = Language.T("Over-optimization Report");
            tsmiOverOptimization.Name   = "tsmiOverOptimization";
            tsmiOverOptimization.Image  = Properties.Resources.overoptimization_chart;
            tsmiOverOptimization.Click += new EventHandler(MenuTools_OnClick);
            miAnalysis.DropDownItems.Add(tsmiOverOptimization);

            ToolStripMenuItem tsmiCumulativeStrategy = new ToolStripMenuItem();
            tsmiCumulativeStrategy.Text   = Language.T("Cumulative Strategy");
            tsmiCumulativeStrategy.Name   = "tsmiCumulativeStrategy";
            tsmiCumulativeStrategy.Image  = Properties.Resources.cumulative_str;
            tsmiCumulativeStrategy.Click += new EventHandler(MenuTools_OnClick);
            miAnalysis.DropDownItems.Add(tsmiCumulativeStrategy);

            // Tools
            ToolStripMenuItem miTools = new ToolStripMenuItem(Language.T("Tools"));

            ToolStripMenuItem miComparator = new ToolStripMenuItem();
            miComparator.Name        = "Comparator";
            miComparator.Text        = Language.T("Comparator") + "...";
            miComparator.ToolTipText = Language.T("Compare the interpolating methods.");
            miComparator.Image       = Properties.Resources.comparator;
            miComparator.Click      += new EventHandler(MenuTools_OnClick);
            miTools.DropDownItems.Add(miComparator);

            ToolStripMenuItem miScanner = new ToolStripMenuItem();
            miScanner.Name        = "Scanner";
            miScanner.Text        = Language.T("Scanner") + "...";
            miScanner.ToolTipText = Language.T("Perform a deep intrabar scanning.");
            miScanner.Image       = Properties.Resources.scanner;
            miScanner.Click      += new EventHandler(MenuTools_OnClick);
            miTools.DropDownItems.Add(miScanner);

            ToolStripMenuItem miOptimizer = new ToolStripMenuItem();
            miOptimizer.Name        = "Optimizer";
            miOptimizer.Text        = Language.T("Optimizer") + "...";
            miOptimizer.ToolTipText = Language.T("Optimize the strategy parameters.");
            miOptimizer.Image       = Properties.Resources.optimizer;
            miOptimizer.Click      += new EventHandler(MenuTools_OnClick);
            miTools.DropDownItems.Add(miOptimizer);

            ToolStripMenuItem miGenerator = new ToolStripMenuItem();
            miGenerator.Name        = "Generator";
            miGenerator.Text        = Language.T("Generator") + "...";
            miGenerator.ToolTipText = Language.T("Generate or improve a strategy.");
            miGenerator.Image       = Properties.Resources.generator;
            miGenerator.Click      += new EventHandler(MenuTools_OnClick);
            miTools.DropDownItems.Add(miGenerator);

            ToolStripMenuItem miBarExplorer = new ToolStripMenuItem();
            miBarExplorer.Name        = "Bar Explorer";
            miBarExplorer.Text        = Language.T("Bar Explorer") + "...";
            miBarExplorer.ToolTipText = Language.T("Show the price route inside a bar.");
            miBarExplorer.Image       = Properties.Resources.bar_explorer;
            miBarExplorer.Click += new EventHandler(MenuTools_OnClick);
            miTools.DropDownItems.Add(miBarExplorer);

            miTools.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miCustomInd = new ToolStripMenuItem();
            miCustomInd.Name        = "CustomIndicators";
            miCustomInd.Text        = Language.T("Custom Indicators");
            miCustomInd.Image       = Properties.Resources.custom_ind;

            ToolStripMenuItem miReloadInd = new ToolStripMenuItem();
            miReloadInd.Name         = "miReloadInd";
            miReloadInd.Text         = Language.T("Reload the Custom Indicators");
            miReloadInd.Image        = Properties.Resources.reload_ind;
            miReloadInd.ShortcutKeys = Keys.Control | Keys.I;
            miReloadInd.Click       += new EventHandler(MenuTools_OnClick);
            miCustomInd.DropDownItems.Add(miReloadInd);

            ToolStripMenuItem miCheckInd = new ToolStripMenuItem();
            miCheckInd.Name   = "miCheckInd";
            miCheckInd.Text   = Language.T("Check the Custom Indicators");
            miCheckInd.Image  = Properties.Resources.check_ind;
            miCheckInd.Click += new EventHandler(MenuTools_OnClick);
            miCustomInd.DropDownItems.Add(miCheckInd);

            miCustomInd.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miExportAsCI = new ToolStripMenuItem();
            miExportAsCI.Name   = "miExportAsCI";
            miExportAsCI.Text   = Language.T("Export the Strategy as a Custom Indicator");
            miExportAsCI.Image  = Properties.Resources.str_export_as_ci;
            miExportAsCI.Click += new EventHandler(MenuTools_OnClick);
            miCustomInd.DropDownItems.Add(miExportAsCI);

            miCustomInd.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miOpenIndFolder = new ToolStripMenuItem();
            miOpenIndFolder.Name   = "miOpenIndFolder";
            miOpenIndFolder.Text   = Language.T("Open the Source Files Folder") + "...";
            miOpenIndFolder.Image  = Properties.Resources.folder_open;
            miOpenIndFolder.Click += new EventHandler(MenuTools_OnClick);
            miCustomInd.DropDownItems.Add(miOpenIndFolder);

            ToolStripMenuItem miCustIndForum = new ToolStripMenuItem();
            miCustIndForum.Text   = Language.T("Custom Indicators Forum") + "...";
            miCustIndForum.Image  = Properties.Resources.forum_icon;
            miCustIndForum.Tag    = "http://forexsb.com/forum/forum/30/";
            miCustIndForum.Click += new EventHandler(MenuHelpContentsOnClick);
            miCustomInd.DropDownItems.Add(miCustIndForum);

            miCustomInd.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miLoadCstomInd = new ToolStripMenuItem();
            miLoadCstomInd.Name         = "miLoadCstomInd";
            miLoadCstomInd.Text         = Language.T("Load the Custom Indicators at Startup");
            miLoadCstomInd.Checked      = Configs.LoadCustomIndicators;
            miLoadCstomInd.CheckOnClick = true;
            miLoadCstomInd.Click       += new EventHandler(LoadCustomIndicators_OnClick);
            miCustomInd.DropDownItems.Add(miLoadCstomInd);

            ToolStripMenuItem miShowCstomInd = new ToolStripMenuItem();
            miShowCstomInd.Name         = "miShowCstomInd";
            miShowCstomInd.Text         = Language.T("Show the Loaded Custom Indicators");
            miShowCstomInd.Checked      = Configs.ShowCustomIndicators;
            miShowCstomInd.CheckOnClick = true;
            miShowCstomInd.Click       += new EventHandler(ShowCustomIndicators_OnClick);
            miCustomInd.DropDownItems.Add(miShowCstomInd);

            miTools.DropDownItems.Add(miCustomInd);

            miTools.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miPlaySounds = new ToolStripMenuItem();
            miPlaySounds.Text         = Language.T("Play Sounds");
            miPlaySounds.Name         = "miPlaySounds";
            miPlaySounds.Checked      = Configs.PlaySounds;
            miPlaySounds.CheckOnClick = true;
            miPlaySounds.Click       += new EventHandler(MenuTools_OnClick);
            miTools.DropDownItems.Add(miPlaySounds);

            miTools.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miProfitCalculator = new ToolStripMenuItem();
            miProfitCalculator.Name   = "ProfitCalculator";
            miProfitCalculator.Image  = Properties.Resources.profit_calculator;
            miProfitCalculator.Text   = Language.T("Profit Calculator") + "...";
            miProfitCalculator.Click += new EventHandler(MenuTools_OnClick);
            miTools.DropDownItems.Add(miProfitCalculator);

            ToolStripMenuItem miPivotPoints = new ToolStripMenuItem();
            miPivotPoints.Name   = "PivotPoints";
            miPivotPoints.Image  = Properties.Resources.pivot_points;
            miPivotPoints.Text   = Language.T("Pivot Points") + "...";
            miPivotPoints.Click += new EventHandler(MenuTools_OnClick);
            miTools.DropDownItems.Add(miPivotPoints);

            ToolStripMenuItem miFibonacciLevels = new ToolStripMenuItem();
            miFibonacciLevels.Name   = "FibonacciLevels";
            miFibonacciLevels.Image  = Properties.Resources.fibo_levels;
            miFibonacciLevels.Text   = Language.T("Fibonacci Levels") + "...";
            miFibonacciLevels.Click += new EventHandler(MenuTools_OnClick);
            miTools.DropDownItems.Add(miFibonacciLevels);

            miTools.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miAdditional = new ToolStripMenuItem();
            miAdditional.Text  = Language.T("Additional");
            miAdditional.Image = Properties.Resources.tools;

            miTools.DropDownItems.Add(miAdditional);

            ToolStripMenuItem miCalculator = new ToolStripMenuItem();
            miCalculator.Name         = "Calculator";
            miCalculator.Image        = Properties.Resources.calculator;
            miCalculator.Text         = Language.T("Calculator") + "...";
            miCalculator.ToolTipText  = Language.T("A simple calculator.");
            miCalculator.ShortcutKeys = Keys.F12;
            miCalculator.Click       += new EventHandler(MenuTools_OnClick);
            miAdditional.DropDownItems.Add(miCalculator);

            miAdditional.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miCommandConsole = new ToolStripMenuItem();
            miCommandConsole.Name   = "CommandConsole";
            miCommandConsole.Text   = Language.T("Command Console") + "...";
            miCommandConsole.Image  = Properties.Resources.prompt;
            miCommandConsole.Click += new EventHandler(MenuTools_OnClick);
            miAdditional.DropDownItems.Add(miCommandConsole);

            miTools.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miResetConfigs = new ToolStripMenuItem();
            miResetConfigs.Name        = "Reset settings";
            miResetConfigs.Text        = Language.T("Reset Settings");
            miResetConfigs.ToolTipText = Language.T("Reset the program settings to their default values. You need to restart!");
            miResetConfigs.Image       = Properties.Resources.warning;
            miResetConfigs.Click      += new EventHandler(MenuTools_OnClick);
            miTools.DropDownItems.Add(miResetConfigs);

            // Help
            ToolStripMenuItem miHelp = new ToolStripMenuItem(Language.T("Help"));

            ToolStripMenuItem miTipOfTheDay = new ToolStripMenuItem();
            miTipOfTheDay.Text        = Language.T("Tip of the Day") + "...";
            miTipOfTheDay.ToolTipText = Language.T("Show a tip.");
            miTipOfTheDay.Image       = Properties.Resources.hint;
            miTipOfTheDay.Tag         = "tips";
            miTipOfTheDay.Click      += new EventHandler(MenuHelpContentsOnClick);
            miHelp.DropDownItems.Add(miTipOfTheDay);

            ToolStripMenuItem miHelpOnlineHelp = new ToolStripMenuItem();
            miHelpOnlineHelp.Text         = Language.T("Online Help") + "...";
            miHelpOnlineHelp.Image        = Properties.Resources.help;
            miHelpOnlineHelp.ToolTipText  = Language.T("Show the online help.");
            miHelpOnlineHelp.Tag          = "http://forexsb.com/wiki/fsb/manual/start";
            miHelpOnlineHelp.ShortcutKeys = Keys.F1;
            miHelpOnlineHelp.Click       += new EventHandler(MenuHelpContentsOnClick);
            miHelp.DropDownItems.Add(miHelpOnlineHelp);

            miHelp.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miHelpForum = new ToolStripMenuItem();
            miHelpForum.Text        = Language.T("Support Forum") + "...";
            miHelpForum.Image       = Properties.Resources.forum_icon;
            miHelpForum.Tag         = "http://forexsb.com/forum/";
            miHelpForum.ToolTipText = Language.T("Show the program's forum.");
            miHelpForum.Click      += new EventHandler(MenuHelpContentsOnClick);
            miHelp.DropDownItems.Add(miHelpForum);

            miHelp.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miHelpDonateNow = new ToolStripMenuItem();
            miHelpDonateNow.Text        = Language.T("Contribute") + "...";
            miHelpDonateNow.Image       = Properties.Resources.contribute;
            miHelpDonateNow.ToolTipText = Language.T("Donate, Support, Advertise!");
            miHelpDonateNow.Tag         = "http://forexsb.com/wiki/contribution";
            miHelpDonateNow.Click      += new EventHandler(MenuHelpContentsOnClick);
            miHelp.DropDownItems.Add(miHelpDonateNow);

            miHelp.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miHelpUpdates = new ToolStripMenuItem();
            miHelpUpdates.Text         = Language.T("Check for Updates at Startup");
            miHelpUpdates.Checked      = Configs.CheckForUpdates;
            miHelpUpdates.CheckOnClick = true;
            miHelpUpdates.Click       += new EventHandler(MenuHelpUpdates_OnClick);
            miHelp.DropDownItems.Add(miHelpUpdates);

            ToolStripMenuItem miHelpNewBeta = new ToolStripMenuItem();
            miHelpNewBeta.Text         = Language.T("Check for New Beta Versions");
            miHelpNewBeta.Checked      = Configs.CheckForNewBeta;
            miHelpNewBeta.CheckOnClick = true;
            miHelpNewBeta.Click       += new EventHandler(MenuHelpNewBeta_OnClick);
            miHelp.DropDownItems.Add(miHelpNewBeta);


            miHelp.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miHelpAbout = new ToolStripMenuItem();
            miHelpAbout.Text        = Language.T("About") + " " + Data.ProgramName + "...";
            miHelpAbout.ToolTipText = Language.T("Show the program information.");
            miHelpAbout.Image       = Properties.Resources.information;
            miHelpAbout.Click      += new EventHandler(MenuHelpAboutOnClick);
            miHelp.DropDownItems.Add(miHelpAbout);

            // Forex
            miForex = new ToolStripMenuItem(Language.T("Forex"));

            ToolStripMenuItem miEconomicCalendar = new ToolStripMenuItem();
            miEconomicCalendar.Text   = Language.T("Economic Calendar") + "...";
            miEconomicCalendar.Image  = Properties.Resources._1day;
            miEconomicCalendar.Tag    = "http://forexsb.com/pages/calendar.html";
            miEconomicCalendar.Click += new EventHandler(MenuForexContentsOnClick);
            miForex.DropDownItems.Add(miEconomicCalendar);

            ToolStripMenuItem miMarketCommentary = new ToolStripMenuItem();
            miMarketCommentary.Text   = Language.T("Market Commentary") + "...";
            miMarketCommentary.Image  = Properties.Resources.pie;
            miMarketCommentary.Tag    = "http://forexsb.com/pages/commentary.html";
            miMarketCommentary.Click += new EventHandler(MenuForexContentsOnClick);
            miForex.DropDownItems.Add(miMarketCommentary);

            ToolStripMenuItem miForexDailyOutlook = new ToolStripMenuItem();
            miForexDailyOutlook.Text   = Language.T("Daily Forex Outlook") + "...";
            miForexDailyOutlook.Image  = Properties.Resources.fx_overview;
            miForexDailyOutlook.Tag    = "http://forexsb.com/pages/daily-outlook.html";
            miForexDailyOutlook.Click += new EventHandler(MenuForexContentsOnClick);
            miForex.DropDownItems.Add(miForexDailyOutlook);

            ToolStripMenuItem miForexWeeklyOutlook = new ToolStripMenuItem();
            miForexWeeklyOutlook.Text   = Language.T("Weekly Forex Outlook") + "...";
            miForexWeeklyOutlook.Image  = Properties.Resources.fx_overview;
            miForexWeeklyOutlook.Tag    = "http://forexsb.com/pages/weekly-outlook.html";
            miForexWeeklyOutlook.Click += new EventHandler(MenuForexContentsOnClick);
            miForex.DropDownItems.Add(miForexWeeklyOutlook);

            miForex.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miForexBrokers = new ToolStripMenuItem();
            miForexBrokers.Text   = Language.T("Forex Brokers") + "...";
            miForexBrokers.Image  = Properties.Resources.forex_brokers;
            miForexBrokers.Tag    = "http://forexsb.com/wiki/brokers";
            miForexBrokers.Click += new EventHandler(MenuForexContentsOnClick);

            miForex.DropDownItems.Add(miForexBrokers);

            // LiveContent
            miLiveContent = new ToolStripMenuItem(Language.T("New Version"));
            miLiveContent.Alignment = ToolStripItemAlignment.Right;
            miLiveContent.BackColor = Color.Khaki;
            miLiveContent.ForeColor = Color.DarkGreen;
            miLiveContent.Visible   = false;

            // Forex Forum
            ToolStripMenuItem miForum = new ToolStripMenuItem(Properties.Resources.forum_icon);
            miForum.Alignment   = ToolStripItemAlignment.Right;
            miForum.Tag         = "http://forexsb.com/forum/";
            miForum.ToolTipText = Language.T("Show the program's forum.");
            miForum.Click      += new EventHandler(MenuForexContentsOnClick);

            // MainMenu
            ToolStripMenuItem[] mainMenu = new ToolStripMenuItem[]
            {
                miFile, miEdit, miView, miAccount, miMarket, miStrategy, miExport,
                miTesting, miAnalysis, miTools, miHelp, miForex, miLiveContent, miForum
            };

            MainMenuStrip.Items.AddRange(mainMenu);
            MainMenuStrip.ShowItemToolTips = true;
        }

        /// <summary>
        /// Sets the StatusBar
        /// </summary>
        protected void InitializeStatusBar()
        {
            sbpInstrument = new ToolStripStatusLabel();
            sbpInstrument.Text        = "";
            sbpInstrument.ToolTipText = Language.T("Symbol Period (Spread, Swap numbers, Slippage)");
            sbpInstrument.BorderStyle = Border3DStyle.Raised;
            statusStrip.Items.Add(sbpInstrument);
            statusStrip.Items.Add(new ToolStripSeparator());

            sbpChartInfo = new ToolStripStatusLabel();
            sbpChartInfo.Text        = "";
            sbpChartInfo.ToolTipText = Language.T("Price close");
            sbpChartInfo.BorderStyle = Border3DStyle.Raised;
            sbpChartInfo.Spring      = true;
            statusStrip.Items.Add(sbpChartInfo);

            statusStrip.Items.Add(new ToolStripSeparator());

            sbpDate = new ToolStripStatusLabel();
            sbpDate.ToolTipText = Language.T("The current date");
            statusStrip.Items.Add(sbpDate);

            sbpTime = new ToolStripStatusLabel();
            sbpTime.ToolTipText = Language.T("The current time");
            statusStrip.Items.Add(sbpTime);

            Timer timer = new Timer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = 1000;
            timer.Start();
        }

        /// <summary>
        /// Ubdates the clock in the statusbar.
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            sbpDate.Text = dt.ToString(Data.DF);
            sbpTime.Text = dt.ToShortTimeString();
        }

        /// <summary>
        /// Saves the current strategy
        /// </summary>
        protected virtual void MenuFileSave_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Opens the SaveAs menu
        /// </summary>
        protected virtual void MenuFileSaveAs_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Opens a saved strategy
        /// </summary>
        protected virtual void MenuFileOpen_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Closes the program
        /// </summary>
        private void MenuFileCloseOnClick(object sender, EventArgs e)
        {
            this.Close();
        }

        // Sets the programs language
        protected virtual void Language_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Gradient View Changed
        /// </summary>
        protected virtual void MenuGradientView_OnClick(object sender, EventArgs e)
        {
            Configs.GradientView = ((ToolStripMenuItem)sender).Checked;
            pnlWorkspace.Invalidate(true);

            return;
        }

        /// <summary>
        /// Load a color scheme
        /// </summary>
        protected virtual void MenuLoadColor_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Change Full Screan mode
        /// </summary>
        protected virtual void MenuViewFullScreen_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Whether to express account in pips or in currency
        /// </summary>
        protected virtual void AccountShowInMoney_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Open the account setting dialog
        /// </summary>
        protected virtual void MenuAccountSettings_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Loads data
        /// </summary>
        protected virtual void MenuLoadData_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Check the data
        /// </summary>
        protected virtual void MenuCheckData_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Refine the data
        /// </summary>
        protected virtual void MenuRefineData_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Data Horizon
        /// </summary>
        protected virtual void MenuDataHorizon_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Data Directory
        /// </summary>
        protected virtual void MenuDataDirectory_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Loads the default strategy
        /// </summary>
        protected virtual void MenuStrategyNew_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Opens the strategy settings dialogue
        /// </summary>
        protected virtual void MenuStrategySettings_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Opens the strategy settings dialogue
        /// </summary>
        protected virtual void MenuStrategyAUPBV_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Use logical groups menu item.
        /// </summary>
        protected virtual void MenuUseLogicalGroups_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Remember the last used strategy
        /// </summary>
        protected void MenuStrategyRemember_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Configs.RememberLastStr = mi.Checked;
            if (mi.Checked == false)
            {
                Configs.LastStrategy = "";
            }

            return;
        }

        /// <summary>
        /// Autoscan
        /// </summary>
        protected virtual void MenuStrategyAutoscan_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Opens the strategy overview window
        /// </summary>
        void MenuStrategyOverview_OnClick(object sender, EventArgs e)
        {
            Browser so = new Browser(Language.T("Strategy Overview"), Data.Strategy.GenerateHTMLOverview());
            so.Show();
        }

        /// <summary>
        /// Undos the strategy
        /// </summary>
        protected virtual void MenuStrategyUndo_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Add a new Open Filter
        /// </summary>
        protected virtual void MenuStrategyAddOpenFilter_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Add a new Close Filter
        /// </summary>
        protected virtual void MenuStrategyAddCloseFilter_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Export the strategy in BBCode format - ready to post in the forum
        /// </summary>
        protected virtual void MenuStrategyBBcode_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Forces the calculating of the strategy
        /// </summary>
        protected virtual void MenuAnalysisCalculate_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Forces the scanning of the strategy
        /// </summary>
        protected virtual void MenuQuickScan_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Performs a detailed back-test
        /// </summary>
        protected virtual void MenuDetailedBacktest_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Opens the about window
        /// </summary>
        private void MenuHelpAboutOnClick(object sender, EventArgs e)
        {
            AboutScreen abScr = new AboutScreen();
            abScr.ShowDialog();

            return;
        }

        /// <summary>
        /// Menu Journal mode click
        /// </summary>
        protected virtual void MenuJournal_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Menu TradeUntilMC mode click
        /// </summary>
        protected virtual void TradeUntilMC_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Menu miAdditionalStats mode click
        /// </summary>
        protected virtual void AdditionalStats_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Tools menu
        /// </summary>
        protected virtual void MenuTools_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Show the full Price Chart
        /// </summary>
        protected virtual void ShowPriceChart_OnClick(object sender, EventArgs e)
        {
            if (Data.IsData && Data.IsResult)
            {
                Chart chart = new Chart();

                chart.BarPixels         = Configs.IndicatorChartZoom;
                chart.ShowInfoPanel     = Configs.IndicatorChartInfoPanel;
                chart.ShowDynInfo       = Configs.IndicatorChartDynamicInfo;
                chart.ShowGrid          = Configs.IndicatorChartGrid;
                chart.ShowCross         = Configs.IndicatorChartCross;
                chart.ShowVolume        = Configs.IndicatorChartVolume;
                chart.ShowPositionLots  = Configs.IndicatorChartLots;
                chart.ShowOrders        = Configs.IndicatorChartEntryExitPoints;
                chart.ShowPositionPrice = Configs.IndicatorChartCorrectedPositionPrice;
                chart.ShowBalanceEquity = Configs.IndicatorChartBalanceEquityChart;
                chart.ShowFloatingPL    = Configs.IndicatorChartFloatingPLChart;
                chart.ShowIndicators    = Configs.IndicatorChartIndicators;
                chart.ShowAmbiguousBars = Configs.IndicatorChartAmbiguousMark;
                chart.TrueCharts        = Configs.IndicatorChartTrueCharts;

                chart.ShowDialog();

                Configs.IndicatorChartZoom                   = chart.BarPixels;
                Configs.IndicatorChartInfoPanel              = chart.ShowInfoPanel;
                Configs.IndicatorChartDynamicInfo            = chart.ShowDynInfo;
                Configs.IndicatorChartGrid                   = chart.ShowGrid;
                Configs.IndicatorChartCross                  = chart.ShowCross;
                Configs.IndicatorChartVolume                 = chart.ShowVolume;
                Configs.IndicatorChartLots                   = chart.ShowPositionLots;
                Configs.IndicatorChartEntryExitPoints        = chart.ShowOrders;
                Configs.IndicatorChartCorrectedPositionPrice = chart.ShowPositionPrice;
                Configs.IndicatorChartBalanceEquityChart     = chart.ShowBalanceEquity;
                Configs.IndicatorChartFloatingPLChart        = chart.ShowFloatingPL;
                Configs.IndicatorChartIndicators             = chart.ShowIndicators;
                Configs.IndicatorChartAmbiguousMark          = chart.ShowAmbiguousBars;
                Configs.IndicatorChartTrueCharts             = chart.TrueCharts;
            }

            return;
        }

        /// <summary>
        /// Show the full Account Chart
        /// </summary>
        protected virtual void ShowAccountChart_OnClick(object sender, EventArgs e)
        {
            if (Data.IsData && Data.IsResult)
            {
                Chart chart = new Chart();

                chart.BarPixels         = Configs.BalanceChartZoom;
                chart.ShowInfoPanel     = Configs.BalanceChartInfoPanel;
                chart.ShowDynInfo       = Configs.BalanceChartDynamicInfo;
                chart.ShowGrid          = Configs.BalanceChartGrid;
                chart.ShowCross         = Configs.BalanceChartCross;
                chart.ShowVolume        = Configs.BalanceChartVolume;
                chart.ShowPositionLots  = Configs.BalanceChartLots;
                chart.ShowOrders        = Configs.BalanceChartEntryExitPoints;
                chart.ShowPositionPrice = Configs.BalanceChartCorrectedPositionPrice;
                chart.ShowBalanceEquity = Configs.BalanceChartBalanceEquityChart;
                chart.ShowFloatingPL    = Configs.BalanceChartFloatingPLChart;
                chart.ShowIndicators    = Configs.BalanceChartIndicators;
                chart.ShowAmbiguousBars = Configs.BalanceChartAmbiguousMark;
                chart.TrueCharts        = Configs.BalanceChartTrueCharts;

                chart.ShowDialog();

                Configs.BalanceChartZoom                   = chart.BarPixels;
                Configs.BalanceChartInfoPanel              = chart.ShowInfoPanel;
                Configs.BalanceChartDynamicInfo            = chart.ShowDynInfo;
                Configs.BalanceChartGrid                   = chart.ShowGrid;
                Configs.BalanceChartCross                  = chart.ShowCross;
                Configs.BalanceChartVolume                 = chart.ShowVolume;
                Configs.BalanceChartLots                   = chart.ShowPositionLots;
                Configs.BalanceChartEntryExitPoints        = chart.ShowOrders;
                Configs.BalanceChartCorrectedPositionPrice = chart.ShowPositionPrice;
                Configs.BalanceChartBalanceEquityChart     = chart.ShowBalanceEquity;
                Configs.BalanceChartFloatingPLChart        = chart.ShowFloatingPL;
                Configs.BalanceChartIndicators             = chart.ShowIndicators;
                Configs.BalanceChartAmbiguousMark          = chart.ShowAmbiguousBars;
                Configs.BalanceChartTrueCharts             = chart.TrueCharts;
            }

            return;
        }

        /// <summary>
        /// Export menu
        /// </summary>
        void Export_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            string strMIName = mi.Name;

            Exporter exporter = new Exporter();

            switch (strMIName)
            {
                case "dataOnly":
                    exporter.ExportDataOnly();
                    break;
                case "CSVData":
                    exporter.ExportCSVData();
                    break;
                case "indicators":
                    exporter.ExportIndicators();
                    break;
                case "summary":
                    exporter.ExportBarSummary();
                    break;
                case "positions":
                    exporter.ExportPositions();
                    break;
                case "positionInMoney":
                    exporter.ExportPositionsInMoney();
                    break;
                default:
                    break;

            }

            return;
        }

        /// <summary>
        /// Opens the help window
        /// </summary>
        private void MenuHelpContentsOnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;

            if ((string)mi.Tag == "tips")
            {
                Starting_Tips shv = new Starting_Tips();
                shv.Show();
                return;
            }

            try
            {
                System.Diagnostics.Process.Start((string)mi.Tag);
            }
            catch{ }

            return;
        }

        /// <summary>
        /// Opens the forex news
        /// </summary>
        private void MenuForexContentsOnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;

            try
            {
                System.Diagnostics.Process.Start((string)mi.Tag);
            }
            catch { }

            return;
        }

        /// <summary>
        /// Menu miHelpUpdates click
        /// </summary>
        protected virtual void MenuHelpUpdates_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Configs.CheckForUpdates = mi.Checked;

            return;
        }

        /// <summary>
        /// Menu miHelpNewBeta  click
        /// </summary>
        protected virtual void MenuHelpNewBeta_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Configs.CheckForNewBeta = mi.Checked;

            return;
        }

        /// <summary>
        /// Menu LoadCustomIndicators click
        /// </summary>
        protected virtual void LoadCustomIndicators_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Configs.LoadCustomIndicators = mi.Checked;

            return;
        }

        /// <summary>
        /// Menu ShowCustomIndicators click
        /// </summary>
        protected virtual void ShowCustomIndicators_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Configs.ShowCustomIndicators = mi.Checked;

            return;
        }

        /// <summary>
        /// Menu MenuOpeningLogicSlots_OnClick
        /// </summary>
        protected virtual void MenuOpeningLogicSlots_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Menu MenuClosingLogicSlots_OnClick
        /// </summary>
        protected virtual void MenuClosingLogicSlots_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Menu MenuPrevHistory_OnClick
        /// </summary>
        protected virtual void MenuPrevHistory_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Menu MenuNextHistory_OnClick
        /// </summary>
        protected virtual void MenuNextHistory_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Menu ShowPriceLine_OnClick
        /// </summary>
        protected virtual void ShowPriceLine_OnClick(object sender, EventArgs e)
        {
        }
    }
}
