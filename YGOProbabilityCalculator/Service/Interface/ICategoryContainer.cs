using System.Collections.ObjectModel;
using YGOProbabilityCalculator.ViewModel.Main;

namespace YGOProbabilityCalculator.Service.Interface;

public interface ICategoryContainer {
    ObservableCollection<CategoryEntry> CategoryEntries { get; }
}
