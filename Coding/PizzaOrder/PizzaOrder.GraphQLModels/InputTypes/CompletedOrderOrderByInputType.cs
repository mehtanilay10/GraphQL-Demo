using GraphQL.Types;
using PizzaOrder.Business.Enums;
using PizzaOrder.Business.Models;
using PizzaOrder.GraphQLModels.Enums;

namespace PizzaOrder.GraphQLModels.InputTypes
{
    public class CompletedOrderOrderByInputType : InputObjectGraphType<SortingDetails<CompletedOrdersSortingFields>>
    {
        public CompletedOrderOrderByInputType()
        {
            Field<CompletedOrdersSortingFieldsEnumType>("field", resolve: context => context.Source.Field);
            Field<SortingDirectionEnumType>("direction", resolve: context => context.Source.Direction);
        }
    }
}
