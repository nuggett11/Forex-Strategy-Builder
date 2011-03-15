// Strategy Analyzer - Over_optimization_Data_Table class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System.Text;

namespace Forex_Strategy_Builder.Dialogs.Analyzer
{
    class Over_optimization_Data_Table
    {
        string name;
        int percentDeviationSteps;
        int percentDeviation;
        int countAllParams;
        int countStrategyParams = 0;
        double[,] data;

        string columnSeparator  = Configs.ColumnSeparator;
        string decimalSeparator = Configs.DecimalSeparator;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int CountDeviationSteps
        {
            get { return percentDeviationSteps; }
        }

        public int CountParams
        {
            get { return countAllParams; }
        }

        public int CountStrategyParams
        {
            get { return countStrategyParams; }
        }

        public Over_optimization_Data_Table(int percentDeviation, int countParam)
        {
            this.percentDeviation = percentDeviation;
            this.percentDeviationSteps = 2 * percentDeviation + 1;
            this.countAllParams = countParam;
            data = new double[percentDeviationSteps, countParam];
        }

        /// <summary>
        /// Sets data
        /// </summary>
        /// <param name="indexDeviation">-10, -9, ... ,0, +1, +2, ... +10</param>
        /// <param name="indexParam">Parameter number</param>
        /// <param name="data">The value to be stored</param>
        public void SetData(int indexDeviation, int indexParam, double data)
        {
            this.data[DevPosition(indexDeviation), indexParam] = data;
            if (countStrategyParams < indexParam + 1)
                countStrategyParams = indexParam + 1;
        }

        public double GetData(int indexDeviation, int parmIndex)
        {
            return data[DevPosition(indexDeviation), parmIndex];
        }

        public string DataToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(name);
            sb.AppendLine(Language.T("Deviation"));
            for (int p = 0; p < percentDeviationSteps; p++)
            {
                int index = percentDeviation - p;
                sb.Append(index + columnSeparator);
                for (int i = 0; i < countAllParams; i++)
                    sb.Append(NumberToString(GetData(index, i)) + columnSeparator);
                sb.AppendLine();
            }
            sb.Append(Language.T("Parameter") + columnSeparator);
            for (int i = 1; i <= countAllParams; i++)
                sb.Append(i.ToString() + columnSeparator);
            sb.AppendLine();
            sb.AppendLine();

            return sb.ToString();
        }

        int DevPosition(int indexDeviation)
        {
            return percentDeviation + indexDeviation;
        }

        string NumberToString(double value)
        {
            string strValue = value.ToString();
            strValue = strValue.Replace(".", decimalSeparator);
            strValue = strValue.Replace(",", decimalSeparator);
            return strValue;
        }
    }
}
