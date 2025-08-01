using System.IO;
using YGOProbabilityCalculator.Service.Interface;

namespace YGOProbabilityCalculator.Service;

public class FileService : IFileService {
    public Task WriteAllTextAsync(string path, string content) =>
        File.WriteAllTextAsync(path, content);

    public Task<string> ReadAllTextAsync(string path) =>
        File.ReadAllTextAsync(path);
}
