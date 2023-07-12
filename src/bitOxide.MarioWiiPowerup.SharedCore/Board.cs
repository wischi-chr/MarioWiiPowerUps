using System;
using System.Linq;

namespace bitOxide.MarioWiiPowerup.Core
{
    public class Board
    {
        private readonly Item[] items;

        public Item this[int index]
        {
            get
            {
                if (index < 0 || index >= SuperMarioWiiConstants.ItemsPerBoard)
                {
                    throw new IndexOutOfRangeException();
                }

                return items[index];
            }
        }

        internal Board(params Item[] items)
        {
            if (!AreBoardItemsValid(items))
            {
                throw new ArgumentException();
            }

            this.items = (Item[])items.Clone();
        }

        /// <summary>
        /// Test if the given array of items represent a valid board.
        /// </summary>
        private static bool AreBoardItemsValid(Item[] items)
        {
            if (items == null || items.Length != SuperMarioWiiConstants.ItemsPerBoard)
            {
                // return false, if board doesn't have exactly 18 items
                return false;
            }

            if (items.Count(i => i == Item.Bowser) != 2)
            {
                // return false, if board doesn't have two bowser items
                return false;
            }

            if (items.Count(i => i == Item.MiniBowser) != 2)
            {
                // return false, if board doesn't have two mini-bowser items
                return false;
            }

            // return false, if there are items with an odd amount
            return items.Distinct().All(i => items.Count(si => si == i) % 2 == 0);
        }
    }
}
