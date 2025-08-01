using System.Collections.ObjectModel;
using YGOProbabilityCalculator.Model;
using YGOProbabilityCalculator.ViewModel.Main;
using YGOProbabilityCalculator.Views.Dialogs;

namespace YGOProbabilityCalculator.Service;

public static class CategoryService {
    public static void RenameCategoryInEntries(
        ObservableCollection<DeckEntry> deckEntries,
        ObservableCollection<ComboEntry> comboEntries,
        string oldCategory,
        string newCategory) {
        foreach (var deck in deckEntries) {
            var updatedCategories = deck.Categories
                .Select(c => c.Name == oldCategory
                    ? new Category(newCategory, c.MinCount, c.MaxCount)
                    : c)
                .ToList();
            deck.Categories.Clear();
            foreach (var category in updatedCategories)
                deck.Categories.Add(category);
        }

        foreach (var combo in comboEntries) {
            var updatedCategories = combo.Categories
                .Select(c => c.Name == oldCategory
                    ? new Category(newCategory, c.MinCount, c.MaxCount)
                    : c)
                .ToList();
            combo.Categories.Clear();
            foreach (var category in updatedCategories)
                combo.Categories.Add(category);
        }
    }

    public static void RemoveCategoryFromEntries(
        ObservableCollection<DeckEntry> deckEntries,
        ObservableCollection<ComboEntry> comboEntries,
        string category) {
        if (string.IsNullOrWhiteSpace(category)) return;

        foreach (var deck in deckEntries) {
            var updatedCategories = deck.Categories
                .Where(c => !string.Equals(c.Name, category, StringComparison.OrdinalIgnoreCase))
                .ToList();
            deck.Categories.Clear();
            foreach (var c in updatedCategories)
                deck.Categories.Add(c);
        }

        foreach (var combo in comboEntries) {
            var updatedCategories = combo.Categories
                .Where(c => !string.Equals(c.Name, category, StringComparison.OrdinalIgnoreCase))
                .ToList();
            combo.Categories.Clear();
            foreach (var c in updatedCategories)
                combo.Categories.Add(c);
        }
    }

    public static bool TrySelectCategories(
        IEnumerable<string> allCategories,
        ObservableCollection<Category> currentCategories,
        out ObservableCollection<Category> newCategories) {
        var currentCats = currentCategories.Select(c => c.Name).ToList();
        var dialog = new CategorySelectorWindow(allCategories.ToList(), currentCats);
        if (dialog.ShowDialog() == true) {
            newCategories = new ObservableCollection<Category>(
                dialog.SelectedCategories.Select(c => new Category(c, 1, 1))
            );
            return true;
        }

        newCategories = [];
        return false;
    }
}
