namespace FinalProject.BL
{
    public class ItemSuggestion
    {
        public string SuggestionText { get; set; }
        public string Source { get; set; }
        public decimal SuggestedPrice { get; set; }
        public int Priority { get; set; }

        public ItemSuggestion() { }

        public ItemSuggestion(string suggestionText, string source, decimal suggestedPrice, int priority)
        {
            SuggestionText = suggestionText;
            Source = source;
            SuggestedPrice = suggestedPrice;
            Priority = priority;
        }
    }
} 