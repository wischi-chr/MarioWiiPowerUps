using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using bitOxide.MarioWiiPowerup.Core;
using bitOxide.MarioWiiPowerup.Core.Strategies;
using Xunit;
using Xunit.Abstractions;

namespace bitOxide.MarioWiiPowerup.Tests
{
    public class StrategieTests
    {
        private readonly ITestOutputHelper output;

        public StrategieTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void TestDefaultStrat()
        {
            var strat = new FindSolutionWithLeastInputs();
            TestStrategy(strat);
        }

        [Fact]
        public void TestDefaultStrat2()
        {
            var strat = new FindSolutionWithLeastInputs2();
            TestStrategy(strat);
        }

        [Fact]
        public void TestDefaultStrat3()
        {
            var strat = new FindSolutionWithLeastInputs3();
            TestStrategy(strat);
        }

        [Fact]
        public void TestDefaultStrat4()
        {
            var strat = new FindSolutionWithLeastInputs4();
            TestStrategy(strat);
        }

        public void TestStrategy(ISuggestionStrategy strategy)
        {
            var allBoards = SuperMarioWiiBoards.DefaultBoardSet.AllBoards.ToArray();
            var suggestions = new List<int>();

            var itemCounts = new Dictionary<string, int>();

            foreach (var b in allBoards)
            {
                suggestions.Add(TestBoard(strategy, b, allBoards, itemCounts));
            }

            output.WriteLine($"Result: Min={suggestions.Min()}, Max={suggestions.Max()}, Avg={suggestions.Average()}");
            var suggestionDictionary = new Dictionary<int, int>();

            foreach (var suggestion in suggestions)
            {
                if (!suggestionDictionary.ContainsKey(suggestion))
                {
                    suggestionDictionary[suggestion] = 0;
                }

                suggestionDictionary[suggestion]++;
            }

            foreach (var suggestion in suggestionDictionary.Keys.OrderByDescending(x => x))
            {
                output.WriteLine($"  [{suggestion}]: {suggestionDictionary[suggestion]}");
            }

            output.WriteLine("");

            var bowserSum = itemCounts.Where(i => i.Key.Contains("Bowser")).Sum(i => i.Value);
            output.WriteLine($"Bowser Sum: {bowserSum}");
        }

        public int TestBoard(ISuggestionStrategy strategy, Board board, Board[] allBoards, IDictionary<string, int> itemCounts)
        {
            Item[] filledItems = new Item[SuperMarioWiiConstants.ItemsPerBoard];
            int suggestion = 0;

            while (allBoards.Where(x => x.Matches(filledItems)).Count() > 1)
            {
                var suggestionId = strategy.SuggestNextItemPosition(allBoards, filledItems);
                suggestion++;

                if (suggestionId != null)
                {
                    var item = board[suggestionId.Value];

                    if (!itemCounts.ContainsKey(item.Name))
                        itemCounts.Add(item.Name, 0);
                    itemCounts[item.Name]++;

                    filledItems[suggestionId.Value] = item;
                }
            }

            Assert.False(filledItems.Where(i => i == Item.Bowser).Count() > 1, "Lost because found two Bowsers");
            Assert.False((filledItems.Where(i => i == Item.MiniBowser).Count() > 1), "Lost because found two MiniBowsers");

            var solutions = allBoards.Where(x => x.Matches(filledItems)).ToArray();
            Assert.Single(solutions);
            return suggestion;
        }
    }
}
