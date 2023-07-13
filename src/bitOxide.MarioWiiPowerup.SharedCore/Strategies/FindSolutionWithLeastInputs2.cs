using System.Linq;

namespace bitOxide.MarioWiiPowerup.Core.Strategies
{
    public class FindSolutionWithLeastInputs2 : ISuggestionStrategy
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
            var instantLose = allMatchingBoards.GetInstantLoseScore(filledItems);
            var nonOpen = BoardScoreExtensions.GetNonOpenPositions(filledItems);
            var props = BoardScoreExtensions.MergeScores(nonOpen, instantLose, duplicates);

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
