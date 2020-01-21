using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PizzaOrder.Data.Enums;

namespace PizzaOrder.Data.Entities
{
    public class OrderDetails
    {
        #region Fields

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(40)]
        public string AddressLine1 { get; set; }

        [Required]
        [StringLength(40)]
        public string AddressLine2 { get; set; }

        [Required]
        [StringLength(10)]
        public string MobileNo { get; set; }

        public List<PizzaDetails> PizzaDetails { get; set; }

        public int Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public OrderStatus OrderStatus { get; set; }

        #endregion

        #region Ctor

        public OrderDetails()
        {

        }

        public OrderDetails(string addressLine1, string addressLine2, string mobileNo, int amount)
        {
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            MobileNo = mobileNo;
            Amount = amount;
            Date = DateTime.Now;
            OrderStatus = OrderStatus.Created;
        }

        #endregion
    }
}
