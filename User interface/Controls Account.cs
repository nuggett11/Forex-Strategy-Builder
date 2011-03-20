// Controls Account
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
    /// Class Controls Account: Menu_and_StatusBar
    /// </summary>
    public partial class Controls : Menu_and_StatusBar
    {
        protected ToolStripComboBox   tscbInterpolationMethod; // Interpolation methods
        protected ToolStripButton     tsbtComparator;          // Opens the Comparator
        protected ToolStripButton     tsbtScanner;             // Opens the Scanner
        protected ToolStripButton     tsbtAnalyzer;            // Opens the Analyzer
        protected Info_Panel          infpnlAccountStatistics; // Account Statistics
        protected Small_Balance_Chart smallBalanceChart;       // Small Balance Chart

        /// <summary>
        /// Initializes the controls in panel pnlOverview
        /// </summary>
        void InitializeAccount()
        {
            string[] asMethods = Enum.GetNames(typeof(InterpolationMethod));
            for (int i = 0; i < asMethods.Length; i++)
                asMethods[i] = Language.T(asMethods[i]);

            Graphics g = CreateGraphics();
            int iLongestMethod = 0;
            foreach (string sMethod in asMethods)
                if ((int)g.MeasureString(sMethod, Font).Width > iLongestMethod)
                    iLongestMethod = (int)g.MeasureString(sMethod, Font).Width;
            g.Dispose();

            // ComboBox Interpolation Methods
            tscbInterpolationMethod = new ToolStripComboBox();
            tscbInterpolationMethod.Name          = "tscbInterpolationMethod";
            tscbInterpolationMethod.AutoSize      = false;
            tscbInterpolationMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            tscbInterpolationMethod.Items.AddRange(asMethods);
            tscbInterpolationMethod.SelectedIndex = 0;
            tscbInterpolationMethod.Width         = iLongestMethod + (int)(18 * Data.HorizontalDLU);
            tscbInterpolationMethod.ToolTipText   = Language.T("Bar interpolation method.");
            tscbInterpolationMethod.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
            tsAccount.Items.Add(tscbInterpolationMethod);

            // Button Comparator
            tsbtComparator =  new ToolStripButton();
            tsbtComparator.Text        = Language.T("Comparator");
            tsbtComparator.Name        = "Comparator";
            tsbtComparator.Click      += new EventHandler(BtnTools_OnClick);
            tsbtComparator.ToolTipText = Language.T("Compare the interpolating methods.");
            tsAccount.Items.Add(tsbtComparator);

            // Button Scanner
            tsbtScanner = new ToolStripButton();
            tsbtScanner.Text        = Language.T("Scanner");
            tsbtScanner.Name        = "Scanner";
            tsbtScanner.Click      += new EventHandler(BtnTools_OnClick);
            tsbtScanner.ToolTipText = Language.T("Perform a deep intrabar scanning.") + Environment.NewLine + Language.T("Quick scan") + " - F6.";
            tsAccount.Items.Add(tsbtScanner);

            // Button Analyzer
            tsbtAnalyzer = new ToolStripButton();
            tsbtAnalyzer.Text   = Language.T("Analyzer");
            tsbtAnalyzer.Name   = "Analyzer";
            tsbtAnalyzer.Click += new EventHandler(BtnTools_OnClick);
            tsAccount.Items.Add(tsbtAnalyzer);

            // Info Panel Account Statistics
            infpnlAccountStatistics = new Info_Panel();
            infpnlAccountStatistics.Parent = pnlAccount;
            infpnlAccountStatistics.Dock   = DockStyle.Fill;

            Splitter spliter    = new Splitter();
            spliter.Parent      = pnlAccount;
            spliter.Dock        = DockStyle.Bottom;
            spliter.BorderStyle = BorderStyle.None;
            spliter.Height      = space;

            // Small Balance Chart
            smallBalanceChart = new Small_Balance_Chart();
            smallBalanceChart.Parent          = pnlAccount;
            smallBalanceChart.Cursor          = Cursors.Hand;
            smallBalanceChart.Dock            = DockStyle.Bottom;
            smallBalanceChart.MinimumSize     = new Size(100, 50);
            smallBalanceChart.ShowDynamicInfo = true;
            smallBalanceChart.MouseMove      += new MouseEventHandler(SmallBalanceChart_MouseMove);
            smallBalanceChart.MouseLeave     += new EventHandler(SmallBalanceChart_MouseLeave);
            smallBalanceChart.MouseUp        += new MouseEventHandler(SmallBalanceChart_MouseUp);
            toolTip.SetToolTip(smallBalanceChart, Language.T("Click to view the full chart.") +
                                                  Environment.NewLine +
                                                  Language.T("Right click to detach chart."));

            pnlAccount.Resize += new EventHandler(pnlAccount_Resize);

            return;
        }

        /// <summary>
        /// Arranges the controls after resizing
        /// </summary>
        void pnlAccount_Resize(object sender, EventArgs e)
        {
            smallBalanceChart.Height = 2 * pnlAccount.ClientSize.Height / (Configs.ShowJournal ? 3 : 4);

            return;
        }

        /// <summary>
        /// Show the dynamic info on the status bar
        /// </summary>
        void SmallBalanceChart_MouseMove(object sender, MouseEventArgs e)
        {
            Small_Balance_Chart chart = (Small_Balance_Chart)sender;
            ToolStripStatusLabelChartInfo = chart.CurrentBarInfo;

            return;
        }

        /// <summary>
        /// Deletes the dynamic info on the status bar
        /// </summary>
        void SmallBalanceChart_MouseLeave(object sender, EventArgs e)
        {
            ToolStripStatusLabelChartInfo = string.Empty;

            return;
        }

        /// <summary>
        /// Shows the full account chart after clicking on it
        /// </summary>
        void SmallBalanceChart_MouseUp(object sender, MouseEventArgs e)
        {
            if (Data.IsData && Data.IsResult && e.Button == MouseButtons.Left)
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
            else if (Data.IsData && Data.IsResult && e.Button == MouseButtons.Right)
            {
                Dialogs.Balance_Chart balanceChart = new Dialogs.Balance_Chart();
                balanceChart.ShowDialog();
            }

            return;
        }

        /// <summary>
        /// Opens the corresponding tool
        /// </summary>
        protected virtual void BtnTools_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Tools menu
        /// </summary>
        protected override void MenuTools_OnClick(object sender, EventArgs e)
        {
        }
    }
}
