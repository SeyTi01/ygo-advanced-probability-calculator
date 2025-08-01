using YGOProbabilityCalculator.Service.Interface;
using YGOProbabilityCalculator.ViewModel.Main;
using YGOProbabilityCalculator.ViewModel.Main.Interface;

namespace YGOProbabilityCalculator.Service;

public class MainWindowCategoryValidator(ICategoryContainer categoryContainer) : ICategoryValidator {
    public bool IsDuplicateCategory(CategoryEntry entry, string categoryName) {
        return categoryContainer.CategoryEntries
            .Where(c => c != entry)
            .Any(c => c.Category.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
    }
}
