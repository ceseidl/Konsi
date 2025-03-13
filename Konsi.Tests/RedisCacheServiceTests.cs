using Konsi.Shared;
using Moq;
using StackExchange.Redis;
using System.Threading.Tasks;
using Xunit;

namespace Konsi.Tests
{
    public class RedisCacheServiceTests
    {
        private readonly Mock<IConnectionMultiplexer> _redisMock;
        private readonly Mock<IDatabase> _databaseMock;
        private readonly RedisCacheService _cacheService;

        public RedisCacheServiceTests()
        {
            _redisMock = new Mock<IConnectionMultiplexer>();
            _databaseMock = new Mock<IDatabase>();
            _redisMock.Setup(r => r.GetDatabase(It.IsAny<int>(), null)).Returns(_databaseMock.Object);

            _cacheService = new RedisCacheService(_redisMock.Object);
        }

        [Fact]
        public async Task GetFromCache_ShouldReturnData_IfExists()
        {
            // Arrange
            var key = "12345678900";
            var value = "{\"beneficio\": \"mockData\"}";
            _databaseMock.Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync(value);

            // Act
            var result = await _cacheService.GetAsync(key);

            // Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public async Task SaveToCache_ShouldStoreDataCorrectly()
        {
            // Arrange
            var key = "12345678900";
            var value = "{\"beneficio\": \"mockData\"}";

            // Act
            await _cacheService.SetAsync(key, value);

            // Assert
            _databaseMock.Verify(db => db.StringSetAsync(key, value, null, When.Always, It.IsAny<CommandFlags>()), Times.Once);
        }
    }
}