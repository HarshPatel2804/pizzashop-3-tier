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
    private readonly ICustomerRepository _customerRepository;
    private readonly Lazy<ITableSectionService> _tableSectionServiceLazy;

    public WaitingTokenService(IWaitingTokenRepository waitingTokenRepository, ICustomerService customerService, Lazy<ITableSectionService> tableSectionService , ICustomerRepository customerRepository)
    {
        _waitingTokenRepository = waitingTokenRepository;
        _customerService = customerService;
        _tableSectionServiceLazy = tableSectionService;
        _customerRepository = customerRepository;
    }

    public async Task<int> SaveWaitingToken(WaitingtokenViewModel model)
    {
        var customer = await _customerService.GetCustomerByEmail(model.Email);
        int customerId = 0;
        if (customer == null)
        {
            var CustomerModel = new Customer
            {
                Customername = model.Customername,
                Email = model.Email,
                Phoneno = model.Phoneno
            };
            customerId = await _customerService.AddCustomer(CustomerModel);
        }
        else
        {
            customerId = customer.Customerid;
        }
        var WaitingTokenModel = new Waitingtoken
        {
            Sectionid = model.Sectionid,
            Noofpeople = model.Noofpeople,
            Customerid = customerId
        };

        return await _waitingTokenRepository.SaveWaitingToken(WaitingTokenModel);
    }

    public async Task<IEnumerable<WaitingtokenViewModel>> GetAllWaitingTokens(int section)
    {
        var tokens = await _waitingTokenRepository.GetAllWaitingTokensWithCustomer(section);

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
        return await _waitingTokenRepository.GetAllWaitingTokensWithCustomer(sectionId);
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
            if (string.IsNullOrEmpty(viewModel.Email))
            {
                return (false, "Customer email is required to update details.");
            }

            Customer? existingCustomer = await _customerRepository.GetCustomerByEmail(viewModel.Email);
            if (existingCustomer == null)
            {
                return (false, $"Customer with email '{viewModel.Email}' not found.");
            }

 
            var customer = new Customer
            {
                Customerid = existingCustomer.Customerid, 
                Email = viewModel.Email, 
                Customername = viewModel.Customername,
                Phoneno = viewModel.Phoneno,
            };

            int updatedCustomerId = await _customerRepository.UpdateCustomer(customer);
            if (updatedCustomerId == 0)
            {
                return (false, "Failed to update customer details.");
            }

            Waitingtoken? waitingToken = await _waitingTokenRepository.GetByIdAsync(viewModel.Waitingtokenid);
            if (waitingToken == null)
            {
                return (false, "Waiting token not found.");
            }

            waitingToken.Noofpeople = viewModel.Noofpeople;
            waitingToken.Sectionid = viewModel.Sectionid;

            bool tokenUpdated = await _waitingTokenRepository.UpdateWaitingTokenAsync(waitingToken);

            if (tokenUpdated)
            {
                return (true, "Waiting token details updated successfully.");
            }
            else
            {
                return (false, "Failed to update waiting token information.");
            }
        }

}
