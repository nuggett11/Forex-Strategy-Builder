// Indicator class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Indicator.
    /// </summary>
    public class Indicator
    {
        string    indicatorName;
        SlotTypes possibleSlots;
        bool      isSeparatedChart;
        double    sepChartMinValue;
        double    sepChartMaxValue;
        double[]  specialValues;
        bool      isDescreteValues;
        bool      isCustomIndicator;
        string    warningMessage;
        bool      allowClosingFilters;

        IndicatorParam  parameters;
        IndicatorComp[] component;

        string entryPointLongDescription   = "Not defined";
        string entryPointShortDescription  = "Not defined";
        string entryFilterLongDescription  = "Not defined";
        string entryFilterShortDescription = "Not defined";
        string exitPointLongDescription    = "Not defined";
        string exitPointShortDescription   = "Not defined";
        string exitFilterLongDescription   = "Not defined";
        string exitFilterShortDescription  = "Not defined";

        /// <summary>
        /// Gets or sets the indicator name.
        /// </summary>
        public string IndicatorName { get { return indicatorName; } set { indicatorName = value; } }

        /// <summary>
        /// Gets or sets the possible slots
        /// </summary>
        protected SlotTypes PossibleSlots { set { possibleSlots = value; } }

        /// <summary>
        /// Tests if this is one of the possible slots.
        /// </summary>
        /// <param name="slotType">The slot we test.</param>
        /// <returns>True if the slot is possible.</returns>
        public bool TestPossibleSlot(SlotTypes slotType)
        {
            if ((slotType & possibleSlots) == SlotTypes.Open)
                return true;

            if ((slotType & possibleSlots) == SlotTypes.OpenFilter)
                return true;

            if ((slotType & possibleSlots) == SlotTypes.Close)
                return true;

            if ((slotType & possibleSlots) == SlotTypes.CloseFilter)
                return true;

            return false;
        }

        /// <summary>
        /// Gets or sets the indicator current parameters.
        /// </summary>
        public IndicatorParam IndParam { get { return parameters; } set { parameters = value; } }

        /// <summary>
        /// If the chart is drown in separated panel.
        /// </summary>
        public bool SeparatedChart { get { return isSeparatedChart; } protected set { isSeparatedChart = value; } }

        /// <summary>
        /// Gets the indicator components.
        /// </summary>
        public IndicatorComp[] Component { get { return component; } protected set { component = value; } }

        /// <summary>
        /// Gets the indicator's special values.
        /// </summary>
        public double[] SpecialValues { get { return specialValues; } protected set { specialValues = value; } }

        /// <summary>
        /// Gets the indicator's min value.
        /// </summary>
        public double SeparatedChartMinValue { get { return sepChartMinValue; } protected set { sepChartMinValue = value; } }

        /// <summary>
        /// Gets the indicator's max value.
        /// </summary>
        public double SeparatedChartMaxValue { get { return sepChartMaxValue; } protected set { sepChartMaxValue = value; } }

        /// <summary>
        /// Shows if the indicator has descrete values.
        /// </summary>
        protected bool IsDescreteValues { set { isDescreteValues = value; } }

        /// <summary>
        /// Shows if the indicator is custom.
        /// </summary>
        public bool CustomIndicator { get { return isCustomIndicator; } protected set { isCustomIndicator = value; } }

        /// <summary>
        /// Gets or sets a warning message about the indicator
        /// </summary>
        public string WarningMessage { get { return warningMessage; } protected set { warningMessage = value; } }

        /// <summary>
        /// Shows if a closing point indicator can be used with closing logic conditions.
        /// </summary>
        public bool AllowClosingFilters { get { return allowClosingFilters; } protected set { allowClosingFilters = value; } }

        /// <summary>
        /// Gets the indicator Entry Point Long Description
        /// </summary>
        public string EntryPointLongDescription { get { return entryPointLongDescription; } protected set { entryPointLongDescription = value; } }

        /// <summary>
        /// Gets the indicator Entry Point Short Description
        /// </summary>
        public string EntryPointShortDescription { get { return entryPointShortDescription; } protected set { entryPointShortDescription = value; } }

        /// <summary>
        /// Gets the indicator Exit Point Long Description
        /// </summary>
        public string ExitPointLongDescription { get { return exitPointLongDescription; } protected set { exitPointLongDescription = value; } }

        /// <summary>
        /// Gets the indicator Exit Point Short Description
        /// </summary>
        public string ExitPointShortDescription { get { return exitPointShortDescription; } protected set { exitPointShortDescription = value; } }

        /// <summary>
        /// Gets the indicator Entry Filter Description
        /// </summary>
        public string EntryFilterLongDescription { get { return entryFilterLongDescription; } protected set { entryFilterLongDescription = value; } }

        /// <summary>
        /// Gets the indicator Exit Filter Description
        /// </summary>
        public string ExitFilterLongDescription { get { return exitFilterLongDescription; } protected set { exitFilterLongDescription = value; } }

        /// <summary>
        /// Gets the indicator Entry Filter Description
        /// </summary>
        public string EntryFilterShortDescription { get { return entryFilterShortDescription; } protected set { entryFilterShortDescription = value; } }

        /// <summary>
        /// Gets the indicator Exit Filter Description
        /// </summary>
        public string ExitFilterShortDescription { get { return exitFilterShortDescription; } protected set { exitFilterShortDescription = value; } }

        /// <summary>
        /// The default constructor
        /// </summary>
        public Indicator()
        {
            indicatorName       = string.Empty;
            possibleSlots       = SlotTypes.NotDefined;
            isSeparatedChart    = false;
            sepChartMinValue    = double.MaxValue;
            sepChartMaxValue    = double.MinValue;
            isDescreteValues    = false;
            isCustomIndicator   = false;
            warningMessage      = string.Empty;
            allowClosingFilters = false;
            specialValues       = new double[] { };
            parameters          = new IndicatorParam();
            component           = new IndicatorComp[] { };
        }

        /// <summary>
        /// Calculates the components
        /// </summary>
        public virtual void Calculate(SlotTypes slotType)
        {
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public virtual void SetDescription(SlotTypes slotType)
        {
        }

        /// <summary>
        /// Time frame of the loaded historical data
        /// </summary>
        protected static DataPeriods Period { get { return Data.Period; } }

        /// <summary>
        /// The minimal price change.
        /// </summary>
        protected static double Point { get { return Data.InstrProperties.Point; } }

        /// <summary>
        /// Number of digits after the decimal point of the historical data.
        /// </summary>
        protected static int Digits { get { return Data.InstrProperties.Digits; } }

        /// <summary>
        /// Number of loaded bars
        /// </summary>
        protected static int Bars { get { return Data.Bars; } }

        /// <summary>
        /// Obsolete! Use 'Time' instead.
        /// </summary>
        protected static DateTime[] Date { get { return Data.Time; } }

        /// <summary>
        /// Bar opening date and time
        /// </summary>
        protected static DateTime[] Time { get { return Data.Time; } }

        /// <summary>
        /// Bar opening price
        /// </summary>
        protected static double[] Open { get { return Data.Open; } }

        /// <summary>
        /// Bar highest price
        /// </summary>
        protected static double[] High { get { return Data.High; } }

        /// <summary>
        /// Bar lowest price
        /// </summary>
        protected static double[] Low { get { return Data.Low; } }

        /// <summary>
        /// Bar closing price
        /// </summary>
        protected static double[] Close { get { return Data.Close; } }

        /// <summary>
        /// Bar volume
        /// </summary>
        protected static int[] Volume { get { return Data.Volume; } }

        /// <summary>
        /// Calculates the base price.
        /// </summary>
        /// <param name="price">The base price type.</param>
        /// <returns>Base price.</returns>
        protected static double[] Price(BasePrice price)
		{
			double[] adPrice = new double[Bars];

			switch(price)
			{
				case BasePrice.Open:
					adPrice = Open;
					break;
				case BasePrice.High:
					adPrice = High;
					break;
				case BasePrice.Low:
					adPrice = Low;
					break;
				case BasePrice.Close:
					adPrice = Close;
					break;
				case BasePrice.Median:
                    for (int iBar = 0; iBar < Bars; iBar++)
                        adPrice[iBar] = (Low[iBar] + High[iBar]) / 2;
					break;
				case BasePrice.Typical:
                    for (int iBar = 0; iBar < Bars; iBar++)
                        adPrice[iBar] = (Low[iBar] + High[iBar] + Close[iBar]) / 3;
					break;
				case BasePrice.Weighted:
                    for (int iBar = 0; iBar < Bars; iBar++)
                        adPrice[iBar] = (Low[iBar] + High[iBar] + 2 * Close[iBar]) / 4;
					break;
				default:
					break;
			}
			return adPrice;
		}

        /// <summary>
        /// Sets the check box "Use previous bar value"
        /// </summary>
        protected bool PrepareUsePrevBarValueCheckBox(SlotTypes slotType)
        {
            return Data.Strategy.PrepareUsePrevBarValueCheckBox(slotType);
        }

        /// <summary>
        /// Calculates a Moving Average
        /// </summary>
        /// <param name="iPeriod">Period</param>
        /// <param name="iShift">Shift</param>
        /// <param name="maMethod">Method of calculation</param>
        /// <param name="afSource">The array of source data</param>
        /// <returns>the Moving Average</returns>
        protected static double[] MovingAverage(int iPeriod, int iShift, MAMethod maMethod, double[] adSource)
        {
            int      iBar;
            double   sum;
            double[] adTarget = new double[Bars];

            if (iPeriod <= 1 && iShift == 0)
            {   // There is no smoothing
                return adSource;
            }

            if (iPeriod > Bars || iPeriod + iShift <= 0 || iPeriod + iShift > Bars)
            {   // Error in the parameters
                return null;
            }

            for (iBar = 0; iBar < iPeriod + iShift - 1; iBar++)
            {
                adTarget[iBar] = 0;
            }

            for (iBar = 0, sum = 0; iBar < iPeriod; iBar++)
            {
                sum += adSource[iBar];
            }

            adTarget[iPeriod + iShift - 1] = sum / iPeriod;

            // Simple Moving Average
            if (maMethod == MAMethod.Simple)
            {
                for (iBar = iPeriod; iBar < Math.Min(Bars, Bars - iShift); iBar++)
                {
                    adTarget[iBar + iShift] = adTarget[iBar + iShift - 1] + adSource[iBar] / iPeriod - adSource[iBar - iPeriod] / iPeriod;
                }
            }

            // Exponential Moving Average
            else if (maMethod == MAMethod.Exponential)
            {
                double pr = 2d / (iPeriod + 1);

                for (iBar = iPeriod; iBar < Math.Min(Bars, Bars - iShift); iBar++)
                {
                    adTarget[iBar + iShift] = adSource[iBar] * pr + adTarget[iBar + iShift - 1] * (1 - pr);
                }
            }

            // Weighted Moving Average
            else if (maMethod == MAMethod.Weighted)
            {
                double dWeight = iPeriod * (iPeriod + 1) / 2d;

                for (iBar = iPeriod; iBar < Math.Min(Bars, Bars - iShift); iBar++)
                {
                    sum = 0;
                    for (int i = 0; i < iPeriod; i++)
                    {
                        sum += adSource[iBar - i] * (iPeriod - i);
                    }

                    adTarget[iBar + iShift] = sum / dWeight;
                }
            }

            // Smoothed Moving Average
            else if (maMethod == MAMethod.Smoothed)
            {
                for (iBar = iPeriod; iBar < Math.Min(Bars, Bars - iShift); iBar++)
                {
                    adTarget[iBar + iShift] = (adTarget[iBar + iShift - 1] * (iPeriod - 1) + adSource[iBar]) / iPeriod;
                }
            }

            for (iBar = Bars + iShift; iBar < Bars; iBar++)
            {
                adTarget[iBar] = 0;
            }

            return adTarget;
        }

        /// <summary>
        /// Maximum error for comparing indicator values
        /// </summary>
        protected double Sigma()
        {
            int iSigmaMode = isSeparatedChart ?
                Configs.SIGMA_MODE_SEPARATED_CHART : // Indicators plotted on its own chart (MACD, RSI, ADX, Momentum, ...)
                Configs.SIGMA_MODE_MAIN_CHART;       // Indicators plotted on the main chart (MA, Bollinger Bands, Alligator, ...)

            double dSigma;

            switch (iSigmaMode)
            {
                case 0:
                    dSigma = 0;
                    break;
                case 1:
                    dSigma = Data.InstrProperties.Point * 0.5;
                    break;
                case 2:
                    dSigma = Data.InstrProperties.Point * 0.05;
                    break;
                case 3:
                    dSigma = Data.InstrProperties.Point * 0.005;
                    break;
                case 4:
                    dSigma = 0.00005;
                    break;
                case 5:
                    dSigma = 0.000005;
                    break;
                case 6:
                    dSigma = 0.0000005;
                    break;
                case 7:
                    dSigma = 0.00000005;
                    break;
                case 8:
                    dSigma = 0.000000005;
                    break;
                default:
                    dSigma = 0;
                    break;
            }

            return dSigma;
        }

        /// <summary>
        /// Calculates the logic of an Oscillator.
        /// </summary>
        /// <param name="iFirstBar">The first bar number.</param>
        /// <param name="iPrvs">To use the previous bar or not.</param>
        /// <param name="adIndValue">The indicator values.</param>
        /// <param name="dLevelLong">The Level value for a Long position.</param>
        /// <param name="dLevelShort">The Level value for a Short position.</param>
        /// <param name="indCompLong">Indicator component for Long position.</param>
        /// <param name="indCompShort">Indicator component for Short position.</param>
        /// <param name="indLogic">The chosen logic.</param>
        /// <returns>True if everyting is ok.</returns>
        protected bool OscillatorLogic(int iFirstBar, int iPrvs, double[] adIndValue, double dLevelLong, double dLevelShort, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort, IndicatorLogic indLogic)
        {
            double dSigma = Sigma();

            switch (indLogic)
            {
                case IndicatorLogic.The_indicator_rises:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        int  iCurrBar  = iBar - iPrvs;
                        int  iBaseBar  = iCurrBar - 1;
                        bool bIsHigher = adIndValue[iCurrBar] > adIndValue[iBaseBar];

                        if (!isDescreteValues)  // Aroon oscillator uses bIsDescreteValues = true
                        {
                            bool bNoChange = true;
                            while (Math.Abs(adIndValue[iCurrBar] - adIndValue[iBaseBar]) < dSigma && bNoChange && iBaseBar > iFirstBar)
                            {
                                bNoChange = (bIsHigher == (adIndValue[iBaseBar + 1] > adIndValue[iBaseBar]));
                                iBaseBar--;
                            }
                        }

                        indCompLong.Value[iBar]  = adIndValue[iBaseBar] < adIndValue[iCurrBar] - dSigma ? 1 : 0;
                        indCompShort.Value[iBar] = adIndValue[iBaseBar] > adIndValue[iCurrBar] + dSigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_falls:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        int  iCurrBar  = iBar - iPrvs;
                        int  iBaseBar  = iCurrBar - 1;
                        bool bIsHigher = adIndValue[iCurrBar] > adIndValue[iBaseBar];

                        if (!isDescreteValues)  // Aroon oscillator uses bIsDescreteValues = true
                        {
                            bool bNoChange = true;
                            while (Math.Abs(adIndValue[iCurrBar] - adIndValue[iBaseBar]) < dSigma && bNoChange && iBaseBar > iFirstBar)
                            {
                                bNoChange = (bIsHigher == (adIndValue[iBaseBar + 1] > adIndValue[iBaseBar]));
                                iBaseBar--;
                            }
                        }

                        indCompLong.Value[iBar]  = adIndValue[iBaseBar] > adIndValue[iCurrBar] + dSigma ? 1 : 0;
                        indCompShort.Value[iBar] = adIndValue[iBaseBar] < adIndValue[iCurrBar] - dSigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_is_higher_than_the_level_line:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        indCompLong.Value[iBar]  = adIndValue[iBar - iPrvs] > dLevelLong  + dSigma ? 1 : 0;
                        indCompShort.Value[iBar] = adIndValue[iBar - iPrvs] < dLevelShort - dSigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_is_lower_than_the_level_line:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        indCompLong.Value[iBar]  = adIndValue[iBar - iPrvs] < dLevelLong  - dSigma ? 1 : 0;
                        indCompShort.Value[iBar] = adIndValue[iBar - iPrvs] > dLevelShort + dSigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_crosses_the_level_line_upward:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        int iBaseBar = iBar - iPrvs - 1;
                        while (Math.Abs(adIndValue[iBaseBar] - dLevelLong) < dSigma && iBaseBar > iFirstBar)
                        { iBaseBar--; }

                        indCompLong.Value[iBar]  = (adIndValue[iBaseBar] < dLevelLong  - dSigma && adIndValue[iBar - iPrvs] > dLevelLong  + dSigma) ? 1 : 0;
                        indCompShort.Value[iBar] = (adIndValue[iBaseBar] > dLevelShort + dSigma && adIndValue[iBar - iPrvs] < dLevelShort - dSigma) ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_crosses_the_level_line_downward:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        int iBaseBar = iBar - iPrvs - 1;
                        while (Math.Abs(adIndValue[iBaseBar] - dLevelLong) < dSigma && iBaseBar > iFirstBar)
                        { iBaseBar--; }

                        indCompLong.Value[iBar]  = (adIndValue[iBaseBar] > dLevelLong  + dSigma && adIndValue[iBar - iPrvs] < dLevelLong  - dSigma) ? 1 : 0;
                        indCompShort.Value[iBar] = (adIndValue[iBaseBar] < dLevelShort - dSigma && adIndValue[iBar - iPrvs] > dLevelShort + dSigma) ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_changes_its_direction_upward:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        int iBar0 = iBar  - iPrvs;
                        int iBar1 = iBar0 - 1;
                        while (Math.Abs(adIndValue[iBar0] - adIndValue[iBar1]) < dSigma && iBar1 > iFirstBar)
                        { iBar1--; }

                        int iBar2 = iBar1 - 1 > iFirstBar ? iBar1 - 1 : iFirstBar;
                        while (Math.Abs(adIndValue[iBar1] - adIndValue[iBar2]) < dSigma && iBar2 > iFirstBar)
                        { iBar2--; }

                        indCompLong.Value[iBar]  = (adIndValue[iBar2] > adIndValue[iBar1] && adIndValue[iBar1] < adIndValue[iBar0] && iBar1 == iBar0 - 1) ? 1 : 0;
                        indCompShort.Value[iBar] = (adIndValue[iBar2] < adIndValue[iBar1] && adIndValue[iBar1] > adIndValue[iBar0] && iBar1 == iBar0 - 1) ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_changes_its_direction_downward:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        int iBar0 = iBar  - iPrvs;
                        int iBar1 = iBar0 - 1;
                        while (Math.Abs(adIndValue[iBar0] - adIndValue[iBar1]) < dSigma && iBar1 > iFirstBar)
                        { iBar1--; }

                        int iBar2 = iBar1 - 1 > iFirstBar ? iBar1 - 1 : iFirstBar;
                        while (Math.Abs(adIndValue[iBar1] - adIndValue[iBar2]) < dSigma && iBar2 > iFirstBar)
                        { iBar2--; }

                        indCompLong.Value[iBar]  = (adIndValue[iBar2] < adIndValue[iBar1] && adIndValue[iBar1] > adIndValue[iBar0] && iBar1 == iBar0 - 1) ? 1 : 0;
                        indCompShort.Value[iBar] = (adIndValue[iBar2] > adIndValue[iBar1] && adIndValue[iBar1] < adIndValue[iBar0] && iBar1 == iBar0 - 1) ? 1 : 0;
                    }
                    break;

                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Calculates the logic of a No Direction Oscillator.
        /// </summary>
        /// <param name="iFirstBar">The first bar number.</param>
        /// <param name="iPrvs">To use the previous bar or not.</param>
        /// <param name="adIndValue">The indicator values.</param>
        /// <param name="dLevel">The Level value.</param>
        /// <param name="indComp">Indicator component where to save the results.</param>
        /// <param name="indLogic">The chosen logic.</param>
        /// <returns>True if everyting is ok.</returns>
        protected bool NoDirectionOscillatorLogic(int iFirstBar, int iPrvs, double[] adIndValue, double dLevel, ref IndicatorComp indComp, IndicatorLogic indLogic)
        {
            double dSigma = Sigma();

            switch (indLogic)
            {
                case IndicatorLogic.The_indicator_rises:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        int  iCurrBar  = iBar - iPrvs;
                        int  iBaseBar  = iCurrBar - 1;
                        bool bIsHigher = adIndValue[iCurrBar] > adIndValue[iBaseBar];
                        bool bNoChange = true;

                        while (Math.Abs(adIndValue[iCurrBar] - adIndValue[iBaseBar]) < dSigma && bNoChange && iBaseBar > iFirstBar)
                        {
                            bNoChange = (bIsHigher == (adIndValue[iBaseBar + 1] > adIndValue[iBaseBar]));
                            iBaseBar--;
                        }

                        indComp.Value[iBar] = adIndValue[iBaseBar] < adIndValue[iCurrBar] - dSigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_falls:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        int  iCurrBar  = iBar - iPrvs;
                        int  iBaseBar  = iCurrBar - 1;
                        bool bIsHigher = adIndValue[iCurrBar] > adIndValue[iBaseBar];
                        bool bNoChange = true;

                        while (Math.Abs(adIndValue[iCurrBar] - adIndValue[iBaseBar]) < dSigma && bNoChange && iBaseBar > iFirstBar)
                        {
                            bNoChange = (bIsHigher == (adIndValue[iBaseBar + 1] > adIndValue[iBaseBar]));
                            iBaseBar--;
                        }

                        indComp.Value[iBar] = adIndValue[iBaseBar] > adIndValue[iCurrBar] + dSigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_is_higher_than_the_level_line:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        indComp.Value[iBar] = adIndValue[iBar - iPrvs] > dLevel + dSigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_is_lower_than_the_level_line:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        indComp.Value[iBar] = adIndValue[iBar - iPrvs] < dLevel - dSigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_crosses_the_level_line_upward:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        int iBaseBar = iBar - iPrvs - 1;
                        while (Math.Abs(adIndValue[iBaseBar] - dLevel) < dSigma && iBaseBar > iFirstBar)
                        { iBaseBar--; }

                        indComp.Value[iBar] = (adIndValue[iBaseBar] < dLevel - dSigma && adIndValue[iBar - iPrvs] > dLevel + dSigma) ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_crosses_the_level_line_downward:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        int iBaseBar = iBar - iPrvs - 1;
                        while (Math.Abs(adIndValue[iBaseBar] - dLevel) < dSigma && iBaseBar > iFirstBar)
                        { iBaseBar--; }

                        indComp.Value[iBar] = (adIndValue[iBaseBar] > dLevel + dSigma && adIndValue[iBar - iPrvs] < dLevel - dSigma) ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_changes_its_direction_upward:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        int iBar0 = iBar - iPrvs;
                        int iBar1 = iBar0 - 1;
                        while (Math.Abs(adIndValue[iBar0] - adIndValue[iBar1]) < dSigma && iBar1 > iFirstBar)
                        { iBar1--; }

                        int iBar2 = iBar1 - 1 > iFirstBar ? iBar1 - 1 : iFirstBar;
                        while (Math.Abs(adIndValue[iBar1] - adIndValue[iBar2]) < dSigma && iBar2 > iFirstBar)
                        { iBar2--; }

                        indComp.Value[iBar] = (adIndValue[iBar2] > adIndValue[iBar1] && adIndValue[iBar1] < adIndValue[iBar0] && iBar1 == iBar0 - 1) ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_changes_its_direction_downward:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        int iBar0 = iBar  - iPrvs;
                        int iBar1 = iBar0 - 1;
                        while (Math.Abs(adIndValue[iBar0] - adIndValue[iBar1]) < dSigma && iBar1 > iFirstBar)
                        { iBar1--; }

                        int iBar2 = iBar1 - 1 > iFirstBar ? iBar1 - 1 : iFirstBar;
                        while (Math.Abs(adIndValue[iBar1] - adIndValue[iBar2]) < dSigma && iBar2 > iFirstBar)
                        { iBar2--; }

                        indComp.Value[iBar] = (adIndValue[iBar2] < adIndValue[iBar1] && adIndValue[iBar1] > adIndValue[iBar0] && iBar1 == iBar0 - 1) ? 1 : 0;
                    }
                    break;

                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Calculates the logic of a band indicator.
        /// </summary>
        /// <param name="iFirstBar">The first bar number.</param>
        /// <param name="iPrvs">To use the previous bar or not.</param>
        /// <param name="adUpperBand">The Upper band values.</param>
        /// <param name="adLowerBand">The Lower band values.</param>
        /// <param name="indCompLong">Indicator component for Long position.</param>
        /// <param name="indCompShort">Indicator component for Short position.</param>
        /// <param name="indLogic">The chosen logic.</param>
        /// <returns>True if everyting is ok.</returns>
        protected bool BandIndicatorLogic(int iFirstBar, int iPrvs, double[] adUpperBand, double[] adLowerBand, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort, BandIndLogic indLogic)
        {
            double dSigma = Sigma();

            switch (indLogic)
            {
                case BandIndLogic.The_bar_opens_below_the_Upper_Band:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        indCompLong.Value[iBar]  = Open[iBar] < adUpperBand[iBar - iPrvs] - dSigma ? 1 : 0;
                        indCompShort.Value[iBar] = Open[iBar] > adLowerBand[iBar - iPrvs] + dSigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_above_the_Upper_Band:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        indCompLong.Value[iBar]  = Open[iBar] > adUpperBand[iBar - iPrvs] + dSigma ? 1 : 0;
                        indCompShort.Value[iBar] = Open[iBar] < adLowerBand[iBar - iPrvs] - dSigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_below_the_Lower_Band:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        indCompLong.Value[iBar]  = Open[iBar] < adLowerBand[iBar - iPrvs] - dSigma ? 1 : 0;
                        indCompShort.Value[iBar] = Open[iBar] > adUpperBand[iBar - iPrvs] + dSigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_above_the_Lower_Band:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        indCompLong.Value[iBar]  = Open[iBar] > adLowerBand[iBar - iPrvs] + dSigma ? 1 : 0;
                        indCompShort.Value[iBar] = Open[iBar] < adUpperBand[iBar - iPrvs] - dSigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_below_the_Upper_Band_after_opening_above_it:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        int iBaseBar = iBar - 1;
                        while (Math.Abs(Open[iBaseBar] - adUpperBand[iBaseBar - iPrvs]) < dSigma && iBaseBar > iFirstBar)
                        { iBaseBar--; }

                        indCompLong.Value[iBar]  = Open[iBar] < adUpperBand[iBar - iPrvs] - dSigma && Open[iBaseBar] > adUpperBand[iBaseBar - iPrvs] + dSigma ? 1 : 0;

                        iBaseBar = iBar - 1;
                        while (Math.Abs(Open[iBaseBar] - adLowerBand[iBaseBar - iPrvs]) < dSigma && iBaseBar > iFirstBar)
                        { iBaseBar--; }

                        indCompShort.Value[iBar] = Open[iBar] > adLowerBand[iBar - iPrvs] + dSigma && Open[iBaseBar] < adLowerBand[iBaseBar - iPrvs] - dSigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_above_the_Upper_Band_after_opening_below_it:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        int iBaseBar = iBar - 1;
                        while (Math.Abs(Open[iBaseBar] - adUpperBand[iBaseBar - iPrvs]) < dSigma && iBaseBar > iFirstBar)
                        { iBaseBar--; }

                        indCompLong.Value[iBar]  = Open[iBar] > adUpperBand[iBar - iPrvs] + dSigma && Open[iBaseBar] < adUpperBand[iBaseBar - iPrvs] - dSigma ? 1 : 0;

                        iBaseBar = iBar - 1;
                        while (Math.Abs(Open[iBaseBar] - adLowerBand[iBaseBar - iPrvs]) < dSigma && iBaseBar > iFirstBar)
                        { iBaseBar--; }

                        indCompShort.Value[iBar] = Open[iBar] < adLowerBand[iBar - iPrvs] - dSigma && Open[iBaseBar] > adLowerBand[iBaseBar - iPrvs] + dSigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_below_the_Lower_Band_after_opening_above_it:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        int iBaseBar = iBar - 1;
                        while (Math.Abs(Open[iBaseBar] - adLowerBand[iBaseBar - iPrvs]) < dSigma && iBaseBar > iFirstBar)
                        { iBaseBar--; }

                        indCompLong.Value[iBar]  = Open[iBar] < adLowerBand[iBar - iPrvs] - dSigma && Open[iBaseBar] > adLowerBand[iBaseBar - iPrvs] + dSigma ? 1 : 0;

                        iBaseBar = iBar - 1;
                        while (Math.Abs(Open[iBaseBar] - adUpperBand[iBaseBar - iPrvs]) < dSigma && iBaseBar > iFirstBar)
                        { iBaseBar--; }

                        indCompShort.Value[iBar] = Open[iBar] > adUpperBand[iBar - iPrvs] + dSigma && Open[iBaseBar] < adUpperBand[iBaseBar - iPrvs] - dSigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_above_the_Lower_Band_after_opening_below_it:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        int iBaseBar = iBar - 1;
                        while (Math.Abs(Open[iBaseBar] - adLowerBand[iBaseBar - iPrvs]) < dSigma && iBaseBar > iFirstBar)
                        { iBaseBar--; }

                        indCompLong.Value[iBar]  = Open[iBar] > adLowerBand[iBar - iPrvs] + dSigma && Open[iBaseBar] < adLowerBand[iBaseBar - iPrvs] - dSigma ? 1 : 0;

                        iBaseBar = iBar - 1;
                        while (Math.Abs(Open[iBaseBar] - adUpperBand[iBaseBar - iPrvs]) < dSigma && iBaseBar > iFirstBar)
                        { iBaseBar--; }

                        indCompShort.Value[iBar] = Open[iBar] < adUpperBand[iBar - iPrvs] - dSigma && Open[iBaseBar] > adUpperBand[iBaseBar - iPrvs] + dSigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_closes_below_the_Upper_Band:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        indCompLong.Value[iBar]  = Close[iBar] < adUpperBand[iBar - iPrvs] - dSigma ? 1 : 0;
                        indCompShort.Value[iBar] = Close[iBar] > adLowerBand[iBar - iPrvs] + dSigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_closes_above_the_Upper_Band:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        indCompLong.Value[iBar]  = Close[iBar] > adUpperBand[iBar - iPrvs] + dSigma ? 1 : 0;
                        indCompShort.Value[iBar] = Close[iBar] < adLowerBand[iBar - iPrvs] - dSigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_closes_below_the_Lower_Band:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        indCompLong.Value[iBar]  = Close[iBar] < adLowerBand[iBar - iPrvs] - dSigma ? 1 : 0;
                        indCompShort.Value[iBar] = Close[iBar] > adUpperBand[iBar - iPrvs] + dSigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_closes_above_the_Lower_Band:
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        indCompLong.Value[iBar]  = Close[iBar] > adLowerBand[iBar - iPrvs] + dSigma ? 1 : 0;
                        indCompShort.Value[iBar] = Close[iBar] < adUpperBand[iBar - iPrvs] - dSigma ? 1 : 0;
                    }
                    break;

                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns signals for the logic rule "Indicator rises".
        /// </summary>
        protected void IndicatorRisesLogic(int iFirstBar, int iPrvs, double[] adIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double dSigma = Sigma();

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                int  iCurrBar  = iBar - iPrvs;
                int  iBaseBar  = iCurrBar - 1;
                bool bNoChange = true;
                bool bIsHigher = adIndValue[iCurrBar] > adIndValue[iBaseBar];

                while (Math.Abs(adIndValue[iCurrBar] - adIndValue[iBaseBar]) < dSigma && bNoChange && iBaseBar > iFirstBar)
                {
                    bNoChange = (bIsHigher == (adIndValue[iBaseBar + 1] > adIndValue[iBaseBar]));
                    iBaseBar--;
                }

                indCompLong.Value[iBar]  = adIndValue[iCurrBar] > adIndValue[iBaseBar] + dSigma ? 1 : 0;
                indCompShort.Value[iBar] = adIndValue[iCurrBar] < adIndValue[iBaseBar] - dSigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "Indicator falls"
        /// </summary>
        protected void IndicatorFallsLogic(int iFirstBar, int iPrvs, double[] adIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double dSigma = Sigma();

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                int  iCurrBar  = iBar - iPrvs;
                int  iBaseBar  = iCurrBar - 1;
                bool bNoChange = true;
                bool bIsLower  = adIndValue[iCurrBar] < adIndValue[iBaseBar];

                while (Math.Abs(adIndValue[iCurrBar] - adIndValue[iBaseBar]) < dSigma && bNoChange && iBaseBar > iFirstBar)
                {
                    bNoChange = (bIsLower == (adIndValue[iBaseBar + 1] < adIndValue[iBaseBar]));
                    iBaseBar--;
                }

                indCompLong.Value[iBar]  = adIndValue[iCurrBar] < adIndValue[iBaseBar] - dSigma ? 1 : 0;
                indCompShort.Value[iBar] = adIndValue[iCurrBar] > adIndValue[iBaseBar] + dSigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The Indicator is higher than the AnotherIndicator"
        /// </summary>
        protected void IndicatorIsHigherThanAnotherIndicatorLogic(int iFirstBar, int iPrvs, double[] adIndValue, double[] adAnotherIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double dSigma = Sigma();

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                int iCurrBar = iBar - iPrvs;
                indCompLong.Value[iBar]  = adIndValue[iCurrBar] > adAnotherIndValue[iCurrBar] + dSigma ? 1 : 0;
                indCompShort.Value[iBar] = adIndValue[iCurrBar] < adAnotherIndValue[iCurrBar] - dSigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The Indicator is lower than the AnotherIndicator"
        /// </summary>
        protected void IndicatorIsLowerThanAnotherIndicatorLogic(int iFirstBar, int iPrvs, double[] adIndValue, double[] adAnotherIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double dSigma = Sigma();

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                int iCurrBar = iBar - iPrvs;
                indCompLong.Value[iBar]  = adIndValue[iCurrBar] < adAnotherIndValue[iCurrBar] - dSigma ? 1 : 0;
                indCompShort.Value[iBar] = adIndValue[iCurrBar] > adAnotherIndValue[iCurrBar] + dSigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The Indicator crosses AnotherIndicator upward"
        /// </summary>
        protected void IndicatorCrossesAnotherIndicatorUpwardLogic(int iFirstBar, int iPrvs, double[] adIndValue, double[] adAnotherIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double dSigma = Sigma();

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                int iCurrBar = iBar - iPrvs;
                int iBaseBar = iCurrBar - 1;
                while (Math.Abs(adIndValue[iBaseBar] - adAnotherIndValue[iBaseBar]) < dSigma && iBaseBar > iFirstBar)
                { iBaseBar--; }

                indCompLong.Value[iBar]  = adIndValue[iCurrBar] > adAnotherIndValue[iCurrBar] + dSigma && adIndValue[iBaseBar] < adAnotherIndValue[iBaseBar] - dSigma ? 1 : 0;
                indCompShort.Value[iBar] = adIndValue[iCurrBar] < adAnotherIndValue[iCurrBar] - dSigma && adIndValue[iBaseBar] > adAnotherIndValue[iBaseBar] + dSigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The Indicator crosses AnotherIndicator downward"
        /// </summary>
        protected void IndicatorCrossesAnotherIndicatorDownwardLogic(int iFirstBar, int iPrvs, double[] adIndValue, double[] adAnotherIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double dSigma = Sigma();

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                int iCurrBar = iBar - iPrvs;
                int iBaseBar = iCurrBar - 1;
                while (Math.Abs(adIndValue[iBaseBar] - adAnotherIndValue[iBaseBar]) < dSigma && iBaseBar > iFirstBar)
                { iBaseBar--; }

                indCompLong.Value[iBar]  = adIndValue[iCurrBar] < adAnotherIndValue[iCurrBar] - dSigma && adIndValue[iBaseBar] > adAnotherIndValue[iBaseBar] + dSigma ? 1 : 0;
                indCompShort.Value[iBar] = adIndValue[iCurrBar] > adAnotherIndValue[iCurrBar] + dSigma && adIndValue[iBaseBar] < adAnotherIndValue[iBaseBar] - dSigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The bar opens above the Indicator"
        /// </summary>
        protected void BarOpensAboveIndicatorLogic(int iFirstBar, int iPrvs, double[] adIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double dSigma = Sigma();

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                indCompLong.Value[iBar]  = Open[iBar] > adIndValue[iBar - iPrvs] + dSigma ? 1 : 0;
                indCompShort.Value[iBar] = Open[iBar] < adIndValue[iBar - iPrvs] - dSigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The bar opens below the Indicator"
        /// </summary>
        protected void BarOpensBelowIndicatorLogic(int iFirstBar, int iPrvs, double[] adIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double dSigma = Sigma();

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                indCompLong.Value[iBar]  = Open[iBar] < adIndValue[iBar - iPrvs] - dSigma ? 1 : 0;
                indCompShort.Value[iBar] = Open[iBar] > adIndValue[iBar - iPrvs] + dSigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The bar opens above the Indicator after opening below it"
        /// </summary>
        protected void BarOpensAboveIndicatorAfterOpeningBelowLogic(int iFirstBar, int iPrvs, double[] adIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double dSigma = Sigma();

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                int iBaseBar = iBar - 1;
                while (Math.Abs(Open[iBaseBar] - adIndValue[iBaseBar - iPrvs]) < dSigma && iBaseBar > iFirstBar)
                { iBaseBar--; }

                indCompLong.Value[iBar]  = Open[iBar] > adIndValue[iBar - iPrvs] + dSigma && Open[iBaseBar] < adIndValue[iBaseBar - iPrvs] - dSigma ? 1 : 0;
                indCompShort.Value[iBar] = Open[iBar] < adIndValue[iBar - iPrvs] - dSigma && Open[iBaseBar] > adIndValue[iBaseBar - iPrvs] + dSigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The bar opens below the Indicator after opening above it"
        /// </summary>
        protected void BarOpensBelowIndicatorAfterOpeningAboveLogic(int iFirstBar, int iPrvs, double[] adIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double dSigma = Sigma();

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                int iBaseBar = iBar - 1;
                while (Math.Abs(Open[iBaseBar] - adIndValue[iBaseBar - iPrvs]) < dSigma && iBaseBar > iFirstBar)
                { iBaseBar--; }

                indCompLong.Value[iBar]  = Open[iBar] < adIndValue[iBar - iPrvs] - dSigma && Open[iBaseBar] > adIndValue[iBaseBar - iPrvs] + dSigma ? 1 : 0;
                indCompShort.Value[iBar] = Open[iBar] > adIndValue[iBar - iPrvs] + dSigma && Open[iBaseBar] < adIndValue[iBaseBar - iPrvs] - dSigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The bar closes above the Indicator"
        /// </summary>
        protected void BarClosesAboveIndicatorLogic(int iFirstBar, int iPrvs, double[] adIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double dSigma = Sigma();

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                indCompLong.Value[iBar]  = Close[iBar] > adIndValue[iBar - iPrvs] + dSigma ? 1 : 0;
                indCompShort.Value[iBar] = Close[iBar] < adIndValue[iBar - iPrvs] - dSigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The bar closes below the Indicator"
        /// </summary>
        protected void BarClosesBelowIndicatorLogic(int iFirstBar, int iPrvs, double[] adIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double dSigma = Sigma();

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                indCompLong.Value[iBar]  = Close[iBar] < adIndValue[iBar - iPrvs] - dSigma ? 1 : 0;
                indCompShort.Value[iBar] = Close[iBar] > adIndValue[iBar - iPrvs] + dSigma ? 1 : 0;
            }

            return;
        }
    }
}