// Journal_Ord Class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    public class Journal_Ord : Panel
    {
        protected Button btnRemoveJournal;
        protected Button btnToggleJournal;
        VScrollBar vScrollBar;
        HScrollBar hScrollBar;
        ToolTip    toolTip;

        Image[]   aiOrderIcons;  // Shows the position's type and transaction
        string[,] asJournalData; // The text journal data
        string[]  asTitlesPips;  // Journal title
        string[]  asTitlesMoney; // Journal title

        int[] aiColumnX;    // The horizontal position of the column
        int[] aiX;          // The scaled horizontal position of the column
        int   columns;      // The number of the columns
        int   rowHeight;    // The journal row height
        int   visibalWidth; // The width of the panel visible part
        int   border = 2;   // The width of outside border of the panel

        int rows;           // The number of rows can be shown (without the caption bar)
        int orders;         // The total number of the orders during the bar
        int firstOrd;       // The number of the first shown orders
        int lastOrd;        // The number of the last shown orders
        int shownOrd;       // How many orders are shown
        int selectedBar;    // The selected bar
        int selectedBarOld; // The old selected bar

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
        /// Gets the Button Remove Journal
        /// </summary>
        public Button BtnRemoveJournal { get { return btnRemoveJournal; } }

        /// <summary>
        /// Gets the Button Toggle Journal
        /// </summary>
        public Button BtnToggleJournal { get { return btnToggleJournal; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public Journal_Ord()
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
            if(Data.IsResult)
                orders = Backtester.Orders(selectedBar);

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
            columns    = 8;
            aiColumnX  = new int[9];
            aiX        = new int[9];
            font       = new Font(Font.FontFamily, 9);
            rowHeight  = (int)Math.Max(font.Height, 18);
            Padding    = new Padding(border, 2 * rowHeight, border, border);

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

            // Button Remove Journal
            btnRemoveJournal = new Button();
            btnRemoveJournal.Parent                  = this;
            btnRemoveJournal.BackgroundImage         = Properties.Resources.close_blue;
            btnRemoveJournal.BackgroundImageLayout   = ImageLayout.Center;
            btnRemoveJournal.Cursor                  = Cursors.Arrow;
            btnRemoveJournal.Size                    = new Size(rowHeight - 2, rowHeight - 2);
            btnRemoveJournal.UseVisualStyleBackColor = true;
            toolTip.SetToolTip(btnRemoveJournal, Language.T("Hide the journal tables."));

            // Button Toggle Journal
            btnToggleJournal = new Button();
            btnToggleJournal.Parent                  = this;
            btnToggleJournal.BackgroundImage         = Properties.Resources.toggle_journal;
            btnToggleJournal.BackgroundImageLayout   = ImageLayout.Center;
            btnToggleJournal.Cursor                  = Cursors.Arrow;
            btnToggleJournal.Size                    = new Size(20, rowHeight - 2);
            btnToggleJournal.UseVisualStyleBackColor = true;
            toolTip.SetToolTip(btnToggleJournal, Language.T("Toggle the journal type."));

            string[] asComments = new string[] {
                "Exit Order to position",
                "Exit Order to order",
                "Take Profit to position",
                "Take Profit to order",
                "Trailing Stop to position",
                "Trailing Stop to order",
                "Permanent S/L to position",
                "Permanent S/L to order",
                "Permanent T/P to position",
                "Permanent T/P to order"
            };

            Graphics g = CreateGraphics();

            string longestDirection = "";
            foreach (OrderDirection ordDir in Enum.GetValues(typeof(OrderDirection)))
                if (g.MeasureString(Language.T(ordDir.ToString()), font).Width > g.MeasureString(longestDirection, font).Width)
                    longestDirection = Language.T(ordDir.ToString());

            string longestType = "";
            foreach (OrderType ordType in Enum.GetValues(typeof(OrderType)))
                if (g.MeasureString(Language.T(ordType.ToString()), font).Width > g.MeasureString(longestType, font).Width)
                    longestType = Language.T(ordType.ToString());

            string longestStatus = "";
            foreach (OrderStatus ordStatus in Enum.GetValues(typeof(OrderStatus)))
                if (g.MeasureString(Language.T(ordStatus.ToString()), font).Width > g.MeasureString(longestStatus, font).Width)
                    longestStatus = Language.T(ordStatus.ToString());

            string longestComment = "";
            foreach (string ordComment in asComments)
                if (g.MeasureString(Language.T(ordComment) + " 99999", font).Width > g.MeasureString(longestComment, font).Width)
                    longestComment = Language.T(ordComment) + " 99999";

            asTitlesPips = new string[8]
            {
                Language.T("Order"),
                Language.T("Direction"),
                Language.T("Type"),
                Language.T("Lots"),
                Language.T("Price 1"),
                Language.T("Price 2"),
                Language.T("Status"),
                Language.T("Comment")
            };

            asTitlesMoney = new string[8]
            {
                Language.T("Order"),
                Language.T("Direction"),
                Language.T("Type"),
                Language.T("Amount"),
                Language.T("Price 1"),
                Language.T("Price 2"),
                Language.T("Status"),
                Language.T("Comment")
            };

            string[] asColumContent = new string[8]
            {
                "99999",
                longestDirection,
                longestType,
                "-9999999",
                "99.99999",
                "99.99999",
                longestStatus,
                longestComment
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

            Order[] aOrders = new Order[orders];
            aiOrderIcons = new Image[orders];

            int ordIndex = 0;
            for (int point = 0; point < Backtester.WayPoints(selectedBar); point++)
            {
                int iOrdNumber = Backtester.WayPoint(selectedBar, point).OrdNumb;
                WayPointType wpType = Backtester.WayPoint(selectedBar, point).WPType;

                if (iOrdNumber == -1) continue; // There is no order
                if (iOrdNumber < Backtester.OrdNumb(selectedBar, 0)) continue; // For a transferred position

                if (wpType == WayPointType.Add    || wpType == WayPointType.Cancel ||
                    wpType == WayPointType.Entry  || wpType == WayPointType.Exit   ||
                    wpType == WayPointType.Reduce || wpType == WayPointType.Reverse)
                {
                    aOrders[ordIndex] = Backtester.OrdFromNumb(iOrdNumber);
                    ordIndex++;
                }
            }

            for (int ord = 0; ord < orders; ord++)
            {
                int  ordNumber  = Backtester.OrdNumb(selectedBar, ord);
                bool toIncluded = true;
                for (int i = 0; i < ordIndex; i++)
                {
                    if (ordNumber == aOrders[i].OrdNumb)
                    {
                        toIncluded = false;
                        break;
                    }
                }
                if (toIncluded)
                {
                    aOrders[ordIndex] = Backtester.OrdFromNumb(ordNumber);
                    ordIndex++;
                }
            }

            asJournalData = new string[orders, columns];

            for (int ord = firstOrd; ord < firstOrd + shownOrd; ord++)
            {
                int row = ord - firstOrd;

                string ordIF     = (aOrders[ord].OrdIF     > 0 ? (aOrders[ord].OrdIF + 1).ToString() : "-");
                string ordPrice2 = (aOrders[ord].OrdPrice2 > 0 ? aOrders[ord].OrdPrice2.ToString(Data.FF) : "-");

                asJournalData[row, 0] = (aOrders[ord].OrdNumb + 1).ToString();
                asJournalData[row, 1] = Language.T(aOrders[ord].OrdDir.ToString());
                asJournalData[row, 2] = Language.T(aOrders[ord].OrdType.ToString());
                if (Configs.AccountInMoney)
                {
                    string sOrdAmount = (aOrders[ord].OrdDir == OrderDirection.Sell ? "-" : "") +
                                        (aOrders[ord].OrdLots * Data.InstrProperties.LotSize).ToString();
                    asJournalData[row, 3] = sOrdAmount;
                }
                else
                    asJournalData[row, 3] = aOrders[ord].OrdLots.ToString();
                asJournalData[row, 4] = aOrders[ord].OrdPrice.ToString(Data.FF);
                asJournalData[row, 5] = ordPrice2;
                asJournalData[row, 6] = Language.T(aOrders[ord].OrdStatus.ToString());
                asJournalData[row, 7] = aOrders[ord].OrdNote;

                // Icons
                aiOrderIcons[row] = aOrders[ord].OrderIcon;
            }

            return;
        }

        /// <summary>
        /// Sets the size and position of the controls
        /// </summary>
        void SetSizes()
        {
            if (orders == 0)
            {
                firstOrd = 0;
                lastOrd  = 0;
                shownOrd = 0;

                vScrollBar.Visible = false;
                visibalWidth = ClientSize.Width;
            }
            else if (orders < rows)
            {
                firstOrd = 0;
                lastOrd  = rows;
                shownOrd = orders;

                vScrollBar.Visible = false;
                visibalWidth = ClientSize.Width;
            }
            else
            {
                vScrollBar.Visible = true;
                if (selectedBar != selectedBarOld)
                    vScrollBar.Value = 0;
                vScrollBar.Maximum = orders - 1;
                visibalWidth = vScrollBar.Left;

                firstOrd = vScrollBar.Value;
                if (firstOrd + rows > orders)
                {
                    lastOrd  = orders - 1;
                    shownOrd = lastOrd - firstOrd + 1;
                }
                else
                {
                    shownOrd = rows;
                    lastOrd  = firstOrd + shownOrd - 1;
                }
            }

            if (visibalWidth <= aiColumnX[columns])
                aiColumnX.CopyTo(aiX, 0);
            else
            {   // Scales the columns position
                float scale = (float)visibalWidth / aiColumnX[columns];
                for (int i = 0; i <= columns; i++)
                    aiX[i] = (int)(aiColumnX[i] * scale);
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
                hScrollBar.Value   = 0;
                hScrollBar.Visible = false;
            }

            selectedBarOld = selectedBar;
            btnRemoveJournal.Location = new Point(ClientSize.Width - btnRemoveJournal.Width - 1, 1);
            btnToggleJournal.Location = new Point(btnRemoveJournal.Left - btnToggleJournal.Width - 1, 1);

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

            // Caption background
            RectangleF rectfCaption = new RectangleF(0, 0, ClientSize.Width, 2 * rowHeight);
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            int  iHScrll = -hScrollBar.Value;
            Size size    = new Size(btnToggleJournal.Left, rowHeight);

            // Print the journal caption
            string stringCaptionText = Language.T("Orders During the Bar") + (Configs.AccountInMoney ? " [" + Configs.AccountCurrency + "]" : " [" + Language.T("pips") + "]");
            g.DrawString(stringCaptionText, font, brushCaptionText, new RectangleF(Point.Empty, size), sf);
            g.SetClip(new RectangleF(border, rowHeight, ClientSize.Width - 2 * border, rowHeight));
            if (Configs.AccountInMoney)
                for (int i = 0; i < columns; i++)
                    g.DrawString(asTitlesMoney[i], font, brushCaptionText, iHScrll + (aiX[i] + aiX[i + 1]) / 2, rowHeight, sf);
            else
                for (int i = 0; i < columns; i++)
                    g.DrawString(asTitlesPips[i], font, brushCaptionText, iHScrll + (aiX[i] + aiX[i + 1]) / 2, rowHeight, sf);
            g.ResetClip();

            // Paints the journal's data field
            RectangleF rectField = new RectangleF(border, 2 * rowHeight, ClientSize.Width - 2 * border, ClientSize.Height - 2 * rowHeight - border);
            g.FillRectangle(new SolidBrush(colorBack), rectField);

            size = new Size(ClientSize.Width - vScrollBar.Width - 2 * border, rowHeight);

            // Prints the journal data
            for (int ord = firstOrd; ord < firstOrd + shownOrd; ord++)
            {
                int row = ord - firstOrd;
                int y = (row + 2) * rowHeight;
                Point point = new Point(border, y);

                // Even row
                if (row % 2f != 0)
                    g.FillRectangle(brushEvenRowBack, new Rectangle(point, size));

                // Draw the position icon
                int iImgY = y + (int)Math.Floor((rowHeight - 16) / 2.0);
                g.DrawImage(aiOrderIcons[row], iHScrll + 2, iImgY, 16, 16);

                // Prints the data
                g.DrawString(asJournalData[row, 0], font, brushRowText, iHScrll + (16 + aiX[1]) / 2, (row + 2) * rowHeight, sf);
                for (int i = 1; i < columns; i++)
                {
                    if(i == columns - 1)
                        g.DrawString(asJournalData[row, i], font, brushRowText, iHScrll + aiX[i], (row + 2) * rowHeight);
                    else
                        g.DrawString(asJournalData[row, i], font, brushRowText, iHScrll + (aiX[i] + aiX[i + 1]) / 2, (row + 2) * rowHeight, sf);
                }
            }

            //g.DrawLine(penLines, 0, iRowHeight, ClientSize.Width, iRowHeight);
            for (int i = 1; i < columns; i++)
                g.DrawLine(penLines, aiX[i] + iHScrll, 2 * rowHeight, aiX[i] + iHScrll, ClientSize.Height);

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
