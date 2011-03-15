// Math Calculator
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    public class Calculator : Form
    {
        TextBox tbxInput;
        Regex   expression;

        bool isDebug = false;
        bool isKeyPhrasesSet = false;

        /// <summary>
        /// Key phrase debug
        /// </summary>
        public bool IsDebug { get { return isDebug; } }

        /// <summary>
        /// Is a key phrase set
        /// </summary>
        public bool IsKeyPhrasesSet { get { return isKeyPhrasesSet; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public Calculator()
        {
            // Test Box Input
            tbxInput = new TextBox();
            tbxInput.Parent   = this;
            tbxInput.Location = Point.Empty;
            tbxInput.Size     = new Size(190, 20);
            tbxInput.KeyUp   += new KeyEventHandler(TbxInput_KeyUp);

            // The Form
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = tbxInput.Size;
            FormBorderStyle     = FormBorderStyle.FixedToolWindow;
            Opacity             = 0.8;
            ShowInTaskbar       = false;
            StartPosition       = FormStartPosition.CenterScreen;
            Text                = Language.T("Calculator") + " (F1 - " + Language.T("Help") + ")";
            TopMost             = true;

            SetPatterns();
        }

        /// <summary>
        /// Sets the patterns
        /// </summary>
        void SetPatterns()
        {
            string patternNumber   = @"(?<numb>\-?\d+([\.,]\d+)?(E[\+\-]\d{1,2})?)";
            string patternOperator = @"(?<operator>[\+\-\*/\^])";
            string patternLast     = @"(?<last>[^\d\.,E])";

            string sOperation = string.Format(
                @"{0}\s*{1}\s*{2}\s*{3}",
                patternNumber.Replace("numb", "arg1"),
                patternOperator,
                patternNumber.Replace("numb", "arg2"),
                patternLast);

            expression = new Regex(sOperation, RegexOptions.Compiled);
        }

        /// <summary>
        /// Catches the hot keys
        /// </summary>
        private void TbxInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                ParseInput(tbxInput.Text + "=");
            else if (e.KeyCode == Keys.Escape)
                tbxInput.Clear();
            else if (e.KeyCode == Keys.F1)
                MessageBox.Show(
                    Language.T("Write the mathematical expression in the box.") + Environment.NewLine +
                    Language.T("Enter the first number, the operator and the second number.") + Environment.NewLine +
                    Language.T("To see the result press a key or continue with the next operation.") + Environment.NewLine + Environment.NewLine +
                    Language.T("Addition")   + ": 12.34 + 8.8 =" + Environment.NewLine +
                    Language.T("Power")      + ": -5.3 ^ 2 ="    + Environment.NewLine + 
                    Language.T("Percent")    + ": 2.2 * 125 %"   + Environment.NewLine + Environment.NewLine +
                    Language.T("Operations") + ": + - * / ^ %"   + Environment.NewLine + Environment.NewLine +
                    Language.T("Hot keys")   + ":" + Environment.NewLine + "   F1 - " + 
                    Language.T("Help")       + Environment.NewLine + "   Esc - " + 
                    Language.T("Clear")      + Environment.NewLine + "   F11 / F12 - " + 
                    Language.T("Opacity")    + Environment.NewLine + "   (Alt + F4) - " + 
                    Language.T("Exit"),
                    Language.T("Calculator Help"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            else if (e.KeyCode == Keys.F11 && Opacity > 0.4f)
                Opacity -= 0.1f;
            else if (e.KeyCode == Keys.F12 && Opacity < 1.0f)
                Opacity += 0.1f;
            else
                ParseInput(tbxInput.Text);
        }

        /// <summary>
        /// Does the job
        /// </summary>
        void ParseInput(string input)
        {
            string dcmlSeparator   = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            input = input.Replace(".", dcmlSeparator);
            input = input.Replace(",", dcmlSeparator);
            Match  match  = expression.Match(input);
            double result = double.NaN;

            if (match.Success)
            {
                double arg1 = double.Parse(match.Groups["arg1"].Value);
                double arg2 = double.Parse(match.Groups["arg2"].Value);
                string optr = match.Groups["operator"].Value;
                string last = match.Groups["last"].Value;

                // Addition
                if (optr == "+")
                    result = arg1 + arg2;

                // Subtraction
                else if (optr == "-")
                    result = arg1 - arg2;

                // Multiplication
                else if (optr == "*" && last != "%")
                    result = arg1 * arg2;

                // Division
                else if (optr == @"/")
                {
                    if (arg2 != 0) result = arg1 / arg2;
                    else if (arg1 > 0) result = double.PositiveInfinity;
                    else if (arg1 < 0) result = double.NegativeInfinity;
                }

                // Percent
                else if (optr == "*" && last == "%")
                {
                    result = arg1 * arg2 / 100;
                    last = "=";
                }

                // Power
                else if (optr == "^")
                    result = Math.Pow(arg1, arg2);

                if (Regex.IsMatch(last, @"[\+\-\*/\^]"))
                    last = " " + last + " ";
                else
                    last = " ";
                
                tbxInput.Clear();
                tbxInput.AppendText(result.ToString() + last);
            }

            return;
        }
    }
}
