using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PizzaOrder.Data.Entities;

namespace PizzaOrder.Data
{
    public class PizzaDBContext : IdentityDbContext
    {
        public PizzaDBContext()
        {
        }

        public PizzaDBContext(DbContextOptions<PizzaDBContext> options)
            : base(options)
        {
        }

        public DbSet<PizzaDetails> PizzaDetails { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
    }
}
