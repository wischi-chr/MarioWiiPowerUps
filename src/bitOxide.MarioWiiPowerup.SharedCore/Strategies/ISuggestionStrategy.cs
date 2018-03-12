using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bitOxide.MarioWiiPowerup.Core.Strategies
{
    public interface ISuggestionStrategy
    {
        int? SuggestNextItemPosition(Board[] boards, Item[] filledItems);
    }
}
