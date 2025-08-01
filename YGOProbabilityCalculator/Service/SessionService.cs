using YGOProbabilityCalculator.Model;
using YGOProbabilityCalculator.Service.Interface;

namespace YGOProbabilityCalculator.Service;

public class SessionService(IFileService fileService, ISerializer serializer) {
    public async Task SaveSessionAsync(string filePath, SessionState state) {
        var json = serializer.Serialize(state);
        await fileService.WriteAllTextAsync(filePath, json);
    }

    public async Task<SessionState> LoadSessionAsync(string filePath) {
        var json = await fileService.ReadAllTextAsync(filePath);
        var state = serializer.Deserialize<SessionState>(json)
                    ?? throw new InvalidOperationException("Invalid session file.");
        return state;
    }
}
