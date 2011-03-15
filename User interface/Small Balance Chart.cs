// Small Balance Chart
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Draws a small balance chart
    /// </summary>
    public class Small_Balance_Chart : Panel
    {
        int    space  = 5;
        int    border = 2;

        int    XLeft;
        int    XRight;
        int    YTop;
        int    YBottom;
        float  XScale;
        float  YScale;
        float  YPriceScale;
        bool   showPriceLine;

        int   countLabels;
        float delta;
        int   step;

        string strStatusBarText;
        bool   isShowDynamicInfo = false;

        int    bars;
        int    chartBars;
        int    maximum;
        int    minimum;
        int    labelWidth;
        string strChartTitle;
        Font   font;
        float  captionHeight;
        RectangleF   rectfCaption;
        StringFormat stringFormatCaption;
        Brush brushFore;
        Pen   penGrid;
        Pen   penBorder;
        PointF[] apntBalance;
        PointF[] apntEquity;
        PointF[] apntLongBalance;
        PointF[] apntShortBalance;
        PointF[] apntClosePrice;
        float  YBalance;
        float  balance;
        float  XMarginCallBar;
        int    marginCallBar;
        int    firstBar;
        bool   isHideScanningLine;
        bool   isScanPerformed;
        string modellingQuolity;

        // Out of Sample
        int   barOOS = Data.Bars - 1;
        bool  isOOS  = false;
        float XOOSBar;

        /// <summary>
        /// Whether to show dynamic info
        /// </summary>
        public bool ShowDynamicInfo {  get { return isShowDynamicInfo; } set { isShowDynamicInfo = value; } }

        /// <summary>
        /// Returnt dynamic info
        /// </summary>
        public string CurrentBarInfo { get { return strStatusBarText; } }

        /// <summary>
        /// Returnt X Left
        /// </summary>
        public int XLeftMargin { get { return border + space; } }

        /// <summary>
        /// Returnt X Right
        /// </summary>
        public int XRightMargin {  get { return XRight - space; } }

        /// <summary>
        /// Set the OOS Bar
        /// </summary>
        public int OOSBar { set { barOOS = value; } }

        /// <summary>
        /// Set the OOS
        /// </summary>
        public bool OOS { set { isOOS = value; } }

        /// <summary>
        /// Sets the chart params
        /// </summary>
        public void InitChart()
        {
            // Chart Title
            strChartTitle = Language.T("Balance / Equity Chart") + " [" + (Configs.AccountInMoney ? Configs.AccountCurrency + "]" : Language.T("pips") + "]");
            font          = new Font(Font.FontFamily, 9);
            captionHeight = (float)Math.Max(font.Height, 18);
            rectfCaption  = new RectangleF(0, 0, ClientSize.Width, captionHeight);
            stringFormatCaption               = new StringFormat();
            stringFormatCaption.Alignment     = StringAlignment.Center;
            stringFormatCaption.LineAlignment = StringAlignment.Center;
            stringFormatCaption.Trimming      = StringTrimming.EllipsisCharacter;
            stringFormatCaption.FormatFlags   = StringFormatFlags.NoWrap;

            brushFore   = new SolidBrush(LayoutColors.ColorChartFore);
            penGrid     = new Pen(LayoutColors.ColorChartGrid);
            penGrid.DashStyle   = DashStyle.Dash;
            penGrid.DashPattern = new float [] {4, 2};
            penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);

            if (!Data.IsData || !Data.IsResult || Data.Bars <= Data.FirstBar) return;

            firstBar   = Data.FirstBar;
            bars       = Data.Bars;
            chartBars  = Data.Bars - firstBar;
            int maxBalance = Configs.AccountInMoney ? (int)Backtester.MaxMoneyBalance : Backtester.MaxBalance;
            int minBalance = Configs.AccountInMoney ? (int)Backtester.MinMoneyBalance : Backtester.MinBalance;
            int maxEquity  = Configs.AccountInMoney ? (int)Backtester.MaxMoneyEquity  : Backtester.MaxEquity;
            int minEquity  = Configs.AccountInMoney ? (int)Backtester.MinMoneyEquity  : Backtester.MinEquity;

            if (Configs.AdditionalStatistics)
            {
                int maxLongBalance  = Configs.AccountInMoney ? (int)Backtester.MaxLongMoneyBalance : Backtester.MaxLongBalance;
                int minLongBalance  = Configs.AccountInMoney ? (int)Backtester.MinLongMoneyBalance : Backtester.MinLongBalance;
                int maxShortBalance = Configs.AccountInMoney ? (int)Backtester.MaxShortMoneyBalance : Backtester.MaxShortBalance;
                int minShortBalance = Configs.AccountInMoney ? (int)Backtester.MinShortMoneyBalance : Backtester.MinShortBalance;
                int maxLSBalance = Math.Max(maxLongBalance, maxShortBalance);
                int minLSBalance = Math.Min(minLongBalance, minShortBalance);

                maximum = Math.Max(Math.Max(maxBalance, maxEquity), maxLSBalance) + 1;
                minimum = Math.Min(Math.Min(minBalance, minEquity), minLSBalance) - 1;
            }
            else
            {
                maximum = Math.Max(maxBalance, maxEquity) + 1;
                minimum = Math.Min(minBalance, minEquity) - 1;
            }

            minimum = (int)(Math.Floor(minimum / 10f) * 10);

            YTop    = (int)captionHeight + 2 * space + 1;
            YBottom = ClientSize.Height   - 2 * space - 1 - border;

            Graphics  g = CreateGraphics();
            labelWidth = (int)Math.Max(g.MeasureString(minimum.ToString(), Font).Width, g.MeasureString(maximum.ToString(), Font).Width);
            g.Dispose();

            labelWidth = Math.Max(labelWidth, 30);
            XLeft  = border + space; 
            XRight = ClientSize.Width - border - space - labelWidth;
            XScale = (XRight - 2 * space - border) / (float)chartBars;

            countLabels = (int)Math.Max((YBottom - YTop) / 20, 1);
            delta       = (float)Math.Max(Math.Round((maximum - minimum) / (float)countLabels), 10);
            step        = (int)Math.Ceiling(delta / 10) * 10;
            countLabels = (int)Math.Ceiling((maximum - minimum) / (float)step);
            maximum     = minimum + countLabels * step;
            YScale      = (YBottom - YTop) / (countLabels * (float)step);

            apntBalance = new PointF[chartBars];
            apntEquity  = new PointF[chartBars];

            if (Configs.AdditionalStatistics)
            {
                apntLongBalance  = new PointF[chartBars];
                apntShortBalance = new PointF[chartBars];
            }

            apntClosePrice = new PointF[chartBars];

            // Close Price
            showPriceLine = Configs.ShowPriceChartOnAccountChart && Backtester.ExecutedOrders > 0;
            if (showPriceLine)
                YPriceScale = (float)((YBottom - YTop) / (Data.MaxPrice - Data.MinPrice));

            int index = 0;
            for (int iBar = firstBar; iBar < bars; iBar++)
            {
                apntBalance[index].X = XLeft + index * XScale;
                apntEquity[index].X  = XLeft + index * XScale;
                if (Configs.AccountInMoney)
                {
                    apntBalance[index].Y = (float)(YBottom - (Backtester.MoneyBalance(iBar) - minimum) * YScale);
                    apntEquity[index].Y  = (float)(YBottom - (Backtester.MoneyEquity(iBar)  - minimum) * YScale);
                }
                else
                {
                    apntBalance[index].Y = YBottom - (Backtester.Balance(iBar) - minimum) * YScale;
                    apntEquity[index].Y  = YBottom - (Backtester.Equity(iBar)  - minimum) * YScale;
                }

                if (Configs.AdditionalStatistics)
                {
                    apntLongBalance[index].X  = XLeft + index * XScale;
                    apntShortBalance[index].X = XLeft + index * XScale;
                    if (Configs.AccountInMoney)
                    {
                        apntLongBalance[index].Y  = (float)(YBottom - (Backtester.LongMoneyBalance(iBar)  - minimum) * YScale);
                        apntShortBalance[index].Y = (float)(YBottom - (Backtester.ShortMoneyBalance(iBar) - minimum) * YScale);
                    }
                    else
                    {
                        apntLongBalance[index].Y  = YBottom - (Backtester.LongBalance(iBar)  - minimum) * YScale;
                        apntShortBalance[index].Y = YBottom - (Backtester.ShortBalance(iBar) - minimum) * YScale;
                    }
                }

                if (showPriceLine)
                {
                    apntClosePrice[index].X = XLeft + index * XScale;
                    apntClosePrice[index].Y = YBottom - (float)(Data.Close[iBar] - Data.MinPrice) * YPriceScale;
                }
                index++;
            }

            balance = (float)(Configs.AccountInMoney ? Backtester.NetMoneyBalance : Backtester.NetBalance);

            // Margin Call
            marginCallBar = Backtester.MarginCallBar;
            if (Backtester.MarginCallBar >= firstBar)
                XMarginCallBar = XLeft + (Backtester.MarginCallBar - firstBar) * XScale;
            else
                XMarginCallBar = 0;

            //OOS
            if (isOOS && barOOS > firstBar)
            {
                XOOSBar = XLeft + (barOOS - firstBar) * XScale;
                balance = (float)(Configs.AccountInMoney ? Backtester.MoneyBalance(barOOS) : Backtester.Balance(barOOS));
                
            }
            else
            {
                balance = (float)(Configs.AccountInMoney ? Backtester.NetMoneyBalance : Backtester.NetBalance);
                XOOSBar = 0;
            }
            YBalance   = YBottom - (balance - minimum) * YScale;

            isHideScanningLine = false;
            isScanPerformed  = Backtester.IsScanPerformed;
            modellingQuolity = " MQ " + Data.ModellingQuality.ToString("N2") + "%";
        }

        /// <summary>
        /// Paints the chart
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Caption bar
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(strChartTitle, Font, new SolidBrush(LayoutColors.ColorCaptionText), rectfCaption, stringFormatCaption);

            // Border
            g.DrawLine(penBorder, 1, captionHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - border + 1, captionHeight, ClientSize.Width - border + 1, ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - border + 1, ClientSize.Width, ClientSize.Height - border + 1);

            // Paints the background by gradient
            RectangleF rectField = new RectangleF(border, captionHeight, ClientSize.Width - 2 * border, ClientSize.Height - captionHeight - border);
            Data.GradientPaint(g, rectField, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            if (!Data.IsData || !Data.IsResult || Data.Bars <= Data.FirstBar) return;

            // Grid and Price labels
            for (int iLabel = minimum; iLabel <= maximum; iLabel += step)
            {
                int iLabelY = (int)(YBottom - (iLabel - minimum) * YScale);
                g.DrawString(iLabel.ToString(), Font, brushFore, XRight, iLabelY - Font.Height / 2 - 1);
                g.DrawLine(penGrid, XLeft, iLabelY, XRight, iLabelY);
            }

            // Out of Sample
            if (isOOS && barOOS > 0)
            {
                g.DrawLine(new Pen(LayoutColors.ColorChartFore), XOOSBar, YTop, XOOSBar, YBottom);
                Brush brushOOS = new Pen(LayoutColors.ColorChartFore).Brush;
                g.DrawString("OOS", Font, brushOOS, XOOSBar, YBottom - Font.Height);
                float fOOSBarDateWith = g.MeasureString(Data.Time[barOOS].ToShortDateString(), Font).Width;
                g.DrawString(Data.Time[barOOS].ToShortDateString(), Font, brushOOS, XOOSBar - fOOSBarDateWith, YBottom - Font.Height);
            }

            // Price close
            if (showPriceLine)
                g.DrawLines(new Pen(LayoutColors.ColorChartGrid), apntClosePrice);

            // Equity line
            g.DrawLines(new Pen(LayoutColors.ColorChartEquityLine), apntEquity);

            // Draw Long and Short balance
            if (Configs.AdditionalStatistics)
            {
                g.DrawLines(new Pen(Color.Red),  apntShortBalance);
                g.DrawLines(new Pen(Color.Green), apntLongBalance);
            }

            // In case of Margin Call
            if (marginCallBar > 0)
            {
                PointF[] apntfGreenBalance = new PointF[marginCallBar - firstBar];
                for (int i = 0; i < apntfGreenBalance.Length; i++)
                    apntfGreenBalance[i] = apntBalance[i];

                PointF[] apntfRedBalance = new PointF[Data.Bars - marginCallBar];
                for (int i = 0; i < apntfRedBalance.Length; i++)
                    apntfRedBalance[i] = apntBalance[i + marginCallBar - firstBar];

                if (apntfGreenBalance.Length > 0)
                    g.DrawLines(new Pen(LayoutColors.ColorChartBalanceLine), apntfGreenBalance);
                g.DrawLines(new Pen(LayoutColors.ColorSignalRed), apntfRedBalance);

                // Margin Call line
                g.DrawLine(new Pen(LayoutColors.ColorChartCross), XMarginCallBar, YTop, XMarginCallBar, YBottom);
                Brush brushBancrupcy = new Pen(LayoutColors.ColorChartBalanceLine).Brush;
                float fMarginCallWidht = g.MeasureString(Language.T("Margin Call"), Font).Width;
                if (XMarginCallBar < XRight - fMarginCallWidht)
                    g.DrawString(Language.T("Margin Call"), Font, brushFore, XMarginCallBar, YTop);
                else if (XMarginCallBar > space + fMarginCallWidht)
                    g.DrawString(Language.T("Margin Call"), Font, brushFore, XMarginCallBar - fMarginCallWidht, YTop);
                else
                    g.DrawString("MC", Font, brushFore, XMarginCallBar, YTop);
            }
            else
            {   // Draw the balance line
                g.DrawLines(new Pen(LayoutColors.ColorChartBalanceLine), apntBalance);
            }

            // Balance level
            g.DrawLine(new Pen(LayoutColors.ColorChartCross), XLeft, YBalance, XRight - space + 1, YBalance);

            // Scanning note
            Font fontNote = new Font(Font.FontFamily, Font.Size - 1);
            if (Data.Period != DataPeriods.min1 && Configs.Autoscan && !Data.IsIntrabarData)
                g.DrawString(Language.T("Load intrabar data"), fontNote, Brushes.Red, XLeft, captionHeight - 2);
            else if (Data.Period != DataPeriods.min1 && isScanPerformed)
                g.DrawString(Language.T("Scanned") + modellingQuolity, fontNote, Brushes.LimeGreen, XLeft, captionHeight - 2);

            // Scanned bars
            if (isScanPerformed && !isHideScanningLine && 
                (Data.IntraBars != null && Data.IsIntrabarData ||
                 Data.Period == DataPeriods.min1 && Data.IsTickData && Configs.UseTickData))
            {
                DataPeriods dataPeriod = Data.Period;
                Color color = Data.PeriodColor[Data.Period];
                int   fromBar = Data.FirstBar;
                for (int bar = Data.FirstBar; bar < Data.Bars; bar++)
                {
                    if (Data.IntraBarsPeriods[bar] != dataPeriod || bar == Data.Bars - 1)
                    {
                        int xStart = (int)((fromBar - Data.FirstBar) * XScale) + XLeft;
                        int xEnd   = (int)((bar     - Data.FirstBar) * XScale) + XLeft;
                        fromBar = bar;
                        dataPeriod = Data.IntraBarsPeriods[bar];
                        Data.GradientPaint(g, new RectangleF(xStart, YBottom + 4, xEnd - xStart + 2, 5), color, 60);
                        color = Data.PeriodColor[Data.IntraBarsPeriods[bar]];
                    }
                }

                // Tick Data
                if (Data.IsTickData && Configs.UseTickData)
                {
                    int firstBarWithTicks = -1;
                    int lastBarWithTicks  = -1;
                    for (int b = 0; b < Data.Bars; b++)
                    {
                        if (firstBarWithTicks == -1 && Data.TickData[b] != null)
                            firstBarWithTicks = b;
                        if (Data.TickData[b] != null)
                            lastBarWithTicks = b;
                    }
                    int xStart = (int)((firstBarWithTicks - Data.FirstBar) * XScale) + XLeft;
                    int xEnd   = (int)((lastBarWithTicks  - Data.FirstBar) * XScale) + XLeft;
                    Data.GradientPaint(g, new RectangleF(xStart, YBottom + 4, xEnd - xStart + 2, 5), color, 60);

                    RectangleF rectf = new RectangleF(xStart, YBottom + 4, xEnd - xStart + 2, 5);
                    Data.GradientPaint(g, rectf, Data.PeriodColor[DataPeriods.min1], 60);
                    rectf = new RectangleF(xStart, YBottom + 6, xEnd - xStart + 2, 1);
                    Data.GradientPaint(g, rectf, Data.PeriodColor[DataPeriods.day], 60);
                }

                // Vertical coordinate axes
                g.DrawLine(new Pen(LayoutColors.ColorChartFore), XLeft - 1, YBottom, XLeft - 1, YBottom + 9);
            }

            // Coordinate axes
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), XLeft - 1, YTop - space, XLeft - 1, YBottom + 1);
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), XLeft - 1, YBottom + 1, XRight, YBottom + 1);

            // Balance label
            Size szBalance = new Size(labelWidth + space, Font.Height + 2);
            Point point = new Point(XRight - space + 2, (int)(YBalance - Font.Height / 2 - 1));
            Rectangle rec = new Rectangle(point, szBalance);
            g.FillRectangle(new SolidBrush(LayoutColors.ColorLabelBack), rec);
            g.DrawRectangle(new Pen(LayoutColors.ColorChartCross), rec);
            g.DrawString((Math.Round(balance)).ToString(), Font, new SolidBrush(LayoutColors.ColorLabelText), rec, stringFormatCaption);

            return;
        }

        /// <summary>
        /// Generates dinamic info on the status bar
        /// when we are Moving the mouse over the SmallBalanceChart.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!isShowDynamicInfo || !Data.IsData || !Data.IsResult) return;

            int bar = (int)((e.X - XLeft) / XScale) + firstBar;

            bar = Math.Max(firstBar, bar);
            bar = Math.Min(Data.Bars - 1, bar);

            if (Configs.AccountInMoney)
                strStatusBarText = String.Format("{0} {1} {2}: {3} {4} {5}: {6} {7}",
                    Data.Time[bar].ToString(Data.DF),
                    Data.Time[bar].ToString("HH:mm"),
                    Language.T("Balance"),
                    Backtester.MoneyBalance(bar).ToString("F2"),
                    Configs.AccountCurrency,
                    Language.T("Equity"),
                    Backtester.MoneyEquity(bar).ToString("F2"),
                    Configs.AccountCurrency);
            else
                strStatusBarText = String.Format("{0} {1} {2}: {3} {4} {5}: {6} {7}",
                    Data.Time[bar].ToString(Data.DF),
                    Data.Time[bar].ToString("HH:mm"),
                    Language.T("Balance"),
                    Backtester.Balance(bar),
                    Language.T("pips"),
                    Language.T("Equity"),
                    Backtester.Equity(bar),
                    Language.T("pips"));

            if (Configs.AdditionalStatistics)
            {
                if (Configs.AccountInMoney)
                    strStatusBarText += String.Format(" {0}: {1} {2} {3}: {4} {5}",
                        Language.T("Long balance"),
                        Backtester.LongMoneyBalance(bar).ToString("F2"),
                        Configs.AccountCurrency,
                        Language.T("Short balance"),
                        Backtester.ShortMoneyBalance(bar).ToString("F2"),
                        Configs.AccountCurrency);
                else
                    strStatusBarText += String.Format(" {0}: {1} {2} {3}: {4} {5}",
                        Language.T("Long balance"),
                        Backtester.LongBalance(bar),
                        Language.T("pips"),
                        Language.T("Short balance"),
                        Backtester.ShortBalance(bar),
                        Language.T("pips"));
            }
            if (Configs.ShowPriceChartOnAccountChart)
                strStatusBarText += String.Format(" {0}: {1}",
                        Language.T("Price close"),
                        Data.Close[bar]);
        }

        /// <summary>
        /// Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            InitChart();
            Invalidate();
        }
    }
}
