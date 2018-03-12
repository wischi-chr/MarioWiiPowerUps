using System;

namespace bitOxide.MarioWiiPowerup.Core
{
    public class WorldBoardCollection : BoardCollection
    {
        internal WorldBoardCollection(params Board[] boards) : base(boards)
        {
            if (boards.Length != 6) throw new IndexOutOfRangeException();
        }
    }
}
