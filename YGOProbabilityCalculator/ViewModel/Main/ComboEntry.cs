namespace YGOProbabilityCalculator.ViewModel.Main;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Model;

public sealed class ComboEntry : INotifyPropertyChanged {
    private string _name = string.Empty;
    private readonly ObservableCollection<Category> _categories = [];

    public string Name {
        get => _name;
        set {
            if (_name == value) return;
            _name = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Category> Categories {
        get => _categories;
        init {
            _categories = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
