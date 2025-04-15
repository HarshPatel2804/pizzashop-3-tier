using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class WaitingTokenService : IWaitingTokenService
{
    private readonly IWaitingTokenRepository _waitingTokenRepository;

    private readonly ICustomerService _customerService;

    public WaitingTokenService(IWaitingTokenRepository waitingTokenRepository , ICustomerService customerService)
    {
        _waitingTokenRepository = waitingTokenRepository;
        _customerService = customerService;
    }

    public async Task<int> SaveWaitingToken(WaitingtokenViewModel model)
    {
        var customer = await _customerService.GetCustomerByEmail(model.Email);
        int customerId = 0;
        if(customer == null){
            var CustomerModel = new Customer{
                Customername = model.Customername,
                Email = model.Email,
                Phoneno = model.Phoneno
            };
        customerId = await _customerService.AddCustomer(CustomerModel);
        }
        else{
        customerId = customer.Customerid;
        }
        var WaitingTokenModel = new Waitingtoken {
            Sectionid = model.Sectionid,
            Noofpeople = model.Noofpeople,
            Customerid = customerId
        };

        return await _waitingTokenRepository.SaveWaitingToken(WaitingTokenModel);
    }

}
