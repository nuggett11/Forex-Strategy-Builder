// Bar Explorer
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
    class Bar_Explorer : Form
    {
        Panel    pnlChart;
        Panel    pnlInfo;
        NumericUpDown nudGo;
        Button        btnGo;
        Button[] btnNavigate;
        Button   btnClose;
        ToolTip  toolTip;

        int bar;

        Brush brushCaptionBack;
        Brush brushCaptionText;
        Brush brushEvenRow;
        Brush brushRed;
        Brush brushBack;
        Brush brushGridText;
        Brush brushTradeLong;
        Brush brushTradeShort;
        Brush brushTradeClose;
        Brush brushBarWhite;
        Brush brushBarBlack;

        Pen penCross;
        Pen penGrid;
        Pen penGridSolid;
        Pen penAxes;
        Pen penBarBorder;

        Color colorBarWight1;
        Color colorBarWight2;
        Color colorBarBlack1;
        Color colorBarBlack2;

        Color colorLongTrade1;
        Color colorLongTrade2;
        Color colorShortTrade1;
        Color colorShortTrade2;
        Color colorClosedTrade1;
        Color colorClosedTrade2;

        Size szPrice;

        string[] asTitles;  // Journal title second row
        string[] asContent; // The text journal data
        int[]    aiColumnX; // The horizontal position of the column
        int[]    aiX;       // The scalled horizontal position of the column
        int      columns;   // The number of the columns
        Font     fontInfo;
        string   barInfo;

        int maxWayPoints = 7;
        int infoRowHeight;
        int border = 2;

        /// <summary>
        /// Initialize the form and controls
        /// </summary>
        public Bar_Explorer(int iBarNumber)
        {
            pnlChart = new Panel();
            pnlInfo  = new Panel();
            toolTip  = new ToolTip();

            bar = iBarNumber < Data.FirstBar ? Data.FirstBar : iBarNumber;

            this.Text            = Language.T("Bar Explorer");
            this.BackColor       = LayoutColors.ColorFormBack;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Icon            = Data.Icon;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.ShowInTaskbar   = false;

            fontInfo       = new Font(Font.FontFamily, 9);
            infoRowHeight = (int)Math.Max(fontInfo.Height, 18);

            barInfo = Language.T("Bar") + ": " + (bar + 1).ToString() + " " +
                Data.Time[bar].ToString(Data.DF) + " "  +
                Data.Time[bar].ToString("HH:mm") + "; " +
                Language.T("Interpolation method") + ": " +
                Backtester.InterpolationMethodToString();

            pnlChart.Parent = this;
            pnlChart.Paint += new PaintEventHandler(PnlChart_Paint);

            pnlInfo.Parent = this;
            pnlInfo.Paint += new PaintEventHandler(PnlInfo_Paint);

            btnNavigate = new Button[4];
            string [] btnNavigateText = new string [4] {"< !", "<", ">", "! >"};
            string[]  btnNavigateTips = new string [4] {
                Language.T("Previous ambiguous bar."),
                Language.T("Previous bar."),
                Language.T("Next bar."),
                Language.T("Next ambiguous bar.")};

            for (int i = 0; i < 4; i++)
            {
                btnNavigate[i] = new Button();
                btnNavigate[i].Parent = this;
                btnNavigate[i].Text   = btnNavigateText[i];
                btnNavigate[i].Name   = btnNavigateText[i];
                btnNavigate[i].Click      += new EventHandler(BtnNavigate_Click);
                btnNavigate[i].MouseWheel += new MouseEventHandler(Bar_Explorer_MouseWheel);
                btnNavigate[i].UseVisualStyleBackColor = true;
                toolTip.SetToolTip(btnNavigate[i], btnNavigateTips[i]);
            }

            btnNavigate[0].Enabled = Backtester.AmbiguousBars > 0;
            btnNavigate[3].Enabled = Backtester.AmbiguousBars > 0;

            nudGo = new NumericUpDown();
            nudGo.Parent    = this;
            nudGo.TextAlign = HorizontalAlignment.Center;
            nudGo.BeginInit();
            nudGo.Minimum   = Data.FirstBar + 1;
            nudGo.Maximum   = Data.Bars;
            nudGo.Increment = 1;
            nudGo.Value     = bar + 1;
            nudGo.EndInit();

            btnGo = new Button();
            btnGo.Parent = this;
            btnGo.Name   = "Go";
            btnGo.Text   = Language.T("Go");
            btnGo.UseVisualStyleBackColor = true;
            btnGo.Click      += new EventHandler(BtnNavigate_Click);
            btnGo.MouseWheel += new MouseEventHandler(Bar_Explorer_MouseWheel);
            toolTip.SetToolTip(btnGo, Language.T("Go to the chosen bar."));

            //Button Close
            btnClose = new Button();
            btnClose.Parent = this;
            btnClose.Text   = Language.T("Close");
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.UseVisualStyleBackColor = true;

            // Colors
            brushRed  = new SolidBrush(LayoutColors.ColorSignalRed);

            brushCaptionBack = new SolidBrush(LayoutColors.ColorCaptionBack);
            brushCaptionText = new SolidBrush(LayoutColors.ColorCaptionText);
            brushEvenRow     = new SolidBrush(LayoutColors.ColorEvenRowBack);
            brushBack        = new SolidBrush(LayoutColors.ColorControlBack);
            brushGridText    = new SolidBrush(LayoutColors.ColorChartFore);
            brushBarWhite    = new SolidBrush(LayoutColors.ColorBarWhite);
            brushBarBlack    = new SolidBrush(LayoutColors.ColorBarBlack);
            brushTradeLong   = new SolidBrush(LayoutColors.ColorTradeLong);
            brushTradeShort  = new SolidBrush(LayoutColors.ColorTradeShort);
            brushTradeClose  = new SolidBrush(LayoutColors.ColorTradeClose);

            penGrid             = new Pen(LayoutColors.ColorChartGrid);
            penGrid.DashStyle   = DashStyle.Dash;
            penGrid.DashPattern = new float[] { 4, 2 };
            penGridSolid        = new Pen(LayoutColors.ColorChartGrid);
            penAxes             = new Pen(LayoutColors.ColorChartFore);
            penCross            = new Pen(LayoutColors.ColorChartCross);
            penBarBorder        = new Pen(LayoutColors.ColorBarBorder);

            colorBarWight1 = Data.GetGradientColor(LayoutColors.ColorBarWhite,  30);
            colorBarWight2 = Data.GetGradientColor(LayoutColors.ColorBarWhite, -30);
            colorBarBlack1 = Data.GetGradientColor(LayoutColors.ColorBarBlack,  30);
            colorBarBlack2 = Data.GetGradientColor(LayoutColors.ColorBarBlack, -30);

            colorLongTrade1   = Data.GetGradientColor(LayoutColors.ColorTradeLong,   30);
            colorLongTrade2   = Data.GetGradientColor(LayoutColors.ColorTradeLong,  -30);
            colorShortTrade1  = Data.GetGradientColor(LayoutColors.ColorTradeShort,  30);
            colorShortTrade2  = Data.GetGradientColor(LayoutColors.ColorTradeShort, -30);
            colorClosedTrade1 = Data.GetGradientColor(LayoutColors.ColorTradeClose,  30);
            colorClosedTrade2 = Data.GetGradientColor(LayoutColors.ColorTradeClose, -30);

            SetJournalPoints();

            return;
        }

        /// <summary>
        /// Sets the journal colums with.
        /// </summary>
        private void SetJournalPoints()
        {
            columns   = 7;
            aiColumnX = new int[8];
            aiX       = new int[8];

            Graphics g = CreateGraphics();

            asTitles = new string[7]
            {
                Language.T("Number"),
                Language.T("Description"),
                Language.T("Price"),
                Language.T("Direction"),
                Language.T("Lots"),
                Language.T("Position"),
                Language.T("Order")
            };

            string longestDescription = "";
            foreach (WayPointType wpType in Enum.GetValues(typeof(WayPointType)))
                if (g.MeasureString(Language.T(Way_Point.WPTypeToString(wpType)), fontInfo).Width >
                    g.MeasureString(longestDescription, fontInfo).Width)
                    longestDescription = Language.T(Way_Point.WPTypeToString(wpType));

            string longestDirection = "";
            foreach (PosDirection posDir in Enum.GetValues(typeof(PosDirection)))
                if (g.MeasureString(Language.T(posDir.ToString()), fontInfo).Width >
                    g.MeasureString(longestDirection, fontInfo).Width)
                    longestDirection = Language.T(posDir.ToString());

            asContent = new string[7]
            {
                "99",
                longestDescription,
                "99.99999",
                longestDirection,
                "99999",
                "99999",
                "99999",
            };

            aiColumnX[0] = border;
            for (int i = 0; i < columns; i++)
                aiColumnX[i + 1] = aiColumnX[i] + (int)Math.Max(g.MeasureString(asContent[i], fontInfo).Width, g.MeasureString(asTitles[i], fontInfo).Width) + 4;

            szPrice = g.MeasureString("9.9999", fontInfo).ToSize();

            g.Dispose();

            return;
        }

        /// <summary>
        /// Resizes the form.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetBtnNavigate();
            for (int iBar = Data.FirstBar; iBar < Data.Bars; iBar++)
                if (Backtester.WayPoints(iBar) > maxWayPoints)
                    maxWayPoints = Backtester.WayPoints(iBar);

            int btnHrzSpace     = (int)(Data.HorizontalDLU * 3);
            int iClientSizeWidth = (int)(Math.Max(aiColumnX[columns] + 2 * btnHrzSpace, 500));
            ClientSize = new Size(iClientSizeWidth, 310 + infoRowHeight * (maxWayPoints + 2));

            return;
       }

        /// <summary>
        /// Arrange the controls.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth  = (int)(Data.HorizontalDLU * 60);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int space        = btnHrzSpace;
            int textHeight   = Font.Height;

            int width = this.ClientSize.Width - 2 * space;

            //Button Close
            btnClose.Size = new Size(buttonWidth, buttonHeight);
            btnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            for (int i = 3; i >= 0; i--)
            {
                btnNavigate[i].Size = new Size(buttonWidth / 2, buttonHeight);
                btnNavigate[i].Location = new Point(btnClose.Left - buttonWidth / 2 - (btnHrzSpace + (buttonWidth / 2 + btnHrzSpace) * (3 - i)),
                                                     ClientSize.Height - buttonHeight - btnVertSpace);
            }

            nudGo.Size     = new Size(65, buttonHeight);
            nudGo.Location = new Point(space, btnNavigate[0].Top + 3);

            btnGo.Size     = new Size(65, buttonHeight);
            btnGo.Location = new Point(nudGo.Right + btnHrzSpace, btnNavigate[0].Top);

            pnlInfo.Size      = new Size(width, infoRowHeight * (maxWayPoints + 2));
            pnlInfo.Location  = new Point(space, btnClose.Top - btnVertSpace - pnlInfo.Height);
            pnlChart.Location = new Point(space, space);
            pnlChart.Size     = new Size(width, pnlInfo.Top - 2 * space);

            btnNavigate[2].Focus();

            // Scales the columns position
            double scale = (double)pnlInfo.Width / aiColumnX[columns];
            for (int i = 0; i <= columns; i++)
                aiX[i] = (int)(aiColumnX[i] * scale);

            return;
        }

        /// <summary>
        /// Form On Paint.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);

            return;
        }

        /// <summary>
        /// Navigate to a bar on mouse wheel.
        /// </summary>
        void Bar_Explorer_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0 && btnNavigate[2].Enabled)
                Navigate(">");
            else if (e.Delta < 0 && btnNavigate[1].Enabled)
                Navigate("<");

            return;
        }

        /// <summary>
        /// Navigate to a bar on button click.
        /// </summary>
        void BtnNavigate_Click(object sender, EventArgs e)
        {
            Navigate(((Button)sender).Name);

            return;
        }

        /// <summary>
        /// Navigates to a bar.
        /// </summary>
        void Navigate(string sDir)
        {
            switch (sDir)
            {
                case "< !":
                    for (int i = bar - 1; i >= Data.FirstBar; i--)
                        if (Backtester.BackTestEval(i) == "Ambiguous")
                        {
                            bar = i;
                            break;
                        }
                    break;
                case "<":
                    if (bar > Data.FirstBar)
                        bar--;
                    break;
                case "! >":
                    for (int i = bar + 1; i < Data.Bars; i++)
                        if (Backtester.BackTestEval(i) == "Ambiguous")
                        {
                            bar = i;
                            break;
                        }
                    break;
                case ">":
                    if (bar < Data.Bars - 1)
                        bar++;
                    break;
                case "Go":
                    bar = (int)nudGo.Value - 1;
                    break;
                default:
                    break;
            }

            SetBtnNavigate();

            barInfo = Language.T("Bar") + ": " + (bar + 1).ToString() +
                    " " + Data.Time[bar].ToString(Data.DF) +
                    " " + Data.Time[bar].ToString("HH:mm") + "; " +
                    Language.T("Interpolation method") + ": " +
                    Backtester.InterpolationMethodToString();

            Rectangle rectPnlChart = new Rectangle(border, infoRowHeight, pnlChart.ClientSize.Width - 2 * border, pnlChart.ClientSize.Height - infoRowHeight - border);
            pnlChart.Invalidate(rectPnlChart);

            Rectangle rectPnlInfo = new Rectangle(border, 2 * infoRowHeight, pnlInfo.ClientSize.Width - 2 * border, pnlInfo.ClientSize.Height - 2 * infoRowHeight - border);
            pnlInfo.Invalidate(rectPnlInfo);

            nudGo.Value = bar + 1;

            return;
        }

        /// <summary>
        /// Sets the nav buttons
        /// </summary>
        void SetBtnNavigate()
        {
            if (Backtester.AmbiguousBars > 0)
            {
                bool isButtonAmbiguous = false;
                for (int i = Data.FirstBar; i < bar; i++)
                    if (Backtester.BackTestEval(i) == "Ambiguous")
                    {
                        isButtonAmbiguous = true;
                        break;
                    }
                btnNavigate[0].Enabled = isButtonAmbiguous;

                isButtonAmbiguous = false;
                for (int i = bar + 1; i < Data.Bars; i++)
                    if (Backtester.BackTestEval(i) == "Ambiguous")
                    {
                        isButtonAmbiguous = true;
                        break;
                    }
                btnNavigate[3].Enabled = isButtonAmbiguous;
            }

            btnNavigate[1].Enabled = bar > Data.FirstBar;
            btnNavigate[2].Enabled = bar < Data.Bars - 1;

            if (btnNavigate[0].Enabled)
                btnNavigate[0].ForeColor = Color.Red;
            else
                btnNavigate[0].ForeColor = btnNavigate[2].ForeColor;

            if (btnNavigate[3].Enabled)
                btnNavigate[3].ForeColor = Color.Red;
            else
                btnNavigate[3].ForeColor = btnNavigate[2].ForeColor;


            return;
        }

        /// <summary>
        /// Paints panel pnlChart
        /// </summary>
        void PnlChart_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(LayoutColors.ColorChartBack);

            if (!Data.IsData || !Data.IsResult) return;

            Panel  pnl        = (Panel)sender;
            Pen    penFore    = new Pen(LayoutColors.ColorControlText);
            string FF         = Data.FF; // Format modifier to print the floats
            int    width     = pnl.ClientSize.Width;

            Size size = new Size(width, infoRowHeight);

            StringFormat sf  = new StringFormat();
            sf.Alignment     = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Near;

            // Caption background
            PointF     pntStart     = new PointF(0, 0);
            SizeF      szfCaption   = new Size(width, infoRowHeight);
            RectangleF rectfCaption = new RectangleF(pntStart, szfCaption);
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            // Caption Text
            StringFormat stringFormatCaption  = new StringFormat();
            stringFormatCaption.LineAlignment = StringAlignment.Center;
            stringFormatCaption.Trimming      = StringTrimming.EllipsisCharacter;
            stringFormatCaption.FormatFlags   = StringFormatFlags.NoWrap;
            stringFormatCaption.Alignment     = StringAlignment.Center;
            string stringCaptionText = Language.T("Price Route Inside the Bar");
            rectfCaption = new RectangleF(border, 0, pnl.ClientSize.Width - 2 * border, infoRowHeight);
            g.DrawString(stringCaptionText, fontInfo, brushCaptionText, rectfCaption, stringFormatCaption);

            // Paint the panel background
            RectangleF rectClient = new RectangleF(0, infoRowHeight, pnl.ClientSize.Width, pnl.Height - infoRowHeight);
            Data.GradientPaint(g, rectClient, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            // Paint bar info
            RectangleF rectBarInfo = new RectangleF(border, infoRowHeight + 1, pnl.ClientSize.Width - 2 * border, infoRowHeight);
            g.DrawString(barInfo, fontInfo, brushGridText, rectBarInfo, stringFormatCaption);

            // Searching the min and the max price and volume
            width = pnl.ClientSize.Width - 2 * border;
            double maxPrice = Data.High[bar];
            double minPrice = Data.Low[bar];
            int space       = 8;
            int spcRight    = szPrice.Width + 4;
            int XLeft       = border + space;
            int XRight      = width - spcRight;
            int chartWidth  = XRight - XLeft;
            int YTop        = 2 * infoRowHeight + 6;
            int YBottom     = pnl.ClientSize.Height - 22;
            int barPixels   = 28;
            int spcLeft     = 3;
            int x           = barPixels + spcLeft;

            int pointLeft   = x + barPixels + 30;
            int pointX      = pointLeft;
            int pointRight  = XRight - 20;
            int points      = Backtester.WayPoints(bar);
            int pointRadius = 3;

            // Grid
            int iCntLabels = (int)Math.Max((YBottom - YTop) / 30d, 1);
            double deltaPoint = (Data.InstrProperties.Digits == 5 || Data.InstrProperties.Digits == 3) ? Data.InstrProperties.Point * 100 : Data.InstrProperties.Point * 10;
            double delta = Math.Max(Math.Round((maxPrice - minPrice) / iCntLabels, Data.InstrProperties.Point < 0.001 ? 3 : 1), deltaPoint);

            minPrice  = Math.Round(minPrice, Data.InstrProperties.Point < 0.001f ? 3 : 1) - Data.InstrProperties.Point * 10;
            minPrice -= delta;
            maxPrice += delta;
            iCntLabels = (int)Math.Ceiling((maxPrice - minPrice) / delta);
            maxPrice  = minPrice + iCntLabels * delta;

            double scaleY = (YBottom - YTop) / (iCntLabels * delta);
            int   yOpen   = (int)(YBottom - (Data.Open[bar]  - minPrice) * scaleY);
            int   yHigh   = (int)(YBottom - (Data.High[bar]  - minPrice) * scaleY);
            int   yLow    = (int)(YBottom - (Data.Low[bar]   - minPrice) * scaleY);
            int   yClose  = (int)(YBottom - (Data.Close[bar] - minPrice) * scaleY);

            // Find the price distance
            double priceDistance = 0;
            for (int point = 1; point < points; point++)
            {
                priceDistance += Math.Abs(Backtester.WayPoint(bar, point).Price - Backtester.WayPoint(bar, point - 1).Price);
            }
            double dPriceForAPixel = (pointRight - pointLeft) / priceDistance;

            // Points X
            int[] aiPointX = new int[points];
            aiPointX[0] = pointLeft;
            for (int point = 1; point < points - 1; point++)
            {
                int iDistance = (int)(Math.Abs(Backtester.WayPoint(bar, point).Price - Backtester.WayPoint(bar, point - 1).Price) * dPriceForAPixel);
                aiPointX[point] = aiPointX[point - 1] + iDistance;
            }
            aiPointX[points - 1] = pointRight;
            for (int point = 1; point < points - 1; point++)
            {
                if (aiPointX[point] - aiPointX[point - 1] < barPixels + 1)
                    aiPointX[point] = aiPointX[point - 1] + barPixels + 1;
            }
            for (int point = points - 2; point > 0; point--)
            {
                if (aiPointX[point + 1] - aiPointX[point] < barPixels + 1)
                    aiPointX[point] = aiPointX[point + 1] - barPixels - 1;
            }

            // Find coordinates of the Way Points
            Point[] pntWay = new Point[points];
            for (int point = 0; point < points; point++)
            {
                int pointY = (int)(YBottom - (Backtester.WayPoint(bar, point).Price - minPrice) * scaleY);
                pntWay[point] = new Point(aiPointX[point], pointY);
            }

            // Horizontal grid and Price labels
            for (double label = minPrice; label <= maxPrice + Data.InstrProperties.Point; label += delta)
            {
                int labelY = (int)(YBottom - (label - minPrice) * scaleY);
                g.DrawString(label.ToString(Data.FF), Font, brushGridText, XRight, labelY - Font.Height / 2 - 1);
                g.DrawLine(penGrid, border + space, labelY, XRight, labelY);
            }

            // Vertical Grid
            g.DrawLine(penGrid, x + barPixels / 2 - 1, YTop , x + barPixels / 2 - 1, YBottom + 2);
            for (int point = 0; point < points; point++)
            {
                Point pt1 = new Point(pntWay[point].X, YTop);
                Point pt2 = new Point(pntWay[point].X, YBottom + 2);
                Point pt3 = new Point(pntWay[point].X - 5, YBottom + 4);
                g.DrawLine(penGrid, pt1, pt2);
                g.DrawString((point + 1).ToString(), Font, brushGridText, pt3);
            }

            // Bar Number
            string barNumber = (bar + 1).ToString();
            int    stringX   = x + barPixels / 2 - 1 - g.MeasureString(barNumber, Font).ToSize().Width / 2;
            if (Backtester.BackTestEval(bar) == "Ambiguous")
                g.DrawString(barNumber, Font, brushRed, stringX, YBottom + 4);
            else
                g.DrawString(barNumber, Font, brushGridText, stringX, YBottom + 4);

            // Draw the bar
            g.DrawLine(penBarBorder, x + barPixels / 2 - 1, yLow, x + barPixels / 2 - 1, yHigh);
            if (yClose < yOpen) // White bar
            {
                Rectangle rect = new Rectangle(x, yClose, barPixels - 2, yOpen - yClose);
                LinearGradientBrush lgBrush = new LinearGradientBrush(rect, colorBarWight1, colorBarWight2, 5f);
                g.FillRectangle(lgBrush, rect);
                g.DrawRectangle(penBarBorder, x, yClose, barPixels - 2, yOpen - yClose);
            }
            else if (yClose > yOpen) // Black bar
            {
                Rectangle rect = new Rectangle(x, yOpen, barPixels - 2, yClose - yOpen);
                LinearGradientBrush lgBrush = new LinearGradientBrush(rect, colorBarBlack1, colorBarBlack2, 5f);
                g.FillRectangle(lgBrush, rect);
                g.DrawRectangle(penBarBorder, x, yOpen, barPixels - 2, yClose - yOpen);
            }
            else    // Cross
            {
                g.DrawLine(penBarBorder, x, yClose, x + barPixels - 2, yClose);
            }

            // Draw cancelled orders
            for (int orderIndex = 0; orderIndex < Backtester.Orders(bar); orderIndex++)
            {
                int ordNumber = Backtester.OrdNumb(bar, orderIndex);
                Order order = Backtester.OrdFromNumb(ordNumber);
                if (order.OrdStatus != OrderStatus.Cancelled)
                    continue;

                if (order.OrdPrice > Data.High[bar] || order.OrdPrice < Data.Low[bar])
                    continue;

                int d = barPixels / 2 - 1;
                int x1 = x + d;
                int x2 = x + barPixels - 2;
                int yDeal = (int)(YBottom - (order.OrdPrice - minPrice) * scaleY);
                Pen pen = new Pen(LayoutColors.ColorChartGrid, 2);

                if (order.OrdDir == OrderDirection.Buy)
                {
                    g.DrawLine(pen, x, yDeal, x1, yDeal);
                    g.DrawLine(pen, x1, yDeal, x2, yDeal - d);
                    g.DrawLine(pen, x2 + 1, yDeal - d + 1, x1 + d / 2 + 1, yDeal - d + 1);
                    g.DrawLine(pen, x2, yDeal - d, x2, yDeal - d / 2);
                }
                else if (order.OrdDir == OrderDirection.Sell)
                {
                    g.DrawLine(pen, x, yDeal + 1, x1 + 1, yDeal + 1);
                    g.DrawLine(pen, x1, yDeal, x2, yDeal + d);
                    g.DrawLine(pen, x1 + d / 2 + 1, yDeal + d, x2, yDeal + d);
                    g.DrawLine(pen, x2, yDeal + d, x2, yDeal + d / 2 + 1);
                }
            }

            // Draw the deals on the bar
            for (int pos = 0; pos < Backtester.Positions(bar); pos++)
            {
                if (Backtester.PosTransaction(bar, pos) == Transaction.Transfer)
                    continue;

                int yDeal = (int)(YBottom - (Backtester.PosOrdPrice(bar, pos) - minPrice) * scaleY);

                if (Backtester.PosDir(bar, pos) == PosDirection.Long ||
                    Backtester.PosDir(bar, pos) == PosDirection.Short)
                {
                    int d = barPixels / 2 - 1;
                    int x1 = x + d;
                    int x2 = x + barPixels - 2;
                    if (Backtester.OrdFromNumb(Backtester.PosOrdNumb(bar, pos)).OrdDir == OrderDirection.Buy)
                    {   // Buy
                        Pen pen = new Pen(LayoutColors.ColorTradeLong, 2);
                        g.DrawLine(pen, x, yDeal, x1, yDeal);
                        g.DrawLine(pen, x1, yDeal, x2, yDeal - d);
                        g.DrawLine(pen, x2 + 1, yDeal - d + 1, x1 + d / 2 + 1, yDeal - d + 1);
                        g.DrawLine(pen, x2, yDeal - d, x2, yDeal - d / 2);
                    }
                    else
                    {   // Sell
                        Pen pen = new Pen(LayoutColors.ColorTradeShort, 2);
                        g.DrawLine(pen, x, yDeal + 1, x1 + 1, yDeal + 1);
                        g.DrawLine(pen, x1, yDeal, x2, yDeal + d);
                        g.DrawLine(pen, x1 + d / 2 + 1, yDeal + d, x2, yDeal + d);
                        g.DrawLine(pen, x2, yDeal + d, x2, yDeal + d / 2 + 1);
                    }
                }
                else if (Backtester.PosDir(bar, pos) == PosDirection.Closed)
                {   // Close position
                    int d = barPixels / 2 - 1;
                    int x1 = x + d;
                    int x2 = x + barPixels - 3;
                    Pen pen = new Pen(LayoutColors.ColorTradeClose, 2);
                    g.DrawLine(pen, x, yDeal, x1, yDeal);
                    g.DrawLine(pen, x1, yDeal + d / 2, x2, yDeal - d / 2);
                    g.DrawLine(pen, x1, yDeal - d / 2, x2, yDeal + d / 2);
                }
            }

            // Draw position lots
            for (int point = 0; point < points; point++)
            {
                int posNumber = Backtester.WayPoint(bar, point).PosNumb;
                if (posNumber == -1) continue;

                double posLots = Backtester.PosFromNumb(posNumber).PosLots;
                PosDirection posDirection = Backtester.PosFromNumb(posNumber).PosDir;
                WayPointType wpType = Backtester.WayPoint(bar, point).WPType;

                int posHight = (int)(Math.Max(posLots * 2, 2));
                int posY     = YBottom - posHight;
                int d        = (barPixels - 1) / 2;
                x = pntWay[point].X - d;

                if (posDirection == PosDirection.Long)
                 {   // Long
                    Rectangle rect = new Rectangle(x - 1, posY, barPixels + 1, posHight);
                    LinearGradientBrush lgBrush = new LinearGradientBrush(rect, colorLongTrade1, colorLongTrade2, 0f);
                    rect = new Rectangle(x, posY, barPixels - 1, posHight);
                    g.FillRectangle(lgBrush, rect);
                }
                else if (posDirection == PosDirection.Short)
                {   // Short
                    Rectangle rect = new Rectangle(x - 1, posY, barPixels + 1, posHight);
                    LinearGradientBrush lgBrush = new LinearGradientBrush(rect, colorShortTrade1, colorShortTrade2, 0f);
                    rect = new Rectangle(x, posY, barPixels - 1, posHight);
                    g.FillRectangle(lgBrush, rect);
                }
                else if (posDirection == PosDirection.Closed && wpType == WayPointType.Exit)
                {   // Closed
                    Rectangle rect = new Rectangle(x - 1, YBottom - 2, barPixels + 1, 2);
                    LinearGradientBrush lgBrush = new LinearGradientBrush(rect, colorClosedTrade1, colorClosedTrade2, 0f);
                    rect = new Rectangle(x, YBottom - 2, barPixels - 1, 2);
                    g.FillRectangle(lgBrush, rect);
                }
            }

            // Draw the Beziers
            for (int point = 1; point < points; point++)
            {
                Point ptKnot1 = pntWay[point - 1];
                Point ptKnot2 = pntWay[point];

                int ctrlX1 = (ptKnot1.X + ptKnot2.X) / 2;
                int ctrlX2 = (ptKnot1.X + ptKnot2.X) / 2;

                int ctrlY1 = ptKnot1.Y;
                int ctrlY2 = ptKnot2.Y;

                if (point > 1)
                {
                    if (pntWay[point - 2].Y > pntWay[point - 1].Y && pntWay[point - 1].Y > pntWay[point].Y ||
                        pntWay[point - 2].Y < pntWay[point - 1].Y && pntWay[point - 1].Y < pntWay[point].Y)
                    {
                        ctrlY1 = (pntWay[point - 1].Y + pntWay[point].Y) / 2;
                    }
                }
                if (point < points - 1)
                {
                    if (pntWay[point - 1].Y > pntWay[point].Y && pntWay[point].Y > pntWay[point + 1].Y ||
                        pntWay[point - 1].Y < pntWay[point].Y && pntWay[point].Y < pntWay[point + 1].Y)
                    {
                        ctrlY2 = (pntWay[point - 1].Y + pntWay[point].Y) / 2;
                    }
                }

                if(point == 1)
                {
                    ctrlX1 = ptKnot1.X;
                    ctrlY1 = ptKnot1.Y;
                }
                if(point == points - 1)
                {
                    ctrlX2 = ptKnot2.X;
                    ctrlY2 = ptKnot2.Y;
                }

                Point ptControl1 = new Point(ctrlX1, ctrlY1);
                Point ptControl2 = new Point(ctrlX2, ctrlY2);

                g.DrawBezier(penCross, ptKnot1, ptControl1, ptControl2, ptKnot2);
            }

            // Draw the WayPoints
            Brush brushWeyPnt = new SolidBrush(LayoutColors.ColorChartBack);
            for (int point = 0; point < points; point++)
            {
                g.FillEllipse(brushWeyPnt, pntWay[point].X - pointRadius, pntWay[point].Y - pointRadius, 2 * pointRadius, 2 * pointRadius);
                g.DrawEllipse(penCross,    pntWay[point].X - pointRadius, pntWay[point].Y - pointRadius, 2 * pointRadius, 2 * pointRadius);
            }

            // Draw the deals on the route
            for (int point = 0; point < points; point++)
            {
                int posNumber = Backtester.WayPoint(bar, point).PosNumb;
                int ordNumber = Backtester.WayPoint(bar, point).OrdNumb;

                if (posNumber < 0 || ordNumber < 0)
                    continue;

                PosDirection   posDirection = Backtester.PosFromNumb(posNumber).PosDir;
                OrderDirection ordDirection = Backtester.OrdFromNumb(ordNumber).OrdDir;
                WayPointType   wpType       = Backtester.WayPoint(bar, point).WPType;

                if (Backtester.PosFromNumb(posNumber).Transaction == Transaction.Transfer ||
                    wpType == WayPointType.Cancel || wpType == WayPointType.None ||
                    wpType == WayPointType.Open   || wpType == WayPointType.High ||
                    wpType == WayPointType.Low    || wpType == WayPointType.Close )
                    continue;

                int yDeal = pntWay[point].Y;

                if (posDirection == PosDirection.Long ||
                    posDirection == PosDirection.Short)
                {
                    int d = barPixels / 2 - 1;
                    x = pntWay[point].X - d;
                    int x1 = pntWay[point].X;
                    int x2 = x + barPixels - 2;
                    if (ordDirection == OrderDirection.Buy)
                    {   // Buy
                        Pen pen = new Pen(LayoutColors.ColorTradeLong, 2);
                        g.DrawLine(pen, x, yDeal, x1, yDeal);
                        g.DrawLine(pen, x1, yDeal, x2, yDeal - d);
                        g.DrawLine(pen, x2 + 1, yDeal - d + 1, x1 + d / 2 + 1, yDeal - d + 1);
                        g.DrawLine(pen, x2, yDeal - d, x2, yDeal - d / 2);
                    }
                    else
                    {   // Sell
                        Pen pen = new Pen(LayoutColors.ColorTradeShort, 2);
                        g.DrawLine(pen, x, yDeal + 1, x1 + 1, yDeal + 1);
                        g.DrawLine(pen, x1, yDeal, x2, yDeal + d);
                        g.DrawLine(pen, x1 + d / 2 + 1, yDeal + d, x2, yDeal + d);
                        g.DrawLine(pen, x2, yDeal + d, x2, yDeal + d / 2 + 1);
                    }
                }
                else if (posDirection == PosDirection.Closed)
                {   // Close position
                    int d = barPixels / 2 - 1;
                    x = pntWay[point].X - d;
                    int x1 = pntWay[point].X;
                    int x2 = x + barPixels - 3;
                    Pen pen = new Pen(LayoutColors.ColorTradeClose, 2);
                    g.DrawLine(pen, x, yDeal, x1, yDeal);
                    g.DrawLine(pen, x1, yDeal + d / 2, x2, yDeal - d / 2);
                    g.DrawLine(pen, x1, yDeal - d / 2, x2, yDeal + d / 2);
                }
            }

            // Coordinate axes
            g.DrawLine(penAxes, XLeft, YTop - 4, XLeft, YBottom); // Vertical left line
            g.DrawLine(penAxes, XLeft, YBottom, XRight, YBottom);

            // Border
            Pen penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);
            g.DrawLine(penBorder, 1, infoRowHeight, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - border + 1, infoRowHeight, pnl.ClientSize.Width - border + 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - border + 1, pnl.ClientSize.Width, pnl.ClientSize.Height - border + 1);
        }

        /// <summary>
        /// Paints panel pnlInfo
        /// </summary>
        void PnlInfo_Paint(object sender, PaintEventArgs e)
        {
            // ---------------------------------------------------------------------+
            // |                          Way points description                    |
            // |--------------------------------------------------------------------+
            // | Number | Description | Price | Direction | Lots | Position | Order |
            // |--------------------------------------------------------------------+
            //xp0      xp1           xp2     xp3         xp4    xp5        xp6     xp7

            Graphics g = e.Graphics;
            g.Clear(LayoutColors.ColorControlBack);

            if (!Data.IsData || !Data.IsResult) return;

            Panel  pnl     = (Panel)sender;
            Brush  brush   = Brushes.White;
            string FF      = Data.FF; // Format modifier to print the floats

            Size size = new Size(aiX[columns] - aiX[0], infoRowHeight);

            StringFormat sf  = new StringFormat();
            sf.Alignment     = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Near;

            // Caption background
            PointF     pntStart     = new PointF(0, 0);
            SizeF      szfCaption   = new Size(pnl.ClientSize.Width, 2 * infoRowHeight);
            RectangleF rectfCaption = new RectangleF(pntStart, szfCaption);
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);

            // Caption Text
            StringFormat stringFormatCaption  = new StringFormat();
            stringFormatCaption.LineAlignment = StringAlignment.Center;
            stringFormatCaption.Trimming      = StringTrimming.EllipsisCharacter;
            stringFormatCaption.FormatFlags   = StringFormatFlags.NoWrap;
            stringFormatCaption.Alignment     = StringAlignment.Near;
            string stringCaptionText = Language.T("Way Points Description");
            float  fCaptionWidth     = (float)Math.Min(pnlInfo.ClientSize.Width, aiX[columns] - aiX[0]);
            float  fCaptionTextWidth = g.MeasureString(stringCaptionText, fontInfo).Width;
            float  fCaptionTextX     = (float)Math.Max((fCaptionWidth - fCaptionTextWidth) / 2f, 0);
            PointF pfCaptionText     = new PointF(fCaptionTextX, 0);
            SizeF  sfCaptionText     = new SizeF(fCaptionWidth - fCaptionTextX, infoRowHeight);
            rectfCaption = new RectangleF(pfCaptionText, sfCaptionText);

            // First caption raw
            g.DrawString(stringCaptionText, fontInfo, brushCaptionText, rectfCaption, stringFormatCaption);

            // Second caption raw
            for (int i = 0; i < columns; i++)
                g.DrawString(asTitles[i], fontInfo, brushCaptionText, (aiX[i] + aiX[i + 1]) / 2, infoRowHeight, sf);

            brush = new SolidBrush(LayoutColors.ColorControlText);

            for (int pnt = 0; pnt < Backtester.WayPoints(bar); pnt++)
            {
                int   y     = (pnt + 2) * infoRowHeight;
                Point point = new Point(aiX[0], y);

                // Even row
                if (pnt % 2f != 0)
                    g.FillRectangle(brushEvenRow, new Rectangle(point, size));

                int positionNumber        = Backtester.WayPoint(bar, pnt).PosNumb;
                WayPointType wpType       = Backtester.WayPoint(bar, pnt).WPType;
                PosDirection posDirection = Backtester.PosFromNumb(positionNumber).PosDir;
                double posLots   = Backtester.PosFromNumb(positionNumber).PosLots;
                int    ordNumber = Backtester.WayPoint(bar, pnt).OrdNumb;

                g.DrawString((pnt + 1).ToString(), fontInfo, brush, (aiX[0] + aiX[1]) / 2, y, sf);
                g.DrawString(Language.T(Way_Point.WPTypeToString(wpType)), fontInfo, brush, aiX[1] + 2, y);
                g.DrawString(Backtester.WayPoint(bar, pnt).Price.ToString(FF), fontInfo, brush, (aiX[3] + aiX[2]) / 2, y, sf);

                if (positionNumber > -1)
                {
                    g.DrawString(Language.T(posDirection.ToString()), fontInfo, brush, (aiX[4] + aiX[3]) / 2, y, sf);
                    g.DrawString(posLots.ToString(), fontInfo, brush, (aiX[5] + aiX[4]) / 2, y, sf);
                    g.DrawString((positionNumber + 1).ToString(), fontInfo, brush, (aiX[6] + aiX[5]) / 2, y, sf);
                }

                if (ordNumber > -1)
                {
                    g.DrawString((ordNumber + 1).ToString(), fontInfo, brush, (aiX[7] + aiX[6]) / 2, y, sf);
                }
            }

            // Vertical lines
            Pen penLine = new Pen(LayoutColors.ColorJournalLines);
            for (int i = 1; i < columns; i++)
                g.DrawLine(penLine, aiX[i], 2 * infoRowHeight, aiX[i], ClientSize.Height - border);

            // Border
            Pen penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);
            g.DrawLine(penBorder, 1, 2 * infoRowHeight, 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, pnl.ClientSize.Width - border + 1, 2 * infoRowHeight, pnl.ClientSize.Width - border + 1, pnl.ClientSize.Height);
            g.DrawLine(penBorder, 0, pnl.ClientSize.Height - border + 1, pnl.ClientSize.Width, pnl.ClientSize.Height - border + 1);

            return;
        }
    }
}
