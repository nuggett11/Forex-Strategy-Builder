// Backtester - Interpolation
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Backtester
    /// </summary>
    public partial class Backtester : Data
    {
        /// <summary>
        /// Arranges the orders inside the bar.
        /// </summary>
        static void BarInterpolation(int bar)
        {
            BacktestEval eval = BacktestEval.Error;
            double open    = Open[bar];
            double high    = High[bar];
            double low     = Low[bar];
            double close   = Close[bar];
            double current = open;
            int reachedIntrabar = 0;
            int tradedIntrabar  = -1;
            int reachedTick     = 0;

            do
            {
                Order orderHigher       = null;
                Order orderLower        = null;
                double priceHigher      = high;
                double priceLower       = low;
                bool isHigherPrice      = false;
                bool isLowerPrice       = false;
                bool isTopReachable     = true;
                bool isBottomReachable  = true;
                bool isClosingAmbiguity = false;
                bool isScanningResult   = false;

                // Setting the parameters
                for (int ord = 0; ord < session[bar].Orders; ord++)
                {
                    if (!CheckOrd(bar, ord)) continue;
                    Order order = session[bar].Order[ord];

                    double price0 = order.OrdPrice;
                    double price1 = order.OrdPrice2;

                    if (high + micron > price0 && price0 > low - micron)
                    {
                        if (isTopReachable)
                            isTopReachable = current > price0 + micron;

                        if (isBottomReachable)
                            isBottomReachable = current < price0 - micron;

                        if (price0 > current - micron && price0 < priceHigher + micron)
                        {   // New nearer Upper price
                            isHigherPrice  = true;
                            priceHigher    = price0;
                            orderHigher    = order;
                            isTopReachable = false;
                        }
                        else if (price0 < current && price0 > priceLower - micron)
                        {   // New nearer Lower price
                            isLowerPrice      = true;
                            priceLower        = price0;
                            orderLower        = order;
                            isBottomReachable = false;
                        }
                    }

                    if (high + micron > price1 && price1 > low - micron)
                    {
                        if (isTopReachable)
                            isTopReachable = current > price1 + micron;

                        if (isBottomReachable)
                            isBottomReachable = current < price1 - micron;

                        if (price1 > current - micron && price1 < priceHigher + micron)
                        {   // New nearer Upper price
                            isHigherPrice = true;
                            priceHigher   = price1;
                            orderHigher   = order;
                        }
                        else if (price1 < current && price1 > priceLower - micron)
                        {   // New nearer Lower price
                            isLowerPrice = true;
                            priceLower   = price1;
                            orderLower   = order;
                        }
                    }
                }

                // Evaluate the bar
                if (!isLowerPrice && !isHigherPrice)
                {
                    eval = BacktestEval.None;
                }
                else if (isLowerPrice && isHigherPrice)
                {
                    eval = BacktestEval.Ambiguous;
                }
                else
                {
                    // Check for a Closing Ambiguity
                    if (session[bar].IsBottomReached && session[bar].IsTopReached &&
                        ((current > close - micron && close > priceLower) ||
                         (current < close + micron && close < priceHigher)))
                    {
                        isClosingAmbiguity = true;
                        eval = BacktestEval.Ambiguous;
                    }
                    else
                    {
                        eval = BacktestEval.Correct;
                    }
                }

                //if (session[bar].IsTopReached && isHigherPrice && current > close - micron)
                //{
                //    Console.WriteLine("Ambiguous - " + bar);
                //}
                //else if (session[bar].IsBottomReached && isLowerPrice && current < close + micron)
                //{
                //    Console.WriteLine("Ambiguous - " + bar);
                //}

                if (isScanning && Configs.UseTickData && Data.IsTickData && Data.TickData[bar] != null)
                {
                    isScanningResult = TickScanning(bar, eval,
                        ref current, ref reachedTick,
                        isTopReachable, isBottomReachable,
                        isHigherPrice, isLowerPrice,
                        priceHigher, priceLower,
                        orderHigher, orderLower,
                        isClosingAmbiguity);
                }

                if (isScanning && !isScanningResult && Data.IntraBarsPeriods[bar] != Data.Period)
                {
                    isScanningResult = IntrabarScanning(bar, eval, ref current,
                                ref reachedIntrabar, ref tradedIntrabar,
                                isTopReachable, isBottomReachable,
                                isHigherPrice, isLowerPrice,
                                priceHigher, priceLower,
                                orderHigher, orderLower,
                                isClosingAmbiguity);
                }

                // Calls a method
                if (!isScanningResult)
                {
                    switch (interpolationMethod)
                    {
                        case InterpolationMethod.Optimistic:
                        case InterpolationMethod.Pessimistic:
                            OptimisticPessimisticMethod(bar, eval, ref current,
                                isTopReachable, isBottomReachable,
                                isHigherPrice, isLowerPrice,
                                priceHigher, priceLower,
                                orderHigher, orderLower,
                                isClosingAmbiguity);
                            break;
                        case InterpolationMethod.Shortest:
                            ShortestMethod(bar, eval, ref current,
                                isTopReachable, isBottomReachable,
                                isHigherPrice, isLowerPrice,
                                priceHigher, priceLower,
                                orderHigher, orderLower,
                                isClosingAmbiguity);
                            break;
                        case InterpolationMethod.Nearest:
                            NearestMethod(bar, eval, ref current,
                                isTopReachable, isBottomReachable,
                                isHigherPrice, isLowerPrice,
                                priceHigher, priceLower,
                                orderHigher, orderLower,
                                isClosingAmbiguity);
                            break;
                        case InterpolationMethod.Random:
                            RandomMethod(bar, eval, ref current,
                                isTopReachable, isBottomReachable,
                                isHigherPrice, isLowerPrice,
                                priceHigher, priceLower,
                                orderHigher, orderLower,
                                isClosingAmbiguity);
                            break;
                        default:
                            break;
                    }
                }

            } while (!(eval == BacktestEval.None && session[bar].IsTopReached && session[bar].IsBottomReached));

            return;
        }

        /// <summary>
        /// Tick Scanning
        /// </summary>
        static bool TickScanning(int bar, BacktestEval eval,
            ref double current, ref int reachedTick,
            bool isTopReachable, bool isBottomReachable,
            bool isHigherPrice, bool isLowerPrice,
            double priceHigher, double priceLower,
            Order orderHigher, Order orderLower,
            bool isClosingAmbiguity)
        {
            double high = High[bar];
            double low = Low[bar];
            double close = Close[bar];
            bool isScanningResult = false;

            if (eval == BacktestEval.None)
            {   // There isn't any orders
                if (!session[bar].IsTopReached && !session[bar].IsBottomReached)
                {   // Neither the top nor the bottom was reached
                    int tickCount = Data.TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (Data.TickData[bar][tick] + micron > high)
                        {   // Top found
                            current = high;
                            session[bar].SetWayPoint(high, WayPointType.High);
                            session[bar].IsTopReached = true;
                            isScanningResult = true;
                            break;
                        }
                        else if (Data.TickData[bar][tick] - micron < low)
                        {   // Bottom found
                            current = low;
                            session[bar].SetWayPoint(low, WayPointType.Low);
                            session[bar].IsBottomReached = true;
                            isScanningResult = true;
                            break;
                        }
                    }
                }
                else if (!session[bar].IsTopReached)
                {   // Whether hit the Top
                    int tickCount = Data.TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (Data.TickData[bar][tick] + micron > high)
                        {   // Top found
                            current = high;
                            session[bar].SetWayPoint(high, WayPointType.High);
                            session[bar].IsTopReached = true;
                            isScanningResult = true;
                            break;
                        }
                    }
                }
                else if (!session[bar].IsBottomReached)
                {   // Whether hit the Bottom
                    int tickCount = Data.TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (Data.TickData[bar][tick] - micron < low)
                        {   // Bottom found
                            current = low;
                            session[bar].SetWayPoint(low, WayPointType.Low);
                            session[bar].IsBottomReached = true;
                            isScanningResult = true;
                            break;
                        }
                    }
                }
            }

            if (eval == BacktestEval.Correct)
            {   // Hit the order or the top / bottom
                Order theOrder = null;
                double thePrice = 0;
                if (isHigherPrice)
                {
                    theOrder = orderHigher;
                    thePrice = priceHigher;
                }
                else if (isLowerPrice)
                {
                    theOrder = orderLower;
                    thePrice = priceLower;
                }

                if (!session[bar].IsBottomReached && isBottomReachable)
                {   // The order or the Bottom
                    int tickCount = Data.TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (Data.TickData[bar][tick] + micron > thePrice)
                        {   // The order is reached
                            current = thePrice;
                            ExecOrd(bar, theOrder, thePrice, BacktestEval.Correct);
                            isScanningResult = true;
                            break;
                        }
                        else if (Data.TickData[bar][tick] - micron < low)
                        {   // Bottom is reached
                            current = low;
                            session[bar].SetWayPoint(low, WayPointType.Low);
                            session[bar].IsBottomReached = true;
                            isScanningResult = true;
                            break;
                        }
                    }
                }
                else if (!session[bar].IsTopReached && isTopReachable)
                {   // The order or the Top
                    int tickCount = Data.TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (Data.TickData[bar][tick] + micron > high)
                        {   // The Top is reached
                            current = high;
                            session[bar].SetWayPoint(high, WayPointType.High);
                            session[bar].IsTopReached = true;
                            isScanningResult = true;
                            break;
                        }
                        else if (Data.TickData[bar][tick] - micron < thePrice)
                        {   // The order is reached
                            current = thePrice;
                            ExecOrd(bar, theOrder, thePrice, BacktestEval.Correct);
                            isScanningResult = true;
                            break;
                        }
                    }
                }
                else
                {   // Execute the order
                    double priceOld = Data.TickData[bar][reachedTick];
                    int tickCount = Data.TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (priceOld - micron < thePrice && Data.TickData[bar][tick] + micron > thePrice ||
                            priceOld + micron > thePrice && Data.TickData[bar][tick] - micron < thePrice)
                        {   // Order reached
                            current = thePrice;
                            ExecOrd(bar, theOrder, thePrice, BacktestEval.Correct);
                            isScanningResult = true;
                            break;
                        }
                    }
                }
            }
            else if (eval == BacktestEval.Ambiguous)
            {   // Ambiguous - two orders or order and bar closing
                if (!isClosingAmbiguity)
                {   // Execute the the first reached order
                    int tickCount = Data.TickData[bar].Length;
                    for (int tick = reachedTick; tick < tickCount; tick++)
                    {
                        reachedTick = tick;
                        if (Data.TickData[bar][tick] + micron > priceHigher)
                        {   // Upper order is reached
                            current = priceHigher;
                            ExecOrd(bar, orderHigher, priceHigher, BacktestEval.Correct);
                            isScanningResult = true;
                            break;
                        }
                        else if (Data.TickData[bar][tick] - micron < priceLower)
                        {   // Lower order is reached
                            current = priceLower;
                            ExecOrd(bar, orderLower, priceLower, BacktestEval.Correct);
                            isScanningResult = true;
                            break;
                        }
                    }
                }
                else
                {   // Execute or exit the bar
                    Order  theOrder = null;
                    double thePrice = 0.0;
                    if (isHigherPrice)
                    {
                        theOrder = orderHigher;
                        thePrice = priceHigher;
                    }
                    else if (isLowerPrice)
                    {
                        theOrder = orderLower;
                        thePrice = priceLower;
                    }

                    bool executeOrder = false;
                    if (isHigherPrice)
                    {
                        int tickCount = Data.TickData[bar].Length;
                        for (int tick = reachedTick; tick < tickCount; tick++)
                        {
                            reachedTick = tick;
                            if (Data.TickData[bar][tick] + micron > thePrice)
                            {   // The order is reached
                                executeOrder = true;
                                break;
                            }
                        }
                    }
                    else if (isLowerPrice)
                    {   // The priceLower or Exit the bar
                        int tickCount = Data.TickData[bar].Length;
                        for (int tick = reachedTick; tick < tickCount; tick++)
                        {
                            reachedTick = tick;
                            if (Data.TickData[bar][tick] - micron < thePrice)
                            {   // The order is reached
                                executeOrder = true;
                                break;
                            }
                        }
                    }

                    if (executeOrder)
                    {   // Execute the order
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, BacktestEval.Correct);
                    }
                    else
                    {   // Exit the bar
                        current = close;
                        theOrder.OrdStatus = OrderStatus.Cancelled;
                        session[bar].BacktestEval = BacktestEval.Correct;
                    }

                    isScanningResult = true;
                }
            }

            return isScanningResult;
        }

        /// <summary>
        /// Intrabar Scanning
        /// </summary>
        static bool IntrabarScanning(int bar, BacktestEval eval, ref double current,
            ref int reachedIntrabar, ref int tradedIntrabar,
            bool isTopReachable, bool isBottomReachable,
            bool isHigherPrice, bool isLowerPrice,
            double priceHigher, double priceLower,
            Order orderHigher, Order orderLower,
            bool isClosingAmbiguity)
        {
            double high  = High[bar];
            double low   = Low[bar];
            double close = Close[bar];
            bool isScanningResult = false;

            if (eval == BacktestEval.None)
            {   // There is no more orders
                if (!session[bar].IsTopReached && !session[bar].IsBottomReached)
                {   // Neither the top nor the bottom was reached
                    bool goUpward = false;
                    for (int intraBar = reachedIntrabar; intraBar < Data.IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (Data.IntraBarData[bar][intraBar].High + micron > high)
                        {   // Top found
                            goUpward = true;
                            isScanningResult = true;
                        }

                        if (Data.IntraBarData[bar][intraBar].Low - micron < low)
                        {   // Bottom found
                            if (isScanningResult)
                            {   // Top and Bottom into the same intrabar
                                isScanningResult = false;
                                break;
                            }
                            goUpward = false;
                            isScanningResult = true;
                        }

                        if (isScanningResult)
                            break;
                    }

                    if (isScanningResult)
                    {
                        if (goUpward)
                        {   // Hit the Top
                            current = high;
                            session[bar].SetWayPoint(high, WayPointType.High);
                            session[bar].IsTopReached = true;
                        }
                        else
                        {   // Hit the Bottom
                            current = low;
                            session[bar].SetWayPoint(low, WayPointType.Low);
                            session[bar].IsBottomReached = true;
                        }
                    }
                }
                else if (!session[bar].IsTopReached)
                {   // Whether hit the Top
                    for (int intraBar = reachedIntrabar; intraBar < Data.IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (Data.IntraBarData[bar][intraBar].High + micron > high)
                        {   // Top found
                            current = high;
                            session[bar].SetWayPoint(high, WayPointType.High);
                            session[bar].IsTopReached = true;
                            isScanningResult = true;
                            break;
                        }
                    }
                }
                else if (!session[bar].IsBottomReached)
                {   // Whether hit the Bottom
                    for (int intraBar = reachedIntrabar; intraBar < Data.IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (Data.IntraBarData[bar][intraBar].Low - micron < low)
                        {   // Bottom found
                            current = low;
                            session[bar].SetWayPoint(low, WayPointType.Low);
                            session[bar].IsBottomReached = true;
                            isScanningResult = true;
                            break;
                        }
                    }
                }
            }

            if (eval == BacktestEval.Correct)
            {   // Hit the order or the top / bottom
                Order  theOrder = null;
                double thePrice = 0;
                if (isHigherPrice)
                {
                    theOrder = orderHigher;
                    thePrice = priceHigher;
                }
                else if (isLowerPrice)
                {
                    theOrder = orderLower;
                    thePrice = priceLower;
                }

                if (!session[bar].IsBottomReached && isBottomReachable)
                {   // The order or the bottom
                    bool goUpward = false;
                    for (int intraBar = reachedIntrabar; intraBar < Data.IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (Data.IntraBarData[bar][intraBar].High + micron > thePrice)
                        {   // The order is reached
                            goUpward = true;
                            isScanningResult = true;
                        }

                        if (Data.IntraBarData[bar][intraBar].Low - micron < low)
                        {   // Bottom is reached
                            if (isScanningResult)
                            {   // The Order and Bottom into the same intrabar
                                isScanningResult = false;
                                break;
                            }
                            goUpward = false;
                            isScanningResult = true;
                        }

                        if (isScanningResult)
                            break;
                    }

                    if (isScanningResult)
                    {
                        if (goUpward)
                        {   // Execute
                            if (tradedIntrabar == reachedIntrabar)
                            {
                                isScanningResult = false;
                                return isScanningResult;
                            }
                            current = thePrice;
                            ExecOrd(bar, theOrder, thePrice, eval);
                            tradedIntrabar = reachedIntrabar;
                        }
                        else
                        {
                            // Hit the Bottom
                            current = low;
                            session[bar].SetWayPoint(low, WayPointType.Low);
                            session[bar].IsBottomReached = true;
                        }
                    }
                }
                else if (!session[bar].IsTopReached && isTopReachable)
                {   // The order or the Top
                    bool goUpward = false;
                    for (int intraBar = reachedIntrabar; intraBar < Data.IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (Data.IntraBarData[bar][intraBar].High + micron > high)
                        {   // The Top is reached
                            goUpward = true;
                            isScanningResult = true;
                        }

                        if (Data.IntraBarData[bar][intraBar].Low - micron < thePrice)
                        {   // The order is reached
                            if (isScanningResult)
                            {   // The Top and the order are into the same intrabar
                                isScanningResult = false;
                                break;
                            }

                            // The order is reachable downlards
                            goUpward = false;
                            isScanningResult = true;
                        }

                        if (isScanningResult)
                            break;
                    }

                    if (isScanningResult)
                    {
                        if (goUpward)
                        {   // Hit the Top
                            current = high;
                            session[bar].SetWayPoint(high, WayPointType.High);
                            session[bar].IsTopReached = true;
                        }
                        else
                        {   // Execute
                            if (tradedIntrabar == reachedIntrabar)
                            {
                                isScanningResult = false;
                                return isScanningResult;
                            }
                            current = thePrice;
                            ExecOrd(bar, theOrder, thePrice, eval);
                            tradedIntrabar = reachedIntrabar;
                        }
                    }
                }
                else
                {   // Execute the order
                    for (int intraBar = reachedIntrabar; intraBar < Data.IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (Data.IntraBarData[bar][intraBar].High + micron > thePrice &&
                            Data.IntraBarData[bar][intraBar].Low  - micron < thePrice)
                        {   // Order reached
                            if (tradedIntrabar == reachedIntrabar)
                            {
                                isScanningResult = false;
                                return isScanningResult;
                            }
                            current = thePrice;
                            ExecOrd(bar, theOrder, thePrice, eval);
                            isScanningResult = true;
                            tradedIntrabar = reachedIntrabar;
                            break;
                        }
                    }
                }
            }
            else if (eval == BacktestEval.Ambiguous)
            {   // Ambiguous - two orders or order and bar closing
                if (!isClosingAmbiguity)
                {   // Execute the the first reached order
                    bool executeUpper = false;
                    for (int intraBar = reachedIntrabar; intraBar < Data.IntraBarBars[bar]; intraBar++)
                    {
                        reachedIntrabar = intraBar;

                        if (Data.IntraBarData[bar][intraBar].High + micron > priceHigher)
                        {   // Upper order is reached
                            executeUpper = true;
                            isScanningResult = true;
                        }

                        if (Data.IntraBarData[bar][intraBar].Low - micron < priceLower)
                        {   // Lower order is reached
                            if (isScanningResult)
                            {   // Top and Bottom into the same intrabar
                                isScanningResult = false;
                                break;
                            }
                            executeUpper = false;
                            isScanningResult = true;
                        }

                        if (isScanningResult)
                            break;
                    }

                    if (isScanningResult)
                    {
                        Order theOrder;
                        double thePrice;
                        if (executeUpper)
                        {
                            theOrder = orderHigher;
                            thePrice = priceHigher;
                        }
                        else
                        {
                            theOrder = orderLower;
                            thePrice = priceLower;
                        }

                        if (tradedIntrabar == reachedIntrabar)
                        {
                            isScanningResult = false;
                            return isScanningResult;
                        }
                        eval = BacktestEval.Correct;
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                        tradedIntrabar = reachedIntrabar;
                    }
                }
                else
                {   // Execute or exit the bar
                    Order  theOrder = null;
                    double thePrice = 0;
                    if (isHigherPrice)
                    {
                        theOrder = orderHigher;
                        thePrice = priceHigher;
                    }
                    else if (isLowerPrice)
                    {
                        theOrder = orderLower;
                        thePrice = priceLower;
                    }

                    bool executeOrder = false;
                    if (isHigherPrice)
                    {
                        for (int intraBar = reachedIntrabar; intraBar < Data.IntraBarBars[bar]; intraBar++)
                        {
                            reachedIntrabar = intraBar;

                            if (Data.IntraBarData[bar][intraBar].High + micron > thePrice)
                            {   // The order is reached
                                executeOrder = true;
                                break;
                            }
                        }
                    }
                    else if (isLowerPrice)
                    {   // The priceLower or Exit the bar
                        for (int b = reachedIntrabar; b < Data.IntraBarBars[bar]; b++)
                        {
                            reachedIntrabar = b;

                            if (Data.IntraBarData[bar][b].Low - micron < thePrice)
                            {   // The order is reached
                                executeOrder = true;
                                break;
                            }
                        }
                    }

                    if (executeOrder)
                    {   // Execute the order
                        if (tradedIntrabar == reachedIntrabar)
                        {
                            isScanningResult = false;
                            return isScanningResult;
                        }
                        current = thePrice;
                        eval = BacktestEval.Correct;
                        ExecOrd(bar, theOrder, thePrice, eval);
                        tradedIntrabar = reachedIntrabar;
                    }
                    else
                    {   // Exit the bar
                        current = close;
                        theOrder.OrdStatus = OrderStatus.Cancelled;
                        session[bar].BacktestEval = BacktestEval.Correct;
                    }
                    isScanningResult = true;
                }
            }

            return isScanningResult;
        }

        /// <summary>
        /// Random Execution Method
        /// </summary>
        static void RandomMethod(int bar, BacktestEval eval, ref double current,
            bool isTopReachable, bool isBottomReachable,
            bool isHigherPrice, bool isLowerPrice,
            double priceHigher, double priceLower,
            Order orderHigher, Order orderLower,
            bool isClosingAmbiguity)
        {
            double high  = High[bar];
            double low   = Low[bar];
            double close = Close[bar];

            if (eval == BacktestEval.None)
            {   // There is no more orders
                if (!session[bar].IsTopReached && !session[bar].IsBottomReached)
                {   // Neither the top nor the bottom was reached
                    int upRange = (int)Math.Round((high - current) / Data.InstrProperties.Point);
                    int downRange = (int)Math.Round((current - low) / Data.InstrProperties.Point);
                    upRange = upRange < 0 ? 0 : upRange;
                    downRange = downRange < 0 ? 0 : downRange;
                    if (downRange + downRange == 0)
                    {
                        upRange   = 1;
                        downRange = 1;
                    }
                    int iRandom = random.Next(upRange + downRange);
                    bool bHitHigh = false;

                    if (upRange > downRange)
                        bHitHigh = iRandom > upRange;
                    else
                        bHitHigh = iRandom < downRange;

                    if (bHitHigh)
                    {   // Hit the Top
                        current = high;
                        session[bar].SetWayPoint(high, WayPointType.High);
                        session[bar].IsTopReached = true;
                    }
                    else
                    {   // Hit the Bottom
                        current = low;
                        session[bar].SetWayPoint(low, WayPointType.Low);
                        session[bar].IsBottomReached = true;
                    }
                }
                else if (!session[bar].IsTopReached)
                {   // Hit the Top
                    current = high;
                    session[bar].SetWayPoint(high, WayPointType.High);
                    session[bar].IsTopReached = true;
                }
                else if (!session[bar].IsBottomReached)
                {   // Hit the Bottom
                    current = low;
                    session[bar].SetWayPoint(low, WayPointType.Low);
                    session[bar].IsBottomReached = true;
                }
            }
            if (eval == BacktestEval.Correct)
            {   // Hit the order or the top/bottom
                Order theOrder = null;
                double thePrice = 0;
                if (isHigherPrice)
                {
                    theOrder = orderHigher;
                    thePrice = priceHigher;
                }
                else if (isLowerPrice)
                {
                    theOrder = orderLower;
                    thePrice = priceLower;
                }

                if (!session[bar].IsBottomReached && isBottomReachable)
                {   // The order or the bottom
                    int iUpRange = (int)Math.Round((thePrice - current) / Data.InstrProperties.Point);
                    int iDnRange = (int)Math.Round((current - low) / Data.InstrProperties.Point);
                    iUpRange = iUpRange < 0 ? 0 : iUpRange;
                    iDnRange = iDnRange < 0 ? 0 : iDnRange;
                    if (iDnRange + iDnRange == 0)
                    {
                        iUpRange = 1;
                        iDnRange = 1;
                    }
                    int iRandom = random.Next(iUpRange + iDnRange);
                    bool executeUpper = false;

                    if (iUpRange > iDnRange)
                        executeUpper = iRandom > iUpRange;
                    else
                        executeUpper = iRandom < iDnRange;

                    if (executeUpper)
                    {   // Execute
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                    else
                    {
                        // Hit the Bottom
                        current = low;
                        session[bar].SetWayPoint(low, WayPointType.Low);
                        session[bar].IsBottomReached = true;
                    }
                }
                else if (!session[bar].IsTopReached && isTopReachable)
                {   // The order or the Top
                    int upRange   = (int)Math.Round((high - current) / Data.InstrProperties.Point);
                    int downRange = (int)Math.Round((current - thePrice) / Data.InstrProperties.Point);
                    upRange   = upRange   < 0 ? 0 : upRange;
                    downRange = downRange < 0 ? 0 : downRange;
                    if (downRange + downRange == 0)
                    {
                        upRange   = 1;
                        downRange = 1;
                    }
                    int iRandom = random.Next(upRange + downRange);
                    bool executeUpper = false;

                    if (upRange > downRange)
                        executeUpper = iRandom > upRange;
                    else
                        executeUpper = iRandom < downRange;

                    if (executeUpper)
                    {   // Hit the Top
                        current = high;
                        session[bar].SetWayPoint(high, WayPointType.High);
                        session[bar].IsTopReached = true;
                    }
                    else
                    {   // Execute
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                }
                else
                {   // Execute the order
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
            }
            else if (eval == BacktestEval.Ambiguous)
            {   // Ambiguouse - two orders or order and bar closing
                if (!isClosingAmbiguity)
                {   // Execute the randomly chosen order
                    int upRange   = (int)Math.Round((priceHigher - current) / Data.InstrProperties.Point);
                    int downRange = (int)Math.Round((current - priceLower) / Data.InstrProperties.Point);
                    upRange   = upRange   < 0 ? 0 : upRange;
                    downRange = downRange < 0 ? 0 : downRange;
                    if (downRange + downRange == 0)
                    {
                        upRange   = 1;
                        downRange = 1;
                    }
                    int iRandom = random.Next(upRange + downRange);
                    bool executeUpper = false;

                    if (upRange > downRange)
                        executeUpper = iRandom > upRange;
                    else
                        executeUpper = iRandom < downRange;

                    Order theOrder;
                    double thePrice;
                    if (executeUpper)
                    {
                        theOrder = orderHigher;
                        thePrice = priceHigher;
                    }
                    else
                    {
                        theOrder = orderLower;
                        thePrice = priceLower;
                    }
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
                else
                {   // Execute or exit the bar
                    if (isHigherPrice)
                    {
                        int upRange   = (int)Math.Round((priceHigher - current) / Data.InstrProperties.Point);
                        int downRange = (int)Math.Round((close - current) / Data.InstrProperties.Point);
                        upRange   = upRange   < 0 ? 0 : upRange;
                        downRange = downRange < 0 ? 0 : downRange;
                        if (downRange + downRange == 0)
                        {
                            upRange   = 1;
                            downRange = 0;
                        }
                        int iRandom = random.Next(upRange + downRange);

                        if (iRandom > upRange)
                        {   // Execute the order
                            current = priceHigher;
                            ExecOrd(bar, orderHigher, priceHigher, eval);
                        }
                        else
                        {   // Exit the bar
                            current = close;
                            orderHigher.OrdStatus = OrderStatus.Cancelled;
                            session[bar].BacktestEval = BacktestEval.Ambiguous;
                        }
                    }
                    else if (isLowerPrice)
                    {   // The priceLower or Exit the bar
                        int upRange   = (int)Math.Round((current - close) / Data.InstrProperties.Point);
                        int downRange = (int)Math.Round((current - priceLower) / Data.InstrProperties.Point);
                        upRange   = upRange   < 0 ? 0 : upRange;
                        downRange = downRange < 0 ? 0 : downRange;
                        if (downRange + downRange == 0)
                        {
                            upRange   = 0;
                            downRange = 1;
                        }
                        int iRandom = random.Next(upRange + downRange);

                        if (iRandom > downRange)
                        {   // Execute the order
                            current = priceLower;
                            ExecOrd(bar, orderLower, priceLower, eval);
                        }
                        else
                        {   // Exit the bar
                            current = close;
                            orderLower.OrdStatus = OrderStatus.Cancelled;
                            session[bar].BacktestEval = BacktestEval.Ambiguous;
                        }
                    }
                }
            }

            return;
        }

        /// <summary>
        /// Nearest order first Method
        /// </summary>
        static void NearestMethod(int bar, BacktestEval eval, ref double current,
            bool isTopReachable, bool isBottomReachable,
            bool isHigherPrice, bool isLowerPrice,
            double priceHigher, double priceLower,
            Order orderHigher, Order orderLower,
            bool isClosingAmbiguity)
        {
            double open  = Open[bar];
            double high  = High[bar];
            double low   = Low[bar];
            double close = Close[bar];

            if (eval == BacktestEval.None)
            {   // There is no more orders
                if (!session[bar].IsTopReached && !session[bar].IsBottomReached)
                {   // Neither the top nor the bottom was reached
                    if (close < open)
                    {   // Hit the Top
                        current = high;
                        session[bar].SetWayPoint(high, WayPointType.High);
                        session[bar].IsTopReached = true;
                    }
                    else
                    {   // Hit the Bottom
                        current = low;
                        session[bar].SetWayPoint(low, WayPointType.Low);
                        session[bar].IsBottomReached = true;
                    }
                }
                else if (!session[bar].IsTopReached)
                {   // Hit the Top
                    current = high;
                    session[bar].SetWayPoint(high, WayPointType.High);
                    session[bar].IsTopReached = true;
                }
                else if (!session[bar].IsBottomReached)
                {   // Hit the Bottom
                    current = low;
                    session[bar].SetWayPoint(low, WayPointType.Low);
                    session[bar].IsBottomReached = true;
                }
            }
            if (eval == BacktestEval.Correct)
            {   // Hit the order or the top/bottom
                Order theOrder = null;
                double thePrice = 0;
                if (isHigherPrice)
                {
                    theOrder = orderHigher;
                    thePrice = priceHigher;
                }
                else if (isLowerPrice)
                {
                    theOrder = orderLower;
                    thePrice = priceLower;
                }

                if (!session[bar].IsBottomReached && isBottomReachable)
                {   // The order or the bottom
                    double upRange   = thePrice - current;
                    double downRange = current - low;

                    if (upRange < downRange)
                    {   // Execute
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                    else
                    {   // Hit the Bottom
                        current = low;
                        session[bar].SetWayPoint(low, WayPointType.Low);
                        session[bar].IsBottomReached = true;
                    }
                }
                else if (!session[bar].IsTopReached && isTopReachable)
                {   // The order or the bottom
                    double upRange   = high - current;
                    double downRange = current - thePrice;

                    if (upRange < downRange)
                    {   // Hit the Top
                        current = high;
                        session[bar].SetWayPoint(high, WayPointType.High);
                        session[bar].IsTopReached = true;
                    }
                    else
                    {   // Execute
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                }
                else
                {   // Execute the order
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
            }
            else if (eval == BacktestEval.Ambiguous)
            {   // Ambiguouse - two orders or order and bar closing
                if (!isClosingAmbiguity)
                {   // Execute the nearest order
                    double upRange   = priceHigher - current;
                    double downRange = current - priceLower;

                    Order  theOrder;
                    double thePrice;
                    if (upRange < downRange)
                    {
                        theOrder = orderHigher;
                        thePrice = priceHigher;
                    }
                    else
                    {
                        theOrder = orderLower;
                        thePrice = priceLower;
                    }
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
                else
                {   // Exit the bar
                    current = close;
                    session[bar].BacktestEval = BacktestEval.Ambiguous;
                    if (isHigherPrice)
                        orderHigher.OrdStatus = OrderStatus.Cancelled;
                    else if (isLowerPrice)
                        orderLower.OrdStatus = OrderStatus.Cancelled;
                }
            }

            return;
        }

        /// <summary>
        /// Shortest route inside the bar Method
        /// </summary>
        static void ShortestMethod(int bar, BacktestEval eval, ref double current,
            bool isTopReachable, bool isBottomReachable,
            bool isHigherPrice, bool isLowerPrice,
            double priceHigher, double priceLower,
            Order orderHigher, Order orderLower,
            bool isClosingAmbiguity)
        {
            double open  = Open[bar];
            double high  = High[bar];
            double low   = Low[bar];
            double close = Close[bar];

            bool isGoUpward;
            if (!session[bar].IsTopReached && !session[bar].IsBottomReached)
                isGoUpward = open > close;
            else if (session[bar].IsTopReached && !session[bar].IsBottomReached)
                isGoUpward = false;
            else if (!session[bar].IsTopReached && session[bar].IsBottomReached)
                isGoUpward = true;
            else
                isGoUpward = open > close;

            if (isLowerPrice && current - priceLower < micron)
                isGoUpward = false;
            if (isHigherPrice && priceHigher - current < micron)
                isGoUpward = true;

            if (eval == BacktestEval.None)
            {   // There is no more orders
                if (!session[bar].IsTopReached && !session[bar].IsBottomReached)
                {   // Neither the top nor the bottom was reached
                    if (isGoUpward)
                    {   // Hit the Top
                        current = high;
                        session[bar].SetWayPoint(high, WayPointType.High);
                        session[bar].IsTopReached = true;
                    }
                    else
                    {   // Hit the Bottom
                        current = low;
                        session[bar].SetWayPoint(low, WayPointType.Low);
                        session[bar].IsBottomReached = true;
                    }
                }
                else if (!session[bar].IsTopReached)
                {   // Hit the Top
                    current = high;
                    session[bar].SetWayPoint(high, WayPointType.High);
                    session[bar].IsTopReached = true;
                }
                else if (!session[bar].IsBottomReached)
                {   // Hit the Bottom
                    current = low;
                    session[bar].SetWayPoint(low, WayPointType.Low);
                    session[bar].IsBottomReached = true;
                }
            }
            if (eval == BacktestEval.Correct)
            {   // Hit the top/bottom or execute
                Order  theOrder = null;
                double thePrice = 0;
                if (isHigherPrice)
                {
                    theOrder = orderHigher;
                    thePrice = priceHigher;
                }
                else if (isLowerPrice)
                {
                    theOrder = orderLower;
                    thePrice = priceLower;
                }

                if (!session[bar].IsBottomReached && isBottomReachable && !isGoUpward)
                {   // Hit the Bottom
                    current = low;
                    session[bar].SetWayPoint(low, WayPointType.Low);
                    session[bar].IsBottomReached = true;
                }
                else if (!session[bar].IsTopReached && isTopReachable && isGoUpward)
                {   // Hit the Top
                    current = high;
                    session[bar].SetWayPoint(high, WayPointType.High);
                    session[bar].IsTopReached = true;
                }
                else
                {   // Execute the order
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
            }
            else if (eval == BacktestEval.Ambiguous)
            {   // Ambiguouse - two orders or order and bar closing
                if (!isClosingAmbiguity)
                {   // Execute the nearest order
                    Order  theOrder;
                    double thePrice;

                    if (isGoUpward)
                    {
                        theOrder = orderHigher;
                        thePrice = priceHigher;
                    }
                    else
                    {
                        theOrder = orderLower;
                        thePrice = priceLower;
                    }
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
                else
                {   // Exit the bar
                    current = close;
                    session[bar].BacktestEval = BacktestEval.Ambiguous;
                    if (isHigherPrice)
                        orderHigher.OrdStatus = OrderStatus.Cancelled;
                    else if (isLowerPrice)
                        orderLower.OrdStatus = OrderStatus.Cancelled;
                }
            }

            return;
        }

        /// <summary>
        /// Optimistic / Pessimistic Method
        /// </summary>
        static void OptimisticPessimisticMethod(int bar, BacktestEval eval, ref double current,
            bool isTopReachable, bool isBottomReachable,
            bool isHigherPrice, bool isLowerPrice,
            double priceHigher, double priceLower,
            Order orderHigher, Order orderLower,
            bool isClosingAmbiguity)
        {
            double open  = Open[bar];
            double high  = High[bar];
            double low   = Low[bar];
            double close = Close[bar];

            bool isOptimistic = interpolationMethod == InterpolationMethod.Optimistic;

            if (eval == BacktestEval.None)
            {   // There is no more orders
                if (!session[bar].IsTopReached && !session[bar].IsBottomReached)
                {   // Neither the top nor the bottom was reached
                    if (close < open)
                    {   // Hit the Top
                        current = high;
                        session[bar].SetWayPoint(high, WayPointType.High);
                        session[bar].IsTopReached = true;
                    }
                    else
                    {   // Hit the Bottom
                        current = low;
                        session[bar].SetWayPoint(low, WayPointType.Low);
                        session[bar].IsBottomReached = true;
                    }
                }
                else if (!session[bar].IsTopReached)
                {   // Hit the Top
                    current = high;
                    session[bar].SetWayPoint(high, WayPointType.High);
                    session[bar].IsTopReached = true;
                }
                else if (!session[bar].IsBottomReached)
                {   // Hit the Bottom
                    current = low;
                    session[bar].SetWayPoint(low, WayPointType.Low);
                    session[bar].IsBottomReached = true;
                }
            }
            if (eval == BacktestEval.Correct)
            {   // Hit the order or the top/bottom
                Order  theOrder = null;
                double thePrice = 0;
                if (isHigherPrice)
                {
                    theOrder = orderHigher;
                    thePrice = priceHigher;
                }
                else if (isLowerPrice)
                {
                    theOrder = orderLower;
                    thePrice = priceLower;
                }

                if (!session[bar].IsBottomReached && isBottomReachable)
                {   // The order or the bottom
                    bool goUpward;

                    if (current - low < micron)
                        goUpward = false;
                    else if (thePrice - current < micron)
                        goUpward = true;
                    else if (theOrder.OrdDir == OrderDirection.Buy)
                        goUpward = !isOptimistic;
                    else if (theOrder.OrdDir == OrderDirection.Sell)
                        goUpward = isOptimistic;
                    else
                        goUpward = thePrice - current < current - low;

                    if (goUpward)
                    {   // Execute order
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                    else
                    {   // Hit the Bottom
                        current = low;
                        session[bar].SetWayPoint(low, WayPointType.Low);
                        session[bar].IsBottomReached = true;
                    }
                }
                else if (!session[bar].IsTopReached && isTopReachable)
                {   // The order or the top
                    bool goUpward;

                    if (current - high < micron)
                        goUpward = true;
                    else if (current - thePrice < micron)
                        goUpward = false;
                    else if (theOrder.OrdDir == OrderDirection.Buy)
                        goUpward = !isOptimistic;
                    else if (theOrder.OrdDir == OrderDirection.Sell)
                        goUpward = isOptimistic;
                    else
                        goUpward = high - current < current - thePrice;

                    if (goUpward)
                    {   // Hit the Top
                        current = high;
                        session[bar].SetWayPoint(high, WayPointType.High);
                        session[bar].IsTopReached = true;
                    }
                    else
                    {   // Execute order
                        current = thePrice;
                        ExecOrd(bar, theOrder, thePrice, eval);
                    }
                }
                else
                {   // Execute the order
                    current = thePrice;
                    ExecOrd(bar, theOrder, thePrice, eval);
                }
            }
            else if (eval == BacktestEval.Ambiguous)
            {   // Ambiguous - two orders or order and bar closing
                if (!isClosingAmbiguity)
                {   // Execute one of both orders
                    bool executeUpper;

                    if (priceHigher - current < micron)
                        executeUpper = true;
                    else if (current - priceLower < micron)
                        executeUpper = false;
                    else if (session[bar].Summary.PosDir == PosDirection.Long)
                        executeUpper = isOptimistic;
                    else if (session[bar].Summary.PosDir == PosDirection.Short)
                        executeUpper = !isOptimistic;
                    else
                    {
                        if (orderHigher.OrdDir == OrderDirection.Buy && orderLower.OrdDir == OrderDirection.Buy)
                            executeUpper = !isOptimistic;
                        else if (orderHigher.OrdDir == OrderDirection.Sell && orderLower.OrdDir == OrderDirection.Sell)
                            executeUpper = isOptimistic;
                        else if (orderHigher.OrdDir == OrderDirection.Buy && orderLower.OrdDir == OrderDirection.Sell)
                        {
                            if (current < close)
                                executeUpper = isOptimistic;
                            else
                                executeUpper = !isOptimistic;

                            if (Data.Strategy.OppSignalAction == OppositeDirSignalAction.Reverse)
                                executeUpper = !executeUpper;
                        }
                        else
                        {
                            if (current < close)
                                executeUpper = !isOptimistic;
                            else
                                executeUpper = isOptimistic;

                            if (Data.Strategy.OppSignalAction == OppositeDirSignalAction.Reverse)
                                executeUpper = !executeUpper;
                        }
                    }

                    Order theOrder;
                    double thePrice;
                    if (executeUpper)
                    {
                        theOrder = orderHigher;
                        thePrice = priceHigher;
                    }
                    else
                    {
                        theOrder = orderLower;
                        thePrice = priceLower;
                    }
                    current = thePrice;

                    ExecOrd(bar, theOrder, thePrice, eval);
                }
                else
                {   // Execute or exit the bar
                    if (isHigherPrice)
                    {
                        bool toExecute = false;
                        if (session[bar].Summary.PosDir == PosDirection.Long)
                            toExecute = isOptimistic;
                        else if (session[bar].Summary.PosDir == PosDirection.Short)
                            toExecute = !isOptimistic;
                        else if (orderHigher.OrdDir == OrderDirection.Buy)
                            toExecute = !isOptimistic;
                        else if (orderHigher.OrdDir == OrderDirection.Sell)
                            toExecute = isOptimistic;

                        if (toExecute)
                        {   // Execute
                            current = priceHigher;
                            ExecOrd(bar, orderHigher, priceHigher, eval);
                        }
                        else
                        {   // Exit the bar
                            current = close;
                            orderHigher.OrdStatus = OrderStatus.Cancelled;
                            session[bar].BacktestEval = BacktestEval.Ambiguous;
                        }
                    }
                    else if (isLowerPrice)
                    {   // The priceLower or Exit the bar
                        bool toExecute = false;

                        if (session[bar].Summary.PosDir == PosDirection.Long)
                            toExecute = !isOptimistic;
                        else if (session[bar].Summary.PosDir == PosDirection.Short)
                            toExecute = isOptimistic;
                        else if (orderLower.OrdDir == OrderDirection.Buy)
                            toExecute = isOptimistic;
                        else if (orderLower.OrdDir == OrderDirection.Sell)
                            toExecute = !isOptimistic;

                        if (toExecute)
                        {   // Execute
                            current = priceLower;
                            ExecOrd(bar, orderLower, priceLower, eval);
                        }
                        else
                        {   // Exit the bar
                            current = close;
                            orderLower.OrdStatus = OrderStatus.Cancelled;
                            session[bar].BacktestEval = BacktestEval.Ambiguous;
                        }
                    }
                }
            }

            return;
        }
    }
}
