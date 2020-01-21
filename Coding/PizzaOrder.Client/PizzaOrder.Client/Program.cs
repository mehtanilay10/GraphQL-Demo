using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using PizzaOrder.Client.Models;

namespace PizzaOrder.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var graphQLClient = new GraphQLHttpClient("https://localhost:44310/graphql");

            #region Query 

            string newOrdersQuery = @"query {
    newOrders {
        addressLine1
        addressLine2
        amount
    }
}";

            var newOrderResponse = await graphQLClient.SendQueryAsync(newOrdersQuery);
            List<NewOrderDetails> newOrderDetails = newOrderResponse.GetDataFieldAs<List<NewOrderDetails>>("newOrders");

            #endregion

            string createOrderMutation = @"mutation ($order:OrderDetailsInputType!) {
  createOrder(orderDetails:$order)
  {
    id
    orderStatus
    addressLine1
    addressLine2
    pizzaDetails
    {
      id
      toppings
    }
  }
}";

            GraphQLRequest createOrderRequest = new GraphQLRequest
            {
                Query = createOrderMutation,
                Variables = new
                {
                    order = new
                    {
                        addressLine1 = "1 Address Line",
                        addressLine2 = "2 Address Line",
                        mobileNo = "0123456789",
                        amount = 500,
                        pizzaDetails = new[]{
                            new {
                                name = "My Nilay Pizza",
                                price = 10,
                                size = 5,
                                toppings = "EXTRA_CHEESE"
                            }
                        }
                    }
                }
            };

            var createOrderResponse = await graphQLClient.SendMutationAsync(createOrderRequest);
            CreateOrderDetails orderDetails = createOrderResponse.GetDataFieldAs<CreateOrderDetails>("createOrder");
        }
    }
}
