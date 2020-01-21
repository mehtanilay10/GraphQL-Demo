using GraphQL.Types;
using PizzaOrder.Business.Services;
using PizzaOrder.Data.Entities;
using PizzaOrder.GraphQLModels.Enums;

namespace PizzaOrder.GraphQLModels.Types
{
    public class OrderDetailsType : ObjectGraphType<OrderDetails>
    {
        public OrderDetailsType(IPizzaDetailsService pizzaDetailsService)
        {
            Name = nameof(OrderDetailsType);

            Field(x => x.Id);
            Field(x => x.AddressLine1);
            Field(x => x.AddressLine2);
            Field(x => x.MobileNo);
            Field(x => x.Amount);
            Field(x => x.Date);

            Field<OrderStatusEnumType>(
                name: "orderStatus",
                resolve: context => context.Source.OrderStatus);

            Field<ListGraphType<PizzaDetailsType>>(
                name: "pizzaDetails",
                resolve: context => pizzaDetailsService.GetAllPizzaDetailsForOrder(context.Source.Id));
        }
    }
}
