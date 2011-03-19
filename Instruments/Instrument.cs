// Instrument class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.IO;

namespace Forex_Strategy_Builder
{
    public class Instrument
    {
        Instrument_Properties instrProperties; // The instrument properties.
        int      period; // Represented as a number of minutes
        DateTime timeUpdate;  // Data update time
        int      bars; // Count of data bars
        Bar[]    aBar; // An array containing the data

        // Statistical information
        double instrMinPrice;
        double instrMaxPrice;
        int averageGap;
        int maxGap;
        int averageHighLow;
        int maxHighLow;
        int averageCloseOpen;
        int maxCloseOpen;
        int instrDaysOff;

        // Statistical information
        public double MinPrice         { get { return instrMinPrice; } }
        public double MaxPrice         { get { return instrMaxPrice; } }
        public int    MaxGap           { get { return maxGap; } }
        public int    DaysOff          { get { return instrDaysOff; } }
        public int    MaxHighLow       { get { return maxHighLow; } }
        public int    MaxCloseOpen     { get { return maxCloseOpen; } }
        public int    AverageGap       { get { return averageGap; } }
        public int    AverageHighLow   { get { return averageHighLow; } }
        public int    AverageCloseOpen { get { return averageCloseOpen; } }

        // Date format
        DateFormat dfDateFormat = DateFormat.Unknown;

        // Data directory
        string pathDataDir = "." + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar;

        bool isCut = false; // Whether the data have been cut

        /// <summary>
        /// Whether the data have been cut
        /// </summary>
        public bool Cut { get { return isCut; } set { isCut = value; } }


        // General instrument info
        public string   Symbol { get { return instrProperties.Symbol; } }
        public int      Period { get { return period; } }
        public int      Bars   { get { return bars; } }
        public double   Point  { get { return instrProperties.Point; } }
        public DateTime Update { get { return timeUpdate; } }

        // -------------------------------------------------------------
        int  maxBars    = 20000;
        int  startYear  = 1990;
        int  startMonth = 1;
        int  startDay   = 1;
        int  endYear    = 2020;
        int  endMonth   = 1;
        int  endDay     = 1;
        bool useStartDate = false;
        bool useEndDate   = false;

        /// <summary>
        /// Maximum data bars
        /// </summary>
        public int MaxBars { set { maxBars = value; } }
        /// <summary>
        /// Starting year
        /// </summary>
        public int StartYear { set { startYear = value; } }
        /// <summary>
        /// Starting month
        /// </summary>
        public int StartMonth { set { startMonth = value; } }
        /// <summary>
        /// Starting day
        /// </summary>
        public int StartDay { set { startDay = value; } }
        /// <summary>
        /// Ending year
        /// </summary>
        public int EndYear { set { endYear = value; } }
        /// <summary>
        /// Ending month
        /// </summary>
        public int EndMonth { set { endMonth = value; } }
        /// <summary>
        /// Ending day
        /// </summary>
        public int EndDay { set { endDay = value; } }
        /// <summary>
        /// Use end date
        /// </summary>
        public bool UseEndDate { get { return useEndDate; } set { useEndDate = value; } }
        /// <summary>
        /// Use start date
        /// </summary>
        public bool UseStartDate { get { return useStartDate; } set { useStartDate = value; } }
        // -------------------------------------------------------------

        // Bar info
        public DateTime Time    (int bar) { return aBar[bar].Time  ; }
        public double   Open    (int bar) { return aBar[bar].Open  ; }
        public double   High    (int bar) { return aBar[bar].High  ; }
        public double   Low     (int bar) { return aBar[bar].Low   ; }
        public double   Close   (int bar) { return aBar[bar].Close ; }
        public int      Volume (int bar) { return aBar[bar].Volume; }

        public DateFormat FormatDate { get { return dfDateFormat; } set { dfDateFormat = value; } }
        public string DataDir { get { return pathDataDir; } set { pathDataDir = value; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        Instrument()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Instrument(Instrument_Properties instrProperties, int period)
        {
            this.instrProperties = instrProperties;
            this.period = period;
        }

        /// <summary>
        /// Loads the data file
        /// </summary>
        /// <returns>0 - success</returns>
        public int LoadData()
        {
            // The source data file full name
            string sourceDataFile = pathDataDir + instrProperties.BaseFileName + period.ToString() + ".csv";

            // Checks the access to the file
            if (!File.Exists(sourceDataFile))
                return 1;

            StreamReader sr = new StreamReader(sourceDataFile);
            string sData = sr.ReadToEnd();
            sr.Close();

            Data_Parser dp = new Data_Parser(sData);

            int respond = dp.Parse();

            if (respond == 0)
            {
                aBar = dp.Bar;
                bars = dp.Bars;
                RefineData();
                DataHorizon();
                CheckMarketData();
                SetDataStats();
                timeUpdate = aBar[bars - 1].Time;
            }

            return respond;
        }

        /// <summary>
        /// Loads the data file
        /// </summary>
        /// <returns>0 - success</returns>
        public int LoadResourceData()
        {
            Data_Parser dataParser = new Data_Parser(Properties.Resources.EURUSD1440);
            int respond = dataParser.Parse();

            if (respond == 0)
            {
                aBar = dataParser.Bar;
                bars = dataParser.Bars;
                RefineData();
                DataHorizon();
                CheckMarketData();
                SetDataStats();
                timeUpdate = aBar[bars - 1].Time;
            }

            return respond;
        }

        /// <summary>
        /// Refines the market data
        /// </summary>
        void RefineData()
        {
            // Fill In data gaps
            if (Configs.FillInDataGaps)
            {
                for (int bar = 1; bar < Bars; bar++)
                {
                    aBar[bar].Open = aBar[bar - 1].Close;
                    if (aBar[bar].Open > aBar[bar].High || aBar[bar].Close > aBar[bar].High)
                        aBar[bar].High = aBar[bar].Open > aBar[bar].Close ? aBar[bar].Open : aBar[bar].Close;
                    if (aBar[bar].Open < aBar[bar].Low || aBar[bar].Close < aBar[bar].Low)
                        aBar[bar].Low = aBar[bar].Open < aBar[bar].Close ? aBar[bar].Open : aBar[bar].Close;
                }
            }

            // Cuts off the bad data
            if (Configs.CutBadData)
            {
                int maxConsecutiveBars = 0;
                int maxConsecutiveBar  = 0;
                int consecutiveBars    = 0;
                int lastBar            = 0;

                for (int bar = 0; bar < Bars; bar++)
                {
                    if (Math.Abs(aBar[bar].Open - aBar[bar].Close) < Data.InstrProperties.Point / 2)
                    {
                        if (lastBar == bar - 1 || lastBar == 0)
                        {
                            consecutiveBars++;
                            lastBar = bar;

                            if (consecutiveBars > maxConsecutiveBars)
                            {
                                maxConsecutiveBars = consecutiveBars;
                                maxConsecutiveBar  = bar;
                            }
                        }
                    }
                    else
                    {
                        consecutiveBars = 0;
                    }
                }

                if (maxConsecutiveBars < 10)
                    maxConsecutiveBar = 0;

                int firstBar = Math.Max(maxConsecutiveBar, 1);
                for (int bar = firstBar; bar < Bars; bar++)
                    if ((Time(bar) - Time(bar - 1)).Days > 5 && period < 10080)
                        firstBar = bar;

                if (firstBar == 1)
                    firstBar = 0;

                if (firstBar > 0)
                {
                    Bar[] aBarCopy = new Bar[bars];
                    aBar.CopyTo(aBarCopy, 0);

                    aBar = new Bar[bars - firstBar];
                    for (int bar = firstBar; bar < bars; bar++)
                        aBar[bar - firstBar] = aBarCopy[bar];

                    bars = bars - firstBar;
                    isCut  = true;
                }
            }

            return;
        }

        /// <summary>
        /// Data Horizon - Cuts some data
        /// </summary>
        int DataHorizon()
        {
            if (bars < Configs.MIN_BARS) return 0;

            int startBar = 0;
            int endBar   = bars - 1;
            DateTime startDate = new DateTime(startYear, startMonth, startDay);
            DateTime endDate   = new DateTime(endYear,   endMonth,   endDay);

            // Set the starting date
            if (useStartDate && aBar[0].Time < startDate)
            {
                for (int bar = 0; bar < bars; bar++)
                {
                    if (aBar[bar].Time >= startDate)
                    {
                        startBar  = bar;
                        break;
                    }
                }
            }

            // Set the end date
            if (useEndDate && aBar[bars - 1].Time > endDate)
            {   // We need to cut out the newest bars
                for (int bar = 0; bar < bars; bar++)
                {
                    if (aBar[bar].Time >= endDate)
                    {
                        endBar  = bar - 1;
                        break;
                    }
                }
            }

            if (endBar - startBar > maxBars - 1)
            {
                startBar = endBar - maxBars + 1;
            }

            if (endBar - startBar < Configs.MIN_BARS)
            {
                startBar = endBar - Configs.MIN_BARS + 1;
                if (startBar < 0)
                {
                    startBar = 0;
                    endBar = Configs.MIN_BARS - 1;
                }
            }

            // Cut the data
            if (startBar > 0 || endBar < bars - 1)
            {
                Bar[] aBarCopy = new Bar[bars];
                aBar.CopyTo(aBarCopy, 0);

                int newBars = endBar - startBar + 1;

                aBar = new Bar[newBars];
                for (int bar = startBar; bar <= endBar; bar++)
                    aBar[bar - startBar] = aBarCopy[bar];

                bars   = newBars;
                timeUpdate = aBar[newBars - 1].Time;
                isCut  = true;
            }

            return 0;
        }

        /// <summary>
        /// Checks the loaded data
        /// </summary>
        bool CheckMarketData()
        {
            for (int bar = 0; bar < Bars; bar++)
            {
                if (High(bar) < Open(bar)  ||
                    High(bar) < Low(bar)   ||
                    High(bar) < Close(bar) ||
                    Low(bar)  > Open(bar)  ||
                    Low(bar)  > Close(bar))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Calculate statistics for the loaded data.
        /// </summary>
        void SetDataStats()
        {
            instrMinPrice = double.MaxValue;
            instrMaxPrice = double.MinValue;
            double maxHighLowPrice   = double.MinValue;
            double maxCloseOpenPrice = double.MinValue;
            double sumHighLow   = 0;
            double sumCloseOpen = 0;
            instrDaysOff = 0;
            double sumGap = 0;
            double instrMaxGap = double.MinValue;
            double gap;

            for (int bar = 1; bar < Bars; bar++)
            {
                if (High(bar) > instrMaxPrice)
                    instrMaxPrice = High(bar);

                if (Low(bar) < instrMinPrice)
                    instrMinPrice = Low(bar);

                if (Math.Abs(High(bar) - Low(bar)) > maxHighLowPrice)
                    maxHighLowPrice = Math.Abs(High(bar) - Low(bar));
                sumHighLow += Math.Abs(High(bar) - Low(bar));

                if (Math.Abs(Close(bar) - Open(bar)) > maxCloseOpenPrice)
                    maxCloseOpenPrice = Math.Abs(Close(bar) - Open(bar));
                sumCloseOpen += Math.Abs(Close(bar) - Open(bar));

                int dayDiff = (Time(bar) - Time(bar - 1)).Days;
                if (instrDaysOff < dayDiff)
                    instrDaysOff = dayDiff;

                gap = Math.Abs(Open(bar) - Close(bar - 1));
                sumGap += gap;
                if (instrMaxGap < gap)
                    instrMaxGap = gap;
            }

            maxHighLow       = (int)(maxHighLowPrice / Point);
            averageHighLow   = (int)(sumHighLow / (Bars * Point));
            maxCloseOpen     = (int)(maxCloseOpenPrice / Point);
            averageCloseOpen = (int)(sumCloseOpen / (Bars * Point));
            maxGap           = (int)(instrMaxGap / Point);
            averageGap       = (int)(sumGap / ((Bars - 1) * Point));

            return;
        }
    }

    public enum DateFormat
    {
        DayMonthYear,
        MonthDayYear,
        YearDayMonth,
        YearMonthDay,
        Unknown
    }
}
