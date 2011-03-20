// Exporter class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    public class Exporter
    {
        string FF; // Format modifier to print the floats
        string sDF; // Format modifier to print the date
        StringBuilder sb; // The data file

        /// <summary>
        /// Default constructor
        /// </summary>
        public Exporter()
        {
            FF  = Data.FF; // Format modifier to print the floats
            sDF = Data.DF; // Format modifier to print the date
            sb = new StringBuilder();
       }

        /// <summary>
        /// Exports the data
        /// </summary>
        public void ExportCSVData()
        {
            for (int bar = 0; bar < Data.Bars; bar++)
            {
                sb.Append(Data.Time[bar].ToString(sDF)     + "\t");
                sb.Append(Data.Time[bar].ToString("HH:mm") + "\t");
                sb.Append(Data.Open[bar].ToString(FF)      + "\t");
                sb.Append(Data.High[bar].ToString(FF)      + "\t");
                sb.Append(Data.Low[bar].ToString(FF)       + "\t");
                sb.Append(Data.Close[bar].ToString(FF)     + "\t");
                sb.Append(Data.Volume[bar].ToString() + Environment.NewLine);
            }

            string fileName = Data.Symbol.ToString() + ((int)Data.Period).ToString() + ".csv";
            SaveData(fileName);
            return;
        }

        /// <summary>
        /// Exports the data
        /// </summary>
        public void ExportDataOnly()
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramRC)
                stage = " " + "RC";

            sb.Append("Forex Strategy Builder v" + Data.ProgramVersion + stage + Environment.NewLine);
            sb.Append("Strategy name: " + Data.Strategy.StrategyName + Environment.NewLine);
            sb.Append("Exported on " + DateTime.Now.ToString() + Environment.NewLine);
            sb.Append(Data.Symbol + " " +Data.PeriodString + Environment.NewLine);
            sb.Append("Number of bars: " + Data.Bars + Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("Date"   + "\t");
            sb.Append("Hour"   + "\t");
            sb.Append("Open"   + "\t");
            sb.Append("High"   + "\t");
            sb.Append("Low"    + "\t");
            sb.Append("Close"  + "\t");
            sb.Append("Volume" + Environment.NewLine);

            for (int bar = 0; bar < Data.Bars; bar++)
            {
                sb.Append(Data.Time[bar].ToString(sDF)     + "\t");
                sb.Append(Data.Time[bar].ToString("HH:mm") + "\t");
                sb.Append(Data.Open[bar].ToString(FF)      + "\t");
                sb.Append(Data.High[bar].ToString(FF)      + "\t");
                sb.Append(Data.Low[bar].ToString(FF)       + "\t");
                sb.Append(Data.Close[bar].ToString(FF)     + "\t");
                sb.Append(Data.Volume[bar].ToString() + Environment.NewLine);
            }

            string fileName = Data.Strategy.StrategyName + "-" + Data.Symbol.ToString() + "-" + Data.Period.ToString();
            SaveData(fileName);
            return;
        }

        /// <summary>
        /// Exports data and indicators values
        /// </summary>
        public void ExportIndicators()
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramRC)
                stage = " " + "RC";

            sb.Append("Forex Strategy Builder v" + Data.ProgramVersion + stage + Environment.NewLine);
            sb.Append("Strategy name: " + Data.Strategy.StrategyName + Environment.NewLine);
            sb.Append("Exported on " + DateTime.Now.ToString() + Environment.NewLine);
            sb.Append(Data.Symbol + " " +Data.PeriodString + Environment.NewLine);
            sb.Append("Number of bars: " + Data.Bars);

            sb.Append("\t\t\t\t\t\t\t\t");

            for (int iSlot = 0; iSlot < Data.Strategy.Slots; iSlot++)
            {
                string strSlotType = "";
                switch (Data.Strategy.Slot[iSlot].SlotType)
                {
                    case SlotTypes.Open:
                        strSlotType = "Opening point of the position";
                        break;
                    case SlotTypes.OpenFilter:
                        strSlotType = "Opening logic condition";
                        break;
                    case SlotTypes.Close:
                        strSlotType = "Closing point of the position";
                        break;
                    case SlotTypes.CloseFilter:
                        strSlotType = "Closing logic condition";
                        break;
                    default:
                        break;
                }

                sb.Append(strSlotType + "\t");
                foreach (IndicatorComp indComp in Data.Strategy.Slot[iSlot].Component)
                    sb.Append("\t");
            }
            sb.Append(Environment.NewLine);


            // Names of the indicator components
            sb.Append("\t\t\t\t\t\t\t\t");

            for (int iSlot = 0; iSlot < Data.Strategy.Slots; iSlot++)
            {
                Indicator indicator = Indicator_Store.ConstructIndicator(Data.Strategy.Slot[iSlot].IndicatorName,
                                                             Data.Strategy.Slot[iSlot].SlotType);

                sb.Append(indicator.ToString() + "\t");
                foreach (IndicatorComp indComp in Data.Strategy.Slot[iSlot].Component)
                    sb.Append("\t");
            }
            sb.Append(Environment.NewLine);

            // Data
            sb.Append("Date"   + "\t");
            sb.Append("Hour"   + "\t");
            sb.Append("Open"   + "\t");
            sb.Append("High"   + "\t");
            sb.Append("Low"    + "\t");
            sb.Append("Close"  + "\t");
            sb.Append("Volume" + "\t");

            for (int iSlot = 0; iSlot < Data.Strategy.Slots; iSlot++)
            {
                sb.Append("\t");
                foreach (IndicatorComp indComp in Data.Strategy.Slot[iSlot].Component)
                    sb.Append(indComp.CompName + "\t");
            }
            sb.Append(Environment.NewLine);

            for (int bar = 0; bar < Data.Bars; bar++)
            {
                sb.Append(Data.Time[bar].ToString(sDF)     + "\t");
                sb.Append(Data.Time[bar].ToString("HH:mm") + "\t");
                sb.Append(Data.Open[bar].ToString(FF)      + "\t");
                sb.Append(Data.High[bar].ToString(FF)      + "\t");
                sb.Append(Data.Low[bar].ToString(FF)       + "\t");
                sb.Append(Data.Close[bar].ToString(FF)     + "\t");
                sb.Append(Data.Volume[bar].ToString()      + "\t");

                for (int iSlot = 0; iSlot < Data.Strategy.Slots; iSlot++)
                {
                    sb.Append("\t");
                    foreach (IndicatorComp indComp in Data.Strategy.Slot[iSlot].Component)
                        sb.Append(indComp.Value[bar].ToString() + "\t");
                }
                sb.Append(Environment.NewLine);
            }

            string fileName = Data.Strategy.StrategyName + "-" + Data.Symbol.ToString() + "-" + Data.Period.ToString();
            SaveData(fileName);
            return;
        }

        /// <summary>
        /// Exports the bar summary
        /// </summary>
        public void ExportBarSummary()
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramRC)
                stage = " " + "RC";

            sb.Append("Forex Strategy Builder v" + Data.ProgramVersion + stage + Environment.NewLine);
            sb.Append("Strategy name: " + Data.Strategy.StrategyName + Environment.NewLine);
            sb.Append("Exported on " + DateTime.Now.ToString() + Environment.NewLine);
            sb.Append(Data.Symbol + " " + Data.PeriodString + "; Values in pips" + Environment.NewLine);

            sb.Append("Bar Numb\t");
            sb.Append("Date\t");
            sb.Append("Hour\t");
            sb.Append("Open\t");
            sb.Append("High\t");
            sb.Append("Low\t");
            sb.Append("Close\t");
            sb.Append("Volume\t");
            sb.Append("Direction\t");
            sb.Append("Lots\t");
            sb.Append("Transaction\t");
            sb.Append("Price\t");
            sb.Append("Profit Loss\t");
            sb.Append("Floating P/L\t");
            sb.Append("Spread\t");
            sb.Append("Rollover\t");
            sb.Append("Balance\t");
            sb.Append("Equity\t");
            sb.Append("Interpolation" + Environment.NewLine);

            for (int bar = 0; bar < Data.Bars; bar++)
            {
                sb.Append((bar + 1).ToString()             + "\t");
                sb.Append(Data.Time[bar].ToString(sDF)     + "\t");
                sb.Append(Data.Time[bar].ToString("HH:mm") + "\t");
                sb.Append(Data.Open[bar].ToString(FF)      + "\t");
                sb.Append(Data.High[bar].ToString(FF)      + "\t");
                sb.Append(Data.Low[bar].ToString(FF)       + "\t");
                sb.Append(Data.Close[bar].ToString(FF)     + "\t");
                sb.Append(Data.Volume[bar].ToString()      + "\t");
                if (Backtester.IsPos(bar))
                {
                    sb.Append(Backtester.SummaryDir(bar).ToString()     + "\t");
                    sb.Append(Backtester.SummaryLots(bar).ToString()    + "\t");
                    sb.Append(Backtester.SummaryTrans(bar).ToString()   + "\t");
                    sb.Append(Backtester.SummaryPrice(bar).ToString(FF) + "\t");
                    sb.Append(Backtester.ProfitLoss(bar).ToString()     + "\t");
                    sb.Append(Backtester.FloatingPL(bar).ToString()     + "\t");
                }
                else
                {
                    sb.Append("\t\t\t\t\t\t");
                }
                sb.Append(Backtester.ChargedSpread(bar).ToString()   + "\t");
                sb.Append(Backtester.ChargedRollOver(bar).ToString() + "\t");
                sb.Append(Backtester.Balance(bar).ToString()         + "\t");
                sb.Append(Backtester.Equity(bar).ToString()          + "\t");
                sb.Append(Backtester.BackTestEval(bar)               + "\t");
                sb.Append(Environment.NewLine);
            }

            string fileName = Data.Strategy.StrategyName + "-" + Data.Symbol.ToString() + "-" + Data.Period.ToString();
            SaveData(fileName);
            return;
        }

        /// <summary>
        /// Exports the positions info
        /// </summary>
        public void ExportPositions()
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramRC)
                stage = " " + "RC";

            sb.Append("Forex Strategy Builder v" + Data.ProgramVersion + stage + Environment.NewLine);
            sb.Append("Strategy name: " + Data.Strategy.StrategyName + Environment.NewLine);
            sb.Append("Exported on " + DateTime.Now.ToString() + Environment.NewLine);
            sb.Append(Data.Symbol + " " + Data.PeriodString + "; Values in pips" + Environment.NewLine);

            sb.Append("Pos Numb\t");
            sb.Append("Bar Numb\t");
            sb.Append("Bar Opening Time\t");
            sb.Append("Direction\t");
            sb.Append("Lots\t");
            sb.Append("Transaction\t");
            sb.Append("Order Price\t");
            sb.Append("Average Price\t");
            sb.Append("Profit Loss\t");
            sb.Append("Floating P/L\t");
            sb.Append("Balance\t");
            sb.Append("Equity\t");
            sb.Append(Environment.NewLine);

            for (int iPos = 0; iPos < Backtester.PositionsTotal; iPos++)
            {
                Position position = Backtester.PosFromNumb(iPos);
                int bar = Backtester.PosCoordinates[iPos].Bar;
                sb.Append((position.PosNumb + 1).ToString()            + "\t");
                sb.Append((bar + 1).ToString()                         + "\t");
                sb.Append((Data.Time[bar]).ToString()                  + "\t");
                sb.Append((position.PosDir).ToString()                 + "\t");
                sb.Append((position.PosLots).ToString()                + "\t");
                sb.Append((position.Transaction).ToString()            + "\t");
                sb.Append((position.FormOrdPrice).ToString(FF)         + "\t");
                sb.Append((position.PosPrice).ToString(FF)             + "\t");
                sb.Append((Math.Round(position.ProfitLoss)).ToString() + "\t");
                sb.Append((Math.Round(position.FloatingPL)).ToString() + "\t");
                sb.Append((Math.Round(position.Balance   )).ToString() + "\t");
                sb.Append((Math.Round(position.Equity    )).ToString() + "\t");
                sb.Append(Environment.NewLine);
            }

            string fileName = Data.Strategy.StrategyName + "-" + Data.Symbol.ToString() + "-" + Data.Period.ToString();
            SaveData(fileName);
            return;
        }

        /// <summary>
        /// Exports the positions info in currency
        /// </summary>
        public void ExportPositionsInMoney()
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");
            else if (Data.IsProgramRC)
                stage = " " + "RC";

            sb.Append("Forex Strategy Builder v" + Data.ProgramVersion + stage + Environment.NewLine);
            sb.Append("Strategy name: " + Data.Strategy.StrategyName + Environment.NewLine);
            sb.Append("Exported on " + DateTime.Now.ToString() + Environment.NewLine);
            sb.Append(Data.Symbol + " " + Data.PeriodString + "; Values in " + Configs.AccountCurrency + Environment.NewLine);

            sb.Append("Pos Numb\t");
            sb.Append("Bar Numb\t");
            sb.Append("Bar Opening Time\t");
            sb.Append("Direction\t");
            sb.Append("Amount\t");
            sb.Append("Transaction\t");
            sb.Append("Order Price\t");
            sb.Append("Price\t");
            sb.Append("Profit Loss\t");
            sb.Append("Floating P/L\t");
            sb.Append("Balance\t");
            sb.Append("Equity\t");
            sb.Append(Environment.NewLine);

            for (int iPos = 0; iPos < Backtester.PositionsTotal; iPos++)
            {
                Position position = Backtester.PosFromNumb(iPos);
                int bar = Backtester.PosCoordinates[iPos].Bar;
                sb.Append((position.PosNumb + 1).ToString() + "\t");
                sb.Append((bar + 1).ToString()              + "\t");
                sb.Append((Data.Time[bar]).ToString()       + "\t");
                sb.Append((position.PosDir).ToString()      + "\t");
                sb.Append((position.PosDir == PosDirection.Long ? "" : "-") +
                          (position.PosLots * Data.InstrProperties.LotSize).ToString() + "\t");
                sb.Append((position.Transaction    ).ToString()     + "\t");
                sb.Append((position.FormOrdPrice   ).ToString(FF)   + "\t");
                sb.Append((position.PosPrice       ).ToString(FF)   + "\t");
                sb.Append((position.MoneyProfitLoss).ToString("F2") + "\t");
                sb.Append((position.MoneyFloatingPL).ToString("F2") + "\t");
                sb.Append((position.MoneyBalance   ).ToString("F2") + "\t");
                sb.Append((position.MoneyEquity    ).ToString("F2") + "\t");
                sb.Append(Environment.NewLine);
            }

            string fileName =  Data.Strategy.StrategyName + "-" + Data.Symbol.ToString() + "-" + Data.Period.ToString();
            SaveData(fileName);
            return;
        }

        /// <summary>
        /// Saves the data file
        /// </summary>
        void SaveData(string fileName)
        {
            SaveFileDialog sfdExport = new SaveFileDialog();
            sfdExport.AddExtension = true;
            sfdExport.InitialDirectory = Data.ProgramDir;
            sfdExport.Title = Language.T("Export");
            sfdExport.FileName = fileName;
            if (fileName.EndsWith(".csv"))
            {
                sfdExport.InitialDirectory = Data.OfflineDataDir;
                sfdExport.Filter = "FSB data (*.csv)|*.csv|All files (*.*)|*.*";
            }
            else
                sfdExport.Filter = "Excel file (*.xls)|*.xls|FSB data (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (sfdExport.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamWriter sw = new StreamWriter(sfdExport.FileName);
                    sw.Write(sb.ToString());
                    sw.Close();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }

        }
    }
}
