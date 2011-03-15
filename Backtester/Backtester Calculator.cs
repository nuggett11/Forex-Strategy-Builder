// Backtester - Calculator
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Backtester
    /// </summary>
    public partial class Backtester : Data
    {
        // Private fields
        static int totalOrders;
        static int totalPositions;
        static Session[] session;
        static OrderCoordinates[] ordCoord;
        static PositionCoordinates[] posCoord;
        static StrategyPriceType openStrPriceType;
        static StrategyPriceType closeStrPriceType;

        // Environment
        static InterpolationMethod interpolationMethod = InterpolationMethod.Pessimistic;
        static ExecutionTime openTimeExec;
        static ExecutionTime closeTimeExec;
        static bool   isScanning  = false;
        static double maximumLots = 100;

        // Additional
        static double micron = InstrProperties.Point / 2;
        static Random random = new Random();
        static DateTime lastEntryTime;
        static int calculations;

        // Logical Groups
        static Dictionary<string, bool> groupsAllowLong;
        static Dictionary<string, bool> groupsAllowShort;
        static List<string> openingLogicGroups;
        static List<string> closingLogicGroups;

        /// <summary>
        /// Gets the maximum number of orders.
        /// Entry - 2, Exit - 3, Exit Perm. S/L - 3, Exit Perm. T/P - 3, Exit Margin Call - 1
        /// </summary>
        static int MaxOrders
        {
            get
            {
                int maxOrders = 6;
                if (Strategy.UsePermanentSL)
                    maxOrders += 3;
                if (Strategy.UsePermanentTP)
                    maxOrders += 3;
                return maxOrders;
            }
        }

        /// <summary>
        /// Gets the maximum number of positions.
        /// Transferred - 1, Transferred closing - 1, Opening - 2, Closing - 2
        /// </summary>
        static int MaxPositions
        {
            get
            {
                int maxPositions = 6;
                return maxPositions;
            }
        }

        /// <summary>
        /// Resets the variables and prepares the arrays
        /// </summary>
        static void ResetStart()
        {
            marginCallBar  = 0;
            totalOrders    = 0;
            totalPositions = 0;
            isScanned      = false;
            micron         = InstrProperties.Point / 2d;
            lastEntryTime  = new DateTime();
            calculations++;

            // Sets the maximum lots
            maximumLots = 100;
            foreach (IndicatorSlot slot in Strategy.Slot)
                if (slot.IndicatorName == "Lot Limiter")
                    maximumLots = (int)slot.IndParam.NumParam[0].Value;

            maximumLots = Math.Min(maximumLots, Strategy.MaxOpenLots);

            session = new Session[Bars];
            for (int bar = 0; bar < Bars; bar++)
                session[bar] = new Session(bar, MaxPositions, MaxOrders);

            for (int bar = 0; bar < FirstBar; bar++)
            {
                session[bar].Summary.MoneyBalance = Configs.InitialAccount;
                session[bar].Summary.MoneyEquity  = Configs.InitialAccount;
            }

            ordCoord = new OrderCoordinates[Bars * MaxOrders];
            posCoord = new PositionCoordinates[Bars * MaxPositions];

            openTimeExec  = Strategy.Slot[Strategy.OpenSlot ].IndParam.ExecutionTime;
            closeTimeExec = Strategy.Slot[Strategy.CloseSlot].IndParam.ExecutionTime;

            openStrPriceType  = StrategyPriceType.Unknown;
            if (openTimeExec == ExecutionTime.AtBarOpening)
                openStrPriceType = StrategyPriceType.Open;
            else if (openTimeExec == ExecutionTime.AtBarClosing)
                openStrPriceType = StrategyPriceType.Close;
            else
                openStrPriceType = StrategyPriceType.Indicator;

            closeStrPriceType = StrategyPriceType.Unknown;
            if (closeTimeExec == ExecutionTime.AtBarOpening)
                closeStrPriceType = StrategyPriceType.Open;
            else if (closeTimeExec == ExecutionTime.AtBarClosing)
                closeStrPriceType = StrategyPriceType.Close;
            else if (closeTimeExec == ExecutionTime.CloseAndReverse)
                closeStrPriceType = StrategyPriceType.CloseAndReverce;
            else
                closeStrPriceType = StrategyPriceType.Indicator;

            if (Configs.UseLogicalGroups)
            {
                Strategy.Slot[Strategy.OpenSlot].LogicalGroup  = "All"; // Allows calculation of open slot for each group.
                Strategy.Slot[Strategy.CloseSlot].LogicalGroup = "All"; // Allows calculation of close slot for each group.

                groupsAllowLong = new Dictionary<string, bool>();
                groupsAllowShort = new Dictionary<string, bool>();
                for (int slot = Strategy.OpenSlot; slot < Strategy.CloseSlot; slot++)
                {
                    if (!groupsAllowLong.ContainsKey(Strategy.Slot[slot].LogicalGroup))
                        groupsAllowLong.Add(Strategy.Slot[slot].LogicalGroup, false);
                    if (!groupsAllowShort.ContainsKey(Strategy.Slot[slot].LogicalGroup))
                        groupsAllowShort.Add(Strategy.Slot[slot].LogicalGroup, false);
                }

                // List of logical groups
                openingLogicGroups = new List<string>();
                foreach (System.Collections.Generic.KeyValuePair<string, bool> kvp in groupsAllowLong)
                    openingLogicGroups.Add(kvp.Key);

                // Logical groups of the closing conditions.
                closingLogicGroups = new List<string>();
                for (int slot = Strategy.CloseSlot + 1; slot < Strategy.Slots; slot++)
                {
                    string group = Strategy.Slot[slot].LogicalGroup;
                    if (!closingLogicGroups.Contains(group) && group != "all")
                        closingLogicGroups.Add(group); // Adds all groups except "all"
                }

                if (closingLogicGroups.Count == 0)
                    closingLogicGroups.Add("all"); // If all the slots are in "all" group, adds "all" to the list.
            }

            return;
        }

        /// <summary>
        /// Resets the variables at the end of the test.
        /// </summary>
        static void ResetStop()
        {
            if (Configs.UseLogicalGroups)
            {
                Strategy.Slot[Strategy.OpenSlot].LogicalGroup  = ""; // Delete the group of open slot.
                Strategy.Slot[Strategy.CloseSlot].LogicalGroup = ""; // Delete the group of close slot.
            }
        }

        /// <summary>
        /// Sets the position.
        /// </summary>
        static void SetPosition(int bar, OrderDirection ordDir, double lots, double price, int ordNumb)
        {
            int sessionPosition;
            Position position;
            double pipsToMoneyRate = InstrProperties.Point * InstrProperties.LotSize / AccountExchangeRate(price);
            bool isAbsoluteSL = Strategy.UsePermanentSL && Strategy.PermanentSLType == PermanentProtectionType.Absolute;
            bool isAbsoluteTP = Strategy.UsePermanentTP && Strategy.PermanentTPType == PermanentProtectionType.Absolute;

            if (session[bar].Positions == 0 || session[bar].Summary.PosLots == 0)
            {   // Open new position when either we have not opened one or it has been closed
                if (ordDir == OrderDirection.Buy)
                {   // Opens a long position
                    sessionPosition = session[bar].Positions;
                    position = session[bar].Position[sessionPosition] = new Position();

                    position.Transaction     = Transaction.Open;
                    position.PosDir          = PosDirection.Long;
                    position.OpeningBar      = bar;
                    position.FormOrdNumb     = ordNumb;
                    position.FormOrdPrice    = price;
                    position.PosNumb         = totalPositions;
                    position.PosLots         = lots;
                    position.AbsoluteSL      = isAbsoluteSL ? price - Strategy.PermanentSL * InstrProperties.Point : 0;
                    position.AbsoluteTP      = isAbsoluteTP ? price + Strategy.PermanentTP * InstrProperties.Point : 0;
                    position.RequiredMargin  = RequiredMargin(position.PosLots, bar);

                    position.Spread          = lots * InstrProperties.Spread;
                    position.Commission      = Commission(lots, price, false);
                    position.Slippage        = lots * InstrProperties.Slippage;
                    position.PosPrice        = price + (InstrProperties.Spread + InstrProperties.Slippage) * InstrProperties.Point;
                    position.FloatingPL      = lots * (Close[bar] - position.PosPrice) / InstrProperties.Point;
                    position.ProfitLoss      = 0;
                    position.Balance         = PosFromNumb(totalPositions - 1).Balance - position.Commission;
                    position.Equity          = position.Balance + position.FloatingPL;

                    position.MoneySpread     = lots * InstrProperties.Spread * pipsToMoneyRate;
                    position.MoneyCommission = CommissionInMoney(lots, price, false);
                    position.MoneySlippage   = lots * InstrProperties.Slippage * pipsToMoneyRate;
                    position.MoneyFloatingPL = lots * (Close[bar] - position.PosPrice) * InstrProperties.LotSize / AccountExchangeRate(price);
                    position.MoneyProfitLoss = 0;
                    position.MoneyBalance    = PosFromNumb(totalPositions - 1).MoneyBalance - position.MoneyCommission;
                    position.MoneyEquity     = position.MoneyBalance + position.MoneyFloatingPL;

                    posCoord[totalPositions].Bar = bar;
                    posCoord[totalPositions].Pos = sessionPosition;
                    session[bar].Positions++;
                    totalPositions++;

                    return;
                }
                else
                {   // Opens a short position
                    sessionPosition = session[bar].Positions;
                    position = session[bar].Position[sessionPosition] = new Position();

                    position.Transaction     = Transaction.Open;
                    position.PosDir          = PosDirection.Short;
                    position.OpeningBar      = bar;
                    position.FormOrdNumb     = ordNumb;
                    position.FormOrdPrice    = price;
                    position.PosNumb         = totalPositions;
                    position.PosLots         = lots;
                    position.AbsoluteSL      = isAbsoluteSL ? price + Strategy.PermanentSL * InstrProperties.Point : 0;
                    position.AbsoluteTP      = isAbsoluteTP ? price - Strategy.PermanentTP * InstrProperties.Point : 0;
                    position.RequiredMargin  = RequiredMargin(position.PosLots, bar);

                    position.Spread          = lots * InstrProperties.Spread;
                    position.Commission      = Commission(lots, price, false);
                    position.Slippage        = lots * InstrProperties.Slippage;
                    position.PosPrice        = price - (InstrProperties.Spread + InstrProperties.Slippage) * InstrProperties.Point;
                    position.FloatingPL      = lots * (position.PosPrice - Close[bar]) / InstrProperties.Point;
                    position.ProfitLoss      = 0;
                    position.Balance         = PosFromNumb(totalPositions - 1).Balance - position.Commission;
                    position.Equity          = position.Balance + position.FloatingPL;

                    position.MoneySpread     = lots * InstrProperties.Spread * pipsToMoneyRate;
                    position.MoneyCommission = CommissionInMoney(lots, price, false);
                    position.MoneySlippage   = lots * InstrProperties.Slippage * pipsToMoneyRate;
                    position.MoneyFloatingPL = lots * (position.PosPrice - Close[bar]) * InstrProperties.LotSize / AccountExchangeRate(price);
                    position.MoneyProfitLoss = 0;
                    position.MoneyBalance    = PosFromNumb(totalPositions - 1).MoneyBalance - position.MoneyCommission;
                    position.MoneyEquity     = position.MoneyBalance + position.MoneyFloatingPL;

                    posCoord[totalPositions].Bar = bar;
                    posCoord[totalPositions].Pos = sessionPosition;
                    session[bar].Positions++;
                    totalPositions++;

                    return;
                }
            }

            int sessionPosOld = session[bar].Positions - 1;
            Position positionOld = session[bar].Position[sessionPosOld];
            PosDirection posDirOld = positionOld.PosDir;
            double lotsOld  = positionOld.PosLots;
            double priceOld = positionOld.PosPrice;
            double absoluteSL = positionOld.AbsoluteSL;
            double absoluteTP = positionOld.AbsoluteTP;
            double posBalanceOld = positionOld.Balance;
            double posEquityOld  = positionOld.Equity;

            sessionPosition = sessionPosOld + 1;
            position = session[bar].Position[sessionPosition] = new Position();

            position.PosNumb      = totalPositions;
            position.FormOrdPrice = price;
            position.FormOrdNumb  = ordNumb;
            position.Balance      = posBalanceOld;
            position.Equity       = posEquityOld;
            position.MoneyBalance = positionOld.MoneyBalance;
            position.MoneyEquity  = positionOld.MoneyEquity;

            posCoord[totalPositions].Bar = bar;
            posCoord[totalPositions].Pos = sessionPosition;
            session[bar].Positions++;
            totalPositions++;

            // Closing of a long position
            if (posDirOld == PosDirection.Long && ordDir == OrderDirection.Sell && lotsOld == lots)
            {
                position.Transaction     = Transaction.Close;
                position.PosDir          = PosDirection.Closed;
                position.PosLots         = 0;
                position.AbsoluteSL      = 0;
                position.AbsoluteTP      = 0;
                position.RequiredMargin  = 0;

                position.Commission      = Commission(lots, price, true);
                position.Slippage        = lots * InstrProperties.Slippage;
                position.PosPrice        = priceOld;
                position.FloatingPL      = 0;
                position.ProfitLoss      = lots * (price - priceOld) / InstrProperties.Point - position.Slippage;
                position.Balance        += position.ProfitLoss - position.Commission;
                position.Equity          = position.Balance;

                position.MoneyCommission = CommissionInMoney(lots, price, true);
                position.MoneySlippage   = lots * InstrProperties.Slippage * pipsToMoneyRate;
                position.MoneyFloatingPL = 0;
                position.MoneyProfitLoss = lots * (price - priceOld) * InstrProperties.LotSize / AccountExchangeRate(price) - position.MoneySlippage;
                position.MoneyBalance   += position.MoneyProfitLoss - position.MoneyCommission;
                position.MoneyEquity     = position.MoneyBalance;
                return;
            }

            // Closing of a short position
            if (posDirOld == PosDirection.Short && ordDir == OrderDirection.Buy && lotsOld == lots)
            {
                position.Transaction     = Transaction.Close;
                position.PosDir          = PosDirection.Closed;
                position.PosLots         = 0;
                position.AbsoluteSL      = 0;
                position.AbsoluteTP      = 0;
                position.RequiredMargin  = 0;

                position.Commission      = Commission(lots, price, true);
                position.Slippage        = lots * InstrProperties.Slippage;
                position.PosPrice        = priceOld;
                position.FloatingPL      = 0;
                position.ProfitLoss      = lots * (priceOld - price) / InstrProperties.Point - position.Slippage;
                position.Balance        += position.ProfitLoss - position.Commission;
                position.Equity          = position.Balance;

                position.MoneyCommission = CommissionInMoney(lots, price, true);
                position.MoneySlippage   = lots * InstrProperties.Slippage * pipsToMoneyRate;
                position.MoneyFloatingPL = 0;
                position.MoneyProfitLoss = lots * (priceOld - price) * InstrProperties.LotSize / AccountExchangeRate(price) - position.MoneySlippage;
                position.MoneyBalance   += position.MoneyProfitLoss - position.MoneyCommission;
                position.MoneyEquity     = position.MoneyBalance;
                return;
            }

            // Adding to a long position
            if (posDirOld == PosDirection.Long && ordDir == OrderDirection.Buy)
            {
                position.Transaction     = Transaction.Add;
                position.PosDir          = PosDirection.Long;
                position.PosLots         = lotsOld + lots;
                position.AbsoluteSL      = absoluteSL;
                position.AbsoluteTP      = absoluteTP;
                position.RequiredMargin  = RequiredMargin(position.PosLots, bar);

                position.Spread          = lots * InstrProperties.Spread;
                position.Commission      = Commission(lots, price, false);
                position.Slippage        = lots * InstrProperties.Slippage;
                position.PosPrice        = (lotsOld * priceOld + lots * (price + (InstrProperties.Spread + InstrProperties.Slippage) * InstrProperties.Point)) / (lotsOld + lots);
                position.FloatingPL      = (lotsOld + lots) * (Close[bar] - position.PosPrice) / InstrProperties.Point;
                position.ProfitLoss      = 0;
                position.Balance        -= position.Commission;
                position.Equity          = position.Balance + position.FloatingPL;

                position.MoneySpread     = lots * InstrProperties.Spread * pipsToMoneyRate;
                position.MoneyCommission = CommissionInMoney(lots, price, false);
                position.MoneySlippage   = lots * InstrProperties.Slippage * pipsToMoneyRate;
                position.MoneyFloatingPL = (lotsOld + lots) * (Close[bar] - position.PosPrice) * InstrProperties.LotSize / AccountExchangeRate(price);
                position.MoneyProfitLoss = 0;
                position.MoneyBalance   -= position.MoneyCommission;
                position.MoneyEquity     = position.MoneyBalance + position.MoneyFloatingPL;
                return;
            }

            // Adding to a short position
            if (posDirOld == PosDirection.Short && ordDir == OrderDirection.Sell)
            {
                position.Transaction     = Transaction.Add;
                position.PosDir          = PosDirection.Short;
                position.PosLots         = lotsOld + lots;
                position.AbsoluteSL      = absoluteSL;
                position.AbsoluteTP      = absoluteTP;
                position.RequiredMargin  = RequiredMargin(position.PosLots, bar);

                position.Spread          = lots * InstrProperties.Spread;
                position.Commission      = Commission(lots, price, false);
                position.Slippage        = lots * InstrProperties.Slippage;
                position.PosPrice        = (lotsOld * priceOld + lots * (price - (InstrProperties.Spread + InstrProperties.Slippage) * InstrProperties.Point)) / (lotsOld + lots);
                position.FloatingPL      = (lotsOld + lots) * (position.PosPrice - Close[bar]) / InstrProperties.Point;
                position.ProfitLoss      = 0;
                position.Balance        -= position.Commission;
                position.Equity          = position.Balance + position.FloatingPL;

                position.MoneySpread     = lots * InstrProperties.Spread * pipsToMoneyRate;
                position.MoneyCommission = CommissionInMoney(lots, price, false);
                position.MoneySlippage   = lots * InstrProperties.Slippage * pipsToMoneyRate;
                position.MoneyFloatingPL = (lotsOld + lots) * (position.PosPrice - Close[bar]) * InstrProperties.LotSize / AccountExchangeRate(price);
                position.MoneyProfitLoss = 0;
                position.MoneyBalance   -= position.MoneyCommission;
                position.MoneyEquity     = position.MoneyBalance + position.MoneyFloatingPL;
                return;
            }

            // Reducing of a long position
            if (posDirOld == PosDirection.Long && ordDir == OrderDirection.Sell && lotsOld > lots)
            {
                position.Transaction     = Transaction.Reduce;
                position.PosDir          = PosDirection.Long;
                position.PosLots         = lotsOld - lots;
                position.AbsoluteSL      = absoluteSL;
                position.AbsoluteTP      = absoluteTP;
                position.RequiredMargin  = RequiredMargin(position.PosLots, bar);

                position.Commission      = Commission(lots, price, true);
                position.Slippage        = lots * InstrProperties.Slippage;
                position.PosPrice        = priceOld;
                position.FloatingPL      = (lotsOld - lots) * (Close[bar] - priceOld) / InstrProperties.Point;
                position.ProfitLoss      = lots * ((price - priceOld) / InstrProperties.Point - InstrProperties.Slippage);
                position.Balance        += position.ProfitLoss - position.Commission;
                position.Equity          = position.Balance + position.FloatingPL;

                position.MoneyCommission = CommissionInMoney(lots, price, true);
                position.MoneySlippage   = lots * InstrProperties.Slippage * pipsToMoneyRate;
                position.MoneyFloatingPL = (lotsOld - lots) * (Close[bar] - priceOld) * InstrProperties.LotSize / AccountExchangeRate(price);
                position.MoneyProfitLoss = lots * (price - priceOld) * InstrProperties.LotSize / AccountExchangeRate(price) - position.MoneySlippage;
                position.MoneyBalance   += position.MoneyProfitLoss - position.MoneyCommission;
                position.MoneyEquity     = position.MoneyBalance + position.MoneyFloatingPL;
                return;
            }

            // Reducing of a short position
            if (posDirOld == PosDirection.Short && ordDir == OrderDirection.Buy && lotsOld > lots)
            {
                position.Transaction     = Transaction.Reduce;
                position.PosDir          = PosDirection.Short;
                position.PosLots         = lotsOld - lots;
                position.AbsoluteSL      = absoluteSL;
                position.AbsoluteTP      = absoluteTP;
                position.RequiredMargin  = RequiredMargin(position.PosLots, bar);

                position.Commission      = Commission(lots, price, true);
                position.Slippage        = lots * InstrProperties.Slippage;
                position.PosPrice        = priceOld;
                position.FloatingPL      = (lotsOld - lots) * (priceOld - Close[bar]) / InstrProperties.Point;
                position.ProfitLoss      = lots * ((priceOld - price) / InstrProperties.Point - InstrProperties.Slippage);
                position.Balance        += position.ProfitLoss - position.Commission;
                position.Equity          = position.Balance + position.FloatingPL;

                position.MoneyCommission = CommissionInMoney(lots, price, true);
                position.MoneySlippage   = lots * InstrProperties.Slippage * pipsToMoneyRate;
                position.MoneyFloatingPL = (lotsOld - lots) * (priceOld - Close[bar]) * InstrProperties.LotSize / AccountExchangeRate(price);
                position.MoneyProfitLoss = lots * (priceOld - price) * InstrProperties.LotSize / AccountExchangeRate(price) - position.MoneySlippage;
                position.MoneyBalance   += position.MoneyProfitLoss - position.MoneyCommission;
                position.MoneyEquity     = position.MoneyBalance + position.MoneyFloatingPL;
                return;
            }

            // Reversing of a long position
            if (posDirOld == PosDirection.Long && ordDir == OrderDirection.Sell && lotsOld < lots)
            {
                position.Transaction     = Transaction.Reverse;
                position.PosDir          = PosDirection.Short;
                position.PosLots         = lots - lotsOld;
                position.OpeningBar      = bar;
                position.AbsoluteSL      = isAbsoluteSL ? price + Strategy.PermanentSL * InstrProperties.Point : 0;
                position.AbsoluteTP      = isAbsoluteTP ? price - Strategy.PermanentTP * InstrProperties.Point : 0;
                position.RequiredMargin  = RequiredMargin(position.PosLots, bar);

                position.Spread          = (lots - lotsOld) * InstrProperties.Spread;
                position.Commission      = Commission(lotsOld, price, true) + Commission(lots - lotsOld, price, false);
                position.Slippage        = lots * InstrProperties.Slippage;
                position.PosPrice        = price - (InstrProperties.Spread + InstrProperties.Slippage) * InstrProperties.Point;
                position.FloatingPL      = (lots - lotsOld) * (position.PosPrice - Close[bar]) / InstrProperties.Point;
                position.ProfitLoss      = lotsOld * ((price - priceOld) / InstrProperties.Point - InstrProperties.Slippage);
                position.Balance        += position.ProfitLoss - position.Commission;
                position.Equity          = position.Balance + position.FloatingPL;

                position.MoneySpread     = (lots - lotsOld) * InstrProperties.Spread * pipsToMoneyRate;
                position.MoneyCommission = CommissionInMoney(lotsOld, price, true) + CommissionInMoney(lots - lotsOld, price, false);
                position.MoneySlippage   = lots * InstrProperties.Slippage * pipsToMoneyRate;
                position.MoneyFloatingPL = (lots - lotsOld) * (position.PosPrice - Close[bar]) * InstrProperties.LotSize / AccountExchangeRate(price);
                position.MoneyProfitLoss = lotsOld * (price - priceOld) * InstrProperties.LotSize / AccountExchangeRate(price) - lotsOld * InstrProperties.Slippage * pipsToMoneyRate;
                position.MoneyBalance   += position.MoneyProfitLoss - position.MoneyCommission;
                position.MoneyEquity     = position.MoneyBalance + position.MoneyFloatingPL;
                return;
            }

            // Reversing of a short position
            if (posDirOld == PosDirection.Short && ordDir == OrderDirection.Buy && lotsOld < lots)
            {
                position.Transaction     = Transaction.Reverse;
                position.PosDir          = PosDirection.Long;
                position.PosLots         = lots - lotsOld;
                position.OpeningBar      = bar;
                position.AbsoluteSL      = Strategy.UsePermanentSL ? price - Strategy.PermanentSL * InstrProperties.Point : 0;
                position.AbsoluteTP      = Strategy.UsePermanentTP ? price + Strategy.PermanentTP * InstrProperties.Point : 0;
                position.RequiredMargin  = RequiredMargin(position.PosLots, bar);

                position.Spread          = (lots - lotsOld) * InstrProperties.Spread;
                position.Commission      = Commission(lotsOld, price, true) + Commission(lots - lotsOld, price, false);
                position.Slippage        = lots * InstrProperties.Slippage;
                position.PosPrice        = price + (InstrProperties.Spread + InstrProperties.Slippage) * InstrProperties.Point;
                position.FloatingPL      = (lots - lotsOld) * (Close[bar] - position.PosPrice) / InstrProperties.Point;
                position.ProfitLoss      = lotsOld * ((priceOld - price) / InstrProperties.Point - InstrProperties.Slippage);
                position.Balance        += position.ProfitLoss - position.Commission;
                position.Equity          = position.Balance + position.FloatingPL;

                position.MoneySpread     = (lots - lotsOld) * InstrProperties.Spread * pipsToMoneyRate;
                position.MoneyCommission = CommissionInMoney(lotsOld, price, true) + CommissionInMoney(lots - lotsOld, price, false);
                position.MoneySlippage   = lots * InstrProperties.Slippage * pipsToMoneyRate;
                position.MoneyFloatingPL = (lots - lotsOld) * (Close[bar] - position.PosPrice) * InstrProperties.LotSize / AccountExchangeRate(price);
                position.MoneyProfitLoss = lotsOld * (priceOld - price) * InstrProperties.LotSize / AccountExchangeRate(price) - lotsOld * InstrProperties.Slippage * pipsToMoneyRate;
                position.MoneyBalance   += position.MoneyProfitLoss - position.MoneyCommission;
                position.MoneyEquity     = position.MoneyBalance + position.MoneyFloatingPL;
                return;
            }

            // We should never be here !!
            return;
        }

        /// <summary>
        /// Checks all orders in the current bar and cancels the invalid ones.
        /// </summary>
        static void CancelInvalidOrders(int bar)
        {
            // Cancelling the EOP orders
            for(int ord = 0; ord < session[bar].Orders; ord++)
                if (session[bar].Order[ord].OrdStatus == OrderStatus.Confirmed)
                    session[bar].Order[ord].OrdStatus = OrderStatus.Cancelled;

            // Cancelling the invalid IF orders
            for (int ord = 0; ord < session[bar].Orders; ord++)
                if (session[bar].Order[ord].OrdStatus == OrderStatus.Confirmed &&
                    session[bar].Order[ord].OrdCond   == OrderCondition.If     &&
                    OrdFromNumb(session[bar].Order[ord].OrdIF).OrdStatus != OrderStatus.Executed)
                        session[bar].Order[ord].OrdStatus = OrderStatus.Cancelled;

            return;
        }

        /// <summary>
        /// Cancel all no executed entry orders
        /// </summary>
        static void CancelNoexecutedEntryOrders(int bar)
        {
            for (int ord = 0; ord < session[bar].Orders; ord++)
            {
                if (session[bar].Order[ord].OrdSender == OrderSender.Open &&
                    session[bar].Order[ord].OrdStatus != OrderStatus.Executed)
                {
                    session[bar].Order[ord].OrdStatus = OrderStatus.Cancelled;
                }
            }

            return;
        }

        /// <summary>
        /// Cancel all no executed exit orders
        /// </summary>
        static void CancelNoexecutedExitOrders(int bar)
        {
            for (int ord = 0; ord < session[bar].Orders; ord++)
            {
                if (session[bar].Order[ord].OrdSender == OrderSender.Close &&
                    session[bar].Order[ord].OrdStatus != OrderStatus.Executed)
                {
                    session[bar].Order[ord].OrdStatus = OrderStatus.Cancelled;
                }
            }

            return;
        }

        /// <summary>
        /// Executes an entry at the beginning of the bar
        /// </summary>
        static void ExecuteEntryAtOpeningPrice(int bar)
        {
            for (int ord = 0; ord < session[bar].Orders; ord++)
            {
                if (session[bar].Order[ord].OrdSender == OrderSender.Open)
                {
                    ExecOrd(bar, session[bar].Order[ord], Open[bar], BacktestEval.Correct);
                }
            }

            return;
        }

        /// <summary>
        /// Executes an entry at the closing of the bar
        /// </summary>
        static void ExecuteEntryAtClosingPrice(int bar)
        {
            for (int ord = 0; ord < session[bar].Orders; ord++)
            {
                if (session[bar].Order[ord].OrdSender == OrderSender.Open)
                {
                    ExecOrd(bar, session[bar].Order[ord], Close[bar], BacktestEval.Correct);
                }
            }

            return;
        }

        /// <summary>
        /// Executes an exit at the closing of the bar
        /// </summary>
        static void ExecuteExitAtClosingPrice(int bar)
        {
            for (int ord = 0; ord < session[bar].Orders; ord++)
            {
                if (session[bar].Order[ord].OrdSender == OrderSender.Close && CheckOrd(bar, ord))
                {
                    ExecOrd(bar, session[bar].Order[ord], Close[bar], BacktestEval.Correct);
                }
            }

            return;
        }

        /// <summary>
        /// Checks and perform actions in case of a Margin Call
        /// </summary>
        static void MarginCallCheckAtBarClosing(int bar)
        {
            if (!Configs.TradeUntilMarginCall ||
                session[bar].Summary.FreeMargin >= 0)
                return;

            if (session[bar].Summary.PosDir == PosDirection.None ||
                session[bar].Summary.PosDir == PosDirection.Closed)
                return;

            CancelNoexecutedExitOrders(bar);

            int    ifOrd = 0;
            int    toPos = session[bar].Summary.PosNumb;
            double lots  = session[bar].Summary.PosLots;
            string note  = Language.T("Close due to a Margin Call");

            if (session[bar].Summary.PosDir == PosDirection.Long)
            {
                OrdSellMarket(bar, ifOrd, toPos, lots, Close[bar], OrderSender.Close, OrderOrigin.MarginCall, note);
            }
            else if (session[bar].Summary.PosDir == PosDirection.Short)
            {
                OrdBuyMarket(bar, ifOrd, toPos, lots, Close[bar], OrderSender.Close, OrderOrigin.MarginCall, note);
            }

            ExecuteExitAtClosingPrice(bar);

            // Margin Call bar
            if (marginCallBar == 0)
                marginCallBar = bar;

            return;
        }

        /// <summary>
        /// Checks the order
        /// </summary>
        /// <returns>True if the order is valid.</returns>
        static bool CheckOrd(int bar, int iOrd)
        {
            if (session[bar].Order[iOrd].OrdStatus != OrderStatus.Confirmed)
                return false;
            else if (session[bar].Order[iOrd].OrdCond != OrderCondition.If)
                return true;
            else if (OrdFromNumb(session[bar].Order[iOrd].OrdIF).OrdStatus == OrderStatus.Executed)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Tunes and Executes an order
        /// </summary>
        static OrderStatus ExecOrd(int bar, Order order, double price, BacktestEval testEval)
        {
            Position       position     = session[bar].Summary;
            PosDirection   posDir       = position.PosDir;
            OrderDirection ordDir       = order.OrdDir;
            WayPointType   wayPointType = WayPointType.None;

            // Orders modification on a fly
            // Checks whether we are on the market
            if (posDir == PosDirection.Long || posDir == PosDirection.Short)
            {   // We are on the market
                if (order.OrdSender == OrderSender.Open)
                {   // Entry orders
                    if (ordDir == OrderDirection.Buy  && posDir == PosDirection.Long ||
                        ordDir == OrderDirection.Sell && posDir == PosDirection.Short)
                    {   // In case of a Same Dir Signal
                        switch (Strategy.SameSignalAction)
                        {
                            case SameDirSignalAction.Add:
                                order.OrdLots = TradingSize(Strategy.AddingLots, bar);
                                if (position.PosLots + TradingSize(Strategy.AddingLots, bar) <= maximumLots)
                                {   // Adding
                                    wayPointType = WayPointType.Add;
                                }
                                else
                                {   // Cancel the Adding
                                    order.OrdStatus = OrderStatus.Cancelled;
                                    wayPointType = WayPointType.Cancel;
                                    FindCancelExitOrder(bar, order); // Canceling of its exit order
                                }
                                break;
                            case SameDirSignalAction.Winner:
                                order.OrdLots = TradingSize(Strategy.AddingLots, bar);
                                if (position.PosLots + TradingSize(Strategy.AddingLots, bar) <= maximumLots &&
                                    (position.PosDir == PosDirection.Long  && position.PosPrice < order.OrdPrice ||
                                     position.PosDir == PosDirection.Short && position.PosPrice > order.OrdPrice))
                                {   // Adding
                                    wayPointType = WayPointType.Add;
                                }
                                else
                                {   // Cancel the Adding
                                    order.OrdStatus = OrderStatus.Cancelled;
                                    wayPointType    = WayPointType.Cancel;
                                    FindCancelExitOrder(bar, order);  // Canceling of its exit order
                                }
                                break;
                            case SameDirSignalAction.Nothing:
                                order.OrdLots   = TradingSize(Strategy.AddingLots, bar);
                                order.OrdStatus = OrderStatus.Cancelled;
                                wayPointType    = WayPointType.Cancel;
                                FindCancelExitOrder(bar, order);  // Canceling of its exit order
                                break;
                            default:
                                break;
                        }
                    }
                    else if (ordDir == OrderDirection.Buy  && posDir == PosDirection.Short ||
                             ordDir == OrderDirection.Sell && posDir == PosDirection.Long)
                    {   // In case of an Opposite Dir Signal
                        switch (Strategy.OppSignalAction)
                        {
                            case OppositeDirSignalAction.Reduce:
                                if (position.PosLots > TradingSize(Strategy.ReducingLots, bar))
                                {   // Reducing
                                    order.OrdLots = TradingSize(Strategy.ReducingLots, bar);
                                    wayPointType  = WayPointType.Reduce;
                                }
                                else
                                {   // Closing
                                    order.OrdLots = position.PosLots;
                                    wayPointType  = WayPointType.Exit;
                                }
                                break;
                            case OppositeDirSignalAction.Close:
                                order.OrdLots = position.PosLots;
                                wayPointType  = WayPointType.Exit;
                                break;
                            case OppositeDirSignalAction.Reverse:
                                order.OrdLots = position.PosLots + TradingSize(Strategy.EntryLots, bar);
                                wayPointType  = WayPointType.Reverse;
                                break;
                            case OppositeDirSignalAction.Nothing:
                                order.OrdStatus = OrderStatus.Cancelled;
                                wayPointType    = WayPointType.Cancel;
                                FindCancelExitOrder(bar, order);  // Canceling of its exit order
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {   // Exit orders
                    if (ordDir == OrderDirection.Buy  && posDir == PosDirection.Short ||
                        ordDir == OrderDirection.Sell && posDir == PosDirection.Long)
                    {   // The Close strategy can only close the position
                        order.OrdLots = position.PosLots;
                        wayPointType = WayPointType.Exit;
                    }
                    else
                    {   // If the direction of the exit order is same as the position's direction
                        // the order have to be cancelled
                        order.OrdStatus = OrderStatus.Cancelled;
                        wayPointType    = WayPointType.Cancel;
                    }
                }
            }
            else
            {   // We are out of the market
                if (order.OrdSender == OrderSender.Open)
                {   // Open a new position
                    order.OrdLots = Math.Min(TradingSize(Strategy.EntryLots, bar), maximumLots);
                    wayPointType  = WayPointType.Entry;
                }
                else// if (order.OrdSender == OrderSender.Close)
                {   // The Close strategy cannot do anything
                    order.OrdStatus = OrderStatus.Cancelled;
                    wayPointType    = WayPointType.Cancel;
                }
            }

            // Enter Once can cancel an entry order
            if (order.OrdSender == OrderSender.Open && order.OrdStatus == OrderStatus.Confirmed)
            {
                foreach (IndicatorSlot slot in Strategy.Slot)
                    if (slot.IndicatorName == "Enter Once")
                    {
                        bool toCancel = false;
                        switch (slot.IndParam.ListParam[0].Text)
                        {
                            case "Enter no more than once a bar":
                                toCancel = Time[bar] == lastEntryTime;
                                break;
                            case "Enter no more than once a day":
                                toCancel = Time[bar].DayOfYear == lastEntryTime.DayOfYear;
                                break;
                            case "Enter no more than once a week":
                                toCancel = (Time[bar].DayOfWeek >= lastEntryTime.DayOfWeek && Time[bar] < lastEntryTime.AddDays(7));
                                break;
                            case "Enter no more than once a month":
                                toCancel = Time[bar].Month == lastEntryTime.Month;
                                break;
                            default:
                                break;
                        }

                        if (toCancel)
                        {   // Cancel the entry order
                            order.OrdStatus = OrderStatus.Cancelled;
                            wayPointType    = WayPointType.Cancel;
                            FindCancelExitOrder(bar, order);  // Canceling of its exit order
                            break;
                        }
                        else
                            lastEntryTime = Time[bar];
                    }
            }

            // Do not trade after Margin Call or after -1000000 Loss
            if (order.OrdSender == OrderSender.Open && order.OrdStatus == OrderStatus.Confirmed)
            {
                if (position.FreeMargin < -1000000 ||
                    Configs.TradeUntilMarginCall && RequiredMargin(order.OrdLots, bar) > position.FreeMargin)
                {   // Cancel the entry order
                    order.OrdStatus = OrderStatus.Cancelled;
                    wayPointType    = WayPointType.Cancel;
                    FindCancelExitOrder(bar, order);  // Canceling of its exit order
                }
            }

            // Executing the order
            if (order.OrdStatus == OrderStatus.Confirmed)
            {   // Executes the order
                SetPosition(bar, ordDir, order.OrdLots, price, order.OrdNumb);
                order.OrdStatus = OrderStatus.Executed;

                // Set the evaluation
                switch (testEval)
                {
                    case BacktestEval.Error:
                        session[bar].BacktestEval = BacktestEval.Error;
                        break;
                    case BacktestEval.None:
                        break;
                    case BacktestEval.Ambiguous:
                        session[bar].BacktestEval = BacktestEval.Ambiguous;
                        break;
                    case BacktestEval.Unknown:
                        if (session[bar].BacktestEval != BacktestEval.Ambiguous)
                            session[bar].BacktestEval = BacktestEval.Unknown;
                        break;
                    case BacktestEval.Correct:
                        if (session[bar].BacktestEval == BacktestEval.None)
                            session[bar].BacktestEval = BacktestEval.Correct;
                        break;
                    default:
                        break;
                }

                // If entry order closes or reverses the position the exit orthers of the
                // initial position have to be cancelled
                if (order.OrdSender == OrderSender.Open &&
                    (session[bar].Summary.Transaction == Transaction.Close ||
                     session[bar].Summary.Transaction == Transaction.Reverse))
                {
                    int initialNumber = session[bar].Position[session[bar].Positions - 2].FormOrdNumb;
                    // If the position was opened during the current bar, we can find its exit order
                    bool isFound = false;
                    for (int ord = 0; ord < session[bar].Orders; ord++)
                    {
                        if (session[bar].Order[ord].OrdIF == initialNumber &&
                            session[bar].Order[ord].OrdSender == OrderSender.Close)
                        {
                            session[bar].Order[ord].OrdStatus = OrderStatus.Cancelled;
                            isFound = true;
                            break;
                        }
                    }

                    // In case when the order is not found, this means that the position is transfered
                    // so its exit order is not conditional
                    if (!isFound)
                    {
                        for (int ord = 0; ord < session[bar].Orders; ord++)
                        {
                            if (session[bar].Order[ord].OrdSender == OrderSender.Close &&
                                session[bar].Order[ord].OrdIF == 0)
                            {
                                session[bar].Order[ord].OrdStatus = OrderStatus.Cancelled;
                                break;
                            }
                        }
                    }

                    // Setting the exit order of the current position
                    if (session[bar].Summary.Transaction == Transaction.Close)
                    {   // In case of closing we have to cancel the exit order
                        int number = session[bar].Summary.FormOrdNumb;
                        for (int ord = 0; ord < session[bar].Orders; ord++)
                        {
                            if (session[bar].Order[ord].OrdIF == number)
                            {
                                session[bar].Order[ord].OrdStatus = OrderStatus.Cancelled;
                                break;
                            }
                        }
                    }
                    else if (session[bar].Summary.Transaction == Transaction.Reduce)
                    {   // In case of reducing we have to change the direction of the exit order
                        int number = session[bar].Summary.FormOrdNumb;
                        for (int ord = 0; ord < session[bar].Orders; ord++)
                        {
                            if (session[bar].Order[ord].OrdIF == number)
                            {
                                if (session[bar].Summary.PosDir == PosDirection.Long)
                                    session[bar].Order[ord].OrdDir = OrderDirection.Sell;
                                else
                                    session[bar].Order[ord].OrdDir = OrderDirection.Buy;
                                break;
                            }
                        }
                    }
                }
            }

            session[bar].SetWayPoint(price, wayPointType);
            if (order.OrdStatus == OrderStatus.Cancelled)
            {
                session[bar].WayPoint[session[bar].WayPoints - 1].OrdNumb = order.OrdNumb;
            }

            return order.OrdStatus;
        }

        /// <summary>
        /// Finds and cancels the exit order of an entry order
        /// </summary>
        static void FindCancelExitOrder(int bar, Order order)
        {
            for (int ord = 0; ord < session[bar].Orders; ord++)
                if (session[bar].Order[ord].OrdIF == order.OrdNumb)
                {
                    session[bar].Order[ord].OrdStatus = OrderStatus.Cancelled;
                    break;
                }
        }

        /// <summary>
        /// Transfers the orders and positions from the previous bar.
        /// <summary>
        static void TransferFromPreviousBar(int bar)
        {
            // Check the previous bar for an open position
            if (session[bar - 1].Summary.PosDir == PosDirection.Long ||
                session[bar - 1].Summary.PosDir == PosDirection.Short)
            {   // Yes, we have a position
                // We copy the position from the previous bar
                int sessionPosition = session[bar].Positions;
                Position position = session[bar].Position[sessionPosition] = session[bar - 1].Summary.Copy();

                // How many days we transfer the positions with
                int days = Time[bar].DayOfYear - Time[bar - 1].DayOfYear;
                if (days < 0) days += 365;

                position.Rollover      = 0;
                position.MoneyRollover = 0;

                if (days > 0)
                {   // Calculate the Rollover fee
                    double swapLongPips  = 0;
                    double swapShortPips = 0;

                    if (InstrProperties.SwapType == Commission_Type.pips)
                    {
                        swapLongPips  = InstrProperties.SwapLong;
                        swapShortPips = InstrProperties.SwapShort;
                    }
                    else if (InstrProperties.SwapType == Commission_Type.percents)
                    {
                        swapLongPips  = (Close[bar - 1] / InstrProperties.Point) * (0.01 * InstrProperties.SwapLong  / 365);
                        swapShortPips = (Close[bar - 1] / InstrProperties.Point) * (0.01 * InstrProperties.SwapShort / 365);
                    }
                    else if (InstrProperties.SwapType == Commission_Type.money)
                    {
                        swapLongPips  = InstrProperties.SwapLong  / (InstrProperties.Point * InstrProperties.LotSize);
                        swapShortPips = InstrProperties.SwapShort / (InstrProperties.Point * InstrProperties.LotSize);
                    }

                    if (position.PosDir == PosDirection.Long)
                    {
                        position.PosPrice     += InstrProperties.Point * days * swapLongPips;
                        position.Rollover      = position.PosLots * days * swapLongPips;
                        position.MoneyRollover = position.PosLots * days * swapLongPips * InstrProperties.Point * InstrProperties.LotSize / AccountExchangeRate(Close[bar - 1]);
                    }
                    else
                    {
                        position.PosPrice     += InstrProperties.Point * days * swapShortPips;
                        position.Rollover      = -position.PosLots * days * swapShortPips;
                        position.MoneyRollover = -position.PosLots * days * swapShortPips * InstrProperties.Point * InstrProperties.LotSize / AccountExchangeRate(Close[bar - 1]);
                    }
                }

                if (position.PosDir == PosDirection.Long)
                {
                    position.FloatingPL      = position.PosLots * (Close[bar] - position.PosPrice) / InstrProperties.Point;
                    position.MoneyFloatingPL = position.PosLots * (Close[bar] - position.PosPrice) * InstrProperties.LotSize / AccountExchangeRate(Close[bar]);
                }
                else
                {
                    position.FloatingPL      = position.PosLots * (position.PosPrice - Close[bar]) / InstrProperties.Point;
                    position.MoneyFloatingPL = position.PosLots * (position.PosPrice - Close[bar]) * InstrProperties.LotSize / AccountExchangeRate(Close[bar]);
                }

                position.PosNumb         = totalPositions;
                position.Transaction     = Transaction.Transfer;
                position.RequiredMargin  = RequiredMargin(position.PosLots, bar);
                position.Spread          = 0;
                position.Commission      = 0;
                position.Slippage        = 0;
                position.ProfitLoss      = 0;
                position.Equity          = position.Balance + position.FloatingPL;
                position.MoneySpread     = 0;
                position.MoneyCommission = 0;
                position.MoneySlippage   = 0;
                position.MoneyProfitLoss = 0;
                position.MoneyEquity     = position.MoneyBalance + position.MoneyFloatingPL;

                posCoord[totalPositions].Bar = bar;
                posCoord[totalPositions].Pos = sessionPosition;
                session[bar].Positions++;
                totalPositions++;

                // Saves the Trailing Stop price
                if (Strategy.Slot[Strategy.CloseSlot].IndicatorName == "Trailing Stop" &&
                    session[bar - 1].Summary.Transaction != Transaction.Transfer)
                {
                    double deltaStop = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value * InstrProperties.Point;
                    double stop = position.FormOrdPrice + (position.PosDir == PosDirection.Long ? -deltaStop : deltaStop);
                    Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1] = stop;
                }

                // Saves the Trailing Stop Limit price
                if (Strategy.Slot[Strategy.CloseSlot].IndicatorName == "Trailing Stop Limit" &&
                    session[bar - 1].Summary.Transaction != Transaction.Transfer)
                {
                    double deltaStop = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value * InstrProperties.Point;
                    double stop = position.FormOrdPrice + (position.PosDir == PosDirection.Long ? -deltaStop : deltaStop);
                    Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1] = stop;
                }

                // Saves the ATR Stop price
                if (Strategy.Slot[Strategy.CloseSlot].IndicatorName == "ATR Stop" &&
                    session[bar - 1].Summary.Transaction != Transaction.Transfer)
                {
                    double deltaStop = Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1];
                    double stop = position.FormOrdPrice + (position.PosDir == PosDirection.Long ? -deltaStop : deltaStop);
                    Strategy.Slot[Strategy.CloseSlot].Component[1].Value[bar - 1] = stop;
                }

                // Saves the Account Percen Stop price
                if (Strategy.Slot[Strategy.CloseSlot].IndicatorName == "Account Percent Stop" &&
                    session[bar - 1].Summary.Transaction != Transaction.Transfer)
                {
                    double deltaMoney = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value * MoneyBalance(bar - 1) / (100 * position.PosLots);
                    double deltaStop  = Math.Max(MoneyToPips(deltaMoney, bar), 5) * InstrProperties.Point;
                    double stop = position.FormOrdPrice + (position.PosDir == PosDirection.Long ? -deltaStop : deltaStop);
                    Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1] = stop;
                }
            }
            else
            {   // When there is no position transffer the old balance and equity
                session[bar].Summary.Balance      = session[bar - 1].Summary.Balance;
                session[bar].Summary.Equity       = session[bar - 1].Summary.Equity;
                session[bar].Summary.MoneyBalance = session[bar - 1].Summary.MoneyBalance;
                session[bar].Summary.MoneyEquity  = session[bar - 1].Summary.MoneyEquity;
            }

            // Transfer all confirmed orders
            for (int iOrd = 0; iOrd < session[bar-1].Orders; iOrd++)
                if (session[bar - 1].Order[iOrd].OrdStatus == OrderStatus.Confirmed)
                {
                    int iSessionOrder = session[bar].Orders;
                    Order order = session[bar].Order[iSessionOrder] = session[bar - 1].Order[iOrd].Copy();
                    ordCoord[order.OrdNumb].Bar = bar;
                    ordCoord[order.OrdNumb].Ord = iSessionOrder;
                    session[bar].Orders++;
                }

            return;
        }

        /// <summary>
        /// Sets an entry order
        /// </summary>
        static void SetEntryOrders(int bar, double price, PosDirection posDir, double lots)
        {
            if (lots < 0.005)
                return; // This is a manner of cancellation an order.

            int ifOrder = 0;
            int toPos   = 0;

            if (posDir == PosDirection.Long)
            {
                if (openStrPriceType == StrategyPriceType.Open || openStrPriceType == StrategyPriceType.Close)
                    OrdBuyMarket(bar, ifOrder, toPos, lots, price, OrderSender.Open, OrderOrigin.Strategy, Language.T("Entry Order"));
                else if (price > Open[bar])
                    OrdBuyStop(bar, ifOrder, toPos, lots, price, OrderSender.Open, OrderOrigin.Strategy, Language.T("Entry Order"));
                else if (price < Open[bar])
                    OrdBuyLimit(bar, ifOrder, toPos, lots, price, OrderSender.Open, OrderOrigin.Strategy, Language.T("Entry Order"));
                else
                    OrdBuyMarket(bar, ifOrder, toPos, lots, price, OrderSender.Open, OrderOrigin.Strategy, Language.T("Entry Order"));
            }
            else
            {
                if (openStrPriceType == StrategyPriceType.Open || openStrPriceType == StrategyPriceType.Close)
                    OrdSellMarket(bar, ifOrder, toPos, lots, price, OrderSender.Open, OrderOrigin.Strategy, Language.T("Entry Order"));
                else if (price < Open[bar])
                    OrdSellStop(bar, ifOrder, toPos, lots, price, OrderSender.Open, OrderOrigin.Strategy, Language.T("Entry Order"));
                else if (price > Open[bar])
                    OrdSellLimit(bar, ifOrder, toPos, lots, price, OrderSender.Open, OrderOrigin.Strategy, Language.T("Entry Order"));
                else
                    OrdSellMarket(bar, ifOrder, toPos, lots, price, OrderSender.Open, OrderOrigin.Strategy, Language.T("Entry Order"));
            }

            return;
        }

        /// <summary>
        /// Sets an exit order
        /// </summary>
        static void SetExitOrders(int bar, double priceStopLong, double priceStopShort)
        {
            // When there is a Long Position we send a Stop Order to it
            if (session[bar].Summary.PosDir == PosDirection.Long)
            {
                double lots    = session[bar].Summary.PosLots;
                int    ifOrder = 0;
                int    toPos   = session[bar].Summary.PosNumb;
                string note    = Language.T("Exit Order to position") + " " + (toPos + 1);

                if (closeStrPriceType == StrategyPriceType.Close)
                    OrdSellMarket(bar, ifOrder, toPos, lots, priceStopLong, OrderSender.Close, OrderOrigin.Strategy, note);
                // The Stop Price can't be higher from the bar's opening price
                else if (priceStopLong < Open[bar])
                    OrdSellStop(bar, ifOrder, toPos, lots, priceStopLong, OrderSender.Close, OrderOrigin.Strategy, note);
                else
                    OrdSellMarket(bar, ifOrder, toPos, lots, priceStopLong, OrderSender.Close, OrderOrigin.Strategy, note);
            }

            // When there is a Short Position we send a Stop Order to it
            if (session[bar].Summary.PosDir == PosDirection.Short)
            {
                double lots    = session[bar].Summary.PosLots;
                int    ifOrder = 0;
                int    toPos   = session[bar].Summary.PosNumb;
                string note    = Language.T("Exit Order to position") + " " + (toPos + 1);

                if (closeStrPriceType == StrategyPriceType.Close)
                    OrdBuyMarket(bar, ifOrder, toPos, lots, priceStopShort, OrderSender.Close, OrderOrigin.Strategy, note);
                // The Stop Price can't be lower from the bar's opening price
                else if (priceStopShort > Open[bar])
                    OrdBuyStop(bar, ifOrder, toPos, lots, priceStopShort, OrderSender.Close, OrderOrigin.Strategy, note);
                else
                    OrdBuyMarket(bar, ifOrder, toPos, lots, priceStopShort, OrderSender.Close, OrderOrigin.Strategy, note);
            }

            // We send IfOrder Order Stop for each Entry Order
            for (int iOrd = 0; iOrd<session[bar].Orders; iOrd++)
            {
                Order entryOrder = session[bar].Order[iOrd];
                if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                {
                    int    ifOrder = entryOrder.OrdNumb;
                    int    toPos   = 0;
                    double lots    = entryOrder.OrdLots;
                    string note    = Language.T("Exit Order to order") + " " + (ifOrder + 1);

                    if (entryOrder.OrdDir == OrderDirection.Buy)
                        OrdSellStop(bar, ifOrder, toPos, lots, priceStopLong, OrderSender.Close, OrderOrigin.Strategy, note);
                    else
                        OrdBuyStop(bar, ifOrder, toPos, lots, priceStopShort, OrderSender.Close, OrderOrigin.Strategy, note);
                }
            }

            return;
        }

        /// <summary>
        /// Checks the slots for a permission to open a position.
        /// If there are no filters that forbid it, sets the entry orders.
        /// </summary>
        static void AnalyseEntry(int bar)
        {
            // Do not send entry order when we are not on time
            if (openTimeExec == ExecutionTime.AtBarOpening && Strategy.Slot[Strategy.OpenSlot].Component[0].Value[bar] < 0.5)
                return;

            // Determining of the buy/sell entry price
            double openLongPrice  = 0;
            double openShortPrice = 0;
            for (int comp = 0; comp < Strategy.Slot[Strategy.OpenSlot].Component.Length; comp++)
            {
                IndComponentType compType = Strategy.Slot[Strategy.OpenSlot].Component[comp].DataType;

                if (compType == IndComponentType.OpenLongPrice)
                    openLongPrice = Strategy.Slot[Strategy.OpenSlot].Component[comp].Value[bar];
                else if (compType == IndComponentType.OpenShortPrice)
                    openShortPrice = Strategy.Slot[Strategy.OpenSlot].Component[comp].Value[bar];
                else if (compType == IndComponentType.OpenPrice || compType == IndComponentType.OpenClosePrice)
                    openLongPrice = openShortPrice = Strategy.Slot[Strategy.OpenSlot].Component[comp].Value[bar];
            }

            // Decide whether to open
            bool canOpenLong  = openLongPrice  > InstrProperties.Point;
            bool canOpenShort = openShortPrice > InstrProperties.Point;

            if (Configs.UseLogicalGroups)
            {
                foreach (string group in openingLogicGroups)
                {
                    bool groupOpenLong  = canOpenLong;
                    bool groupOpenShort = canOpenShort;

                    EntryLogicConditions(bar, group, openLongPrice, openShortPrice, ref groupOpenLong, ref groupOpenShort);

                    groupsAllowLong[group]  = groupOpenLong;
                    groupsAllowShort[group] = groupOpenShort;
                }

                bool groupLongEntry = false;
                foreach (KeyValuePair<string, bool> groupLong in groupsAllowLong)
                    if ((groupsAllowLong.Count > 1 && groupLong.Key != "All") || groupsAllowLong.Count == 1)
                        groupLongEntry = groupLongEntry || groupLong.Value;

                bool groupShortEntry = false;
                foreach (KeyValuePair<string, bool> groupShort in groupsAllowShort)
                    if ((groupsAllowShort.Count > 1 && groupShort.Key != "All") || groupsAllowShort.Count == 1)
                        groupShortEntry = groupShortEntry || groupShort.Value;

                canOpenLong  = canOpenLong  && groupLongEntry  && groupsAllowLong["All"];
                canOpenShort = canOpenShort && groupShortEntry && groupsAllowShort["All"];
            }
            else
            {
                EntryLogicConditions(bar, "A", openLongPrice, openShortPrice, ref canOpenLong, ref canOpenShort);
            }

            if (canOpenLong && canOpenShort && Math.Abs(openLongPrice - openShortPrice) < micron)
            {
                session[bar].BacktestEval = BacktestEval.Ambiguous;
            }
            else
            {
                if (canOpenLong)
                    SetEntryOrders(bar, openLongPrice, PosDirection.Long, TradingSize(Strategy.EntryLots, bar));
                if (canOpenShort)
                    SetEntryOrders(bar, openShortPrice, PosDirection.Short, TradingSize(Strategy.EntryLots, bar));
            }

            return;
        }

        /// <summary>
        /// checks if the opening logic conditions allow long or short entry.
        /// </summary>
        static void EntryLogicConditions(int bar, string group, double buyPrice, double sellPrice, ref bool canOpenLong, ref bool canOpenShort)
        {
            for (int slot = 0; slot < Strategy.CloseSlot + 1; slot++)
            {
                if (Configs.UseLogicalGroups && Strategy.Slot[slot].LogicalGroup != group && Strategy.Slot[slot].LogicalGroup != "All")
                    continue;

                foreach (IndicatorComp component in Strategy.Slot[slot].Component)
                {
                    if (component.DataType == IndComponentType.AllowOpenLong && component.Value[bar] < 0.5)
                        canOpenLong = false;

                    if (component.DataType == IndComponentType.AllowOpenShort && component.Value[bar] < 0.5)
                        canOpenShort = false;

                    if (component.PosPriceDependence != PositionPriceDependence.None)
                    {
                        double indVal = component.Value[bar - component.UsePreviousBar];
                        switch (component.PosPriceDependence)
                        {
                            case PositionPriceDependence.PriceBuyHigher:
                                canOpenLong = canOpenLong == true && buyPrice > indVal + micron;
                                break;
                            case PositionPriceDependence.PriceBuyLower:
                                canOpenLong = canOpenLong == true && buyPrice < indVal - micron;
                                break;
                            case PositionPriceDependence.PriceSellHigher:
                                canOpenShort = canOpenShort == true && sellPrice > indVal + micron;
                                break;
                            case PositionPriceDependence.PriceSellLower:
                                canOpenShort = canOpenShort == true && sellPrice < indVal - micron;
                                break;
                            case PositionPriceDependence.BuyHigherSellLower:
                                canOpenLong  = canOpenLong  == true && buyPrice  > indVal + micron;
                                canOpenShort = canOpenShort == true && sellPrice < indVal - micron;
                                break;
                            case PositionPriceDependence.BuyLowerSelHigher:
                                canOpenLong  = canOpenLong  == true && buyPrice  < indVal - micron;
                                canOpenShort = canOpenShort == true && sellPrice > indVal + micron;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the close orders for the indicated bar.
        /// </summary>
        static void AnalyseExit(int bar)
        {
            #region On Time Closing indicators

            if (closeTimeExec == ExecutionTime.AtBarClosing &&
                Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar] == 0f)
                return;

            #endregion

            #region Indicator "Account Percent Stop"

            if (Strategy.Slot[Strategy.CloseSlot].IndicatorName == "Account Percent Stop")
            {
                // If there is a transferred position, sends a Stop Order for it.
                if (session[bar].Summary.PosDir == PosDirection.Long &&
                    session[bar].Summary.Transaction == Transaction.Transfer)
                {
                    int    ifOrder = 0;
                    int    toPos   = session[bar].Summary.PosNumb;
                    double lots    = session[bar].Summary.PosLots;
                    double stop    = Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1];

                    if (stop > Open[bar])
                        stop = Open[bar];

                    string note = Language.T("Stop order to position") + " " + (toPos + 1);
                    OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                    Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar] = stop;
                }
                else if (session[bar].Summary.PosDir == PosDirection.Short &&
                    session[bar].Summary.Transaction == Transaction.Transfer)
                {
                    int    ifOrder = 0;
                    int    toPos   = session[bar].Summary.PosNumb;
                    double lots    = session[bar].Summary.PosLots;
                    double stop    = Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1];

                    if (stop < Open[bar])
                        stop = Open[bar];

                    string note = Language.T("Stop order to position") + " " + (toPos + 1);
                    OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                    Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar] = stop;
                }


                // If there is a new position, sends a Stop Order for it.
                if (session[bar].Summary.PosDir == PosDirection.Long && session[bar].Summary.Transaction != Transaction.Transfer)
                {
                    int    ifOrder = 0;
                    int    toPos   = session[bar].Summary.PosNumb;
                    double lots    = session[bar].Summary.PosLots;
                    double deltaMoney = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value * MoneyBalance(bar) / (100 * lots);
                    double deltaStop  = Math.Max(MoneyToPips(deltaMoney, bar), 5) * InstrProperties.Point;
                    double stop = session[bar].Summary.FormOrdPrice - deltaStop;
                    string note = Language.T("Stop order to position") + " " + (toPos + 1);

                    if (Close[bar - 1] > stop && stop > Open[bar])
                        OrdSellMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                    else
                        OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                }
                else if (session[bar].Summary.PosDir == PosDirection.Short && session[bar].Summary.Transaction != Transaction.Transfer)
                {
                    int    ifOrder = 0;
                    int    toPos   = session[bar].Summary.PosNumb;
                    double lots    = session[bar].Summary.PosLots;
                    double deltaMoney = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value * MoneyBalance(bar) / (100 * lots);
                    double deltaStop  = Math.Max(MoneyToPips(deltaMoney, bar), 5) * InstrProperties.Point;
                    double stop = session[bar].Summary.FormOrdPrice + deltaStop;
                    string note = Language.T("Stop order to position") + " " + (toPos + 1);

                    if (Open[bar] > stop && stop > Close[bar - 1])
                        OrdBuyMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                    else
                        OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                }

                // If there are Open orders, sends an if order for each of them.
                for (int iOrd = 0; iOrd < session[bar].Orders; iOrd++)
                {
                    Order entryOrder = session[bar].Order[iOrd];

                    if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                    {
                        int    ifOrder = entryOrder.OrdNumb;
                        int    toPos = 0;
                        double lots  = entryOrder.OrdLots;
                        double stop  = entryOrder.OrdPrice;
                        double deltaMoney = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value * MoneyBalance(bar) / (100 * lots);
                        double deltaStop  = -Math.Max(MoneyToPips(deltaMoney, bar), 5) * InstrProperties.Point;
                        string note = Language.T("Stop Order to order") + " " + (ifOrder + 1);

                        if (entryOrder.OrdDir == OrderDirection.Buy)
                        {
                            stop -= deltaStop;
                            OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                        }
                        else
                        {
                            stop += deltaStop;
                            OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                        }
                    }
                }

                return;
            }

            #endregion

            #region Indicator "ATR Stop"

            if (Strategy.Slot[Strategy.CloseSlot].IndicatorName == "ATR Stop")
            {
                double deltaStop = Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar];

                // If there is a transferred position, sends a Stop Order for it.
                if (session[bar].Summary.PosDir == PosDirection.Long &&
                    session[bar].Summary.Transaction == Transaction.Transfer)
                {
                    int    ifOrder = 0;
                    int    toPos   = session[bar].Summary.PosNumb;
                    double lots    = session[bar].Summary.PosLots;
                    double stop    = Strategy.Slot[Strategy.CloseSlot].Component[1].Value[bar - 1];

                    if (stop > Open[bar])
                        stop = Open[bar];

                    string note = Language.T("ATR Stop to position") + " " + (toPos + 1);
                    OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                    Strategy.Slot[Strategy.CloseSlot].Component[1].Value[bar] = stop;
                }
                else if (session[bar].Summary.PosDir == PosDirection.Short &&
                    session[bar].Summary.Transaction == Transaction.Transfer)
                {
                    int    ifOrder = 0;
                    int    toPos   = session[bar].Summary.PosNumb;
                    double lots    = session[bar].Summary.PosLots;
                    double stop    = Strategy.Slot[Strategy.CloseSlot].Component[1].Value[bar - 1];

                    if (stop < Open[bar])
                        stop = Open[bar];

                    string note = Language.T("ATR Stop to position") + " " + (toPos + 1);
                    OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                    Strategy.Slot[Strategy.CloseSlot].Component[1].Value[bar] = stop;
                }

                // If there is a new position, sends an ATR Stop Order for it.
                if (session[bar].Summary.PosDir == PosDirection.Long && session[bar].Summary.Transaction != Transaction.Transfer)
                {
                    int    ifOrder = 0;
                    int    toPos   = session[bar].Summary.PosNumb;
                    double lots = session[bar].Summary.PosLots;
                    double stop = session[bar].Summary.FormOrdPrice - deltaStop;
                    string note = Language.T("ATR Stop to position") + " " + (toPos + 1);

                    if (Close[bar - 1] > stop && stop > Open[bar])
                        OrdSellMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                    else
                        OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                }
                else if (session[bar].Summary.PosDir == PosDirection.Short && session[bar].Summary.Transaction != Transaction.Transfer)
                {
                    int    ifOrder = 0;
                    int    toPos   = session[bar].Summary.PosNumb;
                    double lots    = session[bar].Summary.PosLots;
                    double stop    = session[bar].Summary.FormOrdPrice + deltaStop;
                    string note    = Language.T("ATR Stop to position") + " " + (toPos + 1);

                    if (Open[bar] > stop && stop > Close[bar - 1])
                        OrdBuyMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                    else
                        OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                }

                // If there are Open orders, sends an IfOrder ATR Stop for each of them.
                for (int iOrd = 0; iOrd < session[bar].Orders; iOrd++)
                {
                    Order entryOrder = session[bar].Order[iOrd];

                    if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                    {
                        int    ifOrder = entryOrder.OrdNumb;
                        int    toPos   = 0;
                        double lots = entryOrder.OrdLots;
                        double stop = entryOrder.OrdPrice;
                        string note = Language.T("ATR Stop to order") + " " + (ifOrder + 1);

                        if (entryOrder.OrdDir == OrderDirection.Buy)
                        {
                            stop -= deltaStop;
                            OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                        }
                        else
                        {
                            stop += deltaStop;
                            OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                        }
                    }
                }

                return;
            }

            #endregion

            #region Indicator "Stop Limit"

            if (Strategy.Slot[Strategy.CloseSlot].IndicatorName == "Stop Limit")
            {
                // If there is a position, sends a StopLimit Order for it.
                if (session[bar].Summary.PosDir == PosDirection.Long)
                {
                    int    ifOrder = 0;
                    int    toPos  = session[bar].Summary.PosNumb;
                    double lots   = session[bar].Summary.PosLots;
                    double stop   = session[bar].Summary.FormOrdPrice;
                    double dLimit = session[bar].Summary.FormOrdPrice;
                    stop  -= Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value * InstrProperties.Point;
                    dLimit += Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[1].Value * InstrProperties.Point;
                    string note = Language.T("Stop Limit to position") + " " + (toPos + 1);

                    if (Open[bar] > dLimit && dLimit > Close[bar - 1] || Close[bar - 1] > stop && stop > Open[bar])
                        OrdSellMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                    else
                        OrdSellStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                }
                else if (session[bar].Summary.PosDir == PosDirection.Short)
                {
                    int    ifOrder = 0;
                    int    toPos  = session[bar].Summary.PosNumb;
                    double lots   = session[bar].Summary.PosLots;
                    double stop   = session[bar].Summary.FormOrdPrice;
                    double dLimit = session[bar].Summary.FormOrdPrice;
                    stop  += Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value * InstrProperties.Point;
                    dLimit -= Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[1].Value * InstrProperties.Point;
                    string note = Language.T("Stop Limit to position") + " " + (toPos + 1);

                    if (Open[bar] > stop && stop > Close[bar - 1] || Close[bar - 1] > dLimit && dLimit > Open[bar])
                        OrdBuyMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                    else
                        OrdBuyStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                }

                // If there are Entry orders, sends an IfOrder stop for each of them.
                for (int iOrd = 0; iOrd < session[bar].Orders; iOrd++)
                {
                    Order entryOrder = session[bar].Order[iOrd];

                    if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                    {
                        int    ifOrder = entryOrder.OrdNumb;
                        int    toPos   = 0;
                        double lots    = entryOrder.OrdLots;
                        double stop    = entryOrder.OrdPrice;
                        double dLimit  = entryOrder.OrdPrice;
                        string note    = Language.T("Stop Limit to order") + " " + (ifOrder + 1);

                        if (entryOrder.OrdDir == OrderDirection.Buy)
                        {
                            stop  -= Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value * InstrProperties.Point;
                            dLimit += Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[1].Value * InstrProperties.Point;
                            OrdSellStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                        }
                        else
                        {
                            stop  += Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value * InstrProperties.Point;
                            dLimit -= Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[1].Value * InstrProperties.Point;
                            OrdBuyStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                        }
                    }
                }

                return;
            }

            #endregion

            #region Indicator "Stop Loss"

            if (Strategy.Slot[Strategy.CloseSlot].IndicatorName == "Stop Loss")
            {   // The stop is exactly n pips below the entry point (also when add, reduce, reverse)
                double deltaStop = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value * InstrProperties.Point;

                // If there is a position, sends a Stop Order for it.
                if (session[bar].Summary.PosDir == PosDirection.Long)
                {
                    int    ifOrder = 0;
                    int    toPos = session[bar].Summary.PosNumb;
                    double lots  = session[bar].Summary.PosLots;
                    double stop  = session[bar].Summary.FormOrdPrice - deltaStop;
                    string note  = Language.T("Stop Loss to position") + " " + (toPos + 1);

                    if (Close[bar - 1] > stop && stop > Open[bar])
                        OrdSellMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                    else
                        OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                }
                else if(session[bar].Summary.PosDir == PosDirection.Short)
                {
                    int    ifOrder = 0;
                    int    toPos = session[bar].Summary.PosNumb;
                    double lots  = session[bar].Summary.PosLots;
                    double stop  = session[bar].Summary.FormOrdPrice + deltaStop;
                    string note  = Language.T("Stop Loss to position") + " " + (toPos + 1);

                    if (Open[bar] > stop && stop > Close[bar - 1])
                        OrdBuyMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                    else
                        OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                }

                // If there are Entry orders, sends an IfOrder stop for each of them.
                for (int iOrd = 0; iOrd < session[bar].Orders; iOrd++)
                {
                    Order entryOrder = session[bar].Order[iOrd];

                    if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                    {
                        int    ifOrder = entryOrder.OrdNumb;
                        int    toPos = 0;
                        double lots  = entryOrder.OrdLots;
                        double stop  = entryOrder.OrdPrice;
                        string note  = Language.T("Stop Loss to order") + " " + (ifOrder + 1);

                        if (entryOrder.OrdDir == OrderDirection.Buy)
                        {
                            stop -= deltaStop;
                            OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                        }
                        else
                        {
                            stop += deltaStop;
                            OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                        }
                    }
                }

                return;
            }

            #endregion

            #region Indicator "Take Profit"

            if (Strategy.Slot[Strategy.CloseSlot].IndicatorName == "Take Profit")
            {
                double dDeltaLimit = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value * InstrProperties.Point;

                // If there is a position, sends a Limit Order for it.
                if (session[bar].Summary.PosDir == PosDirection.Long)
                {
                    int    ifOrder = 0;
                    int    toPos   = session[bar].Summary.PosNumb;
                    double lots    = session[bar].Summary.PosLots;
                    double dLimit  = session[bar].Summary.FormOrdPrice + dDeltaLimit;
                    string note    = Language.T("Take Profit to position") + " " + (toPos + 1);

                    if (Open[bar] > dLimit && dLimit > Close[bar - 1])
                        OrdSellMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                    else
                        OrdSellLimit(bar, ifOrder, toPos, lots, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                }
                else if (session[bar].Summary.PosDir == PosDirection.Short)
                {
                    int    ifOrder = 0;
                    int    toPos   = session[bar].Summary.PosNumb;
                    double lots    = session[bar].Summary.PosLots;
                    double dLimit  = session[bar].Summary.FormOrdPrice - dDeltaLimit;
                    string note    = Language.T("Take Profit to position") + " " + (toPos + 1);

                    if (Close[bar - 1] > dLimit && dLimit > Open[bar])
                        OrdBuyMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                    else
                        OrdBuyLimit(bar, ifOrder, toPos, lots, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                }

                // If there are Open orders, sends an IfOrder Limit for each of them.
                for (int iOrd = 0; iOrd < session[bar].Orders; iOrd++)
                {
                    Order entryOrder = session[bar].Order[iOrd];

                    if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                    {
                        int    ifOrder = entryOrder.OrdNumb;
                        int    toPos   = 0;
                        double lots    = entryOrder.OrdLots;
                        double dLimit  = entryOrder.OrdPrice;
                        string note    = Language.T("Take Profit to order") + " " + (ifOrder + 1);

                        if (entryOrder.OrdDir == OrderDirection.Buy)
                        {
                            dLimit += dDeltaLimit;
                            OrdSellLimit(bar, ifOrder, toPos, lots, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                        }
                        else
                        {
                            dLimit -= dDeltaLimit;
                            OrdBuyLimit(bar, ifOrder, toPos, lots, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                        }
                    }
                }
                return;
            }

            #endregion

            #region Indicator "Trailing Stop"

            if (Strategy.Slot[Strategy.CloseSlot].IndicatorName == "Trailing Stop")
            {
                double deltaStop = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value * InstrProperties.Point;

                // If there is a transferred position, sends a Stop Order for it.
                if (session[bar].Summary.PosDir == PosDirection.Long &&
                    session[bar].Summary.Transaction == Transaction.Transfer)
                {
                    int    ifOrder = 0;
                    int    toPos   = session[bar].Summary.PosNumb;
                    double lots    = session[bar].Summary.PosLots;
                    double stop    = Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1];

                    // When the position is modified after the previous bar high
                    // we do not modify the Trailing Stop
                    int wayPointOrder = 0;
                    int wayPointHigh  = 0;
                    for (int wayPoint = 0; wayPoint < WayPoints(bar - 1); wayPoint++)
                    {
                        if (WayPoint(bar - 1, wayPoint).OrdNumb == session[bar - 1].Summary.FormOrdNumb)
                            wayPointOrder = wayPoint;
                        if (WayPoint(bar - 1, wayPoint).WPType == WayPointType.High)
                            wayPointHigh = wayPoint;
                    }

                    if (wayPointOrder < wayPointHigh && stop < High[bar - 1] - deltaStop)
                        stop = High[bar - 1] - deltaStop;

                    if (stop > Open[bar])
                        stop = Open[bar];

                    string note = Language.T("Trailing Stop to position") + " " + (toPos + 1);
                    OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                    Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar] = stop;
                }
                else if (session[bar].Summary.PosDir == PosDirection.Short &&
                    session[bar].Summary.Transaction == Transaction.Transfer)
                {
                    int    ifOrder = 0;
                    int    toPos   = session[bar].Summary.PosNumb;
                    double lots    = session[bar].Summary.PosLots;
                    double stop    = Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1];

                    // When the position is modified after the previous bar low
                    // we do not modify the Trailing Stop
                    int wayPointOrder = 0;
                    int wayPointLow   = 0;
                    for (int wayPoint = 0; wayPoint < WayPoints(bar - 1); wayPoint++)
                    {
                        if (WayPoint(bar - 1, wayPoint).OrdNumb == session[bar - 1].Summary.FormOrdNumb)
                            wayPointOrder = wayPoint;
                        if (WayPoint(bar - 1, wayPoint).WPType == WayPointType.Low)
                            wayPointLow = wayPoint;
                    }

                    if (wayPointOrder < wayPointLow && stop > Low[bar - 1] + deltaStop)
                        stop = Low[bar - 1] + deltaStop;

                    if (stop < Open[bar])
                        stop = Open[bar];

                    string note = Language.T("Trailing Stop to position") + " " + (toPos + 1);
                    OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                    Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar] = stop;
                }

                // If there is a new position, sends a Trailing Stop Order for it.
                if (session[bar].Summary.PosDir == PosDirection.Long && session[bar].Summary.Transaction != Transaction.Transfer)
                {
                    int    ifOrder = 0;
                    int    toPos   = session[bar].Summary.PosNumb;
                    double lots    = session[bar].Summary.PosLots;
                    double stop    = session[bar].Summary.FormOrdPrice - deltaStop;
                    string note    = Language.T("Trailing Stop to position") + " " + (toPos + 1);
                    OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                }
                else if (session[bar].Summary.PosDir == PosDirection.Short && session[bar].Summary.Transaction != Transaction.Transfer)
                {
                    int    ifOrder = 0;
                    int    toPos   = session[bar].Summary.PosNumb;
                    double lots    = session[bar].Summary.PosLots;
                    double stop    = session[bar].Summary.FormOrdPrice + deltaStop;
                    string note    = Language.T("Trailing Stop to position") + " " + (toPos + 1);
                    OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                }

                // If there are Open orders, sends an IfOrder Trailing Stop for each of them.
                for (int ord = 0; ord < session[bar].Orders; ord++)
                {
                    Order entryOrder = session[bar].Order[ord];

                    if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                    {
                        int    ifOrder = entryOrder.OrdNumb;
                        int    toPos   = 0;
                        double lots    = entryOrder.OrdLots;
                        double stop    = entryOrder.OrdPrice;
                        string note    = Language.T("Trailing Stop to order") + " " + (ifOrder + 1);

                        if (entryOrder.OrdDir == OrderDirection.Buy)
                        {
                            stop -= deltaStop;
                            OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                        }
                        else
                        {
                            stop += deltaStop;
                            OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.Strategy, note);
                        }
                    }
                }

                return;
            }

            #endregion

            #region Indicator "Trailing Stop Limit"

            if (Strategy.Slot[Strategy.CloseSlot].IndicatorName == "Trailing Stop Limit")
            {
                double deltaStop  = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[0].Value * InstrProperties.Point;
                double dDeltaLimit = Strategy.Slot[Strategy.CloseSlot].IndParam.NumParam[1].Value * InstrProperties.Point;

                // If there is a transferred position, sends a Stop Order for it.
                if (session[bar].Summary.PosDir == PosDirection.Long && session[bar].Summary.Transaction == Transaction.Transfer)
                {
                    int    ifOrder = 0;
                    int    toPos   = session[bar].Summary.PosNumb;
                    double lots    = session[bar].Summary.PosLots;
                    double stop    = Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1];
                    double dLimit  = session[bar].Summary.FormOrdPrice + dDeltaLimit;

                    // When the position is modified after the previous bar high
                    // we do not modify the Trailing Stop
                    int wayPointOrder = 0;
                    int iWayPointHigh  = 0;
                    for (int wayPoint = 0; wayPoint < WayPoints(bar - 1); wayPoint++)
                    {
                        if (WayPoint(bar - 1, wayPoint).OrdNumb == session[bar - 1].Summary.FormOrdNumb)
                            wayPointOrder = wayPoint;
                        if (WayPoint(bar - 1, wayPoint).WPType == WayPointType.High)
                            iWayPointHigh = wayPoint;
                    }

                    if (wayPointOrder < iWayPointHigh && stop < High[bar - 1] - deltaStop)
                        stop = High[bar - 1] - deltaStop;

                    if (stop > Open[bar])
                        stop = Open[bar];

                    string note = Language.T("Trailing Stop Limit to position") + " " + (toPos + 1);
                    if (Open[bar] > dLimit && dLimit > Close[bar - 1] || Close[bar - 1] > stop && stop > Open[bar])
                        OrdSellMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                    else
                        OrdSellStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                    Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar] = stop;
                }
                else if (session[bar].Summary.PosDir == PosDirection.Short && session[bar].Summary.Transaction == Transaction.Transfer)
                {
                    int    ifOrder = 0;
                    int    toPos   = session[bar].Summary.PosNumb;
                    double lots    = session[bar].Summary.PosLots;
                    double stop    = Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar - 1];
                    double dLimit  = session[bar].Summary.FormOrdPrice - dDeltaLimit;

                    // When the position is modified after the previous bar low
                    // we do not modify the Trailing Stop
                    int wayPointOrder = 0;
                    int wayPointLow   = 0;
                    for (int wayPoint = 0; wayPoint < WayPoints(bar - 1); wayPoint++)
                    {
                        if (WayPoint(bar - 1, wayPoint).OrdNumb == session[bar - 1].Summary.FormOrdNumb)
                            wayPointOrder = wayPoint;
                        if (WayPoint(bar - 1, wayPoint).WPType == WayPointType.Low)
                            wayPointLow = wayPoint;
                    }

                    if (wayPointOrder < wayPointLow && stop > Low[bar - 1] + deltaStop)
                        stop = Low[bar - 1] + deltaStop;

                    if (stop < Open[bar])
                        stop = Open[bar];

                    string note = Language.T("Trailing Stop Limit to position") + " " + (toPos + 1);
                    if (Open[bar] > stop && stop > Close[bar - 1] || Close[bar - 1] > dLimit && dLimit > Open[bar])
                        OrdBuyMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.Strategy, note);
                    else
                        OrdBuyStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                    Strategy.Slot[Strategy.CloseSlot].Component[0].Value[bar] = stop;
                }

                // If there is a new position, sends a Trailing Stop Limit Order for it.
                if (session[bar].Summary.PosDir == PosDirection.Long && session[bar].Summary.Transaction != Transaction.Transfer)
                {
                    int    ifOrder = 0;
                    int    toPos   = session[bar].Summary.PosNumb;
                    double lots    = session[bar].Summary.PosLots;
                    double stop    = session[bar].Summary.FormOrdPrice - deltaStop;
                    double dLimit  = session[bar].Summary.FormOrdPrice + dDeltaLimit;
                    string note    = Language.T("Trailing Stop Limit to position") + " " + (toPos + 1);
                    OrdSellStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                }
                else if (session[bar].Summary.PosDir == PosDirection.Short && session[bar].Summary.Transaction != Transaction.Transfer)
                {
                    int    ifOrder = 0;
                    int    toPos   = session[bar].Summary.PosNumb;
                    double lots    = session[bar].Summary.PosLots;
                    double stop    = session[bar].Summary.FormOrdPrice + deltaStop;
                    double dLimit  = session[bar].Summary.FormOrdPrice - dDeltaLimit;
                    string note    = Language.T("Trailing Stop Limit to position") + " " + (toPos + 1);
                    OrdBuyStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                }

                // If there are Open orders, sends an IfOrder Trailing Stop for each of them.
                for (int iOrd = 0; iOrd < session[bar].Orders; iOrd++)
                {
                    Order entryOrder = session[bar].Order[iOrd];

                    if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                    {
                        int    ifOrder = entryOrder.OrdNumb;
                        int    toPos   = 0;
                        double lots    = entryOrder.OrdLots;
                        double stop    = entryOrder.OrdPrice;
                        double dLimit  = entryOrder.OrdPrice;
                        string note    = Language.T("Trailing Stop Limit to order") + " " + (ifOrder + 1);

                        if (entryOrder.OrdDir == OrderDirection.Buy)
                        {
                            stop  -= deltaStop;
                            dLimit += dDeltaLimit;
                            OrdSellStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                        }
                        else
                        {
                            stop  += deltaStop;
                            dLimit -= dDeltaLimit;
                            OrdBuyStopLimit(bar, ifOrder, toPos, lots, stop, dLimit, OrderSender.Close, OrderOrigin.Strategy, note);
                        }
                    }
                }

                return;
            }

            #endregion


            // Searching the components to find the exit price for a long position.
            double priceExitLong = 0;
            foreach (IndicatorComp indComp in Strategy.Slot[Strategy.CloseSlot].Component)
                if (indComp.DataType == IndComponentType.CloseLongPrice ||
                    indComp.DataType == IndComponentType.ClosePrice     ||
                    indComp.DataType == IndComponentType.OpenClosePrice)
                {
                    priceExitLong = indComp.Value[bar];
                    break;
                }

            // Searching the components to find the exit price for a short position.
            double priceExitShort = 0;
            foreach (IndicatorComp indComp in Strategy.Slot[Strategy.CloseSlot].Component)
                if (indComp.DataType == IndComponentType.CloseShortPrice ||
                    indComp.DataType == IndComponentType.ClosePrice      ||
                    indComp.DataType == IndComponentType.OpenClosePrice)
                {
                    priceExitShort = indComp.Value[bar];
                    break;
                }

            if (Strategy.CloseFilters == 0)
            {
                SetExitOrders(bar, priceExitLong, priceExitShort);
                return;
            }

            // If we do not have a position we do not have anything to close.
            if (session[bar].Summary.PosDir != PosDirection.Long &&
                session[bar].Summary.PosDir != PosDirection.Short)
                return;

            if (Configs.UseLogicalGroups)
            {
                foreach (string group in closingLogicGroups)
                {
                    bool isGroupAllowExit = false;
                    for (int slot = Strategy.CloseSlot + 1; slot < Strategy.Slots; slot++)
                    {
                        if (Strategy.Slot[slot].LogicalGroup == group || Strategy.Slot[slot].LogicalGroup == "all")
                        {
                            bool isSlotAllowExit = false;
                            foreach(IndicatorComp component in Strategy.Slot[slot].Component)
                            {   // We are searching the components for a permission to close the position.
                                if (component.Value[bar] == 0)
                                    continue;

                                if (component.DataType == IndComponentType.ForceClose ||
                                    component.DataType == IndComponentType.ForceCloseLong  && session[bar].Summary.PosDir == PosDirection.Long ||
                                    component.DataType == IndComponentType.ForceCloseShort && session[bar].Summary.PosDir == PosDirection.Short)
                                {
                                    isSlotAllowExit = true;
                                    break;
                                }
                            }

                            if (!isSlotAllowExit)
                            {
                                isGroupAllowExit = false;
                                break;
                            }
                            else
                                isGroupAllowExit = true;
                        }
                    }

                    if (isGroupAllowExit)
                    {
                        SetExitOrders(bar, priceExitLong, priceExitShort);
                        break;
                    }
                }
            }
            else
            {
                bool stopSearching = false;
                for (int slot = Strategy.CloseSlot + 1; slot < Strategy.Slots && !stopSearching; slot++)
                {
                    for (int comp = 0; comp < Strategy.Slot[slot].Component.Length && !stopSearching; comp++)
                    {   // We are searching the components for a permission to close the position.
                        if (Strategy.Slot[slot].Component[comp].Value[bar] == 0)
                            continue;

                        IndComponentType compDataType = Strategy.Slot[slot].Component[comp].DataType;

                        if (compDataType == IndComponentType.ForceClose ||
                            compDataType == IndComponentType.ForceCloseLong  && session[bar].Summary.PosDir == PosDirection.Long ||
                            compDataType == IndComponentType.ForceCloseShort && session[bar].Summary.PosDir == PosDirection.Short)
                        {
                            SetExitOrders(bar, priceExitLong, priceExitShort);
                            stopSearching = true;
                        }
                    }
                }
            }

            return;
        }

        /// <summary>
        /// Sets Permanent Stop Loss close orders for the indicated bar
        /// </summary>
        static void AnalysePermanentSLExit(int bar)
        {
            double deltaStop = Strategy.PermanentSL * InstrProperties.Point;

            // If there is a position, sends a Stop Order for it
            if (session[bar].Summary.PosDir == PosDirection.Long)
            {   // Sets Permanent S/L for a Long Position
                int    ifOrder = 0;
                int    toPos   = session[bar].Summary.PosNumb;
                double lots    = session[bar].Summary.PosLots;
                double stop    = Strategy.PermanentSLType == PermanentProtectionType.Absolute ? session[bar].Summary.AbsoluteSL : session[bar].Summary.FormOrdPrice - deltaStop;
                string note    = Language.T("Permanent S/L to position") + " " + (toPos + 1);

                if (Close[bar - 1] > stop && stop > Open[bar])
                    OrdSellMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.PermanentStopLoss, note);
                else
                    OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.PermanentStopLoss, note);
            }
            else if (session[bar].Summary.PosDir == PosDirection.Short)
            {   // Sets Permanent S/L for a Short Position
                int    ifOrder = 0;
                int    toPos   = session[bar].Summary.PosNumb;
                double lots    = session[bar].Summary.PosLots;
                double stop    = Strategy.PermanentSLType == PermanentProtectionType.Absolute ? session[bar].Summary.AbsoluteSL : session[bar].Summary.FormOrdPrice + deltaStop;
                string note    = Language.T("Permanent S/L to position") + " " + (toPos + 1);

                if (Open[bar] > stop && stop > Close[bar - 1])
                    OrdBuyMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.PermanentStopLoss, note);
                else
                    OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.PermanentStopLoss, note);
            }

            // If there are Entry orders, sends an IfOrder stop for each of them.
            for (int ord = 0; ord < session[bar].Orders; ord++)
            {
                Order entryOrder = session[bar].Order[ord];

                if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                {
                    int    ifOrder  = entryOrder.OrdNumb;
                    int    toPos    = 0;
                    double lots     = entryOrder.OrdLots;
                    double ordPrice = entryOrder.OrdPrice;
                    string note     = Language.T("Permanent S/L to order") + " " + (ifOrder + 1);

                    if (entryOrder.OrdDir == OrderDirection.Buy)
                        OrdSellStop(bar, ifOrder, toPos, lots, ordPrice - deltaStop, OrderSender.Close, OrderOrigin.PermanentStopLoss, note);
                    else
                        OrdBuyStop(bar, ifOrder, toPos, lots, ordPrice + deltaStop, OrderSender.Close, OrderOrigin.PermanentStopLoss, note);
                }
            }

            return;
        }

        /// <summary>
        /// Sets Permanent Take Profit close orders for the indicated bar
        /// </summary>
        static void AnalysePermanentTPExit(int bar)
        {
            double deltaStop = Strategy.PermanentTP * InstrProperties.Point;

            // If there is a position, sends a Stop Order for it
            if (session[bar].Summary.PosDir == PosDirection.Long)
            {   // Sets Permanent T/P for a Long Position
                int    ifOrder = 0;
                int    toPos   = session[bar].Summary.PosNumb;
                double lots    = session[bar].Summary.PosLots;
                double stop    = Strategy.PermanentTPType == PermanentProtectionType.Absolute ? session[bar].Summary.AbsoluteTP : session[bar].Summary.FormOrdPrice + deltaStop;
                string note    = Language.T("Permanent T/P to position") + " " + (toPos + 1);

                if (Open[bar] > stop && stop > Close[bar - 1])
                    OrdSellMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.PermanentTakeProfit, note);
                else
                    OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.PermanentTakeProfit, note);
            }
            else if (session[bar].Summary.PosDir == PosDirection.Short)
            {   // Sets Permanent T/P for a Short Position
                int    ifOrder = 0;
                int    toPos   = session[bar].Summary.PosNumb;
                double lots    = session[bar].Summary.PosLots;
                double stop    = Strategy.PermanentTPType == PermanentProtectionType.Absolute ? session[bar].Summary.AbsoluteTP : session[bar].Summary.FormOrdPrice - deltaStop;
                string note    = Language.T("Permanent T/P to position") + " " + (toPos + 1);

                if (Close[bar - 1] > stop && stop > Open[bar])
                    OrdBuyMarket(bar, ifOrder, toPos, lots, Open[bar], OrderSender.Close, OrderOrigin.PermanentTakeProfit, note);
                else
                    OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.PermanentTakeProfit, note);
            }

            // If there are Entry orders, sends an IfOrder stop for each of them.
            for (int iOrd = 0; iOrd < session[bar].Orders; iOrd++)
            {
                Order entryOrder = session[bar].Order[iOrd];

                if (entryOrder.OrdSender == OrderSender.Open && entryOrder.OrdStatus == OrderStatus.Confirmed)
                {
                    int    ifOrder = entryOrder.OrdNumb;
                    int    toPos = 0;
                    double lots  = entryOrder.OrdLots;
                    double stop  = entryOrder.OrdPrice;
                    string note  = Language.T("Permanent T/P to order") + " " + (ifOrder + 1);

                    if (entryOrder.OrdDir == OrderDirection.Buy)
                    {
                        stop += deltaStop;
                        OrdSellStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.PermanentTakeProfit, note);
                    }
                    else
                    {
                        stop -= deltaStop;
                        OrdBuyStop(bar, ifOrder, toPos, lots, stop, OrderSender.Close, OrderOrigin.PermanentTakeProfit, note);
                    }
                }
            }

            return;
        }

        /// <summary>
        /// The main calculating cycle
        /// </summary>
        public static void Calculation()
        {
            ResetStart();

            if (closeStrPriceType == StrategyPriceType.CloseAndReverce)
            {   // Close and Reverse.
                if (openStrPriceType == StrategyPriceType.Open)
                {   // Opening price - Close and Reverse
                    for (int bar = FirstBar; bar < Bars; bar++)
                    {
                        TransferFromPreviousBar(bar);
                        if (FastCalculating(bar)) break;
                        session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                        AnalyseEntry(bar);
                        ExecuteEntryAtOpeningPrice(bar);
                        CancelNoexecutedEntryOrders(bar);
                        if (Strategy.UsePermanentSL)
                            AnalysePermanentSLExit(bar);
                        if (Strategy.UsePermanentTP)
                            AnalysePermanentTPExit(bar);
                        BarInterpolation(bar);
                        CancelNoexecutedExitOrders(bar);
                        MarginCallCheckAtBarClosing(bar);
                        session[bar].SetWayPoint(Close[bar], WayPointType.Close);
                    }
                }
                else if (openStrPriceType == StrategyPriceType.Close)
                {   // Closing price - Close and Reverse
                    for (int bar = FirstBar; bar < Bars; bar++)
                    {
                        TransferFromPreviousBar(bar);
                        if (FastCalculating(bar)) break;
                        session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                        if (Strategy.UsePermanentSL)
                            AnalysePermanentSLExit(bar);
                        if (Strategy.UsePermanentTP)
                            AnalysePermanentTPExit(bar);
                        BarInterpolation(bar);
                        AnalyseEntry(bar);
                        ExecuteEntryAtClosingPrice(bar);
                        CancelInvalidOrders(bar);
                        MarginCallCheckAtBarClosing(bar);
                        session[bar].SetWayPoint(Close[bar], WayPointType.Close);
                    }
                }
                else
                {   // Indicator - Close and Reverse
                    for (int bar = FirstBar; bar < Bars; bar++)
                    {
                        TransferFromPreviousBar(bar);
                        if (FastCalculating(bar)) break;
                        session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                        AnalyseEntry(bar);
                        if (Strategy.UsePermanentSL)
                            AnalysePermanentSLExit(bar);
                        if (Strategy.UsePermanentTP)
                            AnalysePermanentTPExit(bar);
                        BarInterpolation(bar);
                        CancelInvalidOrders(bar);
                        MarginCallCheckAtBarClosing(bar);
                        session[bar].SetWayPoint(Close[bar], WayPointType.Close);
                    }
                }

                ResetStop();

                return;
            }

            if (openStrPriceType == StrategyPriceType.Open)
            {
                if (closeStrPriceType == StrategyPriceType.Close)
                {   // Opening price - Closing price
                    for (int bar = FirstBar; bar < Bars; bar++)
                    {
                        TransferFromPreviousBar(bar);
                        if (FastCalculating(bar)) break;
                        session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                        AnalyseEntry(bar);
                        ExecuteEntryAtOpeningPrice(bar);
                        CancelNoexecutedEntryOrders(bar);
                        if (Strategy.UsePermanentSL)
                            AnalysePermanentSLExit(bar);
                        if (Strategy.UsePermanentTP)
                            AnalysePermanentTPExit(bar);
                        BarInterpolation(bar);
                        CancelNoexecutedExitOrders(bar);
                        AnalyseExit(bar);
                        ExecuteExitAtClosingPrice(bar);
                        CancelNoexecutedExitOrders(bar);
                        MarginCallCheckAtBarClosing(bar);
                        session[bar].SetWayPoint(Close[bar], WayPointType.Close);
                    }
                }
                else
                {   // Opening price - Indicator
                    for (int bar = FirstBar; bar < Bars; bar++)
                    {
                        TransferFromPreviousBar(bar);
                        if (FastCalculating(bar)) break;
                        session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                        AnalyseEntry(bar);
                        ExecuteEntryAtOpeningPrice(bar);
                        CancelNoexecutedEntryOrders(bar);
                        if (Strategy.UsePermanentSL)
                            AnalysePermanentSLExit(bar);
                        if (Strategy.UsePermanentTP)
                            AnalysePermanentTPExit(bar);
                        AnalyseExit(bar);
                        BarInterpolation(bar);
                        CancelInvalidOrders(bar);
                        MarginCallCheckAtBarClosing(bar);
                        session[bar].SetWayPoint(Close[bar], WayPointType.Close);
                    }
                }
            }
            else if (openStrPriceType == StrategyPriceType.Close)
            {
                if (closeStrPriceType == StrategyPriceType.Close)
                {   // Closing price - Closing price
                    for (int bar = FirstBar; bar < Bars; bar++)
                    {
                        AnalyseEntry(bar - 1);
                        ExecuteEntryAtClosingPrice(bar - 1);
                        CancelNoexecutedEntryOrders(bar - 1);
                        MarginCallCheckAtBarClosing(bar - 1);
                        session[bar - 1].SetWayPoint(Close[bar - 1], WayPointType.Close);
                        TransferFromPreviousBar(bar);
                        if (FastCalculating(bar)) break;
                        session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                        if (Strategy.UsePermanentSL)
                            AnalysePermanentSLExit(bar);
                        if (Strategy.UsePermanentTP)
                            AnalysePermanentTPExit(bar);
                        BarInterpolation(bar);
                        CancelNoexecutedExitOrders(bar);
                        AnalyseExit(bar);
                        ExecuteExitAtClosingPrice(bar);
                        CancelNoexecutedExitOrders(bar);
                    }
                }
                else
                {   // Closing price - Indicator
                    for (int bar = FirstBar; bar < Bars; bar++)
                    {
                        AnalyseEntry(bar - 1);
                        ExecuteEntryAtClosingPrice(bar - 1);
                        CancelNoexecutedEntryOrders(bar - 1);
                        MarginCallCheckAtBarClosing(bar - 1);
                        session[bar - 1].SetWayPoint(Close[bar - 1], WayPointType.Close);
                        TransferFromPreviousBar(bar);
                        if (FastCalculating(bar)) break;
                        session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                        if (Strategy.UsePermanentSL)
                            AnalysePermanentSLExit(bar);
                        if (Strategy.UsePermanentTP)
                            AnalysePermanentTPExit(bar);
                        AnalyseExit(bar);
                        BarInterpolation(bar);
                        CancelInvalidOrders(bar);
                    }
                }
            }
            else
            {
                if (closeStrPriceType == StrategyPriceType.Close)
                {   // Indicator - Closing price
                    for (int bar = FirstBar; bar < Bars; bar++)
                    {
                        TransferFromPreviousBar(bar);
                        if (FastCalculating(bar)) break;
                        session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                        AnalyseEntry(bar);
                        if (Strategy.UsePermanentSL)
                            AnalysePermanentSLExit(bar);
                        if (Strategy.UsePermanentTP)
                            AnalysePermanentTPExit(bar);
                        BarInterpolation(bar);
                        CancelInvalidOrders(bar);
                        AnalyseExit(bar);
                        ExecuteExitAtClosingPrice(bar);
                        CancelNoexecutedExitOrders(bar);
                        MarginCallCheckAtBarClosing(bar);
                        session[bar].SetWayPoint(Close[bar], WayPointType.Close);
                    }
                }
                else
                {   // Indicator - Indicator
                    for (int bar = FirstBar; bar < Bars; bar++)
                    {
                        TransferFromPreviousBar(bar);
                        if (FastCalculating(bar)) break;
                        session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                        AnalyseEntry(bar);
                        if (Strategy.UsePermanentSL)
                            AnalysePermanentSLExit(bar);
                        if (Strategy.UsePermanentTP)
                            AnalysePermanentTPExit(bar);
                        AnalyseExit(bar);
                        BarInterpolation(bar);
                        CancelInvalidOrders(bar);
                        MarginCallCheckAtBarClosing(bar);
                        session[bar].SetWayPoint(Close[bar], WayPointType.Close);
                    }
                }
            }

            return;
        }

        /// <summary>
        /// Performs an intrabar scanning
        /// </summary>
        public static void Scan()
        {
            isScanning = true;
            Calculation();
            isScanned  = true;
            CalculateAccountStats();
            isScanning = false;
            return;
        }

        /// <summary>
        /// Calculate strategy
        /// </summary>
        public static void Calculate()
        {
            if (Configs.Autoscan && (IsIntrabarData || Configs.UseTickData && Data.IsTickData))
                Scan();
            else
                Calculation();

            return;
        }

        /// <summary>
        /// Calculate statistics
        /// </summary>
        static bool FastCalculating(int startBar)
        {
            bool isFastCalc = false;

            if (session[startBar].Positions != 0)
                return isFastCalc;

            if (Configs.TradeUntilMarginCall &&
                session[startBar].Summary.FreeMargin < RequiredMargin(TradingSize(Strategy.EntryLots, startBar), startBar) ||
                session[startBar].Summary.FreeMargin < -1000000)
            {
                for (int bar = startBar; bar < Bars; bar++)
                {
                    session[bar].Summary.Balance      = session[bar - 1].Summary.Balance;
                    session[bar].Summary.Equity       = session[bar - 1].Summary.Equity;
                    session[bar].Summary.MoneyBalance = session[bar - 1].Summary.MoneyBalance;
                    session[bar].Summary.MoneyEquity  = session[bar - 1].Summary.MoneyEquity;

                    session[bar].SetWayPoint(Open[bar], WayPointType.Open);
                    ArrangeBarsHighLow(bar);
                    session[bar].SetWayPoint(Close[bar], WayPointType.Close);
                }

                isFastCalc = true;
            }

            return isFastCalc;
        }

        /// <summary>
        /// Arranges the order of hitting the bar's Top and Bottom.
        /// </summary>
        static void ArrangeBarsHighLow(int bar)
        {
            double low  = Low[bar];
            double high = High[bar];
            bool isTopFirst   = false;
            bool isOrderFound = false;

            if (isScanning && IntraBarsPeriods[bar] != Period)
            {
                for (int b = 0; b < IntraBarBars[bar]; b++)
                {
                    if (IntraBarData[bar][b].High + micron > high)
                    {   // Top founded
                        isTopFirst   = true;
                        isOrderFound = true;
                    }

                    if (IntraBarData[bar][b].Low - micron < low)
                    {   // Bottom founded
                        if (isOrderFound)
                        {   // Top and Bottom into the same intrabar
                            isOrderFound = false;
                            break;
                        }
                        isTopFirst   = false;
                        isOrderFound = true;
                    }

                    if (isOrderFound)
                        break;
                }
            }

            if (!isScanning || !isOrderFound)
            {
                isTopFirst = Open[bar] > Close[bar];
            }

            if (isTopFirst)
            {
                session[bar].SetWayPoint(high, WayPointType.High);
                session[bar].SetWayPoint(low,  WayPointType.Low);
            }
            else
            {
                session[bar].SetWayPoint(low,  WayPointType.Low);
                session[bar].SetWayPoint(high, WayPointType.High);
            }

            return;
        }
    }
}