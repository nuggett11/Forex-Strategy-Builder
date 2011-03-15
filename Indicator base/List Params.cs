// List parameters
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Describes a parameter that has to be selected from a list.
    /// </summary>
    public class ListParam
    {
        private string   sCaption;
        private string[] asItemList;
        private string   sText;
        private int      iIndex;
        private bool     bEnabled;
        private string   sToolTip;

        /// <summary>
        /// Gets or sets the text describing the parameter.
        /// </summary>
        public string Caption { get { return sCaption; } set { sCaption = value; } }

        /// <summary>
        /// Gets or sets the list of parameter values.
        /// </summary>
        public string[] ItemList { get { return asItemList; } set { asItemList = value; } }

        /// <summary>
        /// Gets or sets the text associated whit this parameter.
        /// </summary>
        public string Text { get { return sText; } set { sText = value; } }

        /// <summary>
        /// Gets or sets the index specifying the currently selected item.
        /// </summary>
        public int Index { get { return iIndex; } set { iIndex = value; } }

        /// <summary>
        /// Gets or sets the value indicating whether the control can respond to user interaction.
        /// </summary>
        public bool Enabled { get { return bEnabled; } set { bEnabled = value; } }

        /// <summary>
        /// Gets or sets the text of tool tip associated with this control.
        /// </summary>
        public string ToolTip { get { return sToolTip; } set { sToolTip = value; } }

        /// <summary>
        /// Zeroing the parameters.
        /// </summary>
        public ListParam()
        {
            sCaption   = String.Empty;
            asItemList = new string[] { "" };
            iIndex     = 0;
            sText      = String.Empty;
            bEnabled   = false;
            sToolTip   = String.Empty;
        }

        /// <summary>
        /// Returns a copy
        /// </summary>
        public ListParam Clone()
        {
            ListParam lparam = new ListParam();

            lparam.sCaption   = sCaption;
            lparam.asItemList = new string[asItemList.Length];
            asItemList.CopyTo(lparam.asItemList, 0);
            lparam.iIndex     = iIndex;
            lparam.sText      = sText;
            lparam.bEnabled   = bEnabled;
            lparam.sToolTip   = sToolTip;

            return lparam;
        }
    }
}