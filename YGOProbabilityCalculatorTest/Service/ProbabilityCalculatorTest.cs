using YGOProbabilityCalculator.Model;
using YGOProbabilityCalculator.Service;

namespace YGOProbabilityCalculatorTest.Service;

[TestFixture]
public class ProbabilityCalculatorTest {
    private const double Tolerance = 1e-12;

    [Test]
    public void AllCategoriesMaxZero() {
        var deck = new List<Card> {
            new(2, "A"),
            new(2)
        };

        var calculator = new ProbabilityCalculator(deck);

        var categories = new[] {
            new Category("A", 0, 0),
        };

        var probability = calculator.CalculateProbabilityRange(categories, 2);

        Assert.That(probability, Is.EqualTo(1.0 / 6.0).Within(Tolerance));
    }

    [Test]
    public void MultipleRangedCategories_ZeroToThree_ShouldBe100Percent() {
        const int handSize = 5;

        var deck = new List<Card> {
            new(3, "A", "B"),
            new(3, "B", "C"),
            new(3, "A", "C"),
            new(3, "A"),
            new(3, "B"),
            new(3, "C"),
            new(2, "A", "B", "C"),
            new(20)
        };

        var calculator = new ProbabilityCalculator(deck);

        var categories = new[] {
            new Category("A", 0, handSize),
            new Category("B", 0, handSize),
            new Category("C", 0, handSize)
        };

        var probability = calculator.CalculateProbabilityRange(categories, handSize);
        Assert.That(probability, Is.EqualTo(1.0).Within(Tolerance));
    }

    [Test]
    public void CalculateProbabilityForCombos_SubsetScenario_EqualsSingleComboProbability() {
        var deck = new List<Card> {
            new(1, "A"),
            new(1, "A"),
            new(1, "A"),
            new(1, "B"),
            new(1, "B"),
            new(1, "B")
        };

        var calculator = new ProbabilityCalculator(deck);
        const int handSize = 2;

        var comboA = new Combo([
            new Category("A", 1, 1)
        ]);

        var comboAb = new Combo([
            new Category("A", 1, 1),
            new Category("B", 1, 1)
        ]);

        var categories = new[] { new Category("A", 1, 1) };

        var singleProb = calculator.CalculateProbabilityRange(categories, handSize);
        var comboProb = calculator.CalculateProbabilityForCombos([comboA, comboAb], handSize);

        Assert.That(comboProb, Is.EqualTo(singleProb).Within(Tolerance));
    }

    [Test]
    public void CalculateProbabilityForCombos_ExampleScenario_CalculatedCorrectly() {
        var cards = new List<Card> {
            new(1, "A"),
            new(1, "B"),
            new(1, "C"),
            new()
        };

        var calculator = new ProbabilityCalculator(cards);

        var combo1 = new Combo([new Category("A", 1, 1)]);
        var combo2 = new Combo([
            new Category("B", 1, 1),
            new Category("C", 1, 1)
        ]);

        var probability = calculator.CalculateProbabilityForCombos([combo1, combo2], 2);
        Assert.That(probability, Is.EqualTo(0.5 + 1.0 / 6.0).Within(Tolerance));
    }

    [Test]
    public void ExactRangeRequirements_CalculateCorrectProbability() {
        var deck = new List<Card> {
            new(2, "starter"),
            new(1, "extender"),
            new(1, "starter", "extender")
        };

        var calculator = new ProbabilityCalculator(deck);

        var requirements = new[] {
            new Category("starter", 1, 1),
            new Category("extender", 1, 1)
        };

        var probability = calculator.CalculateProbabilityRange(requirements, 2);
        Assert.That(probability, Is.EqualTo(5.0 / 6.0).Within(Tolerance));
    }

    [Test]
    public void MinGreaterThanOneRequirements_CalculatedCorrectly() {
        const int handSize = 3;

        var deck = new List<Card> {
            new(3, "starter"),
            new(2, "extender"),
        };

        var calculator = new ProbabilityCalculator(deck);

        var requirements = new[] {
            new Category("starter", 2, handSize)
        };

        var probability = calculator.CalculateProbabilityRange(requirements, handSize);
        Assert.That(probability, Is.EqualTo(7.0 / 10.0).Within(Tolerance));
    }

    [Test]
    public void OverlapThreeCategories_CalculatedCorrectly() {
        const int handSize = 3;

        var deck = new List<Card> {
            new(1, "starter"),
            new(1, "extender"),
            new(1, "combo"),
            new(1, "starter", "extender", "combo"),
            new()
        };

        var calculator = new ProbabilityCalculator(deck);

        var categories = new[] {
            new Category("starter", 1, handSize),
            new Category("extender", 1, handSize),
            new Category("combo", 1, handSize)
        };

        var probability = calculator.CalculateProbabilityRange(categories, handSize);
        Assert.That(probability, Is.EqualTo(7.0 / 10.0).Within(Tolerance));
    }
}
