// Data_Parser Class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Forex_Strategy_Builder
{
    public class Data_Parser
    {
        int    bars;
        Bar[]  aBar;
        string input;
        string parsingErrorMessage;
        string generalDataRowMatchPattern;
        string numberDecimalSeparator;
        string columnDelimiter;
        string timeMatchPattern;
        string dateSeparator;
        string dateMatchPattern;
        string priceMatchPattern;
        string dataRowMatchPattern;
        bool isSeconds;
        bool isVolumeColumn;
        bool isFileMatchPattern;

        /// <summary>
        /// Gets the count of the data bars
        /// </summary>
        public int Bars
        {
            get { return bars; }
        }

        /// <summary>
        /// Gets the the data array
        /// </summary>
        public Bar[] Bar
        {
            get { return aBar; }
        }

        /// <summary>
        /// Gets the parsing error message
        /// </summary>
        public string ParsingErrorMessage
        {
            get { return parsingErrorMessage; }
        }

        /// <summary>
        /// Gets or sets the general data row match pattern
        /// </summary>
        string GeneralDataRowMatchPattern
        {
            get { return generalDataRowMatchPattern; }
            set { generalDataRowMatchPattern = value; }
        }

        /// <summary>
        /// Gets or sets the number decimal separator
        /// </summary>
        string NumberDecimalSeparator
        {
            get { return numberDecimalSeparator; }
            set { numberDecimalSeparator = value; }
        }

        /// <summary>
        /// Gets or sets the column delimiter
        /// </summary>
        string ColumnDelimiter
        {
            get { return columnDelimiter; }
            set { columnDelimiter = value; }
        }

        /// <summary>
        /// Gets or sets the time match pattern
        /// </summary>
        string TimeMatchPattern
        {
            get { return timeMatchPattern; }
            set { timeMatchPattern = value; }
        }

        /// <summary>
        /// Gets or sets the date separator
        /// </summary>
        string DateSeparator
        {
            get { return dateSeparator; }
            set { dateSeparator = value; }
        }

        /// <summary>
        /// Gets or sets the date match pattern
        /// </summary>
        string DateMatchPattern
        {
            get { return dateMatchPattern; }
            set { dateMatchPattern = value; }
        }

        /// <summary>
        /// Gets or sets the price match pattern
        /// </summary>
        string PriceMatchPattern
        {
            get { return priceMatchPattern; }
            set { priceMatchPattern = value; }
        }

        /// <summary>
        /// Gets or sets the data row match pattern
        /// </summary>
        string DataRowMatchPattern
        {
            get { return dataRowMatchPattern; }
            set { dataRowMatchPattern = value; }
        }

        /// <summary>
        /// Gets or sets whether a seconds info present
        /// </summary>
        bool IsSeconds
        {
            get { return isSeconds; }
            set { isSeconds = value; }
        }

        /// <summary>
        /// Gets or sets whether a volume column present
        /// </summary>
        bool IsVolumeColumn
        {
            get { return isVolumeColumn; }
            set { isVolumeColumn = value; }
        }

        /// <summary>
        /// Gets or sets whether the file matches the pattern
        /// </summary>
        bool IsFileMatchPattern
        {
            get { return isFileMatchPattern; }
            set { isFileMatchPattern = value; }
        }

        /// <summary>
        /// Analyses and parses an input string
        /// </summary>
        /// <param name="sInput"></param>
        public Data_Parser(string sInput)
        {
            this.input = sInput;
        }

        /// <summary>
        /// Parses the input string
        /// </summary>
        public int Parse()
        {
            int respond = 0;

            try
            {
                respond = AnaliseInput();
            }
            catch (Exception e)
            {
                respond = -1;
                System.Windows.Forms.MessageBox.Show(
                    ParsingErrorMessage + Environment.NewLine + e.Message,
                    Language.T("Data File Loading"),
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }

            if (respond != 0)
            {
                System.Windows.Forms.MessageBox.Show(
                    ParsingErrorMessage,
                    Language.T("Data File Loading"),
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Exclamation);
                return respond;
            }

            try
            {
                respond = ParseInput();
            }
            catch (Exception e)
            {
                respond = -1;
                System.Windows.Forms.MessageBox.Show(
                    ParsingErrorMessage + Environment.NewLine + e.Message,
                    Language.T("Data File Loading"),
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                return respond;
            }

            if (respond != 0)
            {
                System.Windows.Forms.MessageBox.Show(
                    ParsingErrorMessage,
                    Language.T("Data File Loading"),
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Exclamation);
                return respond;
            }

            return respond;
        }

        /// <summary>
        /// Analyses the input file
        /// </summary>
        int AnaliseInput()
        {
            generalDataRowMatchPattern = @"^[\t ;,]*\d{1,4}[\./-]\d{1,4}[\./-]\d{1,4}[\t ;,]+\d{2}(:\d{2}){1,2}([\t ;,]+\d+([\.,]\d+)?){4}([\t ;,]+\d{1,10})?";
            Regex regexGeneralDataRowMatchPattern = new Regex(generalDataRowMatchPattern, RegexOptions.Compiled);

            // Takes a data line
            string dataLine = null;
            StringReader sr = new StringReader(input);
            while ((dataLine = sr.ReadLine()) != null)
            {
                if (regexGeneralDataRowMatchPattern.IsMatch(dataLine))
                    break;
            }
            sr.Close();

            if (dataLine == null)
            {
                parsingErrorMessage = Language.T("Could not recognize the data file format!");
                return -1;
            }

            // Number decimal separator
            if (Regex.IsMatch(dataLine, @"([\t, ;]+\d+\.\d+){4}"))
                NumberDecimalSeparator = @"\.";
            else if (Regex.IsMatch(dataLine, @"([\t ;]+\d+,\d+){4}"))
                NumberDecimalSeparator = @",";
            else
            {
                parsingErrorMessage = Language.T("Could not determine the number decimal separator!");
                return -1;
            }

            // Column delimiter
            if (NumberDecimalSeparator == @"\." || NumberDecimalSeparator == @"")
                ColumnDelimiter = @"[\t, ;]";
            else if (numberDecimalSeparator == @",")
                ColumnDelimiter = @"[\t ;]";

            // Time format
            if (Regex.IsMatch(dataLine, ColumnDelimiter + @"\d{2}:\d{2}" + ColumnDelimiter))
            {
                TimeMatchPattern = @"(?<hour>\d{2}):(?<min>\d{2})";
                IsSeconds = false;
            }
            else if (Regex.IsMatch(dataLine, ColumnDelimiter + @"\d{2}:\d{2}:\d{2}" + ColumnDelimiter))
            {
                TimeMatchPattern = @"(?<hour>\d{2}):(?<min>\d{2}):(?<sec>\d{2})";
                IsSeconds = true;
            }
            else
            {
                parsingErrorMessage = Language.T("Could not determine the time format!");
                return -1;
            }

            // Date separator
            if (Regex.IsMatch(dataLine, @"\d{1,4}\.\d{1,4}\.\d{1,4}" + ColumnDelimiter))
                DateSeparator = @"\.";
            else if (Regex.IsMatch(dataLine, @"\d{1,4}/\d{1,4}/\d{1,4}" + ColumnDelimiter))
                DateSeparator = @"/";
            else if (Regex.IsMatch(dataLine, @"\d{1,4}-\d{1,4}-\d{1,4}" + ColumnDelimiter))
                DateSeparator = @"-";
            else
            {
                parsingErrorMessage = Language.T("Could not determine the date separator!");
                return -1;
            }

            // Date format
            string line;
            int yearPos  = 0;
            int monthPos = 0;
            int dayPos   = 0;
            Regex regexGeneralDataPattern = new Regex(@"(?<1>\d{1,4})" + DateSeparator + @"(?<2>\d{1,4})" + DateSeparator + @"(?<3>\d{1,4})", RegexOptions.Compiled);
            sr = new StringReader(input);
            while ((line = sr.ReadLine()) != null)
            {
                Match matchDate = regexGeneralDataPattern.Match(line);

                if (!matchDate.Success)
                    continue;

                int date1 = int.Parse(matchDate.Result("$1"));
                int date2 = int.Parse(matchDate.Result("$2"));
                int date3 = int.Parse(matchDate.Result("$3"));

                // Determines the year index
                if (yearPos == 0)
                {
                    if (date1 > 31) yearPos = 1;
                    else if (date2 > 31) yearPos = 2;
                    else if (date3 > 31) yearPos = 3;
                }

                // Determines the day index
                if (dayPos == 0 && yearPos > 0)
                {
                    if (yearPos == 1)
                    {
                        if (date2 > 12) dayPos = 2;
                        else if (date3 > 12) dayPos = 3;
                    }
                    else if (yearPos == 2)
                    {
                        if (date1 > 12) dayPos = 1;
                        else if (date3 > 12) dayPos = 3;
                    }
                    else if (yearPos == 3)
                    {
                        if (date1 > 12) dayPos = 1;
                        else if (date2 > 12) dayPos = 2;
                    }
                }

                // Determines the month index
                if (dayPos > 0 && yearPos > 0)
                {
                    if (yearPos != 1 && dayPos != 1)
                        monthPos = 1;
                    else if (yearPos != 2 && dayPos != 2)
                        monthPos = 2;
                    else if (yearPos != 3 && dayPos != 3)
                        monthPos = 3;
                }

                if (yearPos > 0 && monthPos > 0 && dayPos > 0)
                    break;
            }
            sr.Close();

            // If the date format is not recognized we try to find the number of changes
            if (yearPos == 0 || monthPos == 0 || dayPos == 0)
            {
                int dateOld1 = 0;
                int dateOld2 = 0;
                int dateOld3 = 0;

                int dateChanges1 = -1;
                int dateChanges2 = -1;
                int dateChanges3 = -1;

                sr = new StringReader(input);
                while ((line = sr.ReadLine()) != null)
                {
                    Match matchDate = regexGeneralDataPattern.Match(line);

                    if (!matchDate.Success)
                        continue;

                    int date1 = int.Parse(matchDate.Result("$1"));
                    int date2 = int.Parse(matchDate.Result("$2"));
                    int date3 = int.Parse(matchDate.Result("$3"));

                    if (date1 != dateOld1)
                    {   // date1 has changed
                        dateOld1 = date1;
                        dateChanges1++;
                    }

                    if (date2 != dateOld2)
                    {   // date2 has changed
                        dateOld2 = date2;
                        dateChanges2++;
                    }

                    if (date3 != dateOld3)
                    {   // date2 has changed
                        dateOld3 = date3;
                        dateChanges3++;
                    }
                }
                sr.Close();

                if (yearPos > 0)
                {   // The year position is known
                    if (yearPos == 1)
                    {
                        if (dateChanges3 > dateChanges2)
                        {
                            monthPos = 2;
                            dayPos   = 3;
                        }
                        else if (dateChanges2 > dateChanges3)
                        {
                            monthPos = 3;
                            dayPos   = 2;
                        }
                    }
                    else if (yearPos == 2)
                    {
                        if (dateChanges3 > dateChanges1)
                        {
                            monthPos = 1;
                            dayPos   = 3;
                        }
                        else if (dateChanges1 > dateChanges3)
                        {
                            monthPos = 3;
                            dayPos   = 1;
                        }
                    }
                    else if (yearPos == 3)
                    {
                        if (dateChanges2 > dateChanges1)
                        {
                            monthPos = 1;
                            dayPos   = 2;
                        }
                        else if (dateChanges1 > dateChanges2)
                        {
                            monthPos = 2;
                            dayPos   = 1;
                        }
                    }
                }
                else
                {   // The year position is unknown
                    if (dateChanges1 >= 0 && dateChanges2 > dateChanges1 && dateChanges3 > dateChanges2)
                    {
                        yearPos  = 1;
                        monthPos = 2;
                        dayPos   = 3;
                    }
                    else if (dateChanges1 >= 0 && dateChanges3 > dateChanges1 && dateChanges2 > dateChanges3)
                    {
                        yearPos  = 1;
                        monthPos = 3;
                        dayPos   = 2;
                    }
                    else if (dateChanges2 >= 0 && dateChanges1 > dateChanges2 && dateChanges3 > dateChanges1)
                    {
                        yearPos  = 2;
                        monthPos = 1;
                        dayPos   = 3;
                    }
                    else if (dateChanges2 >= 0 && dateChanges3 > dateChanges2 && dateChanges1 > dateChanges3)
                    {
                        yearPos  = 2;
                        monthPos = 3;
                        dayPos   = 1;
                    }
                    else if (dateChanges3 >= 0 && dateChanges1 > dateChanges3 && dateChanges2 > dateChanges1)
                    {
                        yearPos  = 3;
                        monthPos = 1;
                        dayPos   = 2;
                    }
                    else if (dateChanges3 >= 0 && dateChanges2 > dateChanges3 && dateChanges1 > dateChanges2)
                    {
                        yearPos  = 3;
                        monthPos = 2;
                        dayPos   = 1;
                    }
                }
            }

            if (yearPos * monthPos * dayPos > 0)
            {
                if (yearPos == 1 && monthPos == 2 && dayPos == 3)
                    DateMatchPattern = @"(?<year>\d{1,4})"  + DateSeparator + @"(?<month>\d{1,4})" + DateSeparator + @"(?<day>\d{1,4})";
                else if (yearPos == 1 && monthPos == 3 && dayPos == 2)
                    DateMatchPattern = @"(?<year>\d{1,4})"  + DateSeparator + @"(?<day>\d{1,4})"   + DateSeparator + @"(?<month>\d{1,4})";
                else if (yearPos == 2 && monthPos == 1 && dayPos == 3)
                    DateMatchPattern = @"(?<month>\d{1,4})" + DateSeparator + @"(?<year>\d{1,4})"  + DateSeparator + @"(?<day>\d{1,4})";
                else if (yearPos == 2 && monthPos == 3 && dayPos == 1)
                    DateMatchPattern = @"(?<day>\d{1,4})"   + DateSeparator + @"(?<year>\d{1,4})"  + DateSeparator + @"(?<month>\d{1,4})";
                else if (yearPos == 3 && monthPos == 1 && dayPos == 2)
                    DateMatchPattern = @"(?<month>\d{1,4})" + DateSeparator + @"(?<day>\d{1,4})"   + DateSeparator + @"(?<year>\d{1,4})";
                else if (yearPos == 3 && monthPos == 2 && dayPos == 1)
                    DateMatchPattern = @"(?<day>\d{1,4})"   + DateSeparator + @"(?<month>\d{1,4})" + DateSeparator + @"(?<year>\d{1,4})";
            }
            else
            {
                parsingErrorMessage = Language.T("Could not determine the date format!");
                return -1;
            }

            // Price match pattern
            PriceMatchPattern = "";
            string sCurrentNumberDecimalSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            char cCurrentNumberDecimalSeparator = sCurrentNumberDecimalSeparator.ToCharArray()[0];
            char cNumberDecimalSeparator = numberDecimalSeparator.ToCharArray()[numberDecimalSeparator.ToCharArray().Length - 1];
            sr = new StringReader(input);
            while ((line = sr.ReadLine()) != null)
            {
                if (!regexGeneralDataRowMatchPattern.IsMatch(line))
                    continue;

                Match mPrice = Regex.Match(line,
                    ColumnDelimiter + @"+(?<1>\d+" + NumberDecimalSeparator + @"\d+)" +
                    ColumnDelimiter + @"+(?<2>\d+" + NumberDecimalSeparator + @"\d+)" +
                    ColumnDelimiter + @"+(?<3>\d+" + NumberDecimalSeparator + @"\d+)" +
                    ColumnDelimiter + @"+(?<4>\d+" + NumberDecimalSeparator + @"\d+)");

                string sPrice1 = mPrice.Result("$1").Replace(cNumberDecimalSeparator, cCurrentNumberDecimalSeparator);
                string sPrice2 = mPrice.Result("$2").Replace(cNumberDecimalSeparator, cCurrentNumberDecimalSeparator);
                string sPrice3 = mPrice.Result("$3").Replace(cNumberDecimalSeparator, cCurrentNumberDecimalSeparator);
                string sPrice4 = mPrice.Result("$4").Replace(cNumberDecimalSeparator, cCurrentNumberDecimalSeparator);

                double dPrice1 = double.Parse(sPrice1);
                double dPrice2 = double.Parse(sPrice2);
                double dPrice3 = double.Parse(sPrice3);
                double dPrice4 = double.Parse(sPrice4);

                if (dPrice2 > dPrice1 + 0.00001 && dPrice2 > dPrice3 + 0.00001 && dPrice2 > dPrice4 + 0.00001 &&
                    dPrice3 < dPrice1 - 0.00001 && dPrice3 < dPrice2 - 0.00001 && dPrice3 < dPrice4 - 0.00001)
                {
                    PriceMatchPattern = @"(?<open>\d+" + NumberDecimalSeparator + @"\d+)" +
                        ColumnDelimiter + @"+(?<high>\d+"  + NumberDecimalSeparator + @"\d+)" +
                        ColumnDelimiter + @"+(?<low>\d+"   + NumberDecimalSeparator + @"\d+)" +
                        ColumnDelimiter + @"+(?<close>\d+" + NumberDecimalSeparator + @"\d+)";
                    break;
                }
                if (dPrice3 > dPrice1 + 0.00001 && dPrice3 > dPrice2 + 0.00001 && dPrice3 > dPrice4 + 0.00001 &&
                    dPrice2 < dPrice1 - 0.00001 && dPrice2 < dPrice3 - 0.00001 && dPrice2 < dPrice4 - 0.00001)
                {
                    PriceMatchPattern = @"(?<open>\d+" + NumberDecimalSeparator + @"\d+)" +
                        ColumnDelimiter + @"+(?<low>\d+"   + NumberDecimalSeparator + @"\d+)" +
                        ColumnDelimiter + @"+(?<high>\d+"  + NumberDecimalSeparator + @"\d+)" +
                        ColumnDelimiter + @"+(?<close>\d+" + NumberDecimalSeparator + @"\d+)";
                    break;
                }
            }
            sr.Close();

            if (PriceMatchPattern == "")
            {
                parsingErrorMessage = Language.T("Could not determine the price columns order!");
                return -1;
            }

            // Check for a volume column
            IsVolumeColumn = Regex.IsMatch(dataLine, PriceMatchPattern + ColumnDelimiter + @"+\d+" + ColumnDelimiter + "*$");

            DataRowMatchPattern = "^" + ColumnDelimiter + "*" + DateMatchPattern + ColumnDelimiter + "*" +
                TimeMatchPattern + ColumnDelimiter + "*" + PriceMatchPattern + ColumnDelimiter + "*" +
                (IsVolumeColumn ? @"(?<volume>\d+)" : "") + ColumnDelimiter + "*$";

            return 0;
        }

        /// <summary>
        /// Parses the input file
        /// </summary>
        int ParseInput()
        {
            bars = 0;
            StringReader sr;
            string line;
            Match  matchLine;
            string sCurrentNumberDecimalSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            char   cCurrentNumberDecimalSeparator = sCurrentNumberDecimalSeparator.ToCharArray()[0];
            char   cNumberDecimalSeparator   = numberDecimalSeparator.ToCharArray()[numberDecimalSeparator.ToCharArray().Length - 1];
            bool   isChangeDecSep = cNumberDecimalSeparator != cCurrentNumberDecimalSeparator;
            Regex  rx = new Regex(DataRowMatchPattern, RegexOptions.Compiled);

            // Counts the data bars
            bars = 0;
            sr = new StringReader(input);
            while ((line = sr.ReadLine()) != null)
                if (rx.IsMatch(line))
                    bars++;
            sr.Close();

            if (bars == 0)
            {
                parsingErrorMessage = Language.T("Could not count the data bars!");
                return -1;
            }

            int bar = 0;
            aBar = new Bar[bars];
            sr   = new StringReader(input);
            while ((line = sr.ReadLine()) != null)
            {
                matchLine = rx.Match(line);
                if (matchLine.Success)
                {
                    int year  = int.Parse(matchLine.Groups["year"].Value);
                    int month = int.Parse(matchLine.Groups["month"].Value);
                    int day   = int.Parse(matchLine.Groups["day"].Value);
                    int hour  = int.Parse(matchLine.Groups["hour"].Value);
                    int min   = int.Parse(matchLine.Groups["min"].Value);
                    int sec   = (IsSeconds ? int.Parse(matchLine.Groups["sec"].Value) : 0);

                    if (year < 100)
                        year += 2000;
                    if (year > DateTime.Now.Year)
                        year -= 100;

                    if (isChangeDecSep)
                    {
                        aBar[bar].Time   = new DateTime(year, month, day, hour, min, sec);
                        aBar[bar].Open   = double.Parse(matchLine.Groups["open"].Value.Replace(cNumberDecimalSeparator, cCurrentNumberDecimalSeparator));
                        aBar[bar].High   = double.Parse(matchLine.Groups["high"].Value.Replace(cNumberDecimalSeparator, cCurrentNumberDecimalSeparator));
                        aBar[bar].Low    = double.Parse(matchLine.Groups["low"].Value.Replace(cNumberDecimalSeparator, cCurrentNumberDecimalSeparator));
                        aBar[bar].Close  = double.Parse(matchLine.Groups["close"].Value.Replace(cNumberDecimalSeparator, cCurrentNumberDecimalSeparator));
                        aBar[bar].Volume = (IsVolumeColumn ? int.Parse(matchLine.Groups["volume"].Value) : 0);
                    }
                    else
                    {
                        aBar[bar].Time   = new DateTime(year, month, day, hour, min, sec);
                        aBar[bar].Open   = double.Parse(matchLine.Groups["open"].Value);
                        aBar[bar].High   = double.Parse(matchLine.Groups["high"].Value);
                        aBar[bar].Low    = double.Parse(matchLine.Groups["low"].Value);
                        aBar[bar].Close  = double.Parse(matchLine.Groups["close"].Value);
                        aBar[bar].Volume = (IsVolumeColumn ? int.Parse(matchLine.Groups["volume"].Value) : 0);
                    }

                    bar++;
                }
            }

            sr.Close();

            return 0;
        }
    }
}
