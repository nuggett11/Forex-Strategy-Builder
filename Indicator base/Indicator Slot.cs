// IndicatorSlot Class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Builder
{
    public enum OppositeDirSignalAction { Nothing, Reduce, Close, Reverse }
    public enum SameDirSignalAction { Nothing, Winner, Add }

    /// <summary>
    /// Class IndicatorSlot.
    /// </summary>
    public class IndicatorSlot
    {
        int       slotNumb;
        SlotTypes slotType;
        string    group;
        bool      isDefined;
        StrategySlotStatus slotStatus;
        string    indicatorName;
        bool      isSeparatedChart;
        IndicatorParam  indicatorParam;
        IndicatorComp[] component;
        double[]  adSpecValue;
        double    minValue;
        double    maxValue;

        /// <summary>
        /// Gets or sets the number of the slot.
        /// </summary>
        public int SlotNumber { get { return slotNumb; } set { slotNumb = value; } }

        /// <summary>
        /// Gets or sets the type of the slot.
        /// </summary>
        public SlotTypes SlotType { get { return slotType; } set { slotType = value; } }

        /// <summary>
        /// Gets or sets the logical group of the slot.
        /// </summary>
        public string LogicalGroup { get { return group; } set { group = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether the indicator is defined.
        /// </summary>
        public bool IsDefined { get { return isDefined; } set { isDefined = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether it is a locked slot (Generator)
        /// </summary>
        public StrategySlotStatus SlotStatus { get { return slotStatus; } set { slotStatus = value; } }

        /// <summary>
        /// Gets or sets the indicator name.
        /// </summary>
        public string IndicatorName { get { return indicatorName; } set { indicatorName = value; } }

        /// <summary>
        /// Gets or sets the indicator parameters.
        /// </summary>
        public IndicatorParam IndParam { get { return indicatorParam; } set { indicatorParam = value; } }

        /// <summary>
        /// If the chart is drown in separated panel.
        /// </summary>
        public bool SeparatedChart { get { return isSeparatedChart; } set { isSeparatedChart = value; } }

        /// <summary>
        /// Gets or sets an indicator component.
        /// </summary>
        public IndicatorComp[] Component { get { return component; } set { component = value; } }

        /// <summary>
        /// Gets or sets an indicator's special values.
        /// </summary>
        public double[] SpecValue { get { return adSpecValue; } set { adSpecValue = value; } }

        /// <summary>
        /// Gets or sets an indicator's min value.
        /// </summary>
        public double MinValue { get { return minValue; } set { minValue = value; } }

        /// <summary>
        /// Gets or sets an indicator's max value.
        /// </summary>
        public double MaxValue { get { return maxValue; } set { maxValue = value; } }

        /// <summary>
        ///  The default constructor.
        /// </summary>
        public IndicatorSlot()
        {
            slotNumb         = 0;
            slotType         = SlotTypes.NotDefined;
            group            = "";
            isDefined        = false;
            slotStatus       = StrategySlotStatus.Open;
            indicatorName    = "Not defined";
            indicatorParam   = new IndicatorParam();
            isSeparatedChart = false;
            component        = new IndicatorComp[] { };
            adSpecValue      = new double[] { };
            minValue         = double.MaxValue;
            maxValue         = double.MinValue;
        }

        /// <summary>
        ///  Returns a copy
        /// </summary>
        public IndicatorSlot Clone()
        {
            IndicatorSlot islot = new IndicatorSlot();
            islot.slotNumb         = slotNumb;
            islot.slotType         = slotType;
            islot.slotStatus       = slotStatus;
            islot.group            = group;
            islot.isDefined        = isDefined;
            islot.indicatorName    = indicatorName;
            islot.isSeparatedChart = isSeparatedChart;
            islot.minValue         = minValue;
            islot.maxValue         = maxValue;
            islot.indicatorParam   = indicatorParam.Clone();

            islot.adSpecValue = new double[adSpecValue.Length];
            adSpecValue.CopyTo(islot.adSpecValue, 0);

            islot.component = new IndicatorComp[component.Length];
            for (int i = 0; i < component.Length; i++)
                islot.component[i] = component[i].Clone();

            return islot;
        }
    }
}
