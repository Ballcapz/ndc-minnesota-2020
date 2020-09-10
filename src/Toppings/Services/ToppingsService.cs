
using Toppings.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Grpc.Core;

namespace Toppings
{
    public class ToppingsService : Toppings.ToppingsBase
    {
        private readonly IToppingData _data;

        public ToppingsService(IToppingData data)
        {
            _data = data;
        }
        public override async Task<AvailableResponse> GetAvailable(AvailableRequest request, Grpc.Core.ServerCallContext context)
        {
            List<ToppingEntity> toppings;

            try
            {
                toppings = await _data.GetAsync();
            }
            catch (System.Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }

            var response = new AvailableResponse();
            foreach (var entity in toppings)
            {
                var topping = new Topping
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Price = entity.Price
                };

                var availableTopping = new AvailableTopping
                {
                    Topping = topping,
                    Quantity = entity.StockCount
                };

                // Toppings is google generated, so we can't create with it we can only add to it or remove etc...
                response.Toppings.Add(availableTopping);
            }

            return response;
        }

        public override async Task<DecrementStockResponse> DecrementStock(DecrementStockRequest request, ServerCallContext context)
        {
            // this does not have excption handling rn
            foreach (var id in request.ToppingIds)
            {
                await _data.DecrementStockAsync(id);
            }

            return new DecrementStockResponse();
        }

    }
}