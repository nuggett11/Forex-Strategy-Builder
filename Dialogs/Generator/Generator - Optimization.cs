// Strategy Generator - Math
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Forex_Strategy_Builder.Dialogs.Generator
{
    /// <summary>
    /// Strategy Generator
    /// </summary>
    public partial class Generator : Form
    {
        /// <summary>
        /// Initial Optimization
        /// </summary>
        void PerformInitialOptimization(BackgroundWorker worker, bool isBetter)
        {
            bool secondChance = (random.Next(100) < 10 && Backtester.NetBalance > 500);
            int maxCycles = isBetter ? 3 : 1;

            if (isBetter || secondChance)
            {
                for (int cycle = 0; cycle < maxCycles; cycle++)
                {
                    // Change parameters
                    ChangeNumericParameters(worker);

                    // Change Permanent Stop Loss
                    ChangePermanentSL(worker);

                    // Change Permanent Take Profit
                    ChangePermanentTP(worker);

                }

                // Remove needless filters
                RemoveNeedlessFilters(worker);

                // Tries to clear the Same / Opposite Signals
                NormalizeSameOppositeSignalBehaviour(worker);

                // Remove Permanent Stop Loss
                if (!chbPreservPermSL.Checked && strategyBest.PropertiesStatus == StrategySlotStatus.Open && Data.Strategy.UsePermanentSL && !worker.CancellationPending)
                    RemovePermanentSL();

                // Remove Permanent Take Profit
                if (!chbPreservPermTP.Checked && strategyBest.PropertiesStatus == StrategySlotStatus.Open && Data.Strategy.UsePermanentTP && !worker.CancellationPending)
                    RemovePermanentTP(worker);

                // Reduce the value of numeric parameters
                if (!chbUseDefaultIndicatorValues.Checked)
                    ReduceTheValuesOfNumericParams(worker);
           }
        }

        /// <summary>
        /// Tries to clear the Same / Opposite Signals
        /// </summary>
        void NormalizeSameOppositeSignalBehaviour(BackgroundWorker worker)
        {
            if (strategyBest.PropertiesStatus == StrategySlotStatus.Open)
            {
                if (Data.Strategy.SameSignalAction != SameDirSignalAction.Nothing)
                {
                    if (!worker.CancellationPending)
                    {
                        Data.Strategy.SameSignalAction = SameDirSignalAction.Nothing;
                        bool isBetterORSame = CalculateTheResult(true);
                        if (!isBetterORSame)
                            RestoreFromBest();
                    }
                }

                if (Data.Strategy.OppSignalAction != OppositeDirSignalAction.Nothing &&
                    Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName != "Close and Reverse")
                {
                    if (!worker.CancellationPending)
                    {
                        Data.Strategy.OppSignalAction = OppositeDirSignalAction.Nothing;
                        bool isBetterORSame = CalculateTheResult(true);
                        if (!isBetterORSame)
                            RestoreFromBest();
                    }
                }
            }
        }

        /// <summary>
        /// Removes the excessive filter.
        /// </summary>
        void RemoveNeedlessFilters(BackgroundWorker worker)
        {
            for (int slot = 1; slot < Data.Strategy.Slots; slot++)
            {
                if (Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Locked || Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Linked)
                    continue;

                if (Data.Strategy.Slot[slot].SlotType == SlotTypes.OpenFilter || Data.Strategy.Slot[slot].SlotType == SlotTypes.CloseFilter)
                {
                    if (worker.CancellationPending) break;

                    Data.Strategy.RemoveFilter(slot);
                    bool isBetterORSame = CalculateTheResult(true);
                    if (!isBetterORSame)
                        RestoreFromBest();
                }
            }
        }

        /// <summary>
        /// Change Numeric Parameters
        /// </summary>
        void ChangeNumericParameters(BackgroundWorker worker)
        {
            bool isDoAgain;
            int repeats = 0;
            do
            {
                isDoAgain = repeats < 4;
                repeats++;
                for (int slot = 0; slot < Data.Strategy.Slots; slot++)
                {
                    if (Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Locked) continue;
                    if (worker.CancellationPending) break;

                    GenerateIndicatorParameters(slot);
                    RecalculateSlots();
                    isDoAgain = CalculateTheResult(false);
                    if (!isDoAgain)
                        RestoreFromBest();
                }
            } while (isDoAgain);
        }

        /// <summary>
        /// Change Permanent Stop Loss
        /// </summary>
        void ChangePermanentSL(BackgroundWorker worker)
        {
            int repeats = 0;
            bool isDoAgain;
            do
            {
                if (worker.CancellationPending) break;
                if (chbPreservPermSL.Checked || strategyBest.PropertiesStatus == StrategySlotStatus.Locked)
                    break;

                int oldPermSL = Data.Strategy.PermanentSL;
                Data.Strategy.UsePermanentSL = true;
                int multiplier = Data.InstrProperties.IsFiveDigits ? 50 : 5;
                Data.Strategy.PermanentSL = multiplier * random.Next(5, 100);

                repeats++;
                isDoAgain = repeats < 5;
                isDoAgain = CalculateTheResult(false);
                if (!isDoAgain)
                    Data.Strategy.PermanentSL = oldPermSL;
            } while (isDoAgain);
        }

        /// <summary>
        /// Remove Permanent Stop Loss
        /// </summary>
        void RemovePermanentSL()
        {
            int oldPermSL = Data.Strategy.PermanentSL;
            Data.Strategy.UsePermanentSL = false;
            Data.Strategy.PermanentSL    = Data.InstrProperties.IsFiveDigits ? 1000 : 100;
            bool isBetterORSame = CalculateTheResult(true);
            if (!isBetterORSame)
            {
                Data.Strategy.UsePermanentSL = true;
                Data.Strategy.PermanentSL = oldPermSL;
            }
        }

        /// <summary>
        ///  Change Permanent Take Profit
        /// </summary>
        void ChangePermanentTP(BackgroundWorker worker)
        {
            bool isDoAgain;
            int  repeats    = 0;
            int  multiplier = Data.InstrProperties.IsFiveDigits ? 50 : 5;

            do
            {
                if (worker.CancellationPending) break;
                if (chbPreservPermTP.Checked || strategyBest.PropertiesStatus == StrategySlotStatus.Locked || !Data.Strategy.UsePermanentTP)
                    break;

                int oldPermTP = Data.Strategy.PermanentTP;
                Data.Strategy.UsePermanentTP = true;
                Data.Strategy.PermanentTP = multiplier * random.Next(5, 100);

                repeats++;
                isDoAgain = repeats < 2;
                isDoAgain = CalculateTheResult(false);
                if (!isDoAgain)
                    Data.Strategy.PermanentTP = oldPermTP;
            } while (isDoAgain);
        }

        /// <summary>
        /// Removes the Permanent Take Profit
        /// </summary>
        void RemovePermanentTP(BackgroundWorker worker)
        {
            int oldPermTP = Data.Strategy.PermanentTP;
            Data.Strategy.UsePermanentTP = false;
            Data.Strategy.PermanentTP = Data.InstrProperties.IsFiveDigits ? 1000 : 100;
            bool isBetterORSame = CalculateTheResult(true);
            if (!isBetterORSame)
            {
                Data.Strategy.UsePermanentTP = true;
                Data.Strategy.PermanentTP = oldPermTP;
            }
        }

        /// <summary>
        /// Normalizes the numeric parameters.
        /// </summary>
        void ReduceTheValuesOfNumericParams(BackgroundWorker worker)
        { 
            bool isDoAgain;
            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                if (bestBalance < 500) break;
                if (Data.Strategy.Slot[slot].SlotStatus == StrategySlotStatus.Locked) continue;

                // Numeric params
                for (int param = 0; param < 6; param++)
                {
                    if (!Data.Strategy.Slot[slot].IndParam.NumParam[param].Enabled) continue;

                    do
                    {
                        if (worker.CancellationPending) break;
                        isDoAgain = false;

                        IndicatorSlot indSlot = Data.Strategy.Slot[slot];
                        NumericParam num = Data.Strategy.Slot[slot].IndParam.NumParam[param];
                        if (num.Caption == "Level" && !indSlot.IndParam.ListParam[0].Text.Contains("Level")) break;

                        Indicator indicator = Indicator_Store.ConstructIndicator(indSlot.IndicatorName, indSlot.SlotType);
                        double defaultValue = indicator.IndParam.NumParam[param].Value;

                        double numOldValue = num.Value;
                        if (num.Value == defaultValue) break;

                        double step  = Math.Pow(10, -num.Point);
                        double value = num.Value;
                        double delta = (defaultValue - value) * 3 / 4;
                        value += delta;
                        value = Math.Round(value, num.Point);

                        if (Math.Abs(value - numOldValue) < value) break;

                        num.Value = value;

                        RecalculateSlots();
                        isDoAgain = CalculateTheResult(true);
                        if (!isDoAgain) RestoreFromBest();

                    } while (isDoAgain);
                }
            }
        }
    }
}
