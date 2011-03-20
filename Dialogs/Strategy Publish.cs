// Forex Strategy Builder - Strategy Publish
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    public class Strategy_Publish : Form
    {
        Fancy_Panel pnlBBCodeBase;
        Fancy_Panel pnlInfoBase;
        TextBox txboxBBCode;
        Label   lblInformation;
        Button  btnClose;
        Button  btnConnect;

        /// <summary>
        /// Make a form
        /// </summary>
        public Strategy_Publish()
        {
            pnlBBCodeBase  = new Fancy_Panel();
            pnlInfoBase    = new Fancy_Panel();
            txboxBBCode    = new TextBox();
            lblInformation = new Label();
            btnClose       = new Button();
            btnConnect     = new Button();

            // BBCode_viewer
            AcceptButton = btnClose;
            BackColor    = LayoutColors.ColorFormBack;
            Icon         = Data.Icon;
            Controls.Add(btnConnect);
            Controls.Add(btnClose);
            Controls.Add(pnlBBCodeBase);
            Controls.Add(pnlInfoBase);
            MinimumSize = new System.Drawing.Size(400, 400);
            Text = Language.T("Publish a Strategy");

            pnlBBCodeBase.Padding = new Padding(4, 4, 2, 2);
            pnlInfoBase.Padding   = new Padding(4, 4, 2, 2);

            // txboxBBCode
            txboxBBCode.Parent        = pnlBBCodeBase;
            txboxBBCode.BorderStyle   = BorderStyle.None;
            txboxBBCode.Dock          = DockStyle.Fill;
            txboxBBCode.BackColor     = LayoutColors.ColorControlBack;
            txboxBBCode.ForeColor     = LayoutColors.ColorControlText;
            txboxBBCode.Multiline     = true;
            txboxBBCode.AcceptsReturn = true;
            txboxBBCode.AcceptsTab    = true;
            txboxBBCode.ScrollBars    = ScrollBars.Vertical;
            txboxBBCode.KeyDown      += new KeyEventHandler(TxboxBBCode_KeyDown);
            txboxBBCode.Text          = Data.Strategy.GenerateBBCode();

            // lblInformation
            lblInformation.Parent      = pnlInfoBase;
            lblInformation.Dock        = DockStyle.Fill;
            lblInformation.BackColor   = Color.Transparent;
            lblInformation.ForeColor   = LayoutColors.ColorControlText;
            string strInfo = Language.T("Publishing a strategy in the program's forum:") + Environment.NewLine +
                "1) " + Language.T("Open a new topic in the forum") + " \"Users' strategies\";" + Environment.NewLine +
                "2) " + Language.T("Copy / Paste the following code;") + Environment.NewLine +
                "3) " + Language.T("Describe the strategy.");
            lblInformation.Text = strInfo;

            // btnClose
            btnClose.Text   = Language.T("Close");
            btnClose.Click += new System.EventHandler(btnClose_Click);
            btnClose.UseVisualStyleBackColor = true;

            // btnConnect
            btnConnect.Text   = Language.T("Connect to") + " http://forexsb.com/forum";
            btnConnect.Click += new System.EventHandler(btnConnect_Click);
            btnConnect.UseVisualStyleBackColor = true;
        }

        /// <summary>
        /// Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            int buttonWidth = (int)(Data.HorizontalDLU * 60);
            int btnHrzSpace = (int)(Data.HorizontalDLU * 3);
            ClientSize  = new Size(4 * buttonWidth + 3 * btnHrzSpace, 480);
            MinimumSize = new Size(Width, 300);
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth  = (int)(Data.HorizontalDLU * 60);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int border       = btnHrzSpace;
            int textHeight   = Font.Height;

            // Button Close
            btnClose.Size     = new Size(buttonWidth, buttonHeight);
            btnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Connect
            btnConnect.Size     = new Size(3 * buttonWidth, buttonHeight);
            btnConnect.Location = new Point(btnClose.Left - btnConnect.Width - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // pnlInfoBase
            pnlInfoBase.Size     = new Size(ClientSize.Width - 2 * border, 65);
            pnlInfoBase.Location = new Point(border, border);

            // pnlBBCodeBase
            pnlBBCodeBase.Size     = new Size(ClientSize.Width - 2 * border, btnClose.Top - pnlInfoBase.Bottom - btnVertSpace - border);
            pnlBBCodeBase.Location = new Point(border, pnlInfoBase.Bottom + border);

            return;
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// Accept Ctrl-A
        /// </summary>
        void TxboxBBCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.A))
            {
                ((TextBox)sender).SelectAll();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        /// </summary>
        /// Connects to the forum
        /// </summary>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://forexsb.com/forum/");
            }
            catch { }
        }

        /// </summary>
        /// Closes the form
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
