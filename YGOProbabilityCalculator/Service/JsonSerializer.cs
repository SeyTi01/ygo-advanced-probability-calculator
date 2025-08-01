using System.Text.Json;
using YGOProbabilityCalculator.Service.Interface;

namespace YGOProbabilityCalculator.Service;

public class JsonSerializer(JsonSerializerOptions? options = null) : ISerializer {
    private readonly JsonSerializerOptions _options = options ?? new JsonSerializerOptions { WriteIndented = true };

    public string Serialize<T>(T value) =>
        System.Text.Json.JsonSerializer.Serialize(value, _options);

    public T? Deserialize<T>(string json) =>
        System.Text.Json.JsonSerializer.Deserialize<T>(json, _options);
}
