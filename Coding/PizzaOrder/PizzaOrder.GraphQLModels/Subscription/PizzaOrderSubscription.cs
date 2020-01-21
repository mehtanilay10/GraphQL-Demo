using System.Reactive.Linq;
using GraphQL.Resolvers;
using GraphQL.Types;
using PizzaOrder.Business.Models;
using PizzaOrder.Business.Services;
using PizzaOrder.Data.Enums;
using PizzaOrder.GraphQLModels.Enums;
using PizzaOrder.GraphQLModels.Types;

namespace PizzaOrder.GraphQLModels.Subscription
{
    public class PizzaOrderSubscription : ObjectGraphType
    {
        private readonly IEventService _eventService;

        public PizzaOrderSubscription(IEventService eventService)
        {
            _eventService = eventService;
            Name = nameof(PizzaOrderSubscription);

            AddField(new EventStreamFieldType
            {
                Name = "ordderCreated",
                Type = typeof(EventDataType),
                Resolver = new FuncFieldResolver<EventDataModel>(context => context.Source as EventDataModel),
                Subscriber = new EventStreamResolver<EventDataModel>(context =>
                {
                    return _eventService.OnCreateObservable();
                })
            });

            AddField(new EventStreamFieldType
            {
                Name = "statusUpdate",
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<OrderStatusEnumType>> { Name = "status" }),
                Type = typeof(EventDataType),
                Resolver = new FuncFieldResolver<EventDataModel>(context => context.Source as EventDataModel),
                Subscriber = new EventStreamResolver<EventDataModel>(context =>
                {
                    OrderStatus status = context.GetArgument<OrderStatus>("status");
                    var events = eventService.OnStatusUpdateOnservable();
                    return events.Where(x => x.OrderStatus == status);
                })
            });
        }
    }
}
