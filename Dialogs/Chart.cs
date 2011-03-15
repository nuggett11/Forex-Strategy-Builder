// Chart class 
// Part of Forex Strategy Builde
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    // Chart ToolStrip Buttons 
    public enum ChartButtons
    {
        Grid, Cross,
        Volume, Orders, PositionLots, PositionPrice, AmbiguousBars,
        Indicators, BalanceEquity, FloatingPL,
        ZoomIn, ZoomOut, TrueCharts,
        DInfoUp, DInfoDwn, DynamicInfo
    }

    /// <summary>
    /// Class Indicator Chart : Form
    /// </summary>
    public class Chart : Form
    {
        bool isDEBUG = false;

        Panel      pnlCharts;
        Panel      pnlInfo;
        Panel      pnlPrice;
        Panel      pnlFloatingPLChart;
        Panel      pnlBalanceChart;
        Panel[]    pnlInd;
        Splitter[] splitterInd;
        Splitter   spliterFloatingPL;
        Splitter   spliterBalance;
        HScrollBar scroll;
        ToolTip    mouseTips;

        ToolStrip tsChartButtons;
        ToolStripButton[] aChartButtons;

        string   chartTitle;
        int      indPanels;
        int      chartBars;
        int      chartWidth;
        int      firstBar;
        int      lastBar;
        double   maxPrice;
        double   minPrice;
        double   YScale;
        string[] asInfoTitle;
        string[] asInfoValue;
        int[]    aiInfoType; // 0 - text; 1 - Idicator; 
        bool[]   repeatedIndicator;
        int      mouseX;
        int      mouseY;
        int      mouseXOld;
        int      mouseYOld;
        bool     isMouseInPriceChart;
        bool     isMouseInIndicatorChart;
        int      barOld;
        int      posCount = 0;

        int barPixels = 8;
        int verticalScale = 1;

        bool isGridShown;
        bool isCrossShown;
        bool isVolumeShown;
        bool isOrdersShown;
        bool isPositionLotsShown;
        bool isPositionPriceShown;
        bool isAmbiguousBarsShown;
        bool isIndicatorsShown;
        bool isBalanceEquityShown;
        bool isFloatingPLShown;
        bool isInfoPanelShown;
        bool isDynInfoShown;
        bool isTrueChartsShown;
        bool isCandleChart = true;

        public int BarPixels 
        { 
            get { return barPixels; }
            set { barPixels = value;}
        }
        public bool ShowInfoPanel 
        {
            get { return isInfoPanelShown; }
            set { isInfoPanelShown = value; }
        }
        public bool ShowGrid 
        { 
            get { return isGridShown; }
            set { isGridShown = value; }
        }
        public bool ShowCross
        { 
            get { return isCrossShown; } 
            set { isCrossShown = value; }
        }
        public bool ShowVolume 
        { 
            get { return isVolumeShown; }
            set { isVolumeShown = value; }
        }
        public bool ShowPositionLots
        { 
            get { return isPositionLotsShown; }
            set { isPositionLotsShown = value; }
        }
        public bool ShowOrders
        { 
            get { return isOrdersShown; }
            set { isOrdersShown = value; }
        }
        public bool ShowDynInfo 
        { 
            get { return isDynInfoShown; }
            set { isDynInfoShown = value; } 
        }
        public bool ShowPositionPrice
        { 
            get { return isPositionPriceShown; }
            set { isPositionPriceShown = value; } 
        }
        public bool ShowBalanceEquity 
        {
            get { return isBalanceEquityShown; }
            set { isBalanceEquityShown = value; } 
        }
        public bool ShowFloatingPL 
        {
            get { return isFloatingPLShown; }
            set { isFloatingPLShown = value; }
        }

        public bool ShowIndicators 
        {
            get { return isIndicatorsShown; }
            set { isIndicatorsShown = value; } 
        }

        public bool ShowAmbiguousBars
        {
            get { return isAmbiguousBarsShown; }
            set { isAmbiguousBarsShown = value; }
        }

        public bool TrueCharts
        {
            get { return isTrueChartsShown; }
            set { isTrueChartsShown = value; }
        }

        int  infoRows;	         // Dynamic info rows
        int  XDynInfoCol2;       // Dynamic info second column X
        int  dynInfoWidth;       // Dynamic info width
        bool isDrawDinInfo;      // Draw or not
        int  dynInfoScrollValue; // Dynamic info vertical scrolling position

        int spcBottom;		// pnlPrice bottom margin
        int spcTop;			// pnlPrice top margin
        int spcLeft;		// pnlPrice left margin
        int spcRight;		// pnlPrice right margin

        int XLeft;			// pnlPrice left coordinate
        int XRight;		    // pnlPrice right coordinate
        int YTop;			// pnlPrice top coordinate
        int YBottom;		// pnlPrice bottom coordinate
        int YBottomText;	// pnlPrice bottom coordinate for date text

        int    maxVolume;  // Max Volume in the chart
        double YVolScale;  // The scale for drawing the Volume

        bool isKeyCtrlPressed = false;

        Font font;
        Font fontDI;    // Font for Dynamic info
        Font fontDIInd; // Font for Dynamic info Indicators
        Size szDate;
        Size szDateL;
        Size szPrice;
        Size szInfoHelp;

        Brush brushBack;
        Brush brushFore;
        Brush brushLabelBkgrd;
        Brush brushLabelFore;
        Brush brushDynamicInfo;
        Brush brushDIIndicator;
        Brush brushEvenRows;
        Brush brushTradeLong;
        Brush brushTradeShort;
        Brush brushTradeClose;
        Brush brushBarWhite;
        Brush brushBarBlack;
        Brush brushSignalRed;

        Pen penCross;
        Pen penGrid;
        Pen penGridSolid;
        Pen penAxes;
        Pen penTradeLong;
        Pen penTradeShort;
        Pen penTradeClose;
        Pen penVolume;
        Pen penBarBorder;
        Pen penBarThick;

        Color colorBarWhite1;
        Color colorBarWhite2;
        Color colorBarBlack1;
        Color colorBarBlack2;

        Color colorLongTrade1;
        Color colorLongTrade2;
        Color colorShortTrade1;
        Color colorShortTrade2;
        Color colorClosedTrade1;
        Color colorClosedTrade2;

// ------------------------------------------------------------
        /// <summary>
        /// The dafault constructor.
        /// </summary>
        public Chart()
        {
            Text = Language.T("Chart") + " " + Data.Symbol + " " + Data.PeriodString + " - " + Data.ProgramName;
            Icon = Data.Icon;
            BackColor = LayoutColors.ColorFormBack;

            pnlCharts = new Panel();
            pnlCharts.Parent  = this;
            pnlCharts.Dock    = DockStyle.Fill;

            pnlInfo = new Panel();
            pnlInfo.Parent    = this;
            pnlInfo.BackColor = LayoutColors.ColorControlBack;
            pnlInfo.Dock      = DockStyle.Right;
            pnlInfo.Resize   += new EventHandler(PnlInfo_Resize);
            pnlInfo.Paint    += new PaintEventHandler(PnlInfo_Paint);

            isInfoPanelShown    = true;
            dynInfoScrollValue = 0;

            font = new Font(Font.FontFamily, Font.Size);

            // Dynamic info fonts
            fontDI = font;
            fontDIInd = new Font(Font.FontFamily, 10);

            Graphics g = CreateGraphics();

            szDate  = g.MeasureString("99/99 99:99"   , font).ToSize();
            szDateL = g.MeasureString("99/99/99 99:99", font).ToSize();
            szPrice = g.MeasureString("9.99999"       , font).ToSize();

            g.Dispose();

            spcTop    = font.Height;
            spcBottom = font.Height * 8 / 5;
            spcLeft   = 2;
            spcRight  = szPrice.Width + 4;

            brushBack        = new SolidBrush(LayoutColors.ColorChartBack);
            brushFore        = new SolidBrush(LayoutColors.ColorChartFore);
            brushLabelBkgrd  = new SolidBrush(LayoutColors.ColorLabelBack);
            brushLabelFore   = new SolidBrush(LayoutColors.ColorLabelText);
            brushDynamicInfo = new SolidBrush(LayoutColors.ColorControlText);
            brushDIIndicator = new SolidBrush(LayoutColors.ColorSlotIndicatorText);
            brushEvenRows    = new SolidBrush(LayoutColors.ColorEvenRowBack);
            brushTradeLong   = new SolidBrush(LayoutColors.ColorTradeLong);
            brushTradeShort  = new SolidBrush(LayoutColors.ColorTradeShort);
            brushTradeClose  = new SolidBrush(LayoutColors.ColorTradeClose);
            brushBarWhite    = new SolidBrush(LayoutColors.ColorBarWhite);
            brushBarBlack    = new SolidBrush(LayoutColors.ColorBarBlack);
            brushSignalRed   = new SolidBrush(LayoutColors.ColorSignalRed);

            penGrid       = new Pen(LayoutColors.ColorChartGrid);
            penGridSolid  = new Pen(LayoutColors.ColorChartGrid);
            penAxes       = new Pen(LayoutColors.ColorChartFore);
            penCross      = new Pen(LayoutColors.ColorChartCross);
            penVolume     = new Pen(LayoutColors.ColorVolume);
            penBarBorder  = new Pen(LayoutColors.ColorBarBorder);
            penBarThick   = new Pen(LayoutColors.ColorBarBorder, 3);
            penTradeLong  = new Pen(LayoutColors.ColorTradeLong);
            penTradeShort = new Pen(LayoutColors.ColorTradeShort);
            penTradeClose = new Pen(LayoutColors.ColorTradeClose);

            penGrid.DashStyle   = DashStyle.Dash;
            penGrid.DashPattern = new float[] { 4, 2 };

            colorBarWhite1 = Data.GetGradientColor(LayoutColors.ColorBarWhite,  30);
            colorBarWhite2 = Data.GetGradientColor(LayoutColors.ColorBarWhite, -30);
            colorBarBlack1 = Data.GetGradientColor(LayoutColors.ColorBarBlack,  30);
            colorBarBlack2 = Data.GetGradientColor(LayoutColors.ColorBarBlack, -30);

            colorLongTrade1   = Data.GetGradientColor(LayoutColors.ColorTradeLong,   30);
            colorLongTrade2   = Data.GetGradientColor(LayoutColors.ColorTradeLong,  -30);
            colorShortTrade1  = Data.GetGradientColor(LayoutColors.ColorTradeShort,  30);
            colorShortTrade2  = Data.GetGradientColor(LayoutColors.ColorTradeShort, -30);
            colorClosedTrade1 = Data.GetGradientColor(LayoutColors.ColorTradeClose,  30);
            colorClosedTrade2 = Data.GetGradientColor(LayoutColors.ColorTradeClose, -30);

            return;
        }

        /// <summary>
        /// After loading select the Scrollbar
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            pnlInfo.Visible   = isInfoPanelShown;
            pnlCharts.Padding = isInfoPanelShown ? new Padding(0, 0, 2, 0) : new Padding(0);

            if (isInfoPanelShown)
                Text = Language.T("Chart") + " " + Data.Symbol + " " + Data.PeriodString + " - " + Data.ProgramName;
            else
                Text = Data.ProgramName + @"   http://forexsb.com";

            SetupDynInfoWidth();
            SetupIndicatorPanels();
            SetupButtons();
            SetupDynamicInfo();
            SetupChartTitle();

            pnlCharts.Resize += new EventHandler(PnlBase_Resize);
            pnlPrice.Resize  += new EventHandler(PnlPrice_Resize);

            Size = new Size(720, 500);
            MinimumSize = new Size(500, 500);

            if (isInfoPanelShown)
            {
                GenerateDynamicInfo(lastBar);
                SetupDynamicInfo();
                isDrawDinInfo = true;
                pnlInfo.Invalidate();
            }

            scroll.Select();

            return;
        }

        /// <summary>
        /// Sets ups the chart's buttons.
        /// </summary>
        void SetupButtons()
        {
            tsChartButtons = new ToolStrip();
            tsChartButtons.Parent = pnlCharts;

            aChartButtons = new ToolStripButton[16];
            for (int i = 0; i < 16; i++)
            {
                aChartButtons[i] = new ToolStripButton();
                aChartButtons[i].Tag = (ChartButtons)i;
                aChartButtons[i].DisplayStyle = ToolStripItemDisplayStyle.Image;
                aChartButtons[i].Click += new EventHandler(ButtonChart_Click);
                tsChartButtons.Items.Add(aChartButtons[i]);
                if (i > 12)
                    aChartButtons[i].Alignment = ToolStripItemAlignment.Right;
                if (i == 1 || i == 6 || i == 9 || i == 11)
                    tsChartButtons.Items.Add(new ToolStripSeparator());
            }

            // Grid
            aChartButtons[(int)ChartButtons.Grid].Image = Properties.Resources.chart_grid;
            aChartButtons[(int)ChartButtons.Grid].ToolTipText = Language.T("Grid") + "   G";
            aChartButtons[(int)ChartButtons.Grid].Checked = isGridShown;

            // Cross
            aChartButtons[(int)ChartButtons.Cross].Image = Properties.Resources.chart_cross;
            aChartButtons[(int)ChartButtons.Cross].ToolTipText = Language.T("Cross") + "   C";
            aChartButtons[(int)ChartButtons.Cross].Checked = isCrossShown;

            // Volume
            aChartButtons[(int)ChartButtons.Volume].Image = Properties.Resources.chart_volume;
            aChartButtons[(int)ChartButtons.Volume].ToolTipText = Language.T("Volume") + "   V";
            aChartButtons[(int)ChartButtons.Volume].Checked = isVolumeShown;

            // Orders
            aChartButtons[(int)ChartButtons.Orders].Image = Properties.Resources.chart_entry_points;
            aChartButtons[(int)ChartButtons.Orders].ToolTipText = Language.T("Entry / Exit points") + "   O";
            aChartButtons[(int)ChartButtons.Orders].Checked = isOrdersShown;

            // Position lots
            aChartButtons[(int)ChartButtons.PositionLots].Image = Properties.Resources.chart_lots;
            aChartButtons[(int)ChartButtons.PositionLots].ToolTipText = Language.T("Position lots") + "   L";
            aChartButtons[(int)ChartButtons.PositionLots].Checked = isPositionLotsShown;

            // Position price
            aChartButtons[(int)ChartButtons.PositionPrice].Image = Properties.Resources.chart_pos_price;
            aChartButtons[(int)ChartButtons.PositionPrice].ToolTipText = Language.T("Corrected position price") + "   P";
            aChartButtons[(int)ChartButtons.PositionPrice].Checked = isPositionPriceShown;

            // Ambiguous Bars
            aChartButtons[(int)ChartButtons.AmbiguousBars].Image = Properties.Resources.chart_ambiguous_bars;
            aChartButtons[(int)ChartButtons.AmbiguousBars].ToolTipText = Language.T("Ambiguous bars mark") + "   M";
            aChartButtons[(int)ChartButtons.AmbiguousBars].Checked = isAmbiguousBarsShown;

            // Indicators
            aChartButtons[(int)ChartButtons.Indicators].Image = Properties.Resources.chart_indicators;
            aChartButtons[(int)ChartButtons.Indicators].ToolTipText = Language.T("Indicators chart") + "   D";
            aChartButtons[(int)ChartButtons.Indicators].Checked = isIndicatorsShown;

            // Balance Equity
            aChartButtons[(int)ChartButtons.BalanceEquity].Image = Properties.Resources.chart_balance_equity;
            aChartButtons[(int)ChartButtons.BalanceEquity].ToolTipText = Language.T("Balance / Equity chart") + "   B";
            aChartButtons[(int)ChartButtons.BalanceEquity].Checked = isBalanceEquityShown;

            // Floating P/L
            aChartButtons[(int)ChartButtons.FloatingPL].Image = Properties.Resources.chart_floating_pl;
            aChartButtons[(int)ChartButtons.FloatingPL].ToolTipText = Language.T("Floating P/L chart") + "   F";
            aChartButtons[(int)ChartButtons.FloatingPL].Checked = isFloatingPLShown;

            // Zoom in
            aChartButtons[(int)ChartButtons.ZoomIn].Image = Properties.Resources.chart_zoom_in;
            aChartButtons[(int)ChartButtons.ZoomIn].ToolTipText = Language.T("Zoom in") + "   +";

            // Zoom out
            aChartButtons[(int)ChartButtons.ZoomOut].Image = Properties.Resources.chart_zoom_out;
            aChartButtons[(int)ChartButtons.ZoomOut].ToolTipText = Language.T("Zoom out") + "   -";

            // True Charts
            aChartButtons[(int)ChartButtons.TrueCharts].Image       = Properties.Resources.chart_true_charts;
            aChartButtons[(int)ChartButtons.TrueCharts].Checked     = isTrueChartsShown;
            aChartButtons[(int)ChartButtons.TrueCharts].ToolTipText = Language.T("True indicator charts") + "   T";

            // Show dynamic info
            aChartButtons[(int)ChartButtons.DynamicInfo].Image = Properties.Resources.chart_dyninfo;
            aChartButtons[(int)ChartButtons.DynamicInfo].Checked = isInfoPanelShown;
            aChartButtons[(int)ChartButtons.DynamicInfo].ToolTipText = Language.T("Show / hide the info panel") + "   I";

            // Move Dyn Info Up
            aChartButtons[(int)ChartButtons.DInfoUp].Image = Properties.Resources.chart_dinfo_up;
            aChartButtons[(int)ChartButtons.DInfoUp].ToolTipText = Language.T("Scroll info upwards") + "   A, S";
            aChartButtons[(int)ChartButtons.DInfoUp].Visible = isInfoPanelShown;

            // Move Dyn Info Down
            aChartButtons[(int)ChartButtons.DInfoDwn].Image = Properties.Resources.chart_dinfo_down;
            aChartButtons[(int)ChartButtons.DInfoDwn].ToolTipText = Language.T("Scroll info downwards") + "   Z, X";
            aChartButtons[(int)ChartButtons.DInfoDwn].Visible = isInfoPanelShown;

            return;
        }

        /// <summary>
        /// Create and sets the indicator panels
        /// <summary>
        void SetupIndicatorPanels()
        {
            pnlPrice = new Panel();
            pnlPrice.Parent    = pnlCharts;
            pnlPrice.Dock      = DockStyle.Fill;
            pnlPrice.BackColor = LayoutColors.ColorChartBack;
            pnlPrice.MouseLeave       += new EventHandler(PnlPrice_MouseLeave);
            pnlPrice.MouseDoubleClick += new MouseEventHandler(PnlPrice_MouseDoubleClick);
            pnlPrice.MouseMove        += new MouseEventHandler(PnlPrice_MouseMove);
            pnlPrice.MouseDown        += new MouseEventHandler(Panel_MouseDown);
            pnlPrice.MouseUp          += new MouseEventHandler(Panel_MouseUp);
            pnlPrice.Paint            += new PaintEventHandler(PnlPrice_Paint);

            // Indicator panels
            indPanels = 0;
            string[] asIndicatorTexts = new string[Data.Strategy.Slots];
            for (int iSlot = 0; iSlot < Data.Strategy.Slots; iSlot++)
            {
                Indicator indicator = Indicator_Store.ConstructIndicator(Data.Strategy.Slot[iSlot].IndicatorName, Data.Strategy.Slot[iSlot].SlotType);
                indicator.IndParam = Data.Strategy.Slot[iSlot].IndParam;
                asIndicatorTexts[iSlot] = indicator.ToString();
                indPanels += Data.Strategy.Slot[iSlot].SeparatedChart ? 1 : 0;
            }

            // Repeated indicators
            repeatedIndicator = new bool[Data.Strategy.Slots];
            for (int iSlot = 0; iSlot < Data.Strategy.Slots; iSlot++)
            {
                repeatedIndicator[iSlot] = false;
                for (int i = 0; i < iSlot; i++)
                    repeatedIndicator[iSlot] = asIndicatorTexts[iSlot] == asIndicatorTexts[i];
            }
            
            pnlInd      = new Panel[indPanels];
            splitterInd = new Splitter [indPanels];
            for (int i = 0; i < indPanels; i++)
            {
                splitterInd[i] = new Splitter();
                splitterInd[i].BorderStyle = BorderStyle.None;
                splitterInd[i].Dock        = DockStyle.Bottom;
                splitterInd[i].Height      = 2;

                pnlInd[i] = new Panel();
                pnlInd[i].Dock        = DockStyle.Bottom;
                pnlInd[i].BackColor   = LayoutColors.ColorControlBack;
                pnlInd[i].Paint      += new PaintEventHandler(PnlInd_Paint);
                pnlInd[i].Resize     += new EventHandler(PnlInd_Resize);
                pnlInd[i].MouseMove  += new MouseEventHandler(SepChart_MouseMove);
                pnlInd[i].MouseLeave += new EventHandler(SepChart_MouseLeave);
                pnlInd[i].MouseDown  += new MouseEventHandler(Panel_MouseDown);
                pnlInd[i].MouseUp    += new MouseEventHandler(Panel_MouseUp);
            }

            int iIndex = 0;
            for (int iSlot = 0; iSlot < Data.Strategy.Slots; iSlot++)
            {
                if (!Data.Strategy.Slot[iSlot].SeparatedChart) continue;
                pnlInd[iIndex].Tag = iSlot;
                iIndex++;
            }

            for (int i = 0; i < indPanels && isIndicatorsShown; i++)
            {
                splitterInd[i].Parent = pnlCharts;
                pnlInd[i].Parent      = pnlCharts;
            }

            // Balance chart
            spliterBalance = new Splitter();
            spliterBalance.BorderStyle = BorderStyle.None;
            spliterBalance.Dock        = DockStyle.Bottom;
            spliterBalance.Height      = 2;

            pnlBalanceChart = new Panel();
            pnlBalanceChart.Dock        = DockStyle.Bottom;
            pnlBalanceChart.BackColor   = LayoutColors.ColorChartBack;
            pnlBalanceChart.Paint      += new PaintEventHandler(PnlBalance_Paint);
            pnlBalanceChart.Resize     += new EventHandler(PnlBalance_Resize);
            pnlBalanceChart.MouseMove  += new MouseEventHandler(SepChart_MouseMove);
            pnlBalanceChart.MouseLeave += new EventHandler(SepChart_MouseLeave);
            pnlBalanceChart.MouseDown  += new MouseEventHandler(Panel_MouseDown);
            pnlBalanceChart.MouseUp    += new MouseEventHandler(Panel_MouseUp);

            if (isBalanceEquityShown)
            {
                spliterBalance.Parent  = pnlCharts;
                pnlBalanceChart.Parent = pnlCharts;
            }

            // Floating Profit Loss chart
            spliterFloatingPL = new Splitter();
            spliterFloatingPL.BorderStyle = BorderStyle.None;
            spliterFloatingPL.Dock        = DockStyle.Bottom;
            spliterFloatingPL.Height      = 2;

            pnlFloatingPLChart = new Panel();
            pnlFloatingPLChart.Dock        = DockStyle.Bottom;
            pnlFloatingPLChart.BackColor   = LayoutColors.ColorChartBack;
            pnlFloatingPLChart.Paint      += new PaintEventHandler(PnlFloatingPL_Paint);
            pnlFloatingPLChart.Resize     += new EventHandler(PnlFloatingPL_Resize);
            pnlFloatingPLChart.MouseMove  += new MouseEventHandler(SepChart_MouseMove);
            pnlFloatingPLChart.MouseLeave += new EventHandler(SepChart_MouseLeave);
            pnlFloatingPLChart.MouseDown  += new MouseEventHandler(Panel_MouseDown);
            pnlFloatingPLChart.MouseUp    += new MouseEventHandler(Panel_MouseUp);

            if (isFloatingPLShown)
            {
                spliterFloatingPL.Parent  = pnlCharts;
                pnlFloatingPLChart.Parent = pnlCharts;
            }

            scroll = new HScrollBar();
            scroll.Parent        = pnlCharts;
            scroll.Dock          = DockStyle.Bottom;
            scroll.TabStop       = true;
            scroll.Minimum       = Data.FirstBar;
            scroll.Maximum       = Data.Bars - 1;
            scroll.SmallChange   = 1;
            scroll.ValueChanged += new EventHandler(Scroll_ValueChanged);
            scroll.MouseWheel   += new MouseEventHandler(Scroll_MouseWheel);
            scroll.KeyUp        += new KeyEventHandler(Scroll_KeyUp);
            scroll.KeyDown      += new KeyEventHandler(Scroll_KeyDown);

            mouseTips = new ToolTip();
            mouseTips.IsBalloon      = true;
            mouseTips.BackColor      = LayoutColors.ColorControlBack;
            mouseTips.ForeColor      = LayoutColors.ColorControlText;
            mouseTips.AutomaticDelay = 0;
            mouseTips.ReshowDelay    = 0;
            mouseTips.UseFading      = true;

            return;
        }

        /// <summary>
        /// Sets the chart's parametes.
        /// </summary>
        void SetPriceChartParam()
        {
            chartBars = chartWidth / barPixels;
            chartBars = Math.Min(chartBars, Data.Bars - Data.FirstBar);
            firstBar  = Math.Max(Data.FirstBar, Data.Bars - chartBars);
            firstBar  = Math.Min(firstBar, Data.Bars - 1);
            lastBar   = Math.Max(firstBar + chartBars - 1, firstBar);

            scroll.Value       = firstBar;
            scroll.LargeChange = Math.Max(chartBars, 1);

            return;
        }

        /// <summary>
        /// Sets the indicator chart title
        /// </summary>
        void SetupChartTitle()
        {
            // Chart title
            chartTitle = Data.Symbol + "  " + Data.PeriodString + " " + Data.Strategy.StrategyName;

            if (!isIndicatorsShown) return;

            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                if (Data.Strategy.Slot[slot].SeparatedChart) continue;

                bool isChart = false;
                for (int iComp = 0; iComp < Data.Strategy.Slot[slot].Component.Length; iComp++)
                {
                    if (Data.Strategy.Slot[slot].Component[iComp].ChartType != IndChartType.NoChart)
                    {
                        isChart = true;
                        break;
                    }
                }
                if (isChart)
                {
                    Indicator indicator = Indicator_Store.ConstructIndicator(Data.Strategy.Slot[slot].IndicatorName, Data.Strategy.Slot[slot].SlotType);
                    indicator.IndParam = Data.Strategy.Slot[slot].IndParam;
                    if (!chartTitle.Contains(indicator.ToString()))
                        chartTitle += Environment.NewLine + indicator.ToString();
                }
            }

            return;
        }

        /// <summary>
        /// Sets the sizes of the panels after resizing.
        /// </summary>
        void PnlBase_Resize(object sender, EventArgs e)
        {
            SetAllPanelsHeight();
            SetPriceChartParam();
            dynInfoScrollValue = 0;

            return;
        }

        /// <summary>
        /// Calculates the panels' height
        /// </summary>
        void SetAllPanelsHeight()
        {
            int iPanelNumber = isIndicatorsShown ? indPanels : 0;
            iPanelNumber += isFloatingPLShown ? 1 : 0;
            iPanelNumber += isBalanceEquityShown ? 1 : 0;

            int iAvailableHeight = pnlCharts.ClientSize.Height - tsChartButtons.Height - scroll.Height - iPanelNumber * 4;

            int iPnlIndHeight = iAvailableHeight / (2 + iPanelNumber);

            for (int i = 0; i < indPanels && isIndicatorsShown; i++)
                pnlInd[i].Height = iPnlIndHeight;

            if(isFloatingPLShown)
                pnlFloatingPLChart.Height = iPnlIndHeight;

            if(isBalanceEquityShown)
                pnlBalanceChart.Height = iPnlIndHeight;

            return;
        }

        /// <summary>
        /// Sets the parameters after resizing of the PnlPrice.
        /// </summary>
        void PnlPrice_Resize(object sender, EventArgs e)
        {
            XLeft       = spcLeft;
            XRight      = pnlPrice.ClientSize.Width - spcRight;
            chartWidth  = Math.Max(XRight - XLeft, 0);
            YTop        = spcTop;
            YBottom     = pnlPrice.ClientSize.Height - spcBottom;
            YBottomText = pnlPrice.ClientSize.Height - spcBottom * 5 / 8 - 4;
            pnlPrice.Invalidate();

            return;
        }

        /// <summary>
        /// Invalidates the panels
        /// </summary>
        void PnlInd_Resize(object sender, EventArgs e)
        {
            if (!isIndicatorsShown) return;
            ((Panel)sender).Invalidate();

            return;
        }

        /// <summary>
        /// Invalidates the panel
        /// </summary>
        void PnlBalance_Resize(object sender, EventArgs e)
        {
            if (!isBalanceEquityShown) return;

            ((Panel)sender).Invalidate();

            return;
        }

        /// <summary>
        /// Invalidates the panel
        /// </summary>
        void PnlFloatingPL_Resize(object sender, EventArgs e)
        {
            if (!isFloatingPLShown) return;

            ((Panel)sender).Invalidate();

            return;
        }

        /// <summary>
        /// Paints the panel PnlPrice
        /// </summary>
        void PnlPrice_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.Clear(LayoutColors.ColorChartBack);

            if (chartBars == 0) return;
            
            // Searching the min and the max price and volume
            maxPrice  = double.MinValue;
            minPrice  = double.MaxValue;
            maxVolume = int.MinValue;
            for (int bar = firstBar; bar <= lastBar; bar++)
            {
                if (Data.High[bar] > maxPrice) maxPrice = Data.High[bar];
                if (Data.Low[bar] < minPrice)  minPrice = Data.Low[bar];
                if (Data.Volume[bar] > maxVolume) maxVolume = Data.Volume[bar];
            }

            double pricePixel = (maxPrice - minPrice) / (YBottom - YTop);
            if (isVolumeShown)
                minPrice -= pricePixel * 30;
            else if (isPositionLotsShown)
                minPrice -= pricePixel * 10;

            maxPrice += pricePixel * verticalScale;
            minPrice -= pricePixel * verticalScale;

            // Grid
            int countLabels = (int)Math.Max((YBottom - YTop) / 30, 1);
            double deltaPoint = (Data.InstrProperties.Digits == 5 || Data.InstrProperties.Digits == 3) ? Data.InstrProperties.Point * 100 : Data.InstrProperties.Point * 10;
            double deltaLabel = Math.Max(Math.Round((maxPrice - minPrice) / countLabels, Data.InstrProperties.Point < 0.001 ? 3 : 1), deltaPoint);
            minPrice = Math.Round(minPrice, Data.InstrProperties.Point < 0.001 ? 3 : 1) - deltaPoint;
            countLabels = (int)Math.Ceiling((maxPrice - minPrice) / deltaLabel);
            maxPrice  = minPrice + countLabels * deltaLabel;
            YScale    = (YBottom - YTop) / (countLabels * deltaLabel);
            YVolScale = maxVolume > 0 ? 40.0f / maxVolume : 0f; // 40 - the highest volume line
          
            // Price labels
            for (double label = minPrice; label <= maxPrice + Data.InstrProperties.Point; label += deltaLabel)
            {
                int iLabelY = (int)Math.Round(YBottom - (label - minPrice) * YScale);
                g.DrawString(label.ToString(Data.FF), Font, brushFore, XRight, iLabelY - Font.Height / 2 - 1);
                if (isGridShown || label == minPrice)
                    g.DrawLine(penGrid, spcLeft, iLabelY, XRight, iLabelY);
                else
                    g.DrawLine(penGrid, XRight - 5, iLabelY, XRight, iLabelY);
            }
            // Date labels
            for (int iVertLineBar = lastBar; iVertLineBar > firstBar; iVertLineBar -= (int)Math.Round((szDate.Width + 10.0) / barPixels + 1))
            {
                int iXVertLine = (iVertLineBar - firstBar) * barPixels + spcLeft + barPixels / 2 - 1;
                if (isGridShown)
                    g.DrawLine(penGrid, iXVertLine, YTop, iXVertLine, YBottom + 2);
                string date = String.Format("{0} {1}", Data.Time[iVertLineBar].ToString(Data.DFS), Data.Time[iVertLineBar].ToString("HH:mm"));
                g.DrawString(date, font, brushFore, iXVertLine - szDate.Width / 2, YBottomText);
            }

            // Cross
            if (isCrossShown && mouseX > XLeft - 1 && mouseX < XRight  + 1)
            {
                Point point;
                Rectangle rec;
                int crossBar;

                crossBar = (mouseX - spcLeft) / barPixels;
                crossBar = Math.Max(0, crossBar);
                crossBar = Math.Min(chartBars - 1, crossBar);
                crossBar += firstBar;
                crossBar = Math.Min(Data.Bars - 1, crossBar);

                // Vertical positions
                point = new Point(mouseX - szDateL.Width / 2, YBottomText);
                rec   = new Rectangle(point, szDateL);

                // Vertical line
                if (isMouseInPriceChart && mouseY > YTop - 1 && mouseY < YBottom + 1)
                {
                    g.DrawLine(penCross, mouseX, YTop, mouseX, mouseY - 10);
                    g.DrawLine(penCross, mouseX, mouseY + 10, mouseX, YBottomText);
                }
                else if (isMouseInPriceChart || isMouseInIndicatorChart)
                {
                    g.DrawLine(penCross, mouseX, YTop, mouseX, YBottomText);
                }

                // Date Window
                if (isMouseInPriceChart || isMouseInIndicatorChart)
                {
                    g.FillRectangle(brushLabelBkgrd, rec);
                    g.DrawRectangle(penCross, rec);
                    string sDate = Data.Time[crossBar].ToString(Data.DF) + " " + Data.Time[crossBar].ToString("HH:mm");
                    g.DrawString(sDate, font, brushLabelFore, point);
                }

                if (isMouseInPriceChart && mouseY > YTop - 1 && mouseY < YBottom + 1)
                {
                    //Horizontal positions
                    point = new Point(XRight, mouseY - szPrice.Height / 2);
                    rec   = new Rectangle(point, szPrice);
                    // Horisontal line
                    g.DrawLine(penCross, XLeft, mouseY, mouseX - 10, mouseY);
                    g.DrawLine(penCross, mouseX + 10, mouseY, XRight, mouseY);
                    // Price Window
                    g.FillRectangle(brushLabelBkgrd, rec);
                    g.DrawRectangle(penCross, rec);
                    string sPrice = ((YBottom - mouseY) / YScale + minPrice).ToString(Data.FF);
                    g.DrawString(sPrice, font, brushLabelFore, point);
                }
            }

            // Draws Volume, Lots and Bars
            for (int bar = firstBar; bar <= lastBar; bar++)
            {
                int x       = (bar - firstBar) * barPixels + spcLeft;
                int yOpen   = (int)Math.Round(YBottom - (Data.Open[bar]   - minPrice) * YScale);
                int yHigh   = (int)Math.Round(YBottom - (Data.High[bar]   - minPrice) * YScale);
                int yLow    = (int)Math.Round(YBottom - (Data.Low[bar]    - minPrice) * YScale);
                int yClose  = (int)Math.Round(YBottom - (Data.Close[bar]  - minPrice) * YScale);
                int yVolume = (int)Math.Round(YBottom -  Data.Volume[bar] * YVolScale);

                // Draw the volume
                if (isVolumeShown && yVolume != YBottom)
                    g.DrawLine(penVolume, x + barPixels / 2 - 1, yVolume, x + barPixels / 2 - 1, YBottom);

                // Draw position lots
                if (isPositionLotsShown && Backtester.IsPos(bar))
                {
                    int iPosHight = (int)Math.Round(Math.Max(Backtester.SummaryLots(bar) * 2, 2));
                    int iPosY     = YBottom - iPosHight + 1;

                    if (Backtester.SummaryDir(bar) == PosDirection.Long)
                    {   // Long
                        Rectangle rect = new Rectangle(x - 1, iPosY, barPixels + 1, iPosHight);
                        LinearGradientBrush lgBrush = new LinearGradientBrush(rect, colorLongTrade1, colorLongTrade2, 0f);
                        rect = new Rectangle(x, iPosY, barPixels - 1, iPosHight);
                        g.FillRectangle(lgBrush, rect);
                    }
                    else if (Backtester.SummaryDir(bar) == PosDirection.Short)
                    {   // Short
                        Rectangle rect = new Rectangle(x - 1, iPosY, barPixels + 1, iPosHight);
                        LinearGradientBrush lgBrush = new LinearGradientBrush(rect, colorShortTrade1, colorShortTrade2, 0f);
                        rect = new Rectangle(x, iPosY, barPixels - 1, iPosHight);
                        g.FillRectangle(lgBrush, rect);
                    }
                    else
                    {   // Closed
                        Rectangle rect = new Rectangle(x - 1, 2, barPixels + 1, 2);
                        LinearGradientBrush lgBrush = new LinearGradientBrush(rect, colorClosedTrade1, colorClosedTrade2, 0f);
                        rect = new Rectangle(x, YBottom - 1, barPixels - 1, 2);
                        g.FillRectangle(lgBrush, rect);
                    }
                }

                // Draw the bar
                if (isCandleChart)
                {
                    if (barPixels < 25)
                        g.DrawLine(penBarBorder, x + barPixels / 2 - 1, yLow, x + barPixels / 2 - 1, yHigh);
                    else
                        g.DrawLine(penBarThick, x + barPixels / 2 - 1, yLow, x + barPixels / 2 - 1, yHigh);

                    if (barPixels == 2)
                        g.DrawLine(penBarBorder, x, yClose, x + 1, yClose);
                    else
                    {
                        if (yClose < yOpen)
                        {   // White bar
                            Rectangle rect = new Rectangle(x, yClose, barPixels - 2, yOpen - yClose);
                            LinearGradientBrush lgBrush = new LinearGradientBrush(rect, colorBarWhite1, colorBarWhite2, 5f);
                            g.FillRectangle(lgBrush, rect);
                            g.DrawRectangle(penBarBorder, x, yClose, barPixels - 2, yOpen - yClose);
                        }
                        else if (yClose > yOpen)
                        {   // Black bar
                            Rectangle rect = new Rectangle(x, yOpen, barPixels - 2, yClose - yOpen);
                            LinearGradientBrush lgBrush = new LinearGradientBrush(rect, colorBarBlack1, colorBarBlack2, 5f);
                            g.FillRectangle(lgBrush, rect);
                            g.DrawRectangle(penBarBorder, rect);
                        }
                        else
                        {   // Cross
                            if (barPixels < 25)
                                g.DrawLine(penBarBorder, x, yClose, x + barPixels - 2, yClose);
                            else
                                g.DrawLine(penBarThick, x, yClose, x + barPixels - 2, yClose);
                        }
                    }
                }
                else
                {
                    if (barPixels == 2)
                    {
                        g.DrawLine(penBarBorder, x, yClose, x + 1, yClose);
                        g.DrawLine(penBarBorder, x + barPixels / 2 - 1, yLow, x + barPixels / 2 - 1, yHigh);
                    }
                    else if (barPixels <= 16)
                    {
                        g.DrawLine(penBarBorder, x + barPixels / 2 - 1, yLow, x + barPixels / 2 - 1, yHigh);
                        if (yClose != yOpen)
                        {
                            g.DrawLine(penBarBorder, x, yOpen, x + barPixels / 2 - 1, yOpen);
                            g.DrawLine(penBarBorder, x + barPixels / 2 - 1, yClose, x + barPixels - 2, yClose);
                        }
                        else
                        {
                            g.DrawLine(penBarBorder, x, yClose, x + barPixels - 2, yClose);
                        }
                    }
                    else
                    {
                        g.DrawLine(penBarThick, x + barPixels / 2 - 1, yLow + 2, x + barPixels / 2 - 1, yHigh - 1);
                        if (yClose != yOpen)
                        {
                            g.DrawLine(penBarThick, x + 1, yOpen, x + barPixels / 2 - 1, yOpen);
                            g.DrawLine(penBarThick, x + barPixels / 2, yClose, x + barPixels - 2, yClose);
                        }
                        else
                        {
                            g.DrawLine(penBarThick, x, yClose, x + barPixels - 2, yClose);
                        }
                    }
                }
            }

            // Drawing the indicators in the chart
            g.SetClip(new RectangleF(0, YTop, XRight, YBottom - YTop));
            for (int slot = 0; slot < Data.Strategy.Slots && isIndicatorsShown; slot++)
            {
                if (Data.Strategy.Slot[slot].SeparatedChart || repeatedIndicator[slot]) continue;

                int cloudUp   = 0; // For Ichimoku and similar
                int cloudDown = 0; // For Ichimoku and similar

                bool isIndicatorValueAtClose = true;
                int  indicatorValueShift = 1;
                foreach (ListParam listParam in Data.Strategy.Slot[slot].IndParam.ListParam)
                    if (listParam.Caption == "Base price" && listParam.Text == "Open")
                    {
                        isIndicatorValueAtClose = false;
                        indicatorValueShift = 0;
                    }

                for (int comp = 0; comp < Data.Strategy.Slot[slot].Component.Length; comp++)
                {
                    Pen pen   = new Pen(Data.Strategy.Slot[slot].Component[comp].ChartColor);
                    Pen penTC = new Pen(Data.Strategy.Slot[slot].Component[comp].ChartColor);
                    penTC.DashStyle   = DashStyle.Dash;
                    penTC.DashPattern = new float[] { 2, 1 };

                    if (Data.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Line)
                    {   // Line
                        if (isTrueChartsShown)
                        {   // True Charts
                            Point[] point = new Point[lastBar - firstBar + 1];
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double value = Data.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = spcLeft + (bar - firstBar) * barPixels + indicatorValueShift * (barPixels - 2);
                                int y = (int)Math.Round(YBottom - (value - minPrice) * YScale);

                                if (value == 0)
                                    point[bar - firstBar] = point[Math.Max(bar - firstBar - 1, 0)];
                                else
                                    point[bar - firstBar] = new Point(x, y);
                            }

                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {   // All bars except the last one
                                int i = bar - firstBar;

                                // The indicator value point
                                g.DrawLine(pen, point[i].X - 1, point[i].Y, point[i].X + 1, point[i].Y);
                                g.DrawLine(pen, point[i].X, point[i].Y - 1, point[i].X, point[i].Y + 1);
                                
                                if (bar == firstBar && isIndicatorValueAtClose)
                                {   // First bar
                                    double value = Data.Strategy.Slot[slot].Component[comp].Value[bar - 1];
                                    int x = spcLeft + (bar - firstBar) * barPixels;
                                    int y = (int)Math.Round(YBottom - (value - minPrice) * YScale);

                                    int deltaY = Math.Abs(y - point[i].Y);
                                    if (barPixels > 3)
                                    {   // Horizontal part
                                        if (deltaY == 0)
                                            g.DrawLine(pen, x + 1, y, x + barPixels - 5, y);
                                        else if (deltaY < 3)
                                            g.DrawLine(pen, x + 1, y, x + barPixels - 4, y);
                                        else
                                            g.DrawLine(pen, x + 1, y, x + barPixels - 2, y);
                                    } 
                                    if (deltaY > 4)
                                    {   // Vertical part
                                        if (point[i].Y > y)
                                            g.DrawLine(penTC, x + barPixels - 2, y + 2, x + barPixels - 2, point[i].Y - 2);
                                        else
                                            g.DrawLine(penTC, x + barPixels - 2, y - 2, x + barPixels - 2, point[i].Y + 2);
                                    }
                                }
                                
                                if (bar < lastBar)
                                {
                                    int deltaY = Math.Abs(point[i + 1].Y - point[i].Y);
                                 
                                    if (barPixels > 3)
                                    {   // Horizontal part
                                        if (deltaY == 0)
                                            g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 3, point[i].Y);
                                        else if (deltaY < 3)
                                            g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 2, point[i].Y);
                                        else
                                            g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X, point[i].Y);
                                    } 
                                    if (deltaY > 4)
                                    {   // Vertical part
                                        if (point[i + 1].Y > point[i].Y)
                                            g.DrawLine(penTC, point[i + 1].X, point[i].Y + 2, point[i + 1].X, point[i + 1].Y - 2);
                                        else
                                            g.DrawLine(penTC, point[i + 1].X, point[i].Y - 2, point[i + 1].X, point[i + 1].Y + 2);
                                    }
                                }
                                
                                if (bar == lastBar && !isIndicatorValueAtClose && barPixels > 3)
                                {   // Last bar
                                    g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i].X + barPixels - 2, point[i].Y);
                                }
                            }
                        } 
                        else
                        {   // Regular Charts
                            Point[] aPoint = new Point[lastBar - firstBar + 1];
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double dValue = Data.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = spcLeft + (bar - firstBar) * barPixels + barPixels / 2 - 1;
                                int y = (int)Math.Round(YBottom - (dValue - minPrice) * YScale);

                                if (dValue == 0)
                                    aPoint[bar - firstBar] = aPoint[Math.Max(bar - firstBar - 1, 0)];
                                else
                                    aPoint[bar - firstBar] = new Point(x, y);
                            }
                            g.DrawLines(pen, aPoint);
                        }
                    }
                    else if (Data.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Dot)
                    {   // Dots
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double dValue = Data.Strategy.Slot[slot].Component[comp].Value[bar];
                            int x = (bar - firstBar) * barPixels + barPixels / 2 - 1 + spcLeft;
                            int y = (int)Math.Round(YBottom - (dValue - minPrice) * YScale);
                            if (barPixels == 2)
                                g.FillRectangle(pen.Brush, x, y, 1, 1);
                            else
                            {
                                g.DrawLine(pen, x - 1, y, x + 1, y);
                                g.DrawLine(pen, x, y - 1, x, y + 1);
                            }
                        }
                    }
                    else if (Data.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Level)
                    {   // Level
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double dValue = Data.Strategy.Slot[slot].Component[comp].Value[bar];
                            int x = (bar - firstBar) * barPixels + spcLeft;
                            int y = (int)Math.Round(YBottom - (dValue - minPrice) * YScale);
                            g.DrawLine(pen, x, y, x + barPixels - 1, y);
                        }
                    }
                    else if (Data.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.CloudUp)
                    {
                        cloudUp = comp;
                    }
                    else if (Data.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.CloudDown)
                    {
                        cloudDown = comp;
                    }
                }

                // Clouds
                if (cloudUp > 0 && cloudDown > 0)
                {
                    PointF[] apntUp   = new PointF[lastBar - firstBar + 1];
                    PointF[] apntDown = new PointF[lastBar - firstBar + 1];
                    for (int bar = firstBar; bar <= lastBar; bar++)
                    {
                        double dValueUp   = Data.Strategy.Slot[slot].Component[cloudUp].Value[bar];
                        double dValueDown = Data.Strategy.Slot[slot].Component[cloudDown].Value[bar];
                        apntUp[bar - firstBar].X = (bar - firstBar) * barPixels + barPixels / 2 - 1 + spcLeft;
                        apntUp[bar - firstBar].Y = (int)Math.Round(YBottom - (dValueUp - minPrice) * YScale);
                        apntDown[bar - firstBar].X = (bar - firstBar) * barPixels + barPixels / 2 - 1 + spcLeft;
                        apntDown[bar - firstBar].Y = (int)Math.Round(YBottom - (dValueDown - minPrice) * YScale);
                    }

                    GraphicsPath pathUp = new GraphicsPath();
                    pathUp.AddLine(new PointF(apntUp[0].X, 0), apntUp[0]);
                    pathUp.AddLines(apntUp);
                    pathUp.AddLine(apntUp[lastBar - firstBar], new PointF(apntUp[lastBar - firstBar].X, 0));
                    pathUp.AddLine(new PointF(apntUp[lastBar - firstBar].X, 0), new PointF(apntUp[0].X, 0));
                    
                    GraphicsPath pathDown = new GraphicsPath();
                    pathDown.AddLine(new PointF(apntDown[0].X, 0), apntDown[0]);
                    pathDown.AddLines(apntDown);
                    pathDown.AddLine(apntDown[lastBar - firstBar], new PointF(apntDown[lastBar - firstBar].X, 0));
                    pathDown.AddLine(new PointF(apntDown[lastBar - firstBar].X, 0), new PointF(apntDown[0].X, 0));

                    Color colorUp   = Color.FromArgb(50, Data.Strategy.Slot[slot].Component[cloudUp].ChartColor);
                    Color colorDown = Color.FromArgb(50, Data.Strategy.Slot[slot].Component[cloudDown].ChartColor);

                    Pen penUp   = new Pen(Data.Strategy.Slot[slot].Component[cloudUp].ChartColor);
                    Pen penDown = new Pen(Data.Strategy.Slot[slot].Component[cloudDown].ChartColor);

                    penUp.DashStyle   = DashStyle.Dash;
                    penDown.DashStyle = DashStyle.Dash;

                    Brush brushUp   = new SolidBrush(colorUp);
                    Brush brushDown = new SolidBrush(colorDown);

                    System.Drawing.Region regionUp = new Region(pathUp);
                    regionUp.Exclude(pathDown);
                    g.FillRegion(brushDown, regionUp);

                    System.Drawing.Region regionDown = new Region(pathDown);
                    regionDown.Exclude(pathUp);
                    g.FillRegion(brushUp, regionDown);

                    g.DrawLines(penUp,   apntUp);
                    g.DrawLines(penDown, apntDown);
                }

            }
            g.ResetClip();

            // Draws position price, deals and Ambiguous note
            for (int bar = firstBar; bar <= lastBar; bar++)
            {
                int x     = (bar - firstBar) * barPixels + spcLeft;
                int yHigh = (int)Math.Round(YBottom - (Data.High[bar] - minPrice) * YScale);

                // Draw the corrected position price
                for (int iPos = 0; iPos < Backtester.Positions(bar) && isPositionPriceShown; iPos++)
                {
                    int yPrice = (int)Math.Round(YBottom - (Backtester.SummaryPrice(bar) - minPrice) * YScale);

                    if (yPrice >= YBottom || yPrice <= YTop) continue;

                    if (Backtester.SummaryDir(bar) == PosDirection.Long)
                    {   // Long
                        g.DrawLine(penTradeLong, x, yPrice, x + barPixels - 2, yPrice);
                    }
                    else if (Backtester.SummaryDir(bar) == PosDirection.Short)
                    {   // Short
                        g.DrawLine(penTradeShort, x, yPrice, x + barPixels - 2, yPrice);
                    }
                    else if (Backtester.SummaryDir(bar) == PosDirection.Closed)
                    {   // Closed
                        g.DrawLine(penTradeClose, x, yPrice, x + barPixels - 2, yPrice);
                    }
                }

                //// Draw Permanent SL and TP
                //for (int iPos = 0; iPos < Backtester.Positions(bar) && isPositionPriceShown; iPos++)
                //{
                //    int yAbsSL = (int)Math.Round(YBottom - (Backtester.SummaryAbsoluteSL(bar) - minPrice) * YScale);
                //    int yAbsTP = (int)Math.Round(YBottom - (Backtester.SummaryAbsoluteTP(bar) - minPrice) * YScale);

                //    if (yAbsTP < YBottom && yAbsTP > YTop) 
                //        g.DrawLine(penTradeLong, x, yAbsTP, x + barPixels - 2, yAbsTP);
                //    if (yAbsSL < YBottom && yAbsSL > YTop)
                //        g.DrawLine(penTradeShort, x, yAbsSL, x + barPixels - 2, yAbsSL);
                //}

                // Draw the deals
                for (int iPos = 0; iPos < Backtester.Positions(bar) && isOrdersShown; iPos++)
                {
                    if (Backtester.PosTransaction(bar, iPos) == Transaction.Transfer) continue;

                    int yDeal = (int)Math.Round(YBottom - (Backtester.PosOrdPrice(bar, iPos) - minPrice) * YScale);

                    if (Backtester.PosDir(bar, iPos) == PosDirection.Long ||
                        Backtester.PosDir(bar, iPos) == PosDirection.Short)
                    {
                        if (Backtester.OrdFromNumb(Backtester.PosOrdNumb(bar, iPos)).OrdDir == OrderDirection.Buy)
                        {   // Buy
                            Pen pen = new Pen(brushTradeLong, 2);
                            if (barPixels < 8)
                            {
                                g.DrawLine(pen, x, yDeal, x + barPixels - 1, yDeal);
                            }
                            else if (barPixels == 8)
                            {
                                g.DrawLine(pen, x, yDeal, x + 4, yDeal);
                                pen.EndCap = LineCap.DiamondAnchor;
                                g.DrawLine(pen, x + 2, yDeal, x + 5, yDeal - 3);
                            }
                            else if (barPixels > 8)
                            {
                                int d = barPixels / 2 - 1;
                                int x1 = x + d;
                                int x2 = x + barPixels - 2;
                                g.DrawLine(pen, x,  yDeal, x1, yDeal);
                                g.DrawLine(pen, x1, yDeal, x2, yDeal - d);
                                g.DrawLine(pen, x2 + 1, yDeal - d + 1, x1 + d / 2 + 1, yDeal - d + 1);
                                g.DrawLine(pen, x2, yDeal - d, x2, yDeal - d / 2);
                            }
                        }
                        else
                        {   // Sell
                            Pen pen = new Pen(brushTradeShort, 2);
                            if (barPixels < 8)
                            {
                                g.DrawLine(pen, x, yDeal, x + barPixels - 1, yDeal);
                            }
                            else if (barPixels == 8)
                            {
                                g.DrawLine(pen, x, yDeal + 1, x + 4, yDeal + 1);
                                pen.EndCap = LineCap.DiamondAnchor;
                                g.DrawLine(pen, x + 2, yDeal, x + 5, yDeal + 3);
                            }
                            else if (barPixels > 8)
                            {
                                int d = barPixels / 2 - 1;
                                int x1 = x + d;
                                int x2 = x + barPixels - 2;
                                g.DrawLine(pen, x,  yDeal + 1, x1 + 1, yDeal + 1);
                                g.DrawLine(pen, x1, yDeal, x2, yDeal + d);
                                g.DrawLine(pen, x1 + d / 2 + 1, yDeal + d, x2, yDeal + d);
                                g.DrawLine(pen, x2, yDeal + d, x2, yDeal + d / 2 + 1);
                            }
                        }
                    }
                    else if (Backtester.PosDir(bar, iPos) == PosDirection.Closed)
                    {   // Close
                        Pen pen = new Pen(brushTradeClose, 2);
                        if (barPixels < 8)
                        {
                            g.DrawLine(pen, x, yDeal, x + barPixels - 1, yDeal);
                        }
                        else if (barPixels == 8)
                        {
                            g.DrawLine(pen, x, yDeal, x + 7, yDeal);
                            g.DrawLine(pen, x + 5, yDeal - 2, x + 5, yDeal + 2);
                        }
                        else if (barPixels > 8)
                        {
                            int d = barPixels / 2 - 1;
                            int x1 = x + d;
                            int x2 = x + barPixels - 3;
                            g.DrawLine(pen, x, yDeal, x1, yDeal);
                            g.DrawLine(pen, x1, yDeal + d / 2, x2, yDeal - d / 2);
                            g.DrawLine(pen, x1, yDeal - d / 2, x2, yDeal + d / 2);
                        }
                    }
                }

                // Ambiguous note
                if (isAmbiguousBarsShown && Backtester.BackTestEval(bar) == "Ambiguous")
                    g.DrawString("!", Font, brushSignalRed, x + barPixels / 2 - 4, yHigh - 20);
            }

            // Chart title
            g.DrawString(chartTitle, font, brushFore, spcLeft, 0);

            return;
        }

        /// <summary>
        /// Paints the panel PnlInd
        /// </summary>
        void PnlInd_Paint(object sender, PaintEventArgs e)
        {
            if (!isIndicatorsShown) return;

            Panel pnl = (Panel)sender;
            Graphics g = e.Graphics;
            
            int slot = (int)pnl.Tag;
            
            int topSpace    = font.Height / 2 + 2;
            int bottomSpace = font.Height / 2;

            double minValue = double.MaxValue;
            double maxValue = double.MinValue;
            
            g.Clear(LayoutColors.ColorChartBack);

            if (chartBars == 0) return;

            foreach(IndicatorComp component in Data.Strategy.Slot[slot].Component)
                if (component.ChartType != IndChartType.NoChart)
                    for (int bar = Math.Max(firstBar - 1, component.FirstBar); bar <= lastBar; bar++)
                    {
                        double value = component.Value[bar];
                        if (value > maxValue) maxValue = value;
                        if (value < minValue) minValue = value;
                    }

            minValue = Math.Min(minValue, Data.Strategy.Slot[slot].MinValue);
            maxValue = Math.Max(maxValue, Data.Strategy.Slot[slot].MaxValue);

            foreach (double value in Data.Strategy.Slot[slot].SpecValue)
                if (value == 0)
                {
                    minValue = Math.Min(minValue, 0);
                    maxValue = Math.Max(maxValue, 0);
                }

            double scale = (pnl.ClientSize.Height - topSpace - bottomSpace) / (Math.Max(maxValue - minValue, 0.0001));

            // Grid
            double label;
            int    labelY;
            String strFormat;
            double labelAbs;
            int    XGridRight = pnl.ClientSize.Width - spcRight + 2;
           
            label = 0; // Zero line
            int labelYZero = (int)Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue) * scale);
            if (label >= minValue && label <= maxValue)
            {
                labelAbs = Math.Abs(label);
                strFormat = labelAbs < 10 ? "F4" : labelAbs < 100 ? "F3" : labelAbs < 1000 ? "F2" : labelAbs < 10000 ? "F1" : "F0";
                g.DrawString(label.ToString(strFormat), font, brushFore, XRight, labelYZero - font.Height / 2 - 1);
                g.DrawLine(penGridSolid, spcLeft, labelYZero, XGridRight, labelYZero);
            }

            label = minValue; // Bottom line
            int labelYMin = (int)Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue) * scale);
            if (Math.Abs(labelYZero - labelYMin) >= font.Height)
            {
                labelAbs = Math.Abs(label);
                strFormat = labelAbs < 10 ? "F4" : labelAbs < 100 ? "F3" : labelAbs < 1000 ? "F2" : labelAbs < 10000 ? "F1" : "F0";
                g.DrawString(label.ToString(strFormat), font, brushFore, XRight, labelYMin - font.Height / 2 - 1);
                if (isGridShown)
                    g.DrawLine(penGrid, spcLeft, labelYMin, XGridRight, labelYMin);
                else
                    g.DrawLine(penGrid, XGridRight - 5, labelYMin, XGridRight, labelYMin);
            }

            label = maxValue; // Top line
            int labelYMax = (int)Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue) * scale);
            if (Math.Abs(labelYZero - labelYMax) >= font.Height)
            {
                labelAbs = Math.Abs(label);
                strFormat = labelAbs < 10 ? "F4" : labelAbs < 100 ? "F3" : labelAbs < 1000 ? "F2" : labelAbs < 10000 ? "F1" : "F0";
                g.DrawString(label.ToString(strFormat), font, brushFore, XRight, labelYMax - font.Height / 2 - 1);
                if (isGridShown)
                    g.DrawLine(penGrid, spcLeft, labelYMax, XGridRight, labelYMax);
                else
                    g.DrawLine(penGrid, XGridRight - 5, labelYMax, XGridRight, labelYMax);
            }

            if (Data.Strategy.Slot[slot].SpecValue != null)
                for (int i = 0; i < Data.Strategy.Slot[slot].SpecValue.Length; i++)
                {
                    label = Data.Strategy.Slot[slot].SpecValue[i];
                    if (label <= maxValue && label >= minValue)
                    {
                        labelY = (int)Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue) * scale);
                        if (Math.Abs(labelY - labelYZero) < font.Height) continue;
                        if (Math.Abs(labelY - labelYMin)  < font.Height) continue;
                        if (Math.Abs(labelY - labelYMax)  < font.Height) continue;
                        labelAbs = Math.Abs(label);
                        strFormat = labelAbs < 10 ? "F4" : labelAbs < 100 ? "F3" : labelAbs < 1000 ? "F2" : labelAbs < 10000 ? "F1" : "F0";
                        g.DrawString(label.ToString(strFormat), font, brushFore, XRight, labelY - font.Height / 2 - 1);
                        if (isGridShown)
                            g.DrawLine(penGrid, spcLeft, labelY, XGridRight, labelY);
                        else
                            g.DrawLine(penGrid, XGridRight - 5, labelY, XGridRight, labelY);
                    }
                }

            // Vertical line
            if (isGridShown)
            {
                string date = Data.Time[firstBar].ToString("dd.MM") + " " + Data.Time[firstBar].ToString("HH:mm");
                int dateWidth = (int)g.MeasureString(date, font).Width;
                for (int vertLineBar = lastBar; vertLineBar > firstBar; vertLineBar -= (int)Math.Round((dateWidth + 10.0) / barPixels + 1))
                {
                    int XVertLine = (vertLineBar - firstBar) * barPixels + barPixels / 2 - 1 + spcLeft;
                    g.DrawLine(penGrid, XVertLine, topSpace, XVertLine, pnl.ClientSize.Height - bottomSpace);
                }
            }

            bool isIndicatorValueAtClose = true;
            int  indicatorValueShift = 1;
            foreach (ListParam listParam in Data.Strategy.Slot[slot].IndParam.ListParam)
                if (listParam.Caption == "Base price" && listParam.Text == "Open")
                {
                    isIndicatorValueAtClose = false;
                    indicatorValueShift = 0;
                }

            // Indicator chart
            foreach (IndicatorComp component in Data.Strategy.Slot[slot].Component)
            {
                if (component.ChartType == IndChartType.Histogram)
                {   // Histogram
                    double zero = 0;
                    if (zero < minValue) zero = minValue;
                    if (zero > maxValue) zero = maxValue;
                    int y0 = (int)Math.Round(pnl.ClientSize.Height - 5 - (zero - minValue) * scale);
                    
                    Rectangle rect;
                    LinearGradientBrush lgBrush;
                    Pen penGreen = new Pen(LayoutColors.ColorTradeLong);
                    Pen penRed   = new Pen(LayoutColors.ColorTradeShort);
                    
                    bool isPrevBarGreen = false;

                    if (isTrueChartsShown)
                    {
                        if (isIndicatorValueAtClose)
                        {
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double value     = component.Value[bar - 1];
                                double prevValue = component.Value[bar - 2];
                                int x = spcLeft + (bar - firstBar) * barPixels + barPixels / 2 - 1;
                                int y = (int)Math.Round(pnl.ClientSize.Height - 7 - (value - minValue) * scale);

                                if (value > prevValue || value == prevValue && isPrevBarGreen)
                                {
                                    if (y != y0)
                                    {
                                        if (y > y0)
                                            g.DrawLine(penGreen, x, y0, x, y);
                                        else if (y < y0 - 2)
                                            g.DrawLine(penGreen, x, y0 - 2, x, y);
                                        isPrevBarGreen = true;
                                    }
                                }
                                else
                                {
                                    if (y != y0)
                                    {
                                        if (y > y0)
                                            g.DrawLine(penRed, x, y0, x, y);
                                        else if (y < y0 - 2)
                                            g.DrawLine(penRed, x, y0 - 2, x, y);
                                        isPrevBarGreen = false;
                                    }
                                }
                            }
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double value     = component.Value[bar];
                                double prevValue = component.Value[bar - 1];
                                int x = spcLeft + (bar - firstBar) * barPixels + barPixels - 2;
                                int y = (int)Math.Round(pnl.ClientSize.Height - 7 - (value - minValue) * scale);

                                if (value > prevValue || value == prevValue && isPrevBarGreen)
                                {
                                    g.DrawLine(penGreen, x, y + 1, x, y - 1);
                                    g.DrawLine(penGreen, x - 1, y, x + 1, y);
                                    isPrevBarGreen = true;
                                }
                                else
                                {
                                    g.DrawLine(penRed, x, y + 1, x, y - 1);
                                    g.DrawLine(penRed, x - 1, y, x + 1, y);
                                    isPrevBarGreen = false;
                                }
                            }

                        }
                        else
                        {
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double value     = component.Value[bar];
                                double prevValue = component.Value[bar - 1];
                                int x = spcLeft + (bar - firstBar) * barPixels + barPixels / 2 - 1;
                                int y = (int)Math.Round(pnl.ClientSize.Height - 7 - (value - minValue) * scale);

                                if (value > prevValue || value == prevValue && isPrevBarGreen)
                                {
                                    g.DrawLine(penGreen, x, y + 1, x, y - 1);
                                    g.DrawLine(penGreen, x - 1, y, x + 1, y);
                                    if (y != y0)
                                    {
                                        if (y > y0 + 3)
                                            g.DrawLine(penGreen, x, y0, x, y - 3);
                                        else if (y < y0 - 5)
                                            g.DrawLine(penGreen, x, y0 - 2, x, y + 3);
                                        isPrevBarGreen = true;
                                    }
                                }
                                else
                                {
                                    g.DrawLine(penRed, x, y + 1, x, y - 1);
                                    g.DrawLine(penRed, x - 1, y, x + 1, y);
                                    if (y != y0)
                                    {
                                        if (y > y0 + 3)
                                            g.DrawLine(penRed, x, y0, x, y - 3);
                                        else if (y < y0 - 5)
                                            g.DrawLine(penRed, x, y0 - 2, x, y + 3);
                                        isPrevBarGreen = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double value     = component.Value[bar];
                            double prevValue = component.Value[bar - 1];
                            int x = (bar - firstBar) * barPixels + spcLeft;
                            int y = (int)Math.Round(pnl.ClientSize.Height - 7 - (value - minValue) * scale);

                            if (value > prevValue || value == prevValue && isPrevBarGreen)
                            {
                                if (y > y0)
                                {
                                    rect = new Rectangle(x - 1, y0, barPixels + 1, y - y0);
                                    lgBrush = new LinearGradientBrush(rect, colorLongTrade1, colorLongTrade2, 0f);
                                    rect = new Rectangle(x, y0, barPixels - 1, y - y0);
                                }
                                else if (y < y0)
                                {
                                    rect = new Rectangle(x - 1, y, barPixels + 1, y0 - y);
                                    lgBrush = new LinearGradientBrush(rect, colorLongTrade1, colorLongTrade2, 0f);
                                    rect = new Rectangle(x, y, barPixels - 1, y0 - y);
                                }
                                else
                                    continue;
                                g.FillRectangle(lgBrush, rect);
                                isPrevBarGreen = true;
                            }
                            else
                            {
                                if (y > y0)
                                {
                                    rect = new Rectangle(x - 1, y0, barPixels + 1, y - y0);
                                    lgBrush = new LinearGradientBrush(rect, colorShortTrade1, colorShortTrade2, 0f);
                                    rect = new Rectangle(x, y0, barPixels - 1, y - y0);
                                }
                                else if (y < y0)
                                {
                                    rect = new Rectangle(x - 1, y, barPixels + 1, y0 - y);
                                    lgBrush = new LinearGradientBrush(rect, colorShortTrade1, colorShortTrade2, 0f);
                                    rect = new Rectangle(x, y, barPixels - 1, y0 - y);
                                }
                                else
                                    continue;
                                g.FillRectangle(lgBrush, rect);
                                isPrevBarGreen = false;
                            }
                        }
                    }
                }

                if (component.ChartType == IndChartType.Line)
                {   // Line
                    Pen pen   = new Pen(component.ChartColor);
                    Pen penTC = new Pen(component.ChartColor);
                    penTC.DashStyle   = DashStyle.Dash;
                    penTC.DashPattern = new float[] { 2, 1 };

                    int YIndChart = pnl.ClientSize.Height - 7;

                    if (isTrueChartsShown)
                    {   // True Charts
                        Point[] point = new Point[lastBar - firstBar + 1];
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double value = component.Value[bar];
                            int x = spcLeft + (bar - firstBar + indicatorValueShift) * barPixels - 2 * indicatorValueShift;
                            int y = (int)Math.Round(YIndChart - (value - minValue) * scale);

                            point[bar - firstBar] = new Point(x, y);
                        }

                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {   // All bars except the last one
                            int i = bar - firstBar;

                            // The indicator value point
                            g.DrawLine(pen, point[i].X - 1, point[i].Y, point[i].X + 1, point[i].Y);
                            g.DrawLine(pen, point[i].X, point[i].Y - 1, point[i].X, point[i].Y + 1);

                            if (bar == firstBar && isIndicatorValueAtClose)
                            {   // First bar
                                double value = component.Value[bar - 1];
                                int x = spcLeft + (bar - firstBar) * barPixels;
                                int y = (int)Math.Round(YIndChart - (value - minValue) * scale);

                                int deltaY = Math.Abs(y - point[i].Y);
                                if (barPixels > 3)
                                {   // Horizontal part
                                    if (deltaY == 0)
                                        g.DrawLine(pen, x + 1, y, x + barPixels - 5, y);
                                    else if (deltaY < 3)
                                        g.DrawLine(pen, x + 1, y, x + barPixels - 4, y);
                                    else
                                        g.DrawLine(pen, x + 1, y, x + barPixels - 2, y);
                                }
                                if (deltaY > 4)
                                {   // Vertical part
                                    if (point[i].Y > y)
                                        g.DrawLine(penTC, x + barPixels - 2, y + 2, x + barPixels - 2, point[i].Y - 2);
                                    else
                                        g.DrawLine(penTC, x + barPixels - 2, y - 2, x + barPixels - 2, point[i].Y + 2);
                                }
                            }

                            if (bar < lastBar)
                            {
                                int deltaY = Math.Abs(point[i + 1].Y - point[i].Y);
                                if (barPixels > 3)
                                {   // Horizontal part
                                    if (deltaY == 0)
                                        g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 3, point[i].Y);
                                    else if (deltaY < 3)
                                        g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 2, point[i].Y);
                                    else
                                        g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X, point[i].Y);
                                }
                                if (deltaY > 4)
                                {   // Vertical part
                                    if (point[i + 1].Y > point[i].Y)
                                        g.DrawLine(penTC, point[i + 1].X, point[i].Y + 2, point[i + 1].X, point[i + 1].Y - 2);
                                    else
                                        g.DrawLine(penTC, point[i + 1].X, point[i].Y - 2, point[i + 1].X, point[i + 1].Y + 2);
                                }
                            }

                            if (bar == lastBar && !isIndicatorValueAtClose && barPixels > 3)
                            {   // Last bar
                                g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i].X + barPixels - 2, point[i].Y);
                            }
                        }
                    }
                    else
                    {   // Regular Charts
                        Point[] aPoint = new Point[lastBar - firstBar + 1];
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double value = component.Value[bar];
                            int x = (bar - firstBar) * barPixels + barPixels / 2 - 1 + spcLeft;
                            int y = (int)Math.Round(YIndChart - (value - minValue) * scale);
                            aPoint[bar - firstBar] = new Point(x, y);
                        }
                        g.DrawLines(pen, aPoint);
                    }
                }
            }

            // Vertical cross line
            if (isCrossShown && mouseX > XLeft - 1 && mouseX < XRight + 1)
                g.DrawLine(penCross, mouseX, 0, mouseX, pnl.ClientSize.Height);


            // Chart title
            Indicator indicator = Indicator_Store.ConstructIndicator(Data.Strategy.Slot[slot].IndicatorName, Data.Strategy.Slot[slot].SlotType);
            indicator.IndParam = Data.Strategy.Slot[slot].IndParam;
            string indicatorText = indicator.ToString();
            Size sizeTitle = g.MeasureString(indicatorText, Font).ToSize();
            g.FillRectangle(brushBack, new Rectangle(spcLeft, 0, sizeTitle.Width, sizeTitle.Height));
            g.DrawString(indicatorText, Font, brushFore, spcLeft, 0);

            return;
        }

        /// <summary>
        /// Paints the panel FloatingPL
        /// </summary>
        void PnlFloatingPL_Paint(object sender, PaintEventArgs e)
        {
            if (!isFloatingPLShown) return;

            Panel pnl = (Panel)sender;
            Graphics g = e.Graphics;
            int topSpace   = font.Height / 2 + 2;
            int bottmSpace = font.Height / 2;
            int maxValue   =  10;
            int minValue   = -10;
            int value;

            g.Clear(LayoutColors.ColorChartBack);

            if (chartBars == 0) return;

            for (int bar = Math.Max(firstBar, Data.FirstBar); bar <= lastBar; bar++)
            {
                if (!Backtester.IsPos(bar)) continue;
                value = Configs.AccountInMoney ? (int)Math.Round(Backtester.MoneyProfitLoss(bar) + Backtester.MoneyFloatingPL(bar)) :
                                                        Backtester.ProfitLoss(bar) + Backtester.FloatingPL(bar);
                if (value > maxValue) maxValue = value;
                if (value < minValue) minValue = value;
            }

            minValue = 10 * (int)Math.Floor(minValue / 10.0);
            maxValue = 10 * (int)Math.Ceiling(maxValue / 10.0);

            double scale = (pnl.ClientSize.Height - topSpace - bottmSpace) / ((double)Math.Max(maxValue - minValue, 1));

            // Grid
            int XGridRight = pnl.ClientSize.Width - spcRight + 2;
            int label = 0;
            int labelYZero = (int)Math.Round(pnl.ClientSize.Height - bottmSpace - (label - minValue) * scale);
            if (label >= minValue && label <= maxValue)
            {
                g.DrawString(label.ToString(), Font, brushFore, XRight, labelYZero - Font.Height / 2 - 1);
                g.DrawLine(penGridSolid, spcLeft, labelYZero, XGridRight, labelYZero);
            }

            label = minValue;
            int labelYMin = (int)Math.Round(pnl.ClientSize.Height - bottmSpace - (label - minValue) * scale);
            if (Math.Abs(labelYZero - labelYMin) >= Font.Height)
            {
                g.DrawString(label.ToString(), Font, brushFore, XRight, labelYMin - Font.Height / 2 - 1);
                if (isGridShown)
                    g.DrawLine(penGrid, spcLeft, labelYMin, XGridRight, labelYMin);
                else
                    g.DrawLine(penGrid, XGridRight - 5, labelYMin, XGridRight, labelYMin);
            }
            label = maxValue;
            int labelYMax = (int)Math.Round(pnl.ClientSize.Height - bottmSpace - (label - minValue) * scale);
            if (Math.Abs(labelYZero - labelYMax) >= Font.Height)
            {
                g.DrawString(label.ToString(), Font, brushFore, XRight, labelYMax - Font.Height / 2 - 1);
                if (isGridShown)
                    g.DrawLine(penGrid, spcLeft, labelYMax, XGridRight, labelYMax);
                else
                    g.DrawLine(penGrid, XGridRight - 5, labelYMax, XGridRight, labelYMax);
            }

            // Vertical line
            if (isGridShown)
            {
                string date = Data.Time[firstBar].ToString("dd.MM") + " " + Data.Time[firstBar].ToString("HH:mm");
                int isDataWidth = (int)g.MeasureString(date, Font).Width;
                for (int vertLineBar = lastBar; vertLineBar > firstBar; vertLineBar -= (int)Math.Round((isDataWidth + 10.0) / barPixels + 1))
                {
                    int XVertLine = (vertLineBar - firstBar) * barPixels + barPixels / 2 - 1 + spcLeft;
                    g.DrawLine(penGrid, XVertLine, topSpace, XVertLine, pnl.ClientSize.Height - bottmSpace);
                }
            }

            // Chart
            Rectangle rect;
            LinearGradientBrush lgBrush;
            int y0 = (int)Math.Round(pnl.ClientSize.Height - 5 + minValue * scale);
            for (int bar = firstBar; bar <= lastBar; bar++)
            {
                if (!Backtester.IsPos(bar)) continue;
                value = Configs.AccountInMoney ? (int)Math.Round(Backtester.MoneyProfitLoss(bar) + Backtester.MoneyFloatingPL(bar)) :
                                                        Backtester.ProfitLoss(bar) + Backtester.FloatingPL(bar);
                int x = (bar - firstBar) * barPixels + spcLeft;
                int y = (int)Math.Round(pnl.ClientSize.Height - 7 - (value - minValue) * scale);

                if (y == y0) continue;
                if (Backtester.SummaryDir(bar) == PosDirection.Long)
                {
                    if (y > y0)
                    {
                        rect = new Rectangle(x - 1, y0, barPixels + 1, y - y0);
                        lgBrush = new LinearGradientBrush(rect, colorLongTrade1, colorLongTrade2, 0f);
                        rect = new Rectangle(x, y0, barPixels - 1, y - y0);
                    }
                    else if (y < y0)
                    {
                        rect = new Rectangle(x - 1, y, barPixels + 1, y0 - y);
                        lgBrush = new LinearGradientBrush(rect, colorLongTrade1, colorLongTrade2, 0f);
                        rect = new Rectangle(x, y, barPixels - 1, y0 - y);
                    }
                    else
                        continue;
                    g.FillRectangle(lgBrush, rect);
                }
                else if (Backtester.SummaryDir(bar) == PosDirection.Short)
                {
                    if (y > y0)
                    {
                        rect = new Rectangle(x - 1, y0, barPixels + 1, y - y0);
                        lgBrush = new LinearGradientBrush(rect, colorShortTrade1, colorShortTrade2, 0f);
                        rect = new Rectangle(x, y0, barPixels - 1, y - y0);
                    }
                    else if (y < y0)
                    {
                        rect = new Rectangle(x - 1, y, barPixels + 1, y0 - y);
                        lgBrush = new LinearGradientBrush(rect, colorShortTrade1, colorShortTrade2, 0f);
                        rect = new Rectangle(x, y, barPixels - 1, y0 - y);
                    }
                    else
                        continue;
                    g.FillRectangle(lgBrush, rect);
                }
                else
                {
                    if (y > y0)
                    {
                        rect = new Rectangle(x - 1, y0, barPixels + 1, y - y0);
                        lgBrush = new LinearGradientBrush(rect, colorClosedTrade1, colorClosedTrade2, 0f);
                        rect = new Rectangle(x, y0, barPixels - 1, y - y0);
                    }
                    else if (y < y0)
                    {
                        rect = new Rectangle(x - 1, y, barPixels + 1, y0 - y);
                        lgBrush = new LinearGradientBrush(rect, colorClosedTrade1, colorClosedTrade2, 0f);
                        rect = new Rectangle(x, y, barPixels - 1, y0 - y);
                    }
                    else
                        continue;
                    g.FillRectangle(lgBrush, rect);
                }
            }

            // Vertical cross line
            if (isCrossShown && mouseX > XLeft - 1 && mouseX < XRight + 1)
                g.DrawLine(penCross, mouseX, 0, mouseX, pnl.ClientSize.Height);


            // Chart title
            string sTitle = Language.T("Floating P/L") + " [" + (Configs.AccountInMoney ? Configs.AccountCurrency + "]" : Language.T("pips") + "]");
            Size szTitle = g.MeasureString(sTitle, Font).ToSize();
            g.FillRectangle(brushBack, new Rectangle(spcLeft, 0, szTitle.Width, szTitle.Height));
            g.DrawString(sTitle, Font, brushFore, spcLeft, 0);

            return;
        }

        /// <summary>
        /// Paints the panel PnlBalance
        /// </summary>
        void PnlBalance_Paint(object sender, PaintEventArgs e)
        {
            if (!isBalanceEquityShown) return;

            Panel   pnl = (Panel)sender;
            Graphics g  = e.Graphics;

            g.Clear(LayoutColors.ColorChartBack);

            if (chartBars == 0) return;

            int topSpace   = Font.Height / 2 + 2;
            int bottmSpace = Font.Height / 2;
            int yTop       = topSpace;
            int yBottom    = pnl.ClientSize.Height - bottmSpace;
            int xLeft      = XLeft;
            int xRight     = XRight;

            // Min and Max values
            int maxValue = int.MinValue;
            int minValue = int.MaxValue;
            int value;
            for (int iBar = Math.Max(firstBar, Data.FirstBar); iBar <= lastBar; iBar++)
            {
                value = Configs.AccountInMoney ? (int)Backtester.MoneyBalance(iBar) : Backtester.Balance(iBar);
                if (value > maxValue) maxValue = value;
                if (value < minValue) minValue = value;
                value = Configs.AccountInMoney ? (int)Backtester.MoneyEquity(iBar) : Backtester.Equity(iBar);
                if (value > maxValue) maxValue = value;
                if (value < minValue) minValue = value;
            }

            if (maxValue == 0 && minValue == 0)
            {
                maxValue = 10;
                minValue = -10;
            }

            if (maxValue == minValue)
            {
                maxValue += 10;
                minValue -= 10;
            }

            int countLabels = (int)Math.Max((yBottom - yTop) / 30, 1);
            int deltaLabels = 10 * (((int)Math.Max((maxValue - minValue) / countLabels, 10)) / 10);

            minValue  = 10 * (int)Math.Floor(minValue / 10.0);
            countLabels = (int)Math.Ceiling((maxValue - minValue) / (double)deltaLabels);
            maxValue  = minValue + countLabels * deltaLabels;

            double scale = (yBottom - yTop) / ((double)countLabels * deltaLabels);

            // Grid
            for (int label = minValue; label <= maxValue; label += deltaLabels)
            {
                int yLabel = (int)Math.Round(yBottom - (label - minValue) * scale);
                g.DrawString(label.ToString(), Font, brushFore, XRight, yLabel - Font.Height / 2 - 1);
                if (isGridShown)
                    g.DrawLine(penGrid, xLeft, yLabel, xRight, yLabel);
                else
                    g.DrawLine(penGrid, xRight - 5, yLabel, xRight, yLabel);
            }

                // Vertical grid lines
            if (isGridShown)
            {
                for (int vertLineBar = lastBar; vertLineBar > firstBar; vertLineBar -= (int)Math.Round((szDate.Width + 10.0) / barPixels + 1))
                {
                    int xVertLine = (vertLineBar - firstBar) * barPixels + xLeft + barPixels / 2 - 1;
                    g.DrawLine(penGrid, xVertLine, yTop, xVertLine, yBottom);
                }
            }

            // Chart
            Point[] apntBalance = new Point[lastBar - firstBar + 1];
            Point[] apntEquity  = new Point[lastBar - firstBar + 1];
            for (int bar = firstBar; bar <= lastBar; bar++)
            {
                int x, y;
                value = Configs.AccountInMoney ? (int)Backtester.MoneyBalance(bar) : Backtester.Balance(bar);
                x = (bar - firstBar) * barPixels + barPixels / 2 - 1 + xLeft;
                y = (int)Math.Round(yBottom - (value - minValue) * scale);
                apntBalance[bar - firstBar] = new Point(x, y);
                value = Configs.AccountInMoney ? (int)Backtester.MoneyEquity(bar) : Backtester.Equity(bar);
                y = (int)Math.Round(yBottom - (value - minValue) * scale);
                apntEquity[bar - firstBar] = new Point(x, y);
            }
            g.DrawLines(new Pen(LayoutColors.ColorChartEquityLine ), apntEquity);
            g.DrawLines(new Pen(LayoutColors.ColorChartBalanceLine), apntBalance);

            // Vertical cross line
            if (isCrossShown && mouseX > XLeft - 1 && mouseX < XRight + 1)
                g.DrawLine(penCross, mouseX, 0, mouseX, pnl.ClientSize.Height);

            // Chart title
            string sTitle = Language.T("Balance") + " / " + Language.T("Equity") +
                " [" + (Configs.AccountInMoney ? Configs.AccountCurrency + "]" : Language.T("pips") + "]");
            Size szTitle = g.MeasureString(sTitle, Font).ToSize();
            g.FillRectangle(brushBack, new Rectangle(spcLeft, 0, szTitle.Width, szTitle.Height));
            g.DrawString(sTitle, Font, brushFore, spcLeft, 0);

            return;
        }

        /// <summary>
        ///  Invalidates the panels
        /// </summary>
        void InvalidateAllPanels()
        {
            pnlPrice.Invalidate();

            if (isIndicatorsShown)
                foreach (Panel pnlind in pnlInd)
                    pnlind.Invalidate();

            if (isFloatingPLShown)
                pnlFloatingPLChart.Invalidate();

            if (isBalanceEquityShown)
                pnlBalanceChart.Invalidate();

            return;
        }

        /// <summary>
        /// Sets the width of the info panel
        /// </summary>
        void SetupDynInfoWidth()
        {
            asInfoTitle = new string[200];
            aiInfoType  = new int[200];
            infoRows   = 0;

            string sUnit = " [" + (Configs.AccountInMoney ? Configs.AccountCurrency : Language.T("pips")) + "]";

            // Dynamic info titles
            asInfoTitle[infoRows++] = Language.T("Bar number");
            asInfoTitle[infoRows++] = Language.T("Date");
            asInfoTitle[infoRows++] = Language.T("Opening time");
            asInfoTitle[infoRows++] = Language.T("Opening price");
            asInfoTitle[infoRows++] = Language.T("Highest price");
            asInfoTitle[infoRows++] = Language.T("Lowest price");
            asInfoTitle[infoRows++] = Language.T("Closing price");
            asInfoTitle[infoRows++] = Language.T("Volume");
            asInfoTitle[infoRows++] = Language.T("Balance")      + sUnit;
            asInfoTitle[infoRows++] = Language.T("Equity")       + sUnit;
            asInfoTitle[infoRows++] = Language.T("Profit Loss")  + sUnit;
            asInfoTitle[infoRows++] = Language.T("Floating P/L") + sUnit;

            for (int iSlot = 0; iSlot < Data.Strategy.Slots; iSlot++)
            {
                int iCompToShow = 0;
                foreach (IndicatorComp indComp in Data.Strategy.Slot[iSlot].Component)
                    if (indComp.ShowInDynInfo) iCompToShow++;
                if (iCompToShow == 0) continue;

                aiInfoType[infoRows] = 1;
                asInfoTitle[infoRows++] = Data.Strategy.Slot[iSlot].IndicatorName +
                    (Data.Strategy.Slot[iSlot].IndParam.CheckParam[0].Checked ? "*" : "");
                foreach (IndicatorComp indComp in Data.Strategy.Slot[iSlot].Component)
                    if (indComp.ShowInDynInfo) asInfoTitle[infoRows++] = indComp.CompName;
            }

            asInfoTitle[infoRows++] = "";
            asInfoTitle[infoRows++] = Language.T("Position direction");
            asInfoTitle[infoRows++] = Language.T("Number of open lots");
            asInfoTitle[infoRows++] = Language.T("Type of the transaction");
            asInfoTitle[infoRows++] = Language.T("Forming order number");
            asInfoTitle[infoRows++] = Language.T("Forming order price");
            asInfoTitle[infoRows++] = Language.T("Corrected position price");
            asInfoTitle[infoRows++] = Language.T("Profit Loss")  + sUnit;
            asInfoTitle[infoRows++] = Language.T("Floating P/L") + sUnit;

            Graphics g = CreateGraphics();

            int iMaxLenght = 0;
            foreach (string str in asInfoTitle)
            {
                int iLenght = (int)g.MeasureString(str, fontDI).Width;
                if (iMaxLenght < iLenght) iMaxLenght = iLenght;
            }

            XDynInfoCol2 = iMaxLenght + 10;
            int iMaxInfoWidth = Configs.AccountInMoney ?
                (int)Math.Max(g.MeasureString(Backtester.MinMoneyEquity.ToString("F2"), fontDI).Width,
                              g.MeasureString(Backtester.MaxMoneyEquity.ToString("F2"), fontDI).Width) :
                (int)Math.Max(g.MeasureString(Backtester.MinEquity.ToString(), fontDI).Width,
                              g.MeasureString(Backtester.MaxEquity.ToString(), fontDI).Width);
            iMaxInfoWidth = (int)Math.Max(g.MeasureString("99/99/99", fontDI).Width, iMaxInfoWidth);

            foreach (PosDirection posDir in Enum.GetValues(typeof(PosDirection)))
                if (g.MeasureString(Language.T(posDir.ToString()), fontDI).Width > iMaxInfoWidth)
                    iMaxInfoWidth = (int)g.MeasureString(Language.T(posDir.ToString()), fontDI).Width;

            foreach (Transaction transaction in Enum.GetValues(typeof(Transaction)))
                if (g.MeasureString(Language.T(transaction.ToString()), fontDI).Width > iMaxInfoWidth)
                    iMaxInfoWidth = (int)g.MeasureString(Language.T(transaction.ToString()), fontDI).Width;

            g.Dispose();

            dynInfoWidth = XDynInfoCol2 + iMaxInfoWidth + (isDEBUG ? 40 : 5);

            int iPasiveWidth = szInfoHelp.Width + 5;

            if (iPasiveWidth > dynInfoWidth)
            {
                XDynInfoCol2 += (iPasiveWidth - dynInfoWidth) / 2;
                dynInfoWidth = iPasiveWidth;
            }

            pnlInfo.ClientSize = new Size(dynInfoWidth, pnlInfo.ClientSize.Height);

            isDrawDinInfo = false;

            return;
        }

        /// <summary>
        /// Sets the dynamic info panel
        /// </summary>
        void SetupDynamicInfo()
        {
            asInfoTitle = new string[200];
            aiInfoType  = new int[200];
            infoRows    = 0;

            string unit = " [" + (Configs.AccountInMoney ? Configs.AccountCurrency : Language.T("pips")) + "]";

            // Dynamic info titles
            asInfoTitle[infoRows++] = Language.T("Bar number");
            asInfoTitle[infoRows++] = Language.T("Date");
            asInfoTitle[infoRows++] = Language.T("Opening time");
            asInfoTitle[infoRows++] = Language.T("Opening price");
            asInfoTitle[infoRows++] = Language.T("Highest price");
            asInfoTitle[infoRows++] = Language.T("Lowest price");
            asInfoTitle[infoRows++] = Language.T("Closing price");
            asInfoTitle[infoRows++] = Language.T("Volume");
            asInfoTitle[infoRows++] = "";
            asInfoTitle[infoRows++] = Language.T("Balance")      + unit;
            asInfoTitle[infoRows++] = Language.T("Equity")       + unit;
            asInfoTitle[infoRows++] = Language.T("Profit Loss")  + unit;
            asInfoTitle[infoRows++] = Language.T("Floating P/L") + unit;

            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                int compToShow = 0;
                foreach (IndicatorComp indComp in Data.Strategy.Slot[slot].Component)
                    if (indComp.ShowInDynInfo) compToShow++;
                if (compToShow == 0) continue;

                asInfoTitle[infoRows++] = "";
                aiInfoType[infoRows]    = 1;
                asInfoTitle[infoRows++] = Data.Strategy.Slot[slot].IndicatorName +
                    (Data.Strategy.Slot[slot].IndParam.CheckParam[0].Checked ? "*" : "");
                foreach (IndicatorComp indComp in Data.Strategy.Slot[slot].Component)
                    if (indComp.ShowInDynInfo) asInfoTitle[infoRows++] = indComp.CompName;
            }

            for (int pos = 0; pos < posCount; pos++)
            {
                asInfoTitle[infoRows++] = "";
                asInfoTitle[infoRows++] = Language.T("Position direction");
                asInfoTitle[infoRows++] = Language.T("Number of open lots");
                asInfoTitle[infoRows++] = Language.T("Type of the transaction");
                asInfoTitle[infoRows++] = Language.T("Forming order number");
                asInfoTitle[infoRows++] = Language.T("Forming order price");
                asInfoTitle[infoRows++] = Language.T("Corrected position price");
                asInfoTitle[infoRows++] = Language.T("Profit Loss")  + unit;
                asInfoTitle[infoRows++] = Language.T("Floating P/L") + unit;
            }

            isDrawDinInfo = false;

            return;
        }

        /// <summary>
        /// Generates the DynamicInfo.
        /// </summary>
        void GenerateDynamicInfo(int barNumb)
        {
            if (!isDynInfoShown || !isInfoPanelShown) return;

            barNumb = Math.Max(0, barNumb);
            barNumb = Math.Min(chartBars - 1, barNumb);

            int bar;
            bar = firstBar + barNumb;
            bar = Math.Min(Data.Bars - 1, bar);

            if (barOld == bar) return;
            barOld = bar;

            int row = 0;
            asInfoValue = new String[200];
            asInfoValue[row++] = (bar + 1).ToString();
            asInfoValue[row++] = Data.Time[bar].ToString(Data.DF);
            asInfoValue[row++] = Data.Time[bar].ToString("HH:mm");
            if (isDEBUG)
            {
                asInfoValue[row++] = Data.Open[bar].ToString();
                asInfoValue[row++] = Data.High[bar].ToString();
                asInfoValue[row++] = Data.Low[bar].ToString();
                asInfoValue[row++] = Data.Close[bar].ToString();
            }
            else
            {

                asInfoValue[row++] = Data.Open[bar].ToString(Data.FF);
                asInfoValue[row++] = Data.High[bar].ToString(Data.FF);
                asInfoValue[row++] = Data.Low[bar].ToString(Data.FF);
                asInfoValue[row++] = Data.Close[bar].ToString(Data.FF);
            }
            asInfoValue[row++] = Data.Volume[bar].ToString();

            asInfoValue[row++] = "";
            if (Configs.AccountInMoney)
            {
                // Balance
                asInfoValue[row++] = Backtester.MoneyBalance(bar).ToString("F2");

                // Equity
                asInfoValue[row++] = Backtester.MoneyEquity(bar).ToString("F2");

                // Profit Loss
                if (Backtester.SummaryTrans(bar) == Transaction.Close  ||
                    Backtester.SummaryTrans(bar) == Transaction.Reduce ||
                    Backtester.SummaryTrans(bar) == Transaction.Reverse)
                    asInfoValue[row++] = Backtester.MoneyProfitLoss(bar).ToString("F2");
                else
                    asInfoValue[row++] = "   -";

                // Floating P/L
                if (Backtester.Positions(bar) > 0 && Backtester.SummaryTrans(bar) != Transaction.Close)
                    asInfoValue[row++] = Backtester.MoneyFloatingPL(bar).ToString("F2");
                else
                    asInfoValue[row++] = "   -";
            }
            else
            {
                // Balance
                asInfoValue[row++] = Backtester.Balance(bar).ToString();

                // Equity
                asInfoValue[row++] = Backtester.Equity(bar).ToString();

                // Profit Loss
                if (Backtester.SummaryTrans(bar) == Transaction.Close  ||
                    Backtester.SummaryTrans(bar) == Transaction.Reduce ||
                    Backtester.SummaryTrans(bar) == Transaction.Reverse)
                    asInfoValue[row++] = Backtester.ProfitLoss(bar).ToString();
                else
                    asInfoValue[row++] = "   -";

                // Profit Loss
                if (Backtester.Positions(bar) > 0 && Backtester.SummaryTrans(bar) != Transaction.Close)
                    asInfoValue[row++] = Backtester.FloatingPL(bar).ToString();
                else
                    asInfoValue[row++] = "   -";
            }

            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                if (Data.Strategy.Slot[slot] != null)
                {
                    int compToShow = 0;
                    foreach (IndicatorComp indComp in Data.Strategy.Slot[slot].Component)
                        if (indComp.ShowInDynInfo) compToShow++;
                    if (compToShow == 0) continue;

                    asInfoValue[row++] = "";
                    asInfoValue[row++] = "";
                    foreach (IndicatorComp indComp in Data.Strategy.Slot[slot].Component)
                    {
                        if (indComp.ShowInDynInfo)
                        {
                            IndComponentType indDataTipe = indComp.DataType;
                            if (indDataTipe == IndComponentType.AllowOpenLong  ||
                                indDataTipe == IndComponentType.AllowOpenShort ||
                                indDataTipe == IndComponentType.ForceClose     ||
                                indDataTipe == IndComponentType.ForceCloseLong ||
                                indDataTipe == IndComponentType.ForceCloseShort)
                                asInfoValue[row++] = (indComp.Value[bar] < 1 ? Language.T("No") : Language.T("Yes"));
                            else
                            {
                                if (isDEBUG)
                                {
                                    asInfoValue[row++] = indComp.Value[bar].ToString();
                                }
                                else
                                {
                                    double dl = Math.Abs(indComp.Value[bar]);
                                    string sFR = dl < 10 ? "F5" : dl < 100 ? "F4" : dl < 1000 ? "F3" : dl < 10000 ? "F2" : dl < 100000 ? "F1" : "F0";
                                    if (indComp.Value[bar] != 0)
                                        asInfoValue[row++] = indComp.Value[bar].ToString(sFR);
                                    else
                                        asInfoValue[row++] = "   -";
                                }
                            }
                        }
                    }
                }
            }

            // Positions
            int pos;
            for (pos = 0; pos < Backtester.Positions(bar); pos++)
            {
                asInfoValue[row++] = "";
                asInfoValue[row++] = Language.T(Backtester.PosDir(bar, pos).ToString());
                asInfoValue[row++] = Backtester.PosLots(bar, pos).ToString();
                asInfoValue[row++] = Language.T(Backtester.PosTransaction(bar, pos).ToString());
                asInfoValue[row++] = Backtester.PosOrdNumb(bar, pos).ToString();
                asInfoValue[row++] = Backtester.PosOrdPrice(bar, pos).ToString(Data.FF);
                asInfoValue[row++] = Backtester.PosPrice(bar, pos).ToString(Data.FF);

                // Profit Loss
                if (Backtester.PosTransaction(bar, pos) == Transaction.Close  ||
                    Backtester.PosTransaction(bar, pos) == Transaction.Reduce ||
                    Backtester.PosTransaction(bar, pos) == Transaction.Reverse)
                    asInfoValue[row++] = Configs.AccountInMoney ?
                        Backtester.PosMoneyProfitLoss(bar, pos).ToString("F2") :
                        Math.Round(Backtester.PosProfitLoss(bar, pos)).ToString();
                else
                    asInfoValue[row++] = "   -";

                // Floating P/L
                if (pos == Backtester.Positions(bar) - 1 && Backtester.PosTransaction(bar, pos) != Transaction.Close)
                    asInfoValue[row++] = Configs.AccountInMoney ?
                        Backtester.PosMoneyFloatingPL(bar, pos).ToString("F2"):
                        Math.Round(Backtester.PosFloatingPL(bar, pos)).ToString(); 
                else
                    asInfoValue[row++] = "   -";
            }

            if (posCount != pos)
            {
                posCount = pos;
                SetupDynamicInfo();
                isDrawDinInfo = true;
                pnlInfo.Invalidate();
            }
            else
            {
                pnlInfo.Invalidate(new Rectangle(XDynInfoCol2, 0, dynInfoWidth - XDynInfoCol2, pnlInfo.ClientSize.Height));
            }

            return;
        }

        /// <summary>
        /// When the mous is near to an important value - show a tip
        /// </summary>
        void GenerateMouseTip(int barNumb)
        {
            barNumb = Math.Max(0, barNumb);
            barNumb = Math.Min(chartBars - 1, barNumb);

            int iBar;
            iBar  = firstBar + barNumb;
            iBar  = Math.Min(Data.Bars - 1, iBar);
            
            string sTip = "";
            string sTipBarInfo = "";
            double dMousePrice = (YBottom - mouseY) / YScale + minPrice;
            if (Math.Abs(dMousePrice - Data.Open[iBar]) < 5 * Data.InstrProperties.Point)
            {
                sTipBarInfo += "    Open: " + Data.High[iBar].ToString() + Environment.NewLine;
            }
            if (Math.Abs(dMousePrice - Data.High[iBar])  < 5 * Data.InstrProperties.Point)
            {
                sTipBarInfo += "    High: " + Data.High[iBar].ToString() + Environment.NewLine;
            }
            if (Math.Abs(dMousePrice - Data.Low[iBar]) < 5 * Data.InstrProperties.Point)
            {
                sTipBarInfo += "    Low: " + Data.High[iBar].ToString() + Environment.NewLine;
            }
            if (Math.Abs(dMousePrice - Data.Close[iBar])< 5 * Data.InstrProperties.Point)
            {
                sTipBarInfo += "    Close: " + Data.High[iBar].ToString() + Environment.NewLine;
            }

            if (sTipBarInfo != "")
            {
                sTipBarInfo = "Bar information" + Environment.NewLine + sTipBarInfo;
                sTip += sTipBarInfo;
            }

            for (int iSlot = 0; iSlot < Data.Strategy.Slots; iSlot++)
            {
                if (Data.Strategy.Slot[iSlot] != null)
                {
                    for (int iComp = 0; iComp < Data.Strategy.Slot[iSlot].Component.Length; iComp++)
                    {
                        IndComponentType indDataTipe = Data.Strategy.Slot[iSlot].Component[iComp].DataType;
                        if (indDataTipe == IndComponentType.CloseLongPrice  ||
                            indDataTipe == IndComponentType.ClosePrice      ||
                            indDataTipe == IndComponentType.CloseShortPrice ||
                            indDataTipe == IndComponentType.IndicatorValue  ||
                            indDataTipe == IndComponentType.OpenClosePrice  ||
                            indDataTipe == IndComponentType.OpenLongPrice   ||
                            indDataTipe == IndComponentType.OpenPrice       ||
                            indDataTipe == IndComponentType.OpenShortPrice
                           )
                        {
                            double dValue = Data.Strategy.Slot[iSlot].Component[iComp].Value[iBar];
                            if (Math.Abs(dMousePrice - dValue) < 5 * Data.InstrProperties.Point)
                            {
                                double dl  = Math.Abs(dValue);
                                string sFR = dl < 10 ? "F4" : dl < 100 ? "F3" : dl < 1000 ? "F2" : dl < 10000 ? "F1" : "F0";
                                sTip += Data.Strategy.Slot[iSlot].Component[iComp].CompName + ": " + dValue.ToString(sFR) + Environment.NewLine;
                            }
                        }
                    }
                }
            }

            // Positions
            int iPos;
            for (iPos = 0; iPos < Backtester.Positions(iBar); iPos++)
            {
                double fOrderPrice    = Backtester.SummaryOrdPrice(iBar);
                double fPositionPrice = Backtester.SummaryPrice(iBar);
                if (Math.Abs(dMousePrice - fOrderPrice) < 5 * Data.InstrProperties.Point) sTip += "Order price: " + fOrderPrice.ToString() + Environment.NewLine;
                if (Math.Abs(dMousePrice - fPositionPrice) < 5 * Data.InstrProperties.Point) sTip += "Positions price: " + fPositionPrice.ToString() + Environment.NewLine;
            }

            if (sTip != "")
            {
                mouseTips.Show(sTip, pnlPrice, mouseX + 10, mouseY + 10, 5000);
            }

            return;
        }

        /// <summary>
        /// PnlInfo Resize
        /// </summary>
        void PnlInfo_Resize(object sender, EventArgs e)
        {
            pnlInfo.Invalidate();

            return;
        }

        /// <summary>
        /// Paints the panel PnlInfo.
        /// </summary>
        void PnlInfo_Paint(object sender, PaintEventArgs e)
        {
            if (!isInfoPanelShown) return;

            Graphics g = e.Graphics;
            g.Clear(LayoutColors.ColorControlBack);

            if (isDrawDinInfo && isDynInfoShown)
            {
                int rowHeight = fontDI.Height + 1;
                Size size = new Size(dynInfoWidth, rowHeight);

                for (int i = 0; i < infoRows; i++)
                {
                    if (i % 2f != 0)
                        g.FillRectangle(brushEvenRows, new Rectangle(new Point(0, i * rowHeight + 1), size));

                    if (aiInfoType[i + dynInfoScrollValue] == 1)
                        g.DrawString(asInfoTitle[i + dynInfoScrollValue], fontDIInd, brushDIIndicator, 5, i * rowHeight - 1);
                    else
                        g.DrawString(asInfoTitle[i + dynInfoScrollValue], fontDI, brushDynamicInfo, 5, i * rowHeight + 1);

                    g.DrawString(asInfoValue[i + dynInfoScrollValue], fontDI, brushDynamicInfo, XDynInfoCol2, i * rowHeight + 1);
                }
            }

            return;
        }

        /// <summary>
        /// Invalidate Cross Old/New position and Dynamic Info
        /// </summary>
        void PnlPrice_MouseMove(object sender, MouseEventArgs e)
        {
            mouseXOld = mouseX;
            mouseYOld = mouseY;
            mouseX = e.X;
            mouseY = e.Y;

            if (e.Button == MouseButtons.Left)
            {
                if (mouseX > XRight)
                {
                    if (mouseY > mouseYOld)
                        VerticalScaleDecrease();
                    else
                        VerticalScaleIncrease();

                    return;
                }
                else
                {
                    int newScrollValue = scroll.Value;

                    if (mouseX > mouseXOld)
                        newScrollValue -= (int)Math.Round(scroll.SmallChange * 0.1 * (100 - barPixels));
                    else if (mouseX < mouseXOld)
                        newScrollValue += (int)Math.Round(scroll.SmallChange * 0.1 * (100 - barPixels));

                    if (newScrollValue < scroll.Minimum)
                        newScrollValue = scroll.Minimum;
                    else if (newScrollValue > scroll.Maximum + 1 - scroll.LargeChange)
                        newScrollValue = scroll.Maximum + 1 - scroll.LargeChange;

                    scroll.Value = newScrollValue;
                    
                }
            }

            if (isCrossShown)
            {
                GraphicsPath path = new GraphicsPath(FillMode.Winding);

                // Adding the old positions
                if (mouseXOld >= XLeft && mouseXOld <= XRight)
                {
                    if (mouseYOld >= YTop && mouseYOld <= YBottom)
                    {
                        // Horizontal Line
                        path.AddRectangle(new Rectangle(0, mouseYOld, pnlPrice.ClientSize.Width, 1));
                        // PriceBox
                        path.AddRectangle(new Rectangle(XRight - 1, mouseYOld - font.Height / 2 - 1, szPrice.Width + 2, font.Height + 2));
                    }
                    // Vertical Line
                    path.AddRectangle(new Rectangle(mouseXOld, 0, 1, pnlPrice.ClientSize.Height));
                    // DateBox
                    path.AddRectangle(new Rectangle(mouseXOld - szDateL.Width / 2 - 1, YBottomText - 1, szDateL.Width + 2, font.Height + 3));
                }

                // Adding the new positions
                if (mouseX >= XLeft && mouseX <= XRight)
                {
                    if (mouseYOld >= YTop && mouseYOld <= YBottom)
                    {
                        // Horizontal Line
                        path.AddRectangle(new Rectangle(0, mouseY, pnlPrice.ClientSize.Width, 1));
                        // PriceBox
                        path.AddRectangle(new Rectangle(XRight - 1, mouseY - font.Height / 2 - 1, szPrice.Width + 2, font.Height + 2));
                    }
                    // Vertical Line
                    path.AddRectangle(new Rectangle(mouseX, 0, 1, pnlPrice.ClientSize.Height));
                    // DateBox
                    path.AddRectangle(new Rectangle(mouseX - szDateL.Width / 2 - 1, YBottomText - 1, szDateL.Width + 2, font.Height + 3));
                }
                pnlPrice.Invalidate(new Region(path));

                for (int i = 0; i < indPanels && isIndicatorsShown; i++)
                {
                    GraphicsPath path1 = new GraphicsPath(FillMode.Winding);
                    if (mouseXOld > XLeft - 1 && mouseXOld < XRight + 1)
                        path1.AddRectangle(new Rectangle(mouseXOld, 0, 1, pnlInd[i].ClientSize.Height));
                    if (mouseX > XLeft - 1 && mouseX < XRight + 1)
                        path1.AddRectangle(new Rectangle(mouseX, 0, 1, pnlInd[i].ClientSize.Height));
                    pnlInd[i].Invalidate(new Region(path1));
                }

                if (isBalanceEquityShown)
                {
                    GraphicsPath path1 = new GraphicsPath(FillMode.Winding);
                    if (mouseXOld > XLeft - 1 && mouseXOld < XRight + 1)
                        path1.AddRectangle(new Rectangle(mouseXOld, 0, 1, pnlBalanceChart.ClientSize.Height));
                    if (mouseX > XLeft - 1 && mouseX < XRight + 1)
                        path1.AddRectangle(new Rectangle(mouseX, 0, 1, pnlBalanceChart.ClientSize.Height));
                    pnlBalanceChart.Invalidate(new Region(path1));
                }

                if (isFloatingPLShown)
                {
                    GraphicsPath path1 = new GraphicsPath(FillMode.Winding);
                    if (mouseXOld > XLeft - 1 && mouseXOld < XRight + 1)
                        path1.AddRectangle(new Rectangle(mouseXOld, 0, 1, pnlFloatingPLChart.ClientSize.Height));
                    if (mouseX > XLeft - 1 && mouseX < XRight + 1)
                        path1.AddRectangle(new Rectangle(mouseX, 0, 1, pnlFloatingPLChart.ClientSize.Height));
                    pnlFloatingPLChart.Invalidate(new Region(path1));
                }
            }

            // Determines the shown bar.
            if (mouseXOld >= XLeft && mouseXOld <= XRight)
            {
                if (mouseX >= XLeft && mouseX <= XRight)
                {   // Moving inside the chart
                    isMouseInPriceChart = true;
                    isDrawDinInfo = true;
                    GenerateDynamicInfo((e.X - XLeft) / barPixels);
                }
                else
                {   // Escaping from the bar area of chart
                    isMouseInPriceChart = false;
                    pnlPrice.Cursor = Cursors.Default;
                }
            }
            else if (mouseX >= XLeft && mouseX <= XRight)
            {   // Entering into the chart
                if (isCrossShown) 
                    pnlPrice.Cursor = Cursors.Cross;
                isMouseInPriceChart = true;
                isDrawDinInfo = true;
                pnlInfo.Invalidate();
                GenerateDynamicInfo((e.X - XLeft) / barPixels);
            }

            return;
        }

        /// <summary>
        /// Deletes the cross and Dynamic Info
        /// </summary>
        void PnlPrice_MouseLeave(object sender, EventArgs e)
        {
            pnlPrice.Cursor    = Cursors.Default;
            isMouseInPriceChart = false;

            mouseXOld = mouseX;
            mouseYOld = mouseY;
            mouseX    = -1;
            mouseY    = -1;
            barOld    = -1;

            if (isCrossShown)
            {
                GraphicsPath path = new GraphicsPath(FillMode.Winding);

                // Horizontal Line
                path.AddRectangle(new Rectangle(0, mouseYOld, pnlPrice.ClientSize.Width, 1));
                // PriceBox
                path.AddRectangle(new Rectangle(XRight - 1, mouseYOld - font.Height / 2 - 1, szPrice.Width + 2, font.Height + 2));
                // Vertical Line
                path.AddRectangle(new Rectangle(mouseXOld, 0, 1, pnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(mouseXOld - szDateL.Width / 2 - 1, YBottomText - 1, szDateL.Width + 2, font.Height + 3));

                pnlPrice.Invalidate(new Region(path));

                for (int i = 0; i < indPanels && isIndicatorsShown; i++)
                    pnlInd[i].Invalidate(new Rectangle(mouseXOld, 0, 1, pnlInd[i].ClientSize.Height));

                if (isBalanceEquityShown)
                    pnlBalanceChart.Invalidate(new Rectangle(mouseXOld, 0, 1, pnlBalanceChart.ClientSize.Height));

                if (isFloatingPLShown)
                    pnlFloatingPLChart.Invalidate(new Rectangle(mouseXOld, 0, 1, pnlFloatingPLChart.ClientSize.Height));
            }

            return;
        }

        /// <summary>
        /// Mouse moves inside a chart
        /// </summary>
        void SepChart_MouseMove(object sender, MouseEventArgs e)
        {
            Panel panel = (Panel)sender;

            mouseXOld = mouseX;
            mouseYOld = mouseY;
            mouseX = e.X;
            mouseY = e.Y;

            if (e.Button == MouseButtons.Left)
            {
                int newScrollValue = scroll.Value;

                if (mouseX > mouseXOld)
                    newScrollValue -= (int)Math.Round(scroll.SmallChange * 0.1 * (100 - barPixels));
                else if (mouseX < mouseXOld)
                    newScrollValue += (int)Math.Round(scroll.SmallChange * 0.1 * (100 - barPixels));

                if (newScrollValue < scroll.Minimum)
                    newScrollValue = scroll.Minimum;
                else if (newScrollValue > scroll.Maximum + 1 - scroll.LargeChange)
                    newScrollValue = scroll.Maximum + 1 - scroll.LargeChange;

                scroll.Value = newScrollValue;
            }

            if (isCrossShown)
            {
                GraphicsPath path = new GraphicsPath(FillMode.Winding);

                // Adding the old positions
                if (mouseXOld >= XLeft && mouseXOld <= XRight)
                {
                    // Vertical Line
                    path.AddRectangle(new Rectangle(mouseXOld, 0, 1, pnlPrice.ClientSize.Height));
                    // DateBox
                    path.AddRectangle(new Rectangle(mouseXOld - szDateL.Width / 2 - 1, YBottomText - 1, szDateL.Width + 2, font.Height + 3));
                }

                // Adding the new positions
                if (mouseX >= XLeft && mouseX <= XRight)
                {
                    // Vertical Line
                    path.AddRectangle(new Rectangle(mouseX, 0, 1, pnlPrice.ClientSize.Height));
                    // DateBox
                    path.AddRectangle(new Rectangle(mouseX - szDateL.Width / 2 - 1, YBottomText - 1, szDateL.Width + 2, font.Height + 3));
                }
                pnlPrice.Invalidate(new Region(path));

                for (int i = 0; i < indPanels && isIndicatorsShown; i++)
                {
                    GraphicsPath path1 = new GraphicsPath(FillMode.Winding);
                    if (mouseXOld > XLeft - 1 && mouseXOld < XRight + 1)
                        path1.AddRectangle(new Rectangle(mouseXOld, 0, 1, pnlInd[i].ClientSize.Height));
                    if (mouseX > XLeft - 1 && mouseX < XRight + 1)
                        path1.AddRectangle(new Rectangle(mouseX, 0, 1, pnlInd[i].ClientSize.Height));
                    pnlInd[i].Invalidate(new Region(path1));
                }

                if (isBalanceEquityShown)
                {
                    GraphicsPath path1 = new GraphicsPath(FillMode.Winding);
                    if (mouseXOld > XLeft - 1 && mouseXOld < XRight + 1)
                        path1.AddRectangle(new Rectangle(mouseXOld, 0, 1, pnlBalanceChart.ClientSize.Height));
                    if (mouseX > XLeft - 1 && mouseX < XRight + 1)
                        path1.AddRectangle(new Rectangle(mouseX, 0, 1, pnlBalanceChart.ClientSize.Height));
                    pnlBalanceChart.Invalidate(new Region(path1));
                }

                if (isFloatingPLShown)
                {
                    GraphicsPath path1 = new GraphicsPath(FillMode.Winding);
                    if (mouseXOld > XLeft - 1 && mouseXOld < XRight + 1)
                        path1.AddRectangle(new Rectangle(mouseXOld, 0, 1, pnlFloatingPLChart.ClientSize.Height));
                    if (mouseX > XLeft - 1 && mouseX < XRight + 1)
                        path1.AddRectangle(new Rectangle(mouseX, 0, 1, pnlFloatingPLChart.ClientSize.Height));
                    pnlFloatingPLChart.Invalidate(new Region(path1));
                }
            }

            // Determines the shown bar.
            if (mouseXOld >= XLeft && mouseXOld <= XRight)
            {
                if (mouseX >= XLeft && mouseX <= XRight)
                {   // Moving inside the chart
                    isMouseInIndicatorChart = true;
                    isDrawDinInfo = true;
                    GenerateDynamicInfo((e.X - XLeft) / barPixels);
                }
                else
                {   // Escaping from the bar area of chart
                    isMouseInIndicatorChart = false;
                    panel.Cursor = Cursors.Default;
                }
            }
            else if (mouseX >= XLeft && mouseX <= XRight)
            {    // Entering into the chart
                if (isCrossShown) 
                    panel.Cursor = Cursors.Cross;
                isMouseInIndicatorChart = true;
                isDrawDinInfo = true;
                pnlInfo.Invalidate();
                GenerateDynamicInfo((e.X - XLeft) / barPixels);
            }

            return;
        }

        /// <summary>
        /// Mouse leaves a chart.
        /// </summary>
        void SepChart_MouseLeave(object sender, EventArgs e)
        {
            Panel panel = (Panel)sender;
            panel.Cursor = Cursors.Default;

            isMouseInIndicatorChart = false;

            mouseXOld = mouseX;
            mouseYOld = mouseY;
            mouseX    = -1;
            mouseY    = -1;
            barOld    = -1;

            if (isCrossShown)
            {
                GraphicsPath path = new GraphicsPath(FillMode.Winding);

                // Vertical Line
                path.AddRectangle(new Rectangle(mouseXOld, 0, 1, pnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(mouseXOld - szDateL.Width / 2 - 1, YBottomText - 1, szDateL.Width + 2, font.Height + 3));

                pnlPrice.Invalidate(new Region(path));

                for (int i = 0; i < indPanels && isIndicatorsShown; i++)
                    pnlInd[i].Invalidate(new Rectangle(mouseXOld, 0, 1, pnlInd[i].ClientSize.Height));

                if (isBalanceEquityShown)
                    pnlBalanceChart.Invalidate(new Rectangle(mouseXOld, 0, 1, pnlBalanceChart.ClientSize.Height));

                if (isFloatingPLShown)
                    pnlFloatingPLChart.Invalidate(new Rectangle(mouseXOld, 0, 1, pnlFloatingPLChart.ClientSize.Height));
            }

            return;
        }

        /// <summary>
        /// Sets the parameters when scrolling.
        /// </summary>
        void Scroll_ValueChanged(object sender, EventArgs e)
        {
            firstBar = scroll.Value;
            lastBar = Math.Min(Data.Bars - 1, firstBar + chartBars - 1);
            lastBar = Math.Max(lastBar, firstBar);

            InvalidateAllPanels();

            // Dynamic Info
            barOld = 0;
            // Sends the shown bar from the chart beginning
            if (isDrawDinInfo && isDynInfoShown)
            {
                int selectedBar = (mouseX - spcLeft) / barPixels;
                GenerateDynamicInfo(selectedBar);
            }

            return;
        }

        /// <summary>
        /// Scrolls the scrollbar when turning the mouse wheel.
        /// </summary>
        void Scroll_MouseWheel(object sender, MouseEventArgs e)
        {
            if (isKeyCtrlPressed)
            {
                if (e.Delta > 0)
                    ZoomIn();
                if (e.Delta < 0)
                    ZoomOut();
            }
            else
            {
                int newScrollValue = scroll.Value + scroll.LargeChange * e.Delta / SystemInformation.MouseWheelScrollLines / 120;

                if (newScrollValue < scroll.Minimum)
                    newScrollValue = scroll.Minimum;
                else if (newScrollValue > scroll.Maximum + 1 - scroll.LargeChange)
                    newScrollValue = scroll.Maximum + 1 - scroll.LargeChange;

                scroll.Value = newScrollValue;
            }

            return;
        }

        /// <summary>
        /// Call KeyUp method
        /// </summary>
        void Scroll_KeyUp(object sender, KeyEventArgs e)
        {
            isKeyCtrlPressed = false;

            ShortcutKeyUp(e);

            return;
        }

        /// <summary>
        /// Call KeyUp method
        /// </summary>
        void Scroll_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
                isKeyCtrlPressed = true;

            return;
        }

        /// <summary>
        /// Call KeyUp method
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            isKeyCtrlPressed = false;

            ShortcutKeyUp(e);

            return;
        }

        /// <summary>
        /// Mouse button down on a panel.
        /// </summary>
        void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            Panel panel = (Panel)sender;
            if (panel == pnlPrice && mouseX > XRight)
                panel.Cursor = Cursors.SizeNS;
            else
                panel.Cursor = Cursors.SizeWE;
        }

        /// <summary>
        /// Mouse button up on a panel.
        /// </summary>
        void Panel_MouseUp(object sender, MouseEventArgs e)
        {
            Panel panel = (Panel)sender;
            panel.Cursor = isCrossShown ? Cursors.Cross : Cursors.Default; ;
        }

        /// <summary>
        /// Shows the Bar Explorer
        /// </summary>
        void PnlPrice_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Determines the shown bar.
            if (mouseXOld >= XLeft && mouseXOld <= XRight && mouseYOld >= YTop && mouseYOld <= YBottom)
            {
                // Moving inside the chart
                if (mouseX >= XLeft && mouseX <= XRight && mouseY >= YTop && mouseY <= YBottom)
                {
                    int selectedBar = (e.X - XLeft) / barPixels + firstBar;
                    Bar_Explorer be = new Bar_Explorer(selectedBar);
                    be.ShowDialog();
                }
            }

            return;
        }

        /// <summary>
        /// Changes chart's settings after a button click.
        /// </summary>
        void ButtonChart_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;
            ChartButtons button = (ChartButtons)btn.Tag;

            switch (button)
            {
                case ChartButtons.Grid:
                    ShortcutKeyUp(new KeyEventArgs(Keys.G));
                    break;
                case ChartButtons.Cross:
                    ShortcutKeyUp(new KeyEventArgs(Keys.C));
                    break;
                case ChartButtons.Volume:
                    ShortcutKeyUp(new KeyEventArgs(Keys.V));
                    break;
                case ChartButtons.Orders:
                    ShortcutKeyUp(new KeyEventArgs(Keys.O));
                    break;
                case ChartButtons.PositionLots:
                    ShortcutKeyUp(new KeyEventArgs(Keys.L));
                    break;
                case ChartButtons.PositionPrice:
                    ShortcutKeyUp(new KeyEventArgs(Keys.P));
                    break;
                case ChartButtons.AmbiguousBars:
                    ShortcutKeyUp(new KeyEventArgs(Keys.M));
                    break;
                case ChartButtons.Indicators:
                    ShortcutKeyUp(new KeyEventArgs(Keys.D));
                    break;
                case ChartButtons.BalanceEquity:
                    ShortcutKeyUp(new KeyEventArgs(Keys.B));
                    break;
                case ChartButtons.FloatingPL:
                    ShortcutKeyUp(new KeyEventArgs(Keys.F));
                    break;
                case ChartButtons.ZoomIn:
                    ShortcutKeyUp(new KeyEventArgs(Keys.Add));
                    break;
                case ChartButtons.ZoomOut:
                    ShortcutKeyUp(new KeyEventArgs(Keys.Subtract));
                    break;
                case ChartButtons.TrueCharts:
                    ShortcutKeyUp(new KeyEventArgs(Keys.T));
                    break;
                case ChartButtons.DynamicInfo:
                    ShortcutKeyUp(new KeyEventArgs(Keys.I));
                    break;
                case ChartButtons.DInfoDwn:
                    ShortcutKeyUp(new KeyEventArgs(Keys.Z));
                    break;
                case ChartButtons.DInfoUp:
                    ShortcutKeyUp(new KeyEventArgs(Keys.A));
                    break;
                default:
                    break;
            }

            return;
        }

        /// <summary>
        /// Shortcut keys
        /// </summary>
        void ShortcutKeyUp(KeyEventArgs e)
        {
            // Zoom in
            if (!e.Control && (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus))
            {
                ZoomIn();
            }
            // Zoom out
            else if (!e.Control && (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus))
            {
                ZoomOut();
            }
            // Vertical scale increase
            else if (e.Control && (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus))
            {
                VerticalScaleIncrease();
            }
            // Vertical scale decrease
            else if (e.Control && (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus))
            {
                VerticalScaleDecrease();
            }
            else if (e.KeyCode == Keys.Space)
            {
                isCandleChart = !isCandleChart;
                pnlPrice.Invalidate();
            }
            // Dyn info scroll up
            else if (e.KeyCode == Keys.A)
            {
                if (!isInfoPanelShown)
                    return;
                dynInfoScrollValue -= 5;
                dynInfoScrollValue = dynInfoScrollValue < 0 ? 0 : dynInfoScrollValue;
                pnlInfo.Invalidate();

            }
            // Dyn info scroll up fast
            else if (e.KeyCode == Keys.S)
            {
                if (!isInfoPanelShown)
                    return;
                dynInfoScrollValue -= 10;
                dynInfoScrollValue = dynInfoScrollValue < 0 ? 0 : dynInfoScrollValue;
                pnlInfo.Invalidate();

            }
            // Dyn info scroll down
            else if (e.KeyCode == Keys.Z)
            {
                if (!isInfoPanelShown)
                    return;
                dynInfoScrollValue += 5;
                dynInfoScrollValue = dynInfoScrollValue > infoRows - 5 ? infoRows - 5 : dynInfoScrollValue;
                pnlInfo.Invalidate();
            }
            // Dyn info scroll down fast
            else if (e.KeyCode == Keys.X)
            {
                if (!isInfoPanelShown)
                    return;
                dynInfoScrollValue += 10;
                dynInfoScrollValue = dynInfoScrollValue > infoRows - 5 ? infoRows - 5 : dynInfoScrollValue;
                pnlInfo.Invalidate();
            }
            // Grid
            else if (e.KeyCode == Keys.G)
            {
                isGridShown = !isGridShown;
                aChartButtons[(int)ChartButtons.Grid].Checked = isGridShown;
                InvalidateAllPanels();
            }
            // Cross
            else if (e.KeyCode == Keys.C)
            {
                isCrossShown = !isCrossShown;
                aChartButtons[(int)ChartButtons.Cross].Checked = isCrossShown;
                pnlPrice.Cursor = isCrossShown ? Cursors.Cross : Cursors.Default;
                InvalidateAllPanels();
            }
            // Volume
            else if (e.KeyCode == Keys.V)
            {
                isVolumeShown = !isVolumeShown;
                aChartButtons[(int)ChartButtons.Volume].Checked = isVolumeShown;
                pnlPrice.Invalidate();
            }
            // Lots
            else if (e.KeyCode == Keys.L)
            {
                isPositionLotsShown = !isPositionLotsShown;
                aChartButtons[(int)ChartButtons.PositionLots].Checked = isPositionLotsShown;
                pnlPrice.Invalidate();
            }
            // Orders
            else if (e.KeyCode == Keys.O)
            {
                isOrdersShown = !isOrdersShown;
                aChartButtons[(int)ChartButtons.Orders].Checked = isOrdersShown;
                pnlPrice.Invalidate();
            }
            // Position price
            else if (e.KeyCode == Keys.P)
            {
                isPositionPriceShown = !isPositionPriceShown;
                aChartButtons[(int)ChartButtons.PositionPrice].Checked = isPositionPriceShown;
                pnlPrice.Invalidate();
            }
            // Ambiguous bars mark
            else if (e.KeyCode == Keys.M)
            {
                isAmbiguousBarsShown = !isAmbiguousBarsShown;
                aChartButtons[(int)ChartButtons.AmbiguousBars].Checked = isAmbiguousBarsShown;
                pnlPrice.Invalidate();
            }
            // True Charts
            else if (e.KeyCode == Keys.T)
            {
                isTrueChartsShown = !isTrueChartsShown;
                aChartButtons[(int)ChartButtons.TrueCharts].Checked = isTrueChartsShown;
                InvalidateAllPanels();
            }
            // Indicator Charts
            else if (e.KeyCode == Keys.D)
            {
                isIndicatorsShown = !isIndicatorsShown;
                aChartButtons[(int)ChartButtons.Indicators].Checked = isIndicatorsShown;
                if (isIndicatorsShown)
                {
                    scroll.Parent = null;
                    for (int i = 0; i < indPanels; i++)
                    {
                        splitterInd[i].Parent = pnlCharts;
                        pnlInd[i].Parent = pnlCharts;
                    }
                    scroll.Parent = pnlCharts;
                }
                else
                {
                    for (int i = 0; i < indPanels; i++)
                    {
                        pnlInd[i].Parent = null;
                        splitterInd[i].Parent = null;
                    }
                }
                SetAllPanelsHeight();
                InvalidateAllPanels();
                scroll.Focus();
            }
            // FloatingPL Charts
            else if (e.KeyCode == Keys.F)
            {
                isFloatingPLShown = !isFloatingPLShown;
                aChartButtons[(int)ChartButtons.FloatingPL].Checked = isFloatingPLShown;
                if (isFloatingPLShown)
                {
                    scroll.Parent = null;
                    spliterFloatingPL.Parent = pnlCharts;
                    pnlFloatingPLChart.Parent = pnlCharts;
                    scroll.Parent = pnlCharts;
                }
                else
                {
                    spliterFloatingPL.Parent = null;
                    pnlFloatingPLChart.Parent = null;
                }
                SetAllPanelsHeight();
                InvalidateAllPanels();
                scroll.Focus();
            }
            // Balance/Equity Charts
            else if (e.KeyCode == Keys.B)
            {
                isBalanceEquityShown = !isBalanceEquityShown;
                aChartButtons[(int)ChartButtons.BalanceEquity].Checked = isBalanceEquityShown;
                if (isBalanceEquityShown)
                {
                    scroll.Parent = null;
                    spliterBalance.Parent = pnlCharts;
                    pnlBalanceChart.Parent = pnlCharts;
                    scroll.Parent = pnlCharts;
                }
                else
                {
                    spliterBalance.Parent = null;
                    pnlBalanceChart.Parent = null;
                }
                SetAllPanelsHeight();
                InvalidateAllPanels();
                scroll.Focus();
            }
            // Show info panel
            else if (e.KeyCode == Keys.I)
            {
                isInfoPanelShown = !isInfoPanelShown;
                pnlInfo.Visible   = isInfoPanelShown;
                pnlCharts.Padding = isInfoPanelShown ? new Padding(0, 0, 2, 0) : new Padding(0);
                Text = isInfoPanelShown ? Language.T("Chart") + " " + Data.Symbol + " " + Data.PeriodString : Data.ProgramName + @"   http://forexsb.com";
                aChartButtons[(int)ChartButtons.DynamicInfo].Checked = isInfoPanelShown;
                aChartButtons[(int)ChartButtons.DInfoUp].Visible     = isInfoPanelShown;
                aChartButtons[(int)ChartButtons.DInfoDwn].Visible    = isInfoPanelShown;
                if (isInfoPanelShown)
                {
                    GenerateDynamicInfo(lastBar);
                    SetupDynamicInfo();
                    isDrawDinInfo = true;
                    pnlInfo.Invalidate();
                }
            }
            // Debug
            else if (e.KeyCode == Keys.F12)
            {
                isDEBUG = !isDEBUG;
                SetupDynInfoWidth();
                SetupDynamicInfo();
                pnlInfo.Invalidate();
            }

            return;
        }

        /// <summary>
        /// Changes vertical scale of the Price Chart
        /// </summary>
        void VerticalScaleDecrease()
        {
            if (verticalScale > 10)
            {
                verticalScale -= 10;
                pnlPrice.Invalidate();
            }
        }

        /// <summary>
        /// Changes vertical scale of the Price Chart
        /// </summary>
        void VerticalScaleIncrease()
        {
            if (verticalScale < 300)
            {
                verticalScale += 10;
                pnlPrice.Invalidate();
            }
        }

        /// <summary>
        /// Zooms the chart in.
        /// </summary>
        void ZoomIn()
        {
            barPixels += 4;
            if (barPixels == 6)
                barPixels = 4;
            if (barPixels > 40)
                barPixels = 40;

            int oldChartBars = chartBars;

            chartBars = chartWidth / barPixels;
            if (chartBars > Data.Bars - Data.FirstBar)
                chartBars = Data.Bars - Data.FirstBar;

            if (lastBar < Data.Bars - 1)
            {
                firstBar += (oldChartBars - chartBars) / 2;
                if (firstBar > Data.Bars - chartBars)
                    firstBar = Data.Bars - chartBars;
            }
            else
            {
                firstBar = Math.Max(Data.FirstBar, Data.Bars - chartBars);
            }

            lastBar = firstBar + chartBars - 1;

            scroll.Value = firstBar;
            scroll.LargeChange = chartBars;

            InvalidateAllPanels();

            return;
        }

        /// <summary>
        /// Zooms the chart out.
        /// </summary>
        void ZoomOut()
        {
            barPixels -= 4;
            if (barPixels < 4)
                barPixels = 2;

            int oldChartBars = chartBars;

            chartBars = chartWidth / barPixels;
            if (chartBars > Data.Bars - Data.FirstBar)
                chartBars = Data.Bars - Data.FirstBar;

            if (lastBar < Data.Bars - 1)
            {
                firstBar -= (chartBars - oldChartBars) / 2;
                if (firstBar < Data.FirstBar)
                    firstBar = Data.FirstBar;

                if (firstBar > Data.Bars - chartBars)
                    firstBar = Data.Bars - chartBars;
            }
            else
            {
                firstBar = Math.Max(Data.FirstBar, Data.Bars - chartBars);
            }

            lastBar = firstBar + chartBars - 1;

            scroll.Value = firstBar;
            scroll.LargeChange = chartBars;

            InvalidateAllPanels();

            return;
        }
    }
}