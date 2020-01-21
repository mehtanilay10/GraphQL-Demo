using System.Collections.Generic;
using System.Linq;
using PizzaOrder.Data.Entities;
using PizzaOrder.Data.Enums;

namespace PizzaOrder.Data
{
    public static class DataSeeder
    {
        public static void EnsureDataSeeding(this PizzaDBContext dbContext
            )
        {
            if (!dbContext.OrderDetails.Any())
            {
                dbContext.OrderDetails.AddRange(new List<OrderDetails> {
                    new OrderDetails("4481  Thrash Trail", "Longview", "5033514855", 100),
                    new OrderDetails("4973  Crestview Terrace", "San Antonio", "2108543822", 180),
                    new OrderDetails("4019  Burwell Heights Road", "Sugar Land", "8329883910", 50),
                    new OrderDetails("2208  Charmaine Lane", "Lubbock", "8067739574", 120),
                });

                dbContext.SaveChanges();
            }

            if (!dbContext.PizzaDetails.Any())
            {
                dbContext.PizzaDetails.AddRange(new List<PizzaDetails>
                {
                    new PizzaDetails("Neapolitan Pizza", Toppings.ExtraCheese | Toppings.Onions, 100, 11, 1),
                    new PizzaDetails("Greek Pizza", Toppings.Mushrooms | Toppings.Pepperoni | Toppings.Bacon, 100, 11, 2),
                    new PizzaDetails("New York Style Pizza", Toppings.Sausage, 80, 11, 2),
                    new PizzaDetails("Sicilian Pizza", Toppings.NONE, 50, 9, 3),
                    new PizzaDetails("Pan Pizza", Toppings.Onions, 60, 7, 4),
                    new PizzaDetails("Thin-crust Pizza", Toppings.BlackOlives, 60, 7, 4),
                });

                dbContext.SaveChanges();
            }
        }
    }
}
