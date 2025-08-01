namespace YGOProbabilityCalculator.Model;

public class Card(int copies = 1, params string[] categories) {
    public int Copies { get; } = copies;
    public HashSet<string> Categories { get; } = [.. categories];
}
