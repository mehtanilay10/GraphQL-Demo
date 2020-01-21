using System;
using PizzaOrder.GraphQLModels.Mutations;
using PizzaOrder.GraphQLModels.Queries;
using PizzaOrder.GraphQLModels.Subscription;

namespace PizzaOrder.GraphQLModels.Schema
{
    public class PizzaOrderSchema : GraphQL.Types.Schema
    {
        public PizzaOrderSchema(IServiceProvider services) : base(services)
        {
            Services = services;
            Query = (PizzaOrderQuery)services.GetService(typeof(PizzaOrderQuery));
            Mutation = (PizzaOrderMutation)services.GetService(typeof(PizzaOrderMutation));
            Subscription = (PizzaOrderSubscription)services.GetService(typeof(PizzaOrderSubscription));
        }
    }
}
