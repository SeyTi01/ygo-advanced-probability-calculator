using System.IO;
using YGOProbabilityCalculator.Service.Interface;
using YGOProbabilityCalculator.ViewModel.Main;

namespace YGOProbabilityCalculator.Service;

public class DeckImportService(ICardInfoService cardInfoService) {
    public async Task<List<DeckEntry>> ImportDeckFromYdkAsync(string filePath) {
        var lines = await File.ReadAllLinesAsync(filePath);
        var cardCounts = new Dictionary<int, int>();
        var deckEntries = new List<DeckEntry>();

        foreach (var raw in lines) {
            var line = raw.Trim();
            if (line.Equals("#extra", StringComparison.OrdinalIgnoreCase)) break;
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#')) continue;

            if (!int.TryParse(line, out var cardId)) continue;
            if (!cardCounts.TryAdd(cardId, 1)) cardCounts[cardId]++;
        }

        foreach (var (id, count) in cardCounts) {
            string cardName;
            try {
                cardName = await cardInfoService.GetCardNameAsync(id);
            }
            catch {
                cardName = id.ToString();
            }

            deckEntries.Add(new DeckEntry {
                Name = cardName,
                Copies = count,
                Categories = []
            });
        }

        return deckEntries;
    }
}
