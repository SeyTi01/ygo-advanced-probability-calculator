namespace YGOProbabilityCalculator.ViewModel.Dialogs;

public class ComboCategory {
    public string Category { get; init; } = string.Empty;
    public int MinCount { get; init; } = 1;
    public int MaxCount { get; init; } = 1;
    public bool IsSelected { get; init; }
}
