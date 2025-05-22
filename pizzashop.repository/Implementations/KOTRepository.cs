using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Models;
using pizzashop.repository.Interfaces;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Repositories
{
    public class KOTRepository : IKOTRepository
    {
        private readonly PizzaShopContext _context;

        public KOTRepository(PizzaShopContext context)
        {
            _context = context;
        }

        public async Task<(List<KOTRawDataviewModel>, int totalOrders)> GetKOTOrdersByCategoryAndStatus(int? categoryId, string status, int skip, int take)
        {
            var result = await _context.Set<KOTRawDataviewModel>()
                .FromSqlInterpolated($@"
            SELECT * FROM getkotordersbycategoryandstatus({categoryId}, {status}, {skip}, {take})
        ")
                .ToListAsync();

            var totalOrders = result.FirstOrDefault()?.TotalOrders ?? 0;

            return (result, totalOrders);
        }
        public async Task<int> UpdatePreparedQuantities(List<PreparedItemviewModel> updates, string status)
        {
            // var orderId = 0;
            // if (updates.Any())
            // {
            //     var orderitem = await _context.Ordereditems.FirstOrDefaultAsync(oi => oi.Ordereditemid == updates[0].OrderedItemId);
            //     var order = await _context.Orders.FirstOrDefaultAsync(o => o.Orderid == orderitem.Orderid);
            //     if (order.ServedTime == null)
            //     {
            //         order.ServedTime = DateTime.Now;
            //     }
            //     orderId = order.Orderid;
            // }
            // foreach (var Item in updates)
            // {
            //     var item = await _context.Ordereditems.FirstOrDefaultAsync(o => o.Ordereditemid == Item.OrderedItemId);
            //     if (item != null)
            //     {
            //         if (status == "Ready")
            //         {
            //             item.ReadyQuantity = item.ReadyQuantity - Item.ReadyQuantity;
            //         }
            //         else
            //         {
            //             item.ReadyQuantity = item.ReadyQuantity + Item.ReadyQuantity;
            //         }
            //     }
            // }
            // await _context.SaveChangesAsync();
            // return orderId;

            var updatesJson = System.Text.Json.JsonSerializer.Serialize(updates);

            // Call the stored procedure
            var orderId = await _context.Database.ExecuteSqlInterpolatedAsync($@"SELECT update_prepared_quantities({updatesJson}::jsonb, {status})");

            return orderId;
        }
    }
}