using PizzaOrder.Data.Enums;

namespace PizzaOrder.Business.Models
{
    public class PizzaDetailsModel
    {
        public string Name { get; set; }
        public Toppings Toppings { get; set; }
        public double Price { get; set; }
        public int Size { get; set; }
    }
}
