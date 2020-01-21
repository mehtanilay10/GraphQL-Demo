using System.Linq;
using GraphQL.Types;
using PizzaOrder.Business.Models;
using PizzaOrder.Business.Services;
using PizzaOrder.Data.Entities;
using PizzaOrder.Data.Enums;
using PizzaOrder.GraphQLModels.Enums;
using PizzaOrder.GraphQLModels.InputTypes;
using PizzaOrder.GraphQLModels.Types;

namespace PizzaOrder.GraphQLModels.Mutations
{
    public class PizzaOrderMutation : ObjectGraphType
    {
        public PizzaOrderMutation(IPizzaDetailsService pizzaDetailsService, IOrderDetailsService orderDetailsService)
        {
            Name = nameof(PizzaOrderMutation);

            FieldAsync<OrderDetailsType>(
                name: "createOrder",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<OrderDetailsInputType>> { Name = "orderDetails" }),
                resolve: async context =>
                {
                    var order = context.GetArgument<OrderDetailsModel>("orderDetails");

                    var orderDetails = new OrderDetails(order.AddressLine1, order.AddressLine2, order.MobileNo, order.Amount);
                    orderDetails = await orderDetailsService.CreateAsync(orderDetails);

                    var pizzaDetails = order.PizzaDetails.Select(x => new PizzaDetails(x.Name, x.Toppings, x.Price, x.Size, orderDetails.Id));
                    pizzaDetails = await pizzaDetailsService.CreateBulkAsync(pizzaDetails, orderDetails.Id);

                    orderDetails.PizzaDetails = pizzaDetails.ToList();
                    return orderDetails;
                });

            FieldAsync<OrderDetailsType>(
                name: "updateStatus",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "id" },
                    new QueryArgument<NonNullGraphType<OrderStatusEnumType>> { Name = "status" }),
                resolve: async context =>
                {
                    int orderId = context.GetArgument<int>("id");
                    OrderStatus orderStatus = context.GetArgument<OrderStatus>("status");

                    return await orderDetailsService.UpdateStatusAsync(orderId, orderStatus);
                });


            FieldAsync<OrderDetailsType>(
                name: "deletePizzaDetails",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "pizzaDetailsId" }),
                resolve: async context =>
                {
                    int pizzaDetailsId = context.GetArgument<int>("pizzaDetailsId");

                    int orderId = await pizzaDetailsService.DeletePizzaDetailsAsync(pizzaDetailsId);

                    return await orderDetailsService.GetOrderDetailsAsync(orderId);
                });
        }
    }
}
