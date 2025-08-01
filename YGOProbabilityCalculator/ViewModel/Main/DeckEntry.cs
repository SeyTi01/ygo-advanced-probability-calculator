using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using YGOProbabilityCalculator.Model;

namespace YGOProbabilityCalculator.ViewModel.Main;

public sealed class DeckEntry : INotifyPropertyChanged {
    private int _copies = 1;
    private readonly ObservableCollection<Category> _categories = [];

    public required string Name { get; set; }

    public ObservableCollection<Category> Categories {
        get => _categories;
        init {
            _categories = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CategoriesString));
        }
    }

    public string CategoriesString => string.Join(", ", Categories.Select(c => c.Name));

    public int Copies {
        get => _copies;
        set {
            if (_copies == value) return;
            _copies = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
