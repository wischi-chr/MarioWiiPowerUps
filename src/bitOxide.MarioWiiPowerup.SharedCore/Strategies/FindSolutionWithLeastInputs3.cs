using System.Linq;

namespace bitOxide.MarioWiiPowerup.Core.Strategies
{
    public class FindSolutionWithLeastInputs3 : ISuggestionStrategy
    {
        public int? SuggestNextItemPosition(Board[] boards, Item[] filledItems, int? lastPosition)
        {
            var allMatchingBoards = boards.Where(x => x.Matches(filledItems)).ToArray();

            if (allMatchingBoards.Length <= 1)
            {
                // nothing to suggest
                return null;
            }

            var duplicates = allMatchingBoards.GetInformationContent();
            var bowserPenality = allMatchingBoards.GetBowserFieldPenalty();
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
