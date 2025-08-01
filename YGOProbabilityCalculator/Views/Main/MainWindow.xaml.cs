using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using YGOProbabilityCalculator.Model;
using YGOProbabilityCalculator.Service;
using YGOProbabilityCalculator.Service.Interface;
using YGOProbabilityCalculator.ViewModel.Main;
using YGOProbabilityCalculator.ViewModel.Main.Interface;
using YGOProbabilityCalculator.Views.Dialogs;
using YGOProbabilityCalculator.Views.EventArgs;

namespace YGOProbabilityCalculator.Views.Main;

public sealed partial class MainWindow : INotifyPropertyChanged, ICategoryContainer {
    private readonly DeckImportService _deckImportService;
    private readonly SessionService _sessionService;
    private readonly ICategoryValidator _categoryValidator;
    private CalculationMode _mode = CalculationMode.Default;
    private int _deckCount;
    private bool _isCalculating;
    private CancellationTokenSource? _calculationCts;

    private enum CalculationMode {
        Default,
        Simple
    }

    public ObservableCollection<DeckEntry> DeckEntries { get; } = [];
    public ObservableCollection<CategoryEntry> CategoryEntries { get; } = [];
    public ObservableCollection<ComboEntry> ComboEntries { get; } = [];

    public int DeckCount {
        get => _deckCount;
        private set {
            if (_deckCount == value) return;
            _deckCount = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public MainWindow() {
        var fileService = new FileService();
        var jsonSerializer = new JsonSerializer();
        var cardInfoService = new CardInfoService(fileService, jsonSerializer);
        _deckImportService = new DeckImportService(cardInfoService);
        _sessionService = new SessionService(fileService, jsonSerializer);
        _categoryValidator = new MainWindowCategoryValidator(this);

        InitializeComponent();
        DataContext = this;

        Loaded += MainWindow_Loaded;

        CategoryEntries.CollectionChanged += CategoryEntriesCollectionChanged;
        foreach (var entry in CategoryEntries)
            entry.CategoryChanged += CategoryEntry_CategoryChanged;

        DeckEntries.CollectionChanged += DeckEntries_CollectionChanged;
        DeckEntries.CollectionChanged += (_, e) => {
            if (e.NewItems != null)
                foreach (DeckEntry entry in e.NewItems)
                    entry.PropertyChanged += DeckEntry_PropertyChanged;
            if (e.OldItems != null)
                foreach (DeckEntry entry in e.OldItems)
                    entry.PropertyChanged -= DeckEntry_PropertyChanged;
        };

        UpdateDeckCount();
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
        UpdateModeUI();
    }

    private async void LoadDeckButton_Click(object sender, RoutedEventArgs e) {
        var path = ShowOpenFileDialog("Select YDK Deck File", "YDK Deck Files (*.ydk)|*.ydk|All Files (*.*)|*.*");
        if (path == null) return;

        try {
            var deck = await _deckImportService.ImportDeckFromYdkAsync(path);
            PopulateCollection(DeckEntries, deck);
            UpdateDeckCount();
        }
        catch (Exception ex) {
            ShowError($"Failed to load deck: {ex.Message}");
        }
    }

    private async void LoadSessionButton_Click(object sender, RoutedEventArgs e) {
        var path = ShowOpenFileDialog("Load Calculator Session", "JSON Files (*.json)|*.json|All Files (*.*)|*.*");
        if (path == null) return;

        try {
            var state = await _sessionService.LoadSessionAsync(path);

            PopulateCollection(DeckEntries, state.DeckEntries);
            PopulateCollection(CategoryEntries, state.CategoryEntries);
            PopulateCollection(ComboEntries, state.ComboEntries);
            UpdateDeckCount();

            HandSizeTextBox.Text = state.HandSize.ToString();
            ResultTextBlock.Text = state.ResultText;
        }
        catch (Exception ex) {
            ShowError($"Failed to load session: {ex.Message}");
        }
    }

    private async void SaveSessionButton_Click(object sender, RoutedEventArgs e) {
        var path = ShowSaveFileDialog("Save Calculator Session", "JSON Files (*.json)|*.json|All Files (*.*)|*.*");
        if (path == null) return;

        try {
            var state = new SessionState {
                HandSize = int.TryParse(HandSizeTextBox.Text, out var hs) ? hs : 0,
                DeckEntries = DeckEntries.ToList(),
                CategoryEntries = CategoryEntries.ToList(),
                ComboEntries = ComboEntries.ToList(),
                ResultText = ResultTextBlock.Text
            };
            await _sessionService.SaveSessionAsync(path, state);
        }
        catch (Exception ex) {
            ShowError($"Failed to save session: {ex.Message}");
        }
    }

    private void UpdateCalculationState(bool isCalculating) {
        _isCalculating = isCalculating;
        CalculateProbabilityButton.IsEnabled = !isCalculating;
        CalculateCombosButton.IsEnabled = !isCalculating;
        LoadingOverlay.Visibility = isCalculating ? Visibility.Visible : Visibility.Collapsed;

        if (isCalculating)
            _calculationCts = new CancellationTokenSource();
        else
            _calculationCts?.Dispose();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) {
        _calculationCts?.Cancel();
    }

    private bool ValidateCategoryNames() {
        var duplicateCategories = CategoryEntries
            .GroupBy(c => c.Category.Trim())
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateCategories.Count == 0) return true;

        ShowError($"Duplicate category names found: {string.Join(", ", duplicateCategories)}", "Category Error");

        return false;
    }

    private async void CalculateButton_Click(object sender, RoutedEventArgs e) {
        if (_isCalculating) return;
        if (!TryGetHandSize(out var handSize)) return;
        if (!ValidateCategoryNames()) return;
        var cards = GetCards();

        List<Category> categories;
        try {
            categories = CategoryEntries
                .Select(entry => new Category(
                    entry.Category.Trim(),
                    entry.MinCount,
                    entry.MaxCount))
                .ToList();
        }
        catch (ArgumentException ae) {
            MessageBox.Show(ae.Message, "Category Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try {
            UpdateCalculationState(true);
            var calculator = new ProbabilityCalculator(cards);
            var probability = await Task.Run(() =>
                    calculator.CalculateProbabilityRange(categories, handSize, _calculationCts!.Token),
                _calculationCts!.Token);
            ResultTextBlock.Text = $"Probability: {probability:P2}";
        }
        catch (OperationCanceledException) {
            ResultTextBlock.Text = "Calculation cancelled";
        }
        catch (Exception ex) {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally {
            UpdateCalculationState(false);
        }
    }

    private async void CalculateCombosButton_Click(object sender, RoutedEventArgs e) {
        if (_isCalculating) return;
        if (!TryGetHandSize(out var handSize)) return;
        if (!ValidateCategoryNames()) return;
        var cards = GetCards();

        List<Combo> combos;
        try {
            combos = ComboEntries
                .Select(ce => new Combo(ce.Categories))
                .ToList();
        }
        catch (ArgumentException ae) {
            MessageBox.Show(ae.Message, "Combo Category Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try {
            UpdateCalculationState(true);
            var calculator = new ProbabilityCalculator(cards);
            var prob = await Task.Run(() =>
                    calculator.CalculateProbabilityForCombos(combos, handSize, _calculationCts!.Token),
                _calculationCts!.Token);
            ResultTextBlock.Text = $"Probability: {prob:P2}";
        }
        catch (OperationCanceledException) {
            ResultTextBlock.Text = "Calculation cancelled";
        }
        catch (Exception ex) {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally {
            UpdateCalculationState(false);
        }
    }

    private void EditCategoriesButton_Click(object sender, RoutedEventArgs e) {
        CommitDataGridEdits(DeckDataGrid);
        if (sender is not Button { Tag: DeckEntry entry }) return;

        var allCats = CategoryEntries
            .Select(r => r.Category)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .ToList();

        if (!CategoryService.TrySelectCategories(allCats, entry.Categories, out var newCategories)) return;

        entry.Categories.Clear();
        foreach (var category in newCategories)
            entry.Categories.Add(category);

        DeckDataGrid.Items.Refresh();
    }

    private void EditComboCategoriesButton_Click(object sender, RoutedEventArgs e) {
        CommitDataGridEdits(ComboDataGrid);
        if (sender is not Button { Tag: ComboEntry entry }) return;

        var allCats = CategoryEntries
            .Select(r => r.Category)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .ToList();

        if (!TryGetHandSize(out var handSize)) return;

        var existingCategories = entry.Categories
            .ToDictionary(
                c => c.Name,
                c => (c.MinCount, c.MaxCount)
            );

        var window = new ComboCategorySelectorWindow(allCats, existingCategories, handSize);
        if (window.ShowDialog() != true) return;

        var selectedCategories = window.GetSelectedCategories();
        entry.Categories.Clear();
        foreach (var kvp in selectedCategories)
            entry.Categories.Add(new Category(kvp.Key, kvp.Value.Min, kvp.Value.Max));

        ComboDataGrid.Items.Refresh();
    }

    private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        _mode = ModeComboBox.SelectedIndex == 0
            ? CalculationMode.Default
            : CalculationMode.Simple;
        UpdateModeUI();
    }

    private void DeleteDeckEntry_Click(object sender, RoutedEventArgs e) {
        if (sender is Button { Tag: DeckEntry entry })
            DeckEntries.Remove(entry);
    }

    private void DeleteCategoryEntry_Click(object sender, RoutedEventArgs e) {
        if (sender is not Button { Tag: CategoryEntry entry }) return;
        CategoryService.RemoveCategoryFromEntries(DeckEntries, ComboEntries, entry.Category);
        CategoryEntries.Remove(entry);
        DeckDataGrid.Items.Refresh();
        ComboDataGrid.Items.Refresh();
    }

    private void DeleteComboEntry_Click(object sender, RoutedEventArgs e) {
        if (sender is Button { Tag: ComboEntry entry })
            ComboEntries.Remove(entry);
    }

    private void CategoryEntriesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.NewItems != null)
            foreach (CategoryEntry entry in e.NewItems)
                entry.CategoryChanged += CategoryEntry_CategoryChanged;

        if (e.OldItems != null)
            foreach (CategoryEntry entry in e.OldItems) {
                entry.CategoryChanged -= CategoryEntry_CategoryChanged;
                CategoryService.RemoveCategoryFromEntries(DeckEntries, ComboEntries, entry.Category);
            }
    }

    private void CategoryEntry_CategoryChanged(object? sender, CategoryChangedEventArgs e) {
        CategoryService.RenameCategoryInEntries(
            DeckEntries,
            ComboEntries,
            e.OldValue,
            e.NewValue
        );

        DeckDataGrid.Items.Refresh();
        ComboDataGrid.Items.Refresh();
    }

    private void DeckEntries_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        UpdateDeckCount();
    }

    private void DeckEntry_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(DeckEntry.Copies)) {
            UpdateDeckCount();
        }
    }

    private void UpdateModeUI() {
        var isComboMode = _mode == CalculationMode.Default;

        if (CombosGroupBox != null)
            CombosGroupBox.Visibility = isComboMode
                ? Visibility.Visible
                : Visibility.Collapsed;

        if (DeckRowDefinition != null && CategoriesRowDefinition != null) {
            if (isComboMode) {
                DeckRowDefinition.Height = new GridLength(2, GridUnitType.Star);
                CategoriesRowDefinition.Height = new GridLength(2, GridUnitType.Star);
            }
            else {
                DeckRowDefinition.Height = new GridLength(6, GridUnitType.Star);
                CategoriesRowDefinition.Height = new GridLength(6, GridUnitType.Star);
            }
        }

        if (CalculateCombosButton != null)
            CalculateCombosButton.Visibility = isComboMode
                ? Visibility.Visible
                : Visibility.Collapsed;

        if (CalculateProbabilityButton != null)
            CalculateProbabilityButton.Visibility = !isComboMode
                ? Visibility.Visible
                : Visibility.Collapsed;

        if (CategoriesDataGrid == null)
            return;

        foreach (var header in new[] { "Min", "Max" }) {
            var col = CategoriesDataGrid.Columns
                .FirstOrDefault(c => (c.Header?.ToString() ?? "") == header);
            if (col != null)
                col.Visibility = !isComboMode
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
    }

    private void UpdateDeckCount() {
        DeckCount = DeckEntries.Sum(entry => entry.Copies);
    }

    private bool TryGetHandSize(out int handSize) {
        if (int.TryParse(HandSizeTextBox.Text, out handSize))
            return true;

        MessageBox.Show("Invalid hand size.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return false;
    }

    private List<Card> GetCards() {
        return DeckEntries
            .Select(entry => new Card(
                entry.Copies,
                entry.Categories.Select(c => c.Name).ToArray()
            ))
            .ToList();
    }

    private static void CommitDataGridEdits(DataGrid grid) {
        grid.CommitEdit(DataGridEditingUnit.Cell, true);
        grid.CommitEdit(DataGridEditingUnit.Row, true);
    }

    private static string? ShowOpenFileDialog(string title, string filter) {
        var dlg = new OpenFileDialog { Title = title, Filter = filter };
        return dlg.ShowDialog() == true ? dlg.FileName : null;
    }

    private static string? ShowSaveFileDialog(string title, string filter) {
        var dlg = new SaveFileDialog { Title = title, Filter = filter };
        return dlg.ShowDialog() == true ? dlg.FileName : null;
    }

    private static void ShowError(string message, string title = "Error") {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void PopulateCollection<T>(ObservableCollection<T> collection, IEnumerable<T> items) {
        collection.Clear();
        foreach (var item in items) {
            if (item is CategoryEntry categoryEntry) {
                var newEntry = new CategoryEntry(_categoryValidator) {
                    Category = categoryEntry.Category,
                    MinCount = categoryEntry.MinCount,
                    MaxCount = categoryEntry.MaxCount
                };
                collection.Add((T)(object)newEntry);
            }
            else {
                collection.Add(item);
            }
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
