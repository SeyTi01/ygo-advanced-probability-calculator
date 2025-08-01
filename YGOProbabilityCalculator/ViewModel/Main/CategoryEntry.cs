using System.ComponentModel;
using YGOProbabilityCalculator.ViewModel.Main.Interface;
using YGOProbabilityCalculator.Views.EventArgs;

namespace YGOProbabilityCalculator.ViewModel.Main;

public class CategoryEntry(ICategoryValidator? categoryValidator = null) : INotifyPropertyChanged {
    private string _category = string.Empty;
    private string _pendingCategory = string.Empty;

    public string Category {
        get => _category;
        set {
            if (_category == value) return;

            _pendingCategory = value;

            if (categoryValidator == null || !categoryValidator.IsDuplicateCategory(this, _pendingCategory)) {
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
