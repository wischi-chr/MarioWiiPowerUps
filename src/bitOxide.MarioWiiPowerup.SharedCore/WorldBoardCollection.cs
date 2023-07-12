using System;

namespace bitOxide.MarioWiiPowerup.Core
{
    public class WorldBoardCollection : BoardCollection
    {
        internal WorldBoardCollection(params Board[] boards)
            : base(boards)
        {
            if (boards.Length != SuperMarioWiiConstants.BoardsPerWorld)
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}
