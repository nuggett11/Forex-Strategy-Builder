// Numeric parameters
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Describes a parameter represented by means of a NumericUpDown control.
    /// </summary>
    public class NumericParam
    {
        private string sCaption;
        private double dValue;
        private double dMin;
        private double dMax;
        private int    iPoint;
        private bool   bEnabled;
        private string sTip;

        /// <summary>
        /// Gets or sets the text describing the parameter.
        /// </summary>
        public string Caption { get { return sCaption; } set { sCaption = value; } }

        /// <summary>
        /// Gets or sets the value of parameter.
        /// </summary>
        public double Value { get { return dValue; } set { dValue = value; } }

        /// <summary>
        /// Gets the value of parameter as a string.
        /// </summary>
        public string ValueToString { get { return String.Format("{0:F" + iPoint.ToString() + "}", dValue); } }

        /// <summary>
        /// Gets the corrected value of parameter as a string.
        /// </summary>
        public string AnotherValueToString(double dAnotherValue)
        {
            return String.Format("{0:F" + iPoint.ToString() + "}", dAnotherValue);
        }

        /// <summary>
        /// Gets or sets the minimum value of parameter.
        /// </summary>
        public double Min { get { return dMin; } set { dMin = value; } }

        /// <summary>
        /// Gets or sets the maximum value of parameter.
        /// </summary>
        public double Max { get { return dMax; } set { dMax = value; } }

        /// <summary>
        /// Gets or sets the number of meaning decimal points of parameter.
        /// </summary>
        public int Point { get { return iPoint; } set { iPoint = value; } }

        /// <summary>
        /// Gets or sets the value indicating whether the control can respond to user interaction.
        /// </summary>
        public bool Enabled { get { return bEnabled; } set { bEnabled = value; } }

        /// <summary>
        /// Gets or sets the text of tool tip asociated with this control.
        /// </summary>
        public string ToolTip { get { return sTip; } set { sTip = value; } }

        /// <summary>
        /// The default constructor.
        /// </summary>
        public NumericParam()
        {
            sCaption = String.Empty;
            dValue   = 0;
            dMin     = 0;
            dMax     = 100;
            iPoint   = 0;
            bEnabled = false;
            sTip     = String.Empty;
        }

        /// <summary>
        /// Returns a copy
        /// </summary>
        public NumericParam Clone()
        {
            NumericParam nparam = new NumericParam();

            nparam.sCaption = sCaption;
            nparam.dValue   = dValue;
            nparam.dMin     = dMin;
            nparam.dMax     = dMax;
            nparam.iPoint   = iPoint;
            nparam.bEnabled = bEnabled;
            nparam.sTip     = sTip;

            return nparam;
        }
    }
}