// Indicator component
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Indicator's component.
    /// </summary>
    public class IndicatorComp
    {
        private string   compName;
        private IndComponentType dataType;
        private IndChartType     chartType;
        private Color    chartColor;
        private int      firstBar;
        private int      prvs;
        private bool     isDynInfo;
        private double[] adValue;
        private PositionPriceDependence posPriceDependence;

        /// <summary>
        /// The component's name
        /// </summary>
        public string CompName { get { return compName; } set { compName = value; } }

        /// <summary>
        /// The component's data type
        /// </summary>
        public IndComponentType DataType { get { return dataType; } set { dataType = value; } }

        /// <summary>
        /// The component's chart type
        /// </summary>
        public IndChartType ChartType { get { return chartType; } set { chartType = value; } }

        /// <summary>
        /// The component's chart color
        /// </summary>
        public Color ChartColor { get { return chartColor; } set { chartColor = value; } }

        /// <summary>
        /// The component's first bar
        /// </summary>
        public int FirstBar { get { return firstBar; } set { firstBar = value; } }

        /// <summary>
        /// The indicator uses the previous bar value
        /// </summary>
        public int UsePreviousBar { get { return prvs; } set { prvs = value; } }

        /// <summary>
        /// Whether the component has to be shown on dynamic info or not?
        /// </summary>
        public bool ShowInDynInfo { get { return isDynInfo; } set { isDynInfo = value; } }

        /// <summary>
        /// Whether the component depends of the position entry price.
        /// </summary>
        public PositionPriceDependence PosPriceDependence { get { return posPriceDependence; } set { posPriceDependence = value; } }

        /// <summary>
        /// The component's data value
        /// </summary>
        public double[] Value { get { return adValue; } set { adValue = value; } }

        /// <summary>
        /// Public constructor
        /// </summary>
        public IndicatorComp()
        {
            compName   = "Not defined";
            dataType   = IndComponentType.NotDefined;
            chartType  = IndChartType.NoChart;
            chartColor = Color.Red;
            firstBar   = 0;
            prvs       = 0;
            isDynInfo  = true;
            adValue    = new double[] { };
            posPriceDependence = PositionPriceDependence.None;
        }

        /// <summary>
        /// Returns a copy
        /// </summary>
        public IndicatorComp Clone()
        {
            IndicatorComp icomp = new IndicatorComp();

            icomp.compName   = compName;
            icomp.dataType   = dataType;
            icomp.chartType  = chartType;
            icomp.chartColor = chartColor;
            icomp.firstBar   = firstBar;
            icomp.prvs       = prvs;
            icomp.isDynInfo  = isDynInfo;
            icomp.posPriceDependence = posPriceDependence;

            if (adValue != null)
            {
                icomp.adValue = new double[adValue.Length];
                adValue.CopyTo(icomp.adValue, 0);
            }

            return icomp;
        }
    }
}
