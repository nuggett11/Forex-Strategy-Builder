// NUD class
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
    /// New NumericUpDown
    /// </summary>
    public class NUD : NumericUpDown
    {
        Timer timer;
        Color foreColor;
        Color errorColor;

        public NUD()
        {
            timer = new Timer();
            timer.Tick += new EventHandler(Timer_Tick);

            foreColor  = ForeColor;
            errorColor = Color.Red;
        }

        protected override void OnValueChanged(EventArgs e)
        {
            if (ForeColor != foreColor)
                ForeColor = foreColor;

            timer.Tag = Value;

            base.OnValueChanged(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            decimal value;
            if (decimal.TryParse(Text, out value))
            {
                if (Minimum <= value && value <= Maximum)
                {
                    SetValue(value);

                    if (ForeColor != foreColor)
                        ForeColor = foreColor;
                }
                else
                {
                    if (timer.Enabled)
                        timer.Stop();

                    if (ForeColor != errorColor)
                        ForeColor = errorColor;
                }
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.Delta > 0)
                ChangeValue(Increment);
            else
                ChangeValue(-Increment);
        }

        void SetValue(decimal value)
        {
            if (timer.Enabled)
                timer.Stop();

            timer.Tag = value;
            timer.Interval = 500;

            timer.Start();
        }

        void ChangeValue(decimal change)
        {
            if (timer.Enabled)
                timer.Stop();

            decimal oldValue = (timer.Tag == null) ? Value : (decimal)timer.Tag;
            decimal newValue = oldValue + change;

            timer.Tag = newValue;
            if (newValue > Maximum)
                timer.Tag = Maximum;
            if (newValue < Minimum)
                timer.Tag = Minimum;

            timer.Interval = 50;
            timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (timer.Enabled)
                timer.Stop();

            decimal value = (decimal)timer.Tag;
            if (value > Maximum)
                value = Maximum;
            if (value < Minimum)
                value = Minimum;

            Value = value;
        }
    }
}
