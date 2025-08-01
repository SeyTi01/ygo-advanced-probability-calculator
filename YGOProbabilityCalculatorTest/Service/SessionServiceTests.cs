using Moq;
using YGOProbabilityCalculator.Model;
using YGOProbabilityCalculator.Service;
using YGOProbabilityCalculator.Service.Interface;
using YGOProbabilityCalculator.ViewModel.Main;

namespace YGOProbabilityCalculatorTest.Service;

[TestFixture]
public class SessionServiceTests {
    private Mock<IFileService> _fileServiceMock = null!;
    private Mock<ISerializer> _serializerMock = null!;
    private SessionService _sessionService = null!;
    private SessionState _testState = null!;
    private const string TestFilePath = "test.json";
    private const string TestJson = "{\"test\":\"json\"}";

    [SetUp]
    public void Setup() {
        _fileServiceMock = new Mock<IFileService>();
        _serializerMock = new Mock<ISerializer>();
        _sessionService = new SessionService(_fileServiceMock.Object, _serializerMock.Object);
        _testState = new SessionState {
            HandSize = 5,
            DeckEntries = [new DeckEntry { Name = "TestCard", Copies = 1 }],
            CategoryEntries = [new CategoryEntry { Category = "TestCategory" }],
            ComboEntries = [],
            ResultText = "Test Result"
        };
    }

    [Test]
    public async Task SaveSessionAsync_ValidState_SavesCorrectly() {
        _serializerMock.Setup(x => x.Serialize(_testState))
            .Returns(TestJson);

        await _sessionService.SaveSessionAsync(TestFilePath, _testState);

        _fileServiceMock.Verify(x => x.WriteAllTextAsync(TestFilePath, TestJson), Times.Once);
    }

    [Test]
    public async Task LoadSessionAsync_ValidFile_LoadsCorrectly() {
        _fileServiceMock.Setup(x => x.ReadAllTextAsync(TestFilePath))
            .ReturnsAsync(TestJson);
        _serializerMock.Setup(x => x.Deserialize<SessionState>(TestJson))
            .Returns(_testState);

        var result = await _sessionService.LoadSessionAsync(TestFilePath);

        Assert.That(result, Is.EqualTo(_testState));
        _fileServiceMock.Verify(x => x.ReadAllTextAsync(TestFilePath), Times.Once);
        _serializerMock.Verify(x => x.Deserialize<SessionState>(TestJson), Times.Once);
    }

    [Test]
    public Task LoadSessionAsync_DeserializerReturnsNull_ThrowsInvalidOperationException() {
        _fileServiceMock.Setup(x => x.ReadAllTextAsync(TestFilePath))
            .ReturnsAsync(TestJson);
        _serializerMock.Setup(x => x.Deserialize<SessionState>(TestJson))
            .Returns((SessionState?)null);

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sessionService.LoadSessionAsync(TestFilePath));
        return Task.CompletedTask;
    }

    [Test]
    public Task LoadSessionAsync_FileServiceThrows_PropagatesException() {
        _fileServiceMock.Setup(x => x.ReadAllTextAsync(TestFilePath))
            .ThrowsAsync(new FileNotFoundException());

        Assert.ThrowsAsync<FileNotFoundException>(() =>
            _sessionService.LoadSessionAsync(TestFilePath));
        return Task.CompletedTask;
    }

    [Test]
    public Task SaveSessionAsync_FileServiceThrows_PropagatesException() {
        _serializerMock.Setup(x => x.Serialize(_testState))
            .Returns(TestJson);
        _fileServiceMock.Setup(x => x.WriteAllTextAsync(TestFilePath, TestJson))
            .ThrowsAsync(new UnauthorizedAccessException());

        Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _sessionService.SaveSessionAsync(TestFilePath, _testState));
        return Task.CompletedTask;
    }
}
