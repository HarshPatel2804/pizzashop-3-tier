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

        public async Task<List<KOTOrdersViewModel>> GetKOTOrdersByCategoryAndStatus(int? categoryId, string status, int skip, int take)
        {
            var query = _context.Orders
                .Where(o => o.OrderStatus == orderstatus.InProgress)
                .OrderByDescending(o => o.Orderdate)
                .AsQueryable();

            if (!categoryId.HasValue)
            {
                var orders = await query
                    .Where(o => o.Ordereditems.Any())
                    .Skip(skip)
                    .Take(take)
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

                return orders.Where(o => o.Items.Any()).ToList();
            }
            else
            {
                var ordersWithSpecifiedCategory = await query
                    .Where(o => o.Ordereditems.Any(oi => oi.Item.Categoryid == categoryId.Value))
                    .Skip(skip)
                    .Take(take)
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

                return ordersWithSpecifiedCategory
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
                    .ToList();
            }
        }
        public async Task UpdatePreparedQuantities(List<PreparedItemviewModel> updates, string status)
        {
            if (updates.Any())
            {
                var orderitem = await _context.Ordereditems.FirstOrDefaultAsync(oi => oi.Ordereditemid == updates[0].OrderedItemId);
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.Orderid == orderitem.Orderid);
                if(order.ServedTime == null){
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