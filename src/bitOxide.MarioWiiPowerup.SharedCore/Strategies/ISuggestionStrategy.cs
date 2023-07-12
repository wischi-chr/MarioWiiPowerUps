namespace bitOxide.MarioWiiPowerup.Core.Strategies
{
    public interface ISuggestionStrategy
    {
        int? SuggestNextItemPosition(Board[] boards, Item[] filledItems);
    }
}
