﻿using System;
using System.Collections.Generic;
using System.Linq;
using bitOxide.MarioWiiPowerup.Core.Strategies;

namespace bitOxide.MarioWiiPowerup.Core.ViewModel
{
    public class MainPanel
    {
        private readonly Item[] itemInformations = new Item[SuperMarioWiiConstants.ItemsPerBoard];
        private readonly Board[] allBoards = SuperMarioWiiBoards.DefaultBoardSet.AllBoards.ToArray();
        private int focusedItem = 0;
        private readonly bool[] noBowserInPosition = new bool[SuperMarioWiiConstants.ItemsPerBoard];

        private readonly Item[] derivedItems = new Item[SuperMarioWiiConstants.ItemsPerBoard];
        private readonly ISuggestionStrategy strat = new FindSolutionWithLeastInputs4();
        private readonly Stack<Item[]> itemHistory = new Stack<Item[]>();

        private Board currentSolution = null;
        public Board SolvedBoard => currentSolution;
        public int FocusedPositionId => focusedItem;
        public int MatchingBoardCount { get; private set; }

        public Item GetDerivedItem(int pos)
        {
            return derivedItems[pos];
        }

        public bool IsPositionSafe(int pos)
        {
            return noBowserInPosition[pos];
        }

        public Item GetItemFromPosition(int pos)
        {
            return itemInformations[pos];
        }

        public MainPanel()
        {

        }

        private void SaveCurrentStateAsHistory()
        {
            var entry = new Item[SuperMarioWiiConstants.ItemsPerBoard];

            for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
            {
                entry[i] = itemInformations[i];
            }

            itemHistory.Push(entry);
        }

        private void RestoreHistoryState()
        {
            if (itemHistory.Count < 1)
            {
                return;
            }

            var lastEntry = itemHistory.Pop();

            for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
            {
                itemInformations[i] = lastEntry[i];
            }
        }

        public void ResetBoardInfos()
        {
            for (int i = 0; i < itemInformations.Length; i++)
            {
                itemInformations[i] = null;
            }

            itemHistory.Clear();
            RecalcAll();
        }

        public void RemoveFocusedItem()
        {
            RestoreHistoryState();
            RecalcAll();
        }

        public void FocusBestPosition()
        {
            var res = strat.SuggestNextItemPosition(allBoards, itemInformations);

            if (res == null)
            {
                return;
            }

            focusedItem = res.Value;
        }

        public void FocusPosition(int pos)
        {
            if (pos < 0 || pos >= SuperMarioWiiConstants.ItemsPerBoard)
            {
                throw new IndexOutOfRangeException();
            }

            focusedItem = pos;
        }

        public void ClickItem(Item item)
        {
            SaveCurrentStateAsHistory();
            itemInformations[focusedItem] = item;
            RecalcAll();
        }

        private void RecalcAll(bool reposition = true)
        {
            FindPossibleSolutionsAndSafeSpots();

            if (reposition)
            {
                FocusBestPosition();
            }
        }

        private void FindPossibleSolutionsAndSafeSpots()
        {
            var allMatchingBoards = allBoards.Where(x => x.Matches(itemInformations)).ToArray();

            MatchingBoardCount = allMatchingBoards.Length;
            currentSolution = allMatchingBoards.Length == 1 ? allMatchingBoards[0] : null;

            // set derived items to null
            for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
            {
                derivedItems[i] = null;
            }

            // Update bowser safe spots
            if (allMatchingBoards.Length > 0)
            {
                for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
                {
                    // set all states to "safe"
                    noBowserInPosition[i] = true;

                    // find out if items are the same everywhere
                    derivedItems[i] = AllSameOrNull(allMatchingBoards.Select(b => b[i]), null);
                }

                foreach (var b in allMatchingBoards)
                {
                    for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
                    {
                        if (b[i].IsBad)
                        {
                            // if bowser is found in spot, than mark as not safe
                            noBowserInPosition[i] = false;
                        }
                    }
                }
            }
            else
            {
                // set all states to "not safe"
                for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
                {
                    noBowserInPosition[i] = false;
                }
            }
        }

        private static T AllSameOrNull<T>(IEnumerable<T> elements, T fallback)
        {
            var res = elements.Distinct().Take(2).ToList();

            if (res.Count == 1)
            {
                return res[0];
            }
            else if (res.Count == 2 && res[0].Equals(res[1]))
            {
                return res[0];
            }

            return fallback;
        }
    }
}
