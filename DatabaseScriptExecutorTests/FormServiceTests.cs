using System;
using System.Linq;
using System.Threading.Tasks;
using DatabaseScriptExecutor;
using Moq;
using Xunit;

namespace DatabaseScriptExecutorTests
{
    public class FormServiceTests
    {
        private readonly FormService _formService;
        private readonly Mock<IFormRepository> _repositoryMoq;

        FormServiceTests()
        {
            _repositoryMoq = new Mock<IFormRepository>();
            _formService = new FormService(_repositoryMoq.Object);
        }

        [Fact]
        public async Task GetDbNamesTest()
        {
            var connectionString = "";
            var expected = new[] { "1", "2" };

            _repositoryMoq.Setup(r => r.GetDbNamesAsync(It.IsAny<string>())).ReturnsAsync(expected);
            var actual = await _formService.GetDbNamesAsync(connectionString);

            Assert.Equal(expected, actual);
            _repositoryMoq.Verify(r => r.GetDbNamesAsync(It.IsAny<string>()), Times.Once);
        }
    }
}
