// Micro Balance Chart
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
    /// Draws a micro balance chart
    /// </summary>
    public class Micro_Balance_Chart : Panel
    {
        int border = 1;
        int space  = 2;

        int XLeft, XRight, YTop, YBottom;
        float XScale, YScale;

        int bars;
        int chartBars;
        int maximum;
        int minimum;
        int firstBar;
        Pen penBorder;
        PointF[] apntBalance;
        PointF[] apntEquity;
        PointF[] apntLongBalance;
        PointF[] apntShortBalance;

        /// <summary>
        /// Sets the chart params
        /// </summary>
        public void InitChart()
        {

            if (!Data.IsData || !Data.IsResult || Data.Bars <= Data.FirstBar) return;

            firstBar  = Data.FirstBar;
            bars      = Data.Bars;
            chartBars = Data.Bars - firstBar;
            int iMaxBalance = Configs.AccountInMoney ? (int)Backtester.MaxMoneyBalance : Backtester.MaxBalance;
            int iMinBalance = Configs.AccountInMoney ? (int)Backtester.MinMoneyBalance : Backtester.MinBalance;
            int iMaxEquity  = Configs.AccountInMoney ? (int)Backtester.MaxMoneyEquity  : Backtester.MaxEquity;
            int iMinEquity  = Configs.AccountInMoney ? (int)Backtester.MinMoneyEquity  : Backtester.MinEquity;

            if (Configs.AdditionalStatistics)
            {
                int iMaxLongBalance  = Configs.AccountInMoney ? (int)Backtester.MaxLongMoneyBalance  : Backtester.MaxLongBalance;
                int iMinLongBalance  = Configs.AccountInMoney ? (int)Backtester.MinLongMoneyBalance  : Backtester.MinLongBalance;
                int iMaxShortBalance = Configs.AccountInMoney ? (int)Backtester.MaxShortMoneyBalance : Backtester.MaxShortBalance;
                int iMinShortBalance = Configs.AccountInMoney ? (int)Backtester.MinShortMoneyBalance : Backtester.MinShortBalance;
                int iMaxLSBalance = Math.Max(iMaxLongBalance, iMaxShortBalance);
                int iMinLSBalance = Math.Min(iMinLongBalance, iMinShortBalance);

                maximum = Math.Max(Math.Max(iMaxBalance, iMaxEquity), iMaxLSBalance) + 1;
                minimum = Math.Min(Math.Min(iMinBalance, iMinEquity), iMinLSBalance) - 1;
            }
            else
            {
                maximum = Math.Max(iMaxBalance, iMaxEquity) + 1;
                minimum = Math.Min(iMinBalance, iMinEquity) - 1;
            }

            YTop    = border + space;
            YBottom = ClientSize.Height - border - space;
            XLeft   = border;
            XRight  = ClientSize.Width - border - space;
            XScale  = (XRight - XLeft) / (float)chartBars;
            YScale  = (YBottom - YTop) / (float)(maximum - minimum);

            penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);

            apntBalance = new PointF[chartBars];
            apntEquity  = new PointF[chartBars];

            if (Configs.AdditionalStatistics)
            {
                apntLongBalance  = new PointF[chartBars];
                apntShortBalance = new PointF[chartBars];
            }

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

                index++;
            }
        }

        /// <summary>
        /// Paints the chart
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Border
            g.DrawRectangle(penBorder, 1, 1, ClientSize.Width - 1, ClientSize.Height - 1);

            // Paints the background by gradient
            RectangleF rectField = new RectangleF(1, 1, ClientSize.Width - 2, ClientSize.Height - 2);
            g.FillRectangle(new SolidBrush(LayoutColors.ColorChartBack), rectField);
            //Data.GradientPaint(g, rectField, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            if (!Data.IsData || !Data.IsResult || Data.Bars <= Data.FirstBar) return;

            // Equity line
            g.DrawLines(new Pen(LayoutColors.ColorChartEquityLine), apntEquity);

            // Draw Long and Short balance
            if (Configs.AdditionalStatistics)
            {
                g.DrawLines(new Pen(Color.Red),  apntShortBalance);
                g.DrawLines(new Pen(Color.Green), apntLongBalance);
            }

            // Draw the balance line
            g.DrawLines(new Pen(LayoutColors.ColorChartBalanceLine), apntBalance);

            return;
        }

        /// <summary>
        /// Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            Invalidate();
        }
    }
}
