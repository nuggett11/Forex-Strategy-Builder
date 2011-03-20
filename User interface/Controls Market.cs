// Forex Strategy Builder - Market controls.
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
    /// Class Controls : Menu_and_StatusBar
    /// </summary>
    public partial class Controls : Menu_and_StatusBar
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        public Controls()
        {
            InitializeMarket();
            InitializeStrategy();
            InitializeAccount();
            InitializeJournal();
        }

        /// <summary>
        /// Invalidates the sender after scrolling
        /// </summary>
        void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            ((Control)sender).Parent.Invalidate();
        }

        // Market
        protected ToolStripComboBox tscbSymbol;   // Symbol
        protected ToolStripComboBox tscbPeriod;   // Period
        protected ToolStripButton   tsbtnCharges; // Charges

        protected Info_Panel infpnlMarketStatistics; // Market Stats
        protected Small_Indicator_Chart smallIndicatorChart; //Indicator chart

        /// <summary>
        /// Initialize the controls in panel pnlMarket
        /// </summary>
        void InitializeMarket()
        {
            // Symbol
            tscbSymbol = new ToolStripComboBox();
            tscbSymbol.Name          = "tscbSymbol";
            tscbSymbol.AutoSize      = false;
            tscbSymbol.Items.AddRange(Instruments.SymbolList);
            tscbSymbol.DropDownStyle = ComboBoxStyle.DropDownList;
            tscbSymbol.SelectedIndex = 0;
            tscbSymbol.ToolTipText   = Language.T("Symbol");
            tscbSymbol.Overflow      = ToolStripItemOverflow.Never;
            tscbSymbol.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
            tsMarket.Items.Add(tscbSymbol);

            // Period
            tscbPeriod = new ToolStripComboBox();
            tscbPeriod.Name          = "tscbPeriod";
            tscbPeriod.AutoSize      = false;
            tscbPeriod.DropDownStyle = ComboBoxStyle.DropDownList;
            tscbPeriod.Items.AddRange(new string []
                {
                    "  1 " + Language.T("Minute"),
                    "  5 " + Language.T("Minutes"),
                    "15 "  + Language.T("Minutes"),
                    "30 "  + Language.T("Minutes"),
                    "  1 " + Language.T("Hour"),
                    "  4 " + Language.T("Hours"),
                    "  1 " + Language.T("Day"),
                    "  1 " + Language.T("Week")
                });
            tscbPeriod.SelectedIndex = 6;
            tscbPeriod.ToolTipText   =  Language.T("Data time frame.");
            tscbPeriod.Overflow      = ToolStripItemOverflow.Never;
            tscbPeriod.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
            tsMarket.Items.Add(tscbPeriod);

            // Button Market Properties
            tsbtnCharges = new ToolStripButton();
            tsbtnCharges.Text        = Language.T("Charges");
            tsbtnCharges.Name        = "Charges";
            tsbtnCharges.ToolTipText = Language.T("Spread, Swap numbers, Slippage.");
            tsbtnCharges.Overflow    = ToolStripItemOverflow.Never;
            tsbtnCharges.Click      += new EventHandler(BtnTools_OnClick);
            tsMarket.Items.Add(tsbtnCharges);

            tsMarket.Resize += new EventHandler(tsMarket_Resize);

            // Info Panel Market Statistics
            infpnlMarketStatistics = new Info_Panel();
            infpnlMarketStatistics.Parent = pnlMarket;
            infpnlMarketStatistics.Dock   = DockStyle.Fill;

            // Splitter
            Splitter splitter    = new Splitter();
            splitter.Parent      = pnlMarket;
            splitter.Dock        = DockStyle.Bottom;
            splitter.BorderStyle = BorderStyle.None;
            splitter.Height      = space;

            // Small Indicator Chart
            smallIndicatorChart = new Small_Indicator_Chart();
            smallIndicatorChart.Parent          = pnlMarket;
            smallIndicatorChart.Cursor          = Cursors.Hand;
            smallIndicatorChart.Dock            = DockStyle.Bottom;
            smallIndicatorChart.MinimumSize     = new Size(100, 50);
            smallIndicatorChart.ShowDynamicInfo = true;
            smallIndicatorChart.MouseUp        += new MouseEventHandler(SmallIndicatorChart_MouseUp);
            smallIndicatorChart.MouseMove      += new MouseEventHandler(SmallIndicatorChart_MouseMove);
            smallIndicatorChart.MouseLeave     += new EventHandler(SmallIndicatorChart_MouseLeave);
            toolTip.SetToolTip(smallIndicatorChart, Language.T("Click to view the full chart."));

            pnlMarket.Resize += new EventHandler(pnlMarket_Resize);

            return;
        }

        /// <summary>
        /// Arrange the controls after resizing
        /// </summary>
        void tsMarket_Resize(object sender, EventArgs e)
        {
            float width = (tsMarket.ClientSize.Width - tsbtnCharges.Width - 18) / 100.0F;
            tscbSymbol.Width = (int)(49 * width);
            tscbPeriod.Width = (int)(51 * width);
        }

        /// <summary>
        /// Arrange the controls after resizing
        /// </summary>
        void pnlMarket_Resize(object sender, EventArgs e)
        {
            smallIndicatorChart.Height = 2 * pnlMarket.ClientSize.Height / (Configs.ShowJournal ? 3 : 4);
        }

        /// <summary>
        /// Controls the ComboBoxes: cbxSymbol, cbxPeriod, cbxTax
        /// </summary>
        protected virtual void SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Shows the full chart after clicking on the small indicator chart
        /// </summary>
        void SmallIndicatorChart_MouseUp(object sender, MouseEventArgs e)
        {
            if (Data.IsData && Data.IsResult && e.Button == MouseButtons.Left)
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
        }

        /// <summary>
        /// Shows the market dynamic info on the Status Bar
        /// </summary>
        void SmallIndicatorChart_MouseMove(object sender, MouseEventArgs e)
        {
            Small_Indicator_Chart chart = (Small_Indicator_Chart)sender;
            ToolStripStatusLabelChartInfo = chart.CurrentBarInfo;
        }

        /// <summary>
        /// Deletes the market dynamic info from the Status Bar
        /// </summary>
        void SmallIndicatorChart_MouseLeave(object sender, EventArgs e)
        {
            ToolStripStatusLabelChartInfo = string.Empty;
        }
    }
}
