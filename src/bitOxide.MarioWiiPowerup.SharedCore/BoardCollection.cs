using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bitOxide.MarioWiiPowerup.Core
{
    public class BoardCollection : IEnumerable<Board>
    {
        private readonly Board[] boards;

        public int Count => boards.Length;
        public Board this[int Index] => boards[Index];

        public BoardCollection(IEnumerable<Board> boards)
        {
            if (boards == null)
                throw new ArgumentNullException();

            this.boards = boards.ToArray();
        }

        public IEnumerator<Board> GetEnumerator()
        {
            foreach (var b in boards)
                yield return b;
        }

        IEnumerator IEnumerable.GetEnumerator() => boards.GetEnumerator();
    }
}
