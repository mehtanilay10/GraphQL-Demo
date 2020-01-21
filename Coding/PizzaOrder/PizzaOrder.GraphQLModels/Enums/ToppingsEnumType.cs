using GraphQL.Types;
using PizzaOrder.Data.Enums;

namespace PizzaOrder.GraphQLModels.Enums
{
    public class ToppingsEnumType : EnumerationGraphType<Toppings>
    {
        public ToppingsEnumType()
        {
            Name = nameof(ToppingsEnumType);
        }
    }
}
