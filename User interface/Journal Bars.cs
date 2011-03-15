// Journal Bars
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    public class Journal_Bars : Panel
    {
        VScrollBar vScrollBar;
        HScrollBar hScrollBar;
        ToolTip    toolTip;

        Image[]   aiPositionIcons; // Shows the position's type and transaction
        string[]  asTitlesPips;    // Journal title second row
        string[]  asTitlesMoney;   // Journal title second row
        string[,] asJournalData;   // The text journal data

        int[] aiColumnX;  // The horizontal position of the column
        int[] aiX;        // The scalled horizontal position of the column
        int   columns;    // The number of the columns
        int   rowHeight;  // The journal row height
        int   border = 2; // The width of outside border of the panel

        int rows;           // The nuber of bars can be shown (without the caption bar)
        int bars;           // The total number of the bars
        int firstBar;       // The number of the first shown bar
        int lastBar;        // The number of the last shown bar
        int shownBars;      // How many bars are shown
        int selectedRow;    // The nuber of the selected row
        int selectedBarOld; // The old selected bar

        Font  font;
        Color colorBack;
        Color colorCaptionBack;
        Brush brushCaptionText;
        Brush brushEvenRowBack;
        Brush brushWarningBack;
        Brush brushWarningText;
        Brush brushSelectedRowBack;
        Brush brushSelectedRowText;
        Brush brushRowText;
        Pen   penLines;
        Pen   penBorder;
        Pen   penSeparator;

        public event EventHandler SelectedBarChange;

        /// <summary>
        /// Gets the selected bar
        /// </summary>
        public int SelectedBar
        {
            get
            {
                return firstBar + selectedRow;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Journal_Bars()
        {
            InitializeJournal();
            SetUpJournal();
            SetJournalColors();

            return;
        }

        /// <summary>
        /// Sets the journal's current data
        /// </summary>
        public void SetUpJournal()
        {
            bars = Data.Bars;

            if (bars == 0)
            {
                firstBar  = 0;
                lastBar   = 0;
                shownBars = 0;

                vScrollBar.Enabled = false;
            }
            else if (bars < rows)
            {
                firstBar  = 0;
                lastBar   = rows;
                shownBars = bars;

                vScrollBar.Enabled = false;
            }
            else
            {
                vScrollBar.Enabled = true;
                vScrollBar.Maximum = bars - 1;

                firstBar = vScrollBar.Value;
                if (firstBar + rows > bars)
                {
                    lastBar   = bars - 1;
                    shownBars = lastBar - firstBar + 1;
                }
                else
                {
                    shownBars = rows;
                    lastBar   = firstBar + shownBars - 1;
                }
            }

            selectedRow = Math.Min(selectedRow, shownBars - 1);
            selectedRow = Math.Max(selectedRow, 0);

            UpdateJournalData();
            SetJournalColors();

            return;
        }

        /// <summary>
        /// Sets the journal colors
        /// </summary>
        void SetJournalColors()
        {
            colorBack            = LayoutColors.ColorControlBack;
            colorCaptionBack     = LayoutColors.ColorCaptionBack;
            brushCaptionText     = new SolidBrush(LayoutColors.ColorCaptionText);
            brushEvenRowBack     = new SolidBrush(LayoutColors.ColorEvenRowBack);
            brushSelectedRowBack = new SolidBrush(LayoutColors.ColorSelectedRowBack);
            brushSelectedRowText = new SolidBrush(LayoutColors.ColorSelectedRowText);
            brushRowText         = new SolidBrush(LayoutColors.ColorControlText);
            brushWarningBack     = new SolidBrush(LayoutColors.ColorWarningRowBack);
            brushWarningText     = new SolidBrush(LayoutColors.ColorWarningRowText);
            penLines     = new Pen(LayoutColors.ColorJournalLines);
            penBorder    = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);
            penSeparator = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -2 * LayoutColors.DepthCaption));

            return;
        }

        /// <summary>
        /// Inits the Journal
        /// </summary>
        void InitializeJournal()
        {
            // Tool Tips
            toolTip = new ToolTip();

            // Horizontal ScrollBar
            hScrollBar = new HScrollBar();
            hScrollBar.Parent        = this;
            hScrollBar.Dock          = DockStyle.Bottom;
            hScrollBar.SmallChange   = 100;
            hScrollBar.LargeChange   = 300;
            hScrollBar.ValueChanged += new EventHandler(HScrollBar_ValueChanged);

            // Vertical ScrollBar
            vScrollBar = new VScrollBar();
            vScrollBar.Parent        = this;
            vScrollBar.Dock          = DockStyle.Right;
            vScrollBar.TabStop       = true;
            vScrollBar.SmallChange   = 1;
            vScrollBar.LargeChange   = 4;
            vScrollBar.ValueChanged += new EventHandler(VScrollBar_ValueChanged);

            Graphics g = CreateGraphics();
            font = new Font(Font.FontFamily, 9);

            string longestDirection = "";
            foreach (PosDirection posDir in Enum.GetValues(typeof(PosDirection)))
                if (g.MeasureString(Language.T(posDir.ToString()), font).Width >
                    g.MeasureString(longestDirection, font).Width)
                    longestDirection = Language.T(posDir.ToString());

            string longestTransaction = "";
            foreach (Transaction transaction in Enum.GetValues(typeof(Transaction)))
                if (g.MeasureString(Language.T(transaction.ToString()), font).Width >
                    g.MeasureString(longestTransaction, font).Width)
                    longestTransaction = Language.T(transaction.ToString());

            string longestBacktestEval = "";
            foreach (BacktestEval eval in Enum.GetValues(typeof(BacktestEval)))
                if (g.MeasureString(Language.T(eval.ToString()), font).Width >
                    g.MeasureString(longestBacktestEval, font).Width)
                    longestBacktestEval = Language.T(eval.ToString());

            asTitlesPips = new string[19]
            {
               Language.T("Bar"),
               Language.T("Date"),
               Language.T("Hour"),
               Language.T("Open"),
               Language.T("High"),
               Language.T("Low"),
               Language.T("Close"),
               Language.T("Volume"),
               Language.T("Transaction"),
               Language.T("Direction"),
               Language.T("Lots"),
               Language.T("Price"),
               Language.T("Profit Loss"),
               Language.T("Floating P/L"),
               Language.T("Balance"),
               Language.T("Equity"),
               Language.T("Required"),
               Language.T("Free"),
               Language.T("Interpolation")
            };

            asTitlesMoney = new string[19]
            {
               Language.T("Bar"),
               Language.T("Date"),
               Language.T("Hour"),
               Language.T("Open"),
               Language.T("High"),
               Language.T("Low"),
               Language.T("Close"),
               Language.T("Volume"),
               Language.T("Transaction"),
               Language.T("Direction"),
               Language.T("Amount"),
               Language.T("Price"),
               Language.T("Profit Loss"),
               Language.T("Floating P/L"),
               Language.T("Balance"),
               Language.T("Equity"),
               Language.T("Required"),
               Language.T("Free"),
               Language.T("Interpolation")
            };

            string[] asColumContent = new string[19]
            {
                "99999",
                "99/99/99",
                "99:99",
                "99.99999",
                "99.99999",
                "99.99999",
                "99.99999",
                "999999",
                longestTransaction,
                longestDirection,
                "-9999999",
                "99.99999",
                "-9999999.99",
                "-9999999.99",
                "-9999999.99",
                "-9999999.99",
                "-9999999.99",
                "-9999999.99",
                longestBacktestEval
            };

            rowHeight = (int)Math.Max(font.Height, 18);
            Padding = new Padding(border, 2 * rowHeight, border, border);

            columns  = 19;
            aiColumnX = new int[20];
            aiX       = new int[20];

            aiColumnX[0] = border;
            aiColumnX[1] = aiColumnX[0] + (int)Math.Max(g.MeasureString(asColumContent[0], font).Width + 16, g.MeasureString(asTitlesMoney[0], font).Width) + 4;
            for (int i = 1; i < columns; i++)
                aiColumnX[i + 1] = aiColumnX[i] + (int)Math.Max(g.MeasureString(asColumContent[i], font).Width, g.MeasureString(asTitlesMoney[i], font).Width) + 4;
            g.Dispose();

            return;
        }

        /// <summary>
        /// Updates the journal data from the backtester
        /// </summary>
        void UpdateJournalData()
        {
            asJournalData = new string[shownBars, columns];
            aiPositionIcons = new Image[shownBars];

            for (int bar = firstBar; bar < firstBar + shownBars; bar++)
            {
                int row = bar - firstBar;

                asJournalData[row, 0] = (bar + 1).ToString();
                asJournalData[row, 1] = Data.Time[bar].ToString(Data.DF);
                asJournalData[row, 2] = Data.Time[bar].ToString("HH:mm");
                asJournalData[row, 3] = Data.Open[bar].ToString(Data.FF);
                asJournalData[row, 4] = Data.High[bar].ToString(Data.FF);
                asJournalData[row, 5] = Data.Low[bar].ToString(Data.FF);
                asJournalData[row, 6] = Data.Close[bar].ToString(Data.FF);
                asJournalData[row, 7] = Data.Volume[bar].ToString();
                if (Backtester.IsPos(bar))
                {
                    asJournalData[row, 8] = Language.T(Backtester.SummaryTrans(bar).ToString());
                    asJournalData[row, 9] = Language.T(Backtester.SummaryDir(bar).ToString());
                    if (Configs.AccountInMoney)
                    {
                        string sign = Backtester.SummaryDir(bar) == PosDirection.Short ? "-" : "";
                        asJournalData[row, 10] = sign + Backtester.SummaryAmount(bar).ToString();
                    }
                    else
                        asJournalData[row, 10] = Backtester.SummaryLots(bar).ToString();
                    asJournalData[row, 11] = Backtester.SummaryPrice(bar).ToString(Data.FF);
                    if (Configs.AccountInMoney)
                    {
                        // Profit Loss
                        if (Backtester.SummaryTrans(bar) == Transaction.Close ||
                            Backtester.SummaryTrans(bar) == Transaction.Reduce ||
                            Backtester.SummaryTrans(bar) == Transaction.Reverse)
                        {
                            asJournalData[row, 12] = Backtester.MoneyProfitLoss(bar).ToString("F2");
                        }
                        else
                        {
                            asJournalData[row, 12] = "-";
                        }

                        // Floating Profit Loss
                        if (Backtester.SummaryTrans(bar) != Transaction.Close)
                        {
                            asJournalData[row, 13] = Backtester.MoneyFloatingPL(bar).ToString("F2");
                        }
                        else
                        {
                            asJournalData[row, 13] = "-";
                        }
                    }
                    else
                    {
                        // Profit Loss
                        if (Backtester.SummaryTrans(bar) == Transaction.Close ||
                            Backtester.SummaryTrans(bar) == Transaction.Reduce ||
                            Backtester.SummaryTrans(bar) == Transaction.Reverse)
                        {
                            asJournalData[row, 12] = Backtester.ProfitLoss(bar).ToString();
                        }
                        else
                        {
                            asJournalData[row, 12] = "-";
                        }

                        // Floating Profit Loss
                        if (Backtester.SummaryTrans(bar) != Transaction.Close)
                        {
                            asJournalData[row, 13] = Backtester.FloatingPL(bar).ToString();
                        }
                        else
                        {
                            asJournalData[row, 13] = "-";
                        }
                    }

                    // Icons
                    aiPositionIcons[row] = Backtester.SummaryPositionIcon(bar);
                }
                else
                {
                    // Icons
                    aiPositionIcons[row] = Properties.Resources.pos_square;
                }


                if (Configs.AccountInMoney)
                {
                    asJournalData[row, 14] = Backtester.MoneyBalance(bar).ToString("F2");
                    asJournalData[row, 15] = Backtester.MoneyEquity(bar).ToString("F2");
                }
                else
                {
                    asJournalData[row, 14] = Backtester.Balance(bar).ToString();
                    asJournalData[row, 15] = Backtester.Equity(bar).ToString();
                }
                asJournalData[row, 16] = Backtester.SummaryRequiredMargin(bar).ToString("F2");
                asJournalData[row, 17] = Backtester.SummaryFreeMargin(bar).ToString("F2");
                asJournalData[row, 18] = Language.T(Backtester.BackTestEval(bar));
            }

            return;
        }

        /// <summary>
        /// Set params on resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            if (ClientSize.Height > 2 * rowHeight)
            {
                rows = (ClientSize.Height - 2 * rowHeight) / rowHeight;
            }
            else
            {
                rows = 0;
            }

            if (ClientSize.Width - vScrollBar.Width - 2 * border <= aiColumnX[columns])
                aiColumnX.CopyTo(aiX, 0);
            else
            {   // Scales the columns position
                float scale = (float)(ClientSize.Width - vScrollBar.Width - 2 * border) / aiColumnX[columns];
                for (int i = 0; i <= columns; i++)
                    aiX[i] = (int)(aiColumnX[i] * scale);
            }

            if (ClientSize.Width - vScrollBar.Width - 2 * border < aiColumnX[columns])
            {
                hScrollBar.Visible = true;
                int poinShort = aiColumnX[columns] - ClientSize.Width + vScrollBar.Width + 2 * border;
                if (hScrollBar.Value > poinShort)
                    hScrollBar.Value = poinShort;
                hScrollBar.Maximum = poinShort + hScrollBar.LargeChange - 2;
            }
            else
            {
                hScrollBar.Value = 0;
                hScrollBar.Visible = false;
            }

            SetUpJournal();
            Invalidate();

            return;
        }

        /// <summary>
        /// Paints the journal
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            int   iHScrll  = -hScrollBar.Value;
            bool  bWarning = false;
            Brush brush    = Brushes.Red;
            Size  size     = new Size(ClientSize.Width - vScrollBar.Width, rowHeight);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            // Caption background
            RectangleF rectfCaption = new RectangleF(0, 0, ClientSize.Width, 2 * rowHeight);
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            // Print the journal caption
            string unit = Configs.AccountInMoney ? " [" + Configs.AccountCurrency + "]" : " [" + Language.T("pips") + "]";
            string accUnit = " [" + Configs.AccountCurrency + "]";
            g.SetClip(new RectangleF(border, 0, ClientSize.Width - 2 * border, 2 * rowHeight));
            g.DrawString(Language.T("Market Data"), font, brushCaptionText, iHScrll + (aiX[8] + aiX[0]) / 2, 0, sf);
            g.DrawString(Language.T("Summary") + unit, font, brushCaptionText, iHScrll + (aiX[14] + aiX[8]) / 2, 0, sf);
            g.DrawString(Language.T("Account") + unit, font, brushCaptionText, iHScrll + (aiX[16] + aiX[14]) / 2, 0, sf);
            g.DrawString(Language.T("Margin")  + accUnit, font, brushCaptionText, iHScrll + (aiX[18] + aiX[16]) / 2, 0, sf);
            g.DrawString(Language.T("Backtest"), font, brushCaptionText, iHScrll + (aiX[19] + aiX[18]) / 2, 0, sf);
            if (Configs.AccountInMoney)
            {
                for (int i = 0; i < columns; i++)
                    g.DrawString(asTitlesMoney[i], font, brushCaptionText, iHScrll + (aiX[i] + aiX[i + 1]) / 2, rowHeight, sf);
            }
            else
            {
                for (int i = 0; i < columns; i++)
                    g.DrawString(asTitlesPips[i], font, brushCaptionText, iHScrll + (aiX[i] + aiX[i + 1]) / 2, rowHeight, sf);
            }
            g.ResetClip();

            RectangleF rectField = new RectangleF(border, 2 * rowHeight, ClientSize.Width - 2 * border, ClientSize.Height - 2 * rowHeight - border);
            g.FillRectangle(new SolidBrush(colorBack), rectField);

            size = new Size(ClientSize.Width - vScrollBar.Width - 2 * border, rowHeight);

            // Prints the journal data
            for (int bar = firstBar; bar < firstBar + shownBars; bar++)
            {
                int y = (bar - firstBar + 2) * rowHeight;
                Point point = new Point(border, y);

                // Even row
                if ((bar - firstBar) % 2f != 0)
                    g.FillRectangle(brushEvenRowBack, new Rectangle(point, size));

                // Warning row
                if (asJournalData[bar - firstBar, columns - 1] == Language.T("Ambiguous"))
                {
                    g.FillRectangle(brushWarningBack, new Rectangle(point, size));
                    bWarning = true;
                }
                else
                {
                    bWarning = false;
                }

                // Selected row
                if (bar - firstBar == selectedRow)
                {
                    g.FillRectangle(brushSelectedRowBack, new Rectangle(point, size));
                    brush = brushSelectedRowText;
                }
                else
                {
                    brush = bWarning ? brushWarningText : brushRowText;
                }

                // Draw the position icon
                int imgY = y + (int)Math.Floor((rowHeight - 16) / 2.0);
                g.DrawImage(aiPositionIcons[bar - firstBar], iHScrll + 2, imgY, 16, 16);

                // Prints the data
                g.DrawString(asJournalData[bar - firstBar, 0], font, brush, iHScrll + (16 + aiX[1]) / 2, (bar - firstBar + 2) * rowHeight, sf);
                for (int i = 1; i < columns; i++)
                    g.DrawString(asJournalData[bar - firstBar, i], font, brush, iHScrll + (aiX[i] + aiX[i + 1]) / 2, (bar - firstBar + 2) * rowHeight, sf);
            }

            // Horizontal line in the title
            //g.DrawLine(penLines, 0, iRowHeight, ClientSize.Width, iRowHeight);

            // Vertical lines
            for (int i = 1; i < columns; i++)
            {
                if (i == 8 || i == 14 || i == 16 || i == 18)
                {
                    RectangleF rectfSeparator = new RectangleF(aiX[i] + iHScrll, rowHeight / 2, 1, 3 * rowHeight / 2);
                    Data.GradientPaint(g, rectfSeparator, LayoutColors.ColorCaptionBack, -2 * LayoutColors.DepthCaption);
                    g.DrawLine(penLines, aiX[i] + iHScrll, 2 * rowHeight, aiX[i] + iHScrll, ClientSize.Height);
                }
                else
                    g.DrawLine(penLines, aiX[i] + iHScrll, 2 * rowHeight, aiX[i] + iHScrll, ClientSize.Height);
            }

            // Border
            g.DrawLine(penBorder, 1, 2 * rowHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - border + 1, 2 * rowHeight, ClientSize.Width - border + 1, ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - border + 1, ClientSize.Width, ClientSize.Height - border + 1);

            OnSelectedBarChange(new EventArgs());

            return;
        }

        /// <summary>
        /// Selects a row on Mous Up
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            selectedRow = e.Y / rowHeight - 2;

            if (selectedRow < 0)
                selectedRow = 0;
            else if(selectedRow > shownBars - 1)
                selectedRow = shownBars - 1;

            Rectangle rect = new Rectangle(0, 2 * rowHeight, ClientSize.Width, ClientSize.Height - 2 * rowHeight);
            Invalidate(rect);
            vScrollBar.Select();
        }

        /// <summary>
        /// Raises the event by invoking the delegates
        /// </summary>
        protected virtual void OnSelectedBarChange(EventArgs e)
        {
            // Invokes the delegate
            if (SelectedBarChange != null && selectedBarOld != SelectedBar)
            {
                SelectedBarChange(this, e);
                selectedBarOld = SelectedBar;
            }

            return;
        }

        /// <summary>
        /// Invalidates the sender after scrolling
        /// </summary>
        void HScrollBar_ValueChanged(object sender, EventArgs e)
        {
            int iScrallBarSize = hScrollBar.Visible ? hScrollBar.Height : 0;
            Rectangle rect = new Rectangle(border, 1, ClientSize.Width - 2 * border, ClientSize.Height - iScrallBarSize - border - 1);
            Invalidate(rect);

            return;
        }

        /// <summary>
        /// Invalidates the sender after scrolling
        /// </summary>
        void VScrollBar_ValueChanged(object sender, EventArgs e)
        {
            SetUpJournal();
            int iScrallBarSize = hScrollBar.Visible ? hScrollBar.Height : 0;
            Rectangle rect = new Rectangle(border, 2 * rowHeight, ClientSize.Width - 2 * border, ClientSize.Height - 2 * rowHeight - iScrallBarSize - border);
            Invalidate(rect);

            return;
        }
    }
}