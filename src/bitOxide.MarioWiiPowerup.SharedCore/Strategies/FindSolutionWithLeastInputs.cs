using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bitOxide.MarioWiiPowerup.Core.Strategies
{
    public class FindSolutionWithLeastInputs : ISuggestionStrategy
    {
        public int? SuggestNextItemPosition(Board[] boards, Item[] filledItems)
        {
            var allMatchingBoards = boards.Where(x => x.Matches(filledItems)).ToArray();
            if (allMatchingBoards.Length <= 1) return null; //nothing to suggest

            var bowserProp = allMatchingBoards.GetGoodPositionScore();
            var duplicates = allMatchingBoards.GetDiversityScore();

            var instantLose = allMatchingBoards.GetInstantLoseScore(filledItems);
            var nonOpen = BoardScoreExtensions.GetNonOpenPositions(filledItems);
            var props = BoardScoreExtensions.MergeScores(nonOpen, instantLose, duplicates);

            //Bestimme minimales Risiko
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

    public class FindSolutionWithLeastInputs2 : ISuggestionStrategy
    {
        public int? SuggestNextItemPosition(Board[] boards, Item[] filledItems)
        {
            var allMatchingBoards = boards.Where(x => x.Matches(filledItems)).ToArray();
            if (allMatchingBoards.Length <= 1) return null; //nothing to suggest

            var duplicates = allMatchingBoards.GetInformationContent();
            var instantLose = allMatchingBoards.GetInstantLoseScore(filledItems);
            var nonOpen = BoardScoreExtensions.GetNonOpenPositions(filledItems);
            var props = BoardScoreExtensions.MergeScores(nonOpen, instantLose, duplicates);

            //Bestimme minimales Risiko
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

    public class FindSolutionWithLeastInputs3 : ISuggestionStrategy
    {
        public int? SuggestNextItemPosition(Board[] boards, Item[] filledItems)
        {
            var allMatchingBoards = boards.Where(x => x.Matches(filledItems)).ToArray();
            if (allMatchingBoards.Length <= 1) return null; //nothing to suggest

            var duplicates = allMatchingBoards.GetInformationContent();
            var bowserPenality = allMatchingBoards.GetBowserFieldPenalty();
            var instantLose = allMatchingBoards.GetInstantLoseScore(filledItems);
            var nonOpen = BoardScoreExtensions.GetNonOpenPositions(filledItems);
            var props = BoardScoreExtensions.MergeScores(nonOpen, instantLose, duplicates, bowserPenality);

            //Bestimme minimales Risiko
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

    internal static class BoardScoreExtensions
    {
        public static double[] GetInformationContent(this Board[] boards)
        {
            var infos = new double[SuperMarioWiiConstants.ItemsPerBoard];
            for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
            {
                infos[i] = GetInformationContentFromBoard(boards, i);
            }
            return infos;
        }

        private static double GetInformationContentFromBoard(Board[] boards, int position)
        {
            return GetInformationContentFromList(boards.Select(b => b[position]));
        }

        private static double GetInformationContentFromList<T>(IEnumerable<T> elements)
        {
            var itemCounter = new Dictionary<T, int>();
            var cnt = 0;

            foreach (var e in elements)
            {
                if (!itemCounter.ContainsKey(e))
                    itemCounter.Add(e, 0);
                itemCounter[e]++;
                cnt++;
            }

            double infoSum = 0;
            foreach (var i in itemCounter)
            {
                var px = i.Value / (double)cnt;
                var info = -Math.Log(px, 2);
                infoSum += info * i.Value;
            }

            return infoSum;
        }

        public static double[] GetDiversityScore(this IEnumerable<Board> boards)
        {
            var cntBoards = 0;
            var cntDuplicates = new int[SuperMarioWiiConstants.ItemsPerBoard];
            var items = new List<Item>[SuperMarioWiiConstants.ItemsPerBoard];

            for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
                items[i] = new List<Item>();

            foreach (var b in boards)
            {
                cntBoards++;
                for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
                {
                    if (items[i].Contains(b[i]))
                    {
                        cntDuplicates[i]++;
                    }
                    else
                    {
                        items[i].Add(b[i]);
                    }
                }
            }

            var maxDup = cntDuplicates.Max();
            if (maxDup == 0)
                return Enumerable.Range(0, SuperMarioWiiConstants.ItemsPerBoard).Select(i => 1.0).ToArray();
            return cntDuplicates.Select(d => 1.0 - (double)d / maxDup).ToArray();
        }

        public static double[] GetInstantLoseScore(this IEnumerable<Board> boards, Item[] filledItems)
        {
            var res = new double[SuperMarioWiiConstants.ItemsPerBoard];
            for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++) res[i] = 1.0;

            bool noSmall = filledItems.Where(x => x == Item.MiniBowser).Count() > 0;
            bool noLarge = filledItems.Where(x => x == Item.Bowser).Count() > 0;

            foreach (var b in boards)
            {
                for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
                {
                    if (b[i] == Item.Bowser && noLarge) res[i] = 0;
                    else if (b[i] == Item.MiniBowser && noSmall) res[i] = 0;
                }
            }

            return res;
        }

        public static double[] GetGoodPositionScore(this IEnumerable<Board> boards)
        {
            var cntBoards = 0;
            var cntBowsers = new int[SuperMarioWiiConstants.ItemsPerBoard];

            foreach (var b in boards)
            {
                cntBoards++;
                for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
                    if (b[i].IsBad)
                        cntBowsers[i]++;
            }

            return cntBowsers.Select(i => (double)(cntBoards - i) / cntBoards).ToArray();
        }

        public static double[] GetBowserFieldPenalty(this IEnumerable<Board> boards)
        {
            var cntBoards = 0;
            var cntBowsers = new int[SuperMarioWiiConstants.ItemsPerBoard];

            foreach (var b in boards)
            {
                cntBoards++;
                for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
                    if (b[i].IsBad)
                        cntBowsers[i]++;
            }

            return cntBowsers.Select(i => i > 0 ? 0.95 : 1.0).ToArray();
        }

        public static double[] DiscardBowserFieldScore(this IEnumerable<Board> boards)
        {
            var cntBoards = 0;
            var cntBowsers = new int[SuperMarioWiiConstants.ItemsPerBoard];

            foreach (var b in boards)
            {
                cntBoards++;
                for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
                    if (b[i].IsBad)
                        cntBowsers[i]++;
            }

            return cntBowsers.Select(i => i > 0 ? 0.0 : 1.0).ToArray();
        }

        public static double[] GetNonOpenPositions(Item[] filledItems)
        {
            var nop = new double[SuperMarioWiiConstants.ItemsPerBoard];
            for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
                nop[i] = filledItems[i] != null ? 0 : 1;
            return nop;
        }

        public static double[] MergeScores(params double[][] props)
        {
            var res = new double[SuperMarioWiiConstants.ItemsPerBoard];
            for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
                res[i] = 1.0;

            foreach (var p in props)
            {
                if (p.Length != SuperMarioWiiConstants.ItemsPerBoard) throw new ArgumentOutOfRangeException();
                for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++) res[i] *= p[i];
            }

            return res;
        }
    }
}
