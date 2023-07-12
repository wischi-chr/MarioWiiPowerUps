using System;

namespace bitOxide.MarioWiiPowerup.Core
{
    public static class BoardExtensions
    {
        public static bool Matches(this Board b, Item[] filter)
        {
            if (filter == null) throw new ArgumentNullException();
            if (filter.Length != SuperMarioWiiConstants.ItemsPerBoard) throw new ArgumentException();

            for (int i = 0; i < SuperMarioWiiConstants.ItemsPerBoard; i++)
            {
                if (filter[i] == null) continue;
                if (b[i] != filter[i]) return false;
            }

            return true;
        }
    }
}
