namespace YGOProbabilityCalculator.Service.Interface;

public interface ISerializer {
    string Serialize<T>(T value);
    T? Deserialize<T>(string json);
}
