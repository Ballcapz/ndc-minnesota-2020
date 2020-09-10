using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;

namespace Orders
{
    public class OrdersService : Orders.OrdersBase
    {
        private readonly Toppings.ToppingsClient _toppingsClient;

        public OrdersService(Toppings.ToppingsClient toppingsClient)
        {
            _toppingsClient = toppingsClient;
        }

        public override async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request, Grpc.Core.ServerCallContext context)
        {
            // update stock
            var decrementRequest = new DecrementStockRequest();
            decrementRequest.ToppingIds.AddRange(request.ToppingIds);

            await _toppingsClient.DecrementStockAsync(decrementRequest);

            return new PlaceOrderResponse
            {
                Time = DateTimeOffset.UtcNow.ToTimestamp()
            };
        }
    }
}
