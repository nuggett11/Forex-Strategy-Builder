// About Screen Form
// Part of Forex Strategy Builder v2.8+
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    public class AboutScreen : Form
    {
        Fancy_Panel pnlBase;
        Label label1;
        Label label2;
        Label label3;
        Label label4;
        Label label5;
        Label label6;
        Button btnOk;
        PictureBox pictureBox1;
        LinkLabel llWebsite;
        LinkLabel llForum;
        LinkLabel llEmail;

        public AboutScreen()
        {
            pnlBase = new Fancy_Panel();
            label1  = new Label();
            label2  = new Label();
            label3  = new Label();
            label4  = new Label();
            label5  = new Label();
            label6  = new Label();
            pictureBox1 = new PictureBox();
            llWebsite   = new LinkLabel();
            llForum     = new LinkLabel();
            llEmail     = new LinkLabel();
            btnOk       = new Button();

            // Panel Base
            pnlBase.Parent = this;

            // pictureBox1
            pictureBox1.TabStop   = false;
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Image     = Properties.Resources.Logo;

            // label1
            label1.AutoSize  = true;
            label1.Font      = new Font("Microsoft Sans Serif", 16F, FontStyle.Bold);
            label1.ForeColor = LayoutColors.ColorControlText;
            label1.BackColor = Color.Transparent;
            label1.Text      = Data.ProgramName;

            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramRC)
                stage = " " + "RC";

            // label2
            label2.AutoSize  = true;
            label2.Font      = new Font("Microsoft Sans Serif", 12F);
            label2.ForeColor = LayoutColors.ColorControlText;
            label2.BackColor = Color.Transparent;
            label2.Text      = Language.T("Version") + ": " + Data.ProgramVersion + stage;

            // label3
            label3.AutoSize  = true;
            label3.Font      = new Font("Microsoft Sans Serif", 10F);
            label3.ForeColor = LayoutColors.ColorControlText;
            label3.BackColor = Color.Transparent;
            label3.Text      = "Copyright (c) 2006 - 2011 Miroslav Popov" + Environment.NewLine + Language.T("Distributor") +
                               " - Forex Software Ltd." + Environment.NewLine + Environment.NewLine + Language.T("This is a freeware program!");

            // label4
            label4.AutoSize  = true;
            label4.ForeColor = LayoutColors.ColorControlText;
            label4.BackColor = Color.Transparent;
            label4.Text      = Language.T("Website") + ":";

            // label5
            label5.AutoSize  = true;
            label5.ForeColor = LayoutColors.ColorControlText;
            label5.BackColor = Color.Transparent;
            label5.Text      = Language.T("Support forum") + ":";

            // label6
            label6.AutoSize  = true;
            label6.ForeColor = LayoutColors.ColorControlText;
            label6.BackColor = Color.Transparent;
            label6.Text      = Language.T("Contacts") + ":";

            // llWebsite
            llWebsite.AutoSize  = true;
            llWebsite.TabStop   = true;
            llWebsite.BackColor = Color.Transparent;
            llWebsite.Text      = "http://forexsb.com";
            llWebsite.LinkClicked += new LinkLabelLinkClickedEventHandler(llWebsite_LinkClicked);

            // llForum
            llForum.AutoSize     = true;
            llForum.TabStop      = true;
            llForum.BackColor    = Color.Transparent;
            llForum.Text         = "http://forexsb.com/forum";
            llForum.LinkClicked += new LinkLabelLinkClickedEventHandler(llForum_LinkClicked);

            // llEmail
            llEmail.AutoSize     = true;
            llEmail.TabStop      = true;
            llEmail.BackColor    = Color.Transparent;
            llEmail.Text         = "info@forexsb.com";
            llEmail.LinkClicked += new LinkLabelLinkClickedEventHandler(llEmail_LinkClicked);

            // Button Base
            btnOk.Parent = this;
            btnOk.Text   = Language.T("Ok");
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += new EventHandler(btnOk_Click);

            // AboutScreen
            pnlBase.Controls.Add(label1);
            pnlBase.Controls.Add(label2);
            pnlBase.Controls.Add(label3);
            pnlBase.Controls.Add(label4);
            pnlBase.Controls.Add(label5);
            pnlBase.Controls.Add(label6);
            pnlBase.Controls.Add(llEmail);
            pnlBase.Controls.Add(llForum);
            pnlBase.Controls.Add(llWebsite);
            pnlBase.Controls.Add(pictureBox1);

            StartPosition   = FormStartPosition.CenterScreen;
            Text            = Language.T("About") + " " + Data.ProgramName;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            BackColor       = LayoutColors.ColorFormBack;
            MaximizeBox     = false;
            MinimizeBox     = false;
            ShowInTaskbar   = false;
            ClientSize      = new Size(360, 280);
        }

        /// <summary>
        /// Form On Resize
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth  = (int)(Data.HorizontalDLU * 60);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int border       = btnHrzSpace;

            btnOk.Size       = new Size(buttonWidth, buttonHeight);
            btnOk.Location   = new Point(ClientSize.Width - btnOk.Width - border, ClientSize.Height - btnOk.Height - btnVertSpace);
            pnlBase.Size     = new Size(ClientSize.Width - 2 * border, btnOk.Top - border - btnVertSpace);
            pnlBase.Location = new Point(border, border);

            pictureBox1.Location = new Point(10, 3);
            pictureBox1.Size     = new Size(48, 48);
            label1.Location      = new Point(63, 10);
            label2.Location      = new Point(66, 45);
            label3.Location      = new Point(66, 77);
            label4.Location      = new Point(67, 160);
            label5.Location      = new Point(67, 180);
            label6.Location      = new Point(67, 200);
            llWebsite.Location   = new Point(label5.Right + 5, label4.Top);
            llForum.Location     = new Point(label5.Right + 5, label5.Top);
            llEmail.Location     = new Point(label5.Right + 5, label6.Top);

            pnlBase.Invalidate();
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// Connects to the web site
        /// </summary>
        void llWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://forexsb.com/");
            }
            catch { }
        }

        /// <summary>
        /// Connects to the forum
        /// </summary>
        void llForum_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://forexsb.com/forum/");
            }
            catch { }
        }

        /// <summary>
        /// Connects to the email
        /// </summary>
        void llEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("mailto:info@forexsb.com");
            }
            catch { }
        }

        /// <summary>
        /// Closes the form
        /// </summary>
        void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
