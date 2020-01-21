using GraphQL.Types;
using PizzaOrder.Business.Enums;

namespace PizzaOrder.GraphQLModels.Enums
{
    public class CompletedOrdersSortingFieldsEnumType : EnumerationGraphType<CompletedOrdersSortingFields>
    {
        public CompletedOrdersSortingFieldsEnumType()
        {
            Name = nameof(CompletedOrdersSortingFieldsEnumType);
        }
    }
}
