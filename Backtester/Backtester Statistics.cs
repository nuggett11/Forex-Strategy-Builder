// Backtester class - Statistics
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
        static double maxBalance;
        static double minBalance;
        static double maxDrawdown;
        static double maxEquity;
        static double minEquity;
        static double maxEquityDrawdown;
        static double grossProfit;
        static double grossLoss;
        static double maxMoneyBalance;
        static double minMoneyBalance;
        static double maxMoneyDrawdown;
        static double maxMoneyEquity;
        static double minMoneyEquity;
        static double maxMoneyEquityDrawdown;
        static double grossMoneyProfit;
        static double grossMoneyLoss;
        static double moneyChargedSpread;
        static double moneyChargedRollOver;
        static double moneyChargedCommission;
        static double moneyChargedSlippage;
        static double chargedSpread;
        static double chargedRollOver;
        static double chargedCommission;
        static double winLossRatio;
        static double moneyEquityPercentDrawdown;
        static double equityPercentDrawdown;
        static int[] balanceDrawdown;
        static int[] equityDrawdown;

        static int    executedOrders;
        static double tradedLots;
        static int    barsInPosition;
        static double chargedSlippage;
        static int    ambiguousBars;
        static int    unknownBars;
        static int    marginCallBar;
        static int    winningTrades;
        static int    losingTrades;
        static int    totalTrades;
        static int    testedDays;
        
        static bool   isScanned;

        static string[] accountStatsParam = new string[0];
        static string[] accountStatsValue = new string[0];
        static bool[]   accountStatsFlag  = new bool[0];

        static DateTime maxBalanceDate;
        static DateTime minBalanceDate;
        static DateTime maxMoneyBalanceDate;
        static DateTime minMoneyBalanceDate;
        static double   maxMoneyDrawdownPercent;
        static DateTime maxDrawdownDate;
        static DateTime maxMoneyDrawdownDate;

        /// <summary>
        /// Gets the account balance in pips
        /// </summary>
        public static int NetBalance { get { return Backtester.Balance(Bars - 1); } }

        /// <summary>
        /// Gets the max balance in pips
        /// </summary>
        public static int MaxBalance { get { return (int)Math.Round(maxBalance); } }

        /// <summary>
        /// Gets the min balance in pips
        /// </summary>
        public static int MinBalance { get { return (int)Math.Round(minBalance); } }

        /// <summary>
        /// Gets the account balance in currency
        /// </summary>
        public static double NetMoneyBalance { get { return Backtester.MoneyBalance(Bars - 1); } }

        /// <summary>
        /// Gets the max balance in currency
        /// </summary>
        public static double MaxMoneyBalance { get { return maxMoneyBalance; } }

        /// <summary>
        /// Gets the min balance in currency
        /// </summary>
        public static double MinMoneyBalance { get { return minMoneyBalance; } }

        /// <summary>
        /// Gets the max equity
        /// </summary>
        public static int MaxEquity { get { return (int)Math.Round(maxEquity); } }

        /// <summary>
        /// Gets the min equity in pips
        /// </summary>
        public static int MinEquity { get { return (int)Math.Round(minEquity); } }

        /// <summary>
        /// Gets the max Equity in currency
        /// </summary>
        public static double MaxMoneyEquity { get { return maxMoneyEquity; } }

        /// <summary>
        /// Gets the min Equity in currency
        /// </summary>
        public static double MinMoneyEquity { get { return minMoneyEquity; } }

        /// <summary>
        /// Gets the maximum drawdown in the account bill
        /// </summary>
        public static int MaxDrawdown { get { return (int)Math.Round(maxDrawdown); } }

        /// <summary>
        /// Gets the maximum equity drawdown in the account bill
        /// </summary>
        public static int MaxEquityDrawdown { get { return (int)Math.Round(maxEquityDrawdown); } }

        /// <summary>
        /// Gets the maximum money drawdown
        /// </summary>
        public static double MaxMoneyDrawdown { get { return maxMoneyDrawdown; } }

        /// <summary>
        /// Gets the maximum money equity drawdown
        /// </summary>
        public static double MaxMoneyEquityDrawdown { get { return maxMoneyEquityDrawdown; } }

        /// <summary>
        /// The total earned pips
        /// </summary>
        public static int GrossProfit { get { return (int)Math.Round(grossProfit); } }

        /// <summary>
        /// The total earned money
        /// </summary>
        public static double GrossMoneyProfit { get { return grossMoneyProfit; } }

        /// <summary>
        /// The total lost pips
        /// </summary>
        public static int GrossLoss { get { return (int)Math.Round(grossLoss); } }

        /// <summary>
        /// The total lost money
        /// </summary>
        public static double GrossMoneyLoss { get { return grossMoneyLoss; } }

        /// <summary>
        /// Gets the count of executed orders
        /// </summary>
        public static int ExecutedOrders { get { return executedOrders; } }

        /// <summary>
        /// Gets the count of lots have been traded
        /// </summary>
        public static double TradedLots { get { return tradedLots; } }

        /// <summary>
        /// Gets the time in position in percents
        /// </summary>
        public static int TimeInPosition { get { return (int)Math.Round(100f * barsInPosition / (Bars - FirstBar)); } }

        /// <summary>
        /// Gets the count of sent orders
        /// </summary>
        public static int SentOrders { get { return totalOrders; } }

        /// <summary>
        /// Gets the Charged Spread
        /// </summary>
        public static double TotalChargedSpread { get { return chargedSpread; } }

        /// <summary>
        /// Gets the Charged Spread in currency
        /// </summary>
        public static double TotalChargedMoneySpread { get { return moneyChargedSpread; } }

        /// <summary>
        /// Gets the Charged RollOver
        /// </summary>
        public static double TotalChargedRollOver { get { return chargedRollOver; } }

        /// <summary>
        /// Gets the Charged RollOver in currency
        /// </summary>
        public static double TotalChargedMoneyRollOver { get { return moneyChargedRollOver; } }

        /// <summary>
        /// Gets the Charged Slippage
        /// </summary>
        public static double TotalChargedSlippage { get { return chargedSlippage; } }

        /// <summary>
        /// Gets the Charged Slippage in currency
        /// </summary>
        public static double TotalChargedMoneySlippage { get { return moneyChargedSlippage; } }

        /// <summary>
        /// Gets the Charged Commission
        /// </summary>
        public static double TotalChargedCommission { get { return chargedCommission; } }

        /// <summary>
        /// Gets the Charged Commission in currency
        /// </summary>
        public static double TotalChargedMoneyCommission { get { return moneyChargedCommission; } }

        /// <summary>
        /// Winning Trades
        /// </summary>
        public static int WinningTrades { get { return winningTrades; } }

        /// <summary>
        /// Losing Trades
        /// </summary>
        public static int LosingTrades { get { return losingTrades; } }

        /// <summary>
        /// Win / Loss ratio
        /// </summary>
        public static double WinLossRatio { get { return winLossRatio; } }

        /// <summary>
        /// Money Equity Percent Drawdown
        /// </summary>
        public static double MoneyEquityPercentDrawdown { get { return moneyEquityPercentDrawdown; } }

        /// <summary>
        /// Equity Percent Drawdown
        /// </summary>
        public static double EquityPercentDrawdown { get { return equityPercentDrawdown; } }

        /// <summary>
        /// Returns the ambiguous calculated bars
        /// </summary>
        public static int AmbiguousBars { get { return ambiguousBars; } }

        /// <summary>
        /// Was the intrabar scanning performed
        /// </summary>
        public static bool IsScanPerformed { get { return isScanned; } }

        /// <summary>
        /// Margin Call Bar
        /// </summary>
        public static int MarginCallBar { get { return marginCallBar; } }

        /// <summary>
        /// Gets the number of days tested.
        /// </summary>
        public static int TestedDays { get { return testedDays; } }

        /// <summary>
        /// Gets the profit per tetsted day.
        /// </summary>
        public static int ProfitPerDay { get { return testedDays > 0 ? Backtester.Balance(Bars - 1) / testedDays : 0; } }

        /// <summary>
        /// Gets the profit per tetsted day in currency.
        /// </summary>
        public static double MoneyProfitPerDay { get { return testedDays > 0 ? (Backtester.MoneyBalance(Bars - 1) - Configs.InitialAccount) / testedDays : 0; } }

        /// <summary>
        /// Gets the account stats params
        /// </summary>
        public static string[] AccountStatsParam { get { return accountStatsParam; } }

        /// <summary>
        /// Gets the account stats values
        /// </summary>
        public static string[] AccountStatsValue { get { return accountStatsValue; } }

        /// <summary>
        /// Gets the account stats flags
        /// </summary>
        public static bool[] AccountStatsFlags { get { return accountStatsFlag; } }

        /// <summary>
        /// Returns the Balance Drawdown in pips
        /// </summary>
        public static int BalanceDrawdown(int bar)
        {
            return balanceDrawdown[bar];
        }

        /// <summary>
        /// Returns the Equity Drawdown in pips
        /// </summary>
        public static int EquityDrawdown(int bar)
        {
            return equityDrawdown[bar];
        }

        /// <summary>
        /// Returns the Balance Drawdown in currency
        /// </summary>
        public static double MoneyBalanceDrawdown(int bar)
        {
            return balanceDrawdown[bar] * InstrProperties.Point * InstrProperties.LotSize / AccountExchangeRate(Close[bar]);
        }

        /// <summary>
        /// Returns the Equity Drawdown in currency.
        /// </summary>
        public static double MoneyEquityDrawdown(int bar)
        {
            return equityDrawdown[bar] * InstrProperties.Point * InstrProperties.LotSize / AccountExchangeRate(Close[bar]);
        }

        /// <summary>
        /// Calculates the accaunt statistics.
        /// </summary>
        public static void CalculateAccountStats()
        {
            maxBalance = 0;
            minBalance = 0;
            maxEquity  = 0;
            minEquity  = 0;
            maxEquityDrawdown = 0;
            maxDrawdown       = 0;

            maxMoneyBalance = Configs.InitialAccount;
            minMoneyBalance = Configs.InitialAccount;
            maxMoneyEquity  = Configs.InitialAccount;
            minMoneyEquity  = Configs.InitialAccount;
            maxMoneyEquityDrawdown = 0;
            maxMoneyDrawdown       = 0;

            barsInPosition    = 0;
            grossProfit       = 0;
            grossLoss         = 0;
            grossMoneyProfit  = 0;
            grossMoneyLoss    = 0;
            chargedSpread     = 0;
            chargedRollOver   = 0;
            chargedCommission = 0;
            chargedSlippage   = 0;
            moneyChargedSpread     = 0;
            moneyChargedRollOver   = 0;
            moneyChargedCommission = 0;
            moneyChargedSlippage   = 0;
            ambiguousBars     = 0;
            unknownBars       = 0;
            balanceDrawdown  = new int[Bars];
            equityDrawdown   = new int[Bars];

            maxBalanceDate       = Time[0];
            minBalanceDate       = Time[0];
            maxMoneyBalanceDate  = Time[0];
            minMoneyBalanceDate  = Time[0];
            maxDrawdownDate      = Time[0];
            maxMoneyDrawdownDate = Time[0];

            equityPercentDrawdown      = 100;
            maxMoneyDrawdownPercent    = 0;
            moneyEquityPercentDrawdown = 0;
            winLossRatio               = 0;

            winningTrades = 0;
            losingTrades  = 0;
            totalTrades   = 0;
            testedDays    = 0;

            for (int bar = FirstBar; bar < Bars; bar++)
            {
                double accountExchangeRate = AccountExchangeRate(Close[bar]);
                double pipsToMoney = InstrProperties.Point * InstrProperties.LotSize / accountExchangeRate;

                // Balance
                double balance = session[bar].Summary.Balance;
                if (balance > maxBalance)
                {
                    maxBalance = balance;
                    maxBalanceDate = Time[bar];
                }
                if (balance < minBalance)
                {
                    minBalance = balance;
                    minBalanceDate = Time[bar];
                }

                // Money Balance
                double moneyBalance = session[bar].Summary.MoneyBalance;
                if (moneyBalance > maxMoneyBalance)
                {
                    maxMoneyBalance = moneyBalance;
                    maxMoneyBalanceDate = Time[bar];
                }
                if (moneyBalance < minMoneyBalance)
                {
                    minMoneyBalance = moneyBalance;
                    minMoneyBalanceDate = Time[bar];
                }

                // Equity
                double equity = session[bar].Summary.Equity;
                if (equity > maxEquity) maxEquity = equity;
                if (equity < minEquity) minEquity = equity;

                // Money Equity
                double moneyEquity = session[bar].Summary.MoneyEquity;
                if (moneyEquity > maxMoneyEquity) maxMoneyEquity = moneyEquity;
                if (moneyEquity < minMoneyEquity) minMoneyEquity = moneyEquity;

                // Maximum Drawdown
                if (maxBalance - balance > maxDrawdown)
                {
                    maxDrawdown = maxBalance - balance;
                    maxDrawdownDate = Time[bar];
                }

                // Maximum Equity Drawdown
                if (maxEquity - equity > maxEquityDrawdown)
                {
                    maxEquityDrawdown = maxEquity - equity;

                    // In percents
                    if (maxEquity > 0)
                        equityPercentDrawdown = 100 * (maxEquityDrawdown / maxEquity);
                }

                // Maximum Money Drawdown
                if (maxMoneyBalance - MoneyBalance(bar) > maxMoneyDrawdown)
                {
                    maxMoneyDrawdown        = maxMoneyBalance - MoneyBalance(bar);
                    maxMoneyDrawdownPercent = 100 * (maxMoneyDrawdown / maxMoneyBalance);
                    maxMoneyDrawdownDate    = Time[bar];
                }

                // Maximum Money Equity Drawdown
                if (maxMoneyEquity - MoneyEquity(bar) > maxMoneyEquityDrawdown)
                {
                    maxMoneyEquityDrawdown = maxMoneyEquity - MoneyEquity(bar);

                    // Maximum Money Equity Drawdown in percents
                    if (100 * maxMoneyEquityDrawdown / maxMoneyEquity > moneyEquityPercentDrawdown)
                        moneyEquityPercentDrawdown = 100 * (maxMoneyEquityDrawdown / maxMoneyEquity);
                }

                // Drawdown
                balanceDrawdown[bar] = (int)Math.Round((maxBalance - balance));
                equityDrawdown[bar]  = (int)Math.Round((maxEquity  - equity));

                // Bars in position
                if (session[bar].Positions > 0)
                    barsInPosition++;

                // Bar interpolation evaluation
                if (session[bar].BacktestEval == BacktestEval.Ambiguous)
                {
                    ambiguousBars++;
                }
                else if (session[bar].BacktestEval == BacktestEval.Unknown)
                {
                    unknownBars++;
                }

                // Margin Call bar
                if (!Configs.TradeUntilMarginCall && marginCallBar == 0 && session[bar].Summary.FreeMargin < 0)
                    marginCallBar = bar;
            }
         
            for (int pos = 0; pos < PositionsTotal; pos++)
            {   // Charged fees
                Position position = Backtester.PosFromNumb(pos);
                chargedSpread          += position.Spread;
                chargedRollOver        += position.Rollover;
                chargedCommission      += position.Commission;
                chargedSlippage        += position.Slippage;
                moneyChargedSpread     += position.MoneySpread;
                moneyChargedRollOver   += position.MoneyRollover;
                moneyChargedCommission += position.MoneyCommission;
                moneyChargedSlippage   += position.MoneySlippage;

                // Winning losing trades.
                if (position.Transaction == Transaction.Close  ||
                    position.Transaction == Transaction.Reduce ||
                    position.Transaction == Transaction.Reverse)
                {
                    if (position.ProfitLoss > 0)
                    {
                        grossProfit      += position.ProfitLoss;
                        grossMoneyProfit += position.MoneyProfitLoss;
                        winningTrades++;
                    }
                    else if (position.ProfitLoss < 0)
                    {
                        grossLoss      += position.ProfitLoss;
                        grossMoneyLoss += position.MoneyProfitLoss;
                        losingTrades++;
                    }
                    totalTrades++;
                }
            }

            winLossRatio = (double)winningTrades / Math.Max((losingTrades + winningTrades), 1.0);

            executedOrders = 0;
            tradedLots = 0;
            for (int ord = 0; ord < totalOrders; ord++)
            {
                if (OrdFromNumb(ord).OrdStatus == OrderStatus.Executed)
                {
                    executedOrders++;
                    tradedLots += OrdFromNumb(ord).OrdLots;
                }
            }

            testedDays = (Time[Bars - 1] - Time[FirstBar]).Days;
            if (testedDays < 1)
                testedDays = 1;

            if (Configs.AccountInMoney)
                GenerateAccountStatsInMoney();
            else
                GenerateAccountStats();

            if (Configs.AdditionalStatistics)
            {
                CalculateAdditionalStats();

                if (Configs.AccountInMoney)
                    SetAdditioanlMoneyStats();
                else
                    SetAdditioanlStats();
            }

            return;
        }

        /// <summary>
        /// Generate the Account Statistics in currency.
        /// </summary>
        static void GenerateAccountStatsInMoney()
        {
            accountStatsParam = new string[28]
            {
                Language.T("Intrabar scanning"),
                Language.T("Interpolation method"),
                Language.T("Ambiguous bars"),
                Language.T("Profit per day"),
                Language.T("Tested bars"),
                Language.T("Initial account"),
                Language.T("Account balance"),
                Language.T("Minimum account"),
                Language.T("Maximum account"),
                Language.T("Maximum drawdown"),
                Language.T("Max equity drawdown"),
                Language.T("Max equity drawdown"),
                Language.T("Gross profit"),
                Language.T("Gross loss"),
                Language.T("Sent orders"),
                Language.T("Executed orders"),
                Language.T("Traded lots"),
                Language.T("Winning trades"),
                Language.T("Losing trades"),
                Language.T("Win/loss ratio"),
                Language.T("Time in position"),
                Language.T("Charged spread"),
                Language.T("Charged rollover"),
                Language.T("Charged commission"),
                Language.T("Charged slippage"),
                Language.T("Total charges"),
                Language.T("Balance without charges"),
                Language.T("Account exchange rate")
            };

            string unit = " " + Configs.AccountCurrency;

            accountStatsValue = new string[28];
            accountStatsValue[0]  = isScanned ? Language.T("Accomplished") : Language.T("Not accomplished");
            accountStatsValue[1]  = InterpolationMethodShortToString();
            accountStatsValue[2]  = ambiguousBars.ToString();
            accountStatsValue[3]  = MoneyProfitPerDay.ToString("F2") + unit;
            accountStatsValue[4]  = (Bars - FirstBar).ToString();
            accountStatsValue[5]  = Configs.InitialAccount.ToString("F2") + unit;
            accountStatsValue[6]  = NetMoneyBalance.ToString("F2")  + unit;
            accountStatsValue[7]  = MinMoneyBalance.ToString("F2")  + unit;
            accountStatsValue[8]  = MaxMoneyBalance.ToString("F2")  + unit;
            accountStatsValue[9]  = MaxMoneyDrawdown.ToString("F2") + unit;
            accountStatsValue[10] = MaxMoneyEquityDrawdown.ToString("F2") + unit;
            accountStatsValue[11] = MoneyEquityPercentDrawdown.ToString("F2") + " %";
            accountStatsValue[12] = GrossMoneyProfit.ToString("F2") + unit;
            accountStatsValue[13] = GrossMoneyLoss.ToString("F2")   + unit;
            accountStatsValue[14] = SentOrders.ToString();
            accountStatsValue[15] = ExecutedOrders.ToString();
            accountStatsValue[16] = TradedLots.ToString();
            accountStatsValue[17] = WinningTrades.ToString();
            accountStatsValue[18] = LosingTrades.ToString();
            accountStatsValue[19] = WinLossRatio.ToString("F2");
            accountStatsValue[20] = TimeInPosition.ToString() + " %";
            accountStatsValue[21] = TotalChargedMoneySpread.ToString("F2")     + unit;
            accountStatsValue[22] = TotalChargedMoneyRollOver.ToString("F2")   + unit;
            accountStatsValue[23] = TotalChargedMoneyCommission.ToString("F2") + unit;
            accountStatsValue[24] = TotalChargedMoneySlippage.ToString("F2")   + unit;
            accountStatsValue[25] = (TotalChargedMoneySpread + TotalChargedMoneyRollOver + TotalChargedMoneyCommission + TotalChargedMoneySlippage).ToString("F2") + unit;
            accountStatsValue[26] = (NetMoneyBalance + TotalChargedMoneySpread + TotalChargedMoneyRollOver + TotalChargedMoneyCommission + TotalChargedMoneySlippage).ToString("F2") + unit;

            if (InstrProperties.PriceIn == Configs.AccountCurrency)
                accountStatsValue[27] = Language.T("Not used");
            else if (InstrProperties.InstrType == Instrumet_Type.Forex && Symbol.StartsWith(Configs.AccountCurrency))
                accountStatsValue[27] = Language.T("Deal price");
            else if (Configs.AccountCurrency == "USD")
                accountStatsValue[27] = InstrProperties.RateToUSD.ToString("F4");
            else if (Configs.AccountCurrency == "EUR")
                accountStatsValue[27] = InstrProperties.RateToEUR.ToString("F4");

            accountStatsFlag = new bool[28];
            accountStatsFlag[0] = ambiguousBars > 0 && !isScanned;
            accountStatsFlag[1] = interpolationMethod != InterpolationMethod.Pessimistic;
            accountStatsFlag[2] = ambiguousBars > 0;
            accountStatsFlag[6] = NetMoneyBalance < Configs.InitialAccount;
            accountStatsFlag[9] = MaxDrawdown > Configs.InitialAccount / 2;

            return;
        }

        /// <summary>
        /// Generate the Account Statistics in pips.
        /// </summary>
        static void GenerateAccountStats()
        {
            accountStatsParam = new string[26]
            {
                Language.T("Intrabar scanning"),
                Language.T("Interpolation method"),
                Language.T("Ambiguous bars"),
                Language.T("Profit per day"),
                Language.T("Tested bars"),
                Language.T("Account balance"),
                Language.T("Minimum account"),
                Language.T("Maximum account"),
                Language.T("Maximum drawdown"),
                Language.T("Max equty drawdown"),
                Language.T("Max equty drawdown"),
                Language.T("Gross profit"),
                Language.T("Gross loss"),
                Language.T("Sent orders"),
                Language.T("Executed orders"),
                Language.T("Traded lots"),
                Language.T("Winning trades"),
                Language.T("Losing trades"),
                Language.T("Win/loss ratio"),
                Language.T("Time in position"),
                Language.T("Charged spread"),
                Language.T("Charged rollover"),
                Language.T("Charged commission"),
                Language.T("Charged slippage"),
                Language.T("Total charges"),
                Language.T("Balance without charges")
            };

            string unit = " " + Language.T("pips");
            accountStatsValue = new string[26];
            accountStatsValue[0]  = isScanned ? Language.T("Accomplished") : Language.T("Not accomplished");
            accountStatsValue[1]  = InterpolationMethodShortToString();
            accountStatsValue[2]  = ambiguousBars.ToString();
            accountStatsValue[3]  = ProfitPerDay.ToString() + unit;
            accountStatsValue[4]  = (Bars - FirstBar).ToString();
            accountStatsValue[5]  = NetBalance.ToString()  + unit;
            accountStatsValue[6]  = MinBalance.ToString()  + unit;
            accountStatsValue[7]  = MaxBalance.ToString()  + unit;
            accountStatsValue[8]  = MaxDrawdown.ToString() + unit;
            accountStatsValue[9]  = MaxEquityDrawdown.ToString() + unit;
            accountStatsValue[10] = EquityPercentDrawdown.ToString("F2") + " %";
            accountStatsValue[11] = GrossProfit.ToString() + unit;
            accountStatsValue[12] = GrossLoss.ToString()   + unit;
            accountStatsValue[13] = SentOrders.ToString();
            accountStatsValue[14] = ExecutedOrders.ToString();
            accountStatsValue[15] = TradedLots.ToString();
            accountStatsValue[16] = winningTrades.ToString();
            accountStatsValue[17] = losingTrades.ToString();
            accountStatsValue[18] = ((float)winningTrades/(winningTrades + losingTrades)).ToString("F2");
            accountStatsValue[19] = TimeInPosition.ToString() + " %";
            accountStatsValue[20] = Math.Round(TotalChargedSpread).ToString()     + unit;
            accountStatsValue[21] = Math.Round(TotalChargedRollOver).ToString()   + unit;
            accountStatsValue[22] = Math.Round(TotalChargedCommission).ToString() + unit;
            accountStatsValue[23] = TotalChargedSlippage.ToString()   + unit;
            accountStatsValue[24] = Math.Round(TotalChargedSpread + TotalChargedRollOver + TotalChargedSlippage).ToString() + unit;
            accountStatsValue[25] = Math.Round(NetBalance + TotalChargedSpread + TotalChargedRollOver + TotalChargedSlippage).ToString() + unit;

            accountStatsFlag = new bool[26];
            accountStatsFlag[0] = ambiguousBars > 0 && !isScanned;
            accountStatsFlag[1] = interpolationMethod != InterpolationMethod.Pessimistic;
            accountStatsFlag[2] = ambiguousBars > 0;
            accountStatsFlag[5] = NetBalance < 0;
            accountStatsFlag[8] = MaxDrawdown > 500;

            return;
        }

        /// <summary>
        /// Calculates the required margin.
        /// </summary>
        public static double RequiredMargin(double lots, int bar)
        {
            double amount = lots * InstrProperties.LotSize;
            double exchangeRate = Close[bar] / AccountExchangeRate(Close[bar]);
            double requiredMargin = amount * exchangeRate / Configs.Leverage;

            return requiredMargin;
        }

        /// <summary>
        /// Calculates the trading size in normalized lots.
        /// </summary>
        public static double TradingSize(double size, int bar)
        {
            if (Strategy.UseAccountPercentEntry)
            {
                double maxMargin = session[bar].Summary.MoneyEquity * size / 100.0;
                double exchangeRate = Close[bar] / AccountExchangeRate(Close[bar]);
                size = maxMargin * Configs.Leverage / (exchangeRate * InstrProperties.LotSize);
            }

            size = NormalizeEntryLots(size);

            return size;
        }

        /// <summary>
        /// Normalizes an entry order's size.
        /// <summary>
        static double NormalizeEntryLots(double lots)
        {
            double minlot  = 0.01;
            double maxlot  = Strategy.MaxOpenLots;
            double lotstep = 0.01;

            if (lots <= 0)
                return (0);

            int steps = (int)Math.Round((lots - minlot) / lotstep);
            lots = minlot + steps * lotstep;

            if (lots <= minlot)
                return (minlot);

            if (lots >= maxlot)
                return (maxlot);

            return lots;
        }

        /// <summary>
        /// Account Exchange Rate.
        /// </summary>
        public static double AccountExchangeRate(double price)
        {
            double exchangeRate = 0;

            if (InstrProperties.PriceIn == Configs.AccountCurrency)
                exchangeRate = 1;
            else if (InstrProperties.InstrType == Instrumet_Type.Forex && Symbol.StartsWith(Configs.AccountCurrency))
                exchangeRate = price;
            else if (Configs.AccountCurrency == "USD")
                exchangeRate = InstrProperties.RateToUSD;
            else if (Configs.AccountCurrency == "EUR")
                exchangeRate = InstrProperties.RateToEUR;

            return exchangeRate;
        }

        /// <summary>
        /// Calculates the commission in pips.
        /// </summary>
        public static double Commission(double lots, double price, bool isPosClosing)
        {
            double commission = 0;

            if (InstrProperties.Commission == 0)
                return 0;

            if (InstrProperties.CommissionTime == Commission_Time.open && isPosClosing)
                return 0; // Commission is not applied to the position closing

            if (InstrProperties.CommissionType == Commission_Type.pips)
                commission = InstrProperties.Commission;

            else if (InstrProperties.CommissionType == Commission_Type.percents)
            {
                commission = (price / InstrProperties.Point) * (InstrProperties.Commission / 100);
                return commission; 
            }

            else if (InstrProperties.CommissionType == Commission_Type.money)
                commission = InstrProperties.Commission / (InstrProperties.Point * InstrProperties.LotSize);

            if (InstrProperties.CommissionScope == Commission_Scope.lot)
                commission *= lots; // Commission per lot

            return commission; 
        }

        /// <summary>
        /// Calculates the commission in currency.
        /// </summary>
        public static double CommissionInMoney(double lots, double price, bool isPosClosing)
        {
            double commission = 0;

            if (InstrProperties.Commission == 0)
                return 0;

            if (InstrProperties.CommissionTime == Commission_Time.open && isPosClosing)
                return 0; // Commission is not applied to the position closing

            if (InstrProperties.CommissionType == Commission_Type.pips)
                commission = InstrProperties.Commission * InstrProperties.Point * InstrProperties.LotSize / AccountExchangeRate(price);

            else if (InstrProperties.CommissionType == Commission_Type.percents)
            {
                commission = lots * InstrProperties.LotSize * price * (InstrProperties.Commission / 100) / AccountExchangeRate(price);
                return commission; 
            }

            else if (InstrProperties.CommissionType == Commission_Type.money)
                commission = InstrProperties.Commission / AccountExchangeRate(price);

            if (InstrProperties.CommissionScope == Commission_Scope.lot)
                commission *= lots; // Commission per lot

            return commission; 
        }

        /// <summary>
        /// Calculates the rollover fee in currency.
        /// </summary>
        public static double RolloverInMoney(PosDirection posDir, double lots, int daysRollover, double price)
        {
            double point   = Data.InstrProperties.Point;
            int    lotSize = Data.InstrProperties.LotSize;
            double swapLongPips  = 0; // Swap long in pips
            double swapShortPips = 0; // Swap short in pips
            if (Data.InstrProperties.SwapType == Commission_Type.pips)
            {
                swapLongPips  = Data.InstrProperties.SwapLong;
                swapShortPips = Data.InstrProperties.SwapShort;
            }
            else if (Data.InstrProperties.SwapType == Commission_Type.percents)
            {
                swapLongPips  = (price / point) * (0.01 * Data.InstrProperties.SwapLong / 365);
                swapShortPips = (price / point) * (0.01 * Data.InstrProperties.SwapShort / 365);
            }
            else if (Data.InstrProperties.SwapType == Commission_Type.money)
            {
                swapLongPips  = Data.InstrProperties.SwapLong  / (point * lotSize);
                swapShortPips = Data.InstrProperties.SwapShort / (point * lotSize);
            }

            double rollover = lots * lotSize * (posDir == PosDirection.Long ? swapLongPips : -swapShortPips) * point * daysRollover / Backtester.AccountExchangeRate(price);

            return rollover; 
        }

        /// <summary>
        /// Converts pips to money.
        /// </summary>
        public static double PipsToMoney(double pips, int bar)
        {
            return pips * InstrProperties.Point * InstrProperties.LotSize / AccountExchangeRate(Close[bar]);
        }

        /// <summary>
        /// Converts money to pips.
        /// </summary>
        public static double MoneyToPips(double money, int bar)
        {
            return money * AccountExchangeRate(Close[bar]) / (InstrProperties.Point * InstrProperties.LotSize);
        }
	}
}