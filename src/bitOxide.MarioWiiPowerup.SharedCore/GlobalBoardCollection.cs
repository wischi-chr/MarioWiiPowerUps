using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bitOxide.MarioWiiPowerup.Core
{
    public class GlobalBoardCollection : IEnumerable<WorldBoardCollection>
    {
        private readonly WorldBoardCollection[] worldBoards;

        public GlobalBoardCollection(WorldBoardCollection[] worldBoards)
        {
            if (worldBoards == null || worldBoards.Length != 9)
                throw new ArgumentOutOfRangeException();
            this.worldBoards = (WorldBoardCollection[])worldBoards.Clone();
        }

        public int Count => 9;
        public WorldBoardCollection this[int worldIndex] => worldBoards[worldIndex];

        public IEnumerator<WorldBoardCollection> GetEnumerator()
        {
            foreach (var wbc in worldBoards)
                yield return wbc;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public IEnumerable<Board> AllBoards
        {
            get
            {
                foreach (var world in worldBoards)
                    foreach (var board in world)
                        yield return board;
            }
        }
    }
}
