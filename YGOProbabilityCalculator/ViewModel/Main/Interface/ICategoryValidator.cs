namespace YGOProbabilityCalculator.ViewModel.Main.Interface;

public interface ICategoryValidator {
    bool IsDuplicateCategory(CategoryEntry entry, string categoryName);
}
