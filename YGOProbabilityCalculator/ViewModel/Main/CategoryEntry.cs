using System.ComponentModel;
using YGOProbabilityCalculator.Views.EventArgs;

namespace YGOProbabilityCalculator.ViewModel.Main;

public class CategoryEntry : INotifyPropertyChanged {
    private string _category = string.Empty;

    public string Category {
        get => _category;
        set {
            if (_category == value) return;
            var old = _category;
            _category = value;
            CategoryChanged?.Invoke(this, new CategoryChangedEventArgs(old, value));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Category)));
        }
    }

    public int MinCount { get; set; }
    public int MaxCount { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<CategoryChangedEventArgs>? CategoryChanged;
}
