using System.Collections.ObjectModel;
using System.Windows;
using YGOProbabilityCalculator.ViewModel.Dialogs;

namespace YGOProbabilityCalculator.Views.Dialogs;

public partial class ComboCategorySelectorWindow {
    public ObservableCollection<ComboCategory> Categories { get; } = [];

    public ComboCategorySelectorWindow(IEnumerable<string> availableCategories,
        Dictionary<string, (int Min, int Max)> selectedCategories, int handSize) {
        InitializeComponent();
        DataContext = this;

        foreach (var category in availableCategories) {
            var isSelected = selectedCategories.ContainsKey(category);
            var (min, max) = isSelected ? selectedCategories[category] : (1, handSize);

            Categories.Add(new ComboCategory {
                Category = category,
                IsSelected = isSelected,
                MinCount = min,
                MaxCount = max
            });
        }
    }

    public Dictionary<string, (int Min, int Max)> GetSelectedCategories() {
        return Categories
            .Where(c => c.IsSelected)
            .ToDictionary(
                c => c.Category,
                c => (c.MinCount, c.MaxCount)
            );
    }

    private void OkButton_Click(object sender, RoutedEventArgs e) {
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) {
        DialogResult = false;
        Close();
    }
}
