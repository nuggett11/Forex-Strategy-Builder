// Instrument_Properties Class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Type of the instrument.
    /// </summary>
    public enum Instrumet_Type { Forex, CFD, Index }

    /// <summary>
    /// Type of the commission.
    /// </summary>
    public enum Commission_Type { pips, percents, money }

    /// <summary>
    /// Scope of the commission.
    /// </summary>
    public enum Commission_Scope { lot, deal }

    /// <summary>
    /// Time of the commission.
    /// </summary>
    public enum Commission_Time { open, openclose }

    /// <summary>
    /// Contains the instrument properties.
    /// </summary>
    public class Instrument_Properties
    {
        string symbol;
        string priceIn;
        string baseFileName;
        string comment;
        int digits;
        int lotSize;
        int slippage;
        double spread;
        double swapLong;
        double swapShort;
        double commission;
        double rateToUSD;
        double rateToEUR;
        double point;
        double pip;
        bool   isFiveDigits;
        Instrumet_Type   instrType;
        Commission_Type  swapType;
        Commission_Type  commissionType;
        Commission_Scope commissionScope;
        Commission_Time  commissionTime;

        public string Symbol       { get { return symbol;  } set { symbol = value; } }
        public string Comment      { get { return comment; } set { comment = value; } }
        public string PriceIn      { get { return priceIn; } set { priceIn = value; } }
        public string BaseFileName { get { return baseFileName; } set { baseFileName = value; } }
        public int    LotSize      { get { return lotSize;    } set { lotSize = value; } }
        public int    Slippage     { get { return slippage;   } set { slippage = value; } }
        public double Spread       { get { return spread;     } set { spread = value; } }
        public double SwapLong     { get { return swapLong;   } set { swapLong = value; } }
        public double SwapShort    { get { return swapShort;  } set { swapShort = value; } }
        public double Commission   { get { return commission; } set { commission = value; } }
        public double RateToUSD    { get { return rateToUSD;  } set { rateToUSD = value; } }
        public double RateToEUR    { get { return rateToEUR;  } set { rateToEUR = value; } }
        public Instrumet_Type   InstrType       { get { return instrType; } set { instrType = value; } }
        public Commission_Type  SwapType        { get { return swapType; } set { swapType = value; } }
        public Commission_Type  CommissionType  { get { return commissionType; } set { commissionType = value; } }
        public Commission_Scope CommissionScope { get { return commissionScope; } set { commissionScope = value; } }
        public Commission_Time  CommissionTime  { get { return commissionTime; } set { commissionTime = value; } }

        public int Digits
        {
            get { return digits; }
            set {
                digits = value;
                point = 1 / Math.Pow(10, digits);
                isFiveDigits = (digits == 3 || digits == 5);
                pip = isFiveDigits ? 10 * point : point;
            }
        }
        public double Point { get { return point; } }
        public double Pip   { get { return pip;   } }
        public bool IsFiveDigits { get { return isFiveDigits; } }

        /// <summary>
        /// Gets the Commission type as a string
        /// </summary>
        public string CommissionTypeToString
        {
            get
            {
                if (commissionType == Commission_Type.pips)
                    return Language.T("pips");
                else if (commissionType == Commission_Type.percents)
                    return Language.T("percents");
                else
                    return Language.T("money");
            }
        }

        /// <summary>
        /// Gets the Commission Scope as a string
        /// </summary>
        public string CommissionScopeToString
        {
            get
            {
                if (commissionScope == Commission_Scope.lot)
                    return Language.T("per lot");
                else
                    return Language.T("per deal");
            }
        }

        /// <summary>
        /// Gets the Commission Time as a string
        /// </summary>
        public string CommissionTimeToString
        {
            get
            {
                if (commissionTime == Commission_Time.open)
                    return Language.T("at opening");
                else
                    return Language.T("at opening and closing");
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Instrument_Properties(string symbol, Instrumet_Type instrType)
        {
            if (instrType == Instrumet_Type.Forex)
            {
                this.symbol     = symbol;
                this.instrType  = instrType;
                comment         = symbol.Substring(0,3) + " vs " + symbol.Substring(3, 3);
                Digits          = (symbol.Contains("JPY") ? 3 : 5);
                lotSize         = 100000;
                spread          = 20;
                swapType        = Commission_Type.pips;
                swapLong        = 2;
                swapShort       = -2;
                commissionType  = Commission_Type.pips;
                commissionScope = Commission_Scope.lot;
                commissionTime  = Commission_Time.openclose;
                commission      = 0;
                slippage        = 0;
                priceIn         = symbol.Substring(3, 3);
                rateToUSD       = (symbol.Contains("JPY") ? 100 : 1);
                rateToEUR       = (symbol.Contains("JPY") ? 100 : 1);
                baseFileName    = symbol;
            }
            else
            {
                this.symbol     = symbol;
                this.instrType  = instrType;
                comment         = symbol + " " + instrType.ToString();
                Digits          = 2;
                lotSize         = 100;
                spread          = 4;
                swapType        = Commission_Type.percents;
                swapLong        = -5;
                swapShort       = -1;
                commissionType  = Commission_Type.percents;
                commissionScope = Commission_Scope.deal;
                commissionTime  = Commission_Time.openclose;
                commission      = 0.25f;
                slippage        = 0;
                priceIn         = "USD";
                rateToUSD       = 1;
                rateToEUR       = 1;
                baseFileName    = symbol;
            }
        }

        /// <summary>
        /// Clones the Instrument_Properties.
        /// </summary>
        public Instrument_Properties Clone()
        {
            Instrument_Properties copy = new Instrument_Properties(symbol, instrType);

            copy.Symbol          = Symbol;
            copy.InstrType       = InstrType;
            copy.Comment         = Comment;
            copy.Digits          = Digits;
            copy.LotSize         = LotSize;
            copy.Spread          = Spread;
            copy.SwapType        = SwapType;
            copy.SwapLong        = SwapLong;
            copy.SwapShort       = SwapShort;
            copy.CommissionType  = CommissionType;
            copy.CommissionScope = CommissionScope;
            copy.CommissionTime  = CommissionTime;
            copy.Commission      = Commission;
            copy.PriceIn         = PriceIn;
            copy.Slippage        = Slippage;
            copy.RateToEUR       = RateToEUR;
            copy.RateToUSD       = RateToUSD;
            copy.BaseFileName    = BaseFileName;

            return copy;
        }
    }
}
