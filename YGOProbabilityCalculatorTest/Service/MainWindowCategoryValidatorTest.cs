using System.Collections.ObjectModel;
using Moq;
using YGOProbabilityCalculator.Service;
using YGOProbabilityCalculator.Service.Interface;
using YGOProbabilityCalculator.ViewModel.Main;

namespace YGOProbabilityCalculatorTest.Service;

[TestFixture]
public class MainWindowCategoryValidatorTest {
    [Test]
    public void IsDuplicateCategory_WhenCategoryExists_ReturnsTrue() {
        var entries = new ObservableCollection<CategoryEntry> {
            new() { Category = "Existing" },
            new() { Category = "Test" }
        };

        var mockContainer = new Mock<ICategoryContainer>();
        mockContainer.Setup(m => m.CategoryEntries).Returns(entries);

        var validator = new MainWindowCategoryValidator(mockContainer.Object);
        var newEntry = new CategoryEntry { Category = "New" };

        var result = validator.IsDuplicateCategory(newEntry, "existing");

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsDuplicateCategory_WhenCheckingOwnCategory_ReturnsFalse() {
        var existingEntry = new CategoryEntry { Category = "Test" };
        var entries = new ObservableCollection<CategoryEntry> { existingEntry };

        var mockContainer = new Mock<ICategoryContainer>();
        mockContainer.Setup(m => m.CategoryEntries).Returns(entries);

        var validator = new MainWindowCategoryValidator(mockContainer.Object);

        var result = validator.IsDuplicateCategory(existingEntry, "Test");

        Assert.That(result, Is.False);
    }
}
