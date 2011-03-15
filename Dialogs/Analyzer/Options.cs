// Strategy Analyzer - Options
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Analyzer
{
    public class Options : Fancy_Panel
    {
        Label lblColumnSeparator;
        Label lblDecimalSeparator;

        ComboBox cbxColumnSeparator;
        ComboBox cbxDecimalSeparator;

        CheckBox chbHideFSB;
        Form formFSB;
        public Form SetParrentForm { set { formFSB = value;  } }

        public Options(string caption) : base (caption)
        {
            Color colorText = LayoutColors.ColorControlText;

            lblColumnSeparator  = new Label();
            lblDecimalSeparator = new Label();

            cbxColumnSeparator  = new ComboBox();
            cbxDecimalSeparator = new ComboBox();

            // Label Decimal Separator
            lblDecimalSeparator.Parent    = this;
            lblDecimalSeparator.ForeColor = colorText;
            lblDecimalSeparator.BackColor = Color.Transparent;
            lblDecimalSeparator.AutoSize  = true;
            lblDecimalSeparator.Text      = Language.T("Report - decimal separator");

            // Label Column Separator
            lblColumnSeparator.Parent    = this;
            lblColumnSeparator.ForeColor = colorText;
            lblColumnSeparator.BackColor = Color.Transparent;
            lblColumnSeparator.AutoSize  = true;
            lblColumnSeparator.Text      = Language.T("Report - column separator");

            // ComboBox Column Separator
            cbxDecimalSeparator.Parent = this;
            cbxDecimalSeparator.Items.AddRange(new string[] { Language.T("Dot") + " '.'", Language.T("Comma") + " ','" });
            cbxDecimalSeparator.SelectedIndex = Configs.DecimalSeparator == "." ? 0 : 1;
            cbxDecimalSeparator.SelectedIndexChanged += new EventHandler(DecimalSeparator_Changed);

            // ComboBox Column Separator
            cbxColumnSeparator.Parent = this;
            cbxColumnSeparator.Items.AddRange(new string[] { Language.T("Comma") + " ','", Language.T("Semicolon") + " ';'", Language.T("Tab") + @" '\t'" });
            cbxColumnSeparator.SelectedIndex = Configs.ColumnSeparator == "," ? 0 : Configs.ColumnSeparator == ";" ? 1 : 2;
            cbxColumnSeparator.SelectedIndexChanged += new EventHandler(ColumnSeparator_Changed);

            // Hide FSB at startup.
            chbHideFSB = new CheckBox();
            chbHideFSB.Parent    = this;
            chbHideFSB.ForeColor = colorText;
            chbHideFSB.BackColor = Color.Transparent;
            chbHideFSB.Text      = Language.T("Hide FSB when Analyzer starts");
            chbHideFSB.Checked   = Configs.AnalyzerHideFSB;
            chbHideFSB.AutoSize  = true;
            chbHideFSB.CheckedChanged += new EventHandler(HideFSB_Click);

            this.Resize += new EventHandler(pnlOptions_Resize);
        }

        void pnlOptions_Resize(object sender, EventArgs e)
        {
            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth  = (int)(Data.HorizontalDLU * 60);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int space    = btnHrzSpace;
            int border   = 2;

            lblDecimalSeparator.Location = new Point(btnHrzSpace + border, 0 * buttonHeight + 2 * space + 18);
            lblColumnSeparator.Location  = new Point(btnHrzSpace + border, 1 * buttonHeight + 3 * space + 18);

            int maxLabelRight = lblDecimalSeparator.Right;
            if (lblColumnSeparator.Right > maxLabelRight) maxLabelRight = lblColumnSeparator.Right;
            int cbxLeft  = maxLabelRight + btnHrzSpace;
            int cbxWidth = 120;
            cbxDecimalSeparator.Size     = new Size(cbxWidth, buttonHeight);
            cbxColumnSeparator.Size      = new Size(cbxWidth, buttonHeight);
            cbxDecimalSeparator.Location = new Point(cbxLeft, 0 * buttonHeight + 2 * space + 16);
            cbxColumnSeparator.Location  = new Point(cbxLeft, 1 * buttonHeight + 3 * space + 16);

            chbHideFSB.Location = new Point(btnHrzSpace + border + 4, 2 * buttonHeight + 4 * space + 18);
        }

        void ColumnSeparator_Changed(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            switch (comboBox.SelectedIndex)
            {
                case 0: // Comma
                    Configs.ColumnSeparator = ",";
                    break;
                case 1: // Semicolon
                    Configs.ColumnSeparator = ";";
                    break;
                case 2: // Tab
                    Configs.ColumnSeparator = "\t";
                    break;
                default:
                    break;

            }
        }

        void DecimalSeparator_Changed(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            switch (comboBox.SelectedIndex)
            {
                case 0: // Dot
                    Configs.DecimalSeparator = ".";
                    break;
                case 1: // Comma
                    Configs.DecimalSeparator = ",";
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Toggles FSB visibility.
        /// </summary>
        void HideFSB_Click(object sender, EventArgs e)
        {
            if (formFSB != null)
                formFSB.Visible = !chbHideFSB.Checked;

            Configs.AnalyzerHideFSB = chbHideFSB.Checked;
        }

        /// <summary>
        /// Shows FSB
        /// </summary>
        public void ShowFSB()
        {
            if (formFSB != null)
                formFSB.Visible = true;
        }

        /// <summary>
        /// Shows or Hides FSB
        /// </summary>
        public void SetFSBVisiability()
        {
            if (formFSB != null)
                formFSB.Visible = !Configs.AnalyzerHideFSB;
        }
    }
}
