// Instruments class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Forex_Strategy_Builder
{
    public static class Instruments
    {
        static string pathToInstrumentsFile;
        static XmlDocument xmlInstruments;
        static Dictionary<String, Instrument_Properties> dictInstrument;
        static bool isReset = false;

        /// <summary>
        /// Gets the symbols list.
        /// </summary>
        public static string[] SymbolList
        {
            get
            {
                string[] asSymbols = new string[dictInstrument.Count];
                dictInstrument.Keys.CopyTo(asSymbols, 0);
                return asSymbols;
            }
        }

        /// <summary>
        /// Gets or sets the instruments list.
        /// </summary>
        public static Dictionary<String, Instrument_Properties> InstrumentList
        {
            get { return dictInstrument; }
            set { dictInstrument = value; }
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        static Instruments()
        {
            xmlInstruments = new XmlDocument();
            pathToInstrumentsFile = Data.ProgramDir + Path.DirectorySeparatorChar + "System" + Path.DirectorySeparatorChar + "instruments.xml";
        }

        /// <summary>
        /// Loads the instruments file.
        /// </summary>
        public static void LoadInstruments()
        {
            try
            {
                xmlInstruments.Load(pathToInstrumentsFile);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Load Instruments");
                xmlInstruments.LoadXml(Properties.Resources.instruments);
            }

            try
            {
                ParseInstruments();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Parse Instruments");
            }
        }

        /// <summary>
        /// Resets the instruments file.
        /// </summary>
        public static void ResetInstruments()
        {
            xmlInstruments.LoadXml(Properties.Resources.instruments);
            ParseInstruments();
            SaveInstruments();
            isReset = true;

            return;
        }

        /// <summary>
        /// Saves the config file.
        /// </summary>
        public static void SaveInstruments()
        {
            if (isReset) return;

            try
            {
                GenerateXMLFile().Save(pathToInstrumentsFile);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Save Instruments");
            }
        }

        /// <summary>
        /// Parses the instruments file.
        /// </summary>
        static void ParseInstruments()
        {
            int instrumentsCount = xmlInstruments.GetElementsByTagName("instrument").Count;
            dictInstrument = new Dictionary<string, Instrument_Properties>(instrumentsCount);

            try
            {
                foreach (XmlNode nodeInstr in xmlInstruments.GetElementsByTagName("instrument"))
                {
                    string symbol = nodeInstr.SelectSingleNode("symbol").InnerText;
                    Instrumet_Type instrType = (Instrumet_Type)Enum.Parse(typeof(Instrumet_Type), nodeInstr.SelectSingleNode("instrumentType").InnerText);
                    Instrument_Properties instrProp = new Instrument_Properties(symbol, instrType);
                    instrProp.Comment         = nodeInstr.SelectSingleNode("comment").InnerText;
                    instrProp.Digits          = int.Parse(nodeInstr.SelectSingleNode("digits").InnerText);
                    instrProp.LotSize         = int.Parse(nodeInstr.SelectSingleNode("contractSize").InnerText);
                    instrProp.Spread          = StringToFloat(nodeInstr.SelectSingleNode("spread").InnerText);
                    instrProp.SwapType        = (Commission_Type)Enum.Parse(typeof(Commission_Type), nodeInstr.SelectSingleNode("swapType").InnerText);
                    instrProp.SwapLong        = StringToFloat(nodeInstr.SelectSingleNode("swapLong").InnerText);
                    instrProp.SwapShort       = StringToFloat(nodeInstr.SelectSingleNode("swapShort").InnerText);
                    instrProp.CommissionType  = (Commission_Type)Enum.Parse(typeof(Commission_Type), nodeInstr.SelectSingleNode("commissionType").InnerText);
                    instrProp.CommissionScope = (Commission_Scope)Enum.Parse(typeof(Commission_Scope), nodeInstr.SelectSingleNode("commissionScope").InnerText);
                    instrProp.CommissionTime  = (Commission_Time)Enum.Parse(typeof(Commission_Time), nodeInstr.SelectSingleNode("commissionTime").InnerText);
                    instrProp.Commission      = StringToFloat(nodeInstr.SelectSingleNode("commission").InnerText);
                    instrProp.Slippage        = int.Parse(nodeInstr.SelectSingleNode("slippage").InnerText);
                    instrProp.PriceIn         = nodeInstr.SelectSingleNode("priceIn").InnerText;
                    instrProp.RateToUSD       = StringToFloat(nodeInstr.SelectSingleNode("rateToUSD").InnerText);
                    instrProp.RateToEUR       = StringToFloat(nodeInstr.SelectSingleNode("rateToEUR").InnerText);
                    instrProp.BaseFileName    = nodeInstr.SelectSingleNode("baseFileName").InnerText;
                    dictInstrument.Add(symbol, instrProp);
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Parsing Instruments");
            }
        }

        /// <summary>
        /// Generates instrument.xml file.
        /// </summary>
        static XmlDocument GenerateXMLFile()
        {
            // Create the XmlDocument.
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<fsb></fsb>");

            //Create the XML declaration.
            XmlDeclaration xmldecl;
            xmldecl = xmlDoc.CreateXmlDeclaration("1.0", null, null);

            //Add new node to the document.
            XmlElement root = xmlDoc.DocumentElement;
            xmlDoc.InsertBefore(xmldecl, root);

            foreach( KeyValuePair<string, Instrument_Properties> kvp in dictInstrument )
            {
                Instrument_Properties instrProp = kvp.Value;

                // Creates an instrument element.
                XmlElement instrument = xmlDoc.CreateElement("instrument");

                XmlElement element;

                element = xmlDoc.CreateElement("symbol");
                element.InnerText = instrProp.Symbol;
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("instrumentType");
                element.InnerText = instrProp.InstrType.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("comment");
                element.InnerText = instrProp.Comment;
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("digits");
                element.InnerText = instrProp.Digits.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("contractSize");
                element.InnerText = instrProp.LotSize.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("spread");
                element.InnerText = instrProp.Spread.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("swapType");
                element.InnerText = instrProp.SwapType.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("swapLong");
                element.InnerText = instrProp.SwapLong.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("swapShort");
                element.InnerText = instrProp.SwapShort.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("commissionType");
                element.InnerText = instrProp.CommissionType.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("commissionScope");
                element.InnerText = instrProp.CommissionScope.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("commissionTime");
                element.InnerText = instrProp.CommissionTime.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("commission");
                element.InnerText = instrProp.Commission.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("slippage");
                element.InnerText = instrProp.Slippage.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("priceIn");
                element.InnerText = instrProp.PriceIn.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("rateToUSD");
                element.InnerText = instrProp.RateToUSD.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("rateToEUR");
                element.InnerText = instrProp.RateToEUR.ToString();
                instrument.AppendChild(element);

                element = xmlDoc.CreateElement("baseFileName");
                element.InnerText = instrProp.BaseFileName.ToString();
                instrument.AppendChild(element);

                xmlDoc.DocumentElement.AppendChild(instrument);
            }

            return xmlDoc;
        }

        /// <summary>
        /// Parses string values to float.
        /// </summary>
        static float StringToFloat(string input)
        {
            float  output = 0;
            string decimalSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            input = input.Replace(",", decimalSeparator);
            input = input.Replace(".", decimalSeparator);

            try
            {
                output = float.Parse(input);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Parsing Instruments");
            }

            return output;
        }
    }
}
