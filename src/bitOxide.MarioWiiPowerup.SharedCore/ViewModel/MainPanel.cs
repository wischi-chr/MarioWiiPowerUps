using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bitOxide.MarioWiiPowerup.Core.Strategies;

namespace bitOxide.MarioWiiPowerup.Core.ViewModel
{
    public class MainPanel
    {
        private readonly Item[] itemInformations = new Item[SuperMarioWiiConstants.ItemsPerBoard];
        private readonly Board[] allBoards = SuperMarioWiiBoards.DefaultBoardSet.AllBoards.ToArray();
        private int focusedItem = 0;

        private readonly ISuggestionStrategy strat = new FindSolutionWithLeastInputs3();
        private readonly Stack<Item[]> itemHistory = new Stack<Item[]>();

        private Board currentSolution = null;
        public Board SolvedBoard => currentSolution;
        public int FocusedPositionId => focusedItem;
        public int MatchingBoardCount { get; private set; }

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
                entry[i] = itemInformations[i];
            itemHistory.Push(entry);
        }

        private void RestoreHistoryState()
        {
            if (itemHistory.Count < 1) return;
            var lastEntry = itemHistory.Pop();
            for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
                itemInformations[i] = lastEntry[i];
        }

        public void ResetBoardInfos()
        {
            for (int i = 0; i < itemInformations.Length; i++)
                itemInformations[i] = null;
            itemHistory.Clear();
            RecalcAll();
        }

        public void RemoveFocusedItem()
        {
            //itemInformations[focusedItem] = null;
            //RecalcAll(reposition: false);
            RestoreHistoryState();
            RecalcAll();
        }

        public void FocusBestPosition()
        {
            var res = strat.SuggestNextItemPosition(allBoards, itemInformations);
            if (res == null) return;
            focusedItem = res.Value;
        }

        public void FocusPosition(int pos)
        {
            if (pos < 0 || pos >= SuperMarioWiiConstants.ItemsPerBoard) throw new IndexOutOfRangeException();
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
            FindPossibleSolutions();
            if (reposition)
                FocusBestPosition();
        }

        private void FindPossibleSolutions()
        {
            var allMatchingBoards = allBoards.Where(x => x.Matches(itemInformations)).ToArray();
            MatchingBoardCount = allMatchingBoards.Length;
            currentSolution = allMatchingBoards.Length == 1 ? allMatchingBoards[0] : null;
        }

    }
}
