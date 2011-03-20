// Journal_Pos Class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    public class Journal_Pos : Panel
    {
        VScrollBar vScrollBar;
        HScrollBar hScrollBar;
        ToolTip    toolTip;

        Image[] aiPositionIcons; // Shows the position's type and transaction
        string[,] asJournalData; // The text journal data
        string[]  asTitlesPips;  // Journal title
        string[]  asTitlesMoney; // Journal title

        int[] aiColumnX;    // The horizontal position of the column
        int[] aiX;          // The scaled horizontal position of the column
        int   columns;      // The number of the columns
        int   rowHeight;    // The journal row height
        int   visibalWidth; // The width of the panel visible part

        int rows;           // The number of rows can be shown (without the caption bar)
        int positions;      // The total number of the positions during the bar
        int firstPos;       // The number of the first shown positions
        int lastPos;        // The number of the last shown positions
        int shownPos;       // How many positions are shown
        int selectedBar;    // The selected bar
        int selectedBarOld; // The old selected bar
        int border = 2;     // The width of outside border of the panel

        Font  font;
        Color colorBack;
        Color colorCaptionBack;
        Brush brushCaptionText;
        Brush brushEvenRowBack;
        Brush brushRowText;
        Pen   penLines;
        Pen   penBorder;

        /// <summary>
        /// Sets the selected bar
        /// </summary>
        public int SelectedBar { set { selectedBar = value; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public Journal_Pos()
        {
            InitializeJournal();
            SetUpJournal();

            return;
        }

        /// <summary>
        /// Sets the journal's current data
        /// </summary>
        public void SetUpJournal()
        {
            if (Data.IsResult)
                positions = Backtester.Positions(selectedBar);

            SetSizes();
            SetJournalColors();
            UpdateJournalData();

            return;
        }

        /// <summary>
        /// Sets the journal colors
        /// </summary>
        void SetJournalColors()
        {
            colorBack        = LayoutColors.ColorControlBack;
            colorCaptionBack = LayoutColors.ColorCaptionBack;
            brushCaptionText = new SolidBrush(LayoutColors.ColorCaptionText);
            brushEvenRowBack = new SolidBrush(LayoutColors.ColorEvenRowBack);
            brushRowText     = new SolidBrush(LayoutColors.ColorControlText);
            penLines         = new Pen(LayoutColors.ColorJournalLines);
            penBorder        = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);

            return;
        }

        /// <summary>
        /// Inits the Journal
        /// </summary>
        void InitializeJournal()
        {
            font      = new Font(Font.FontFamily, 9);
            rowHeight = (int)Math.Max(font.Height, 18);
            columns   = 9;
            aiColumnX = new int[10];
            aiX       = new int[10];
            Padding   = new Padding(border, 2 * rowHeight, border, border);

            // Tool Tips
            toolTip = new ToolTip();

            // Horizontal ScrollBar
            hScrollBar = new HScrollBar();
            hScrollBar.Parent        = this;
            hScrollBar.Dock          = DockStyle.Bottom;
            hScrollBar.SmallChange   = 50;
            hScrollBar.LargeChange   = 200;
            hScrollBar.ValueChanged += new EventHandler(HScrollBar_ValueChanged);

            // Vertical ScrollBar
            vScrollBar = new VScrollBar();
            vScrollBar.Parent        = this;
            vScrollBar.Dock          = DockStyle.Right;
            vScrollBar.TabStop       = true;
            vScrollBar.SmallChange   = 1;
            vScrollBar.LargeChange   = 2;
            vScrollBar.ValueChanged += new EventHandler(VScrollBar_ValueChanged);

            Graphics g = CreateGraphics();

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

            asTitlesPips = new string[9]
            {
               Language.T("Position"),
               Language.T("Transaction"),
               Language.T("Direction"),
               Language.T("Lots"),
               Language.T("Order"),
               Language.T("Ord Price"),
               Language.T("Price"),
               Language.T("Profit Loss"),
               Language.T("Floating P/L")
            };

            asTitlesMoney = new string[9]
            {
               Language.T("Position"),
               Language.T("Transaction"),
               Language.T("Direction"),
               Language.T("Amount"),
               Language.T("Order"),
               Language.T("Ord Price"),
               Language.T("Price"),
               Language.T("Profit Loss"),
               Language.T("Floating P/L")
            };

            string[] asColumContent = new string[9]
            {
                "99999",
                longestTransaction,
                longestDirection,
                "-9999999",
                "99999",
                "99.99999",
                "99.99999",
                "-99999.99",
                "-99999.99"
            };

            aiColumnX[0] = border;
            aiColumnX[1] = aiColumnX[0] + (int)Math.Max(g.MeasureString(asColumContent[0], font).Width + 16, g.MeasureString(asTitlesMoney[0], font).Width) + 4;
            for (int i = 1; i < columns; i++)
                aiColumnX[i + 1] = aiColumnX[i] + (int)Math.Max(g.MeasureString(asColumContent[i], font).Width, g.MeasureString(asTitlesMoney[i],  font).Width) + 4;
            g.Dispose();

            return;
        }

        /// <summary>
        /// Updates the journal data from the backtester
        /// </summary>
        void UpdateJournalData()
        {
            if (!Data.IsResult)
                return;

            asJournalData = new string[positions, columns];
            aiPositionIcons = new Image[shownPos];

            for (int pos = firstPos; pos < firstPos + shownPos; pos++)
            {
                int row = pos - firstPos;

                asJournalData[row, 0] = (Backtester.PosNumb(selectedBar, pos) + 1).ToString();
                asJournalData[row, 1] = Language.T(Backtester.PosTransaction(selectedBar, pos).ToString());
                asJournalData[row, 2] = Language.T(Backtester.PosDir(selectedBar, pos).ToString());
                string sOrdAmount;
                if (Configs.AccountInMoney)
                    sOrdAmount = (Backtester.PosDir(selectedBar, pos) == PosDirection.Short ? "-" : "") +
                                 (Backtester.PosLots(selectedBar, pos) * Data.InstrProperties.LotSize).ToString();
                else
                    sOrdAmount = Backtester.PosLots(selectedBar, pos).ToString();
                asJournalData[row, 3] = sOrdAmount;
                asJournalData[row, 4] = (Backtester.PosOrdNumb(selectedBar, pos) + 1).ToString();
                asJournalData[row, 5] = Backtester.PosOrdPrice(selectedBar, pos).ToString(Data.FF);
                asJournalData[row, 6] = Backtester.PosPrice(selectedBar, pos).ToString(Data.FF);

                // Profit Loss
                if (Backtester.PosTransaction(selectedBar, pos) == Transaction.Close  ||
                    Backtester.PosTransaction(selectedBar, pos) == Transaction.Reduce ||
                    Backtester.PosTransaction(selectedBar, pos) == Transaction.Reverse)
                {
                    string sProfitLoss;
                    if (Configs.AccountInMoney)
                        sProfitLoss = Backtester.PosMoneyProfitLoss(selectedBar, pos).ToString("F2");
                    else
                        sProfitLoss = Math.Round(Backtester.PosProfitLoss(selectedBar, pos)).ToString();
                    asJournalData[row, 7] = sProfitLoss;
                }
                else
                {
                    asJournalData[row, 7] = "-";
                }

                // Floating Profit Loss
                if (pos == positions - 1 && Backtester.PosTransaction(selectedBar, pos) != Transaction.Close)
                { // Last bar position only
                    string sFloatingPL;
                    if (Configs.AccountInMoney)
                        sFloatingPL = Backtester.PosMoneyFloatingPL(selectedBar, pos).ToString("F2");
                    else
                        sFloatingPL = Math.Round(Backtester.PosFloatingPL(selectedBar, pos)).ToString();
                    asJournalData[row, 8] = sFloatingPL;
                }
                else
                {
                    asJournalData[row, 8] = "-";
                }

                // Icons
                aiPositionIcons[row] = Backtester.PosIcon(selectedBar, pos);
            }

            return;
        }

        /// <summary>
        /// Sets the size and position of the controls
        /// </summary>
        void SetSizes()
        {
            if (positions == 0)
            {
                firstPos = 0;
                lastPos  = 0;
                shownPos = 0;

                vScrollBar.Visible = false;
                visibalWidth = ClientSize.Width;
            }
            else if (positions < rows)
            {
                firstPos = 0;
                lastPos  = rows;
                shownPos = positions;

                vScrollBar.Visible = false;
                visibalWidth = ClientSize.Width;
            }
            else
            {
                vScrollBar.Visible = true;
                if (selectedBar != selectedBarOld)
                    vScrollBar.Value = 0;
                vScrollBar.Maximum = positions - 1;
                visibalWidth = vScrollBar.Left;

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

            if (visibalWidth <= aiColumnX[columns])
                aiColumnX.CopyTo(aiX, 0);
            else
            {   // Scales the columns position
                float fScale = (float)visibalWidth / aiColumnX[columns];
                for (int i = 0; i <= columns; i++)
                    aiX[i] = (int)(aiColumnX[i] * fScale);
            }

            if (visibalWidth < aiColumnX[columns])
            {
                hScrollBar.Visible = true;
                int iPoinShort = aiColumnX[columns] - visibalWidth;
                if (hScrollBar.Value > iPoinShort)
                    hScrollBar.Value = iPoinShort;
                hScrollBar.Maximum = iPoinShort + hScrollBar.LargeChange - 2;
            }
            else
            {
                hScrollBar.Value = 0;
                hScrollBar.Visible = false;
            }

            selectedBarOld = selectedBar;

            return;
        }

        /// <summary>
        /// Set params on resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            if (ClientSize.Height > 2 * rowHeight + border)
                rows = (ClientSize.Height - 2 * rowHeight - border) / rowHeight;
            else
                rows = 0;

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

            int  hScrll = -hScrollBar.Value;
            Size size   = new Size(visibalWidth, rowHeight);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            // Caption background
            RectangleF rectfCaption = new RectangleF(0, 0, ClientSize.Width, 2 * rowHeight);
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            // Print the journal caption
            string stringCaptionText = Language.T("Positions During the Bar") + (Configs.AccountInMoney ? " [" + Configs.AccountCurrency + "]" : " [" + Language.T("pips") + "]");
            g.DrawString(stringCaptionText, font, brushCaptionText, new RectangleF(Point.Empty, size), sf);
            g.SetClip(new RectangleF(border, rowHeight, ClientSize.Width - 2 * border, rowHeight));
            if (Configs.AccountInMoney)
                for (int i = 0; i < columns; i++)
                    g.DrawString(asTitlesMoney[i], font, brushCaptionText, hScrll + (aiX[i] + aiX[i + 1]) / 2, rowHeight, sf);
            else
                for (int i = 0; i < columns; i++)
                    g.DrawString(asTitlesPips[i], font, brushCaptionText, hScrll + (aiX[i] + aiX[i + 1]) / 2, rowHeight, sf);
            g.ResetClip();

            // Paints the journal's data field
            RectangleF rectField = new RectangleF(border, 2 * rowHeight, ClientSize.Width - 2 * border, ClientSize.Height - 2 * rowHeight - border);
            g.FillRectangle(new SolidBrush(colorBack), rectField);

            size = new Size(ClientSize.Width - vScrollBar.Width - 2 * border, rowHeight);

            // Prints the journal data
            for (int pos = firstPos; pos < firstPos + shownPos; pos++)
            {
                int row = pos - firstPos;
                int y = (row + 2) * rowHeight;
                Point point = new Point(border, y);

                // Even row
                if (row % 2f != 0)
                    g.FillRectangle(brushEvenRowBack, new Rectangle(point, size));

                // Draw the position icon
                int iImgY = y + (int)Math.Floor((rowHeight - 16) / 2.0);
                g.DrawImage(aiPositionIcons[pos - firstPos], hScrll + 2, iImgY, 16, 16);

                // Prints the data
                g.DrawString(asJournalData[row, 0], font, brushRowText, hScrll + (16 + aiX[1]) / 2, (row + 2) * rowHeight, sf);
                for (int i = 1; i < columns; i++)
                    g.DrawString(asJournalData[row, i], font, brushRowText, hScrll + (aiX[i] + aiX[i + 1]) / 2, (row + 2) * rowHeight, sf);
            }

            //g.DrawLine(penLines, 0, iRowHeight, ClientSize.Width, iRowHeight);
            for (int i = 1; i < columns; i++)
                g.DrawLine(penLines, aiX[i] + hScrll, 2 * rowHeight, aiX[i] + hScrll, ClientSize.Height);

            // Border
            g.DrawLine(penBorder, 1, 2 * rowHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - border + 1, 2 * rowHeight, ClientSize.Width - border + 1, ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - border + 1, ClientSize.Width, ClientSize.Height - border + 1);

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
