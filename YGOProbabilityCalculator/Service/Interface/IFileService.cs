namespace YGOProbabilityCalculator.Service.Interface;

public interface IFileService {
    Task WriteAllTextAsync(string path, string content);
    Task<string> ReadAllTextAsync(string path);
}
