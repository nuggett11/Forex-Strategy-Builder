// Top 10 Strategy Classes
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Generator
{
    //===============================================================
    
    public class Top10StrategyInfo
    {
        int       balance;
        Top10Slot top10slot;
        Strategy  strategy;

        public int       Balance     { get { return balance;   } set { balance   = value; } }
        public Top10Slot Top10Slot   { get { return top10slot; } set { top10slot = value; } }
        public Strategy  TheStrategy { get { return strategy;  } set { strategy  = value; } }
    }

    //===============================================================

    public class Top10Layout : Panel
    {
        VScrollBar VScrollBarStrategy;
        FlowLayoutPanel flowLayoutStrategy;
        SortableDictionary<int, Top10StrategyInfo> top10Holder;
        ToolTip toolTip;

        int maxStrategies;
        int minBalance = int.MaxValue;
        int maxBalance = int.MinValue;

        int margin = 3;

        /// <summary>
        /// Constructor
        /// </summary>
        public Top10Layout(int maxStrategies)
        {
            toolTip = new ToolTip();

            this.maxStrategies = maxStrategies;
            this.BackColor = LayoutColors.ColorControlBack;

            top10Holder = new SortableDictionary<int,Top10StrategyInfo>();

            flowLayoutStrategy = new FlowLayoutPanel();
            VScrollBarStrategy = new VScrollBar();

            // FlowLayoutStrategy
            flowLayoutStrategy.Parent = this;
            flowLayoutStrategy.AutoScroll = false;
            flowLayoutStrategy.BackColor = LayoutColors.ColorControlBack;

            //VScrollBarStrategy
            VScrollBarStrategy.Parent = this;
            VScrollBarStrategy.TabStop = true;
            VScrollBarStrategy.Scroll += new ScrollEventHandler(VScrollBar_Scroll);
        }

        /// <summary>
        /// Check wether the strategy has to be added in Top10 list
        /// </summary>
        public bool IsNominated(int balance)
        {
            bool nominated = false;

            if (top10Holder.Count < maxStrategies && balance > 0)
                nominated = true;

            if(top10Holder.Count == maxStrategies && balance > minBalance)
                nominated = true;

            return nominated;
        }

        /// <summary>
        /// Arrenges the controls after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            flowLayoutStrategy.SuspendLayout();
            SetVerticalScrollBar();
            flowLayoutStrategy.ResumeLayout();
        }

        /// <summary>
        /// Adds a strategy to the top 10 Layout
        /// </summary>
        public void AddStrategyInfo(Top10StrategyInfo top10StrategyInfo)
        {
            if (top10Holder.ContainsKey(top10StrategyInfo.Balance))
            {
                return;
            }

            if(top10Holder.Count == maxStrategies && top10StrategyInfo.Balance <= minBalance)
                return;
            
            if (top10Holder.Count == maxStrategies && top10StrategyInfo.Balance > minBalance)
            {
                top10Holder.Remove(minBalance);
                top10Holder.Add(top10StrategyInfo.Balance, top10StrategyInfo);
            }
            else if (top10Holder.Count < maxStrategies)
                top10Holder.Add(top10StrategyInfo.Balance, top10StrategyInfo);

            top10Holder.ReverseSort();

            minBalance = int.MaxValue;
            maxBalance = int.MinValue;
            foreach (KeyValuePair<int, Top10StrategyInfo> keyValue in top10Holder)
            {
                if (minBalance > keyValue.Key)
                    minBalance = keyValue.Key;
                if (maxBalance < keyValue.Key)
                    maxBalance = keyValue.Key;
            }

            foreach (KeyValuePair<int, Top10StrategyInfo> keyValue in top10Holder)
                    keyValue.Value.Top10Slot.IsSelected = false;

            top10Holder[maxBalance].Top10Slot.IsSelected = true;
            
            ArrangeTop10Slots();
            SetVerticalScrollBar();
        }

        /// <summary>
        /// Resets the Top 10 layout.
        /// </summary>
        public void ClearTop10Slots()
        {
            minBalance = int.MaxValue;
            maxBalance = int.MinValue;
            top10Holder.Clear();

            ArrangeTop10Slots();
            SetVerticalScrollBar();
        }

        /// <summary>
        /// Clears the selectiona attribut of the slots.
        /// </summary>
        public void ClearSelectionOfSelectedSlot()
        {
            foreach (KeyValuePair<int, Top10StrategyInfo> keyValue in top10Holder)
                if (keyValue.Value.Top10Slot.IsSelected)
                {
                    keyValue.Value.Top10Slot.IsSelected = false;
                    keyValue.Value.Top10Slot.Invalidate();
                }
        }

        /// <summary>
        /// Returns strategy with the selected balance;
        /// </summary>
        public Strategy GetStrategy(int balance)
        {
            return top10Holder[balance].TheStrategy.Clone();
        }

        /// <summary>
        /// Arranges slots in the layout.
        /// </summary>
        void ArrangeTop10Slots()
        {
            flowLayoutStrategy.SuspendLayout();
            flowLayoutStrategy.Controls.Clear();
            foreach (KeyValuePair<int, Top10StrategyInfo> keyValue in top10Holder)
            {
                Top10Slot top10Slot = keyValue.Value.Top10Slot;
                top10Slot.Width = ClientSize.Width - VScrollBarStrategy.Width - 2 * margin;
                top10Slot.Margin = new Padding(margin, margin, 0, 0);
                top10Slot.Cursor = Cursors.Hand;
                toolTip.SetToolTip(top10Slot, Language.T("Activate the strategy."));
                flowLayoutStrategy.Controls.Add(top10Slot);
            }
            flowLayoutStrategy.ResumeLayout();
        }

        /// <summary>
        /// Shows, hides, sets the scrollbar.
        /// </summary>
        void SetVerticalScrollBar()
        {
            int width  = ClientSize.Width - VScrollBarStrategy.Width;
            int height = ClientSize.Height;
            int totalHeight = 100;

            if (top10Holder != null && top10Holder.Count > 0)
                totalHeight = top10Holder.Count * 70;
            
            if (totalHeight < height)
            {
                VScrollBarStrategy.Enabled = false;
                VScrollBarStrategy.Visible = false;
            }
            else
            {
                VScrollBarStrategy.Enabled     = true;
                VScrollBarStrategy.Visible     = true;
                VScrollBarStrategy.Value       = 0;
                VScrollBarStrategy.SmallChange = 30;
                VScrollBarStrategy.LargeChange = 60;
                VScrollBarStrategy.Maximum     = Math.Max(totalHeight - height + 80, 0);
                VScrollBarStrategy.Location    = new Point(width, 0);
                VScrollBarStrategy.Height      = height;
                VScrollBarStrategy.Cursor      = Cursors.Default;
            }

            flowLayoutStrategy.Width    = width;
            flowLayoutStrategy.Height   = totalHeight;
            flowLayoutStrategy.Location = Point.Empty;

            return;
        }

        /// <summary>
        /// The Scrolling moves the flowLayout
        /// <summary>
        void VScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            VScrollBar vscroll = (VScrollBar)sender;
            flowLayoutStrategy.Location = new Point(0, -vscroll.Value);
        }
    }

    //===============================================================

    public class Top10Slot : Panel
    {
        int border = 1;
        int space  = 4;

        Bitmap chart;
        int    balance;
        double profitPerDay;
        int    drawdown;
        double winLoss;
        bool   isSelected = false;

        public bool IsSelected { get { return isSelected; }  set { isSelected = value; } }
        public int  Balance { get { return balance; }}

        /// <summary>
        /// Sets the chart params
        /// </summary>
        public void InitSlot()
        {
            int chartHeight = ClientSize.Height - 2 * (border + space) + 1;
            int cahartWidth = (int)(1.5 * chartHeight);
            Micro_Balance_Chart_Image microChart = new Micro_Balance_Chart_Image(cahartWidth, chartHeight);

            chart        = microChart.Chart;
            balance      = Configs.AccountInMoney ? (int)Math.Round(Backtester.NetMoneyBalance) : Backtester.NetBalance;
            profitPerDay = Configs.AccountInMoney ? Backtester.MoneyProfitPerDay : Backtester.ProfitPerDay;
            drawdown     = Configs.AccountInMoney ? (int)Math.Round(Backtester.MaxMoneyDrawdown) : Backtester.MaxDrawdown;
            winLoss      = Backtester.WinLossRatio;
        }

        /// <summary>
        /// Paints the chart
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen penBorder = new Pen(Data.GetGradientColor(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);
            Pen penGlow = new Pen(LayoutColors.ColorWarningRowBack, 3);
            Brush brushFore = new SolidBrush(LayoutColors.ColorChartFore);

            // Paints the background by gradient
            RectangleF rectField = new RectangleF(1, 1, ClientSize.Width - 2, ClientSize.Height - 2);
            Data.GradientPaint(g, rectField, LayoutColors.ColorChartBack, LayoutColors.DepthControl);

            // Border
            g.DrawRectangle(penBorder, 1, 1, ClientSize.Width - 2, ClientSize.Height - 2);

            // Glow
            if (isSelected)
                g.DrawRectangle(penGlow, 3, 3, ClientSize.Width - 6, ClientSize.Height - 6);

            // Drows the chart image
            g.DrawImage(chart, new Point(border + space, border + space));

            // Drows the stats
            int textLeft = border + space + chart.Width + space;

            string[] paramNames = new string[] {
                Language.T("Account balance"),
                Language.T("Profit per day"),
                Language.T("Maximum drawdown"),
                Language.T("Win/loss ratio")
            };
            string[] paramValues = new string[] {
                " " + balance.ToString(),
                " " + (Configs.AccountInMoney ? profitPerDay.ToString("F2") : profitPerDay.ToString("F0")),
                " " + drawdown.ToString(),
                " " + winLoss.ToString("F2")
            };

            int maxParamNameLenght = 0;
            foreach (string parameter in paramNames)
            {
                float nameWidth = g.MeasureString(parameter, Font).Width;
                if (nameWidth > maxParamNameLenght)
                    maxParamNameLenght = (int)nameWidth;
            }
            int valLeft = textLeft + maxParamNameLenght;

            int maxParamValueLenght = 0;
            foreach (string val in paramValues)
            {
                float valWidth = g.MeasureString(val, Font).Width;
                if (valWidth > maxParamValueLenght)
                    maxParamValueLenght = (int)valWidth;
            }
            int unitLeft = valLeft + maxParamValueLenght;

            string unit = (Configs.AccountInMoney ? " " + Configs.AccountCurrency : " " + Language.T("pips")) ;
            int unitWidth = (int)g.MeasureString(unit, Font).Width;

            for (int i = 0; i < 4; i++)
            {
                int vPos = border + space + i * Font.Height;
                g.DrawString(paramNames[i], Font, brushFore, textLeft, vPos);
                g.DrawString(paramValues[i], Font, brushFore, valLeft, vPos);
                if (i < 3 && unitLeft + unitWidth < ClientSize.Width - 4)
                    g.DrawString(unit, Font, brushFore, unitLeft, vPos);
            }
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
