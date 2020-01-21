using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PizzaOrder.Data;
using PizzaOrder.Data.Entities;

namespace PizzaOrder.Business.Services
{
    public interface IPizzaDetailsService
    {
        IEnumerable<PizzaDetails> GetAllPizzaDetailsForOrder(int orderId);
        Task<PizzaDetails> GetPizzaDetailsAsync(int pizzaDetailsId);
        Task<IEnumerable<PizzaDetails>> CreateBulkAsync(IEnumerable<PizzaDetails> pizzaDetails, int orderId);
        Task<int> DeletePizzaDetailsAsync(int pizzaDetailsId);
        PizzaDetails GetPizzaDetailsOrError();
    }

    public class PizzaDetailsService : IPizzaDetailsService
    {
        private readonly PizzaDBContext _dbContext;

        public PizzaDetailsService(PizzaDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PizzaDetails> GetPizzaDetailsAsync(int pizzaDetailsId)
        {
            return await _dbContext.PizzaDetails
                .FindAsync(pizzaDetailsId);
        }

        public IEnumerable<PizzaDetails> GetAllPizzaDetailsForOrder(int orderId)
        {
            return _dbContext.PizzaDetails.Where(x => x.OrderDetailsId == orderId).ToList();
        }

        public async Task<IEnumerable<PizzaDetails>> CreateBulkAsync(IEnumerable<PizzaDetails> pizzaDetails, int orderId)
        {
            await _dbContext.PizzaDetails.AddRangeAsync(pizzaDetails);
            await _dbContext.SaveChangesAsync();
            return _dbContext.PizzaDetails.Where(x => x.OrderDetailsId == orderId);
        }

        public async Task<int> DeletePizzaDetailsAsync(int pizzaDetailsId)
        {
            var pizzaDetails = await _dbContext.PizzaDetails.FindAsync(pizzaDetailsId);

            if (pizzaDetails != null)
            {
                int orderId = pizzaDetails.OrderDetailsId;
                _dbContext.PizzaDetails.Remove(pizzaDetails);
                await _dbContext.SaveChangesAsync();
                return orderId;
            }

            return 0;
        }

        public PizzaDetails GetPizzaDetailsOrError()
        {
            bool generateError = (DateTime.Now.Millisecond % 2 == 0);
            if (generateError)
                throw new Exception("Specific Error Message will came here.");

            return new PizzaDetails { Id = 1, Name = "Without Error" };
        }
    }
}
