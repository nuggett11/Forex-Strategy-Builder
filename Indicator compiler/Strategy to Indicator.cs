// Export Strategy to Indicator Class
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
    public static class Strategy_to_Indicator
    {
        public static void ExportStrategyToIndicator()
        {
            StringBuilder sbLong  = new StringBuilder();
            StringBuilder sbShort = new StringBuilder();

            for (int iBar = Data.FirstBar; iBar < Data.Bars; iBar++)
            for (int iPos = 0; iPos < Backtester.Positions(iBar); iPos++)
            {
                if (Backtester.PosDir(iBar, iPos) == PosDirection.Long)
                    sbLong.AppendLine("				\"" + Data.Time[iBar].ToString() + "\",");

                if (Backtester.PosDir(iBar, iPos) == PosDirection.Short)
                    sbShort.AppendLine("				\"" + Data.Time[iBar].ToString() + "\",");
            }

            string strategy = Properties.Resources.StrategyToIndicator;
            strategy = strategy.Replace("#MODIFIED#",   DateTime.Now.ToString());
            strategy = strategy.Replace("#INSTRUMENT#", Data.Symbol);
            strategy = strategy.Replace("#BASEPERIOD#", Data.DataPeriodToString(Data.Period));
            strategy = strategy.Replace("#STARTDATE#",  Data.Time[Data.FirstBar].ToString());
            strategy = strategy.Replace("#ENDDATE#",    Data.Time[Data.Bars - 1].ToString());

            strategy = strategy.Replace("#PERIODMINUTES#", ((int)Data.Period).ToString());
            strategy = strategy.Replace("#LISTLONG#",  sbLong.ToString());
            strategy = strategy.Replace("#LISTSHORT#", sbShort.ToString());

            SaveFileDialog savedlg = new SaveFileDialog();
            savedlg.InitialDirectory = Data.SourceFolder;
            savedlg.AddExtension     = true;
            savedlg.Title            = Language.T("Custom Indicators");
            savedlg.Filter           = Language.T("Custom Indicators") + " (*.cs)|*.cs";

            if (savedlg.ShowDialog() == DialogResult.OK)
            {
                strategy = strategy.Replace("#INDICATORNAME#", Path.GetFileNameWithoutExtension(savedlg.FileName));
                StreamWriter sw = new StreamWriter(savedlg.FileName);
                try
                {
                    sw.Write(strategy);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, Language.T("Custom Indicators"));
                }
                finally
                {
                    sw.Close();
                }
            }

            return;
        }
    }
}
