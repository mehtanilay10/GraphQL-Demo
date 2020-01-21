using System.Collections.Generic;

namespace PizzaOrder.Business.Models
{
    public class OrderDetailsModel
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string MobileNo { get; set; }
        public List<PizzaDetailsModel> PizzaDetails { get; set; }
        public int Amount { get; set; }
    }
}
