using System.Collections.ObjectModel;
using System.Globalization;
using YGOProbabilityCalculator.Model;
using YGOProbabilityCalculator.Service;

namespace YGOProbabilityCalculatorTest.Service;

[TestFixture]
public class CategoryListConverterTest {
    private CategoryListConverter _converter;

    [SetUp]
    public void Setup() {
        _converter = new CategoryListConverter();
    }

    [Test]
    public void Convert_NullValue_ReturnsEmptyString() {
        var result = _converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.That(result, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Convert_EmptyList_ReturnsEmptyString() {
        var categories = new ObservableCollection<Category>();

        var result = _converter.Convert(categories, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.That(result, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Convert_SingleCategory_ReturnsFormattedString() {
        var categories = new ObservableCollection<Category> {
            new("Test", 1, 3)
        };

        var result = _converter.Convert(categories, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.That(result, Is.EqualTo("Test (1-3)"));
    }

    [Test]
    public void Convert_MultipleCategories_ReturnsCommaSeparatedString() {
        var categories = new ObservableCollection<Category> {
            new("First", 0, 2),
            new("Second", 1, 1),
            new("Third", 0, 3)
        };

        var result = _converter.Convert(categories, typeof(string), null, CultureInfo.InvariantCulture);

        Assert.That(result, Is.EqualTo("First (0-2), Second (1-1), Third (0-3)"));
    }
}
