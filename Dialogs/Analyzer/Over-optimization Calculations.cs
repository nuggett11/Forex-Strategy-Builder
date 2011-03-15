// Strategy Analyzer - OverOptimization Calculations
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.IO;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Analyzer
{
    public partial class OverOptimization : Fancy_Panel
    {
        int countStratParams;
        int deviationSteps;

        string listParametersName;
        Over_optimization_Data_Table tableParameters;
        Over_optimization_Data_Table[] tableReport;
        
        string pathReportFile;

        int cycles;          // Count of the cycles.
        int computedCycles;  // Currently completed cycles.
        int progressPercent; // Reached progress in %.
        int tablesCount;     // The count of data tables.

        /// <summary>
        /// Sets table with parameter values.
        /// </summary>
        void SetParametersValues(int percentDeviation, int countParam)
        {
            listParametersName = "Index" + Configs.ColumnSeparator + "Parameter name" + Environment.NewLine;
            countStratParams = 0;
            cycles = 0;
            deviationSteps = 2 * percentDeviation + 1;

            tableParameters = new Over_optimization_Data_Table(percentDeviation, countParam);
            tableParameters.Name = "Values of the Parameters";

            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
                for (int numParam = 0; numParam < 6; numParam++)
                    if (Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled && countStratParams < countParam)
                    {
                        NumericParam currentParam = Data.Strategy.Slot[slot].IndParam.NumParam[numParam];
                        double minVal = currentParam.Min;
                        double maxVal = currentParam.Max;
                        int    point  = currentParam.Point;
                        double originalValue = currentParam.Value;
                        double deltaStep     = originalValue / 100.0;

                        for (int p = 0; p < deviationSteps; p++)
                        {
                            int    index = percentDeviation - p;
                            double value = originalValue + index * deltaStep;
                            value = Math.Round(value, point);

                            if (index == 0)
                                value = originalValue;
                            if (value < minVal)
                                value = minVal;
                            if (value > maxVal)
                                value = maxVal;

                            tableParameters.SetData(index, countStratParams, value);
                            cycles++;
                        }

                        listParametersName += (countStratParams + 1).ToString() + Configs.ColumnSeparator + currentParam.Caption + Environment.NewLine;
                        countStratParams++;
                    }

            for (int prm = countStratParams; prm < countParam; prm++)
                listParametersName += (prm + 1).ToString() + Environment.NewLine;
            listParametersName += Environment.NewLine;
        }

        /// <summary>
        /// Calculates Data Tables.
        /// </summary>
        void CalculateStatsTables(int percentDeviation, int countParam)
        {
            string unit = " " + Configs.AccountCurrency;

            string[] tableNames = new string[]
            {
                Language.T("Account balance")     + unit,
                Language.T("Profit per day")      + unit,
                Language.T("Maximum drawdown")    + unit,
                Language.T("Gross profit")        + unit,
                Language.T("Gross loss")          + unit,
                Language.T("Executed orders"),
                Language.T("Traded lots"),
                Language.T("Time in position")    + " %",
                Language.T("Sent orders"),
                Language.T("Charged spread")      + unit,
                Language.T("Charged rollover")    + unit,
                Language.T("Winning trades"),
                Language.T("Losing trades"),
                Language.T("Win/loss ratio"),
                Language.T("Max equity drawdown") + " %"
            };

            tablesCount = tableNames.Length;
            tableReport = new Over_optimization_Data_Table[tablesCount];

            for (int t = 0; t < tableNames.Length; t++)
            {
                tableReport[t] = new Over_optimization_Data_Table(percentDeviation, countParam);
                tableReport[t].Name = tableNames[t];
            }

            int parNumber = 0;
            bool isBGWorkCanceled = false;
            for (int slot = 0; slot < Data.Strategy.Slots && !isBGWorkCanceled; slot++)
                for (int numParam = 0; numParam < 6 && !isBGWorkCanceled; numParam++)
                    if (Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Enabled && parNumber < countParam)
                    {
                        for (int index = percentDeviation; index >= -percentDeviation && !isBGWorkCanceled; index--)
                        {
                            isBGWorkCanceled = bgWorker.CancellationPending;
                            Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Value = tableParameters.GetData(index, parNumber);

                            CalculateIndicator(slot);
                            Backtester.Calculate();
                            Backtester.CalculateAccountStats();

                            double[] statValues = new double[] 
                            {
                                Backtester.NetMoneyBalance,
                                Backtester.MoneyProfitPerDay,
                                Backtester.MaxMoneyDrawdown,
                                Backtester.GrossMoneyProfit,
                                Backtester.GrossMoneyLoss,
                                Backtester.ExecutedOrders,
                                Backtester.TradedLots,
                                Backtester.TimeInPosition,
                                Backtester.SentOrders,
                                Backtester.TotalChargedMoneySpread,
                                Backtester.TotalChargedMoneyRollOver,
                                Backtester.WinningTrades,
                                Backtester.LosingTrades,
                                Backtester.WinLossRatio,
                                Backtester.MoneyEquityPercentDrawdown 
                            };

                            for (int tn = 0; tn < tablesCount; tn++)
                                tableReport[tn].SetData(index, parNumber, statValues[tn]);

                            // Report progress as a percentage of the total task.
                            computedCycles++;
                            int percentComplete = Math.Min(100 * computedCycles / cycles, 100);
                            if (percentComplete > progressPercent)
                            {
                                progressPercent = percentComplete;
                                bgWorker.ReportProgress(percentComplete);
                            }
                        }

                        // Set default value
                        Data.Strategy.Slot[slot].IndParam.NumParam[numParam].Value = tableParameters.GetData(0, parNumber);
                        CalculateIndicator(slot);
                        parNumber++;
                    }
        }

        /// <summary>
        /// Calculates the indicator in the designated slot.
        /// </summary>
        void CalculateIndicator(int slot)
        {
            IndicatorParam ip = Data.Strategy.Slot[slot].IndParam;

            Indicator indicator = Indicator_Store.ConstructIndicator(ip.IndicatorName, ip.SlotType);

            // List params
            for (int i = 0; i < 5; i++)
            {
                indicator.IndParam.ListParam[i].Index   = ip.ListParam[i].Index;
                indicator.IndParam.ListParam[i].Text    = ip.ListParam[i].Text;
                indicator.IndParam.ListParam[i].Enabled = ip.ListParam[i].Enabled;
            }

            // Numeric params
            for (int i = 0; i < 6; i++)
            {
                indicator.IndParam.NumParam[i].Value   = ip.NumParam[i].Value;
                indicator.IndParam.NumParam[i].Enabled = ip.NumParam[i].Enabled;
            }

            // Check params
            for (int i = 0; i < 2; i++)
            {
                indicator.IndParam.CheckParam[i].Checked = ip.CheckParam[i].Checked;
                indicator.IndParam.CheckParam[i].Enabled = ip.CheckParam[i].Enabled;
            }

            indicator.Calculate(ip.SlotType);

            // Sets Data.Strategy
            Data.Strategy.Slot[slot].IndicatorName  = indicator.IndicatorName;
            Data.Strategy.Slot[slot].IndParam       = indicator.IndParam;
            Data.Strategy.Slot[slot].Component      = indicator.Component;
            Data.Strategy.Slot[slot].SeparatedChart = indicator.SeparatedChart;
            Data.Strategy.Slot[slot].SpecValue      = indicator.SpecialValues;
            Data.Strategy.Slot[slot].MinValue       = indicator.SeparatedChartMinValue;
            Data.Strategy.Slot[slot].MaxValue       = indicator.SeparatedChartMaxValue;
            Data.Strategy.Slot[slot].IsDefined      = true;

            // Searches the indicators' components to determine the Data.FirstBar 
            Data.FirstBar = Data.Strategy.SetFirstBar();

            return;
        }

        /// <summary>
        /// Generates the Over-optimization report.
        /// </summary>
        string GenerateReport()
        {
            string report = listParametersName + tableParameters.DataToString();
            foreach (Over_optimization_Data_Table table in tableReport)
                report += table.DataToString();

            return report;
        }

        /// <summary>
        /// Saves the report in a file.
        /// </summary>
        void SaveReport(string report)
        {
            string pathReport = Data.StrategyPath.Replace(".xml", ".csv");
            string partilPath = Data.StrategyPath.Replace(".xml", "");
            int reportIndex = 0;
            do
            {
                reportIndex++;
                pathReport = partilPath + "-Over-optimization_Report-" + reportIndex.ToString() + ".csv";

            } while (File.Exists(pathReport));

            try
            {
                using (StreamWriter outfile = new StreamWriter(pathReport))
                {
                    outfile.Write(report);
                    pathReportFile = pathReport;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
