using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bitOxide.MarioWiiPowerup.Core
{
    public class Board
    {
        private readonly Item[] items;

        public Item this[int index]
        {
            get
            {
                if (index < 0 || index >= 18) throw new IndexOutOfRangeException();
                return items[index];
            }
        }

        internal Board(params Item[] items)
        {
            if (!AreBoardItemsValid(items))
                throw new ArgumentException();

            this.items = (Item[])items.Clone();
        }

        private static bool AreBoardItemsValid(Item[] items)
        {
            if (items == null || items.Length != 18)
                return false;

            //Weitere Bestimmungen für ein Board
            if (items.Count(i => i == Item.Bowser) != 2) return false;
            if (items.Count(i => i == Item.MiniBowser) != 2) return false;

            //und die Anzahl der Elemente muss immer durch zwei teilbar sein
            if (!items.Distinct().All(i => items.Count(si => si == i) % 2 == 0)) return false;

            return true;
        }
    }
}
