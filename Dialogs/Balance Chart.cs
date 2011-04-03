// Balance Chart
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs
{
    class Balance_Chart : Form
    {
        Button btnClose;
        Small_Balance_Chart balanceChart;
        Label lblDynInfo;

        /// <summary>
        /// Constructor
        /// </summary>
        public Balance_Chart()
        {
            Text = Language.T("Balance / Equity Chart");
            Icon = Data.Icon;
            AcceptButton = btnClose;

            // Button Close
            btnClose = new Button();
            btnClose.Parent = this;
            btnClose.Text   = Language.T("Close");
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.UseVisualStyleBackColor = true;

            // Balance chart
            balanceChart = new Small_Balance_Chart();
            balanceChart.Parent = this;
            balanceChart.MouseMove  += new MouseEventHandler(BalanceChart_MouseMove);
            balanceChart.MouseLeave += new EventHandler(BalanceChart_MouseLeave);
            balanceChart.ShowDynamicInfo = true;
            balanceChart.SetChartData();
            balanceChart.InitChart();

            // Label Dynamic Info
            lblDynInfo = new Label();
            lblDynInfo.Parent = this;
            lblDynInfo.ForeColor = LayoutColors.ColorControlText;
            lblDynInfo.BackColor = Color.Transparent;
        }

        /// <summary>
        /// Perform initializing.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ClientSize  = new Size(500, 400);
            MinimumSize = new Size(300, 250);

            return;
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth  = (int)(Data.HorizontalDLU * 60);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int space = btnHrzSpace;

            // Button Close
            btnClose.Size = new Size(buttonWidth, buttonHeight);
            btnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Balance Chart
            balanceChart.Size     = new Size(ClientSize.Width - 2 * space, btnClose.Top - space - btnVertSpace);
            balanceChart.Location = new Point(space, space);

            // Label dynamic info.
            lblDynInfo.Width = btnClose.Left - 2 * space;
            lblDynInfo.Location = new Point(space, btnClose.Top + 6);
        }

        /// <summary>
        /// Form On Paint.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// Show the dynamic info on the status bar.
        /// </summary>
        void BalanceChart_MouseMove(object sender, MouseEventArgs e)
        {
            Small_Balance_Chart chart = (Small_Balance_Chart)sender;
            lblDynInfo.Text = chart.CurrentBarInfo;

            return;
        }

        /// <summary>
        /// Deletes the dynamic info on the status bar.
        /// </summary>
        void BalanceChart_MouseLeave(object sender, EventArgs e)
        {
            lblDynInfo.Text = string.Empty;

            return;
        }
    }
}
