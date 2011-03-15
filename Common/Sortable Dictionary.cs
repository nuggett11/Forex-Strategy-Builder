// Sortable Dictionary
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Sortable dictionary by Keys
    /// </summary>
    public class SortableDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey : IComparable
    {
        public void Sort()
        {
            // Copy the dictionary data to a List
            List<KeyValuePair<TKey, TValue>> sortedList = new List<KeyValuePair<TKey, TValue>>(this);

            // Use the List's Sort method, and make sure we are comparing Keys.
            sortedList.Sort(delegate(KeyValuePair<TKey, TValue> first, KeyValuePair<TKey, TValue> second) { return first.Key.CompareTo(second.Key); });

            // Clear the dictionary and repopulate it from the List
            this.Clear();
            foreach (KeyValuePair<TKey, TValue> kvp in sortedList)
                this.Add(kvp.Key, kvp.Value);
        }

        public void ReverseSort()
        {
            // Copy the dictionary data to a List
            List<KeyValuePair<TKey, TValue>> sortedList = new List<KeyValuePair<TKey, TValue>>(this);

            // Use the List's Sort method, and make sure we are comparing Keys.
            sortedList.Sort(delegate(KeyValuePair<TKey, TValue> first, KeyValuePair<TKey, TValue> second) { return second.Key.CompareTo(first.Key); });

            // Clear the dictionary and repopulate it from the List
            this.Clear();
            foreach (KeyValuePair<TKey, TValue> kvp in sortedList)
                this.Add(kvp.Key, kvp.Value);
        }
    }
}
