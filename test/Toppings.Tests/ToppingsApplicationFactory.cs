using System.Net.Http;
using System.Net;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using TestHelpers;
using Toppings.Data;
using System.Collections.Generic;
using NSubstitute;
using Microsoft.Extensions.DependencyInjection;

namespace Toppings.Tests
{
    public class ToppingsApplicationFactory : WebApplicationFactory<Startup>
    {
        public Toppings.ToppingsClient CreateToppingsClient()
        {
            // configured to talk to in memory instance
            var client = CreateDefaultClient(new ResponseVersionHandler());

            var channel = GrpcChannel.ForAddress(client.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = client
            });

            return new Toppings.ToppingsClient(channel);
        }


        // this is so our class does not actually go up and hit the azure database when we run our test
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // unregister the IToppingData from the og startup
                services.Remove<IToppingData>();

                var list = new List<ToppingEntity>
                {
                    new ToppingEntity("cheese", "Cheese", 0.5m, 1),
                    new ToppingEntity("tomato", "Tomato", 0.5m, 1),
                };

                // use nsubstitute to resolve IToppingData
                var sub = Substitute.For<IToppingData>();
                sub.GetAsync().Returns(list);

                // add substitution to service collection
                services.AddSingleton(sub);
            });
        }

        private class ResponseVersionHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = await base.SendAsync(request, cancellationToken);
                response.Version = request.Version;
                return response;
            }
        }
    }
}
