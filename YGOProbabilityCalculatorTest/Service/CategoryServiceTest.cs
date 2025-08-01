using System.Collections.ObjectModel;
using YGOProbabilityCalculator.Model;
using YGOProbabilityCalculator.Service;
using YGOProbabilityCalculator.ViewModel.Main;

namespace YGOProbabilityCalculatorTest.Service;

[TestFixture]
public class CategoryServiceTests {
    private ObservableCollection<DeckEntry> _deckEntries;
    private ObservableCollection<ComboEntry> _comboEntries;

    [SetUp]
    public void Setup() {
        _deckEntries = [];
        _comboEntries = [];
    }

    [Test]
    public void RenameCategoryInEntries_EmptyCollections_NoException() {
        Assert.DoesNotThrow(() => CategoryService.RenameCategoryInEntries(
            _deckEntries,
            _comboEntries,
            "OldCategory",
            "NewCategory"));
    }

    [Test]
    public void RenameCategoryInEntries_RenamesDeckEntryCategories() {
        var deckEntry = new DeckEntry {
            Name = "Test Card",
            Categories = [
                new Category("CategoryA", 1, 2),
                new Category("CategoryB", 0, 1)
            ]
        };
        
        _deckEntries.Add(deckEntry);

        CategoryService.RenameCategoryInEntries(_deckEntries, _comboEntries, "CategoryA", "NewCategory");

        Assert.Multiple(() => {
            Assert.That(deckEntry.Categories.First().Name, Is.EqualTo("NewCategory"));
            Assert.That(deckEntry.Categories.Last().Name, Is.EqualTo("CategoryB"));
        });
    }

    [Test]
    public void RenameCategoryInEntries_RenamesComboEntryCategories() {
        var comboEntry = new ComboEntry {
            Categories = [
                new Category("CategoryA", 1, 2),
                new Category("CategoryB", 0, 1)
            ]
        };
        
        _comboEntries.Add(comboEntry);

        CategoryService.RenameCategoryInEntries(_deckEntries, _comboEntries, "CategoryA", "NewCategory");

        Assert.Multiple(() => {
            Assert.That(comboEntry.Categories.First().Name, Is.EqualTo("NewCategory"));
            Assert.That(comboEntry.Categories.Last().Name, Is.EqualTo("CategoryB"));
        });
    }

    [Test]
    public void RemoveCategoryFromEntries_NullOrEmptyCategory_NoChanges() {
        var deckEntry = new DeckEntry {
            Name = "Test Card",
            Categories = [new Category("CategoryA", 1, 2)]
        };
        
        _deckEntries.Add(deckEntry);

        CategoryService.RemoveCategoryFromEntries(_deckEntries, _comboEntries, "");
        CategoryService.RemoveCategoryFromEntries(_deckEntries, _comboEntries, null!);

        Assert.That(deckEntry.Categories, Has.Count.EqualTo(1));
        Assert.That(deckEntry.Categories.First().Name, Is.EqualTo("CategoryA"));
    }

    [Test]
    public void RemoveCategoryFromEntries_RemovesFromDeckEntries() {
        var deckEntry = new DeckEntry {
            Name = "Test Card",
            Categories = [
                new Category("CategoryA", 1, 2),
                new Category("CategoryB", 0, 1)
            ]
        };
        
        _deckEntries.Add(deckEntry);

        CategoryService.RemoveCategoryFromEntries(_deckEntries, _comboEntries, "CategoryA");

        Assert.That(deckEntry.Categories, Has.Count.EqualTo(1));
        Assert.That(deckEntry.Categories.First().Name, Is.EqualTo("CategoryB"));
    }

    [Test]
    public void RemoveCategoryFromEntries_RemovesFromComboEntries() {
        var comboEntry = new ComboEntry {
            Categories = [
                new Category("CategoryA", 1, 2),
                new Category("CategoryB", 0, 1)
            ]
        };
        
        _comboEntries.Add(comboEntry);

        CategoryService.RemoveCategoryFromEntries(_deckEntries, _comboEntries, "CategoryA");

        Assert.That(comboEntry.Categories, Has.Count.EqualTo(1));
        Assert.That(comboEntry.Categories.First().Name, Is.EqualTo("CategoryB"));
    }

    [Test]
    public void RemoveCategoryFromEntries_CaseInsensitive() {
        var deckEntry = new DeckEntry {
            Name = "Test Card",
            Categories = [
                new Category("CategoryA", 1, 2),
                new Category("CategoryB", 0, 1)
            ]
        };
        
        _deckEntries.Add(deckEntry);

        CategoryService.RemoveCategoryFromEntries(_deckEntries, _comboEntries, "categorya");

        Assert.That(deckEntry.Categories, Has.Count.EqualTo(1));
        Assert.That(deckEntry.Categories.First().Name, Is.EqualTo("CategoryB"));
    }

    [Test]
    public void RenameCategoryInEntries_PreservesMinMaxValues() {
        var deckEntry = new DeckEntry {
            Name = "Test Card",
            Categories = [new Category("CategoryA", 2, 3)]
        };
        
        _deckEntries.Add(deckEntry);

        CategoryService.RenameCategoryInEntries(_deckEntries, _comboEntries, "CategoryA", "NewCategory");

        var renamedCategory = deckEntry.Categories.First();
        Assert.Multiple(() => {
            Assert.That(renamedCategory.Name, Is.EqualTo("NewCategory"));
            Assert.That(renamedCategory.MinCount, Is.EqualTo(2));
            Assert.That(renamedCategory.MaxCount, Is.EqualTo(3));
        });
    }
}
