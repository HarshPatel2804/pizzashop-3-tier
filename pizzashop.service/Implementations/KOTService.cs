using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Services
{
    public class KOTService : IKOTService
    {
        private readonly IKOTRepository _kotRepository;
        private readonly IMenuService _menuService;

        public KOTService(IKOTRepository kotRepository, IMenuService menuService)
        {
            _kotRepository = kotRepository;
            _menuService = menuService;
        }

        public async Task<KOTViewModel> GetKOTViewModel()
        {
            var categories = await _menuService.GetAllCategories();

            var viewModel = new KOTViewModel
            {
                categories = categories.Select(c => new CategoryViewModel
                {
                    Categoryid = c.Categoryid,
                    Categoryname = c.Categoryname
                }).ToList()
            };

            return viewModel;
        }

        public async Task<(List<KOTOrdersViewModel>, int totalPages)> GetKOTOrders(string categoryId, string status, int page, int itemsPerPage)
        {
            int? categoryIdInt = null;
            if (categoryId != "all" && int.TryParse(categoryId, out int parsedCategoryId))
            {
                categoryIdInt = parsedCategoryId;
            }

            var skip = (page - 1) * itemsPerPage;

            var (flatOrders, totalOrders) = await _kotRepository.GetKOTOrdersByCategoryAndStatus(categoryIdInt, status, skip, itemsPerPage);

            var ordersViewModel = flatOrders
                .GroupBy(fo => fo.OrderId)
                .Skip(skip)
                .Take(itemsPerPage)
                .Select(orderGroup => new KOTOrdersViewModel
                {
                    OrderId = orderGroup.Key,
                    OrderDate = orderGroup.First().OrderDate,
                    TableName = orderGroup.First().TableName,
                    SectionName = orderGroup.First().SectionName,
                    Instruction = orderGroup.First().OrderInstruction,
                    Items = orderGroup
                        .GroupBy(fo => fo.OrderedItemId)
                        .Select(itemGroup => new KOTOrderedItemsViewModel
                        {
                            Ordereditemid = itemGroup.Key,
                            ItemName = itemGroup.First().ItemName,
                            Quantity = itemGroup.First().ItemQuantity,
                            Instruction = itemGroup.First().ItemInstruction,
                            Modifiers = itemGroup
                                .Where(fo => fo.ModifierId > 0)
                                .Select(fo => new KOTModifierViewModel
                                {
                                    ModifierId = fo.ModifierId,
                                    ModifierName = fo.ModifierName
                                })
                                .Distinct()
                                .ToList()
                        })
                        .Where(item => item.Quantity > 0)
                        .ToList()
                })
                .Where(o => o.Items.Any())
                .ToList();

            return (ordersViewModel, totalOrders);
        }
        public async Task<int> UpdatePreparedQuantities(List<PreparedItemviewModel> updates, string status)
        {
            return await _kotRepository.UpdatePreparedQuantities(updates, status);
        }

    }
}