using PizzaOrder.Business.Enums;

namespace PizzaOrder.Business.Models
{
    public class SortingDetails<T>
    {
        public T Field { get; set; }
        public SortingDirection Direction { get; set; }
    }
}
