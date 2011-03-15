// Indicator_Store Class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.using System;

using System;
using System.Collections.Generic;

namespace Forex_Strategy_Builder
{
    public static class Indicator_Store
    {
        static Dictionary<string, Indicator> originalIndicators = new Dictionary<string, Indicator>();

        // Stores the custom indicators
        static SortableDictionary<string, Indicator> customIndicators = new SortableDictionary<string, Indicator>();

        // Stores all the indicators
        static SortableDictionary<string, Indicator> indicators = new SortableDictionary<string, Indicator>();

        // All the indicators arranged by slot types.
        static List<string> openPointIndicators   = new List<string>();
        static List<string> closePointIndicators  = new List<string>();
        static List<string> openFilterIndicators  = new List<string>();
        static List<string> closeFilterIndicators = new List<string>();

        // Stores all the closing Point indicators, which can be used with Closing Logic conditions
        static List<string> closingIndicatorsWithClosingFilters = new List<string>();

        /// <summary>
        /// Gets the names of all the original indicators
        /// </summary>
        public static List<string> OriginalIndicatorNames
        {
            get { return new List<string>(originalIndicators.Keys); }
        }

        /// <summary>
        /// Gets the names of all custom indicators
        /// </summary>
        public static List<string> CustomIndicatorNames
        {
            get { return new List<string>(customIndicators.Keys); }
        }

        /// <summary>
        /// Gets the names of all indicators.
        /// </summary>
        public static List<string> IndicatorNames
        {
            get { return new List<string>(indicators.Keys); }
        }

        /// <summary>
        /// Gets the names of all Opening Point indicators.
        /// </summary>
        public static List<string> OpenPointIndicators { get { return openPointIndicators; } }

        /// <summary>
        /// Gets the names of all Closing Point indicators.
        /// </summary>
        public static List<string> ClosePointIndicators { get { return closePointIndicators; } }

        /// <summary>
        /// Gets the names of all Opening Filter indicators.
        /// </summary>
        public static List<string> OpenFilterIndicators { get { return openFilterIndicators; } }

        /// <summary>
        /// Gets the names of all Closing Filter indicators.
        /// </summary>
        public static List<string> CloseFilterIndicators { get { return closeFilterIndicators; } }

        /// <summary>
        /// Gets the names of all losing Point indicators that allow use of Closing Filter indicators.
        /// </summary>
        public static List<string> ClosingIndicatorsWithClosingFilters { get { return closingIndicatorsWithClosingFilters; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        static Indicator_Store()
        {
            AddOriginalIndicators();

            foreach (KeyValuePair<string, Indicator> record in originalIndicators)
                indicators.Add(record.Key, record.Value);

            return;
        }

        /// <summary>
        /// Adds all indicators to the store.
        /// </summary>
        static void AddOriginalIndicators()
        {
            originalIndicators.Add("Accelerator Oscillator", new Accelerator_Oscillator(SlotTypes.NotDefined));
            originalIndicators.Add("Account Percent Stop", new Account_Percent_Stop(SlotTypes.NotDefined));
            originalIndicators.Add("Accumulation Distribution", new Accumulation_Distribution(SlotTypes.NotDefined));
            originalIndicators.Add("ADX", new ADX(SlotTypes.NotDefined));
            originalIndicators.Add("Alligator", new Alligator(SlotTypes.NotDefined));
            originalIndicators.Add("Aroon Histogram", new Aroon_Histogram(SlotTypes.NotDefined));
            originalIndicators.Add("ATR MA Oscillator", new ATR_MA_Oscillator(SlotTypes.NotDefined));
            originalIndicators.Add("ATR Stop", new ATR_Stop(SlotTypes.NotDefined));
            originalIndicators.Add("Average True Range", new Average_True_Range(SlotTypes.NotDefined));
            originalIndicators.Add("Awesome Oscillator", new Awesome_Oscillator(SlotTypes.NotDefined));
            originalIndicators.Add("Balance of Power", new Balance_of_Power(SlotTypes.NotDefined));
            originalIndicators.Add("Bar Closing", new Bar_Closing(SlotTypes.NotDefined));
            originalIndicators.Add("Bar Opening", new Bar_Opening(SlotTypes.NotDefined));
            originalIndicators.Add("Bar Range", new Bar_Range(SlotTypes.NotDefined));
            originalIndicators.Add("BBP MA Oscillator", new BBP_MA_Oscillator(SlotTypes.NotDefined));
            originalIndicators.Add("Bears Power", new Bears_Power(SlotTypes.NotDefined));
            originalIndicators.Add("Bollinger Bands", new Bollinger_Bands(SlotTypes.NotDefined));
            originalIndicators.Add("Bulls Bears Power", new Bulls_Bears_Power(SlotTypes.NotDefined));
            originalIndicators.Add("Bulls Power", new Bulls_Power(SlotTypes.NotDefined));
            originalIndicators.Add("CCI MA Oscillator", new CCI_MA_Oscillator(SlotTypes.NotDefined));
            originalIndicators.Add("Close and Reverse", new Close_and_Reverse(SlotTypes.NotDefined));
            originalIndicators.Add("Commodity Channel Index", new Commodity_Channel_Index(SlotTypes.NotDefined));
            originalIndicators.Add("Cumulative Sum", new Cumulative_Sum(SlotTypes.NotDefined));
            originalIndicators.Add("Data Bars Filter", new Data_Bars_Filter(SlotTypes.NotDefined));
            originalIndicators.Add("Date Filter", new Date_Filter(SlotTypes.NotDefined));
            originalIndicators.Add("Day Closing", new Day_Closing(SlotTypes.NotDefined));
            originalIndicators.Add("Day of Week", new Day_of_Week(SlotTypes.NotDefined));
            originalIndicators.Add("Day Opening", new Day_Opening(SlotTypes.NotDefined));
            originalIndicators.Add("DeMarker", new DeMarker(SlotTypes.NotDefined));
            originalIndicators.Add("Detrended Oscillator", new Detrended_Oscillator(SlotTypes.NotDefined));
            originalIndicators.Add("Directional Indicators", new Directional_Indicators(SlotTypes.NotDefined));
            originalIndicators.Add("Donchian Channel", new Donchian_Channel(SlotTypes.NotDefined));
            originalIndicators.Add("Ease of Movement", new Ease_of_Movement(SlotTypes.NotDefined));
            originalIndicators.Add("Enter Once", new Enter_Once(SlotTypes.NotDefined));
            originalIndicators.Add("Entry Hour", new Entry_Hour(SlotTypes.NotDefined));
            originalIndicators.Add("Entry Time", new Entry_Time(SlotTypes.NotDefined));
            originalIndicators.Add("Envelopes", new Envelopes(SlotTypes.NotDefined));
            originalIndicators.Add("Exit Hour", new Exit_Hour(SlotTypes.NotDefined));
            originalIndicators.Add("Fisher Transform", new Fisher_Transform(SlotTypes.NotDefined));
            originalIndicators.Add("Force Index", new Force_Index(SlotTypes.NotDefined));
            originalIndicators.Add("Fractal", new Fractal(SlotTypes.NotDefined));
            originalIndicators.Add("Heiken Ashi", new Heiken_Ashi(SlotTypes.NotDefined));
            originalIndicators.Add("Hourly High Low", new Hourly_High_Low(SlotTypes.NotDefined));
            originalIndicators.Add("Ichimoku Kinko Hyo", new Ichimoku_Kinko_Hyo(SlotTypes.NotDefined));
            originalIndicators.Add("Inside Bar", new Inside_Bar(SlotTypes.NotDefined));
            originalIndicators.Add("Keltner Channel", new Keltner_Channel(SlotTypes.NotDefined));
            originalIndicators.Add("Long or Short", new Long_or_Short(SlotTypes.NotDefined));
            originalIndicators.Add("Lot Limiter", new Lot_Limiter(SlotTypes.NotDefined));
            originalIndicators.Add("MA Oscillator", new MA_Oscillator(SlotTypes.NotDefined));
            originalIndicators.Add("MACD Histogram", new MACD_Histogram(SlotTypes.NotDefined));
            originalIndicators.Add("MACD", new MACD(SlotTypes.NotDefined));
            originalIndicators.Add("Market Facilitation Index", new Market_Facilitation_Index(SlotTypes.NotDefined));
            originalIndicators.Add("Momentum MA Oscillator", new Momentum_MA_Oscillator(SlotTypes.NotDefined));
            originalIndicators.Add("Momentum", new Momentum(SlotTypes.NotDefined));
            originalIndicators.Add("Money Flow Index", new Money_Flow_Index(SlotTypes.NotDefined));
            originalIndicators.Add("Money Flow", new Money_Flow(SlotTypes.NotDefined));
            originalIndicators.Add("Moving Average", new Moving_Average(SlotTypes.NotDefined));
            originalIndicators.Add("Moving Averages Crossover", new Moving_Averages_Crossover(SlotTypes.NotDefined));
            originalIndicators.Add("Narrow Range", new Narrow_Range(SlotTypes.NotDefined));
            originalIndicators.Add("OBOS MA Oscillator", new OBOS_MA_Oscillator(SlotTypes.NotDefined));
            originalIndicators.Add("On Balance Volume", new On_Balance_Volume(SlotTypes.NotDefined));
            originalIndicators.Add("Oscillator of ATR", new Oscillator_of_ATR(SlotTypes.NotDefined));
            originalIndicators.Add("Oscillator of BBP", new Oscillator_of_BBP(SlotTypes.NotDefined));
            originalIndicators.Add("Oscillator of CCI", new Oscillator_of_CCI(SlotTypes.NotDefined));
            originalIndicators.Add("Oscillator of MACD", new Oscillator_of_MACD(SlotTypes.NotDefined));
            originalIndicators.Add("Oscillator of Momentum", new Oscillator_of_Momentum(SlotTypes.NotDefined));
            originalIndicators.Add("Oscillator of OBOS", new Oscillator_of_OBOS(SlotTypes.NotDefined));
            originalIndicators.Add("Oscillator of ROC", new Oscillator_of_ROC(SlotTypes.NotDefined));
            originalIndicators.Add("Oscillator of RSI", new Oscillator_of_RSI(SlotTypes.NotDefined));
            originalIndicators.Add("Oscillator of Trix", new Oscillator_of_Trix(SlotTypes.NotDefined));
            originalIndicators.Add("Overbought Oversold Index", new Overbought_Oversold_Index(SlotTypes.NotDefined));
            originalIndicators.Add("Parabolic SAR", new Parabolic_SAR(SlotTypes.NotDefined));
            originalIndicators.Add("Percent Change", new Percent_Change(SlotTypes.NotDefined));
            originalIndicators.Add("Pivot Points", new Pivot_Points(SlotTypes.NotDefined));
            originalIndicators.Add("Previous Bar Closing", new Previous_Bar_Closing(SlotTypes.NotDefined));
            originalIndicators.Add("Previous Bar Opening", new Previous_Bar_Opening(SlotTypes.NotDefined));
            originalIndicators.Add("Previous High Low", new Previous_High_Low(SlotTypes.NotDefined));
            originalIndicators.Add("Price Move", new Price_Move(SlotTypes.NotDefined));
            originalIndicators.Add("Price Oscillator", new Price_Oscillator(SlotTypes.NotDefined));
            originalIndicators.Add("Random Filter", new Random_Filter(SlotTypes.NotDefined));
            originalIndicators.Add("Rate of Change", new Rate_of_Change(SlotTypes.NotDefined));
            originalIndicators.Add("Relative Vigor Index", new Relative_Vigor_Index(SlotTypes.NotDefined));
            originalIndicators.Add("ROC MA Oscillator", new ROC_MA_Oscillator(SlotTypes.NotDefined));
            originalIndicators.Add("Ross Hook", new Ross_Hook(SlotTypes.NotDefined));
            originalIndicators.Add("Round Number", new Round_Number(SlotTypes.NotDefined));
            originalIndicators.Add("RSI MA Oscillator", new RSI_MA_Oscillator(SlotTypes.NotDefined));
            originalIndicators.Add("RSI", new RSI(SlotTypes.NotDefined));
            originalIndicators.Add("Standard Deviation", new Standard_Deviation(SlotTypes.NotDefined));
            originalIndicators.Add("Starc Bands", new Starc_Bands(SlotTypes.NotDefined));
            originalIndicators.Add("Steady Bands", new Steady_Bands(SlotTypes.NotDefined));
            originalIndicators.Add("Stochastics", new Stochastics(SlotTypes.NotDefined));
            originalIndicators.Add("Stop Limit", new Stop_Limit(SlotTypes.NotDefined));
            originalIndicators.Add("Stop Loss", new Stop_Loss(SlotTypes.NotDefined));
            originalIndicators.Add("Take Profit", new Take_Profit(SlotTypes.NotDefined));
            originalIndicators.Add("Top Bottom Price", new Top_Bottom_Price(SlotTypes.NotDefined));
            originalIndicators.Add("Trailing Stop Limit", new Trailing_Stop_Limit(SlotTypes.NotDefined));
            originalIndicators.Add("Trailing Stop", new Trailing_Stop(SlotTypes.NotDefined));
            originalIndicators.Add("Trix Index", new Trix_Index(SlotTypes.NotDefined));
            originalIndicators.Add("Trix MA Oscillator", new Trix_MA_Oscillator(SlotTypes.NotDefined));
            originalIndicators.Add("Week Closing", new Week_Closing(SlotTypes.NotDefined));
            originalIndicators.Add("Williams' Percent Range", new Williams_Percent_Range(SlotTypes.NotDefined));
        }

        /// <summary>
        /// Gets the names of all indicators for a given slot type.
        /// </summary>
        public static List<string> GetIndicatorNames(SlotTypes slotType)
        {
            List<string> list = new List<string>();

            foreach (KeyValuePair<string, Indicator> record in indicators)
                if (record.Value.TestPossibleSlot(slotType))
                    list.Add(record.Value.IndicatorName);

            return list;
        }

        /// <summary>
        /// Gets the names of all indicators for a given slot type.
        /// </summary>
        public static List<string> ListIndicatorNames(SlotTypes slotType)
        {
            switch (slotType)
            {
                case SlotTypes.NotDefined:
                    break;
                case SlotTypes.Open:
                    return OpenPointIndicators;
                case SlotTypes.OpenFilter:
                    return OpenFilterIndicators;
                case SlotTypes.Close:
                    return ClosePointIndicators;
                case SlotTypes.CloseFilter:
                    return CloseFilterIndicators;
                default:
                    break;
            }

            return null;
        }

        /// <summary>
        /// Resets the custom indicators in the custom indicators list.
        /// </summary>
        public static void ResetCustomIndicators(List<Indicator> indicatorList)
        {
            customIndicators.Clear();

            if (indicatorList == null)
                return;

            foreach (Indicator indicator in indicatorList)
                if (!customIndicators.ContainsKey(indicator.IndicatorName))
                    customIndicators.Add(indicator.IndicatorName, indicator);

            customIndicators.Sort();

            return;
        }

        /// <summary>
        /// Clears the indicator list and adds to it the original indicators.
        /// </summary>
        public static void CombineAllIndicators()
        {
            indicators.Clear();

            foreach (KeyValuePair<string, Indicator> record in originalIndicators)
                if (!indicators.ContainsKey(record.Key))
                    indicators.Add(record.Key, record.Value);

            foreach (KeyValuePair<string, Indicator> record in customIndicators)
                if (!indicators.ContainsKey(record.Key))
                    indicators.Add(record.Key, record.Value);

            indicators.Sort();

            openPointIndicators   = GetIndicatorNames(SlotTypes.Open);
            closePointIndicators  = GetIndicatorNames(SlotTypes.Close);
            openFilterIndicators  = GetIndicatorNames(SlotTypes.OpenFilter);
            closeFilterIndicators = GetIndicatorNames(SlotTypes.CloseFilter);

            foreach (string indicatorName in closePointIndicators)
            {
                Indicator indicator = ConstructIndicator(indicatorName, SlotTypes.Close);
                if (indicator.AllowClosingFilters)
                    closingIndicatorsWithClosingFilters.Add(indicatorName);
            }

            return;
        }

        /// <summary>
        /// Constructs an indicator with specified name and slot type.
        /// </summary>
        public static Indicator ConstructIndicator(string indicatorName, SlotTypes slotType)
        {
            if (!indicators.ContainsKey(indicatorName))
            {
                System.Windows.Forms.MessageBox.Show("There is no indicator named: " + indicatorName);
                return null;
            }

            Type   indicatorType = indicators[indicatorName].GetType();
            Type[] parameterType = new Type[] { slotType.GetType() };
            System.Reflection.ConstructorInfo constructorInfo = indicatorType.GetConstructor(parameterType);
            Indicator indicator = (Indicator)constructorInfo.Invoke(new object[] { slotType });

            return indicator;
        }
    }
}
