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

        public async Task<List<KOTOrdersViewModel>> GetKOTOrders(string categoryId, string status, int page, int itemsPerPage)
        {

            int? categoryIdInt = null;
            if (categoryId != "all" && int.TryParse(categoryId, out int parsedCategoryId))
            {
                categoryIdInt = parsedCategoryId;
            }

            var skip = (page - 1) * itemsPerPage;

            var orders = await _kotRepository.GetKOTOrdersByCategoryAndStatus(categoryIdInt, status, skip, itemsPerPage);

            return orders;
        }

        public void UpdatePreparedQuantities(List<PreparedItemviewModel> updates , string status)
        {
            _kotRepository.UpdatePreparedQuantities(updates , status);
        }

    }
}