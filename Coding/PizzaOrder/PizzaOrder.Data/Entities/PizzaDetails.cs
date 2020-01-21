using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PizzaOrder.Data.Enums;

namespace PizzaOrder.Data.Entities
{
    public class PizzaDetails
    {
        #region Fields

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(40)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public Toppings Toppings { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public int Size { get; set; }

        [Required]
        public int OrderDetailsId { get; set; }

        [ForeignKey("OrderDetailsId")]
        public OrderDetails OrderDetails { get; set; }

        #endregion

        #region Ctor

        public PizzaDetails()
        {

        }

        public PizzaDetails(string name, Toppings toppings, double price, int size, int orderDetailsId)
        {
            Name = name;
            Toppings = toppings;
            Price = price;
            Size = size;
            OrderDetailsId = orderDetailsId;
        }

        #endregion
    }
}
