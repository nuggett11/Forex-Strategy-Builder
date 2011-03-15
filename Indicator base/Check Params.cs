// Check parameters
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Describes a parameter who can be used or not.
    /// </summary>
    public class CheckParam
    {
        private string sCaption;
        private bool   bChecked;
        private bool   bEnabled;
        private string sToolTip;

        /// <summary>
        /// Gets or sets the text describing the parameter.
        /// </summary>
        public string Caption { get { return sCaption; } set { sCaption = value; } }

        /// <summary>
        /// Gets or sets the value indicating whether the control is checked.
        /// </summary>
        public bool Checked { get { return bChecked; } set { bChecked = value; } }

        /// <summary>
        /// Gets or sets the value indicating whether the control can respond to user interaction.
        /// </summary>
        public bool Enabled { get { return bEnabled; } set { bEnabled = value; } }

        /// <summary>
        /// Gets or sets the text of tool tip asociated with this control.
        /// </summary>
        public string ToolTip { get { return sToolTip; } set { sToolTip = value; } }

        /// <summary>
        /// The default constructor.
        /// </summary>
        public CheckParam()
        {
            sCaption = String.Empty;
            bEnabled = false;
            bChecked = false;
            sToolTip = String.Empty;
        }

        /// <summary>
        /// Returns a copy of the class.
        /// </summary>
        public CheckParam Clone()
        {
            CheckParam cparam = new CheckParam();

            cparam.sCaption = sCaption;
            cparam.bEnabled = bEnabled;
            cparam.bChecked = bChecked;
            cparam.sToolTip = sToolTip;

            return cparam;
        }
    }
}