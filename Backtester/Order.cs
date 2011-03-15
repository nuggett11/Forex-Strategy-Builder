// Order class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Order direction
    /// </summary>
    public enum OrderDirection { Error, None, Buy, Sell }

    /// <summary>
    /// Order type
    /// </summary>
    public enum OrderType { Error, None,  Market, Stop, Limit, StopLimit }

    /// <summary>
    /// Order condition
    /// </summary>
    public enum OrderCondition { Error, None, Norm, EOD, If }

    /// <summary>
    /// Order status
    /// </summary>
    public enum OrderStatus { Error, None, Sent, Confirmed, Executed, Cancelled }

    /// <summary>
    /// Order sender
    /// </summary>
    public enum OrderSender { Error, None, Open, Close }

    /// <summary>
    /// Order origin
    /// </summary>
    public enum OrderOrigin { Error, None, Strategy, PermanentStopLoss, PermanentTakeProfit, BreakEven, MarginCall }

    /// <summary>
    /// Keeps the coordinates of each order
    /// </summary>
    public struct OrderCoordinates
    {
        private int bar;
        private int orderNumber;

        /// <summary>
        /// The bar number
        /// </summary>
        public int Bar { get { return bar; } set { bar = value; } }

        /// <summary>
        /// The order number
        /// </summary>
        public int Ord { get { return orderNumber; } set { orderNumber = value; } }
    }

    /// <summary>
    /// Order
    /// </summary>
    public class Order
    {
        private int             ordNumb;   // ID number
        private double          ordLots;   // The count of lots
        private int             ordIF;     // Zero or the ID number of the other order
        private int             ordPos;    // Zero or the ID number of the target position
        private double          ordPrice;  // The order's price
        private double          ordPrice2; // The order's second price
        private OrderDirection  ordDir;
        private OrderType       ordType;
        private OrderCondition  ordCond;
        private OrderStatus     ordStatus;
        private OrderSender     ordSender;
        private OrderOrigin     ordOrigin;
        private string          ordNote;

        /// <summary>
        /// The ID of the order.
        /// </summary>
        public int OrdNumb { get { return ordNumb; } set { ordNumb = value; } }

        /// <summary>
        /// The amount of the order.
        /// </summary>
        public double OrdLots { get { return ordLots; } set { ordLots = value; } }

        /// <summary>
        /// Zero or the ID number of the other order.
        /// </summary>
        public int OrdIF { get { return ordIF; } set { ordIF = value; } }

        /// <summary>
        /// Zero or the ID number of the target position.
        /// </summary>
        public int OrdPos { get { return ordPos; } set { ordPos = value; } }

        /// <summary>
        /// The order's price.
        /// </summary>
        public double OrdPrice { get { return ordPrice; } set { ordPrice = value; } }

        /// <summary>
        /// The order's second price.
        /// </summary>
        public double OrdPrice2 { get { return ordPrice2; } set { ordPrice2 = value; } }

        /// <summary>
        /// The order's direction.
        /// </summary>
        public OrderDirection OrdDir { get { return ordDir; } set { ordDir = value; } }

        /// <summary>
        /// The order's type.
        /// </summary>
        public OrderType OrdType { get { return ordType; } set { ordType = value; } }

        /// <summary>
        /// The order's condition.
        /// </summary>
        public OrderCondition OrdCond { get { return ordCond; } set { ordCond = value; } }

        /// <summary>
        /// The order's status.
        /// </summary>
        public OrderStatus OrdStatus { get { return ordStatus; } set { ordStatus = value; } }

        /// <summary>
        /// The order's sender.
        /// </summary>
        public OrderSender OrdSender { get { return ordSender; } set { ordSender = value; } }

        /// <summary>
        /// The order's origin.
        /// </summary>
        public OrderOrigin OrdOrigin { get { return ordOrigin; } set { ordOrigin = value; } }

        /// <summary>
        /// The order's note.
        /// </summary>
        public string OrdNote { get { return ordNote; } set { ordNote = value; } }

        /// <summary>
        /// Gets the order's icon.
        /// </summary>
        public Image OrderIcon
        {
            get
            {
                Image img = Properties.Resources.warning;

                if (ordStatus == OrderStatus.Executed)
                {
                    if (ordDir == OrderDirection.Buy)
                        img = Properties.Resources.ord_buy;
                    else
                        img = Properties.Resources.ord_sell;
                }
                else if (ordStatus == OrderStatus.Cancelled)
                {
                    if (ordDir == OrderDirection.Buy)
                        img = Properties.Resources.ord_buy_cancel;
                    else
                        img = Properties.Resources.ord_sell_cancel;
                }

                return img;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Order()
        {
            ordDir    = OrderDirection.None;
            ordType   = OrderType.None;
            ordCond   = OrderCondition.None;
            ordStatus = OrderStatus.None;
            ordSender = OrderSender.None;
            ordOrigin = OrderOrigin.None;
            ordNote   = "Not Defined";
        }
        
        /// <summary>
        /// Makes a deep copy.
        /// </summary>
        public Order Copy()
        {
            Order order = new Order();

            order.ordDir    = this.ordDir;
            order.ordType   = this.ordType;
            order.ordCond   = this.ordCond;
            order.ordStatus = this.ordStatus;
            order.ordSender = this.ordSender;
            order.ordOrigin = this.ordOrigin;
            order.ordNumb   = this.ordNumb;
            order.ordIF     = this.ordIF;
            order.ordPos    = this.ordPos;
            order.ordLots   = this.ordLots;
            order.ordPrice  = this.ordPrice;
            order.ordPrice2 = this.ordPrice2;
            order.ordNote   = this.ordNote;

            return order;
        }

        /// <summary>
        /// Represents the position.
        /// </summary>
        public override string ToString()
        {
            string orderd = "";
            string NL = Environment.NewLine;

            orderd += "Number    " + (ordNumb + 1).ToString() + NL;
            orderd += "Direction " + ordDir.ToString()        + NL;
            orderd += "Type      " + ordType.ToString()       + NL;
            orderd += "Condition " + ordCond.ToString()       + NL;
            orderd += "Status    " + ordStatus.ToString()     + NL;
            orderd += "Sender    " + ordSender.ToString()     + NL;
            orderd += "Origin    " + ordOrigin.ToString()     + NL;
            orderd += "If order  " + (ordIF + 1).ToString()   + NL;
            orderd += "To pos    " + (ordPos + 1).ToString()  + NL;
            orderd += "Lots      " + ordLots.ToString()       + NL;
            orderd += "Price     " + ordPrice.ToString()      + NL;
            orderd += "Price2    " + ordPrice2.ToString()     + NL;
            orderd += "Note      " + ordNote.ToString()       + NL;

            return orderd;
        }
    }
}
