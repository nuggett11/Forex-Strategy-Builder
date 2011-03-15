// Strategy_XML Class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Xml;

namespace Forex_Strategy_Builder
{
    public class Strategy_XML
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Strategy_XML() { }

        /// <summary>
        /// Represents the Strategy as a XmlDocument.
        /// </summary>
        public XmlDocument CreateStrategyXmlDoc(Strategy strategy)
        {
            // Create the XmlDocument.
            XmlDocument xmlDocStrategy = new XmlDocument();
            xmlDocStrategy.LoadXml("<strategy></strategy>");

            //Create the XML declaration.
            XmlDeclaration xmldecl;
            xmldecl = xmlDocStrategy.CreateXmlDeclaration("1.0", null, null);

            //Add new node to the document.
            XmlElement root = xmlDocStrategy.DocumentElement;
            xmlDocStrategy.InsertBefore(xmldecl, root);

            // Add the program name.
            XmlElement newElem = xmlDocStrategy.CreateElement("programName");
            newElem.InnerText = Data.ProgramName;
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the program version.
            newElem = xmlDocStrategy.CreateElement("programVersion");
            newElem.InnerText = Data.ProgramVersion.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the strategy name.
            newElem = xmlDocStrategy.CreateElement("strategyName");
            newElem.InnerText = strategy.StrategyName;
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the Symbol.
            newElem = xmlDocStrategy.CreateElement("instrumentSymbol");
            newElem.InnerText = strategy.Symbol;
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the Period.
            newElem = xmlDocStrategy.CreateElement("instrumentPeriod");
            newElem.InnerText = strategy.DataPeriod.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the same direction signal action.
            newElem = xmlDocStrategy.CreateElement("sameDirSignalAction");
            newElem.InnerText = strategy.SameSignalAction.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the opposite direction signal action.
            newElem = xmlDocStrategy.CreateElement("oppDirSignalAction");
            newElem.InnerText = strategy.OppSignalAction.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the Permanent Stop Loss
            newElem = xmlDocStrategy.CreateElement("permanentStopLoss");
            newElem.InnerText = strategy.PermanentSL.ToString();
            newElem.SetAttribute("usePermanentSL", strategy.UsePermanentSL.ToString());
            newElem.SetAttribute("permanentSLType", strategy.PermanentSLType.ToString());
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the Permanent Take Profit
            newElem = xmlDocStrategy.CreateElement("permanentTakeProfit");
            newElem.InnerText = strategy.PermanentTP.ToString();
            newElem.SetAttribute("usePermanentTP", strategy.UsePermanentTP.ToString());
            newElem.SetAttribute("permanentTPType", strategy.PermanentTPType.ToString());
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the Break Even
            newElem = xmlDocStrategy.CreateElement("breakEven");
            newElem.InnerText = strategy.BreakEven.ToString();
            newElem.SetAttribute("useBreakEven", strategy.UseBreakEven.ToString());
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the max open lots
            newElem = xmlDocStrategy.CreateElement("maxOpenLots");
            newElem.InnerText = strategy.MaxOpenLots.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add Use Acount Percent Entry
            newElem = xmlDocStrategy.CreateElement("useAcountPercentEntry");
            newElem.InnerText = strategy.UseAccountPercentEntry.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the Entry lots
            newElem = xmlDocStrategy.CreateElement("entryLots");
            newElem.InnerText = strategy.EntryLots.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the Adding lots
            newElem = xmlDocStrategy.CreateElement("addingLots");
            newElem.InnerText = strategy.AddingLots.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the Reducing lots
            newElem = xmlDocStrategy.CreateElement("reducingLots");
            newElem.InnerText = strategy.ReducingLots.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Description
            newElem = xmlDocStrategy.CreateElement("description");
            newElem.InnerText = strategy.Description;
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the number of open filters.
            newElem = xmlDocStrategy.CreateElement("openFilters");
            newElem.InnerText = strategy.OpenFilters.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the number of close filters.
            newElem = xmlDocStrategy.CreateElement("closeFilters");
            newElem.InnerText = strategy.CloseFilters.ToString();
            xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the slots.
            for (int slot = 0; slot < strategy.Slots; slot++)
            {
                SlotTypes slType = strategy.Slot[slot].SlotType;

                // Add a slot element.
                XmlElement newSlot = xmlDocStrategy.CreateElement("slot");
                newSlot.SetAttribute("slotNumber", slot.ToString());
                newSlot.SetAttribute("slotType", slType.ToString());

                if (slType == SlotTypes.OpenFilter || slType == SlotTypes.CloseFilter)
                    newSlot.SetAttribute("logicalGroup", strategy.Slot[slot].LogicalGroup);

                // Add an element.
                newElem = xmlDocStrategy.CreateElement("indicatorName");
                newElem.InnerText = strategy.Slot[slot].IndicatorName;
                newSlot.AppendChild(newElem);

                // Add the list params.
                for (int param = 0; param < strategy.Slot[slot].IndParam.ListParam.Length; param++)
                {
                    if (strategy.Slot[slot].IndParam.ListParam[param].Enabled)
                    {
                        // Add an element.
                        XmlElement newListElem = xmlDocStrategy.CreateElement("listParam");
                        newListElem.SetAttribute("paramNumber", param.ToString());

                        // Add an element.
                        newElem = xmlDocStrategy.CreateElement("caption");
                        newElem.InnerText = strategy.Slot[slot].IndParam.ListParam[param].Caption;
                        newListElem.AppendChild(newElem);

                        // Add an element.
                        newElem = xmlDocStrategy.CreateElement("index");
                        newElem.InnerText = strategy.Slot[slot].IndParam.ListParam[param].Index.ToString();
                        newListElem.AppendChild(newElem);

                        // Add an element.
                        newElem = xmlDocStrategy.CreateElement("value");
                        newElem.InnerText = strategy.Slot[slot].IndParam.ListParam[param].Text;
                        newListElem.AppendChild(newElem);

                        newSlot.AppendChild(newListElem);
                    }
                }

                // Add the num params.
                for (int param = 0; param < strategy.Slot[slot].IndParam.NumParam.Length; param++)
                {
                    if (strategy.Slot[slot].IndParam.NumParam[param].Enabled)
                    {
                        // Add an element.
                        XmlElement newNumElem = xmlDocStrategy.CreateElement("numParam");
                        newNumElem.SetAttribute("paramNumber", param.ToString());

                        // Add an element.
                        newElem = xmlDocStrategy.CreateElement("caption");
                        newElem.InnerText = strategy.Slot[slot].IndParam.NumParam[param].Caption;
                        newNumElem.AppendChild(newElem);

                        // Add an element.
                        newElem = xmlDocStrategy.CreateElement("value");
                        newElem.InnerText = strategy.Slot[slot].IndParam.NumParam[param].ValueToString;
                        newNumElem.AppendChild(newElem);

                        newSlot.AppendChild(newNumElem);
                    }
                }

                // Add the check params.
                for (int param = 0; param < strategy.Slot[slot].IndParam.CheckParam.Length; param++)
                {
                    if (strategy.Slot[slot].IndParam.CheckParam[param].Enabled)
                    {
                        // Add an element.
                        XmlElement newCheckElem = xmlDocStrategy.CreateElement("checkParam");
                        newCheckElem.SetAttribute("paramNumber", param.ToString());

                        // Add an element.
                        newElem = xmlDocStrategy.CreateElement("caption");
                        newElem.InnerText = strategy.Slot[slot].IndParam.CheckParam[param].Caption;
                        newCheckElem.AppendChild(newElem);

                        // Add an element.
                        newElem = xmlDocStrategy.CreateElement("value");
                        newElem.InnerText = strategy.Slot[slot].IndParam.CheckParam[param].Checked.ToString();
                        newCheckElem.AppendChild(newElem);

                        newSlot.AppendChild(newCheckElem);
                    }
                }

                xmlDocStrategy.DocumentElement.AppendChild(newSlot);
            }

            return xmlDocStrategy;
        }

        /// <summary>
        /// Pareses a strategy from a xml document.
        /// </summary>
        public Strategy ParseXmlStrategy(XmlDocument xmlDocStrategy)
        {
            // Read the number of slots
            int openFilters  = int.Parse(xmlDocStrategy.GetElementsByTagName("openFilters" )[0].InnerText);
            int closeFilters = int.Parse(xmlDocStrategy.GetElementsByTagName("closeFilters")[0].InnerText);

            // Create the strategy.
            Strategy tempStrategy = new Strategy(openFilters, closeFilters);

            // Same and Opposite direction Actions
            tempStrategy.SameSignalAction = (SameDirSignalAction    )Enum.Parse(typeof(SameDirSignalAction    ), xmlDocStrategy.GetElementsByTagName("sameDirSignalAction")[0].InnerText);
            tempStrategy.OppSignalAction  = (OppositeDirSignalAction)Enum.Parse(typeof(OppositeDirSignalAction), xmlDocStrategy.GetElementsByTagName("oppDirSignalAction" )[0].InnerText);

            // Market
            tempStrategy.Symbol     = xmlDocStrategy.GetElementsByTagName("instrumentSymbol")[0].InnerText;
            tempStrategy.DataPeriod = (DataPeriods)Enum.Parse(typeof(DataPeriods), xmlDocStrategy.GetElementsByTagName("instrumentPeriod")[0].InnerText);

            // Permanent Stop Loss
            tempStrategy.PermanentSL    = Math.Abs(int.Parse(xmlDocStrategy.GetElementsByTagName("permanentStopLoss")[0].InnerText)); // Math.Abs() removes the negative sign from previous versions.
            tempStrategy.UsePermanentSL = bool.Parse(xmlDocStrategy.GetElementsByTagName("permanentStopLoss")[0].Attributes["usePermanentSL"].InnerText);
            try
            {
                tempStrategy.PermanentSLType = (PermanentProtectionType)Enum.Parse(typeof(PermanentProtectionType), xmlDocStrategy.GetElementsByTagName("permanentStopLoss")[0].Attributes["permanentSLType"].InnerText);
            }
            catch
            {
                tempStrategy.PermanentSLType = PermanentProtectionType.Relative;
            }

            // Permanent Take Profit
            tempStrategy.PermanentTP    = int.Parse(xmlDocStrategy.GetElementsByTagName("permanentTakeProfit")[0].InnerText);
            tempStrategy.UsePermanentTP = bool.Parse(xmlDocStrategy.GetElementsByTagName("permanentTakeProfit")[0].Attributes["usePermanentTP"].InnerText);
            try
            {
                tempStrategy.PermanentTPType = (PermanentProtectionType)Enum.Parse(typeof(PermanentProtectionType), xmlDocStrategy.GetElementsByTagName("permanentTakeProfit")[0].Attributes["permanentTPType"].InnerText);
            }
            catch
            {
                tempStrategy.PermanentTPType = PermanentProtectionType.Relative;
            }

            // Break Even
            try
            {
                tempStrategy.BreakEven = int.Parse(xmlDocStrategy.GetElementsByTagName("breakEven")[0].InnerText);
                tempStrategy.UseBreakEven = bool.Parse(xmlDocStrategy.GetElementsByTagName("breakEven")[0].Attributes["useBreakEven"].InnerText);
            }
            catch { }

            // Money Management
            tempStrategy.UseAccountPercentEntry = bool.Parse(xmlDocStrategy.GetElementsByTagName("useAcountPercentEntry")[0].InnerText);
            tempStrategy.MaxOpenLots  = StringToDouble(xmlDocStrategy.GetElementsByTagName("maxOpenLots")[0].InnerText);
            tempStrategy.EntryLots    = StringToDouble(xmlDocStrategy.GetElementsByTagName("entryLots")[0].InnerText);
            tempStrategy.AddingLots   = StringToDouble(xmlDocStrategy.GetElementsByTagName("addingLots")[0].InnerText);
            tempStrategy.ReducingLots = StringToDouble(xmlDocStrategy.GetElementsByTagName("reducingLots")[0].InnerText);

            // Description
            tempStrategy.Description = xmlDocStrategy.GetElementsByTagName("description")[0].InnerText;

            // Strategy name.
            tempStrategy.StrategyName = xmlDocStrategy.GetElementsByTagName("strategyName")[0].InnerText;

            // Reading the slots
            XmlNodeList xmlSlotList = xmlDocStrategy.GetElementsByTagName("slot");
            for (int slot = 0; slot < xmlSlotList.Count; slot++)
            {
                XmlNodeList xmlSlotTagList = xmlSlotList[slot].ChildNodes;

                SlotTypes slotType = (SlotTypes)Enum.Parse(typeof(SlotTypes), xmlSlotList[slot].Attributes["slotType"].InnerText);

                // Logical group
                if (slotType == SlotTypes.OpenFilter || slotType == SlotTypes.CloseFilter)
                {
                    XmlAttributeCollection attributes = xmlSlotList[slot].Attributes;
                    XmlNode nodeGroup = attributes.GetNamedItem("logicalGroup");
                    string defGroup = GetDefaultGroup(slotType, slot, tempStrategy.CloseSlot);
                    if (nodeGroup != null)
                    {
                        string group = nodeGroup.InnerText;
                        tempStrategy.Slot[slot].LogicalGroup = group;
                        if (group != defGroup && group.ToLower() != "all" && !Configs.UseLogicalGroups)
                        {
                            System.Windows.Forms.MessageBox.Show(
                                Language.T("The strategy requires logical groups.") + Environment.NewLine +
                                Language.T("\"Use Logical Groups\" option was temporarily switched on."),
                                Language.T("Logical Groups"),
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                            Configs.UseLogicalGroups = true;
                        }
                    }
                    else
                        tempStrategy.Slot[slot].LogicalGroup = defGroup;
                }

                // Indicator name.
                string    indicatorName = xmlSlotTagList[0].InnerText;
                Indicator indicator     = Indicator_Store.ConstructIndicator(indicatorName, slotType);

                for (int tag = 1; tag < xmlSlotTagList.Count; tag++)
                {
                    // List parameters
                    if (xmlSlotTagList[tag].Name == "listParam")
                    {
                        int listParam = int.Parse(xmlSlotTagList[tag].Attributes["paramNumber"].InnerText);
                        XmlNode xmlListParamNode = xmlSlotTagList[tag].FirstChild;

                        indicator.IndParam.ListParam[listParam].Caption = xmlListParamNode.InnerText;

                        xmlListParamNode = xmlListParamNode.NextSibling;
                        int index = int.Parse(xmlListParamNode.InnerText);
                        indicator.IndParam.ListParam[listParam].Index = index;
                        indicator.IndParam.ListParam[listParam].Text  = indicator.IndParam.ListParam[listParam].ItemList[index];
                    }

                    // Numeric parameters
                    if (xmlSlotTagList[tag].Name == "numParam")
                    {
                        XmlNode xmlNumParamNode = xmlSlotTagList[tag].FirstChild;
                        int numParam = int.Parse(xmlSlotTagList[tag].Attributes["paramNumber"].InnerText);
                        indicator.IndParam.NumParam[numParam].Caption = xmlNumParamNode.InnerText;

                        xmlNumParamNode = xmlNumParamNode.NextSibling;
                        string sNumParamValue = xmlNumParamNode.InnerText;
                        sNumParamValue = sNumParamValue.Replace(',', Data.PointChar);
                        sNumParamValue = sNumParamValue.Replace('.', Data.PointChar);
                        float fValue = float.Parse(sNumParamValue);

                        // Removing of the Stop Loss negative sign used in previous versions.
                        string sParCaption = indicator.IndParam.NumParam[numParam].Caption;
                        if (sParCaption == "Trailing Stop"     ||
                            sParCaption == "Initial Stop Loss" ||
                            sParCaption == "Stop Loss")
                            fValue = Math.Abs(fValue);
                        indicator.IndParam.NumParam[numParam].Value = fValue;
                    }

                    // Check parameters
                    if (xmlSlotTagList[tag].Name == "checkParam")
                    {
                        XmlNode xmlCheckParamNode = xmlSlotTagList[tag].FirstChild;
                        int checkParam = int.Parse(xmlSlotTagList[tag].Attributes["paramNumber"].InnerText);
                        indicator.IndParam.CheckParam[checkParam].Caption = xmlCheckParamNode.InnerText;

                        xmlCheckParamNode = xmlCheckParamNode.NextSibling;
                        indicator.IndParam.CheckParam[checkParam].Checked = bool.Parse(xmlCheckParamNode.InnerText);
                    }
                }

                // Calculate the indicator.
                indicator.Calculate(slotType);
                tempStrategy.Slot[slot].IndicatorName  = indicator.IndicatorName;
                tempStrategy.Slot[slot].IndParam       = indicator.IndParam;
                tempStrategy.Slot[slot].Component      = indicator.Component;
                tempStrategy.Slot[slot].SeparatedChart = indicator.SeparatedChart;
                tempStrategy.Slot[slot].SpecValue      = indicator.SpecialValues;
                tempStrategy.Slot[slot].MinValue       = indicator.SeparatedChartMinValue;
                tempStrategy.Slot[slot].MaxValue       = indicator.SeparatedChartMaxValue;
                tempStrategy.Slot[slot].IsDefined      = true;
            }

            return tempStrategy;
        }

        /// <summary>
        /// Converts a string to a double number.
        /// </summary>
        double StringToDouble(string input)
        {
            string decimalPoint = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            if (!input.Contains(decimalPoint))
            {
                input = input.Replace(".", decimalPoint);
                input = input.Replace(",", decimalPoint);
            }

            double number = double.Parse(input);

            return number;
        }

        /// <summary>
        /// Gets the default logical group of the slot.
        /// </summary>
        string GetDefaultGroup(SlotTypes slotType, int slotIndex, int closeSlotIndex)
        {
            string group = "";

            if (slotType == SlotTypes.OpenFilter)
            {
                group = "A";
            }
            if (slotType == SlotTypes.CloseFilter)
            {
                int index = slotIndex - closeSlotIndex - 1;
                group = char.ConvertFromUtf32(char.ConvertToUtf32("a", 0) + index);
            }

            return group;
        }
    }
}
