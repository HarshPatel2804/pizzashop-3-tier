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

        public async Task<(List<KOTOrdersViewModel>, int totalOrders)> GetKOTOrdersByCategoryAndStatus(int? categoryId, string status, int skip, int take)
        {
            var query = _context.Orders
                .Where(o => o.OrderStatus == orderstatus.InProgress)
                .OrderByDescending(o => o.Orderdate)
                .Where(o => o.Ordereditems.Any())
                .AsQueryable();

            if (!categoryId.HasValue)
            {
                var orders = await query
                    .Where(o => o.Ordereditems.Any())
                    .Select(o => new KOTOrdersViewModel
                    {
                        OrderId = o.Orderid,
                        OrderDate = o.Orderdate ?? DateTime.Now,
                        TableName = o.Ordertables.FirstOrDefault().Table.Tablename,
                        SectionName = o.Ordertables.FirstOrDefault().Table.Section.Sectionname,
                        Instruction = o.Orderwisecomment,
                        Items = o.Ordereditems
                            .Select(oi => new KOTOrderedItemsViewModel
                            {
                                Ordereditemid = oi.Ordereditemid,
                                ItemName = oi.Item.Itemname,
                                Quantity = status != "Ready" ? oi.Quantity - oi.ReadyQuantity : oi.ReadyQuantity,
                                Instruction = oi.Itemwisecomment,
                                Modifiers = oi.Ordereditemmodifers.Select(oim => oim.Modifiers).ToList()
                            })
                            .Where(item => item.Quantity > 0)
                            .ToList()
                    })
                    .ToListAsync();

                var Orders = orders.Where(o => o.Items.Any())
                    .Skip(skip)
                    .Take(take)
                    .ToList();
                int totalOrders = orders.Where(o => o.Items.Any()).Count();
                return (Orders, totalOrders);
            }
            else
            {
                query = query.Where(o => o.Ordereditems.Any(oi => oi.Item.Categoryid == categoryId.Value));
                var ordersWithSpecifiedCategory = await query
                    .Where(o => o.Ordereditems.Any(oi => oi.Item.Categoryid == categoryId.Value))
                    .Select(o => new
                    {
                        OrderId = o.Orderid,
                        OrderDate = o.Orderdate ?? DateTime.Now,
                        TableName = o.Ordertables.FirstOrDefault().Table.Tablename,
                        SectionName = o.Ordertables.FirstOrDefault().Table.Section.Sectionname,
                        Instruction = o.Orderwisecomment,
                        Items = o.Ordereditems
                            .Where(oi => oi.Item.Categoryid == categoryId.Value)
                            .Select(oi => new KOTOrderedItemsViewModel
                            {
                                Ordereditemid = oi.Ordereditemid,
                                ItemName = oi.Item.Itemname,
                                Quantity = status != "Ready" ? oi.Quantity - oi.ReadyQuantity : oi.ReadyQuantity,
                                Instruction = oi.Itemwisecomment,
                                Modifiers = oi.Ordereditemmodifers.Select(oim => oim.Modifiers).ToList()
                            })
                            .Where(item => item.Quantity > 0)
                            .ToList()
                    })
                    .ToListAsync();
                int totalOrders = ordersWithSpecifiedCategory.Where(o => o.Items.Any()).Count();
                var Orders = ordersWithSpecifiedCategory
                    .Where(o => o.Items.Any())
                    .Select(o => new KOTOrdersViewModel
                    {
                        OrderId = o.OrderId,
                        OrderDate = o.OrderDate,
                        TableName = o.TableName,
                        SectionName = o.SectionName,
                        Instruction = o.Instruction,
                        Items = o.Items
                    })
                    .Skip(skip)
                    .Take(take)
                    .ToList();

                return (Orders, totalOrders);
            }
        }
        public async Task UpdatePreparedQuantities(List<PreparedItemviewModel> updates, string status)
        {
            if (updates.Any())
            {
                var orderitem = await _context.Ordereditems.FirstOrDefaultAsync(oi => oi.Ordereditemid == updates[0].OrderedItemId);
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.Orderid == orderitem.Orderid);
                if (order.ServedTime == null)
                {
                    order.ServedTime = DateTime.Now;
                }
            }
            foreach (var Item in updates)
            {
                var item = await _context.Ordereditems.FirstOrDefaultAsync(o => o.Ordereditemid == Item.OrderedItemId);
                if (item != null)
                {
                    if (status == "Ready")
                    {
                        item.ReadyQuantity = item.ReadyQuantity - Item.ReadyQuantity;
                    }
                    else
                    {
                        item.ReadyQuantity = item.ReadyQuantity + Item.ReadyQuantity;
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}