// Date Filter Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Date Filter Indicator
    /// </summary>
    public class Date_Filter : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Date_Filter(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Date Filter";
            PossibleSlots = SlotTypes.OpenFilter;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.DateTime;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "Do not open positions before",
                "Do not open positions after"
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of the date filter.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Year";
            IndParam.NumParam[0].Value   = 2000;
            IndParam.NumParam[0].Min     = 1900;
            IndParam.NumParam[0].Max     = 2100;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The year.";

            IndParam.NumParam[1].Caption = "Month";
            IndParam.NumParam[1].Value   = 1;
            IndParam.NumParam[1].Min     = 1;
            IndParam.NumParam[1].Max     = 12;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "The month.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Reading the parameters
            int iYear  = (int)IndParam.NumParam[0].Value;
            int iMonth = (int)IndParam.NumParam[1].Value;
            DateTime dtKeyDate = new DateTime(iYear, iMonth, 1);

            // Calculation
            int iFirstBar = 0;
            double[] adBars = new double[Bars];

            // Calculation of the logic.
            switch (IndParam.ListParam[0].Text)
            {
                case "Do not open positions after":
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                        if (Time[iBar] < dtKeyDate)
                            adBars[iBar] = 1;

                    break;

                case "Do not open positions before":
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        if (Time[iBar] >= dtKeyDate)
                        {
                            iFirstBar = iBar;
                            break;
                        }
                    }

                    iFirstBar = (int)Math.Min(iFirstBar, Bars - Configs.MIN_BARS);

                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        adBars[iBar] = 1;
                    }

                    break;

                default:
                    break;
            }

            // Saving the components
            Component = new IndicatorComp[2];

            Component[0] = new IndicatorComp();
            Component[0].CompName      = "Allow Open Long";
            Component[0].DataType      = IndComponentType.AllowOpenLong;
            Component[0].ChartType     = IndChartType.NoChart;
            Component[0].ShowInDynInfo = false;
            Component[0].FirstBar      = iFirstBar;
            Component[0].Value         = adBars;

            Component[1] = new IndicatorComp();
            Component[1].CompName      = "Allow Open Short";
            Component[1].DataType      = IndComponentType.AllowOpenShort;
            Component[1].ChartType     = IndChartType.NoChart;
            Component[1].ShowInDynInfo = false;
            Component[1].FirstBar      = iFirstBar;
            Component[1].Value         = adBars;

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            int iYear  = (int)IndParam.NumParam[0].Value;
            int iMonth = (int)IndParam.NumParam[1].Value;
            DateTime dtKeyDate = new DateTime(iYear, iMonth, 1);

            EntryFilterLongDescription  = "(a back tester limitation) Do not open positions ";
            EntryFilterShortDescription = "(a back tester limitation) Do not open positions ";

            switch (IndParam.ListParam[0].Text)
            {
                case "Do not open positions before":
                    EntryFilterLongDescription  += "before " + dtKeyDate.ToShortDateString();
                    EntryFilterShortDescription += "before " + dtKeyDate.ToShortDateString();
                    break;

                case "Do not open positions after":
                    EntryFilterLongDescription  += "after " + dtKeyDate.ToShortDateString();
                    EntryFilterShortDescription += "after " + dtKeyDate.ToShortDateString();
                    break;

                default:
                    break;
            }

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            string sString = IndicatorName;

            return sString;
        }
    }
}
