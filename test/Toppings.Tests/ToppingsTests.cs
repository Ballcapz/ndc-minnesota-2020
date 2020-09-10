using System;
using System.Threading.Tasks;
using Xunit;

namespace Toppings.Tests
{
    // xunit injects ToppingsApplicationFactory
    public class ToppingsTests : IClassFixture<ToppingsApplicationFactory>
    {
        private readonly ToppingsApplicationFactory _factory;

        public ToppingsTests(ToppingsApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Test1()
        {
            var client = _factory.CreateToppingsClient();

            // this is how we actually interact in our app
            var response = await client.GetAvailableAsync(new AvailableRequest());

            Assert.Equal(2, response.Toppings.Count);
        }
    }
}
