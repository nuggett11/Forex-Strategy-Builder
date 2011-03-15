// Backtester - Way Point
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Builder
{
    public enum WayPointType
    {
        None, Open, High, Low, Close, Entry, Exit, Add, Reduce, Reverse, Cancel
    }

    public class Way_Point
    {
        double price;
        WayPointType wpType;
        int ordNumb;
        int posNumb;

        /// <summary>
        /// Gets or sets the waypoint price
        /// </summary>
        public double Price { get { return price; } set { price = value; } }

        /// <summary>
        /// Gets or sets the waypoint type
        /// </summary>
        public WayPointType WPType { get { return wpType; } set { wpType = value; } }

        /// <summary>
        /// Gets or sets the waypoint order number
        /// </summary>
        public int OrdNumb { get { return ordNumb; } set { ordNumb = value; } }

        /// <summary>
        /// Gets or sets the waypoint position number
        /// </summary>
        public int PosNumb { get { return posNumb; } set { posNumb = value; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Way_Point()
        {
            price   = 0;
            wpType  = WayPointType.None;
            ordNumb = -1;
            posNumb = -1;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Way_Point(double price, WayPointType wpType, int ordNumb, int posNumb)
        {
            this.price  = price;
            this.wpType = wpType;

            if (wpType == WayPointType.Open || wpType == WayPointType.High  ||
                wpType == WayPointType.Low  || wpType == WayPointType.Close ||
                wpType == WayPointType.None)
                this.ordNumb = -1;
            else
                this.ordNumb = ordNumb;

            if (Backtester.PosFromNumb(posNumb).PosDir == PosDirection.None   ||
                Backtester.PosFromNumb(posNumb).PosDir == PosDirection.Closed &&
                wpType != WayPointType.Exit && wpType != WayPointType.Reduce)
                this.posNumb = -1;
            else
                this.posNumb = posNumb;
        }

        /// <summary>
        /// Shows the WayPointType as a string.
        /// </summary>
        public static string WPTypeToString(WayPointType wpType)
        {
            string output;

            switch (wpType)
            {
                case WayPointType.None:
                    output = "None";
                    break;
                case WayPointType.Open:
                    output = "Bar open";
                    break;
                case WayPointType.High:
                    output = "Bar high";
                    break;
                case WayPointType.Low:
                    output = "Bar low";
                    break;
                case WayPointType.Close:
                    output = "Bar close";
                    break;
                case WayPointType.Entry:
                    output = "Entry point";
                    break;
                case WayPointType.Exit:
                    output = "Exit point";
                    break;
                case WayPointType.Add:
                    output = "Adding point";
                    break;
                case WayPointType.Reduce:
                    output = "Reducing point";
                    break;
                case WayPointType.Reverse:
                    output = "Reversing point";
                    break;
                case WayPointType.Cancel:
                    output = "Cancelled order";
                    break;
                default:
                    output = "Error";
                    break;
            }

            return output;
        }
    }
}
