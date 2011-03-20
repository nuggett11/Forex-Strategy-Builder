// Data Horizon
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
    /// The Generator
    /// </summary>
    public class Data_Horizon : Form
    {
        Button         btnAccept;
        Button         btnCancel;
        Button         btnHelp;
        Fancy_Panel    pnlBase;
        DateTimePicker dtpStartDate;
        DateTimePicker dtpEndDate;
        CheckBox       chboxUseEndDate;
        CheckBox       chboxUseStartDate;
        NumericUpDown  numUpDownMaxBars;
        Label          lblMaxBars;
        Label          lblMinBars;
        ToolTip        toolTip = new ToolTip();

        Font  font;
        Color colorText;

        int  maxBars      = 20000;
        bool useEndDate   = false;
        bool useStartDate = false;

        DateTime dtStart = new DateTime(1990, 1, 1);
        DateTime dtEnd   = new DateTime(2020, 1, 1);

        /// <summary>
        /// Maximum data bars
        /// </summary>
        public int MaxBars { get {return maxBars;} set { maxBars = value; } }

        /// <summary>
        /// Starting date
        /// </summary>
        public DateTime StartDate { get { return dtStart; } set { dtStart = value; } }

        /// <summary>
        /// Ending date
        /// </summary>
        public DateTime EndDate { get { return dtEnd; } set { dtEnd = value; } }

        /// <summary>
        /// Use end date
        /// </summary>
        public bool UseEndDate { get { return useEndDate; } set { useEndDate = value; } }

        /// <summary>
        /// Use start date
        /// </summary>
        public bool UseStartDate { get { return useStartDate; } set { useStartDate = value; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public Data_Horizon(int iMaxBars, DateTime dtStart, DateTime dtEnd, bool bUseStartDate, bool bUseEndDate)
        {
            this.maxBars      = iMaxBars;
            this.dtStart       = dtStart;
            this.dtEnd         = dtEnd;
            this.useEndDate   = bUseEndDate;
            this.useStartDate = bUseStartDate;

            btnAccept         = new Button();
            btnHelp           = new Button();
            btnCancel         = new Button();
            pnlBase           = new Fancy_Panel();
            dtpStartDate      = new DateTimePicker();
            dtpEndDate        = new DateTimePicker();
            chboxUseEndDate   = new CheckBox();
            chboxUseStartDate = new CheckBox();
            numUpDownMaxBars  = new NumericUpDown();
            lblMaxBars        = new Label();
            lblMinBars        = new Label();

            font             = this.Font;
            colorText        = LayoutColors.ColorControlText;

            MaximizeBox     = false;
            MinimizeBox     = false;
            ShowInTaskbar   = false;
            Icon            = Data.Icon;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton    = btnAccept;
            Text            = Language.T("Data Horizon");

            //Button Help
            btnHelp.Parent = this;
            btnHelp.Name   = "Help";
            btnHelp.Text   = Language.T("Help");
            btnHelp.UseVisualStyleBackColor = true;
            btnHelp.Click += new EventHandler(BtnHelp_Click);

            //Button Accept
            btnAccept.Parent       = this;
            btnAccept.Name         = "Ok";
            btnAccept.Text         = Language.T("Accept");
            btnAccept.DialogResult = DialogResult.OK;
            btnAccept.UseVisualStyleBackColor = true;

            //Button Cancel
            btnCancel.Parent       = this;
            btnCancel.Text         = Language.T("Cancel");
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.UseVisualStyleBackColor = true;

            // Panel pnlBase
            pnlBase.Parent = this;

            // chboxUseEndDate
            chboxUseEndDate.Parent    = pnlBase;
            chboxUseEndDate.ForeColor = LayoutColors.ColorControlText;
            chboxUseEndDate.BackColor = Color.Transparent;
            chboxUseEndDate.AutoSize  = true;
            chboxUseEndDate.Text      = Language.T("Remove data newer than:");
            chboxUseEndDate.CheckStateChanged += new EventHandler(chboxUseEndDate_CheckStateChanged);
            toolTip.SetToolTip(chboxUseEndDate, Language.T("All data newer than the specified date will be cut out."));

            // chboxUseStartDate
            chboxUseStartDate.Parent    = pnlBase;
            chboxUseStartDate.AutoSize  = true;
            chboxUseStartDate.ForeColor = LayoutColors.ColorControlText;
            chboxUseStartDate.BackColor = Color.Transparent;
            chboxUseStartDate.Text      = Language.T("Remove data older than:");
            chboxUseStartDate.CheckStateChanged += new EventHandler(chboxUseStartDate_CheckStateChanged);
            toolTip.SetToolTip(chboxUseStartDate, Language.T("All data older than the specified date will be cut out."));

            // Start Date
            dtpStartDate.Parent        = pnlBase;
            dtpStartDate.ForeColor     = LayoutColors.ColorControlText;
            dtpStartDate.ShowUpDown    = true;
            dtpStartDate.ValueChanged += new EventHandler(dtpStartDate_ValueChanged);

            // End Date
            dtpEndDate.Parent        = pnlBase;
            dtpEndDate.ForeColor     = LayoutColors.ColorControlText;
            dtpEndDate.ShowUpDown    = true;
            dtpEndDate.ValueChanged += new EventHandler(dtpEndDate_ValueChanged);

            //lblMaxBars
            lblMaxBars.Parent    = pnlBase;
            lblMaxBars.AutoSize  = true;
            lblMaxBars.ForeColor = LayoutColors.ColorControlText;
            lblMaxBars.BackColor = Color.Transparent;
            lblMaxBars.Text      = Language.T("Maximum number of bars:");
            lblMaxBars.TextAlign = ContentAlignment.MiddleLeft;

            // numUpDownMaxBars
            numUpDownMaxBars.BeginInit();
            numUpDownMaxBars.Parent    = pnlBase;
            numUpDownMaxBars.Name      = "MaxBars";
            numUpDownMaxBars.Minimum   = Configs.MIN_BARS;
            numUpDownMaxBars.Maximum   = Configs.MAX_BARS;
            numUpDownMaxBars.ThousandsSeparator = true;
            numUpDownMaxBars.ValueChanged += new EventHandler(numUpDown_ValueChanged);
            numUpDownMaxBars.TextAlign     = HorizontalAlignment.Center;
            numUpDownMaxBars.EndInit();

            //lblMinBars
            lblMinBars.Parent    = pnlBase;
            lblMinBars.AutoSize  = true;
            lblMinBars.ForeColor = LayoutColors.ColorControlText;
            lblMinBars.BackColor = Color.Transparent;
            lblMinBars.Text      = Language.T("Minimum number of bars:") + " " + Configs.MIN_BARS.ToString();
            lblMinBars.TextAlign = ContentAlignment.MiddleLeft;
        }

        /// <summary>
        /// Go to the online help
        /// </summary>
        void BtnHelp_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://forexsb.com/wiki/fsb/manual/data_horizon");
            }
            catch { }
        }

        /// <summary>
        /// Change the dtpStartDate value
        /// </summary>
        void dtpStartDate_ValueChanged(object sender, EventArgs e)
        {
            dtStart = dtpStartDate.Value;
            if (dtEnd - dtStart < new TimeSpan(1, 0, 0, 0, 0))
            {
                dtEnd = dtStart + new TimeSpan(1, 0, 0, 0, 0);
                dtpEndDate.Value = dtEnd;
            }
        }

        /// <summary>
        /// Change the dtpEndDate value
        /// </summary>
        void dtpEndDate_ValueChanged(object sender, EventArgs e)
        {
            dtEnd = dtpEndDate.Value;
            if (dtEnd - dtStart < new TimeSpan(1, 0, 0, 0, 0))
            {
                dtStart = dtEnd - new TimeSpan(1, 0, 0, 0, 0);
                dtpStartDate.Value = dtStart;
            }
        }

        /// <summary>
        /// Change the bUseStartDate value
        /// </summary>
        void chboxUseStartDate_CheckStateChanged(object sender, EventArgs e)
        {
            useStartDate = chboxUseStartDate.Checked;
            dtpStartDate.Enabled = useStartDate;
        }

        /// <summary>
        /// Change the bUseEndDate value
        /// </summary>
        void chboxUseEndDate_CheckStateChanged(object sender, EventArgs e)
        {
            useEndDate = chboxUseEndDate.Checked;
            dtpEndDate.Enabled = useEndDate;
        }

        /// <summary>
        /// Change the iMaxBars value
        /// </summary>
        void numUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown num = (NumericUpDown)sender;
            maxBars = (int)num.Value;
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            numUpDownMaxBars.Value    = maxBars;
            dtpStartDate.Value        = dtStart;
            dtpEndDate.Value          = dtEnd;
            chboxUseEndDate.Checked   = useEndDate;
            dtpEndDate.Enabled        = useEndDate;
            chboxUseStartDate.Checked = useStartDate;
            dtpStartDate.Enabled      = useStartDate;

            int buttonWidth  = (int)(Data.HorizontalDLU * 60);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            ClientSize = new Size(3 * buttonWidth + 4 * btnHrzSpace, 230);
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
            int textHeight   = Font.Height;
            int space        = btnHrzSpace;
            int border       = 2;

            // Button Cancel
            btnCancel.Size     = new Size(buttonWidth, buttonHeight);
            btnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            btnAccept.Size     = new Size(buttonWidth, buttonHeight);
            btnAccept.Location = new Point(btnCancel.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Help
            btnHelp.Size     = new Size(buttonWidth, buttonHeight);
            btnHelp.Location = new Point(btnAccept.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Panel Base
            pnlBase.Size     = new Size(ClientSize.Width - 2 * space, btnAccept.Top - btnVertSpace - space);
            pnlBase.Location = new Point(space, space);

            //Label Max bars
            lblMaxBars.Location = new Point(space, 2 * space + 2 * border);

            //chboxUseStartDate
            chboxUseStartDate.Location = new Point(space + 4, lblMaxBars.Bottom + 4 * space);

            // Start Date
            dtpStartDate.Width    = pnlBase.ClientSize.Width - 2 * space - 8;
            dtpStartDate.Location = new Point(space + 4, chboxUseStartDate.Bottom + space);

            //chboxUseEndDate
            chboxUseEndDate.Location = new Point(space + 4, dtpStartDate.Bottom + 4 * space);

            // End Date
            dtpEndDate.Width    = dtpStartDate.Width;
            dtpEndDate.Location = new Point(space + 4, chboxUseEndDate.Bottom + space);

            //numUpDownMaxBars
            numUpDownMaxBars.Width    = 80;
            numUpDownMaxBars.Location = new Point(dtpStartDate.Right - numUpDownMaxBars.Width, 2 * space + 2 * border - 2);

            // lblMinBars
            lblMinBars.Location = new Point(lblMaxBars.Left, dtpEndDate.Bottom + 3 * space);

            return;
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }
    }
}
