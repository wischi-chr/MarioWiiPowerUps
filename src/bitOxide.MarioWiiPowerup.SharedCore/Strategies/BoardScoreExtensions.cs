using System;
using System.Collections.Generic;
using System.Linq;

namespace bitOxide.MarioWiiPowerup.Core.Strategies
{
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
                {
                    itemCounter.Add(e, 0);
                }

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
            {
                items[i] = new List<Item>();
            }

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

            return maxDup == 0
                ? Enumerable.Range(0, SuperMarioWiiConstants.ItemsPerBoard).Select(i => 1.0).ToArray()
                : cntDuplicates.Select(d => 1.0 - (double)d / maxDup).ToArray();
        }

        public static double[] GetInstantLoseScore(this IEnumerable<Board> boards, Item[] filledItems)
        {
            var res = new double[SuperMarioWiiConstants.ItemsPerBoard];

            for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
            {
                res[i] = 1.0;
            }

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
                {
                    if (b[i].IsBad)
                    {
                        cntBowsers[i]++;
                    }
                }
            }

            return cntBowsers.Select(i => (double)(cntBoards - i) / cntBoards).ToArray();
        }

        public static double[] GetDistanceFactors(int? position)
        {
            var res = new double[SuperMarioWiiConstants.ItemsPerBoard];

            if (!position.HasValue)
            {
                for (int i = 0; i < res.Length; i++)
                {
                    res[i] = 1.0;
                }

                return res;
            }

            var inputY = position.Value / 6;
            var inputX = position.Value % 6;

            for (int i = 0; i < res.Length; i++)
            {
                var y = i / 6;
                var x = i % 6;

                var distance = Math.Abs(x - inputX) + Math.Abs(y - inputY);
                res[i] = 1.0 - distance * 0.0001;
            }

            return res;
        }

        public static double[] GetBowserFieldPenalty(this IEnumerable<Board> boards)
        {
            var cntBoards = 0;
            var cntBowsers = new int[SuperMarioWiiConstants.ItemsPerBoard];

            foreach (var b in boards)
            {
                cntBoards++;

                for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
                {
                    if (b[i].IsBad)
                    {
                        cntBowsers[i]++;
                    }
                }
            }

            return cntBowsers.Select(i => i > 0 ? 0.95 : 1.0).ToArray();
        }

        public static double[] GetBowserFieldPenalty2(this IEnumerable<Board> boards)
        {
            var cntBoards = 0;
            var cntBowsers = new int[SuperMarioWiiConstants.ItemsPerBoard];

            foreach (var b in boards)
            {
                cntBoards++;

                for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
                {
                    if (b[i].IsBad)
                    {
                        cntBowsers[i]++;
                    }
                }
            }

            var bowserMin = cntBowsers.Min();
            var bowserDelta = cntBowsers.Max() - bowserMin;

            if (bowserMin == 0)
            {
                return cntBowsers.Select(bCnt => bCnt == 0 ? 1.0 : 0.0).ToArray();
            }

            return cntBowsers
                .Select(bCnt => (1.0 - (bCnt - bowserMin) / (double)bowserDelta) * 0.8 + 0.1)
                .ToArray();
        }

        public static double[] DiscardBowserFieldScore(this IEnumerable<Board> boards)
        {
            var cntBoards = 0;
            var cntBowsers = new int[SuperMarioWiiConstants.ItemsPerBoard];

            foreach (var b in boards)
            {
                cntBoards++;

                for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
                {
                    if (b[i].IsBad)
                    {
                        cntBowsers[i]++;
                    }
                }
            }

            return cntBowsers.Select(i => i > 0 ? 0.0 : 1.0).ToArray();
        }

        public static double[] GetNonOpenPositions(Item[] filledItems)
        {
            var nop = new double[SuperMarioWiiConstants.ItemsPerBoard];

            for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
            {
                nop[i] = filledItems[i] != null ? 0 : 1;
            }

            return nop;
        }

        public static double[] MergeScores(params double[][] props)
        {
            var res = new double[SuperMarioWiiConstants.ItemsPerBoard];

            for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
            {
                res[i] = 1.0;
            }

            foreach (var p in props)
            {
                if (p.Length != SuperMarioWiiConstants.ItemsPerBoard)
                {
                    throw new ArgumentOutOfRangeException();
                }

                for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
                {
                    res[i] *= p[i];
                }
            }

            return res;
        }
    }
}
