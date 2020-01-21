using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaOrder.Business.Helpers;
using PizzaOrder.Business.Models;
using PizzaOrder.Data;
using PizzaOrder.Data.Entities;
using PizzaOrder.Data.Enums;

namespace PizzaOrder.Business.Services
{
    public interface IOrderDetailsService
    {
        Task<OrderDetails> CreateAsync(OrderDetails orderDetails);
        Task<PageResponse<OrderDetails>> GetCompletedOrdersAsync(PageRequest pageRequest);
        Task<OrderDetails> GetOrderDetailsAsync(int orderId);
        Task<IEnumerable<OrderDetails>> GettAllNewOrdersAsync();
        Task<OrderDetails> UpdateStatusAsync(int orderId, OrderStatus orderStatus);
    }

    public class OrderDetailsService : IOrderDetailsService
    {
        private readonly PizzaDBContext _dbContext;
        private readonly IEventService _eventService;

        public OrderDetailsService(PizzaDBContext dbContext, IEventService eventService)
        {
            _dbContext = dbContext;
            _eventService = eventService;
        }

        public async Task<IEnumerable<OrderDetails>> GettAllNewOrdersAsync()
        {
            return await _dbContext.OrderDetails
                .Where(x => x.OrderStatus == Data.Enums.OrderStatus.Created)
                .ToListAsync();
        }

        public async Task<OrderDetails> GetOrderDetailsAsync(int orderId)
        {
            return await _dbContext.OrderDetails.FindAsync(orderId);
        }

        public async Task<OrderDetails> CreateAsync(OrderDetails orderDetails)
        {
            _dbContext.OrderDetails.Add(orderDetails);
            await _dbContext.SaveChangesAsync();
            _eventService.CreateOrderEvent(new Models.EventDataModel(orderDetails.Id));
            return orderDetails;
        }

        public async Task<OrderDetails> UpdateStatusAsync(int orderId, OrderStatus orderStatus)
        {
            var orderDetails = await _dbContext.OrderDetails.FindAsync(orderId);

            if (orderDetails != null)
            {
                orderDetails.OrderStatus = orderStatus;
                await _dbContext.SaveChangesAsync();
                _eventService.StatusUpdateEvent(new Models.EventDataModel(orderId, orderStatus));
            }

            return orderDetails;
        }

        public async Task<PageResponse<OrderDetails>> GetCompletedOrdersAsync(PageRequest pageRequest)
        {
            var filterQuery = _dbContext.OrderDetails
                .Where(x => x.OrderStatus == OrderStatus.Delivered);

            #region Obtain Nodes

            var dataQuery = filterQuery;
            if (pageRequest.First.HasValue)
            {
                if (!string.IsNullOrEmpty(pageRequest.After))
                {
                    int lastId = CursorHelper.FromCursor(pageRequest.After);
                    dataQuery = dataQuery.Where(x => x.Id > lastId);
                }

                dataQuery = dataQuery.Take(pageRequest.First.Value);
            }

            if (pageRequest.OrderBy?.Field == Enums.CompletedOrdersSortingFields.Address)
            {
                dataQuery = (pageRequest.OrderBy.Direction == Enums.SortingDirection.DESC)
                    ? dataQuery.OrderByDescending(x => x.AddressLine1)
                    : dataQuery.OrderBy(x => x.AddressLine1);
            }
            else if (pageRequest.OrderBy?.Field == Enums.CompletedOrdersSortingFields.Amount)
            {
                dataQuery = (pageRequest.OrderBy.Direction == Enums.SortingDirection.DESC)
                    ? dataQuery.OrderByDescending(x => x.Amount)
                    : dataQuery.OrderBy(x => x.Amount);
            }
            else
            {
                dataQuery = (pageRequest.OrderBy.Direction == Enums.SortingDirection.DESC)
                    ? dataQuery.OrderByDescending(x => x.Id)
                    : dataQuery.OrderBy(x => x.Id);
            }

            List<OrderDetails> nodes = await dataQuery.ToListAsync();

            #endregion

            #region Obtain Flags

            int maxId = nodes.Max(x => x.Id);
            int minId = nodes.Min(x => x.Id);
            bool hasNextPage = await filterQuery.AnyAsync(x => x.Id > maxId);
            bool hasPrevPage = await filterQuery.AnyAsync(x => x.Id < minId);
            int totalCount = await filterQuery.CountAsync();

            #endregion

            return new PageResponse<OrderDetails>
            {
                Nodes = nodes,
                HasNextPage = hasNextPage,
                HasPreviousPage = hasPrevPage,
                TotalCount = totalCount
            };
        }
    }
}
