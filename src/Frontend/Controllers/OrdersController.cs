using System.Linq;
using System.Threading.Tasks;
using Frontend.Models;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Frontend.Controllers
{
    [Route("orders")]
    public class OrdersController : Controller
    {
        private readonly Orders.OrdersClient _client;
        private readonly ILogger<OrdersController> _log;

        public OrdersController(ILogger<OrdersController> log, Orders.OrdersClient client)
        {
            _log = log;
            _client = client;
        }

        [HttpPost]
        public async Task<IActionResult> Order([FromForm]HomeViewModel viewModel)
        {
            var ids = viewModel.Toppings.Where(t => t.Selected)
                .Select(t => t.Id)
                .ToArray();
            
            _log.LogInformation("Order received: {Toppings}", string.Join(',', ids));

            var request = new PlaceOrderRequest();
            request.ToppingIds.AddRange(ids);
            await _client.PlaceOrderAsync(request);

            return View();
        }
    }
}