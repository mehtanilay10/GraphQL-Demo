using System.Collections.Generic;

namespace PizzaOrder.Client.Models
{
    public class CreateOrderDetails
    {
        public int Id { get; set; }
        public string OrderStatus { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public List<Pizzadetail> PizzaDetails { get; set; }
    }

    public class Pizzadetail
    {
        public int Id { get; set; }
        public string Toppings { get; set; }
    }
}
