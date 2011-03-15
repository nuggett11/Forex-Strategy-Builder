// Small Indicator Chart
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
    /// Draws a small indicator chart
    /// </summary>
    public class Small_Indicator_Chart : Panel
    {
        HScrollBar scrollBar;
        bool isValueChangedAktive = false;

        string       stringCaptionText;
        Font         fontCaptionText;
        float        captionHeight;
        float        captionWidth;
        Brush        brushCaptionText;
        RectangleF   rectfCaption;
        StringFormat stringFormatCaption;

        int space  = 5;
        int border = 2;
        string statusBarText;
        bool isShowDynamicInfo = false;
        int barPixels;  // Bar's width in pixels
        int chartBars;  // Count of bars in the chart
        int chartWidth; // Chart's width in pixels
        int firstBar;   // Number of the first drown bar
        int lastBar;    // Number of the last drown bar
        int clSzWidth;
        int clSzHeight;
        int xLeft;
        int xRight;
        int yTop;
        int yBottom;
        int yPrcBottom;	// Price chart y
        int inds;       // Count of separated indicators
        int indHeight;  // Height of Ind charts
        int[] aiIndSlot;
        Pen penFore;
        Pen penVolume;

        double maxPrice;
        double minPrice;
        int    maxVolume;
        double scaleY;
        double scaleYVol;

        int[] x;
        int[] yOpen;
        int[] yHigh;
        int[] yLow;
        int[] yClose;
        int[] yVolume;

        Pen penBorder;

        Rectangle[] rectPosition;
        Brush[]     brushPosition;
        int slots;
        bool[] bIsSeparatedChart;
        int[]  iComponentLenght;
        IndChartType[][] chartType;
        Point[][][]      chartLine;
        Rectangle[][][]  chartDot;
        Rectangle[][][]  chartLevel;
        double[][][]     chartValue;
        Pen[][][]        chartPen;
        Brush[][]        chartBrush;

        // Histogram
        Pen penGreen    = new Pen(Color.Green);
        Pen penRed      = new Pen(Color.Red);
        Pen penDarkGray = new Pen(Color.DarkGray);

        // Separate indicators
        int[] yIndTop;
        int[] yIndBottom;
        double[] dMaxValue;
        double[] dMinValue;
        double[] dScale;

        /// <summary>
        /// Gets or sets whether to show dynamic info or not
        /// </summary>
        public bool ShowDynamicInfo
        {
            get { return isShowDynamicInfo; }
            set { isShowDynamicInfo = value; }
        }

        /// <summary>
        /// Returnt dynamic info
        /// </summary>
        public string CurrentBarInfo
        {
            get { return statusBarText; }
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        public Small_Indicator_Chart()
        {
            Padding = new Padding(border, 0, border, border);

            // Horizontal scrol bar
            scrollBar = new HScrollBar();
            scrollBar.Parent        = this;
            scrollBar.Dock          = DockStyle.Bottom;
            scrollBar.SmallChange   = 1;
            scrollBar.LargeChange   = 50;
            scrollBar.Minimum       = 0;
            scrollBar.Maximum       = 1000;
            scrollBar.Visible       = true;
            scrollBar.ValueChanged += new EventHandler(HscrllbInstrChart_ValueChanged);
        }

        /// <summary>
        /// Sets the parameters of the Indicators Chart
        /// </summary>
        public void InitChart()
        {
            if (!Data.IsData || !Data.IsResult) return;
            barPixels = 2;
            xLeft = space;
            clSzWidth = this.ClientSize.Width;
            xRight = clSzWidth - space;
            chartWidth = xRight - xLeft;

            chartBars = chartWidth / barPixels;
            chartBars = Math.Min(chartBars, Data.Bars - Data.FirstBar);

            isValueChangedAktive = false;
            scrollBar.Minimum = Math.Max(Data.FirstBar, 0);
            scrollBar.Maximum = Math.Max(Data.Bars - 1, 1);
            scrollBar.LargeChange = Math.Max(chartBars, 1);

            firstBar = Math.Max(Data.FirstBar, Data.Bars - chartBars);
            firstBar = Math.Min(firstBar, Data.Bars - 1);
            firstBar = Math.Max(firstBar, 1);
            lastBar  = Math.Max(firstBar + chartBars - 1, firstBar);

            scrollBar.Value = firstBar;
            isValueChangedAktive = true;

            SetUpChart();
        }

        /// <summary>
        /// Prepare the parameters
        /// </summary>
        public void SetUpChart()
        {
            // Panel caption
            stringCaptionText = Language.T("Indicator Chart");
            fontCaptionText   = new Font(Font.FontFamily, 9);
            captionHeight     = Math.Max(fontCaptionText.Height, 18);
            captionWidth      = this.ClientSize.Width;
            brushCaptionText  = new SolidBrush(LayoutColors.ColorCaptionText);
            rectfCaption      = new RectangleF(0, 0, captionWidth, captionHeight);
            stringFormatCaption                = new StringFormat();
            stringFormatCaption.Alignment     |= StringAlignment.Center;
            stringFormatCaption.LineAlignment |= StringAlignment.Center;
            stringFormatCaption.Trimming      |= StringTrimming.EllipsisCharacter;
            stringFormatCaption.FormatFlags   |= StringFormatFlags.NoWrap;

            if (!Data.IsData || !Data.IsResult || Data.Bars <= Data.FirstBar) return;

            clSzWidth  = this.ClientSize.Width;
            clSzHeight = this.ClientSize.Height;
            xLeft      = space;
            xRight     = clSzWidth - space;
            yTop       = (int)captionHeight + space;
            yBottom    = clSzHeight - scrollBar.Height - space;
            yPrcBottom = yBottom;		// Price chart y
            inds       = 0; // Count of separated indicators
            indHeight  = 0; // Height of Ind charts
            aiIndSlot  = new int[12];

            penFore   = new Pen(LayoutColors.ColorChartFore);
            penVolume = new Pen(LayoutColors.ColorVolume);
            penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);

            for (int slot = Data.Strategy.Slots - 1; slot >= 0; slot--)
                if (Data.Strategy.Slot[slot].SeparatedChart)
                    aiIndSlot[inds++] = slot;

            if (inds > 0)
            {
                indHeight = (yBottom - yTop) / (2 + inds);
                yPrcBottom = yBottom - inds * indHeight;
            }

            maxPrice  = double.MinValue;
            minPrice  = double.MaxValue;
            maxVolume = int.MinValue;

            for (int bar = firstBar; bar <= lastBar; bar++)
            {
                if (Data.High[bar]   > maxPrice ) maxPrice  = Data.High[bar];
                if (Data.Low[bar]    < minPrice ) minPrice  = Data.Low[bar];
                if (Data.Volume[bar] > maxVolume) maxVolume = Data.Volume[bar];
            }
            minPrice  = Math.Round(minPrice, Data.InstrProperties.Point < 0.001 ? 3 : 1) - Data.InstrProperties.Point * 10;
            maxPrice  = Math.Round(maxPrice, Data.InstrProperties.Point < 0.001 ? 3 : 1) + Data.InstrProperties.Point * 10;
            scaleY    = (yPrcBottom - yTop) / (maxPrice - minPrice);
            scaleYVol = maxVolume > 0 ? ((yPrcBottom - yTop) / 8d) / maxVolume : 0d;

            // Volume, Lots and Price
            x       = new int[chartBars];
            yOpen   = new int[chartBars];
            yHigh   = new int[chartBars];
            yLow    = new int[chartBars];
            yClose  = new int[chartBars];
            yVolume = new int[chartBars];
            rectPosition  = new Rectangle[chartBars];
            brushPosition = new Brush[chartBars];

            int index = 0;
            for (int bar = firstBar; bar <= lastBar; bar++)
            {
                x[index]       = (bar - firstBar) * barPixels + xLeft;
                yOpen[index]   = (int)(yPrcBottom - (Data.Open[bar]   - minPrice) * scaleY);
                yHigh[index]   = (int)(yPrcBottom - (Data.High[bar]   - minPrice) * scaleY);
                yLow[index]    = (int)(yPrcBottom - (Data.Low[bar]    - minPrice) * scaleY);
                yClose[index]  = (int)(yPrcBottom - (Data.Close[bar]  - minPrice) * scaleY);
                yVolume[index] = (int)(yPrcBottom -  Data.Volume[bar] * scaleYVol);

                // Draw position lots
                if (Backtester.IsPos(bar))
                {
                    int iPosHight = (int)(Math.Max(Backtester.SummaryLots(bar) * 2, 2));

                    int iPosY = yPrcBottom - iPosHight;

                    if (Backtester.SummaryDir(bar) == PosDirection.Long)
                    {   // Long
                        rectPosition[index]  = new Rectangle(x[index], iPosY, 1, iPosHight);
                        brushPosition[index] = new SolidBrush(LayoutColors.ColorTradeLong);
                    }
                    else if (Backtester.SummaryDir(bar) == PosDirection.Short)
                    {   // Short
                        rectPosition[index]  = new Rectangle(x[index], iPosY, 1, iPosHight);
                        brushPosition[index] = new SolidBrush(LayoutColors.ColorTradeShort);
                    }
                    else
                    {   // Close position
                        rectPosition[index]  = new Rectangle(x[index], iPosY - 2, 1, 2);
                        brushPosition[index] = new SolidBrush(LayoutColors.ColorTradeClose);
                    }
                }
                else
                {   // There is no position
                    rectPosition[index]  = Rectangle.Empty;
                    brushPosition[index] = new SolidBrush(LayoutColors.ColorChartBack);
                }
                index++;
            }

            // Indicators in the chart
            slots = Data.Strategy.Slots;
            bIsSeparatedChart = new bool[slots];
            iComponentLenght  = new int[slots];
            chartType         = new IndChartType[slots][];
            chartLine         = new Point[slots][][];
            chartDot          = new Rectangle[slots][][];
            chartLevel        = new Rectangle[slots][][];
            chartValue        = new double[slots][][];
            chartPen          = new Pen[slots][][];
            chartBrush        = new Brush[slots][];
            for (int iSlot = 0; iSlot < slots; iSlot++)
            {
                bIsSeparatedChart[iSlot] = Data.Strategy.Slot[iSlot].SeparatedChart;
                int iLenght = Data.Strategy.Slot[iSlot].Component.Length;
                iComponentLenght[iSlot] = iLenght;
                chartType[iSlot]  = new IndChartType[iLenght];
                chartLine[iSlot]  = new Point[iLenght][];
                chartDot[iSlot]   = new Rectangle[iLenght][];
                chartLevel[iSlot] = new Rectangle[iLenght][];
                chartValue[iSlot] = new double[iLenght][];
                chartPen[iSlot]   = new Pen[iLenght][];
                chartBrush[iSlot] = new Brush[iLenght];
            }

            for (int slot = 0; slot < slots; slot++)
            {
                if (bIsSeparatedChart[slot]) continue;

                for (int comp = 0; comp < iComponentLenght[slot]; comp++)
                {
                    chartType[slot][comp] = Data.Strategy.Slot[slot].Component[comp].ChartType;
                    if (Data.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Line    ||
                        Data.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.CloudUp ||
                        Data.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.CloudDown )
                    {   // Line
                        chartBrush[slot][comp] = new SolidBrush(Data.Strategy.Slot[slot].Component[comp].ChartColor);
                        chartLine[slot][comp]  = new Point[lastBar - firstBar + 1];
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double dValue = Data.Strategy.Slot[slot].Component[comp].Value[bar];
                            int ix = (bar - firstBar) * barPixels + xLeft;
                            int iy = (int)(yPrcBottom - (dValue - minPrice) * scaleY);

                            if(dValue == 0)
                                chartLine[slot][comp][bar - firstBar] = chartLine[slot][comp][Math.Max(bar - firstBar - 1, 0)];
                            else
                                chartLine[slot][comp][bar - firstBar] = new Point(ix, iy);

                        }
                    }
                    else if (Data.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Dot)
                    {   // Dots
                        chartBrush[slot][comp] = new SolidBrush(Data.Strategy.Slot[slot].Component[comp].ChartColor);
                        chartDot[slot][comp]   = new Rectangle[lastBar - firstBar + 1];
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double dValue = Data.Strategy.Slot[slot].Component[comp].Value[bar];
                            int ix = (bar - firstBar) * barPixels + xLeft;
                            int iy = (int)(yPrcBottom - (dValue - minPrice) * scaleY);
                            chartDot[slot][comp][bar-firstBar] = new Rectangle(ix, iy, 1, 1);
                        }
                    }
                    else if (Data.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Level)
                    {   // Level
                        chartBrush[slot][comp] = new SolidBrush(Data.Strategy.Slot[slot].Component[comp].ChartColor);
                        chartLevel[slot][comp] = new Rectangle[lastBar - firstBar + 1];
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double dValue = Data.Strategy.Slot[slot].Component[comp].Value[bar];
                            int ix = (bar - firstBar) * barPixels + xLeft;
                            int iy = (int)(yPrcBottom - (dValue - minPrice) * scaleY);
                            chartLevel[slot][comp][bar - firstBar] = new Rectangle(ix, iy, barPixels, 1);
                        }
                    }
                }
            }

            // Separate indicators
            yIndTop    = new int[inds];
            yIndBottom = new int[inds];
            dMaxValue  = new double[inds];
            dMinValue  = new double[inds];
            dScale     = new double[inds];
            for (int ind = 0; ind < inds; ind++)
            {
                yIndTop[ind]    = yBottom - (ind + 1) * indHeight + 1;
                yIndBottom[ind] = yBottom - ind * indHeight - 1;
                dMaxValue[ind]  = double.MinValue;
                dMinValue[ind]  = double.MaxValue;
                int    iSlot = aiIndSlot[ind];
                double dValue;

                for (int iComp = 0; iComp < iComponentLenght[iSlot]; iComp++)
                    if (Data.Strategy.Slot[iSlot].Component[iComp].ChartType != IndChartType.NoChart)
                        for (bar = Math.Max(firstBar, Data.Strategy.Slot[iSlot].Component[iComp].FirstBar); bar <= lastBar; bar++)
                        {
                            dValue = Data.Strategy.Slot[iSlot].Component[iComp].Value[bar];
                            if (dValue > dMaxValue[ind]) dMaxValue[ind] = dValue;
                            if (dValue < dMinValue[ind]) dMinValue[ind] = dValue;
                        }

                dMaxValue[ind] = Math.Max(dMaxValue[ind], Data.Strategy.Slot[iSlot].MaxValue);
                dMinValue[ind] = Math.Min(dMinValue[ind], Data.Strategy.Slot[iSlot].MinValue);

                foreach (double dSpecVal in Data.Strategy.Slot[iSlot].SpecValue)
                    if (dSpecVal == 0)
                    {
                        dMaxValue[ind] = Math.Max(dMaxValue[ind], 0);
                        dMinValue[ind] = Math.Min(dMinValue[ind], 0);
                    }

                dScale[ind] = (yIndBottom[ind] - yIndTop[ind] - 2) / (Math.Max(dMaxValue[ind] - dMinValue[ind], 0.0001f));

                // Indicator chart
                for (int iComp = 0; iComp < Data.Strategy.Slot[iSlot].Component.Length; iComp++)
                {
                    chartType[iSlot][iComp] = Data.Strategy.Slot[iSlot].Component[iComp].ChartType;
                    if (Data.Strategy.Slot[iSlot].Component[iComp].ChartType == IndChartType.Line)
                    {   // Line
                        chartBrush[iSlot][iComp] = new SolidBrush(Data.Strategy.Slot[iSlot].Component[iComp].ChartColor);
                        chartLine[iSlot][iComp]  = new Point[lastBar - firstBar + 1];
                        for (bar = firstBar; bar <= lastBar; bar++)
                        {
                            dValue = Data.Strategy.Slot[iSlot].Component[iComp].Value[bar];
                            int ix = (bar - firstBar) * barPixels + xLeft;
                            int iy = (int)(yIndBottom[ind] - 1 - (dValue - dMinValue[ind]) * dScale[ind]);
                            chartLine[iSlot][iComp][bar - firstBar] = new Point(ix, iy);
                        }
                    }
                    else if (Data.Strategy.Slot[iSlot].Component[iComp].ChartType == IndChartType.Histogram)
                    {   // Histogram
                        chartValue[iSlot][iComp] = new double[lastBar - firstBar + 1];
                        chartPen[iSlot][iComp]   = new Pen[lastBar - firstBar + 1];
                        for (bar = firstBar; bar <= lastBar; bar++)
                        {
                            dValue = Data.Strategy.Slot[iSlot].Component[iComp].Value[bar];
                            chartValue[iSlot][iComp][bar - firstBar] = dValue;
                            if (dValue > Data.Strategy.Slot[iSlot].Component[iComp].Value[bar - 1])
                                chartPen[iSlot][iComp][bar - firstBar] = penGreen;
                            else
                                chartPen[iSlot][iComp][bar - firstBar] = penRed;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Paints the chart
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Data.GradientPaint(g, new Rectangle(border, (int)captionHeight, ClientSize.Width - 2 * border, ClientSize.Height - (int)captionHeight - border),
                LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            // Panel caption
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(stringCaptionText, fontCaptionText, brushCaptionText, rectfCaption, stringFormatCaption);

            // Border
            g.DrawLine(penBorder, 1, captionHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - border + 1, captionHeight, ClientSize.Width - border + 1, ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - border + 1, ClientSize.Width, ClientSize.Height - border + 1);

            if (!Data.IsData || !Data.IsResult || Data.Bars <= Data.FirstBar) return;

            // Limits the drawimg into the chart area only
            g.SetClip(new Rectangle(xLeft, yTop, xRight - xLeft, yPrcBottom - yTop));

            // Draws Volume, Lots and Price
            int index = 0;
            Pen penBar = new Pen(LayoutColors.ColorBarBorder);
            for (bar = firstBar; bar <= lastBar; bar++)
            {
                // Draw the volume
                if (yVolume[index] != yPrcBottom)
                    g.DrawLine(penVolume, x[index], yVolume[index], x[index], yPrcBottom - 1);

                // Draw position lots
                if (rectPosition[index] != Rectangle.Empty)
                    g.FillRectangle(brushPosition[index], rectPosition[index]);

                // Draw the bar
                g.DrawLine(penBar, x[index], yLow[index], x[index], yHigh[index]);
                g.DrawLine(penBar, x[index], yClose[index], x[index] + 1, yClose[index]);
                index++;
            }

            // Drawing the indicators in the chart
            for (int slot = 0; slot < slots; slot++)
            {
                if (bIsSeparatedChart[slot]) continue;
                for (int iComp = 0; iComp < iComponentLenght[slot]; iComp++)
                {
                    if (chartType[slot][iComp] == IndChartType.Line)
                    {   // Line
                        g.DrawLines(new Pen(chartBrush[slot][iComp]), chartLine[slot][iComp]);
                    }
                    else if (chartType[slot][iComp] == IndChartType.Dot)
                    {   // Dots
                        for (bar = firstBar; bar <= lastBar; bar++)
                            g.FillRectangle(chartBrush[slot][iComp], chartDot[slot][iComp][bar - firstBar]);
                    }
                    else if (chartType[slot][iComp] == IndChartType.Level)
                    {   // Level
                        for (bar = firstBar; bar <= lastBar; bar++)
                            g.FillRectangle(chartBrush[slot][iComp], chartLevel[slot][iComp][bar - firstBar]);
                    }
                    else if (chartType[slot][iComp] == IndChartType.CloudUp)
                    {   // CloudUp
                        Pen pen = new Pen(chartBrush[slot][iComp]);
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        g.DrawLines(pen, chartLine[slot][iComp]);
                    }
                    else if (chartType[slot][iComp] == IndChartType.CloudDown)
                    {   // CloudDown
                        Pen pen = new Pen(chartBrush[slot][iComp]);
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        g.DrawLines(pen, chartLine[slot][iComp]);
                    }
                }
            }
            g.ResetClip();

            // Separate indicators
            for (int ind = 0; ind < inds; ind++)
            {
                int slot = aiIndSlot[ind];

                for (int comp = 0; comp < iComponentLenght[slot]; comp++)
                {
                    if (chartType[slot][comp] == IndChartType.Line)
                    {   // Line
                        g.DrawLines(new Pen(chartBrush[slot][comp]), chartLine[slot][comp]);
                    }
                    else if (chartType[slot][comp] == IndChartType.Histogram)
                    {   // Histogram
                        double zero = 0;
                        if (zero < dMinValue[ind]) zero = dMinValue[ind];
                        if (zero > dMaxValue[ind]) zero = dMaxValue[ind];
                        int y0 = (int)(yIndBottom[ind] - (zero - dMinValue[ind]) * dScale[ind]);
                        g.DrawLine(penDarkGray, xLeft, y0, xRight, y0);
                        for (bar = firstBar; bar <= lastBar; bar++)
                        {
                            double val = chartValue[slot][comp][bar - firstBar];
                            int x = (bar - firstBar) * barPixels + xLeft;
                            int y = (int)(yIndBottom[ind] - (val - dMinValue[ind]) * dScale[ind]);
                            g.DrawLine(chartPen[slot][comp][bar - firstBar], x, y0, x, y);
                        }
                    }
                }
            }

            // Lines
            for (int ind = 0; ind < inds; ind++)
            {
                int y = yBottom - (ind + 1) * indHeight;
                g.DrawLine(penFore, xLeft, y, xRight, y);
            }
            g.DrawLine(penFore, xLeft, yBottom, xRight, yBottom);
            g.DrawLine(penFore, xLeft, yTop, xLeft, yBottom);
        }

        /// <summary>
        /// Generates dinamic info on the status bar
        /// when we are Moving the mouse over the SmallIndicatorsChart.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!isShowDynamicInfo || !Data.IsData || !Data.IsResult || Data.Bars < Data.FirstBar) return;

            int barNumb;
            barNumb = (e.X - space) / barPixels;
            barNumb = Math.Max(0, barNumb);
            barNumb = Math.Min(chartBars - 1, barNumb);

            int bar = Math.Min(Data.Bars - 1, firstBar + barNumb);

            statusBarText =
                    Data.Time[bar].ToString(Data.DF)  + " "   +
                    Data.Time[bar].ToString("HH:mm")  + " O:" +
                    Data.Open[bar].ToString(Data.FF)  + " H:" +
                    Data.High[bar].ToString(Data.FF)  + " L:" +
                    Data.Low[bar].ToString(Data.FF)   + " C:" +
                    Data.Close[bar].ToString(Data.FF) + " V:" +
                    Data.Volume[bar].ToString();
        }

        /// <summary>
        /// Sets the parameters after the horisontal scrollbar position has been changed.
        /// </summary>
        void HscrllbInstrChart_ValueChanged(object sender, EventArgs e)
        {
            if (!isValueChangedAktive) return;

            firstBar = scrollBar.Value;
            lastBar  = Math.Max(firstBar + chartBars - 1, firstBar);

            this.SetUpChart();
            Rectangle rect = new Rectangle(xLeft + 1, yTop, xRight - xLeft, yBottom - yTop);
            this.Invalidate(rect);
        }

        /// <summary>
        /// Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            this.InitChart();
            this.Invalidate();
        }


        public int bar { get; set; }
    }
}
