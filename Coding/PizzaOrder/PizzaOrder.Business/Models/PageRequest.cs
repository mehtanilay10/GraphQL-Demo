using PizzaOrder.Business.Enums;

namespace PizzaOrder.Business.Models
{
    public class PageRequest
    {
        public int? First { get; set; }
        public int? Last { get; set; }
        public string After { get; set; }
        public string Before { get; set; }
        public SortingDetails<CompletedOrdersSortingFields> OrderBy { get; set; }
    }
}
