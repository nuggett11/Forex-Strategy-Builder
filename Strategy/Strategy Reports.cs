// Strategy Reports
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Text;

namespace Forex_Strategy_Builder
{
    public partial class Strategy
    {
        /// <summary>
        /// Saves the strategy in BBCode format.
        /// </summary>
        public string GenerateBBCode()
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramRC)
                stage = " " + "RC";

            string strBBCode = "";
            string nl = Environment.NewLine;
            string nl2 = Environment.NewLine + Environment.NewLine;

            strBBCode += "[b]" + Data.ProgramName + " v" + Data.ProgramVersion + stage + "[/b]" + nl;
            strBBCode += "Strategy name: [b]" + strategyName + "[/b]" + nl;
            strBBCode += "Exported on: " + DateTime.Now + nl;
            strBBCode += nl;

            // Description
            strBBCode += "[b]Description[/b]" + nl;

            if (Description != "")
            {
                if (!Data.IsStrDescriptionRelevant())
                    strBBCode += "(This description might be outdated!)" + nl2;

                strBBCode += Description + nl2;
            }
            else
                strBBCode += "   None." + nl2;

            strBBCode += "Market: " + Data.Symbol + " " + Data.PeriodString + nl;
            strBBCode += "Spread in pips: " + Data.InstrProperties.Spread + nl;
            strBBCode += "Swap Long in " +
                (Data.InstrProperties.SwapType == Commission_Type.money ? Data.InstrProperties.PriceIn : Data.InstrProperties.SwapType.ToString()) + ": " +
                Data.InstrProperties.SwapLong.ToString()  + nl;
            strBBCode += "Swap Short in " + 
                (Data.InstrProperties.SwapType == Commission_Type.money ? Data.InstrProperties.PriceIn : Data.InstrProperties.SwapType.ToString()) + ": " +
                Data.InstrProperties.SwapShort.ToString() + nl;
            strBBCode += "Commission per " +
                Data.InstrProperties.CommissionScope.ToString() + " at " +
                (Data.InstrProperties.CommissionTime == Commission_Time.open ? "opening" : "opening and closing") + " in " +
                (Data.InstrProperties.CommissionType == Commission_Type.money ? Data.InstrProperties.PriceIn : Data.InstrProperties.CommissionType.ToString()) + ": " +
                Data.InstrProperties.Commission.ToString() + nl;
            strBBCode += "Slippage in pips: " + Data.InstrProperties.Slippage + nl2;

            strBBCode += UseAccountPercentEntry ? "Use account % for margin round to whole lots" + nl : "";
            string tradingUnit = UseAccountPercentEntry ? "% of the account for margin" : "";
            strBBCode += "Maximum open lots: " + MaxOpenLots + nl;
            strBBCode += "Entry lots: "    + EntryLots    + tradingUnit + nl;
            strBBCode += "Adding lots: "   + AddingLots   + tradingUnit + nl;
            strBBCode += "Reducing lots: " + ReducingLots + tradingUnit + nl;
            strBBCode += nl;
            strBBCode += "Intrabar scanning: "    + (Backtester.IsScanPerformed ? "Accomplished" : "Not accomplished") + nl;
            strBBCode += "Interpolation method: " + Backtester.InterpolationMethodToString() + nl;
            strBBCode += "Ambiguous bars: "       + Backtester.AmbiguousBars          + nl;
            strBBCode += "Tested bars: "          + (Backtester.Bars - Data.FirstBar) + nl;
            strBBCode += "Balance: [b]"           + Backtester.NetBalance  + " pips (" + Backtester.NetMoneyBalance.ToString("F2")  + " " + Data.InstrProperties.PriceIn + ")[/b]" + nl;
            strBBCode += "Minimum account: "      + Backtester.MinBalance  + " pips (" + Backtester.MinMoneyBalance.ToString("F2")  + " " + Data.InstrProperties.PriceIn + ")"     + nl;
            strBBCode += "Maximum drawdown: "     + Backtester.MaxDrawdown + " pips (" + Backtester.MaxMoneyDrawdown.ToString("F2") + " " + Data.InstrProperties.PriceIn + ")"     + nl;
            strBBCode += "Time in position: "     + Backtester.TimeInPosition + " %" + nl;
            strBBCode += nl;

            if (SameSignalAction == SameDirSignalAction.Add)
                strBBCode += "[b]A same direction signal[/b] - [i]Adds to the position[/i]" + nl;
            else if (SameSignalAction == SameDirSignalAction.Winner)
                strBBCode += "[b]A same direction signal[/b] - [i]Adds to a winnig position[/i]" + nl;
            else if (SameSignalAction == SameDirSignalAction.Nothing)
                strBBCode += "[b]A same direction signal[/b] - [i]Does nothing[/i]" + nl;

            if (OppSignalAction == OppositeDirSignalAction.Close)
                strBBCode += "[b]An opposite direction signal[/b] - [i]Closes the position[/i]" + nl;
            else if (OppSignalAction == OppositeDirSignalAction.Reduce)
                strBBCode += "[b]An opposite direction signal[/b] - [i]Reduces the position[/i]" + nl;
            else if (OppSignalAction == OppositeDirSignalAction.Reverse)
                strBBCode += "[b]An opposite direction signal[/b] - [i]Reverses the position[/i]" + nl;
            else
                strBBCode += "[b]An opposite direction signal[/b] - [i]Does nothing[/i]" + nl;

            strBBCode += "[b]Permanent Stop Loss[/b] - [i]"   + (Data.Strategy.UsePermanentSL ? (Data.Strategy.PermanentSLType == PermanentProtectionType.Absolute ? "(Abs) " : "") + Data.Strategy.PermanentSL.ToString() : "None") + "[/i]" + nl;
            strBBCode += "[b]Permanent Take Profit[/b] - [i]" + (Data.Strategy.UsePermanentTP ? (Data.Strategy.PermanentTPType == PermanentProtectionType.Absolute ? "(Abs) " : "") + Data.Strategy.PermanentTP.ToString() : "None") + "[/i]" + nl;
            //strBBCode += "[b]Break Even[/b] - [i]"            + (Data.Strategy.UseBreakEven   ? Data.Strategy.BreakEven.ToString()   : "None") + "[/i]" + nl;
            strBBCode += nl;

            // Add the slots.
            foreach(IndicatorSlot indSlot in this.indicatorSlot)
            {
                string slotTypeName;
                switch (indSlot.SlotType)
                {
                    case SlotTypes.Open:
                        slotTypeName = "Opening Point of the Position";
                        break;
                    case SlotTypes.OpenFilter:
                        slotTypeName = "Opening Logic Condition";
                        break;
                    case SlotTypes.Close:
                        slotTypeName = "Closing Point of the Position";
                        break;
                    case SlotTypes.CloseFilter:
                        slotTypeName = "Closing Logic Condition";
                        break;
                    default:
                        slotTypeName = "";
                        break;
                }

                strBBCode += "[b][" + slotTypeName + "][/b]" + nl;
                strBBCode += "[b][color=blue]" + indSlot.IndicatorName + "[/color][/b]" + nl;

                // Add the list params.
                foreach (ListParam listParam in indSlot.IndParam.ListParam)
                    if (listParam.Enabled)
                    {
                        if (listParam.Caption == "Logic")
                            strBBCode += "     [b][color=teal]" +
                                (Configs.UseLogicalGroups && (indSlot.SlotType == SlotTypes.OpenFilter || indSlot.SlotType == SlotTypes.CloseFilter) ?
                                "[" + (indSlot.LogicalGroup.Length == 1 ? " " + indSlot.LogicalGroup + " " : indSlot.LogicalGroup) + "]   " : "") + listParam.Text + "[/color][/b]" + nl;
                        else
                            strBBCode += "     [b]" + listParam.Caption + "[/b]  -  [i]" + listParam.Text + "[/i]" + nl;
                    }

                // Add the num params.
                foreach(NumericParam numParam in indSlot.IndParam.NumParam)
                    if (numParam.Enabled)
                        strBBCode += "     [b]" + numParam.Caption + "[/b]  -  [i]" + numParam.ValueToString + "[/i]" + nl;

                // Add the check params.
                foreach(CheckParam checkParam in indSlot.IndParam.CheckParam)
                    if (checkParam.Enabled)
                        strBBCode += "     [b]" + checkParam.Caption + "[/b]  -  [i]" + (checkParam.Checked ? "Yes" : "No") + "[/i]" + nl;

                strBBCode += nl;
            }

            return strBBCode;
        }

        /// <summary>
        /// Generate Overview in HTML code
        /// </summary>
        /// <returns>the HTML code</returns>
        public string GenerateHTMLOverview()
        {
            StringBuilder sb = new StringBuilder();
            // Header
            sb.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");
            sb.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\">");
            sb.AppendLine("<head><meta http-equiv=\"content-type\" content=\"text/html;charset=utf-8\" />");
            sb.AppendLine("<title>" + strategyName + "</title>");
            sb.AppendLine("<style type=\"text/css\">");
            sb.AppendLine("body {padding: 0 10px 10px 10px; margin: 0px; background-color: #fff; color: #003; font-size: 16px}");
            sb.AppendLine(".content h1 {font-size: 1.4em;}");
            sb.AppendLine(".content h2 {font-size: 1.2em;}");
            sb.AppendLine(".content h3 {font-size: 1em;}");
            sb.AppendLine(".content p { }");
            sb.AppendLine(".content p.fsb_go_top {text-align: center;}");
            sb.AppendLine(".fsb_strategy_slot {font-family: sans-serif; width: 30em; margin: 2px auto; text-align: center; background-color: #f3ffff; }");
            sb.AppendLine(".fsb_strategy_slot table tr td {text-align: left; color: #033; font-size: 75%;}");
            sb.AppendLine(".fsb_properties_slot {color: #fff; padding: 2px 0px; background: #966; }");
            sb.AppendLine(".fsb_open_slot {color: #fff; padding: 2px 0; background: #693; }");
            sb.AppendLine(".fsb_close_slot {color: #fff; padding: 2px 0; background: #d63; }");
            sb.AppendLine(".fsb_open_filter_slot {color: #fff; padding: 2px 0; background: #699;}");
            sb.AppendLine(".fsb_close_filter_slot {color: #fff; padding: 2px 0; background: #d99;}");
            sb.AppendLine(".fsb_str_indicator {padding: 5px 0; color: #6090c0;}");
            sb.AppendLine(".fsb_str_logic {padding-bottom: 5px; color: #066;}");
            sb.AppendLine(".fsb_table {margin: 0 auto; border: 2px solid #003; border-collapse: collapse;}");
            sb.AppendLine(".fsb_table th {border: 1px solid #006; text-align: center; background: #ccf; border-bottom-width: 2px;}");
            sb.AppendLine(".fsb_table td {border: 1px solid #006;}");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class=\"content\" id=\"fsb_header\">");

            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramRC)
                stage = " " + "RC";

            sb.AppendLine("<h1>" + Language.T("Strategy Overview") + "</h1>");
            sb.AppendLine("<p>");
            sb.AppendLine("\t<strong>" + Data.ProgramName + " v" + Data.ProgramVersion + stage + "</strong><br />");
            sb.AppendLine("\t" + Language.T("Strategy name") + ": <strong>" + strategyName + "</strong><br />");
            sb.AppendLine("\t" + Language.T("Date") + ": " + DateTime.Now);
            sb.AppendLine("</p>");

            // Contents
            sb.AppendLine("<h2 id=\"contents\">" + Language.T("Table of Contents") + "</h2>");
            sb.AppendLine("<a href=\"#description\">" + Language.T("Description")           + "</a><br />");
            sb.AppendLine("<a href=\"#logic\">"       + Language.T("Logic")                 + "</a><br />");
            sb.AppendLine("<a href=\"#environment\">" + Language.T("Environment")           + "</a><br />");
            sb.AppendLine("<a href=\"#properties\">"  + Language.T("Strategy Properties")   + "</a><br />");
            sb.AppendLine("<a href=\"#indicators\">"  + Language.T("Indicator Slots")       + "</a><br />");

            if (Configs.AdditionalStatistics)
            {
                sb.AppendLine("<a href=\"#statistics\">" + Language.T("Statistic Information") + "</a><br />");
                sb.AppendLine("<a href=\"#addstats\">" + Language.T("Additional Statistics") + "</a>");
            }
            else
            {
                sb.AppendLine("<a href=\"#statistics\">" + Language.T("Statistic Information") + "</a>");
            }

            // Description
            sb.AppendLine("<h2 id=\"description\">" + Language.T("Description") + "</h2>");
            if (Description != String.Empty)
            {
                string strStrategyDescription = Description;
                strStrategyDescription = Description.Replace(Environment.NewLine, "<br />");
                strStrategyDescription = strStrategyDescription.Replace("&", "&amp;");
                strStrategyDescription = strStrategyDescription.Replace("\"", "&quot;");

                if(!Data.IsStrDescriptionRelevant())
                    sb.AppendLine("<p style=\"color: #a00\">" + "(" + Language.T("This description might be outdated!") + ")" + "</p>");

                sb.AppendLine("<p>" + strStrategyDescription + "</p>");

                sb.AppendLine("<p class=\"fsb_go_top\"><a href=\"#fsb_header\" title=\"" + Language.T("Go to the beginning.") + "\">" + Language.T("Top") + "</a></p>");
            }
            else
                sb.AppendLine("<p>" + Language.T("None") + ".</p>");

            // Strategy Logic
            sb.AppendLine();
            sb.AppendLine("<h2 id=\"logic\">" + Language.T("Logic") + "</h2>");

            // Opening
            sb.AppendLine("<h3>" + Language.T("Opening (Entry Signal)") + "</h3>");
            sb.AppendLine(OpeningLogicHTMLReport().ToString());

            // Closing
            sb.AppendLine("<h3>" + Language.T("Closing (Exit Signal)") + "</h3>");
            sb.AppendLine(ClosingLogicHTMLReport().ToString());

            // Averaging
            sb.AppendLine("<h3>" + Language.T("Handling of Additional Entry Signals") + "**</h3>");
            sb.AppendLine(AveragingHTMLReport().ToString());

            // Trading Sizes
            sb.AppendLine("<h3>" + Language.T("Trading Size") + "</h3>");
            sb.AppendLine(TradingSizeHTMLReport().ToString());

            // Protection
            sb.AppendLine("<h3>" + Language.T("Permanent Protection") + "</h3>");
            if (!Data.Strategy.UsePermanentSL)
            {
                sb.AppendLine("<p>" + Language.T("The strategy does not provide a permanent loss limitation.") + "</p>");
            }
            else
            {
                sb.AppendLine("<p>" + Language.T("The Permanent Stop Loss limits the loss of a position to") + (Data.Strategy.PermanentSLType == PermanentProtectionType.Absolute ? " (Abs) " : " ") + Data.Strategy.PermanentSL);
                sb.AppendLine(Language.T("pips per open lot (plus the charged spread and rollover).") + "</p>");
            }

            if (!Data.Strategy.UsePermanentTP)
            {
                sb.AppendLine("<p>" + Language.T("The strategy does not use a Permanent Take Profit.") + "</p>");
            }
            else
            {
                sb.AppendLine("<p>" + Language.T("The Permanent Take Profit closes a position at") + (Data.Strategy.PermanentTPType == PermanentProtectionType.Absolute ? " (Abs) " : " ") + Data.Strategy.PermanentTP);
                sb.AppendLine(Language.T("pips profit.") + "</p>");
            }

            if (Data.Strategy.UseBreakEven)
            {
                sb.AppendLine("<p>" + Language.T("The position's Stop Loss will be set to Break Even price when the profit reaches") + " " + Data.Strategy.BreakEven);
                sb.AppendLine(Language.T("pips") + "." + "</p>");
            }
            
            sb.AppendLine("<p>--------------<br />");
            sb.AppendLine("* " + Language.T("Use the indicator value from the previous bar for all asterisk-marked indicators!") + "<br />");
            sb.AppendLine("** " + Language.T("The averaging rules apply to the entry signals only. Exit signals close a position. They cannot open, add or reduce one."));
            sb.AppendLine("</p>");
            sb.AppendLine("<p class=\"fsb_go_top\"><a href=\"#fsb_header\" title=\"" + Language.T("Go to the beginning.") + "\">" + Language.T("Top") + "</a></p>");
            // Environment
            sb.AppendLine();
            sb.AppendLine("<h2 id=\"environment\">" + Language.T("Environment") + "</h2>");
            sb.AppendLine(EnvironmentHTMLReport().ToString());
            sb.AppendLine("<p class=\"fsb_go_top\"><a href=\"#fsb_header\" title=\"" + Language.T("Go to the beginning.") + "\">" + Language.T("Top") + "</a></p>");

            // Strategy properties
            sb.AppendLine();
            sb.AppendLine("<h2 id=\"properties\">" + Language.T("Strategy Properties") + "</h2>");
            sb.AppendLine(StrategyPropertiesHTMLReport().ToString());
            sb.AppendLine("<p class=\"fsb_go_top\"><a href=\"#fsb_header\" title=\"" + Language.T("Go to the beginning.") + "\">" + Language.T("Top") + "</a></p>");

            // Indicator Slots
            sb.AppendLine();
            sb.AppendLine("<h2 id=\"indicators\">" + Language.T("Indicator Slots") + "</h2>");
            sb.AppendLine("<p>" + Language.T("The slots show the logic for the long positions only.") + " ");
            sb.AppendLine(Language.T("Forex Strategy Builder automatically computes the proper logic for the short positions.") + "</p>");
            sb.AppendLine(StrategySlotsHTMLReport().ToString());
            sb.AppendLine("<p class=\"fsb_go_top\"><a href=\"#fsb_header\" title=\"" + Language.T("Go to the beginning.") + "\">" + Language.T("Top") + "</a></p>");

            // Statistic information
            sb.AppendLine();
            sb.AppendLine("<h2 id=\"statistics\">" + Language.T("Statistic Information") + "</h2>" + Environment.NewLine);
            sb.AppendLine(TradingStatsHTMLReport().ToString());
            sb.AppendLine("<p class=\"fsb_go_top\"><a href=\"#fsb_header\" title=\"" + Language.T("Go to the beginning.") + "\">" + Language.T("Top") + "</a></p>");

            // Additional Statistics
            if (Configs.AdditionalStatistics)
            {
                sb.AppendLine();
                sb.AppendLine("<h2 id=\"addstats\">" + Language.T("Additional Statistics") + "</h2>" + Environment.NewLine);
                sb.AppendLine(AdditionalStatsHTMLReport().ToString());
                sb.AppendLine("<p class=\"fsb_go_top\"><a href=\"#fsb_header\" title=\"" + Language.T("Go to the beginning.") + "\">" + Language.T("Top") + "</a></p>");
            }

            // Footer
            sb.AppendLine("</div></body></html>");

            return sb.ToString();
        }

        /// <summary>
        /// Generates a HTML report about the opening logic.
        /// </summary>
        StringBuilder OpeningLogicHTMLReport()
        {
            StringBuilder sb = new StringBuilder();
            string indicatorName = Data.Strategy.Slot[0].IndicatorName;

            Indicator indOpen = Indicator_Store.ConstructIndicator(indicatorName, SlotTypes.Open);
            indOpen.IndParam  = Data.Strategy.Slot[0].IndParam;
            indOpen.SetDescription(SlotTypes.Open);

            // Logical groups of the opening conditions.
            List<string> opengroups = new List<string>();
            for (int slot = 1; slot <= Data.Strategy.OpenFilters; slot++)
            {
                string group = Data.Strategy.Slot[slot].LogicalGroup;
                if (!opengroups.Contains(group) && group != "All")
                    opengroups.Add(group); // Adds all groups except "All"
            }
            if (opengroups.Count == 0 && Data.Strategy.OpenFilters > 0)
                opengroups.Add("All"); // If all the slots are in "All" group, adds "All" to the list.

            // Long position
            string openLong = "<p>";

            if (Data.Strategy.sameDirSignal == SameDirSignalAction.Add)
                openLong = Language.T("Open a new long position or add to an existing position");
            else if (Data.Strategy.sameDirSignal == SameDirSignalAction.Winner)
                openLong = Language.T("Open a new long position or add to a winning position");
            else if (Data.Strategy.sameDirSignal == SameDirSignalAction.Nothing)
                openLong = Language.T("Open a new long position");

            if (OppSignalAction == OppositeDirSignalAction.Close)
                openLong += " " + Language.T("or close a short position");
            else if (OppSignalAction == OppositeDirSignalAction.Reduce)
                openLong += " " + Language.T("or reduce a short position");
            else if (OppSignalAction == OppositeDirSignalAction.Reverse)
                openLong += " " + Language.T("or reverse a short position");
            else if (OppSignalAction == OppositeDirSignalAction.Nothing)
                openLong += "";

            openLong += " " + indOpen.EntryPointLongDescription;

            if (Data.Strategy.OpenFilters == 0)
                openLong += ".</p>";
            else if (Data.Strategy.OpenFilters == 1)
                openLong += " " + Language.T("when the following logic condition is satisfied") + ":</p>";
            else if (opengroups.Count > 1)
                openLong += " " + Language.T("when") + ":</p>";
            else
                openLong += " " + Language.T("when all the following logic conditions are satisfied") + ":</p>";

            sb.AppendLine(openLong);

            // Open Filters
            if (Data.Strategy.OpenFilters > 0)
            {
                int groupnumb = 1;
                if (opengroups.Count > 1)
                    sb.AppendLine("<ul>");

                foreach (string group in opengroups)
                {
                    if (opengroups.Count > 1)
                    {
                        sb.AppendLine("<li>" + (groupnumb == 1 ? "" : Language.T("or") + " ") + Language.T("logical group [#] is satisfied").Replace("#", group) + ":");
                        groupnumb++;
                    }

                    sb.AppendLine("<ul>");
                    int indInGroup = 0;
                    for (int slot = 1; slot <= Data.Strategy.OpenFilters; slot++)
                        if (Data.Strategy.Slot[slot].LogicalGroup == group || Data.Strategy.Slot[slot].LogicalGroup == "All")
                            indInGroup++;

                    int indnumb = 1;
                    for (int slot = 1; slot <= Data.Strategy.OpenFilters; slot++)
                    {
                        if (Data.Strategy.Slot[slot].LogicalGroup != group && Data.Strategy.Slot[slot].LogicalGroup != "All")
                            continue;

                        Indicator indOpenFilter = Indicator_Store.ConstructIndicator(Data.Strategy.Slot[slot].IndicatorName, SlotTypes.OpenFilter);
                        indOpenFilter.IndParam = Data.Strategy.Slot[slot].IndParam;
                        indOpenFilter.SetDescription(SlotTypes.OpenFilter);

                        if (indnumb < indInGroup)
                            sb.AppendLine("<li>" + indOpenFilter.EntryFilterLongDescription + "; " + Language.T("and") + "</li>");
                        else
                            sb.AppendLine("<li>" + indOpenFilter.EntryFilterLongDescription + ".</li>");

                        indnumb++;
                    }
                    sb.AppendLine("</ul>");

                    if (opengroups.Count > 1)
                        sb.AppendLine("</li>");
                }

                if (opengroups.Count > 1)
                    sb.AppendLine("</ul>");
            }

            // Short position
            string openShort = "<p>";

            if (Data.Strategy.sameDirSignal == SameDirSignalAction.Add)
                openShort = Language.T("Open a new short position or add to an existing position");
            else if (Data.Strategy.sameDirSignal == SameDirSignalAction.Winner)
                openShort = Language.T("Open a new short position or add to a winning position");
            else if (Data.Strategy.sameDirSignal == SameDirSignalAction.Nothing)
                openShort = Language.T("Open a new short position");

            if (OppSignalAction == OppositeDirSignalAction.Close)
                openShort += " " + Language.T("or close a long position");
            else if (OppSignalAction == OppositeDirSignalAction.Reduce)
                openShort += " " + Language.T("or reduce a long position");
            else if (OppSignalAction == OppositeDirSignalAction.Reverse)
                openShort += " " + Language.T("or reverse a long position");
            else if (OppSignalAction == OppositeDirSignalAction.Nothing)
                openShort += "";

            openShort += " " + indOpen.EntryPointShortDescription;

            if (Data.Strategy.OpenFilters == 0)
                openShort += ".</p>";
            else if (Data.Strategy.OpenFilters == 1)
                openShort += " " + Language.T("when the following logic condition is satisfied") + ":</p>";
            else if (opengroups.Count > 1)
                openShort += " " + Language.T("when") + ":</p>";
            else
                openShort += " " + Language.T("when all the following logic conditions are satisfied") + ":</p>";

            sb.AppendLine(openShort);

            // Open Filters
            if (Data.Strategy.OpenFilters > 0)
            {
                int groupnumb = 1;
                if (opengroups.Count > 1)
                    sb.AppendLine("<ul>");

                foreach (string group in opengroups)
                {
                    if (opengroups.Count > 1)
                    {
                        sb.AppendLine("<li>" + (groupnumb == 1 ? "" : Language.T("or") + " ") + Language.T("logical group [#] is satisfied").Replace("#", group) + ":");
                        groupnumb++;
                    }

                    sb.AppendLine("<ul>");
                    int indInGroup = 0;
                    for (int slot = 1; slot <= Data.Strategy.OpenFilters; slot++)
                        if (Data.Strategy.Slot[slot].LogicalGroup == group || Data.Strategy.Slot[slot].LogicalGroup == "All")
                            indInGroup++;

                    int indnumb = 1;
                    for (int slot = 1; slot <= Data.Strategy.OpenFilters; slot++)
                    {
                        if (Data.Strategy.Slot[slot].LogicalGroup != group && Data.Strategy.Slot[slot].LogicalGroup != "All")
                            continue;

                        Indicator indOpenFilter = Indicator_Store.ConstructIndicator(Data.Strategy.Slot[slot].IndicatorName, SlotTypes.OpenFilter);
                        indOpenFilter.IndParam = Data.Strategy.Slot[slot].IndParam;
                        indOpenFilter.SetDescription(SlotTypes.OpenFilter);

                        if (indnumb < indInGroup)
                            sb.AppendLine("<li>" + indOpenFilter.EntryFilterShortDescription + "; " + Language.T("and") + "</li>");
                        else
                            sb.AppendLine("<li>" + indOpenFilter.EntryFilterShortDescription + ".</li>");

                        indnumb++;
                    }
                    sb.AppendLine("</ul>");

                    if (opengroups.Count > 1)
                        sb.AppendLine("</li>");
                }
                if (opengroups.Count > 1)
                    sb.AppendLine("</ul>");
            }

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the closing logic.
        /// </summary>
        StringBuilder ClosingLogicHTMLReport()
        {
            StringBuilder sb = new StringBuilder();

            int    closingSlotNmb = Data.Strategy.CloseSlot;
            string indicatorName  = Data.Strategy.Slot[closingSlotNmb].IndicatorName;

            Indicator indClose = Indicator_Store.ConstructIndicator(indicatorName, SlotTypes.Close);
            indClose.IndParam  = Data.Strategy.Slot[closingSlotNmb].IndParam;
            indClose.SetDescription(SlotTypes.Close);

            bool isGroups = false;
            List<string> closegroups = new List<string>();

            if (Data.Strategy.CloseFilters > 0)
                foreach (IndicatorSlot slot in Data.Strategy.Slot)
                {
                    if (slot.SlotType == SlotTypes.CloseFilter)
                    {
                        if (slot.LogicalGroup == "all" && Data.Strategy.CloseFilters > 1)
                            isGroups = true;

                        if (closegroups.Contains(slot.LogicalGroup))
                            isGroups = true;
                        else if (slot.LogicalGroup != "all")
                            closegroups.Add(slot.LogicalGroup);
                    }
                }

            if (closegroups.Count == 0 && Data.Strategy.CloseFilters > 0)
                closegroups.Add("all"); // If all the slots are in "all" group, adds "all" to the list.


            // Long position
            string closeLong = "<p>" + Language.T("Close an existing long position") + " " + indClose.ExitPointLongDescription;

            if (Data.Strategy.CloseFilters == 0)
                closeLong += ".</p>";
            else if (Data.Strategy.CloseFilters == 1)
                closeLong += " " + Language.T("when the following logic condition is satisfied") + ":</p>";
            else if (isGroups)
                closeLong += " " + Language.T("when") + ":</p>";
            else
                closeLong += " " + Language.T("when at least one of the following logic conditions is satisfied") + ":</p>";

            sb.AppendLine(closeLong);

            // Close Filters
            if (Data.Strategy.CloseFilters > 0)
            {
                int groupnumb = 1;
                sb.AppendLine("<ul>");

                foreach (string group in closegroups)
                {
                    if (isGroups)
                    {
                        sb.AppendLine("<li>" + (groupnumb == 1 ? "" : Language.T("or") + " ") + Language.T("logical group [#] is satisfied").Replace("#", group) + ":");
                        sb.AppendLine("<ul>");
                        groupnumb++;
                    }

                    int indInGroup = 0;
                    for (int slot = closingSlotNmb + 1; slot < Data.Strategy.Slots; slot++)
                        if (Data.Strategy.Slot[slot].LogicalGroup == group || Data.Strategy.Slot[slot].LogicalGroup == "all")
                            indInGroup++;

                    int indnumb = 1;
                    for (int slot = closingSlotNmb + 1; slot < Data.Strategy.Slots; slot++)
                    {
                        if (Data.Strategy.Slot[slot].LogicalGroup != group && Data.Strategy.Slot[slot].LogicalGroup != "all")
                            continue;

                        Indicator indCloseFilter = Indicator_Store.ConstructIndicator(Data.Strategy.Slot[slot].IndicatorName, SlotTypes.CloseFilter);
                        indCloseFilter.IndParam = Data.Strategy.Slot[slot].IndParam;
                        indCloseFilter.SetDescription(SlotTypes.CloseFilter);

                        if (isGroups)
                        {
                            if (indnumb < indInGroup)
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterLongDescription + "; " + Language.T("and") + "</li>");
                            else
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterLongDescription + ".</li>");
                        }
                        else
                        {
                            if (slot < Data.Strategy.Slots - 1)
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterLongDescription + "; " + Language.T("or") + "</li>");
                            else
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterLongDescription + ".</li>");
                        }
                        indnumb++;
                    }

                    if (isGroups)
                    {
                        sb.AppendLine("</ul>");
                        sb.AppendLine("</li>");
                    }
                }

                sb.AppendLine("</ul>");
            }

            // Short position
            string closeShort = "<p>" + Language.T("Close an existing short position") + " " + indClose.ExitPointShortDescription;

            if (Data.Strategy.CloseFilters == 0)
                closeShort += ".</p>";
            else if (Data.Strategy.CloseFilters == 1)
                closeShort += " " + Language.T("when the following logic condition is satisfied") + ":</p>";
            else if (isGroups)
                closeShort += " " + Language.T("when") + ":</p>";
            else
                closeShort += " " + Language.T("when at least one of the following logic conditions is satisfied") + ":</p>";

            sb.AppendLine(closeShort);

            // Close Filters
            if (Data.Strategy.CloseFilters > 0)
            {
                int groupnumb = 1;
                sb.AppendLine("<ul>");

                foreach (string group in closegroups)
                {
                    if (isGroups)
                    {
                        sb.AppendLine("<li>" + (groupnumb == 1 ? "" : Language.T("or") + " ") + Language.T("logical group [#] is satisfied").Replace("#", group) + ":");
                        sb.AppendLine("<ul>");
                        groupnumb++;
                    }

                    int indInGroup = 0;
                    for (int slot = closingSlotNmb + 1; slot < Data.Strategy.Slots; slot++)
                        if (Data.Strategy.Slot[slot].LogicalGroup == group || Data.Strategy.Slot[slot].LogicalGroup == "all")
                            indInGroup++;

                    int indnumb = 1;
                    for (int slot = closingSlotNmb + 1; slot < Data.Strategy.Slots; slot++)
                    {
                        if (Data.Strategy.Slot[slot].LogicalGroup != group && Data.Strategy.Slot[slot].LogicalGroup != "all")
                            continue;

                        Indicator indCloseFilter = Indicator_Store.ConstructIndicator(Data.Strategy.Slot[slot].IndicatorName, SlotTypes.CloseFilter);
                        indCloseFilter.IndParam = Data.Strategy.Slot[slot].IndParam;
                        indCloseFilter.SetDescription(SlotTypes.CloseFilter);

                        if (isGroups)
                        {
                            if (indnumb < indInGroup)
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterShortDescription + "; " + Language.T("and") + "</li>");
                            else
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterShortDescription + ".</li>");
                        }
                        else
                        {
                            if (slot < Data.Strategy.Slots - 1)
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterShortDescription + "; " + Language.T("or") + "</li>");
                            else
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterShortDescription + ".</li>");
                        }
                        indnumb++;
                    }

                    if (isGroups)
                    {
                        sb.AppendLine("</ul>");
                        sb.AppendLine("</li>");
                    }
                }

                sb.AppendLine("</ul>");
            }

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the averaging logic.
        /// </summary>
        StringBuilder AveragingHTMLReport()
        {
            StringBuilder sb = new StringBuilder();

            // Same direction
            sb.AppendLine("<p>" + Language.T("Entry signal in the direction of the present position:") + "</p>");

            sb.AppendLine("<ul><li>");
            if (Data.Strategy.sameDirSignal == SameDirSignalAction.Nothing)
                sb.AppendLine(Language.T("No averaging is allowed. Cancel any additional orders which are in the same direction."));
            else if (Data.Strategy.sameDirSignal == SameDirSignalAction.Winner)
                sb.AppendLine(Language.T("Add to a wining position but not to a losing one. If the position is at a loss, cancel the additional entry order. Do not exceed the maximum allowed number of lots to open."));
            else if (Data.Strategy.sameDirSignal == SameDirSignalAction.Add)
                sb.AppendLine(Language.T("Add to the position no matter if it is at a profit or loss. Do not exceed the maximum allowed number of lots to open."));
            sb.AppendLine("</li></ul>");

            // Opposite direction
            sb.AppendLine("<p>" + Language.T("Entry signal in the opposite direction:") + "</p>");

            sb.AppendLine("<ul><li>");
            if (Data.Strategy.oppDirSignal == OppositeDirSignalAction.Nothing)
                sb.AppendLine(Language.T("No modification of the present position is allowed. Cancel any additional orders which are in the opposite direction."));
            else if (Data.Strategy.oppDirSignal == OppositeDirSignalAction.Reduce)
                sb.AppendLine(Language.T("Reduce the present position. If its amount is lower than or equal to the specified reducing lots, close it."));
            else if (Data.Strategy.oppDirSignal == OppositeDirSignalAction.Close)
                sb.AppendLine(Language.T("Close the present position regardless of its amount or result. Do not open a new position until the next entry signal has been raised."));
            else if (Data.Strategy.oppDirSignal == OppositeDirSignalAction.Reverse)
                sb.AppendLine(Language.T("Close the existing position and open a new one in the opposite direction using the entry rules."));
            sb.AppendLine("</li></ul>");

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the trading sizes.
        /// </summary>
        StringBuilder TradingSizeHTMLReport()
        {
            StringBuilder sb = new StringBuilder();

            if (UseAccountPercentEntry)
            {
                sb.AppendLine("<p>" + Language.T("Trade percent of your account.") + "</p>");

                sb.AppendLine("<ul>");
                sb.AppendLine("<li>" + Language.T("Opening of a new position") + " - " + EntryLots + Language.T("% of the account equity") + ".</li>");
                if (sameDirSignal == SameDirSignalAction.Winner)
                    sb.AppendLine("<li>" + Language.T("Adding to a winning position") + " - " + AddingLots + Language.T("% of the account equity") + ". " + Language.T("Do not open more than") + " " + Plural("lot", MaxOpenLots) + ".</li>");
                if (sameDirSignal == SameDirSignalAction.Add)
                    sb.AppendLine("<li>" + Language.T("Adding to a position") + " - " + AddingLots + Language.T("% of the account equity") + ". " + Language.T("Do not open more than") + " " + Plural("lot", MaxOpenLots) + ".</li>");
                if (oppDirSignal == OppositeDirSignalAction.Reduce)
                    sb.AppendLine("<li>" + Language.T("Reducing a position") + " - " + ReducingLots + Language.T("% of the account equity") + ".</li>");
                if (oppDirSignal == OppositeDirSignalAction.Reverse)
                    sb.AppendLine("<li>" + Language.T("Reversing a position") + " - " + EntryLots + Language.T("% of the account equity") + " " + Language.T("in the opposite direction.") + "</li>");
                sb.AppendLine("</ul>");
            }
            else
            {
                sb.AppendLine("<p>" + Language.T("Always trade a constant number of lots.") + "</p>");

                sb.AppendLine("<ul>");
                sb.AppendLine("<li>" + Language.T("Opening of a new position") + " - " + Plural("lot", EntryLots) + ".</li>");
                if (sameDirSignal == SameDirSignalAction.Winner)
                    sb.AppendLine("<li>" + Language.T("Adding to a winning position") + " - " + Plural("lot", AddingLots) + ". " + Language.T("Do not open more than") + " " + Plural("lot", MaxOpenLots) + ".</li>");
                if (sameDirSignal == SameDirSignalAction.Add)
                    sb.AppendLine("<li>" + Language.T("Adding to a position") + " - " + Plural("lot", AddingLots) + ". " + Language.T("Do not open more than") + " " + Plural("lot", MaxOpenLots) + ".</li>");
                if (oppDirSignal == OppositeDirSignalAction.Reduce)
                    sb.AppendLine("<li>" + Language.T("Reducing a position") + " - " + Plural("lot", ReducingLots) + " " + Language.T("from the current position.") + "</li>");
                if (oppDirSignal == OppositeDirSignalAction.Reverse)
                    sb.AppendLine("<li>" + Language.T("Reversing a position") + " - " + Plural("lot", EntryLots) + " " + Language.T("in the opposite direction.") + "</li>");
                sb.AppendLine("</ul>");
            }

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the trading environment.
        /// </summary>
        StringBuilder EnvironmentHTMLReport()
        {
            StringBuilder sb = new StringBuilder();

            string swapUnit = (Data.InstrProperties.SwapType == Commission_Type.money ? Data.InstrProperties.PriceIn : Language.T(Data.InstrProperties.SwapType.ToString()));
            string commission = Language.T("Commission") + " " + Data.InstrProperties.CommissionScopeToString + " " + Data.InstrProperties.CommissionTimeToString;
            string commUnit = (Data.InstrProperties.CommissionType == Commission_Type.money ? Data.InstrProperties.PriceIn : Language.T(Data.InstrProperties.CommissionType.ToString()));

            sb.AppendLine("<h3>" + Language.T("Market") + "</h3>");
            sb.AppendLine("<table class=\"fsb_table_cleen\" cellspacing=\"0\">");
            sb.AppendLine("<tr><td>" + Language.T("Symbol")     + "</td><td> - <strong>" + Data.Symbol    + "</strong></td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("Time frame") + "</td><td> - <strong>" + Data.PeriodString + "</strong></td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<h3>" + Language.T("Account") + "</h3>");
            sb.AppendLine("<table class=\"fsb_table_cleen\" cellspacing=\"0\">");
            sb.AppendLine("<tr><td>" + Language.T("Initial account") + "</td><td> - " + Configs.InitialAccount.ToString("F2") + " " + Configs.AccountCurrency + "</td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("Lot size")        + "</td><td> - "   + Data.InstrProperties.LotSize.ToString() + "</td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("Leverage")        + "</td><td> - 1/" + Configs.Leverage + "</td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("Required margin") + "</td><td> - " + Backtester.RequiredMargin(1, Data.Bars - 1).ToString("F2") + " " + Configs.AccountCurrency + "* " + Language.T("for each open lot") + "</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<h3>" + Language.T("Charges") + "</h3>");
            sb.AppendLine("<table class=\"fsb_table_cleen\" cellspacing=\"0\">");
            sb.AppendLine("<tr><td>" + Language.T("Spread") + "</td><td> - " + Plural("pip", Data.InstrProperties.Spread) + "</td><td>(" + Backtester.PipsToMoney(Data.InstrProperties.Spread, Data.Bars - 1).ToString("F2") + " " + Configs.AccountCurrency + "*)</td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("Swap number for a long position rollover")  + "</td><td> - " + Data.InstrProperties.SwapLong.ToString()  + " " + swapUnit + "</td><td>(" + Backtester.RolloverInMoney(PosDirection.Long,  1, 1, Data.Close[Data.Bars - 1]).ToString("F2") + " " + Configs.AccountCurrency + "*)</td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("Swap number for a short position rollover") + "</td><td> - " + Data.InstrProperties.SwapShort.ToString() + " " + swapUnit + "</td><td>(" + Backtester.RolloverInMoney(PosDirection.Short, 1, 1, Data.Close[Data.Bars - 1]).ToString("F2") + " " + Configs.AccountCurrency + "*)</td></tr>");
            sb.AppendLine("<tr><td>" + commission + "</td><td> - " + Data.InstrProperties.Commission.ToString() + " " + commUnit + "</td><td>(" + Backtester.CommissionInMoney(1, Data.Close[Data.Bars-1], false).ToString("F2") + " " + Configs.AccountCurrency + "*)</td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("Slippage") + "</td><td> - " + Plural("pip", Data.InstrProperties.Slippage) + "</td><td>(" + Backtester.PipsToMoney(Data.InstrProperties.Slippage, Data.Bars - 1).ToString("F2") + " " + Configs.AccountCurrency + "*)</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<p>--------------<br />");
            sb.AppendLine("* " + Language.T("This value may vary!"));
            sb.AppendLine("</p>");

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the strategy properties.
        /// </summary>
        StringBuilder StrategyPropertiesHTMLReport()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<h3>" + Language.T("Handling of Additional Entry Signals") + "</h3>");
            sb.AppendLine("<p>");
            sb.AppendLine(Language.T("Next same direction signal behavior") + " - ");
            if (SameSignalAction == SameDirSignalAction.Add)
                sb.AppendLine(Language.T("Adds to the position"));
            else if (SameSignalAction == SameDirSignalAction.Winner)
                sb.AppendLine(Language.T("Adds to a winning position"));
            else if (SameSignalAction == SameDirSignalAction.Nothing)
                sb.AppendLine(Language.T("Does nothing"));
            sb.AppendLine("<br />");
            sb.AppendLine(Language.T("Next opposite direction signal behavior") + " - ");
            if (OppSignalAction == OppositeDirSignalAction.Close)
                sb.AppendLine(Language.T("Closes the position"));
            else if (OppSignalAction == OppositeDirSignalAction.Reduce)
                sb.AppendLine(Language.T("Reduces the position"));
            else if (OppSignalAction == OppositeDirSignalAction.Reverse)
                sb.AppendLine(Language.T("Reverses the position"));
            else if (OppSignalAction == OppositeDirSignalAction.Nothing)
                sb.AppendLine(Language.T("Does nothing"));
            sb.AppendLine("</p>");

            sb.AppendLine("<h3>" + Language.T("Trading Size") + "</h3>");
            if (UseAccountPercentEntry)
                sb.AppendLine("<p>" + Language.T("Trade percent of your account. The percentage values show the part of the account equity used to cover the required margin.") + "</p>");

            sb.AppendLine("<table cellspacing=\"0\">");
            string sTradingUnit = UseAccountPercentEntry ? Language.T("% of the account equity") : "";
            sb.AppendLine("<tr><td>" + Language.T("Maximum number of open lots")                    + "</td><td> - " + MaxOpenLots + "</td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("Number of entry lots for a new position")        + "</td><td> - " + EntryLots    + sTradingUnit + "</td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("In case of addition - number of lots to add")    + "</td><td> - " + AddingLots   + sTradingUnit + "</td></tr>");
            sb.AppendLine("<tr><td>" + Language.T("In case of reduction - number of lots to close") + "</td><td> - " + ReducingLots + sTradingUnit + "</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<h3>" + Language.T("Permanent Protection") + "</h3>");
            string sPermSL = Data.Strategy.UsePermanentSL ? (Data.Strategy.PermanentSLType == PermanentProtectionType.Absolute ? "(Abs) " : "") + Data.Strategy.PermanentSL.ToString() + " " + Language.T("pips") : "- " + Language.T("None");
            sb.AppendLine("<p>" + Language.T("Permanent Stop Loss") + " " + sPermSL + "</p>");
            string sPermTP = Data.Strategy.UsePermanentTP ? (Data.Strategy.PermanentTPType == PermanentProtectionType.Absolute ? "(Abs) " : "") + Data.Strategy.PermanentTP.ToString() + " " + Language.T("pips") : "- " + Language.T("None");
            sb.AppendLine("<p>" + Language.T("Permanent Take Profit") + " " + sPermTP + "</p>");

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the strategy slots
        /// </summary>
        StringBuilder StrategySlotsHTMLReport()
        {
            StringBuilder sb = new StringBuilder();

            // Strategy Properties
            string sameDir = Language.T(Data.Strategy.SameSignalAction.ToString());
            string oppDir  = Language.T(Data.Strategy.OppSignalAction.ToString());
            string permaSL = Data.Strategy.UsePermanentSL ? (Data.Strategy.PermanentSLType == PermanentProtectionType.Absolute ? "(Abs) " : "") + Data.Strategy.PermanentSL.ToString() : Language.T("None");
            string permaTP = Data.Strategy.UsePermanentTP ? (Data.Strategy.PermanentTPType == PermanentProtectionType.Absolute ? "(Abs) " : "") + Data.Strategy.PermanentTP.ToString() : Language.T("None");
            sb.AppendLine("<div class=\"fsb_strategy_slot\" style=\"border: solid 2px #966\">");
            sb.AppendLine("\t<div class=\"fsb_properties_slot\">" + Language.T("Strategy Properties") + "</div>");
            sb.AppendLine("\t<table style=\"margin-left: auto; margin-right: auto;\">");
            sb.AppendLine("\t\t<tr><td>" + Language.T("Same direction signal")     + "</td><td> - " + sameDir + "</td></tr>");
            sb.AppendLine("\t\t<tr><td>" + Language.T("Opposite direction signal") + "</td><td> - " + oppDir  + "</td></tr>");
            sb.AppendLine("\t\t<tr><td>" + Language.T("Permanent Stop Loss")       + "</td><td> - " + permaSL + "</td></tr>");
            sb.AppendLine("\t\t<tr><td>" + Language.T("Permanent Take Profit")     + "</td><td> - " + permaTP + "</td></tr>");
            sb.AppendLine("\t</table>");
            sb.AppendLine("</div>");

            // Add the slots
            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                IndicatorSlot indSlot = Slot[slot];
                string slotType = "";
                switch (indSlot.SlotType)
                {
                    case SlotTypes.Open:
                        sb.AppendLine("<div class=\"fsb_strategy_slot\" style=\"border: solid 2px #693\">");
                        slotType = "\t<div class=\"fsb_open_slot\">" + Language.T("Opening Point of the Position") + "</div>";
                        break;
                    case SlotTypes.OpenFilter:
                        sb.AppendLine("<div class=\"fsb_strategy_slot\" style=\"border: solid 2px #699\">");
                        slotType = "\t<div class=\"fsb_open_filter_slot\">" + Language.T("Opening Logic Condition") + "</div>";
                        break;
                    case SlotTypes.Close:
                        sb.AppendLine("<div class=\"fsb_strategy_slot\" style=\"border: solid 2px #d63\">");
                        slotType = "\t<div class=\"fsb_close_slot\">" + Language.T("Closing Point of the Position") + "</div>";
                        break;
                    case SlotTypes.CloseFilter:
                        sb.AppendLine("<div class=\"fsb_strategy_slot\" style=\"border: solid 2px #d99\">");
                        slotType = "\t<div class=\"fsb_close_filter_slot\">" + Language.T("Closing Logic Condition") + "</div>";
                        break;
                    default:
                        break;
                }

                sb.AppendLine(slotType);
                sb.AppendLine("\t<div class=\"fsb_str_indicator\">" + indSlot.IndicatorName + "</div>");

                // Add the logic
                foreach (ListParam listParam in indSlot.IndParam.ListParam)
                    if (listParam.Enabled && listParam.Caption == "Logic")
                        sb.AppendLine("\t<div class=\"fsb_str_logic\">" + 
                            (Configs.UseLogicalGroups && (indSlot.SlotType == SlotTypes.OpenFilter || indSlot.SlotType == SlotTypes.CloseFilter) ?
                            "<span style=\"float: left; margin-left: 5px; margin-right: -25px\">" + "[" + indSlot.LogicalGroup + "]" + "</span>" : "") +
                            listParam.Text + "</div>");

                // Adds the parameters
                StringBuilder sbParams = new StringBuilder();

                // Add the list params
                foreach (ListParam listParam in indSlot.IndParam.ListParam)
                    if (listParam.Enabled && listParam.Caption != "Logic")
                        sbParams.AppendLine("\t\t<tr><td>" + listParam.Caption + "</td><td> - " + listParam.Text + "</td></tr>");

                // Add the num params
                foreach (NumericParam numParam in indSlot.IndParam.NumParam)
                    if (numParam.Enabled)
                        sbParams.AppendLine("\t\t<tr><td>" + numParam.Caption + "</td><td> - " + numParam.ValueToString + "</td></tr>");

                // Add the check params
                foreach (CheckParam checkParam in indSlot.IndParam.CheckParam)
                    if (checkParam.Enabled)
                        sbParams.AppendLine("\t\t<tr><td>" + checkParam.Caption + "</td><td> - " + (checkParam.Checked ? "Yes" : "No") + "</td></tr>");

                // Adds the params if there are any
                if (sbParams.Length > 0)
                {
                    sb.AppendLine("\t<table style=\"margin-left: auto; margin-right: auto;\">");
                    sb.AppendLine(sbParams.ToString());
                    sb.AppendLine("\t</table>");
                }

                sb.AppendLine("</div>");
            }

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the trading stats
        /// </summary>
        StringBuilder TradingStatsHTMLReport()
        {
            StringBuilder sb = new StringBuilder();
            int rows = Math.Max(Data.MarketStatsParam.Length, Backtester.AccountStatsParam.Length);

            sb.AppendLine("<table class=\"fsb_table\" cellspacing=\"0\" cellpadding=\"5\">");
            sb.AppendLine("<tr><th colspan=\"2\">" + Language.T("Market") + "</th><th colspan=\"2\">" + Language.T("Account") + "</th></tr>");
            for (int i = 0; i < rows; i++)
            {
                sb.Append("<tr>");
                if (i < Data.MarketStatsParam.Length)
                    sb.Append("<td><strong>" + Data.MarketStatsParam[i] + "</strong></td><td>" + Data.MarketStatsValue[i] + "</td>");
                else
                    sb.Append("<td></td><td></td>");
                if (i < Backtester.AccountStatsParam.Length)
                    sb.Append("<td><strong>" + Backtester.AccountStatsParam[i] + "</strong></td><td>" + Backtester.AccountStatsValue[i] + "</td>");
                else
                    sb.Append("<td></td><td></td>");
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the additional stats
        /// </summary>
        StringBuilder AdditionalStatsHTMLReport()
        {
            StringBuilder sb = new StringBuilder();
            int rows = Backtester.AdditionalStatsParamName.Length;

            sb.AppendLine("<table class=\"fsb_table\" cellspacing=\"0\" cellpadding=\"5\">");
            sb.AppendLine("<tr><th>" + Language.T("Parameter") + "</th><th>" + Language.T("Long") + " + " + Language.T("Short") + "</th><th>" + Language.T("Long") + "</th><th>" + Language.T("Short") + "</th></tr>");
            for (int i = 0; i < rows; i++)
            {
                sb.Append("<tr>");
                sb.Append("<td><strong>" + Backtester.AdditionalStatsParamName[i] + "</strong></td>" +
                    "<td>" + Backtester.AdditionalStatsValueTotal[i] + "</td>" +
                    "<td>" + Backtester.AdditionalStatsValueLong[i]  + "</td>" +
                    "<td>" + Backtester.AdditionalStatsValueShort[i] + "</td>");
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");

            return sb;
        }

        /// <summary>
        /// Represents the strategy in a readable form.
        /// </summary>
        public override string ToString()
        {
            string str = String.Empty;
            str += "Strategy Name - "       + strategyName              + Environment.NewLine;
            str += "Symbol - "              + Symbol                    + Environment.NewLine;
            str += "Period - "              + DataPeriod.ToString()     + Environment.NewLine;
            str += "Same dir signal - "     + sameDirSignal.ToString()  + Environment.NewLine;
            str += "Opposite dir signal - " + oppDirSignal.ToString()   + Environment.NewLine;
            str += "Use account % entry - " + UseAccountPercentEntry    + Environment.NewLine;
            str += "Max open lots - "       + MaxOpenLots               + Environment.NewLine;
            str += "Entry lots - "          + EntryLots                 + Environment.NewLine;
            str += "Adding lots - "         + AddingLots                + Environment.NewLine;
            str += "Reducing lots - "       + ReducingLots              + Environment.NewLine;
            str += "Use Permanent S/L - "   + usePermanentSL.ToString() + Environment.NewLine;
            str += "Permanent S/L - "       + permanentSLType.ToString() + " " + permanentSL.ToString()    + Environment.NewLine;
            str += "Use Permanent T/P - "   + usePermanentTP.ToString() + Environment.NewLine;
            str += "Permanent T/P - "       + permanentTPType.ToString() + " " + permanentTP.ToString()    + Environment.NewLine + Environment.NewLine;
            str += "Description"            + Environment.NewLine + Description + Environment.NewLine + Environment.NewLine;

            for (int slot = 0; slot < Slots; slot++)
            {
                str += Slot[slot].SlotType.ToString() + Environment.NewLine;
                str += Slot[slot].IndicatorName + Environment.NewLine;
                str += indicatorSlot[slot].IndParam.ToString() + Environment.NewLine + Environment.NewLine;
            }

            return str;
        }

        /// <summary>
        /// Appends "s" when plural
        /// </summary>
        string Plural(string unit, double number)
        {
            if (number != 1 && number != -1)
                unit += "s";

            return number.ToString() + " " + Language.T(unit);
        }
    }
}