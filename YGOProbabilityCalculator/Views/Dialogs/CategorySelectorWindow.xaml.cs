using System.Windows;
using YGOProbabilityCalculator.Views.Controls;

namespace YGOProbabilityCalculator.Views.Dialogs;

public partial class CategorySelectorWindow {
    public List<string> SelectedCategories { get; } = [];

    public CategorySelectorWindow(IEnumerable<string> allCategories,
        IEnumerable<string> initiallySelected) {
        InitializeComponent();
        var items = allCategories
            .Select(cat => new CategoryCheckbox {
                Name = cat,
                IsChecked = initiallySelected.Contains(cat)
            })
            .ToList();
        CategoriesItemsControl.ItemsSource = items;
    }

    private void OkButton_Click(object sender, RoutedEventArgs e) {
        SelectedCategories.Clear();
        foreach (CategoryCheckbox cb in CategoriesItemsControl.Items)
            if (cb.IsChecked)
                SelectedCategories.Add(cb.Name);

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) {
        DialogResult = false;
        Close();
    }
}
