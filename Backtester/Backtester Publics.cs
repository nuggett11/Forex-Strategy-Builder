// Backtester - Publics
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;

namespace Forex_Strategy_Builder
{
    public enum StrategyPriceType
    {
        Open, Close, Indicator, CloseAndReverce, Unknown
    }

    public enum InterpolationMethod
    {
        Pessimistic, Shortest, Nearest, Optimistic, Random
    }

    /// <summary>
    /// The backtest evaluation.
    /// </summary>
    public enum BacktestEval { Error, None, Ambiguous, Unknown, Correct }

    /// <summary>
    /// Class Market.
    /// </summary>
    public partial class Backtester : Data
    {
        /// <summary>
        /// Gets or sets the Interpolation Method
        /// </summary>
        public static InterpolationMethod InterpolationMethod
        {
            get { return interpolationMethod; }
            set { interpolationMethod = value; }
        }

        /// <summary>
        /// Interpolation Method string.
        /// </summary>
        public static String InterpolationMethodToString()
        {
            string method = "";
            switch (interpolationMethod)
            {
                case InterpolationMethod.Pessimistic:
                    method = Language.T("Pessimistic scenario");
                    break;
                case InterpolationMethod.Shortest:
                    method = Language.T("Shortest bar route");
                    break;
                case InterpolationMethod.Nearest:
                    method = Language.T("Nearest order first");
                    break;
                case InterpolationMethod.Optimistic:
                    method = Language.T("Optimistic scenario");
                    break;
                case InterpolationMethod.Random:
                    method = Language.T("Random execution");
                    break;
                default:
                    method = Language.T("Error");
                    break;
            }

            return method;
        }

        /// <summary>
        /// Interpolation Method string.
        /// </summary>
        public static String InterpolationMethodShortToString()
        {
            string method = "";
            switch (interpolationMethod)
            {
                case InterpolationMethod.Pessimistic:
                    method = Language.T("Pessimistic");
                    break;
                case InterpolationMethod.Shortest:
                    method = Language.T("Shortest");
                    break;
                case InterpolationMethod.Nearest:
                    method = Language.T("Nearest");
                    break;
                case InterpolationMethod.Optimistic:
                    method = Language.T("Optimistic");
                    break;
                case InterpolationMethod.Random:
                    method = Language.T("Random");
                    break;
                default:
                    method = Language.T("Error");
                    break;
            }

            return method;
        }

        /// <summary>
        /// Gets the position coordinates.
        /// </summary>
        public static PositionCoordinates[] PosCoordinates { get { return posCoord; } }

        /// <summary>
        /// Gets the total number of the positions.
        /// </summary>
        public static int PositionsTotal { get { return totalPositions; } }

        /// <summary>
        /// Number of the positions during de session.
        /// </summary>
        public static int Positions(int bar)
        {
            return session[bar].Positions;
        }

        /// <summary>
        /// Checks whether we have got a position. "Closed" is also a position.
        /// </summary>
        public static bool IsPos(int bar)
        {
            bool isPosition =
                session[bar].Summary.PosDir == PosDirection.Long  ||
                session[bar].Summary.PosDir == PosDirection.Short ||
                session[bar].Summary.PosDir == PosDirection.Closed;

            return isPosition;
        }

        /// <summary>
        /// Last Position's number.
        /// </summary>
        public static int SummaryPosNumb(int bar)
        {
            return session[bar].Summary.PosNumb;
        }

        /// <summary>
        /// Last Position's order number.
        /// </summary>
        public static int SummaryOrdNumb(int bar)
        {
            return session[bar].Summary.FormOrdNumb;
        }

        /// <summary>
        /// Position direction at the end of the bar
        /// </summary>
        public static PosDirection SummaryDir(int bar)
        {
            return session[bar].Summary.PosDir;
        }

        /// <summary>
        /// Position lots at the end of the bar.
        /// </summary>
        public static double SummaryLots(int bar)
        {
            return session[bar].Summary.PosLots;
        }

        /// <summary>
        /// Position amount at the end of the bar.
        /// </summary>
        public static int SummaryAmount(int bar)
        {
            return (int)Math.Round(session[bar].Summary.PosLots * Data.InstrProperties.LotSize);
        }

        /// <summary>
        /// The last transaction for the bar.
        /// </summary>
        public static Transaction SummaryTrans(int bar)
        {
            return session[bar].Summary.Transaction;
        }

        /// <summary>
        /// Position price at the end of the bar.
        /// </summary>
        public static double SummaryPrice(int bar)
        {
            return session[bar].Summary.PosPrice;
        }

        /// <summary>
        /// Returns the position Order Price at the end of the bar.
        /// </summary>
        public static double SummaryOrdPrice(int bar)
        {
            return session[bar].Summary.FormOrdPrice;
        }

        /// <summary>
        /// Returns the Absolute Permanent SL
        /// </summary>
        public static double SummaryAbsoluteSL(int bar)
        {
            return session[bar].Summary.AbsoluteSL;
        }

        /// <summary>
        /// Returns the Absolute Permanent TP
        /// </summary>
        public static double SummaryAbsoluteTP(int bar)
        {
            return session[bar].Summary.AbsoluteTP;
        }

        /// <summary>
        /// Returns the Required Margin at the end of the bar
        /// </summary>
        public static double SummaryRequiredMargin(int bar)
        {
            return session[bar].Summary.RequiredMargin;
        }

        /// <summary>
        /// Returns the Free Margin at the end of the bar
        /// </summary>
        public static double SummaryFreeMargin(int bar)
        {
            return session[bar].Summary.FreeMargin;
        }

        /// <summary>
        /// Position icon at the end of the bar
        /// </summary>
        public static Image SummaryPositionIcon(int bar)
        {
            return session[bar].Summary.PositionIcon;
        }

        /// <summary>
        /// The number of the position
        /// </summary>
        public static int PosNumb(int bar, int pos)
        {
            return session[bar].Position[pos].PosNumb;
        }

        /// <summary>
        /// The position direction
        /// </summary>
        public static PosDirection PosDir(int bar, int pos)
        {
            return session[bar].Position[pos].PosDir;
        }

        /// <summary>
        /// The position lots
        /// </summary>
        public static double PosLots(int bar, int pos)
        {
            return session[bar].Position[pos].PosLots;
        }

        /// <summary>
        /// The position amount in currency
        /// </summary>
        public static int PosAmount(int bar, int pos)
        {
            return (int)session[bar].Position[pos].PosLots * Data.InstrProperties.LotSize;
        }

        /// <summary>
        /// The position forming order number
        /// </summary>
        public static int PosOrdNumb(int bar, int pos)
        {
            return session[bar].Position[pos].FormOrdNumb;
        }

        /// <summary>
        /// The position forming order price
        /// </summary>
        public static double PosOrdPrice(int bar, int pos)
        {
            return session[bar].Position[pos].FormOrdPrice;
        }

        /// <summary>
        /// The position Required Margin
        /// </summary>
        public static double PosRequiredMargin(int bar, int pos)
        {
            return session[bar].Position[pos].RequiredMargin;
        }

        /// <summary>
        /// The position Free Margin
        /// </summary>
        public static double PosFreeMargin(int bar, int pos)
        {
            return session[bar].Position[pos].FreeMargin;
        }

        /// <summary>
        /// The position balance
        /// </summary>
        public static double PosBalance(int bar, int pos)
        {
            return session[bar].Position[pos].Balance;
        }

        /// <summary>
        /// The position equity
        /// </summary>
        public static double PosEquity(int bar, int pos)
        {
            return session[bar].Position[pos].Equity;
        }

        /// <summary>
        /// The position Profit Loss
        /// </summary>
        public static double PosProfitLoss(int bar, int pos)
        {
            return session[bar].Position[pos].ProfitLoss;
        }

        /// <summary>
        /// The position Floating P/L
        /// </summary>
        public static double PosFloatingPL(int bar, int pos)
        {
            return session[bar].Position[pos].FloatingPL;
        }

        /// <summary>
        /// The position Profit Loss in currency
        /// </summary>
        public static double PosMoneyProfitLoss(int bar, int pos)
        {
            return session[bar].Position[pos].MoneyProfitLoss;
        }

        /// <summary>
        /// The position Floating Profit Loss in currency
        /// </summary>
        public static double PosMoneyFloatingPL(int bar, int pos)
        {
            return session[bar].Position[pos].MoneyFloatingPL;
        }

        /// <summary>
        /// The position balance in currency
        /// </summary>
        public static double PosMoneyBalance(int bar, int pos)
        {
            return session[bar].Position[pos].MoneyBalance;
        }

        /// <summary>
        /// The position equity in currency
        /// </summary>
        public static double PosMoneyEquity(int bar, int pos)
        {
            return session[bar].Position[pos].MoneyEquity;
        }

        /// <summary>
        /// The position's corrected price
        /// </summary>
        public static double PosPrice(int bar, int pos)
        {
            return session[bar].Position[pos].PosPrice;
        }

        /// <summary>
        /// The position's Transaction
        /// </summary>
        public static Transaction PosTransaction(int bar, int pos)
        {
            return session[bar].Position[pos].Transaction;
        }

        /// <summary>
        /// The position's Icon
        /// </summary>
        public static Image PosIcon(int bar, int pos)
        {
            return session[bar].Position[pos].PositionIcon;
        }

        /// <summary>
        /// Returns the position's Profit Loss in pips.
        /// </summary>
        public static int ProfitLoss(int bar)
        {
            return (int)Math.Round(session[bar].Summary.ProfitLoss);
        }

        /// <summary>
        /// Returns the Floating Profit Loss at the end of the bar in pips
        /// </summary>
        public static int FloatingPL(int bar)
        {
            return (int)Math.Round(session[bar].Summary.FloatingPL);
        }

        /// <summary>
        /// Returns the account balance at the end of the bar in pips
        /// </summary>
        public static int Balance(int bar)
        {
            return (int)Math.Round(session[bar].Summary.Balance);
        }

        /// <summary>
        /// Returns the equity at the end of the bar in pips
        /// </summary>
        public static int Equity(int bar)
        {
            return (int)Math.Round(session[bar].Summary.Equity);
        }

        /// <summary>
        /// Returns the charged spread.
        /// </summary>
        public static double ChargedSpread(int bar)
        {
            return session[bar].Summary.Spread;
        }

        /// <summary>
        /// Returns the charged rollover.
        /// </summary>
        public static double ChargedRollOver(int bar)
        {
            return session[bar].Summary.Rollover;
        }

        /// <summary>
        /// Returns the bar end Profit Loss in currency.
        /// </summary>
        public static double MoneyProfitLoss(int bar)
        {
            return session[bar].Summary.MoneyProfitLoss;
        }

        /// <summary>
        /// Returns the bar end Floating Profit Loss in currency
        /// </summary>
        public static double MoneyFloatingPL(int bar)
        {
            return session[bar].Summary.MoneyFloatingPL;
        }

        /// <summary>
        /// Returns the account balance in currency
        /// </summary>
        public static double MoneyBalance(int bar)
        {
            return session[bar].Summary.MoneyBalance;
        }

        /// <summary>
        /// Returns the current bill in currency.
        /// </summary>
        public static double MoneyEquity(int bar)
        {
            return session[bar].Summary.MoneyEquity;
        }

        /// <summary>
        /// Returns the charged spread in currency.
        /// </summary>
        public static double MoneyChargedSpread(int bar)
        {
            return session[bar].Summary.MoneySpread;
        }

        /// <summary>
        /// Returns the charged rollover in currency.
        /// </summary>
        public static double MoneyChargedRollOver(int bar)
        {
            return session[bar].Summary.MoneyRollover;
        }

        /// <summary>
        /// Returns the backtest safety evaluation
        /// </summary>
        public static string BackTestEval(int bar)
        {
            string eval = session[bar].BacktestEval.ToString();

            if (bar < Data.FirstBar || session[bar].BacktestEval == BacktestEval.None)
                eval = "";

            return eval;
        }

        /// <summary>
        /// Returns the position with the required number
        /// </summary>
        public static Position PosFromNumb(int posNumber)
        {
            if (posNumber < 0) posNumber = 0;
            return session[posCoord[posNumber].Bar].Position[posCoord[posNumber].Pos];
        }

        /// <summary>
        /// Gets the total number of the orders
        /// </summary>
        public static int OrdersTotal { get { return totalOrders; } }

        /// <summary>
        /// Returns the number of orders for the designated bar
        /// </summary>
        public static int Orders(int bar)
        {
            return session[bar].Orders;
        }

        /// <summary>
        /// Returns the Order Number
        /// </summary>
        public static int OrdNumb(int bar, int ord)
        {
            return session[bar].Order[ord].OrdNumb;
        }

        /// <summary>
        /// Returns the order with the corresponding number
        /// </summary>
        public static Order OrdFromNumb(int ordNumber)
        {
            if (ordNumber < 0) ordNumber = 0;
            return session[ordCoord[ordNumber].Bar].Order[ordCoord[ordNumber].Ord];
        }

        /// <summary>
        ///  Way point
        /// </summary>
        public static Way_Point WayPoint(int bar, int wayPointNumber)
        {
            return session[bar].WayPoint[wayPointNumber];
        }

        /// <summary>
        ///  Bar's way points count.
        /// </summary>
        public static int WayPoints(int bar)
        {
            return session[bar].WayPoints;
        }
    }
}
