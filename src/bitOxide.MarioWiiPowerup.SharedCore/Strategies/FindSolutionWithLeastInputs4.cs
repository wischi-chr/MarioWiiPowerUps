﻿using System.Linq;

namespace bitOxide.MarioWiiPowerup.Core.Strategies
{
    public class FindSolutionWithLeastInputs4 : ISuggestionStrategy
    {
        public int? SuggestNextItemPosition(Board[] boards, Item[] filledItems)
        {
            var allMatchingBoards = boards.Where(x => x.Matches(filledItems)).ToArray();

            if (allMatchingBoards.Length <= 1)
            {
                // nothing to suggest
                return null;
            }

            if (filledItems.All(i => i == null))
            {
                // hardcode the first suggestion to be at the bottom and a low risk of being a bowser.
                return 14;
            }

            var duplicates = allMatchingBoards.GetInformationContent();
            var bowserPenality = allMatchingBoards.GetBowserFieldPenalty2();
            var instantLose = allMatchingBoards.GetInstantLoseScore(filledItems);
            var nonOpen = BoardScoreExtensions.GetNonOpenPositions(filledItems);

            var props = BoardScoreExtensions.MergeScores(nonOpen, instantLose, duplicates, bowserPenality);

            // determine lowest risk factor
            double maxProp = -1;
            int curPos = -1;

            for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
            {
                if (props[i] > maxProp)
                {
                    maxProp = props[i];
                    curPos = i;
                }
            }

            return curPos;
        }
    }
}
