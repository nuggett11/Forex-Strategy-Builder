// Data class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Data Periods.
    /// The value of each period is equal to its duration in minutes.
    /// </summary>
    public enum DataPeriods
    {
        min1  = 1,
        min5  = 5,
        min15 = 15,
        min30 = 30,
        hour1 = 60,
        hour4 = 240,
        day   = 1440,
        week  = 10080
    }

    // Size of the Slot panels
    public enum SlotSizeMinMidMax { min, mid, max };

    /// <summary>
    ///  Base class containing the data.
    /// </summary>
    public class Data
    {
        static bool isBetaVersion = true;
        static bool isReleaseCandidate = false;

        static string programVersion;
        static int    programID;
        static string programName           = "Forex Strategy Builder";
        static string offlineDataDir        = "Data"              + Path.DirectorySeparatorChar;
        static string defaultOfflineDataDir = "Data"              + Path.DirectorySeparatorChar;
        static string offlineDocsDir        = "Docs"              + Path.DirectorySeparatorChar;
        static string systemDir             = "System"            + Path.DirectorySeparatorChar;
        static string languageDir           = "Languages"         + Path.DirectorySeparatorChar;
        static string colorDir              = "Colors"            + Path.DirectorySeparatorChar;
        static string strategyDir           = "Strategies"        + Path.DirectorySeparatorChar;
        static string defaultStrategyDir    = "Strategies"        + Path.DirectorySeparatorChar;
        static string sourceFolder          = "Custom Indicators" + Path.DirectorySeparatorChar;
        static string programDir            = "";
        static string strategyName          = "New.xml";
        static string loadedSavedStrategy   = "";
        static string logFileName           = "Logfile.txt";
        static string dateFormat            = "dd.MM.yy";
        static string dateFormatShort       = "dd.MM";
        static char   decimalPoint          = '.';
        static int    firstBar              = 40;
        static bool   isAutoUsePrvBarValue  = true;
        static bool   isData                = false;
        static bool   isResult              = false;
        static bool   isStrategyReady       = true;
        static bool   isStrategyChanged     = false;
        static bool   toLog                 = false;
        static bool   isDebug               = false;
        static Icon   icon                  = Properties.Resources.Icon;

        static StreamWriter swLogFile = StreamWriter.Null;

        // Scanner colors
        static Dictionary<DataPeriods, Color> periodColor = new Dictionary<DataPeriods, Color>();

        static float horizontalDLU;
        static float verticalDLU;

        // The current strategy.
        static Strategy strategy;

        // The current instrument's properties.
        static Instrument_Properties  instrProperties;

        // Instrument info
        static DateTime    update; // Time of the last bar
        static int         bars;   // Count of bars
        static DataPeriods period; // Time frame

        // Bar info
        static DateTime[] time;  // The bar's time of opening
        static double[]   open;   // Price Open
        static double[]   high;   // Price High
        static double[]   low;    // Price Low
        static double[]   close;  // Price Close
        static int[]      volume; // Volume

        // Bar info
        public static DateTime[] Time   { get { return time;   } set { time   = value; } }
        public static double[]   Open   { get { return open;   } set { open   = value; } }
        public static double[]   High   { get { return high;   } set { high   = value; } }
        public static double[]   Low    { get { return low;    } set { low    = value; } }
        public static double[]   Close  { get { return close;  } set { close  = value; } }
        public static int[]      Volume { get { return volume; } set { volume = value; } }

        // Strategy Undo
        static Stack<Strategy> stckStrategy;
        static List<Strategy> lstGenStrategy;
        static int genHistoryIndex;

#region Intrabar Scanner
        static Bar[][] intraBarData;   // Keeps the data of all periods
        static int[]   aIntraBarBars;  // Number of intrabars of each data bar
        static int[]   aIntraBars;     // Number of bars of each data period
        static int     iLoadedIntraBarPeriods; // Number of intrabar periods that have been loaded
        static bool    isIntrabarData;
        static DataPeriods[]  aIntraBarsPeriods;     // The intrabar data period
        public static Bar[][] IntraBarData   { get { return intraBarData;   } set { intraBarData   = value; } }
        public static int[]   IntraBarBars   { get { return aIntraBarBars;  } set { aIntraBarBars  = value; } }
        public static bool    IsIntrabarData { get { return isIntrabarData; } set { isIntrabarData = value; } }
        public static int[]   IntraBars      { get { return aIntraBars;     } set { aIntraBars     = value; } }

        /// <summary>
        /// Number of intrabar periods that have been loaded
        /// </summary>
        public static int LoadedIntraBarPeriods { get { return iLoadedIntraBarPeriods; } set { iLoadedIntraBarPeriods = value; } }
        public static DataPeriods[] IntraBarsPeriods { get { return aIntraBarsPeriods; } set { aIntraBarsPeriods = value; } }

        // Tick data
        //static Dictionary<DateTime, double[]> tickData;
        //public static Dictionary<DateTime, double[]> TickData { get { return tickData; } set { tickData = value; } }
        static double[][] tickData;
        public static double[][] TickData { get { return tickData; } set { tickData = value; } }
        static bool isTickData;
        public static bool IsTickData { get { return isTickData; } set { isTickData = value; } }
        static long ticks;
        public static long Ticks { get { return ticks; } set { ticks = value; } }


        /// <summary>
        /// Calculates the Modelling Quality
        /// </summary>
        public static double ModellingQuality
        {
            get
            {
                if (!Backtester.IsScanPerformed)
                    return 0;

                int startGen = 0;

                for (int i = 0; i < Bars; i++)
                    if (IntraBarsPeriods[i] < Period)
                    {
                        startGen = i;
                        break;
                    }

                int startGenM1 = Bars - 1;

                for (int i = 0; i < Bars; i++)
                    if (IntraBarsPeriods[i] == DataPeriods.min1)
                    {
                        startGenM1 = i;
                        break;
                    }

                double modellingQuality = (0.25 * (startGen - FirstBar) + 0.5 * (startGenM1 - startGen) + 0.9 * (Bars - startGenM1)) / (Bars - FirstBar) * 100;

                return modellingQuality;
            }
        }

#endregion

#region Market statistics

        // Statistical information for the instrument data.
        static double instrMinPrice;
        static double instrMaxPrice;
        static int    averageGap;
        static int    maxGap;
        static int    averageHighLow;
        static int    maxHighLow;
        static int    averageCloseOpen;
        static int    maxCloseOpen;
        static int    instrDaysOff;
        static bool   isDataCut;

        // Statistical information for the instrument data
        public static double MinPrice          { get { return instrMinPrice;    } set { instrMinPrice     = value; } }
        public static double MaxPrice          { get { return instrMaxPrice;    } set { instrMaxPrice     = value; } }
        public static int    DaysOff           { get { return instrDaysOff;     } set { instrDaysOff      = value; } }
        public static int    AverageGap        { get { return averageGap;       } set { averageGap        = value; } }
        public static int    MaxGap            { get { return maxGap;           } set { maxGap            = value; } }
        public static int    AverageHighLow    { get { return averageHighLow;   } set { averageHighLow    = value; } }
        public static int    MaxHighLow        { get { return maxHighLow;       } set { maxHighLow        = value; } }
        public static int    AverageCloseOpen  { get { return averageCloseOpen; } set { averageCloseOpen  = value; } }
        public static int    MaxCloseOpen      { get { return maxCloseOpen;     } set { maxCloseOpen      = value; } }
        public static bool   DataCut           { get { return isDataCut;        } set { isDataCut         = value; } }

        static string[] marketStatsParam;
        static string[] marketStatsValue;
        static bool[]   marketStatsFlag;

        /// <summary>
        /// Initializes the stats names
        /// </summary>
        internal static void InitMarketStatistic()
        {
            marketStatsParam = new string[21]
            {
               Language.T("Symbol"),
               Language.T("Period"),
               Language.T("Number of bars"),
               Language.T("Date of updating"),
               Language.T("Time of updating"),
               Language.T("Date of beginning"),
               Language.T("Time of beginning"),
               Language.T("Minimum price"),
               Language.T("Maximum price"),
               Language.T("Average Gap"),
               Language.T("Maximum Gap"),
               Language.T("Average High-Low"),
               Language.T("Maximum High-Low"),
               Language.T("Average Close-Open"),
               Language.T("Maximum Close-Open"),
               Language.T("Maximum days off"),
               Language.T("Maximum data bars"),
               Language.T("No data older than"),
               Language.T("No data newer than"),
               Language.T("Fill In Data Gaps"),
               Language.T("Cut Off Bad Data"),
            };

            marketStatsValue = new string[21];
            marketStatsFlag  = new bool[21];
        }


        /// <summary>
        /// Generate the Market Statistics
        /// </summary>
        public static void GenerateMarketStats()
        {
                marketStatsValue[0]  = Data.Symbol.ToString();
                marketStatsValue[1]  = Data.DataPeriodToString(Data.Period);
                marketStatsValue[2]  = Data.Bars.ToString();
                marketStatsValue[3]  = Data.Update.ToString(dateFormat);
                marketStatsValue[4]  = Data.Update.ToString("HH:mm");
                marketStatsValue[5]  = Data.Time[0].ToString(dateFormat);
                marketStatsValue[6]  = Data.Time[0].ToString("HH:mm");
                marketStatsValue[7]  = Data.MinPrice.ToString();
                marketStatsValue[8]  = Data.MaxPrice.ToString();
                marketStatsValue[9]  = Data.AverageGap.ToString()       + " " + Language.T("pips");
                marketStatsValue[10] = Data.MaxGap.ToString()           + " " + Language.T("pips");
                marketStatsValue[11] = Data.AverageHighLow.ToString()   + " " + Language.T("pips");
                marketStatsValue[12] = Data.MaxHighLow.ToString()       + " " + Language.T("pips");
                marketStatsValue[13] = Data.AverageCloseOpen.ToString() + " " + Language.T("pips");
                marketStatsValue[14] = Data.MaxCloseOpen.ToString()     + " " + Language.T("pips");
                marketStatsValue[15] = Data.DaysOff.ToString();
                marketStatsValue[16] = Configs.MaxBars.ToString();
                marketStatsValue[17] = Configs.UseStartDate ?
                    (new DateTime(Configs.StartYear, Configs.StartMonth, Configs.StartDay)).ToShortDateString() :
                    Language.T("No limits");
                marketStatsValue[18] = Configs.UseEndDate ?
                    (new DateTime(Configs.EndYear, Configs.EndMonth, Configs.EndDay)).ToShortDateString() :
                    Language.T("No limits");
                marketStatsValue[19] = Configs.FillInDataGaps ? Language.T("Accomplished") : Language.T("Switched off");
                marketStatsValue[20] = Configs.CutBadData     ? Language.T("Accomplished") : Language.T("Switched off");
            return;
        }

        /// <summary>
        /// Gets the market stats parameters
        /// </summary>
        public static string[] MarketStatsParam { get { return marketStatsParam; } }

        /// <summary>
        /// Gets the market stats values
        /// </summary>
        public static string[] MarketStatsValue { get { return marketStatsValue; } }

        /// <summary>
        /// Gets the market stats flags
        /// </summary>
        public static bool[] MarketStatsFlag { get { return marketStatsFlag; } }

#endregion

        /// <summary>
        /// Gets the program name.
        /// </summary>
        public static string ProgramName { get { return programName; } }

        /// <summary>
        /// Programs icon.
        /// </summary>
        public static Icon Icon { get { return icon; } }

        /// <summary>
        /// Gets the program version.
        /// </summary>
        public static string ProgramVersion { get { return programVersion; } }

        /// <summary>
        /// Gets the program Beta state.
        /// </summary>
        public static bool IsProgramBeta { get { return isBetaVersion; } }

        /// <summary>
        /// Gets the program Release Candidate.
        /// </summary>
        public static bool IsProgramRC { get { return isReleaseCandidate; } }

        /// <summary>
        /// Gets the program ID
        /// </summary>
        public static int ProgramID { get { return programID; } }

        /// <summary>
        /// Gets the program current working directory.
        /// </summary>
        public static string ProgramDir { get { return programDir; } }

        /// <summary>
        /// Gets the path to System Dir.
        /// </summary>
        public static string SystemDir { get { return systemDir; } }

        /// <summary>
        /// Gets the path to LanguageDir Dir.
        /// </summary>
        public static string LanguageDir { get { return languageDir; } }

        /// <summary>
        /// Gets the path to Color Scheme Dir.
        /// </summary>
        public static string ColorDir { get { return colorDir; } }

        /// <summary>
        /// Gets or sets the data directory.
        /// </summary>
        public static string OfflineDataDir { get { return offlineDataDir; } set { offlineDataDir = value; } }

        /// <summary>
        /// Gets the default data directory.
        /// </summary>
        public static string DefaultOfflineDataDir { get { return defaultOfflineDataDir; } }

        /// <summary>
        /// Gets or sets the docs directory.
        /// </summary>
        public static string OfflineDocsDir { get { return offlineDocsDir; } set { offlineDocsDir = value; } }

        /// <summary>
        /// Gets the path to Default Strategy Dir.
        /// </summary>
        public static string DefaultStrategyDir { get { return defaultStrategyDir; } }

        /// <summary>
        /// Gets or sets the path to dir Strategy.
        /// </summary>
        public static string StrategyDir { get { return strategyDir; } set { strategyDir = value; } }

        /// <summary>
        /// Gets or sets the strategy name with extension.
        /// </summary>
        public static string StrategyName { get { return strategyName; } set { strategyName = value; } }

        /// <summary>
        /// Gets the current strategy full path.
        /// </summary>
        public static string StrategyPath { get { return Path.Combine(strategyDir, strategyName); } }

        /// <summary>
        /// Gets or sets the custom indicators folder
        /// </summary>
        public static string SourceFolder { get { return sourceFolder; } }

        /// <summary>
        /// Gets or sets the strategy name for Configs.LastStrategy
        /// </summary>
        public static string LoadedSavedStrategy { get { return loadedSavedStrategy; } set { loadedSavedStrategy = value; } }

        /// <summary>
        /// The current strategy.
        /// </summary>
        public static Strategy Strategy { get { return strategy; } set { strategy = value; } }

        /// <summary>
        /// The current instrument.
        /// </summary>
        public static Instrument_Properties InstrProperties { get { return instrProperties; } set { instrProperties = value; } }

        /// <summary>
        /// The current strategy undo
        /// </summary>
        public static Stack<Strategy> StackStrategy { get { return stckStrategy; } set { stckStrategy = value; } }

        /// <summary>
        /// The Generator History
        /// </summary>
        public static List<Strategy> GeneratorHistory { get { return lstGenStrategy; } set { lstGenStrategy = value; } }

        /// <summary>
        /// The Generator History current strategy
        /// </summary>
        public static int GenHistoryIndex { get { return genHistoryIndex; } set { genHistoryIndex = value; } }

        /// <summary>
        /// The scanner colors
        /// </summary>
        public static Dictionary<DataPeriods, Color> PeriodColor { get { return periodColor; } }

        /// <summary>
        /// Whether to log
        /// </summary>
        public static bool ToLog  { get { return toLog; } set { toLog = value; } }

        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool Debug { get { return isDebug; } set { isDebug = value; } }

        public static string Symbol { get { return instrProperties.Symbol; } }
        public static DataPeriods Period { get { return period; } set { period = value; } }
        public static string PeriodString { get { return DataPeriodToString(Period); } }
        public static DateTime Update { get { return update; } set { update = value; } }
        public static int Bars { get { return bars; } set { bars = value; } }

        public static bool IsData { get { return isData; } set { isData = value; } }
        public static bool IsResult { get { return isResult; } set { isResult = value; } }
        public static bool IsStrategyReady { get { return isStrategyReady; } set { isStrategyReady = value; } }
        public static bool IsStrategyChanged { get { return isStrategyChanged; } set { isStrategyChanged = value; } }
        public static int  FirstBar { get { return firstBar; } set { firstBar = value; } }

        /// <summary>
        /// Sets or gets value of the AutoUsePrvBarValue
        /// </summary>
        public static bool AutoUsePrvBarValue { get { return isAutoUsePrvBarValue; } set { isAutoUsePrvBarValue = value; } }

        /// <summary>
        /// Gets the value of one pip in currency
        /// </summary>
        public double PipsValue { get { return InstrProperties.Point * Data.instrProperties.LotSize; } }

        /// <summary>
        /// Gets the number format.
        /// </summary>
        public static string FF  { get { return "F" + instrProperties.Digits.ToString(); } }

        /// <summary>
        /// Gets the date format.
        /// </summary>
        public static string DF  { get { return dateFormat; } }

        /// <summary>
        /// Gets the short date format.
        /// </summary>
        public static string DFS { get { return dateFormatShort; } }

        /// <summary>
        /// Gets the point character
        /// </summary>
        public static char PointChar { get { return decimalPoint; } }

        /// <summary>
        /// Relative font height
        /// </summary>
        public static float VerticalDLU   { get { return verticalDLU; } set { verticalDLU = value; }  }

        /// <summary>
        /// Relative font width
        /// </summary>
        public static float HorizontalDLU { get { return horizontalDLU; } set { horizontalDLU = value; }  }

        /// <summary>
        /// The default constructor.
        /// </summary>
        static Data()
        {
            // Program's Major, Minor, Version and Build numbers must be <= 99.
            programVersion = Application.ProductVersion;
            string[] version = programVersion.Split('.');
            programID = 1000000 * int.Parse(version[0]) +
                          10000 * int.Parse(version[1]) +
                            100 * int.Parse(version[2]) +
                              1 * int.Parse(version[3]);

            Strategy.GenerateNew();
            stckStrategy   = new Stack<Strategy>();
            lstGenStrategy = new List<Strategy>();

            isIntrabarData = false;
        }

        /// <summary>
        /// Initial settings.
        /// </summary>
        public static void Start()
        {
            // Sets the date format.
            dateFormat = System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            if (dateFormat == "dd/MM yyyy") dateFormat = "dd/MM/yyyy"; // Fixes the Uzbek (Latin) issue
            dateFormat = dateFormat.Replace(" ",""); // Fixes the Slovenian issue
            char[]   acDS = System.Globalization.DateTimeFormatInfo.CurrentInfo.DateSeparator.ToCharArray();
            string[] asSS = dateFormat.Split(acDS, 3);
            asSS[0] = asSS[0].Substring(0, 1) + asSS[0].Substring(0, 1);
            asSS[1] = asSS[1].Substring(0, 1) + asSS[1].Substring(0, 1);
            asSS[2] = asSS[2].Substring(0, 1) + asSS[2].Substring(0, 1);
            dateFormat = asSS[0] + acDS[0].ToString() + asSS[1] + acDS[0].ToString() + asSS[2];

            if (asSS[0].ToUpper() == "YY")
                dateFormatShort = asSS[1] + acDS[0].ToString() + asSS[2];
            else if (asSS[1].ToUpper() == "YY")
                dateFormatShort = asSS[0] + acDS[0].ToString() + asSS[2];
            else
                dateFormatShort = asSS[0] + acDS[0].ToString() + asSS[1];

            // Point character
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            decimalPoint = cultureInfo.NumberFormat.NumberDecimalSeparator.ToCharArray()[0];

            // Set the working directories
            programDir            = Directory.GetCurrentDirectory();
            defaultOfflineDataDir = Path.Combine(programDir, offlineDataDir);
            offlineDataDir        = defaultOfflineDataDir;
            offlineDocsDir        = Path.Combine(programDir, offlineDocsDir);
            strategyDir           = Path.Combine(programDir, defaultStrategyDir);
            sourceFolder          = Path.Combine(programDir, sourceFolder);
            systemDir             = Path.Combine(programDir, systemDir);
            languageDir           = Path.Combine(systemDir,  languageDir);
            colorDir              = Path.Combine(systemDir,  colorDir);
            logFileName           = Path.Combine(programDir, logFileName);

            // Scanner colors
            periodColor.Add(DataPeriods.min1,  Color.Yellow);
            periodColor.Add(DataPeriods.min5,  Color.Lime);
            periodColor.Add(DataPeriods.min15, Color.Green);
            periodColor.Add(DataPeriods.min30, Color.Orange);
            periodColor.Add(DataPeriods.hour1, Color.DarkSalmon);
            periodColor.Add(DataPeriods.hour4, Color.Peru);
            periodColor.Add(DataPeriods.day,   Color.Red);
            periodColor.Add(DataPeriods.week,  Color.DarkViolet);

            // Create a new Log file
            if (toLog == true)
                swLogFile = new StreamWriter(logFileName, false);
        }

        // The names of the strategy indicators
        static string[] asStrategyIndicators;

        /// <summary>
        /// Sets the indicator names
        /// </summary>
        public static void SetStrategyIndicators()
        {
            asStrategyIndicators = new string[Data.Strategy.Slots];
            for (int i = 0; i < Data.Strategy.Slots; i++)
                asStrategyIndicators[i] = Data.Strategy.Slot[i].IndicatorName;
        }

        /// <summary>
        /// It tells if the strategy description is relevant.
        /// </summary>
        public static bool IsStrDescriptionRelevant()
        {
            bool isStrategyIndicatorsChanged = false;

            if (Strategy.Slots != asStrategyIndicators.Length)
                isStrategyIndicatorsChanged = true;

            if (isStrategyIndicatorsChanged == false)
            {
                for (int i = 0; i < Strategy.Slots; i++)
                    if (asStrategyIndicators[i] != Strategy.Slot[i].IndicatorName)
                        isStrategyIndicatorsChanged = true;
            }

            return !isStrategyIndicatorsChanged;
        }

        /// <summary>
        /// Saves a text line in the log file.
        /// </summary>
        public static void Log(string logLine)
        {
            if (toLog == true)
            {
                swLogFile.WriteLine(DateTime.Now.ToString() + "   " + logLine);
                swLogFile.Flush();
            }

            return;
        }

        /// <summary>
        /// Closes the log file.
        /// </summary>
        public static void CloseLogFile()
        {
            // Closes the log file
            if (toLog == true)
            {
                swLogFile.Flush();
                swLogFile.Close();
                toLog = false;
            }
        }

        /// <summary>
        /// Converts a data period from DataPeriods type to string.
        /// </summary>
        public static string DataPeriodToString(DataPeriods dataPeriod)
        {
            switch (dataPeriod)
            {
                case DataPeriods.min1 : return "1 "  + Language.T("Minute");
                case DataPeriods.min5 : return "5 "  + Language.T("Minutes");
                case DataPeriods.min15: return "15 " + Language.T("Minutes");
                case DataPeriods.min30: return "30 " + Language.T("Minutes");
                case DataPeriods.hour1: return "1 "  + Language.T("Hour");
                case DataPeriods.hour4: return "4 "  + Language.T("Hours");
                case DataPeriods.day  : return "1 "  + Language.T("Day");
                case DataPeriods.week : return "1 "  + Language.T("Week");
                default: return String.Empty;
            }
        }

        /// <summary>
        /// Paints a rectangle with gradient
        /// </summary>
        public static void GradientPaint(Graphics g, RectangleF rect, Color color, int depth)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            if (depth > 0 && Configs.GradientView)
            {
                Color color1 = GetGradientColor(color, +depth);
                Color color2 = GetGradientColor(color, -depth);
                RectangleF rect1 = new RectangleF(rect.X, rect.Y - 1, rect.Width, rect.Height + 2);
                LinearGradientBrush lgrdBrush = new LinearGradientBrush(rect1, color1, color2, 90);
                g.FillRectangle(lgrdBrush, rect);
            }
            else
            {
                g.FillRectangle(new SolidBrush(color), rect);
            }

            return;
        }

        /// <summary>
        /// Color change
        /// </summary>
        public static Color GetGradientColor(Color baseColor, int depth)
        {
            if (!Configs.GradientView)
                return baseColor;

            int r = Math.Max(Math.Min(baseColor.R + depth, 255), 0);
            int g = Math.Max(Math.Min(baseColor.G + depth, 255), 0);
            int b = Math.Max(Math.Min(baseColor.B + depth, 255), 0);

            return Color.FromArgb(r, g, b);
        }
    }
}
