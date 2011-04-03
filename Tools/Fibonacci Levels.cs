// Fibonacci Levels Calculator
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    class Fibonacci_Levels_Calculator : Form
    {
        Fancy_Panel pnlInput;
        Fancy_Panel pnlOutput;

        Label[]   alblInputNames;
        TextBox[] atbxInputValues;
        Label[]   alblOutputNames;
        Label[]   alblOutputValues;

        Font font;
        Color colorText;

        float[] afLevels = new float[] { 0.0f, 23.6f, 38.2f, 50.0f, 61.8f, 76.4f, 100.0f, 138.2f, 161.8f, 261.8f };

        /// <summary>
        /// Constructor
        /// </summary>
        public Fibonacci_Levels_Calculator()
        {
            pnlInput  = new Fancy_Panel(Language.T("Input Values"));
            pnlOutput = new Fancy_Panel(Language.T("Output Values"));

            alblInputNames   = new Label[2];
            atbxInputValues  = new TextBox[2];
            alblOutputNames  = new Label[10];
            alblOutputValues = new Label[10];

            font      = this.Font;
            colorText = LayoutColors.ColorControlText;

            MaximizeBox     = false;
            MinimizeBox     = false;
            Icon            = Data.Icon;
            BackColor       = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Text            = Language.T("Fibonacci Levels");

            // Input
            pnlInput.Parent= this;

            // Output
            pnlOutput.Parent= this;

            // Input Names
            string[] asInputNames = new string[] {
                Language.T("First price"),
                Language.T("Second price"),
            };

            int number = 0;
            foreach (string sName in asInputNames)
            {
                alblInputNames[number] = new Label();
                alblInputNames[number].Parent    = pnlInput;
                alblInputNames[number].ForeColor = colorText;
                alblInputNames[number].BackColor = Color.Transparent;
                alblInputNames[number].AutoSize  = true;
                alblInputNames[number].Text      = sName;

                atbxInputValues[number] = new TextBox();
                atbxInputValues[number].Parent = pnlInput;
                atbxInputValues[number].TextChanged += new EventHandler(TbxInput_TextChanged);
                number++;
            }

            // Output Names

            number = 0;
            foreach (float fn in afLevels)
            {
                alblOutputNames[number] = new Label();
                alblOutputNames[number].Parent    = pnlOutput;
                alblOutputNames[number].ForeColor = colorText;
                alblOutputNames[number].BackColor = Color.Transparent;
                alblOutputNames[number].AutoSize  = true;
                alblOutputNames[number].Text      = fn.ToString("F1") + " %";

                alblOutputValues[number] = new Label();
                alblOutputValues[number].Parent    = pnlOutput;
                alblOutputValues[number].ForeColor = colorText;
                alblOutputValues[number].BackColor = Color.Transparent;
                alblOutputValues[number].AutoSize  = true;

                number++;
            }

            alblOutputNames[2].Font  = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
            alblOutputNames[3].Font  = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
            alblOutputNames[4].Font  = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
            alblOutputValues[2].Font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
            alblOutputValues[3].Font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
            alblOutputValues[4].Font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
        }

        /// <summary>
        /// Initial parameters
        /// </summary>
        private void InitParams()
        {
            Fibonacci fibo = new Fibonacci(SlotTypes.Close);
            fibo.Calculate(SlotTypes.Close);
            atbxInputValues[0].Text = fibo.Component[5].Value[Data.Bars -1].ToString();
            atbxInputValues[1].Text = fibo.Component[1].Value[Data.Bars -1].ToString();
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);


            int buttonWidth = (int)(Data.HorizontalDLU * 60);
            int btnHrzSpace = (int)(Data.HorizontalDLU * 3);

            ClientSize = new Size(270, 347);

            InitParams();
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
            int border     = btnHrzSpace;
            int textHeight = Font.Height;
            int width      = 100; // Right side controls

            // pnlInput
            pnlInput.Size = new Size(ClientSize.Width - 2 * border, 85);
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
            numb = 0;
            foreach (TextBox lbl in atbxInputValues)
            {
                lbl.Width = width;
                lbl.Location = new Point(left, numb * buttonHeight + (numb + 1) * vertSpace + shift);
                numb++;
            }

            // pnlOutput
            pnlOutput.Size = new Size(ClientSize.Width - 2 * border, 245);
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
        /// Parses a float number
        /// </summary>
        float ParseInput(string input)
        {
            string dcmlSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            input = input.Replace(".", dcmlSeparator);
            input = input.Replace(",", dcmlSeparator);

            return float.Parse(input);
        }

        /// <summary>
        /// Input parameter changed
        /// </summary>
        void TbxInput_TextChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        /// <summary>
        /// Calculates the result
        /// </summary>
        void Calculate()
        {
            foreach (Label lbl in alblOutputValues)
                lbl.Text = "";

            float price1;
            float price2;

            try
            {
                price1 = ParseInput(atbxInputValues[0].Text);
                price2 = ParseInput(atbxInputValues[1].Text);
            }
            catch
            {
                foreach (Label lbl in alblOutputValues)
                    lbl.Text = "";

                return;
            }

            if (price1 > price2)
            {
                for (int i = afLevels.Length - 1; i >= 0; i--)
                    alblOutputValues[i].Text = ((price1 - price2) * afLevels[i] / 100 + price2).ToString("F4");
            }
            else if (price1 < price2)
            {
                for (int i = 0; i < afLevels.Length; i++)
                alblOutputValues[i].Text = (price2 - (price2 - price1) * afLevels[i] / 100).ToString("F4");
            }
            else
                foreach (Label lbl in alblOutputValues)
                    lbl.Text = "";

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
