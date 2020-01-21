using System;

namespace PizzaOrder.Data.Enums
{
    [Flags]
    public enum Toppings
    {
        NONE = 0,
        Pepperoni = 1,
        Mushrooms = 2,
        Onions = 4,
        Sausage = 8,
        Bacon = 16,
        ExtraCheese = 32,
        BlackOlives = 64
    }
}
