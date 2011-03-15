// Position class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// The positions' direction
    /// </summary>
    public enum PosDirection
    {
        Error, None, Long, Short, Closed
    }

    /// <summary>
    /// The type of transaction
    /// </summary>
    public enum Transaction
    {
        Error, None, Open, Close, Add, Reduce, Reverse, Transfer
    }

    /// <summary>
    /// Keeps the coordinates of each position
    /// </summary>
    public struct PositionCoordinates
    {
        int bar;
        int positionNumber;

        /// <summary>
        /// The bar number
        /// </summary>
        public int Bar { get { return bar; } set { bar = value; } }

        /// <summary>
        /// The position number
        /// </summary>
        public int Pos { get { return positionNumber; } set { positionNumber = value; } }
    }

    /// <summary>
    /// Class Position
    /// </summary>
    public class Position
    {
        Transaction  transaction;  // Action applied to the position
        PosDirection posDirection; // Direction
        int    openingBar;         // The bar when the position was open
        int    ordNumb;            // Forming Order number
        int    posNumb;            // Position number in the scope of the bar
        double posLots;            // Number of lots or zero if neutral
        double ordPrice;           // Forming Order price
        double posPrice;           // Calculated middle price including the rollover fee
        double absoluteSL;         // Absolute mode Permanent SL
        double absoluteTP;         // Absolute mode Permanent TP
        double breakEven;          // Break Even
        double requiredMargin;     // The required marging
        double profitLoss;         // Position Profit Loss
        double floatingPL;         // Position Floating Profit Loss
        double balance;            // Account balance at the end of the session
        double equity;             // Account equity at the end of the session
        double chargedSpread;      // Charged spread
        double chargedRollover;    // Charged rollover
        double chargedCommission;  // Charged commission
        double chargedSlippage;    // Charged slippage
        double moneyProfitLoss;    // Position Profit Loss in currency
        double moneyFloatingPL;    // Position Floating Profit Loss in currency
        double moneyBalance;       // Account balance at the end of the session in currency
        double moneyEquity;        // Account equity at the end of the session in currency
        double moneySpread;        // Charged spread in currency
        double moneyRollover;      // Charged rollover in currency
        double moneyCommission;    // Charged slippage in currency
        double moneySlippage;      // Charged slippage in currency

        /// <summary>
        /// The amount of the position
        /// </summary>
        public Transaction Transaction { get { return transaction; } set { transaction = value; } }

        /// <summary>
        /// The direction of the position
        /// </summary>
        public PosDirection PosDir { get { return posDirection; } set { posDirection = value; } }

        /// <summary>
        /// The amount of the position
        /// </summary>
        public int PosNumb { get { return posNumb; } set { posNumb = value; } }

        /// <summary>
        /// The bar when the position was open
        /// </summary>
        public int OpeningBar { get { return openingBar; } set { openingBar = value; } }

        /// <summary>
        /// The amount of the position in lots
        /// </summary>
        public double PosLots { get { return posLots; } set { posLots = value; } }

        /// <summary>
        /// The corrected position's price
        /// </summary>
        public double PosPrice { get { return posPrice; } set { posPrice = value; } }

        /// <summary>
        /// Absolute mode Permanent SL
        /// </summary>
        public double AbsoluteSL { get { return absoluteSL; } set { absoluteSL = value; } }

        /// <summary>
        /// Absolute mode Permanent TP
        /// </summary>
        public double AbsoluteTP { get { return absoluteTP; } set { absoluteTP = value; } }

        /// <summary>
        /// Break Even
        /// </summary>
        public double BreakEven { get { return breakEven; } set { breakEven = value; } }

        /// <summary>
        /// The required margin
        /// </summary>
        public double RequiredMargin { get { return requiredMargin; } set { requiredMargin = value; } }

        /// <summary>
        /// Gets the free margin
        /// </summary>
        public double FreeMargin { get { return moneyEquity - requiredMargin; } }

        /// <summary>
        /// The forming order number
        /// </summary>
        public int FormOrdNumb { get { return ordNumb; } set { ordNumb = value; } }

        /// <summary>
        /// The forming order price
        /// </summary>
        public double FormOrdPrice { get { return ordPrice; } set { ordPrice = value; } }

        /// <summary>
        /// The position Profit Loss
        /// </summary>
        public double ProfitLoss { get { return profitLoss; } set { profitLoss = value; } }

        /// <summary>
        /// The position Floating Profit Loss
        /// </summary>
        public double FloatingPL { get { return floatingPL; } set { floatingPL = value; } }

        /// <summary>
        /// Account balance at the end of the session
        /// </summary>
        public double Balance { get { return balance; } set { balance = value; } }

        /// <summary>
        /// Account equity at the end of the session
        /// </summary>
        public double Equity { get { return equity; } set { equity = value; } }

        /// <summary>
        /// Charged spread [pips]
        /// </summary>
        public double Spread { get { return chargedSpread; } set { chargedSpread = value; } }

        /// <summary>
        /// Charged rollover
        /// </summary>
        public double Rollover { get { return chargedRollover; } set { chargedRollover = value; } }

        /// <summary>
        /// Charged commission [pips]
        /// </summary>
        public double Commission { get { return chargedCommission; } set { chargedCommission = value; } }

        /// <summary>
        /// Charged slippage [pips]
        /// </summary>
        public double Slippage { get { return chargedSlippage; } set { chargedSlippage = value; } }

        /// <summary>
        /// The position Profit Loss in currency
        /// </summary>
        public double MoneyProfitLoss { get { return moneyProfitLoss; } set { moneyProfitLoss = value; } }

        /// <summary>
        /// The position Floating Profit Loss in currency
        /// </summary>
        public double MoneyFloatingPL { get { return moneyFloatingPL; } set { moneyFloatingPL = value; } }

        /// <summary>
        /// Account balance at the end of the session in currency
        /// </summary>
        public double MoneyBalance { get { return moneyBalance; } set { moneyBalance = value; } }

        /// <summary>
        /// Account equity at the end of the session in currency
        /// </summary>
        public double MoneyEquity { get { return moneyEquity; } set { moneyEquity = value; } }

        /// <summary>
        /// Charged spread in currency
        /// </summary>
        public double MoneySpread { get { return moneySpread; } set { moneySpread = value; } }

        /// <summary>
        /// Charged rollover in currency
        /// </summary>
        public double MoneyRollover { get { return moneyRollover; } set { moneyRollover = value; } }

        /// <summary>
        /// Charged commission in currency
        /// </summary>
        public double MoneyCommission { get { return moneyCommission; } set { moneyCommission = value; } }

        /// <summary>
        /// Charged slippage in currency
        /// </summary>
        public double MoneySlippage { get { return moneySlippage; } set { moneySlippage = value; } }

        /// <summary>
        /// Gets the position's icon
        /// </summary>
        public Image PositionIcon
        {
            get
            {
                Image img = Properties.Resources.pos_square;

                if (transaction == Transaction.Open)
                {
                    if (posDirection == PosDirection.Long)
                        img = Properties.Resources.pos_buy;
                    else
                        img = Properties.Resources.pos_sell;
                }
                else if (transaction == Transaction.Close)
                    img = Properties.Resources.pos_close;
                else if (transaction == Transaction.Transfer)
                {
                    if (posDirection == PosDirection.Long)
                        img = Properties.Resources.pos_transfer_long;
                    else
                        img = Properties.Resources.pos_transfer_short;
                }
                else if (transaction == Transaction.Add)
                {
                    if (posDirection == PosDirection.Long)
                        img = Properties.Resources.pos_add_long;
                    else
                        img = Properties.Resources.pos_add_short;
                }
                else if (transaction == Transaction.Reduce || transaction == Transaction.Reverse)
                {
                    if (posDirection == PosDirection.Long)
                        img = Properties.Resources.pos_revers_long;
                    else
                        img = Properties.Resources.pos_revers_short;
                }

                return img;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Position()
        {
            posDirection = PosDirection.None;
            transaction  = Transaction.None;
        }

        /// <summary>
        /// Deep copy of the position
        /// </summary>
        public Position Copy()
        {
            Position position = new Position();

            position.transaction       = this.transaction;
            position.posDirection      = this.posDirection;
            position.openingBar        = this.openingBar;
            position.ordNumb           = this.ordNumb;
            position.ordPrice          = this.ordPrice;
            position.posNumb           = this.posNumb;
            position.posLots           = this.posLots;
            position.posPrice          = this.posPrice;
            position.absoluteSL        = this.absoluteSL;
            position.absoluteTP        = this.absoluteTP;
            position.breakEven         = this.breakEven;
            position.requiredMargin    = this.requiredMargin;
            position.profitLoss        = this.profitLoss;
            position.floatingPL        = this.floatingPL;
            position.balance           = this.balance;
            position.equity            = this.equity;
            position.chargedSpread     = this.chargedSpread;
            position.chargedRollover   = this.chargedRollover;
            position.chargedCommission = this.chargedCommission;
            position.chargedSlippage   = this.chargedSlippage;
            position.moneyProfitLoss   = this.moneyProfitLoss;
            position.moneyFloatingPL   = this.moneyFloatingPL;
            position.moneyBalance      = this.moneyBalance;
            position.moneyEquity       = this.moneyEquity;
            position.moneySpread       = this.moneySpread;
            position.moneyRollover     = this.moneyRollover;
            position.moneyCommission   = this.moneyCommission;
            position.moneySlippage     = this.moneySlippage;

            return position;
        }

        /// <summary>
        /// represents the position.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string pos = "";
            string NL = Environment.NewLine;
            string AC = Configs.AccountCurrency;

            pos += "Pos Numb           "        + (posNumb + 1).ToString()     + NL;
            pos += "Transaction        "        + transaction.ToString()       + NL;
            pos += "Direction          "        + posDirection.ToString()      + NL;
            pos += "Opening Bar        "        + (openingBar + 1).ToString()  + NL;
            pos += "Ord Numb           "        + (ordNumb + 1).ToString()     + NL;
            pos += "Ord Price          "        + ordPrice.ToString()          + NL;
            pos += "Pos Lots           "        + posLots.ToString()           + NL;
            pos += "Pos Price          "        + posPrice.ToString()          + NL;
            pos += "Req. Margin [" + AC + "]  " + requiredMargin.ToString()    + NL;
            pos += "---------------------------------"  + NL;
            pos += "Abs Permanent SL   "        + absoluteSL.ToString()        + NL;
            pos += "Abs Permanent TP   "        + absoluteTP.ToString()        + NL;
            pos += "Break Even         "        + breakEven.ToString()         + NL;
            pos += "---------------------------------"  + NL;
            pos += "Spread      [pips] "        + chargedSpread.ToString()     + NL;
            pos += "Rollover    [pips] "        + chargedRollover.ToString()   + NL;
            pos += "Commission  [pips] "        + chargedCommission.ToString() + NL;
            pos += "Slippage    [pips] "        + chargedSlippage.ToString()   + NL;
            pos += "Floating PL [pips] "        + floatingPL.ToString()        + NL;
            pos += "Profit Loss [pips] "        + profitLoss.ToString()        + NL;
            pos += "Balance     [pips] "        + balance.ToString()           + NL;
            pos += "Equity      [pips] "        + equity.ToString()            + NL;
            pos += "---------------------------------"  + NL;
            pos += "Spread      [" + AC + "]  " + moneySpread.ToString()       + NL;
            pos += "Rollover    [" + AC + "]  " + moneyRollover.ToString()     + NL;
            pos += "Commission  [" + AC + "]  " + moneyCommission.ToString()   + NL;
            pos += "Slippage    [" + AC + "]  " + moneySlippage.ToString()     + NL;
            pos += "Floating PL [" + AC + "]  " + moneyFloatingPL.ToString()   + NL;
            pos += "Profit Loss [" + AC + "]  " + moneyProfitLoss.ToString()   + NL;
            pos += "Balance     [" + AC + "]  " + moneyBalance.ToString()      + NL;
            pos += "Equity      [" + AC + "]  " + moneyEquity.ToString()       + NL;

            return pos;
        }
    }
}
