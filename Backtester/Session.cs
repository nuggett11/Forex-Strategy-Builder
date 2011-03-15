// Session class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Builder
{
    public class Session
    {
        int  bar;       // The bar number
        int  orders;    // The count of orders
        int  positions; // The count of positions
        int  wayPoints; // The count of interpolating steps

        bool isTopReached;
        bool isBottomReached;

        BacktestEval backTestEval; // The backtest's evaluation
        Order[]      order;        // The orders during the session
        Position[]   position;     // Positions during the session;
        Way_Point[]  wayPoint;     // The price route

        /// <summary>
        /// The bar number
        /// </summary>
        public int Bar { get { return bar; } set { bar = value; } }

        /// <summary>
        /// The number of orders
        /// </summary>
        public int Orders { get { return orders; } set { orders = value; } }

        /// <summary>
        /// The orders during the session
        /// </summary>
        public Order[] Order { get { return order; } set { order = value; } }

        /// <summary>
        /// The number of positions
        /// </summary>
        public int Positions { get { return positions; } set { positions = value; } }

        /// <summary>
        /// The positions during the session
        /// </summary>
        public Position[] Position { get { return position; } set { position = value; } }

        /// <summary>
        /// The position at the end of the session
        /// </summary>
        public Position Summary { get { return positions == 0 ? position[0] : position[positions - 1]; } }

        /// <summary>
        /// The backtest's evaluation
        /// </summary>
        public BacktestEval BacktestEval { get { return backTestEval; } set { backTestEval = value; } }

        /// <summary>
        /// The price route
        /// </summary>
        public Way_Point[] WayPoint { get { return wayPoint; } set { wayPoint = value; } }

        /// <summary>
        /// The count of interpolating steps
        /// </summary>
        public int WayPoints { get { return wayPoints; } set { wayPoints = value; } }

        /// <summary>
        /// Is the top of the bar was reached
        /// </summary>
        public bool IsTopReached { get { return isTopReached; } set { isTopReached = value; } }

        /// <summary>
        /// Is the bottom of the bar was reached
        /// </summary>
        public bool IsBottomReached { get { return isBottomReached; } set { isBottomReached = value; } }

        /// <summary>
        /// Sets a Way Point
        /// </summary>
        public void SetWayPoint(double price, WayPointType type)
        {
            if (positions > 0)
                wayPoint[wayPoints] = new Way_Point(price, type, Summary.FormOrdNumb, Summary.PosNumb);
            else
                wayPoint[wayPoints] = new Way_Point(price, type, -1, -1);

            wayPoints++;

            return;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Session(int bar, int maxPos, int maxOrd)
        {
            this.bar        = bar;
            positions       = 0;
            orders          = 0;
            position        = new Position[maxPos];
            order           = new Order[maxOrd];
            position[0]     = new Position();
            backTestEval    = BacktestEval.None;
            wayPoint        = new Way_Point[15];
            wayPoints       = 0;
            isTopReached    = false;
            isBottomReached = false;
        }
    }
}
