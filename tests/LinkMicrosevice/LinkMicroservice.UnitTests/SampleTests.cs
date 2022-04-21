using LinkMicroservice.Services;
using Xunit;

namespace LinkMicroservice.UnitTests
{
    public class SampleTests
    {
        [Fact]
        public void ReturningResult12()
        {
            CreationService creationService = new CreationService();

            var result = creationService.AddNumbers(6, 6);

            Assert.Equal(12, result);
        }

        [Fact]
        public void ReturningResult36()
        {
            CreationService creationService = new CreationService();

            var result = creationService.AddNumbers(12, 24);

            Assert.Equal(36, result);
        }
    }
}