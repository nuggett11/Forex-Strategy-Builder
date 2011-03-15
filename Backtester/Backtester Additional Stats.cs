// Backtester - Additional Statistics
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Backtester - Additional statistics
    /// </summary>
    public partial class Backtester : Data
    {
        static double[] longBalance;
        static double[] shortBalance;
        static double[] longMoneyBalance;
        static double[] shortMoneyBalance;
        static double maxLongMoneyBalance;
        static double minLongMoneyBalance;
        static double maxShortMoneyBalance;
        static double minShortMoneyBalance;
        static double maxLongBalance;
        static double minLongBalance;
        static double maxShortBalance;
        static double minShortBalance;

        static DateTime maxLongBalanceDate;
        static DateTime minLongBalanceDate;
        static DateTime maxShortBalanceDate;
        static DateTime minShortBalanceDate;
        static DateTime maxLongMoneyBalanceDate;
        static DateTime minLongMoneyBalanceDate;
        static DateTime maxShortMoneyBalanceDate;
        static DateTime minShortMoneyBalanceDate;

        static double grossLongProfit;
        static double grossLongLoss;
        static double grossShortProfit;
        static double grossShortLoss;
        static double grossLongMoneyProfit;
        static double grossLongMoneyLoss;
        static double grossShortMoneyProfit;
        static double grossShortMoneyLoss;

        static string[] additionalStatsParamName  = new string[0];
        static string[] additionalStatsValueTotal = new string[0];
        static string[] additionalStatsValueLong  = new string[0];
        static string[] additionalStatsValueShort = new string[0];

        static double maxLongDrawdown;
        static double maxShortDrawdown;
        static double maxLongMoneyDrawdown;
        static double maxShortMoneyDrawdown;
        static double maxLongMoneyDrawdownPercent;
        static double maxShortMoneyDrawdownPercent;
        static DateTime maxLongDrawdownDate;
        static DateTime maxShortDrawdownDate;
        static DateTime maxLongMoneyDrawdownDate;
        static DateTime maxShortMoneyDrawdownDate;

        static int barsWithLongPos;
        static int barsWithShortPos;
        static int barsWithPos;

        static int winningLongTrades;
        static int winningShortTrades;
        static int losingLongTrades;
        static int losingShortTrades;
        static int totalLongTrades;
        static int totalShortTrades;

        static double maxLongWin;
        static double maxShortWin;
        static double maxLongMoneyWin;
        static double maxShortMoneyWin;
        static double maxLongLoss;
        static double maxShortLoss;
        static double maxLongMoneyLoss;
        static double maxShortMoneyLoss;

        static double AHPR;
        static double AHPRLong;
        static double AHPRShort;
        static double GHPR;
        static double GHPRLong;
        static double GHPRShort;
        static double sharpeRatio;
        static double sharpeRatioLong;
        static double sharpeRatioShort;

        /// <summary>
        /// Gets the Additional Stats Param Name.
        /// </summary>
        public static string[] AdditionalStatsParamName { get { return additionalStatsParamName; } }

        /// <summary>
        /// Gets the Additional Stats Value Long + Short.
        /// </summary>
        public static string[] AdditionalStatsValueTotal { get { return additionalStatsValueTotal; } }

        /// <summary>
        /// Gets the Additional Stats Value Long.
        /// </summary>
        public static string[] AdditionalStatsValueLong { get { return additionalStatsValueLong; } }

        /// <summary>
        /// Gets the Additional Stats Value Short.
        /// </summary>
        public static string[] AdditionalStatsValueShort { get { return additionalStatsValueShort; } }

        /// <summary>
        /// Gets the long balance in pips.
        /// </summary>
        public static int NetLongBalance { get { return (int)Math.Round(longBalance[Bars - 1]); } }

        /// <summary>
        /// Gets the short balance in pips.
        /// </summary>
        public static int NetShortBalance { get { return (int)Math.Round(shortBalance[Bars - 1]); } }

        /// <summary>
        /// Gets the max long balance in pips.
        /// </summary>
        public static int MaxLongBalance { get { return (int)Math.Round(maxLongBalance); } }

        /// <summary>
        /// Gets the max short balance in pips.
        /// </summary>
        public static int MaxShortBalance { get { return (int)Math.Round(maxShortBalance); } }

        /// <summary>
        /// Gets the min long balance in pips.
        /// </summary>
        public static int MinLongBalance { get { return (int)Math.Round(minLongBalance); } }

        /// <summary>
        /// Gets the min short balance in pips.
        /// </summary>
        public static int MinShortBalance { get { return (int)Math.Round(minShortBalance); } }

        /// <summary>
        /// Gets the long balance in money
        /// </summary>
        public static double NetLongMoneyBalance { get { return longMoneyBalance[Bars - 1]; } }

        /// <summary>
        /// Gets the short balance in money.
        /// </summary>
        public static double NetShortMoneyBalance { get { return shortMoneyBalance[Bars - 1]; } }

        /// <summary>
        /// Gets the max long balance in money.
        /// </summary>
        public static double MaxLongMoneyBalance { get { return maxLongMoneyBalance; } }

        /// <summary>
        /// Gets the max short balance in money.
        /// </summary>
        public static double MaxShortMoneyBalance { get { return maxShortMoneyBalance; } }

        /// <summary>
        /// Gets the min long balance in money.
        /// </summary>
        public static double MinLongMoneyBalance { get { return minLongMoneyBalance; } }

        /// <summary>
        /// Gets the min short balance in money.
        /// </summary>
        public static double MinShortMoneyBalance { get { return minShortMoneyBalance; } }

        /// <summary>
        /// Returns the long balance at the end of the bar in pips.
        /// </summary>
        public static int LongBalance(int bar)
        {
            return (int)Math.Round(longBalance[bar]);
        }

        /// <summary>
        /// Returns the short balance at the end of the bar in pips.
        /// </summary>
        public static int ShortBalance(int bar)
        {
            return (int)Math.Round(shortBalance[bar]);
        }

        /// <summary>
        /// Returns the long balance at the end of the bar in money.
        /// </summary>
        public static double LongMoneyBalance(int bar)
        {
            return longMoneyBalance[bar];
        }

        /// <summary>
        /// Returns the short balance at the end of the bar in money.
        /// </summary>
        public static double ShortMoneyBalance(int bar)
        {
            return shortMoneyBalance[bar];
        }

        /// <summary>
        /// Calculates the values of the stats parameters.
        /// </summary>
        static void CalculateAdditionalStats()
        {
            longBalance       = new double[Bars];
            shortBalance      = new double[Bars];
            longMoneyBalance  = new double[Bars];
            shortMoneyBalance = new double[Bars];

            maxLongMoneyBalance  = Configs.InitialAccount;
            minLongMoneyBalance  = Configs.InitialAccount;
            maxShortMoneyBalance = Configs.InitialAccount;
            minShortMoneyBalance = Configs.InitialAccount;
            maxLongBalance  = 0;
            minLongBalance  = 0;
            maxShortBalance = 0;
            minShortBalance = 0;

            maxLongBalanceDate        = Time[0];
            minLongBalanceDate        = Time[0];
            maxShortBalanceDate       = Time[0];
            minShortBalanceDate       = Time[0];
            maxLongMoneyBalanceDate   = Time[0];
            minLongMoneyBalanceDate   = Time[0];
            maxShortMoneyBalanceDate  = Time[0];
            minShortMoneyBalanceDate  = Time[0];
            maxLongDrawdownDate       = Time[0];
            maxShortDrawdownDate      = Time[0];
            maxLongMoneyDrawdownDate  = Time[0];
            maxShortMoneyDrawdownDate = Time[0];

            grossLongProfit       = 0;
            grossLongLoss         = 0;
            grossShortProfit      = 0;
            grossShortLoss        = 0;
            grossLongMoneyProfit  = 0;
            grossLongMoneyLoss    = 0;
            grossShortMoneyProfit = 0;
            grossShortMoneyLoss   = 0;

            maxLongDrawdown       = 0;
            maxShortDrawdown      = 0;
            maxLongMoneyDrawdown  = 0;
            maxShortMoneyDrawdown = 0;
            maxShortDrawdown      = 0;
            maxLongMoneyDrawdown  = 0;
            maxShortMoneyDrawdown = 0;
            maxLongMoneyDrawdownPercent  = 0;
            maxShortMoneyDrawdownPercent = 0;

            barsWithPos        = 0;
            barsWithLongPos    = 0;
            barsWithShortPos   = 0;

            winningLongTrades  = 0;
            winningShortTrades = 0;
            losingLongTrades   = 0;
            losingShortTrades  = 0;

            totalLongTrades   = 0;
            totalShortTrades  = 0;

            maxLongWin        = 0;
            maxShortWin       = 0;
            maxLongMoneyWin   = 0;
            maxShortMoneyWin  = 0;
            maxLongLoss       = 0;
            maxShortLoss      = 0;
            maxLongMoneyLoss  = 0;
            maxShortMoneyLoss = 0;

            for (int bar = 0; bar < FirstBar; bar++)
            {
                longBalance[bar]  = 0;
                shortBalance[bar] = 0;
                longMoneyBalance[bar]  = Configs.InitialAccount;
                shortMoneyBalance[bar] = Configs.InitialAccount;
            }

            for (int bar = Data.FirstBar; bar < Bars; bar++)
            {
                double accountExchangeRate = AccountExchangeRate(Close[bar]);
                double pipsToMoney = InstrProperties.Point * InstrProperties.LotSize / accountExchangeRate;

                longBalance[bar]  = longBalance[bar - 1];
                shortBalance[bar] = shortBalance[bar - 1];
                longMoneyBalance[bar]  = longMoneyBalance[bar - 1];
                shortMoneyBalance[bar] = shortMoneyBalance[bar - 1];

                bool isLong  = false;
                bool isShort = false;
                for (int pos = 0; pos < Positions(bar); pos++)
                {
                    if (PosDir(bar, pos) == PosDirection.Long)
                        isLong = true;

                    if (PosDir(bar, pos) == PosDirection.Short)
                        isShort = true;

                    double positionProfitLoss      = PosProfitLoss(bar, pos);
                    double positionMoneyProfitLoss = PosMoneyProfitLoss(bar, pos);

                    if (PosTransaction(bar, pos) == Transaction.Close  ||
                        PosTransaction(bar, pos) == Transaction.Reduce ||
                        PosTransaction(bar, pos) == Transaction.Reverse)
                    {
                        if (OrdFromNumb(PosOrdNumb(bar, pos)).OrdDir == OrderDirection.Sell)
                        {   // Closing long position
                            longBalance[bar]      += positionProfitLoss;
                            longMoneyBalance[bar] += positionMoneyProfitLoss;

                            if (positionProfitLoss > 0)
                            {
                                grossLongProfit      += positionProfitLoss;
                                grossLongMoneyProfit += positionMoneyProfitLoss;
                                winningLongTrades++;
                                if (positionProfitLoss > maxLongWin)
                                    maxLongWin = positionProfitLoss;
                                if (positionMoneyProfitLoss > maxLongMoneyWin)
                                    maxLongMoneyWin = positionMoneyProfitLoss;
                            }
                            if (positionProfitLoss < 0)
                            {
                                grossLongLoss      += positionProfitLoss;
                                grossLongMoneyLoss += positionMoneyProfitLoss;
                                losingLongTrades++;
                                if (positionProfitLoss < maxLongLoss)
                                    maxLongLoss = positionProfitLoss;
                                if (positionMoneyProfitLoss < maxLongMoneyLoss)
                                    maxLongMoneyLoss = positionMoneyProfitLoss;
                            }

                            totalLongTrades++;
                        }
                        if (OrdFromNumb(PosOrdNumb(bar, pos)).OrdDir == OrderDirection.Buy)
                        {   // Closing short position
                            shortBalance[bar]      += positionProfitLoss;
                            shortMoneyBalance[bar] += positionMoneyProfitLoss;

                            if (positionProfitLoss > 0)
                            {
                                grossShortProfit      += positionProfitLoss;
                                grossShortMoneyProfit += positionMoneyProfitLoss;
                                winningShortTrades++;
                                if (positionProfitLoss > maxShortWin)
                                    maxShortWin = positionProfitLoss;
                                if (positionMoneyProfitLoss > maxShortMoneyWin)
                                    maxShortMoneyWin = positionMoneyProfitLoss;
                            }
                            if (positionProfitLoss < 0)
                            {
                                grossShortLoss      += positionProfitLoss;
                                grossShortMoneyLoss += positionMoneyProfitLoss;
                                losingShortTrades++;
                                if (positionProfitLoss < maxShortLoss)
                                    maxShortLoss = positionProfitLoss;
                                if (positionMoneyProfitLoss < maxShortMoneyLoss)
                                    maxShortMoneyLoss = positionMoneyProfitLoss;
                            }

                            totalShortTrades++;
                        }
                    }
                }

                barsWithPos      += (isLong || isShort) ? 1 : 0;
                barsWithLongPos  += isLong  ? 1 : 0;
                barsWithShortPos += isShort ? 1 : 0;

                if (maxLongBalance < longBalance[bar])
                {
                    maxLongBalance = longBalance[bar];
                    maxLongBalanceDate = Time[bar];
                }
                if (minLongBalance > longBalance[bar])
                {
                    minLongBalance = longBalance[bar];
                    minLongBalanceDate = Time[bar];
                }
                if (maxShortBalance < shortBalance[bar])
                {
                    maxShortBalance = shortBalance[bar];
                    maxShortBalanceDate = Time[bar];
                }
                if (minShortBalance > shortBalance[bar])
                {
                    minShortBalance = shortBalance[bar];
                    minShortBalanceDate = Time[bar];
                }
                if (maxLongMoneyBalance < longMoneyBalance[bar])
                {
                    maxLongMoneyBalance = longMoneyBalance[bar];
                    maxLongMoneyBalanceDate = Time[bar];
                }
                if (minLongMoneyBalance > longMoneyBalance[bar])
                {
                    minLongMoneyBalance = longMoneyBalance[bar];
                    minLongMoneyBalanceDate = Time[bar];
                }
                if (maxShortMoneyBalance < shortMoneyBalance[bar])
                {
                    maxShortMoneyBalance = shortMoneyBalance[bar];
                    maxShortMoneyBalanceDate = Time[bar];
                }
                if (minShortMoneyBalance > shortMoneyBalance[bar])
                {
                    minShortMoneyBalance = shortMoneyBalance[bar];
                    minShortMoneyBalanceDate = Time[bar];
                }

                // Maximum Drawdown
                if (maxLongBalance - longBalance[bar] > maxLongDrawdown)
                {
                    maxLongDrawdown = maxLongBalance - longBalance[bar];
                    maxLongDrawdownDate = Time[bar];
                }

                if (maxLongMoneyBalance - longMoneyBalance[bar] > maxLongMoneyDrawdown)
                {
                    maxLongMoneyDrawdown = maxLongMoneyBalance - longMoneyBalance[bar];
                    maxLongMoneyDrawdownPercent = 100 * maxLongMoneyDrawdown / maxLongMoneyBalance;
                    maxLongMoneyDrawdownDate   = Time[bar];
                }

                if (maxShortBalance - shortBalance[bar] > maxShortDrawdown)
                {
                    maxShortDrawdown = maxShortBalance - shortBalance[bar];
                    maxShortDrawdownDate = Time[bar];
                }

                if (maxShortMoneyBalance - shortMoneyBalance[bar] > maxShortMoneyDrawdown)
                {
                    maxShortMoneyDrawdown = maxShortMoneyBalance - shortMoneyBalance[bar];
                    maxShortMoneyDrawdownPercent = 100 * maxShortMoneyDrawdown / maxShortMoneyBalance;
                    maxShortMoneyDrawdownDate = Time[bar];
                }
            }

            // Holding period returns
            AHPR      = 0;
            AHPRLong  = 0;
            AHPRShort = 0;

            double[] HPR      = new double[totalTrades];
            double[] HPRLong  = new double[totalLongTrades];
            double[] HPRShort = new double[totalShortTrades];

            double totalHPR      = 0;
            double totalHPRLong  = 0;
            double totalHPRShort = 0;

            double startBalance      = Configs.InitialAccount;
            double startBalanceLong  = Configs.InitialAccount;
            double startBalanceShort = Configs.InitialAccount;

            int count  = 0;
            int countL = 0;
            int countS = 0;

            for (int pos = 0; pos < PositionsTotal; pos++)
            {   // Charged fees
                Position position = Backtester.PosFromNumb(pos);
                // Winning losing trades.
                if (position.Transaction == Transaction.Close  ||
                    position.Transaction == Transaction.Reduce ||
                    position.Transaction == Transaction.Reverse)
                {
                    if (OrdFromNumb(position.FormOrdNumb).OrdDir == OrderDirection.Sell)
                    {  // Closing long position
                        HPRLong[countL] = 1 + position.MoneyProfitLoss / startBalanceLong;
                        totalHPRLong += HPRLong[countL];
                        countL++;
                        startBalanceLong += position.MoneyProfitLoss;
                    }
                    if (OrdFromNumb(position.FormOrdNumb).OrdDir == OrderDirection.Buy)
                    {  // Closing short position
                        HPRShort[countS] = 1 + position.MoneyProfitLoss / startBalanceShort;
                        totalHPRShort += HPRShort[countS];
                        countS++;
                        startBalanceShort += position.MoneyProfitLoss;
                    }
                    HPR[count] = 1 + position.MoneyProfitLoss / startBalance;
                    totalHPR += HPR[count];
                    count++;
                    startBalance += position.MoneyProfitLoss;
                }
            }

            double averageHPR      = totalHPR      / totalTrades;
            double averageHPRLong  = totalHPRLong  / totalLongTrades;
            double averageHPRShort = totalHPRShort / totalShortTrades;

            AHPR      = 100 * (averageHPR      - 1);
            AHPRLong  = 100 * (averageHPRLong  - 1);
            AHPRShort = 100 * (averageHPRShort - 1);

            GHPR      = 100 * (Math.Pow((NetMoneyBalance      / Configs.InitialAccount), (1f / totalTrades))      - 1);
            GHPRLong  = 100 * (Math.Pow((NetLongMoneyBalance  / Configs.InitialAccount), (1f / totalLongTrades))  - 1);
            GHPRShort = 100 * (Math.Pow((NetShortMoneyBalance / Configs.InitialAccount), (1f / totalShortTrades)) - 1);

            // Sharpe Ratio
            sharpeRatio      = 0;
            sharpeRatioLong  = 0;
            sharpeRatioShort = 0;

            double sumPow      = 0;
            double sumPowLong  = 0;
            double sumPowShort = 0;

            for (int i = 0; i < totalTrades; i++)
                sumPow += Math.Pow((HPR[i] - averageHPR), 2);
            for (int i = 0; i < totalLongTrades; i++)
                sumPowLong += Math.Pow((HPRLong[i] - averageHPRLong), 2);
            for (int i = 0; i < totalShortTrades; i++)
                sumPowShort += Math.Pow((HPRShort[i] - averageHPRShort), 2);

            double stDev      = Math.Sqrt(sumPow      / (totalTrades      - 1));
            double stDevLong  = Math.Sqrt(sumPowLong  / (totalLongTrades  - 1));
            double stDevShort = Math.Sqrt(sumPowShort / (totalShortTrades - 1));

            sharpeRatio      = (averageHPR      - 1) / stDev;
            sharpeRatioLong  = (averageHPRLong  - 1) / stDevLong;
            sharpeRatioShort = (averageHPRShort - 1) / stDevShort;
        }

        /// <summary>
        /// Sets the additional stats in pips.
        /// </summary>
        static void SetAdditioanlStats()
        {
            string unit = " " + Language.T("pips");

            additionalStatsParamName = new string[]
            {
                Language.T("Initial account"),
                Language.T("Account balance"),
                Language.T("Net profit"),
                Language.T("Gross profit"),
                Language.T("Gross loss"),
                Language.T("Profit factor"),
                Language.T("Annualized profit"),
                Language.T("Minimum account"),
                Language.T("Minimum account date"),
                Language.T("Maximum account"),
                Language.T("Maximum account date"),
                Language.T("Absolute drawdown"),
                Language.T("Maximum drawdown"),
                Language.T("Maximum drawdown date"),
                Language.T("Historical bars"),
                Language.T("Tested bars"),
                Language.T("Bars with trades"),
                Language.T("Bars with trades") + " %",
                Language.T("Number of trades"),
                Language.T("Winning trades"),
                Language.T("Losing trades"),
                Language.T("Win/loss ratio"),
                Language.T("Maximum profit"),
                Language.T("Average profit"),
                Language.T("Maximum loss"),
                Language.T("Average loss"),
                Language.T("Expected payoff")
            };

            int totalWinTrades  = winningLongTrades + winningShortTrades;
            int totalLossTrades = losingLongTrades  + losingShortTrades;
            int totalTrades     = totalWinTrades    + totalLossTrades;

            additionalStatsValueTotal = new string[]
            {
                "0" + unit,
                NetBalance.ToString() + unit,
                NetBalance.ToString() + unit,
                Math.Round(grossProfit).ToString() + unit,
                Math.Round(grossLoss).ToString()   + unit,
                (grossLoss == 0 ? "N/A" : Math.Abs(grossProfit / grossLoss).ToString("F2")),
                Math.Round(((365f / Data.Time[Data.Bars-1].Subtract(Data.Time[0]).Days) * NetBalance)).ToString() + unit,
                MinBalance.ToString() + unit,
                minBalanceDate.ToShortDateString(),
                MaxBalance.ToString() + unit,
                maxBalanceDate.ToShortDateString(),
                Math.Abs(MinBalance).ToString()    + unit,
                Math.Round(maxDrawdown).ToString() + unit,
                maxDrawdownDate.ToShortDateString(),
                Data.Bars.ToString(),
                (Data.Bars - Data.FirstBar).ToString(),
                barsWithPos.ToString(),
                (100f * barsWithPos /(Data.Bars - Data.FirstBar)).ToString("F2") + "%",
                totalTrades.ToString(),
                totalWinTrades.ToString(),
                totalLossTrades.ToString(),
                (1f * totalWinTrades/(totalWinTrades + totalLossTrades)).ToString("F2"),
                Math.Round(Math.Max(maxLongWin, maxShortWin)).ToString()   + unit,
                Math.Round(grossProfit / totalWinTrades).ToString()        + unit,
                Math.Round(Math.Min(maxLongLoss, maxShortLoss)).ToString() + unit,
                Math.Round(grossLoss / totalLossTrades).ToString()         + unit,
                (1f * NetBalance / totalTrades).ToString("F2")             + unit
            };

            additionalStatsValueLong = new string[]
            {
                "0" + unit,
                NetLongBalance.ToString() + unit,
                NetLongBalance.ToString() + unit,
                Math.Round(grossLongProfit).ToString() + unit,
                Math.Round(grossLongLoss).ToString()   + unit,
                (grossLongLoss == 0 ? "N/A" : Math.Abs(grossLongProfit/grossLongLoss).ToString("F2")),
                Math.Round(((365f / Data.Time[Data.Bars-1].Subtract(Data.Time[0]).Days) * NetLongBalance)).ToString() + unit,
                MinLongBalance.ToString() + unit,
                minLongBalanceDate.ToShortDateString(),
                MaxLongBalance.ToString() + unit,
                maxLongBalanceDate.ToShortDateString(),
                Math.Round(Math.Abs(minLongBalance)).ToString() + unit,
                Math.Round(maxLongDrawdown).ToString() + unit,
                maxLongDrawdownDate.ToShortDateString(),
                Data.Bars.ToString(),
                (Data.Bars - Data.FirstBar).ToString(),
                barsWithLongPos.ToString(),
                (100f * barsWithLongPos /(Data.Bars - Data.FirstBar)).ToString("F2") + "%",
                totalLongTrades.ToString(),
                winningLongTrades.ToString(),
                losingLongTrades.ToString(),
                (1f * winningLongTrades /(winningLongTrades + losingLongTrades)).ToString("F2"),
                Math.Round(maxLongWin).ToString() + unit,
                Math.Round(grossLongProfit / winningLongTrades).ToString() + unit,
                Math.Round(maxLongLoss).ToString() + unit,
                Math.Round(grossLongLoss / losingLongTrades).ToString() + unit,
                (1f * NetLongBalance / (winningLongTrades + losingLongTrades)).ToString("F2") + unit
            };

            additionalStatsValueShort = new string[]
            {
                "0" + unit,
                NetShortBalance.ToString() + unit,
                NetShortBalance.ToString() + unit,
                Math.Round(grossShortProfit).ToString() + unit,
                Math.Round(grossShortLoss).ToString()   + unit,
                (grossShortLoss == 0 ? "N/A" : Math.Abs(grossShortProfit/grossShortLoss).ToString("F2")),
                Math.Round(((365f / Data.Time[Data.Bars-1].Subtract(Data.Time[0]).Days) * NetShortBalance)).ToString() + unit,
                MinShortBalance.ToString() + unit,
                minShortBalanceDate.ToShortDateString(),
                MaxShortBalance.ToString() + unit,
                maxShortBalanceDate.ToShortDateString(),
                Math.Round(Math.Abs(minShortBalance)).ToString() + unit,
                Math.Round(maxShortDrawdown).ToString() + unit,
                maxShortDrawdownDate.ToShortDateString(),
                Data.Bars.ToString(),
                (Data.Bars - Data.FirstBar).ToString(),
                barsWithShortPos.ToString(),
                (100f * barsWithShortPos /(Data.Bars - Data.FirstBar)).ToString("F2") + "%",
                totalShortTrades.ToString(),
                winningShortTrades.ToString(),
                losingShortTrades.ToString(),
                (1f * winningShortTrades / (winningShortTrades + losingShortTrades)).ToString("F2"),
                Math.Round(maxShortWin).ToString() + unit,
                Math.Round(grossShortProfit / winningShortTrades).ToString() + unit,
                Math.Round(maxShortLoss).ToString() + unit,
                Math.Round(grossShortLoss / losingShortTrades).ToString() + unit,
                (1f * NetShortBalance / (winningShortTrades + losingShortTrades)).ToString("F2") + unit
            };
        }

        /// <summary>
        /// Sets the additional stats in Money.
        /// </summary>
        static void SetAdditioanlMoneyStats()
        {
            string unit = " " + Configs.AccountCurrency;

            additionalStatsParamName = new string[]
            {
                Language.T("Initial account"),
                Language.T("Account balance"),
                Language.T("Net profit"),
                Language.T("Net profit") + " %",
                Language.T("Gross profit"),
                Language.T("Gross loss"),
                Language.T("Profit factor"),
                Language.T("Annualized profit"),
                Language.T("Annualized profit") + " %",
                Language.T("Minimum account"),
                Language.T("Minimum account date"),
                Language.T("Maximum account"),
                Language.T("Maximum account date"),
                Language.T("Absolute drawdown"),
                Language.T("Maximum drawdown"),
                Language.T("Maximum drawdown") + " %",
                Language.T("Maximum drawdown date"),
                Language.T("Historical bars"),
                Language.T("Tested bars"),
                Language.T("Bars with trades"),
                Language.T("Bars with trades") + " %",
                Language.T("Number of trades"),
                Language.T("Winning trades"),
                Language.T("Losing trades"),
                Language.T("Win/loss ratio"),
                Language.T("Maximum profit"),
                Language.T("Average profit"),
                Language.T("Maximum loss"),
                Language.T("Average loss"),
                Language.T("Expected payoff"),
                Language.T("Average holding period returns"),
                Language.T("Geometric holding period returns"),
                Language.T("Sharpe ratio")
            };

            int totalWinTrades  = winningLongTrades + winningShortTrades;
            int totalLossTrades = losingLongTrades  + losingShortTrades;
            int totalTrades     = totalWinTrades    + totalLossTrades;

            additionalStatsValueTotal = new string[]
            {
                Configs.InitialAccount.ToString("F2") + unit,
                NetMoneyBalance.ToString("F2") + unit,
                (NetMoneyBalance - Configs.InitialAccount).ToString("F2") + unit,
                (100 * ((NetMoneyBalance - Configs.InitialAccount) / Configs.InitialAccount)).ToString("F2") + "%",
                grossMoneyProfit.ToString("F2") + unit,
                grossMoneyLoss.ToString("F2") + unit,
                (grossMoneyLoss == 0 ? "N/A" : Math.Abs(grossMoneyProfit / grossMoneyLoss).ToString("F2")),
                ((365f / Data.Time[Data.Bars-1].Subtract(Data.Time[0]).Days) * (NetMoneyBalance - Configs.InitialAccount)).ToString("F2") + unit,
                (100 * (365f / Data.Time[Data.Bars-1].Subtract(Data.Time[0]).Days) * (NetMoneyBalance - Configs.InitialAccount) / Configs.InitialAccount).ToString("F2") + "%",
                MinMoneyBalance.ToString("F2") + unit,
                minMoneyBalanceDate.ToShortDateString(),
                MaxMoneyBalance.ToString("F2") + unit,
                maxMoneyBalanceDate.ToShortDateString(),
                (Configs.InitialAccount - MinMoneyBalance).ToString("F2") + unit,
                maxMoneyDrawdown.ToString("F2") + unit,
                maxMoneyDrawdownPercent.ToString("F2") + "%",
                maxMoneyDrawdownDate.ToShortDateString(),
                Data.Bars.ToString(),
                (Data.Bars - Data.FirstBar).ToString(),
                barsWithPos.ToString(),
                (100f * barsWithPos /(Data.Bars - Data.FirstBar)).ToString("F2") + "%",
                totalTrades.ToString(),
                totalWinTrades.ToString(),
                totalLossTrades.ToString(),
                (1f * totalWinTrades / (totalWinTrades + totalLossTrades)).ToString("F2"),
                Math.Max(maxLongMoneyWin, maxShortMoneyWin).ToString("F2") + unit,
                (grossMoneyProfit / totalWinTrades).ToString("F2") + unit,
                Math.Min(maxLongMoneyLoss, maxShortMoneyLoss).ToString("F2") + unit,
                (grossMoneyLoss / totalLossTrades).ToString("F2") + unit,
                (1f * (NetMoneyBalance - Configs.InitialAccount) / totalTrades).ToString("F2") + unit,
                AHPR.ToString("F2") + "%",
                GHPR.ToString("F2") + "%",
                sharpeRatio.ToString("F2")
            };

            additionalStatsValueLong = new string[]
            {
                Configs.InitialAccount.ToString("F2") + unit,
                NetLongMoneyBalance.ToString("F2") + unit,
                (NetLongMoneyBalance - Configs.InitialAccount).ToString("F2") + unit,
                (100 * ((NetLongMoneyBalance - Configs.InitialAccount) / Configs.InitialAccount)).ToString("F2") + "%",
                grossLongMoneyProfit.ToString("F2") + unit,
                grossLongMoneyLoss.ToString("F2") + unit,
                (grossLongMoneyLoss == 0 ? "N/A" : Math.Abs(grossLongMoneyProfit/grossLongMoneyLoss).ToString("F2")),
                ((365f / Data.Time[Data.Bars-1].Subtract(Data.Time[0]).Days) * (NetLongMoneyBalance - Configs.InitialAccount)).ToString("F2") + unit,
                (100 * (365f / Data.Time[Data.Bars-1].Subtract(Data.Time[0]).Days) * (NetLongMoneyBalance - Configs.InitialAccount) / Configs.InitialAccount).ToString("F2") + "%",
                MinLongMoneyBalance.ToString("F2") + unit,
                minLongMoneyBalanceDate.ToShortDateString(),
                MaxLongMoneyBalance.ToString("F2") + unit,
                maxLongMoneyBalanceDate.ToShortDateString(),
                (Configs.InitialAccount - MinLongMoneyBalance).ToString("F2") + unit,
                maxLongMoneyDrawdown.ToString("F2") + unit,
                maxLongMoneyDrawdownPercent.ToString("F2") + "%",
                maxLongMoneyDrawdownDate.ToShortDateString(),
                Data.Bars.ToString(),
                (Data.Bars - Data.FirstBar).ToString(),
                barsWithLongPos.ToString(),
                (100f * barsWithLongPos /(Data.Bars - Data.FirstBar)).ToString("F2") + "%",
                totalLongTrades.ToString(),
                winningLongTrades.ToString(),
                losingLongTrades.ToString(),
                (1f * winningLongTrades / (winningLongTrades + losingLongTrades)).ToString("F2"),
                maxLongMoneyWin.ToString("F2") + unit,
                (grossLongMoneyProfit / winningLongTrades).ToString("F2") + unit,
                maxLongMoneyLoss.ToString("F2") + unit,
                (grossLongMoneyLoss / losingLongTrades).ToString("F2") + unit,
                (1f * (NetLongMoneyBalance - Configs.InitialAccount) / (winningLongTrades + losingLongTrades)).ToString("F2") + unit,
                AHPRLong.ToString("F2") + "%",
                GHPRLong.ToString("F2") + "%",
                sharpeRatioLong.ToString("F2")
            };

            additionalStatsValueShort = new string[]
            {
                Configs.InitialAccount.ToString("F2") + unit,
                NetShortMoneyBalance.ToString("F2") + unit,
                (NetShortMoneyBalance - Configs.InitialAccount).ToString("F2") + unit,
                (100 * ((NetShortMoneyBalance - Configs.InitialAccount) / Configs.InitialAccount)).ToString("F2") + "%",
                grossShortMoneyProfit.ToString("F2") + unit,
                grossShortMoneyLoss.ToString("F2") + unit,
                (grossShortMoneyLoss == 0 ? "N/A" : Math.Abs(grossShortMoneyProfit/grossShortMoneyLoss).ToString("F2")),
                ((365f / Data.Time[Data.Bars-1].Subtract(Data.Time[0]).Days) * (NetShortMoneyBalance - Configs.InitialAccount)).ToString("F2") + unit,
                (100 * (365f / Data.Time[Data.Bars-1].Subtract(Data.Time[0]).Days) * (NetShortMoneyBalance - Configs.InitialAccount) / Configs.InitialAccount).ToString("F2") + "%",
                MinShortMoneyBalance.ToString("F2") + unit,
                minShortMoneyBalanceDate.ToShortDateString(),
                MaxShortMoneyBalance.ToString("F2") + unit,
                maxShortMoneyBalanceDate.ToShortDateString(),
                (Configs.InitialAccount - MinShortMoneyBalance).ToString("F2") + unit,
                maxShortMoneyDrawdown.ToString("F2") + unit,
                maxShortMoneyDrawdownPercent.ToString("F2") + "%",
                maxShortMoneyDrawdownDate.ToShortDateString(),
                Data.Bars.ToString(),
                (Data.Bars - Data.FirstBar).ToString(),
                barsWithShortPos.ToString(),
                (100f * barsWithShortPos /(Data.Bars - Data.FirstBar)).ToString("F2") + "%",
                totalShortTrades.ToString(),
                winningShortTrades.ToString(),
                losingShortTrades.ToString(),
                (1f * winningShortTrades / (winningShortTrades + losingShortTrades)).ToString("F2"),
                maxShortMoneyWin.ToString("F2") + unit,
                (grossShortMoneyProfit / winningShortTrades).ToString("F2") + unit,
                maxShortMoneyLoss.ToString("F2") + unit,
                (grossShortMoneyLoss / losingShortTrades).ToString("F2") + unit,
                (1f * (NetShortMoneyBalance - Configs.InitialAccount) / (winningShortTrades + losingShortTrades)).ToString("F2") + unit,
                AHPRShort.ToString("F2") + "%",
                GHPRShort.ToString("F2") + "%",
                sharpeRatioShort.ToString("F2")
           };
        }
    }
}
