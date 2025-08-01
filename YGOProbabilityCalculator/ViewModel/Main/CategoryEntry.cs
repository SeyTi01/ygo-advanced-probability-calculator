using System.ComponentModel;
using System.Text.Json.Serialization;
using YGOProbabilityCalculator.Views.EventArgs;
using YGOProbabilityCalculator.ViewModel.Main.Interface;

namespace YGOProbabilityCalculator.ViewModel.Main;

public class CategoryEntry(ICategoryValidator? categoryValidator) : INotifyPropertyChanged {
    [JsonIgnore] private readonly ICategoryValidator? _categoryValidator = categoryValidator;
    private string _category = string.Empty;
    private string _pendingCategory = string.Empty;

    [JsonConstructor]
    public CategoryEntry() : this(null) {
    }

    public string Category {
        get => _category;
        set {
            if (_category == value) return;

            _pendingCategory = value;

            if (_categoryValidator?.IsDuplicateCategory(this, _pendingCategory) != true) {
                var old = _category;
                _category = value;
                CategoryChanged?.Invoke(this, new CategoryChangedEventArgs(old, value));
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Category)));
        }
    }

    public int MinCount { get; set; }
    public int MaxCount { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<CategoryChangedEventArgs>? CategoryChanged;
}
