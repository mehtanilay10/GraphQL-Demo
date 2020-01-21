using GraphQL.Types;
using PizzaOrder.Business.Models;
using PizzaOrder.GraphQLModels.Enums;

namespace PizzaOrder.GraphQLModels.InputTypes
{
    public class PizzaDetailsInputType : InputObjectGraphType<PizzaDetailsModel>
    {
        public PizzaDetailsInputType()
        {
            Name = nameof(PizzaDetailsInputType);

            Field(x => x.Name);
            Field(x => x.Price);
            Field(x => x.Size);

            Field<ToppingsEnumType>("toppings");
        }
    }
}
