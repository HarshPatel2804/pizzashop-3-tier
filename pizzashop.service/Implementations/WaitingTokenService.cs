using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class WaitingTokenService : IWaitingTokenService
{
    private readonly IWaitingTokenRepository _waitingTokenRepository;

    private readonly ICustomerService _customerService;
    private readonly IOrderService _orderService;
    private readonly ICustomerRepository _customerRepository;
    private readonly ITableSectionRepository _tableSectionRepository;
    private readonly Lazy<ITableSectionService> _tableSectionServiceLazy;

    public WaitingTokenService(IWaitingTokenRepository waitingTokenRepository, ICustomerService customerService, Lazy<ITableSectionService> tableSectionService, ICustomerRepository customerRepository, IOrderService orderService , ITableSectionRepository tableSectionRepository)
    {
        _waitingTokenRepository = waitingTokenRepository;
        _customerService = customerService;
        _tableSectionServiceLazy = tableSectionService;
        _customerRepository = customerRepository;
        _orderService = orderService;
        _tableSectionRepository = tableSectionRepository;
    }

    public async Task<(bool , string)> SaveWaitingToken(WaitingtokenViewModel model)
    {
        // var customer = await _customerService.GetCustomerByEmail(model.Email);
        // int customerId = 0;
        // if (customer == null)
        // {
        //     var CustomerModel = new Customer
        //     {
        //         Customername = model.Customername,
        //         Email = model.Email,
        //         Phoneno = model.Phoneno
        //     };
        //     customerId = await _customerService.AddCustomer(CustomerModel);
        // }
        // else
        // {
        //     customerId = customer.Customerid;
        // }
        // var WaitingTokenModel = new Waitingtoken
        // {
        //     Sectionid = model.Sectionid,
        //     Noofpeople = model.Noofpeople,
        //     Customerid = customerId
        // };

        // return await _waitingTokenRepository.SaveWaitingToken(WaitingTokenModel);

        var result = await _waitingTokenRepository.AddWaitingTokenAsync(model);
        return(result.Success, result.Message);
    }

    public async Task<IEnumerable<WaitingtokenViewModel>> GetAllWaitingTokens(List<int> sectionIds)
    {
        ITableSectionService _tableSectionService = _tableSectionServiceLazy.Value;
        var tokens = await _waitingTokenRepository.GetAllWaitingTokens(sectionIds);

        var result = tokens.Select(token => new WaitingtokenViewModel
        {
            Waitingtokenid = token.Waitingtokenid,
            Noofpeople = token.Noofpeople,
            Email = token.Customer.Email,
            Phoneno = token.Customer.Phoneno,
            Customername = token.Customer.Customername,
        }).ToList();

        return result;
    }

    public async Task<bool> IsCustomerInWaitingList(int customerId)
    {
        return await _waitingTokenRepository.IsCustomerInWaitingList(customerId);
    }

    public async Task WaitingToAssign(int tokenId)
    {
        await _waitingTokenRepository.WaitingToAssign(tokenId);
    }

    public async Task<WaitingListViewModel> GetWaitingData()
    {
        ITableSectionService _tableSectionService = _tableSectionServiceLazy.Value;
        var model = new WaitingListViewModel
        {
            sections = await _tableSectionService.GetAllSections(),

        };
        return model;
    }

    public async Task<IEnumerable<Waitingtoken>> GetWaitingTokensBySectionAsync(int sectionId)
    {
        var Result = await _waitingTokenRepository.GetAllWaitingTokensWithCustomer(sectionId);
        
        return Result.Select(vm => new Waitingtoken
        {
            Waitingtokenid = vm.Waitingtokenid,
            Createdat = vm.Createdat,
            Createdby = vm.Createdby,
            Customerid = vm.Customerid,
            Isassigned = vm.Isassigned,
            Modifiedat = vm.Modifiedat,
            Modifiedby = vm.Modifiedby,
            Noofpeople = vm.Noofpeople,
            Sectionid = vm.Sectionid,
            Customer = new Customer
            {
                Customerid = vm.Customerid,
                Customername = vm.Customername,
                Email = vm.Email,
                Phoneno = vm.Phoneno,
                Totalorder = vm.Totalorder,
                Visitcount = (short?)vm.Visitcount
            }
        }).ToList();
    }

    public async Task<bool> RemoveWaitingTokenAsync(int tokenId)
    {
        return await _waitingTokenRepository.DeleteAsync(tokenId);
    }

    public async Task<(bool success, string message, WaitingtokenViewModel? model)> GetWaitingTokenForEditAsync(int tokenId)
    {
        var token = await _waitingTokenRepository.GetTokenByIdWithCustomerAsync(tokenId);
        if (token == null || token.Customer == null)
        {
            return (false, "Waiting token not found.", null);
        }

        var sections = await _tableSectionServiceLazy.Value.GetAllSections();

        var viewModel = new WaitingtokenViewModel
        {
            Waitingtokenid = token.Waitingtokenid,
            Noofpeople = token.Noofpeople,
            Sectionid = token.Sectionid,
            Isassigned = token.Isassigned,
            Customername = token.Customer.Customername,
            Email = token.Customer.Email,
            Phoneno = token.Customer.Phoneno,
            Sections = sections.Select(s => new SelectListItem
            {
                Value = s.Sectionid.ToString(),
                Text = s.Sectionname
            }).ToList()
        };
        return (true, "Data retrieved successfully.", viewModel);
    }

    public async Task<(bool success, string message)> UpdateWaitingTokenDetailsAsync(WaitingtokenViewModel viewModel)
    {
        // if (string.IsNullOrEmpty(viewModel.Email))
        // {
        //     return (false, "Customer email is required to update details.");
        // }

        // Customer? existingCustomer = await _customerRepository.GetCustomerByEmail(viewModel.Email);
        // if (existingCustomer == null)
        // {
        //     return (false, $"Customer with email '{viewModel.Email}' not found.");
        // }


        // var customer = new Customer
        // {
        //     Customerid = existingCustomer.Customerid,
        //     Email = viewModel.Email,
        //     Customername = viewModel.Customername,
        //     Phoneno = viewModel.Phoneno,
        // };

        // int updatedCustomerId = await _customerRepository.UpdateCustomer(customer);
        // if (updatedCustomerId == 0)
        // {
        //     return (false, "Failed to update customer details.");
        // }

        // Waitingtoken? waitingToken = await _waitingTokenRepository.GetByIdAsync(viewModel.Waitingtokenid);
        // if (waitingToken == null)
        // {
        //     return (false, "Waiting token not found.");
        // }

        // waitingToken.Noofpeople = viewModel.Noofpeople;
        // waitingToken.Sectionid = viewModel.Sectionid;

        // bool tokenUpdated = await _waitingTokenRepository.UpdateWaitingTokenAsync(waitingToken);

        // if (tokenUpdated)
        // {
        //     return (true, "Waiting token details updated successfully.");
        // }
        // else
        // {
        //     return (false, "Failed to update waiting token information.");
        // }
        var result = await _waitingTokenRepository.UpdateWaitingTokenAsync(viewModel);
        return (result.Success, result.Message);
    }

    public async Task<(string, int)> AssignTable(WaitingAssignViewModel model)
    {

        var token = await _waitingTokenRepository.GetTokenByIdWithCustomerAsync(model.WaitingTokenId);
        int totalCapacity = 0;
        foreach (int tableId in model.SelectedTableIds)
        {
            var tableDetails = await _tableSectionServiceLazy.Value.GetTableById(tableId);
            totalCapacity += (int)tableDetails.Capacity;
        }

        if (totalCapacity < token.Noofpeople)
        {
            var result = $"Selected tables don't have enough capacity. Required: {token.Noofpeople}, Available: {totalCapacity}";
            return (result, 0);
        }

        //Create Order
        int orderId = await _orderService.createOrderbycustomerId(token.Customerid , token.Noofpeople);

        //Order Table mapping
        List<Ordertable> orderTables = new List<Ordertable>();
        foreach (int tableId in model.SelectedTableIds)
        {
            orderTables.Add(new Ordertable
            {
                Orderid = orderId,
                Tableid = tableId
            });

            //update Table status to occupied
            await _tableSectionRepository.UpdateTableStatusToOccupied(tableId);
        }

        await _tableSectionRepository.AddOrderTables(orderTables);

        if (model.WaitingTokenId != null)
        {
            await WaitingToAssign(model.WaitingTokenId);
        }

        return ("true", orderId);
    }


}
