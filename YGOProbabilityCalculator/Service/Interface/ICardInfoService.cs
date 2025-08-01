namespace YGOProbabilityCalculator.Service.Interface;

public interface ICardInfoService {
    Task<string> GetCardNameAsync(int id);
}
