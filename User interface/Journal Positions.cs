// Journal_Positions Class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    public class Journal_Positions : Panel
    {
        protected Button btnRemoveJournal;
        protected Button btnToggleJournal;
        VScrollBar vScrollBar;
        HScrollBar hScrollBar;
        ToolTip    toolTip;

        Image[]   aiPositionIcons; // Shows the position's type and transaction
        string[,] asJournalData;   // The text journal data
        string[]  asTitlesPips;    // Journal title
        string[]  asTitlesMoney;   // Journal title

        int[] aiColumnX;  // The horizontal position of the column
        int[] aiX;        // The scalled horizontal position of the column
        int   columns;    // The number of the columns
        int   rowHeight;  // The journal row height
        int   border = 2; // The width of outside border of the panel

        int rows;           // The nuber of rows can be shown (without the caption bar)
        int positions;      // The total number of the positions
        int firstPos;       // The number of the first shown position
        int lastPos;        // The number of the last shown position
        int shownPos;       // How many positions are shown
        int selectedRow;    // The nuber of the selected row
        int selectedBarOld; // The nuber of the old selected bar

        public event EventHandler SelectedBarChange;

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

        /// <summary>
        /// Gets the selected bar
        /// </summary>
        public int SelectedBar { get { return Backtester.PosCoordinates[firstPos + selectedRow].Bar; } }

        /// <summary>
        /// Gets the Button Remove Journal
        /// </summary>
        public Button BtnRemoveJournal { get { return btnRemoveJournal; } }

        /// <summary>
        /// Gets the Journal Toglle Button
        /// </summary>
        public Button BtnToggleJournal { get { return btnToggleJournal; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public Journal_Positions()
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
            positions = Backtester.PositionsTotal;

            if (positions == 0)
            {
                firstPos = 0;
                lastPos  = 0;
                shownPos = 0;

                vScrollBar.Enabled = false;
            }
            else if (positions < rows)
            {
                firstPos = 0;
                lastPos  = rows;
                shownPos = positions;

                vScrollBar.Enabled = false;
            }
            else
            {
                vScrollBar.Enabled = true;
                vScrollBar.Maximum = positions - 1;

                firstPos = vScrollBar.Value;
                if (firstPos + rows > positions)
                {
                    lastPos  = positions - 1;
                    shownPos = lastPos - firstPos + 1;
                }
                else
                {
                    shownPos = rows;
                    lastPos  = firstPos + shownPos - 1;
                }
            }

            selectedRow = Math.Min(selectedRow, shownPos - 1);
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
            penLines             = new Pen(LayoutColors.ColorJournalLines);
            penBorder            = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);

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

            // Button Remove Journal
            btnRemoveJournal = new Button();
            btnRemoveJournal.Parent                  = this;
            btnRemoveJournal.BackgroundImage         = Properties.Resources.close_blue;
            btnRemoveJournal.BackgroundImageLayout   = ImageLayout.Center;
            btnRemoveJournal.Cursor                  = Cursors.Arrow;
            btnRemoveJournal.UseVisualStyleBackColor = true;
            btnRemoveJournal.TabIndex                = 1;
            toolTip.SetToolTip(btnRemoveJournal, Language.T("Hide the journal tables."));

            // Button Toggle Journal
            btnToggleJournal = new Button();
            btnToggleJournal.Parent                  = this;
            btnToggleJournal.BackgroundImage         = Properties.Resources.toggle_journal;
            btnToggleJournal.BackgroundImageLayout   = ImageLayout.Center;
            btnToggleJournal.Cursor                  = Cursors.Arrow;
            btnToggleJournal.TabIndex                = 0;
            btnToggleJournal.UseVisualStyleBackColor = true;
            toolTip.SetToolTip(btnToggleJournal, Language.T("Toggle the journal type."));

            Graphics g = CreateGraphics();
            font = new Font(Font.FontFamily, 9);

            string longestDirection = "";
            foreach(PosDirection posDir in Enum.GetValues(typeof(PosDirection)))
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

            asTitlesPips = new string[18]
            {
                Language.T("Position"),
                Language.T("Bar"),
                Language.T("Bar Opening Time"),
                Language.T("Transaction"),
                Language.T("Direction"),
                Language.T("Lots"),
                Language.T("Ord Price"),
                Language.T("Avrg Price"),
                Language.T("Margin"),
                Language.T("Spread"),
                Language.T("Rollover"),
                Language.T("Commission"),
                Language.T("Slippage"),
                Language.T("Profit Loss"),
                Language.T("Floating P/L"),
                Language.T("Balance"),
                Language.T("Equity"),
                Language.T("Backtest")
            };

            asTitlesMoney = new string[18]
            {
                Language.T("Position"),
                Language.T("Bar"),
                Language.T("Bar Opening Time"),
                Language.T("Transaction"),
                Language.T("Direction"),
                Language.T("Amount"),
                Language.T("Ord Price"),
                Language.T("Avrg Price"),
                Language.T("Margin"),
                Language.T("Spread"),
                Language.T("Rollover"),
                Language.T("Commission"),
                Language.T("Slippage"),
                Language.T("Profit Loss"),
                Language.T("Floating P/L"),
                Language.T("Balance"),
                Language.T("Equity"),
                Language.T("Backtest")
            };

            string[] asColumContent = new string[18]
            {
                "99999",
                "99999",
                "12/03/08 00:00",
                longestTransaction,
                longestDirection,
                "-99999999",
                "99.99999",
                "99.99999",
                "999999.99",
                "-999.99",
                "-999.99",
                "-999.99",
                "-999.99",
                "-999999.99",
                "-999999.99",
                "-99999999.99",
                "-99999999.99",
                longestBacktestEval
            };

            rowHeight = (int)Math.Max(font.Height, 18);
            Padding = new Padding(border, 2 * rowHeight, border, border);

            columns  = 18;
            aiColumnX = new int[19];
            aiX       = new int[19];

            aiColumnX[0] = border;
            aiColumnX[1] = aiColumnX[0] + (int)Math.Max(g.MeasureString(asColumContent[0], font).Width + 16, g.MeasureString(asTitlesMoney[0], font).Width) + 4;
            for(int i = 1; i < columns; i++)
                aiColumnX[i + 1] = aiColumnX[i] + (int)Math.Max(g.MeasureString(asColumContent[i], font).Width,g.MeasureString(asTitlesMoney[i], font).Width) + 4;
            g.Dispose();

            btnRemoveJournal.Size = new Size(rowHeight - 2, rowHeight - 2);
            btnToggleJournal.Size = new Size(20, rowHeight - 2);

            return;
        }

        /// <summary>
        /// Updates the journal data from the backtester
        /// </summary>
        void UpdateJournalData()
        {
            asJournalData   = new string[shownPos, columns];
            aiPositionIcons = new Image[shownPos];

            for (int pos = firstPos; pos < firstPos + shownPos; pos++)
            {
                int row = pos - firstPos;
                int bar = Backtester.PosCoordinates[pos].Bar;
                Position position = Backtester.PosFromNumb(pos);

                string posAmount;
                if (Configs.AccountInMoney)
                    posAmount = (position.PosDir == PosDirection.Short ? "-" : "") +
                                 (position.PosLots * Data.InstrProperties.LotSize).ToString();
                else
                    posAmount = position.PosLots.ToString();

                string profitLoss;
                if (Configs.AccountInMoney)
                    profitLoss = position.MoneyProfitLoss.ToString("F2");
                else
                    profitLoss = position.ProfitLoss.ToString("F2");

                string floatingPL;
                if (Configs.AccountInMoney)
                    floatingPL = position.MoneyFloatingPL.ToString("F2");
                else
                    floatingPL = position.FloatingPL.ToString("F2");

                int p = 0;
                asJournalData[row, p++] = (pos + 1).ToString();
                asJournalData[row, p++] = (bar + 1).ToString();
                asJournalData[row, p++] = Data.Time[bar].ToString(Data.DF) + Data.Time[bar].ToString(" HH:mm");
                asJournalData[row, p++] = Language.T(position.Transaction.ToString());
                asJournalData[row, p++] = Language.T(position.PosDir.ToString());
                asJournalData[row, p++] = posAmount;
                asJournalData[row, p++] = position.FormOrdPrice.ToString(Data.FF);
                asJournalData[row, p++] = position.PosPrice.ToString(Data.FF);
                asJournalData[row, p++] = position.RequiredMargin.ToString("F2");

                // Charges
                if (Configs.AccountInMoney)
                {   // in currency
                    if (position.Transaction == Transaction.Open ||
                        position.Transaction == Transaction.Add  ||
                        position.Transaction == Transaction.Reverse)
                        asJournalData[row, p++] = position.MoneySpread.ToString("F2");
                    else
                        asJournalData[row, p++] = "-";

                    if (position.Transaction == Transaction.Transfer)
                        asJournalData[row, p++] = position.MoneyRollover.ToString("F2");
                    else
                        asJournalData[row, p++] = "-";

                    if (position.Transaction == Transaction.Open   ||
                        position.Transaction == Transaction.Close  ||
                        position.Transaction == Transaction.Add    ||
                        position.Transaction == Transaction.Reduce ||
                        position.Transaction == Transaction.Reverse)
                    {
                        asJournalData[row, p++] = position.MoneyCommission.ToString("F2");
                        asJournalData[row, p++] = position.MoneySlippage.ToString("F2");
                    }
                    else
                    {
                        asJournalData[row, p++] = "-";
                        asJournalData[row, p++] = "-";
                    }
                }
                else
                {   // In pips
                    if (position.Transaction == Transaction.Open ||
                        position.Transaction == Transaction.Add  ||
                        position.Transaction == Transaction.Reverse)
                        asJournalData[row, p++] = position.Spread.ToString();
                    else
                        asJournalData[row, p++] = "-";

                    if (position.Transaction == Transaction.Transfer)
                        asJournalData[row, p++] = position.Rollover.ToString("F2");
                    else
                        asJournalData[row, p++] = "-";

                    if (position.Transaction == Transaction.Open   ||
                        position.Transaction == Transaction.Close  ||
                        position.Transaction == Transaction.Add    ||
                        position.Transaction == Transaction.Reduce ||
                        position.Transaction == Transaction.Reverse)
                    {
                        asJournalData[row, p++] = position.Commission.ToString("F2");
                        asJournalData[row, p++] = position.Slippage.ToString();
                    }
                    else
                    {
                        asJournalData[row, p++] = "-";
                        asJournalData[row, p++] = "-";
                    }
                }

                // Profit Loss
                if (position.Transaction == Transaction.Close  ||
                    position.Transaction == Transaction.Reduce ||
                    position.Transaction == Transaction.Reverse)
                    asJournalData[row, p++] = profitLoss;
                else
                    asJournalData[row, p++] = "-";

                // Floating Profit Loss
                if (position.PosNumb == Backtester.SummaryPosNumb(bar) &&
                    position.Transaction != Transaction.Close)
                    asJournalData[row, p++] = floatingPL;  //Last position of the bar only
                else
                    asJournalData[row, p++] = "-";

                // Blance / Equity
                if (Configs.AccountInMoney)
                {
                    asJournalData[row, p++] = position.MoneyBalance.ToString("F2");
                    asJournalData[row, p++] = position.MoneyEquity.ToString("F2");
                }
                else
                {
                    asJournalData[row, p++] = position.Balance.ToString("F2");
                    asJournalData[row, p++] = position.Equity.ToString("F2");
                }

                asJournalData[row, p++] = Language.T(Backtester.BackTestEval(bar));

                // Icons
                aiPositionIcons[row] = position.PositionIcon;
            }

            return;
        }

        /// <summary>
        /// Set params on resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            btnRemoveJournal.Location = new Point(ClientSize.Width - btnRemoveJournal.Width - 1, 1);
            btnToggleJournal.Location = new Point(btnRemoveJournal.Left - btnToggleJournal.Width - 1, 1);

            if (ClientSize.Height > 2 * rowHeight + border)
            {
                rows = (ClientSize.Height - 2 * rowHeight - border) / rowHeight;
            }
            else
            {
                rows = 0;
            }

            if (ClientSize.Width - vScrollBar.Width - 2 * border <= aiColumnX[columns])
                aiColumnX.CopyTo(aiX, 0);
            else
            {   // Scales the columns position
                float fScale = (float)(ClientSize.Width - vScrollBar.Width - 2 * border) / aiColumnX[columns];
                for (int i = 0; i <= columns; i++)
                    aiX[i] = (int)(aiColumnX[i] * fScale);
            }

            if (ClientSize.Width - vScrollBar.Width - 2 * border < aiColumnX[columns])
            {
                hScrollBar.Visible = true;
                int iPoinShort = aiColumnX[columns] - ClientSize.Width + vScrollBar.Width + 2 * border;
                if (hScrollBar.Value > iPoinShort)
                    hScrollBar.Value = iPoinShort;
                hScrollBar.Maximum = iPoinShort + hScrollBar.LargeChange - 2;
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

            int   scrll     = -hScrollBar.Value;
            bool  isWarning = false;
            Brush brush     = Brushes.Red;
            Size  size      = new Size(ClientSize.Width, rowHeight);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            // Caption background
            RectangleF rectfCaption = new RectangleF(0, 0, ClientSize.Width, 2 * rowHeight);
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            // Print the journal caption
            string stringCaptionText = Language.T("Journal by Positions") + (Configs.AccountInMoney ? " [" + Configs.AccountCurrency + "]" : " [" + Language.T("pips") + "]");
            g.DrawString(stringCaptionText, font, brushCaptionText, new RectangleF(Point.Empty, size), sf);
            g.SetClip(new RectangleF(border, rowHeight, ClientSize.Width - 2 * border, rowHeight));
            if (Configs.AccountInMoney)
            {
                g.DrawString(asTitlesMoney[0], font, brushCaptionText, scrll + (aiX[0] + aiX[1]) / 2, rowHeight, sf);
                for (int i = 1; i < columns; i++)
                    g.DrawString(asTitlesMoney[i], font, brushCaptionText, scrll + (aiX[i] + aiX[i + 1]) / 2, rowHeight, sf);
            }
            else
            {
                g.DrawString(asTitlesPips[0], font, brushCaptionText, scrll + (aiX[0] + aiX[1]) / 2, rowHeight, sf);
                for (int i = 1; i < columns; i++)
                    g.DrawString(asTitlesPips[i], font, brushCaptionText, scrll + (aiX[i] + aiX[i + 1]) / 2, rowHeight, sf);
            }
            g.ResetClip();

            // Paints the journal's data field
            RectangleF rectField = new RectangleF(border, 2 * rowHeight, ClientSize.Width - 2 * border, ClientSize.Height - 2 * rowHeight - border);
            g.FillRectangle(new SolidBrush(colorBack), rectField);

            size = new Size(ClientSize.Width - vScrollBar.Width - 2 * border, rowHeight);

            // Prints the journal data
            for (int pos = firstPos; pos < firstPos + shownPos; pos++)
            {
                int y = (pos - firstPos + 2) * rowHeight;
                Point point = new Point(border, y);

                // Even row
                if ((pos - firstPos) % 2f != 0)
                    g.FillRectangle(brushEvenRowBack, new Rectangle(point, size));

                // Warning row
                if (asJournalData[pos - firstPos, columns - 1] == Language.T("Ambiguous"))
                {
                    g.FillRectangle(brushWarningBack, new Rectangle(point, size));
                    isWarning = true;
                }
                else
                {
                    isWarning = false;
                }

                // Selected row
                if (pos - firstPos == selectedRow)
                {
                    g.FillRectangle(brushSelectedRowBack, new Rectangle(point, size));
                    brush = brushSelectedRowText;
                }
                else
                {
                    brush = isWarning ? brushWarningText : brushRowText;
                }

                // Draw the position icon
                int iImgY = y + (int)Math.Floor((rowHeight - 16) / 2.0);
                g.DrawImage(aiPositionIcons[pos - firstPos], scrll + 2, iImgY, 16, 16);

                // Prints the data
                g.DrawString(asJournalData[pos - firstPos, 0], font, brush, scrll + (16 + aiX[1]) / 2, (pos - firstPos + 2) * rowHeight, sf);
                for (int i = 1; i < columns; i++)
                    g.DrawString(asJournalData[pos - firstPos, i], font, brush, scrll + (aiX[i] + aiX[i + 1]) / 2, (pos - firstPos + 2) * rowHeight, sf);
            }

            //g.DrawLine(penLines, 0, iRowHeight, ClientSize.Width, iRowHeight);
            for (int i = 1; i < columns; i++)
                g.DrawLine(penLines, aiX[i] + scrll, 2 * rowHeight, aiX[i] + scrll, ClientSize.Height);

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
            else if(selectedRow > shownPos - 1)
                selectedRow = shownPos - 1;

            Rectangle rect = new Rectangle(0, 2 * rowHeight, ClientSize.Width, ClientSize.Height - 2 * rowHeight);
            Invalidate(rect);
            vScrollBar.Select();

            return;
        }

        /// <summary>
        /// Raises the event by invoking the delegates
        /// </summary>
        protected virtual void OnSelectedBarChange(EventArgs e)
        {
            // Invokes the delegate
            if (firstPos + selectedRow < 0)
                return;

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
            int scrallBarWidth = hScrollBar.Visible ? hScrollBar.Height : 0;
            Rectangle rect = new Rectangle(border, rowHeight + 1, ClientSize.Width - 2 * border, ClientSize.Height - rowHeight - scrallBarWidth - border - 1);
            Invalidate(rect);

            return;
        }

        /// <summary>
        /// Invalidates the sender after scrolling
        /// </summary>
        void VScrollBar_ValueChanged(object sender, EventArgs e)
        {
            SetUpJournal();
            int scrallBarWidth = hScrollBar.Visible ? hScrollBar.Height : 0;
            Rectangle rect = new Rectangle(border, 2 * rowHeight, ClientSize.Width - 2 * border, ClientSize.Height - 2 * rowHeight - scrallBarWidth - border);
            Invalidate(rect);

            return;
        }
    }
}