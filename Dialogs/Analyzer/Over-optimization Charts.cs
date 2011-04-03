// Strategy Analyzer - OverOptimization Charts
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Analyzer
{
    class Over_optimization_Charts : Panel
    {
        Over_optimization_Data_Table table;

        string chartTitle;

        int space  = 5;
        int border = 2;

        int XLeft;
        int XMiddle;
        int XRight;
        int YTop;
        int YBottom;
        float XScale;
        float YScale;

        int countLabels;
        float delta;
        int step;

        int percentDev;
        int devSteps;
        int parameters;

        int minimum;
        int maximum;
        int labelWidth;
        Font font;
        float captionHeight;
        RectangleF rectfCaption;
        StringFormat stringFormatCaption;
        Brush brushFore;
        Pen penGrid;
        Pen penBorder;

        PointF[][] apntParameters;

        public Over_optimization_Charts()
        {
        }

        /// <summary>
        /// Sets the chart parameters
        /// </summary>
        public void InitChart(Over_optimization_Data_Table table)
        {
            this.table = table;

            // Chart Title
            chartTitle    = table.Name;
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
            penBorder   = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);

            devSteps   = table.CountDeviationSteps;
            percentDev = (devSteps - 1) / 2;
            parameters = table.CountStrategyParams;

            if (parameters == 0)
                return;

            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            for (int param = 0; param < parameters; param++)
            {
                double min = double.MaxValue;
                double max = double.MinValue;
                for (int dev = 0; dev < devSteps; dev++)
                {
                    int index = percentDev - dev;
                    double value = table.GetData(index, param);
                    if (min > value) min = value;
                    if (max < value) max = value;
                }
                if (minValue > min) minValue = min;
                if (maxValue < max) maxValue = max;
            }

            maximum = (int)Math.Round(maxValue + 0.1 * Math.Abs(maxValue));
            minimum = (int)Math.Round(minValue - 0.1 * Math.Abs(minValue));
            int roundStep = Math.Abs(minimum) > 1 ? 10 : 1;
            minimum = (int)(Math.Floor(minimum / (float)roundStep) * roundStep);
            if (Math.Abs(maximum - minimum) < 1) maximum = minimum + 1;

            YTop    = (int)captionHeight + 2 * space + 1;
            YBottom = ClientSize.Height  - 2 * space - 1 - border - space - Font.Height;

            Graphics  g = CreateGraphics();
            labelWidth = (int)Math.Max(g.MeasureString(minimum.ToString(), font).Width, g.MeasureString(maximum.ToString(), font).Width);
            g.Dispose();

            labelWidth = Math.Max(labelWidth, 30);
            XLeft   = border + 3 *space;
            XRight  = ClientSize.Width - border - 2 * space - labelWidth;
            XScale  = (XRight - XLeft) / (float)(devSteps - 1);
            XMiddle = (int)(XLeft + percentDev * XScale);

            countLabels = (int)Math.Max((YBottom - YTop) / 20, 2);
            delta       = (float)Math.Max(Math.Round((maximum - minimum) / (float)countLabels), roundStep);
            step        = (int)Math.Ceiling(delta / roundStep) * roundStep;
            countLabels = (int)Math.Ceiling((maximum - minimum) / (float)step);
            maximum     = minimum + countLabels * step;
            YScale      = (YBottom - YTop) / (countLabels * (float)step);

            apntParameters = new PointF[parameters][];

            for (int param = 0; param < parameters; param++)
            {
                apntParameters[param] = new PointF[devSteps];
                for (int dev = 0; dev < devSteps; dev++)
                {
                    int index = percentDev - dev;
                    apntParameters[param][dev].X = XLeft + dev * XScale;
                    apntParameters[param][dev].Y = (float)(YBottom - (table.GetData(index, param) - minimum) * YScale);
                }
            }
        }

        /// <summary>
        /// Paints the chart
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {

            Graphics g = e.Graphics;

            // Caption bar.
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(chartTitle, Font, new SolidBrush(LayoutColors.ColorCaptionText), rectfCaption, stringFormatCaption);

            // Border.
            g.DrawLine(penBorder, 1, captionHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - border + 1, captionHeight, ClientSize.Width - border + 1, ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - border + 1, ClientSize.Width, ClientSize.Height - border + 1);

            // Paints the background by gradient.
            RectangleF rectField = new RectangleF(border, captionHeight, ClientSize.Width - 2 * border, ClientSize.Height - captionHeight - border);
            Data.GradientPaint(g, rectField, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            if (table == null || parameters == 0)
                return;

            // Grid and value labels.
            for (int label = minimum; label <= maximum; label += step)
            {
                int iLabelY = (int)(YBottom - (label - minimum) * YScale);
                g.DrawString(label.ToString(), Font, brushFore, XRight + space, iLabelY - Font.Height / 2 - 1);
                g.DrawLine(penGrid, XLeft, iLabelY, XRight, iLabelY);
            }

            // Deviation labels.
            StringFormat strFormatDevLabel = new StringFormat();
            strFormatDevLabel.Alignment = StringAlignment.Center;
            int XLabelRight = 0;
            for (int dev = 0; dev < devSteps; dev++)
            {
                int index = percentDev - dev;
                int XVertLine = (int)(XLeft + dev * XScale);
                g.DrawLine(penGrid, XVertLine, YTop, XVertLine, YBottom + 2);
                string devLabel = index.ToString() + (index != 0 ? "%" : "");
                int devLabelWidth = (int)g.MeasureString(devLabel, font).Width;
                if (XVertLine - devLabelWidth / 2 > XLabelRight)
                {
                    g.DrawString(devLabel, font, brushFore, XVertLine, YBottom + space, strFormatDevLabel);
                    XLabelRight = XVertLine + devLabelWidth / 2;
                }
            }

            // Vertical coordinate axes.
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), XMiddle - 1, YTop - space, XMiddle - 1, YBottom + 1);

            // Chart lines
            for (int param = 0; param < parameters; param++)
                g.DrawLines(new Pen(GetNextColor(param)), apntParameters[param]);

            // Horizontal coordinate axes.
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), XLeft - 1, YBottom + 1, XRight, YBottom + 1);

            return;
        }

        /// <summary>
        /// Gets color for the chart lines.
        /// </summary>
        Color GetNextColor(int param)
        {
            Color[] colors = new Color[]
            {
                Color.DarkViolet,
                Color.Red,
                Color.Peru,
                Color.DarkSalmon,
                Color.Orange,
                Color.Green,
                Color.Lime,
                Color.Gold
            };

            if (param >= colors.Length)
                param = param % colors.Length;

            return colors[param];
        }

        /// <summary>
        /// Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            if (table == null)
                return;

            InitChart(table);
            Invalidate();
        }
    }
}
