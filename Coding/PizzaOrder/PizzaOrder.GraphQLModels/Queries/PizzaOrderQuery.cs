using System.Collections.Generic;
using System.Linq;
using GraphQL;
using GraphQL.Authorization;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using PizzaOrder.Business.Enums;
using PizzaOrder.Business.Helpers;
using PizzaOrder.Business.Models;
using PizzaOrder.Business.Services;
using PizzaOrder.Data.Entities;
using PizzaOrder.GraphQLModels.InputTypes;
using PizzaOrder.GraphQLModels.Types;

namespace PizzaOrder.GraphQLModels.Queries
{
    public class PizzaOrderQuery : ObjectGraphType
    {
        public PizzaOrderQuery(IOrderDetailsService orderDetailsService, IPizzaDetailsService pizzaDetailsService)
        {
            Name = nameof(PizzaOrderQuery);
            //this.AuthorizeWith(Constants.AuthPolicy.CustomerPolicy, Constants.AuthPolicy.RestaurantPolicy);

            FieldAsync<ListGraphType<OrderDetailsType>>(
                name: "newOrders",
                resolve: async context => await orderDetailsService.GettAllNewOrdersAsync());
            //.AuthorizeWith(Constants.AuthPolicy.RestaurantPolicy);

            FieldAsync<PizzaDetailsType>(
                name: "pizzaDetails",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: async context => await pizzaDetailsService.GetPizzaDetailsAsync(context.GetArgument<int>("id")));

            FieldAsync<OrderDetailsType>(
                name: "orderDetails",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: async context => await orderDetailsService.GetOrderDetailsAsync(context.GetArgument<int>("id")))
            .AuthorizeWith(Constants.AuthPolicy.AdminPolicy);

            Connection<OrderDetailsType>()
                .Name("completedOrders")
                .Unidirectional()
                .PageSize(10)
                .Argument<CompletedOrderOrderByInputType>("orderBy", "Pass field & direction on which you want to sort data")
                .ResolveAsync(async context =>
                {
                    var pageRequest = new PageRequest
                    {
                        First = context.First,
                        Last = context.Last,
                        After = context.After,
                        Before = context.Before,
                        OrderBy = context.GetArgument<SortingDetails<CompletedOrdersSortingFields>>("orderBy")
                    };

                    var pageResponse = await orderDetailsService.GetCompletedOrdersAsync(pageRequest);

                    (string startCursor, string endCursor) = CursorHelper.GetFirstAndLastCursor(pageResponse.Nodes.Select(x => x.Id));

                    var edge = pageResponse.Nodes.Select(x => new Edge<OrderDetails>
                    {
                        Cursor = CursorHelper.ToCursor(x.Id),
                        Node = x
                    }).ToList();

                    var connection = new Connection<OrderDetails>()
                    {
                        Edges = edge,
                        TotalCount = pageResponse.TotalCount,
                        PageInfo = new PageInfo
                        {
                            HasNextPage = pageResponse.HasNextPage,
                            HasPreviousPage = pageResponse.HasPreviousPage,
                            StartCursor = startCursor,
                            EndCursor = endCursor
                        }
                    };

                    return connection;
                });

            Field<PizzaDetailsType>(
                name: "exceptionDemo",
                resolve: context =>
                {
                    var data = new Dictionary<string, string>
                    {
                        {"key", "value" }
                    };

                    var ex = new ExecutionError("Some error message", data);
                    ex.AddLocation(20, 500);
                    context.Errors.Add(ex);

                    return pizzaDetailsService.GetPizzaDetailsOrError();
                });
        }
    }
}
