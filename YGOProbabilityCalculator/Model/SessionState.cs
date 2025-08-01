using YGOProbabilityCalculator.ViewModel.Main;

namespace YGOProbabilityCalculator.Model;

public class SessionState {
    public int HandSize { get; init; }
    public List<DeckEntry> DeckEntries { get; init; } = [];
    public List<CategoryEntry> CategoryEntries { get; init; } = [];
    public List<ComboEntry> ComboEntries { get; init; } = [];
    public string ResultText { get; init; } = string.Empty;
}
