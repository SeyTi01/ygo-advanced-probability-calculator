namespace YGOProbabilityCalculator.Views.EventArgs;

public class CategoryChangedEventArgs(string oldValue, string newValue) : System.EventArgs {
    public string OldValue { get; } = oldValue;
    public string NewValue { get; } = newValue;
}
