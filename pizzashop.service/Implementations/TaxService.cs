using pizzashop.repository.Interfaces;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class TaxService : ITaxService
{
    private readonly ITaxRepository  _taxRepository;

    public TaxService(ITaxRepository  taxRepository)
    {
        _taxRepository = taxRepository;;
    }

    public async Task<(List<TaxViewModel> tableModel, int totalTaxes, int totalPages)> GetTaxes(int page, int pageSize, string search)
    {
        var (model ,  totalTaxes) = await _taxRepository.GetAllTaxesAsync(page , pageSize , search);

        int totalPages = (int)System.Math.Ceiling((double)totalTaxes / pageSize);

        var viewModel = new List<TaxViewModel>();
 
        foreach (var u in model)
        {
            var taxType = await _taxRepository.GetTaxTypeDetails(u.TaxTypeId);

            viewModel.Add(new TaxViewModel
            {
                Taxid = u.Taxid,
                Taxname = u.Taxname,
                TaxTypeId = u.TaxTypeId,
                Taxvalue = u.Taxvalue,
                TaxType = taxType,
                Isenabled = u.Isenabled,
                Isdefault = u.Isdefault
            });
        }

        return (viewModel , totalTaxes , totalPages );
       
    }
}
