using GraphQL.Types;
using PizzaOrder.Business.Enums;

namespace PizzaOrder.GraphQLModels.Enums
{
    public class SortingDirectionEnumType : EnumerationGraphType<SortingDirection>
    {
        public SortingDirectionEnumType()
        {
            Name = nameof(SortingDirectionEnumType);
        }
    }
}
