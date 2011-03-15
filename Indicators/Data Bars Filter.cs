// Data Bars Filter Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Data Bars Filter Indicator
    /// </summary>
    public class Data_Bars_Filter : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Data_Bars_Filter(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Data Bars Filter";
            PossibleSlots = SlotTypes.OpenFilter;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.Additional;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "Do not use the newest bars",
                "Do not use the oldest bars",
                "Do not use the newest bars and oldest bars",
                "Use the newest bars only",
                "Use the oldest bars only",
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Specify the entry bars.";

            // The NumericUpDown parameters.
            IndParam.NumParam[0].Caption = "Newest bars";
            IndParam.NumParam[0].Value   = 1000;
            IndParam.NumParam[0].Min     = 0;
            IndParam.NumParam[0].Max     = Configs.MAX_BARS;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The number of newest bars.";

            IndParam.NumParam[1].Caption = "Oldest bars";
            IndParam.NumParam[1].Value   = 0;
            IndParam.NumParam[1].Min     = 0;
            IndParam.NumParam[1].Max     = Configs.MAX_BARS;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "The number of oldest bars.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Reading the parameters
            int iNewest = (int)IndParam.NumParam[0].Value;
            int iOldest = (int)IndParam.NumParam[1].Value;

            // Calculation
            int iFirstBar = 0;
            double[] adBars = new double[Bars];

            // Calculation of the logic
            switch (IndParam.ListParam[0].Text)
            {
                case "Do not use the newest bars":
                    for (int iBar = iFirstBar; iBar < Bars - iNewest; iBar++)
                    {
                        adBars[iBar] = 1;
                    }

                    break;

                case "Do not use the oldest bars":
                    iFirstBar = Math.Min(iOldest, Bars - Configs.MIN_BARS);

                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        adBars[iBar] = 1;
                    }

                    break;

                case "Do not use the newest bars and oldest bars":
                    iFirstBar = Math.Min(iOldest, Bars - Configs.MIN_BARS);
                    int iLastBar = Math.Max(iFirstBar + Configs.MIN_BARS, Bars - iNewest);

                    for (int iBar = iFirstBar; iBar < iLastBar; iBar++)
                    {
                        adBars[iBar] = 1;
                    }

                    break;

                case "Use the newest bars only":
                    iFirstBar = Math.Max(0, Bars - iNewest);
                    iFirstBar = Math.Min(iFirstBar, Bars - Configs.MIN_BARS);

                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        adBars[iBar] = 1;
                    }

                    break;

                case "Use the oldest bars only":
                    iOldest = Math.Max(Configs.MIN_BARS, iOldest);

                    for (int iBar = iFirstBar; iBar < iOldest; iBar++)
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
            Component[0].CompName      = "(No) Used bars";
            Component[0].DataType      = IndComponentType.AllowOpenLong;
            Component[0].ChartType     = IndChartType.NoChart;
            Component[0].ShowInDynInfo = false;
            Component[0].FirstBar      = iFirstBar;
            Component[0].Value         = adBars;

            Component[1] = new IndicatorComp();
            Component[1].CompName      = "(No) Used bars";
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
            int iNewest = (int)IndParam.NumParam[0].Value;
            int iOldest = (int)IndParam.NumParam[1].Value;

            EntryFilterLongDescription  = "(a back tester limitation) ";
            EntryFilterShortDescription = "(a back tester limitation) ";

            switch (IndParam.ListParam[0].Text)
            {
                case "Do not use the newest bars":
                    EntryFilterLongDescription  += "Do not use the newest " + iNewest + " bars";
                    EntryFilterShortDescription += "Do not use the newest " + iNewest + " bars";
                    break;

                case "Do not use the oldest bars":
                    EntryFilterLongDescription  += "Do not use the oldest " + iOldest + " bars";
                    EntryFilterShortDescription += "Do not use the oldest " + iOldest + " bars";
                    break;

                case "Do not use the newest bars and oldest bars":
                    EntryFilterLongDescription  += "Do not use the newest " + iNewest + " bars and oldest " + iOldest + " bars";
                    EntryFilterShortDescription += "Do not use the newest " + iNewest + " bars and oldest " + iOldest + " bars";
                    break;

                case "Use the newest bars only":
                    EntryFilterLongDescription  += "Use the newest " + iNewest + " bars only";
                    EntryFilterShortDescription += "Use the newest " + iNewest + " bars only";
                    break;

                case "Use the oldest bars only":
                    EntryFilterLongDescription  += "Use the oldest " + iNewest + " bars only";
                    EntryFilterShortDescription += "Use the oldest " + iNewest + " bars only";
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
