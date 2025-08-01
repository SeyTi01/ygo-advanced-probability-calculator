using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using YGOProbabilityCalculator.Model;

namespace YGOProbabilityCalculator.Service;

public class CategoryListConverter : IValueConverter {
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is ObservableCollection<Category> categories) {
            return string.Join(", ", categories.Select(c => $"{c.Name} ({c.MinCount}-{c.MaxCount})"));
        }

        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
