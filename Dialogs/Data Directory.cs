// Data Directoryr class
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
    /// DataDirectory
    /// </summary>
    public class DataDirectory : Form
    {
        Label   lblIntro;
        TextBox txbDataDirectory;
        Button  btnBrowse;
        Button  btnDefault;
        Button  btnAccept;
        Button  btnCancel;
        ToolTip toolTip = new ToolTip();

        Font   font;
        Color  colorText;

        /// <summary>
        /// Gets the selected Data Directory
        /// </summary>
        public string DataFolder
        {
            get
            {
                return txbDataDirectory.Text;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DataDirectory()
        {
            lblIntro         = new Label();
            txbDataDirectory = new TextBox();
            btnBrowse        = new Button();
            btnDefault       = new Button();
            btnCancel        = new Button();
            btnAccept        = new Button();

            font      = this.Font;
            colorText = LayoutColors.ColorControlText;

            MaximizeBox     = false;
            MinimizeBox     = false;
            ShowInTaskbar   = false;
            Icon            = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton    = btnAccept;
            Text            = Language.T("Data Directory");

            // Label Intro
            lblIntro.Parent    = this;
            lblIntro.ForeColor = colorText;
            lblIntro.BackColor = Color.Transparent;
            lblIntro.Text      = Language.T("Offline data directory:");

            // Data Directory
            txbDataDirectory.Parent    = this;
            txbDataDirectory.BackColor = LayoutColors.ColorControlBack;
            txbDataDirectory.ForeColor = colorText;
            txbDataDirectory.Text = Data.OfflineDataDir;

            //Button Browse
            btnBrowse.Parent = this;
            btnBrowse.Name   = "Browse";
            btnBrowse.Text   = Language.T("Browse");
            btnBrowse.Click += new EventHandler(BtnBrowse_Click);
            btnBrowse.UseVisualStyleBackColor = true;

            //Button Default
            btnDefault.Parent = this;
            btnDefault.Name   = "Default";
            btnDefault.Text   = Language.T("Default");
            btnDefault.Click += new EventHandler(BtnDefault_Click);
            btnDefault.UseVisualStyleBackColor = true;

            //Button Cancel
            btnCancel.Parent       = this;
            btnCancel.Text         = Language.T("Cancel");
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.UseVisualStyleBackColor = true;

            //Button Accept
            btnAccept.Parent       = this;
            btnAccept.Name         = "Accept";
            btnAccept.Text         = Language.T("Accept");
            btnAccept.DialogResult = DialogResult.OK;
            btnAccept.UseVisualStyleBackColor = true;
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Width  = 450;
            Height = 130;

            btnAccept.Focus();
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

            // Label Intro
            lblIntro.Size     = new Size(ClientSize.Width - 2 * btnVertSpace, font.Height);
            lblIntro.Location = new Point(btnHrzSpace, btnVertSpace);

            //Button Browse
            btnBrowse.Size     = new Size(buttonWidth, buttonHeight);
            btnBrowse.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, lblIntro.Bottom + border);

            //TextBox txbDataDirectory
            txbDataDirectory.Width    = btnBrowse.Left - 2 * btnHrzSpace;
            txbDataDirectory.Location = new Point(btnHrzSpace, btnBrowse.Top + (buttonHeight - txbDataDirectory.Height) / 2);

            //Button Cancel
            btnCancel.Size     = new Size(buttonWidth, buttonHeight);
            btnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            //Button Default
            btnDefault.Size     = new Size(buttonWidth, buttonHeight);
            btnDefault.Location = new Point(btnCancel.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            //Button Accept
            btnAccept.Size     = new Size(buttonWidth, buttonHeight);
            btnAccept.Location = new Point(btnDefault.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

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
        /// Button Browse Click
        /// <summary>
        void BtnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                txbDataDirectory.Text = fd.SelectedPath;
            }
        }

        /// <summary>
        /// Button Default Click
        /// <summary>
        void BtnDefault_Click(object sender, EventArgs e)
        {
            txbDataDirectory.Text = "";
        }
    }
}
