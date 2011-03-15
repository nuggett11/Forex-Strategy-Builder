// Pivot Points Calculator
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    class Pivot_Points_Calculator : Form
    {
        Fancy_Panel pnlInput;
        Fancy_Panel pnlOutput;

        Label[]   alblInputNames;
        TextBox[] atbxInputValues;
        Label[]   alblOutputNames;
        Label[]   alblOutputValues;

        Font font;
        Color colorText;

        /// <summary>
        /// Constructor
        /// </summary>
        public Pivot_Points_Calculator()
        {
            pnlInput  = new Fancy_Panel(Language.T("Input Values"));
            pnlOutput = new Fancy_Panel(Language.T("Output Values"));

            alblInputNames   = new Label[3];
            atbxInputValues  = new TextBox[3];
            alblOutputNames  = new Label[7];
            alblOutputValues = new Label[7];

            font      = this.Font;
            colorText = LayoutColors.ColorControlText;

            MaximizeBox     = false;
            MinimizeBox     = false;
            Icon            = Data.Icon;
            BackColor       = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Text            = Language.T("Pivot Points");

            // Input
            pnlInput.Parent = this;

            // Output
            pnlOutput.Parent = this;

            // Input Names
            string[] asInputNames = new string[] {
                Language.T("Highest price"),
                Language.T("Closing price"),
                Language.T("Lowest price"),
            };

            int iNumber = 0;
            foreach (string sName in asInputNames)
            {
                alblInputNames[iNumber] = new Label();
                alblInputNames[iNumber].Parent    = pnlInput;
                alblInputNames[iNumber].ForeColor = colorText;
                alblInputNames[iNumber].BackColor = Color.Transparent;
                alblInputNames[iNumber].AutoSize  = true;
                alblInputNames[iNumber].Text      = sName;

                atbxInputValues[iNumber] = new TextBox();
                atbxInputValues[iNumber].Parent = pnlInput;
                atbxInputValues[iNumber].TextChanged += new EventHandler(TbxInput_TextChanged);
                iNumber++;
            }

            // Output Names
            string[] asOutputNames = new string[] {
                Language.T("Resistance") + " 3",
                Language.T("Resistance") + " 2",
                Language.T("Resistance") + " 1",
                Language.T("Pivot Point"),
                Language.T("Support")    + " 1",
                Language.T("Support")    + " 2",
                Language.T("Support")    + " 3",
            };

            iNumber = 0;
            foreach (string sName in asOutputNames)
            {
                alblOutputNames[iNumber] = new Label();
                alblOutputNames[iNumber].Parent    = pnlOutput;
                alblOutputNames[iNumber].ForeColor = colorText;
                alblOutputNames[iNumber].BackColor = Color.Transparent;
                alblOutputNames[iNumber].AutoSize  = true;
                alblOutputNames[iNumber].Text      = sName;

                alblOutputValues[iNumber] = new Label();
                alblOutputValues[iNumber].Parent    = pnlOutput;
                alblOutputValues[iNumber].ForeColor = colorText;
                alblOutputValues[iNumber].BackColor = Color.Transparent;
                alblOutputValues[iNumber].AutoSize  = true;

                iNumber++;
            }

            alblOutputNames[3].Font  = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
            alblOutputValues[3].Font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);

        }

        /// <summary>
        /// Inits the params
        /// </summary>
        private void InitParams()
        {
            atbxInputValues[0].Text = Data.High[Data.Bars - 1].ToString();
            atbxInputValues[1].Text = Data.Close[Data.Bars - 1].ToString();
            atbxInputValues[2].Text = Data.Low[Data.Bars - 1].ToString();
        }

        /// <summary>
        /// Perform initialising
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);


            int buttonWidth = (int)(Data.HorizontalDLU * 60);
            int btnHrzSpace = (int)(Data.HorizontalDLU * 3);

            ClientSize = new Size(270, 307);

            InitParams();
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth = (int)(Data.HorizontalDLU * 60);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace = (int)(Data.HorizontalDLU * 3);
            int border = btnHrzSpace;
            int textHeight = Font.Height;
            int width = 100; // Right side contrlos

            // pnlInput
            pnlInput.Size = new Size(ClientSize.Width - 2 * border, 112);
            pnlInput.Location = new Point(border, border);

            int left = pnlInput.ClientSize.Width - width - btnHrzSpace - 1;

            int shift     = 26;
            int vertSpace = 2;
            int numb      = 0;
            foreach (Label lbl in alblInputNames)
            {
                lbl.Location = new Point(border, numb * buttonHeight + (numb + 1) * vertSpace + shift);
                numb++;
            }

            shift     = 24;
            vertSpace = 2;
            numb      = 0;
            foreach (TextBox lbl in atbxInputValues)
            {
                lbl.Width = width;
                lbl.Location = new Point(left, numb * buttonHeight + (numb + 1) * vertSpace + shift);
                numb++;
            }

            // pnlOutput
            pnlOutput.Size = new Size(ClientSize.Width - 2 * border, 180);
            pnlOutput.Location = new Point(border, pnlInput.Bottom + border);

            shift     = 24;
            vertSpace = -4;
            numb      = 0;
            foreach (Label lbl in alblOutputNames)
            {
                lbl.Location = new Point(border, numb * (buttonHeight + vertSpace) + shift);
                numb++;
            }

            numb = 0;
            foreach (Label lbl in alblOutputValues)
            {
                lbl.Location = new Point(left, numb * (buttonHeight + vertSpace) + shift);
                numb++;
            }

            return;
        }

        /// <summary>
        /// Parses a float value
        /// </summary>
        float ParseInput(string input)
        {
            string dcmlSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            input = input.Replace(".", dcmlSeparator);
            input = input.Replace(",", dcmlSeparator);

            return float.Parse(input);
        }

        /// <summary>
        /// A param has been changed
        /// </summary>
        void TbxInput_TextChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        /// <summary>
        /// Perform calculation
        /// </summary>
        void Calculate()
        {
            foreach (Label lbl in alblOutputValues)
                lbl.Text = "";

            float high;
            float close;
            float low;

            try
            {
                high  = ParseInput(atbxInputValues[0].Text);
                close = ParseInput(atbxInputValues[1].Text);
                low   = ParseInput(atbxInputValues[2].Text);
            }
            catch
            {
                foreach (Label lbl in alblOutputValues)
                    lbl.Text = "";

                return;
            }

            float pivot       = (high + close + low) / 3;
            float resistance1 = 2 * pivot - low;
            float support1    = 2 * pivot - high;
            float resistance2 = pivot + (resistance1 - support1);
            float support2    = pivot - (resistance1 - support1);
            float resistance3 = high  + 2 * (pivot - low);
            float support3    = low   - 2 * (high  - pivot);

            alblOutputValues[0].Text = resistance3.ToString("F4");
            alblOutputValues[1].Text = resistance2.ToString("F4");
            alblOutputValues[2].Text = resistance1.ToString("F4");
            alblOutputValues[3].Text = pivot.ToString("F4");
            alblOutputValues[4].Text = support1.ToString("F4");
            alblOutputValues[5].Text = support2.ToString("F4");
            alblOutputValues[6].Text = support3.ToString("F4");

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
