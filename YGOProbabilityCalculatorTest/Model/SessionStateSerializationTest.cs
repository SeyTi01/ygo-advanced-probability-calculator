using System.Text.Json;
using YGOProbabilityCalculator.Model;
using YGOProbabilityCalculator.ViewModel.Main;

namespace YGOProbabilityCalculatorTest.Model;

[TestFixture]
public class SessionStateSerializationTest {
    private static readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    private static readonly SessionState _testState = new() {
        HandSize = 5,
        ResultText = "",
        DeckEntries = [
            new DeckEntry {
                Name = "TestCard",
                Copies = 1,
                Categories = [new Category("TestCategory", 1, 1)]
            }
        ],
        CategoryEntries = [
            new CategoryEntry {
                Category = "TestCategory",
                MinCount = 0,
                MaxCount = 0
            }
        ],
        ComboEntries = [
            new ComboEntry {
                Name = "TestCombo",
                Categories = [
                    new Category("TestCategory", 1, 5)
                ]
            }
        ]
    };

    private const string _expectedJson = """
                                         {
                                           "HandSize": 5,
                                           "DeckEntries": [
                                             {
                                               "Name": "TestCard",
                                               "Categories": [
                                                 {
                                                   "Name": "TestCategory",
                                                   "MinCount": 1,
                                                   "MaxCount": 1
                                                 }
                                               ],
                                               "CategoriesString": "TestCategory",
                                               "Copies": 1
                                             }
                                           ],
                                           "CategoryEntries": [
                                             {
                                               "Category": "TestCategory",
                                               "MinCount": 0,
                                               "MaxCount": 0
                                             }
                                           ],
                                           "ComboEntries": [
                                             {
                                               "Name": "TestCombo",
                                               "Categories": [
                                                 {
                                                   "Name": "TestCategory",
                                                   "MinCount": 1,
                                                   "MaxCount": 5
                                                 }
                                               ]
                                             }
                                           ],
                                           "ResultText": ""
                                         }
                                         """;

    [Test]
    public static void Serialization_ProducesExpectedJsonStructure() {
        var actualJson = JsonSerializer.Serialize(_testState, _options);

        var expectedObj = JsonSerializer.Deserialize<JsonDocument>(_expectedJson);
        var actualObj = JsonSerializer.Deserialize<JsonDocument>(actualJson);

        var normalizedExpected = JsonSerializer.Serialize(expectedObj, _options);
        var normalizedActual = JsonSerializer.Serialize(actualObj, _options);

        Assert.That(normalizedActual, Is.EqualTo(normalizedExpected));
    }

    [Test]
    public static void Deserialization_ReconstructsObjectCorrectly() {
        var deserializedState = JsonSerializer.Deserialize<SessionState>(_expectedJson, _options);

        Assert.Multiple(() => {
            Assert.That(deserializedState!.HandSize, Is.EqualTo(_testState.HandSize));
            Assert.That(deserializedState.ResultText, Is.EqualTo(_testState.ResultText));

            Assert.That(deserializedState.DeckEntries, Has.Count.EqualTo(1));
            Assert.That(deserializedState.DeckEntries[0].Name, Is.EqualTo("TestCard"));
            Assert.That(deserializedState.DeckEntries[0].Copies, Is.EqualTo(1));
            Assert.That(deserializedState.DeckEntries[0].Categories[0].Name, Is.EqualTo("TestCategory"));
            Assert.That(deserializedState.DeckEntries[0].Categories[0].MinCount, Is.EqualTo(1));
            Assert.That(deserializedState.DeckEntries[0].Categories[0].MaxCount, Is.EqualTo(1));

            Assert.That(deserializedState.CategoryEntries, Has.Count.EqualTo(1));
            Assert.That(deserializedState.CategoryEntries[0].Category, Is.EqualTo("TestCategory"));
            Assert.That(deserializedState.CategoryEntries[0].MinCount, Is.EqualTo(0));
            Assert.That(deserializedState.CategoryEntries[0].MaxCount, Is.EqualTo(0));

            Assert.That(deserializedState.ComboEntries, Has.Count.EqualTo(1));
            Assert.That(deserializedState.ComboEntries[0].Name, Is.EqualTo("TestCombo"));
            Assert.That(deserializedState.ComboEntries[0].Categories, Has.Count.EqualTo(1));
            Assert.That(deserializedState.ComboEntries[0].Categories[0].Name, Is.EqualTo("TestCategory"));
            Assert.That(deserializedState.ComboEntries[0].Categories[0].MinCount, Is.EqualTo(1));
            Assert.That(deserializedState.ComboEntries[0].Categories[0].MaxCount, Is.EqualTo(5));
        });
    }

    [Test]
    public static void Serialization_RoundTrip_PreservesAllData() {
        var json = JsonSerializer.Serialize(_testState, _options);
        var roundTrippedState = JsonSerializer.Deserialize<SessionState>(json, _options);
        var finalJson = JsonSerializer.Serialize(roundTrippedState, _options);

        var originalObj = JsonSerializer.Deserialize<JsonDocument>(json);
        var finalObj = JsonSerializer.Deserialize<JsonDocument>(finalJson);

        var normalizedOriginal = JsonSerializer.Serialize(originalObj, _options);
        var normalizedFinal = JsonSerializer.Serialize(finalObj, _options);

        Assert.That(normalizedFinal, Is.EqualTo(normalizedOriginal));
    }
}
