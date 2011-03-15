// Indicator parameters
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Text;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Describes an indicator fully
    /// </summary>
    public class IndicatorParam
    {
        int             iSlotNumb;
        SlotTypes       slotType;
        bool            bIsDefined;
        string          sIndicatorName;
        TypeOfIndicator typeOfIndicator;
        ExecutionTime   timeExecution;
        ListParam[]     aListParam;
        NumericParam[]  aNumParam;
        CheckParam[]    aCheckParam;

        /// <summary>
        /// Gets or sets the number of current slot.
        /// </summary>
        public int SlotNumber { get { return iSlotNumb; } set { iSlotNumb = value; } }

        /// <summary>
        /// Gets or sets the type of the slot.
        /// </summary>
        public SlotTypes SlotType { get { return slotType; } set { slotType = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether the indicator is defined.
        /// </summary>
        public bool IsDefined { get { return bIsDefined; } set { bIsDefined = value; } }

        /// <summary>
        /// Gets or sets the indicator name.
        /// </summary>
        public string IndicatorName { get { return sIndicatorName; } set { sIndicatorName = value; } }

        /// <summary>
        /// Gets or sets the type of the indicator
        /// </summary>
        public TypeOfIndicator IndicatorType { get { return typeOfIndicator; } set { typeOfIndicator = value; } }

        /// <summary>
        /// Gets or sets the type of the time execution of the indicator
        /// </summary>
        public ExecutionTime ExecutionTime { get { return timeExecution; } set { timeExecution = value; } }

        /// <summary>
        /// Gets or sets a parameter represented by a ComboBox.
        /// </summary>
        public ListParam[] ListParam { get { return aListParam; } set { aListParam = value; } }

        /// <summary>
        /// Gets or sets a parameter represented by a NumericUpDown.
        /// </summary>
        public NumericParam[] NumParam { get { return aNumParam; } set { aNumParam = value; } }

        /// <summary>
        /// Gets or sets a parameter represented by a CheckBox.
        /// </summary>
        public CheckParam[] CheckParam { get { return aCheckParam; } set { aCheckParam = value; } }

        /// <summary>
        /// Creats emty parameters.
        /// </summary>
        public IndicatorParam()
        {
            iSlotNumb       = 0;
            bIsDefined      = false;
            slotType        = SlotTypes.NotDefined;
            sIndicatorName  = String.Empty;
            typeOfIndicator = TypeOfIndicator.Indicator;
            timeExecution   = ExecutionTime.DuringTheBar;
            aListParam      = new ListParam[5];
            aNumParam       = new NumericParam[6];
            aCheckParam     = new CheckParam[2];

            for (int i = 0; i < 5; i++)
                aListParam[i] = new ListParam();

            for (int i = 0; i < 6; i++)
                aNumParam[i] = new NumericParam();

            for (int i = 0; i < 2; i++)
                aCheckParam[i] = new CheckParam();
        }

        /// <summary>
        /// Returns a copy
        /// </summary>
        public IndicatorParam Clone()
        {
            IndicatorParam iparam = new IndicatorParam();

            iparam.iSlotNumb       = iSlotNumb;
            iparam.bIsDefined      = bIsDefined;
            iparam.slotType        = slotType;
            iparam.sIndicatorName  = sIndicatorName;
            iparam.typeOfIndicator = typeOfIndicator;
            iparam.timeExecution   = timeExecution;
            iparam.aListParam      = new ListParam[5];
            iparam.aNumParam       = new NumericParam[6];
            iparam.aCheckParam     = new CheckParam[2];

            for (int i = 0; i < 5; i++)
                iparam.aListParam[i] = aListParam[i].Clone();

            for (int i = 0; i < 6; i++)
                iparam.aNumParam[i] = aNumParam[i].Clone();

            for (int i = 0; i < 2; i++)
                iparam.aCheckParam[i] = aCheckParam[i].Clone();

            return iparam;
        }

        /// <summary>
        /// Represents the indicator parametres in a readable form.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach(ListParam listParam in aListParam)
                if (listParam.Enabled)
                    sb.AppendLine(listParam.Caption + " - " + listParam.Text);

            foreach (NumericParam numParam in aNumParam)
                if (numParam.Enabled)
                    sb.AppendLine(numParam.Caption + " - " + numParam.ValueToString);

            foreach (CheckParam checkParam in aCheckParam)
                if (checkParam.Enabled)
                    sb.AppendLine(checkParam.Caption + " - " + (checkParam.Checked ? "Yes" : "No"));

            return sb.ToString();
        }
    }
}
