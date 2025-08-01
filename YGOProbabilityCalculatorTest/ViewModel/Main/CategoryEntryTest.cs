using YGOProbabilityCalculator.Views.EventArgs;
using Moq;
using YGOProbabilityCalculator.ViewModel.Main;
using YGOProbabilityCalculator.ViewModel.Main.Interface;

namespace YGOProbabilityCalculatorTest.ViewModel.Main;

[TestFixture]
public class CategoryEntryTest {
    private CategoryEntry _categoryEntry = null!;
    private Mock<ICategoryValidator> _validatorMock = null!;

    [SetUp]
    public void SetUp() {
        _validatorMock = new Mock<ICategoryValidator>();
        _categoryEntry = new CategoryEntry(_validatorMock.Object);
    }

    [Test]
    public void Constructor_InitializesWithEmptyCategory() {
        var entry = new CategoryEntry();
        Assert.That(entry.Category, Is.Empty);
    }

    [Test]
    public void Category_WhenSet_RaisesPropertyChangedEvent() {
        var propertyChangedRaised = false;
        _categoryEntry.PropertyChanged += (_, args) => {
            if (args.PropertyName == nameof(CategoryEntry.Category))
                propertyChangedRaised = true;
        };

        _categoryEntry.Category = "Test";

        Assert.That(propertyChangedRaised, Is.True);
    }

    [Test]
    public void Category_WhenChanged_RaisesCategoryChangedEvent() {
        CategoryChangedEventArgs? receivedArgs = null;
        _categoryEntry.CategoryChanged += (_, args) => receivedArgs = args;

        _categoryEntry.Category = "NewCategory";

        Assert.Multiple(() => {
            Assert.That(receivedArgs, Is.Not.Null);
            Assert.That(receivedArgs!.OldValue, Is.Empty);
            Assert.That(receivedArgs.NewValue, Is.EqualTo("NewCategory"));
        });
    }

    [Test]
    public void Category_WhenSetToSameValue_DoesNotRaiseEvents() {
        _categoryEntry.Category = "Initial";

        var propertyChangedRaised = false;
        var categoryChangedRaised = false;

        _categoryEntry.PropertyChanged += (_, _) => propertyChangedRaised = true;
        _categoryEntry.CategoryChanged += (_, _) => categoryChangedRaised = true;

        _categoryEntry.Category = "Initial";

        Assert.Multiple(() => {
            Assert.That(propertyChangedRaised, Is.False);
            Assert.That(categoryChangedRaised, Is.False);
        });
    }

    [Test]
    public void Category_WhenDuplicateExists_PreventsCategoryChange() {
        _validatorMock.Setup(v => v.IsDuplicateCategory(_categoryEntry, "Duplicate"))
            .Returns(true);

        _categoryEntry.Category = "Duplicate";

        Assert.That(_categoryEntry.Category, Is.Empty);
    }

    [Test]
    public void Category_WhenNoDuplicateExists_AllowsCategoryChange() {
        _validatorMock.Setup(v => v.IsDuplicateCategory(_categoryEntry, "Unique"))
            .Returns(false);

        _categoryEntry.Category = "Unique";

        Assert.That(_categoryEntry.Category, Is.EqualTo("Unique"));
    }

    [Test]
    public void Category_WithNullValidator_AllowsCategoryChange() {
        var entry = new CategoryEntry {
            Category = "Test"
        };
        
        Assert.That(entry.Category, Is.EqualTo("Test"));
    }

    [Test]
    public void MinCount_CanBeSetAndRetrieved() {
        _categoryEntry.MinCount = 5;
        Assert.That(_categoryEntry.MinCount, Is.EqualTo(5));
    }

    [Test]
    public void MaxCount_CanBeSetAndRetrieved() {
        _categoryEntry.MaxCount = 10;
        Assert.That(_categoryEntry.MaxCount, Is.EqualTo(10));
    }
}
