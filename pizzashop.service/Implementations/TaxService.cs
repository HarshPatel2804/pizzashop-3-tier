using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
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
                Isenabled = (bool)u.Isenabled,
                Isdefault = (bool)u.Isdefault
            });
        }

        return (viewModel , totalTaxes , totalPages );
       
    }

    public async Task<TaxViewModel> GetTaxById(int id)
    {
        var tax = await _taxRepository.GetTaxByIdAsync(id);
        if (tax == null)
            return null;

        var taxType = await _taxRepository.GetTaxTypeDetails(tax.TaxTypeId);
        var taxTypes = await _taxRepository.GetAllTaxTypesAsync();

        return new TaxViewModel
        {
            Taxid = tax.Taxid,
            Taxname = tax.Taxname,
            TaxTypeId = tax.TaxTypeId,
            Taxvalue = tax.Taxvalue,
            TaxType = taxType,
            Isenabled = (bool)tax.Isenabled,
            Isdefault = (bool)tax.Isdefault,
            TaxTypes = taxTypes.Select(t => new SelectListItem
            {
                Value = t.TaxTypeId.ToString(),
                Text = t.TaxName
            })
        };
    }

    public async Task<TaxViewModel> PrepareNewTaxViewModel()
    {
        var taxTypes = await _taxRepository.GetAllTaxTypesAsync();

        return new TaxViewModel
        {
            Isenabled = true,
            TaxTypes = taxTypes.Select(t => new SelectListItem
            {
                Value = t.TaxTypeId.ToString(),
                Text = t.TaxName
            })
        };
    }

    public async Task<bool> CreateTax(TaxViewModel viewModel)
    {
        var tax = new Taxis
        {
            Taxname = viewModel.Taxname,
            TaxTypeId = viewModel.TaxTypeId,
            Taxvalue = viewModel.Taxvalue,
            Isenabled = viewModel.Isenabled,
            Isdefault = viewModel.Isdefault,
            Isdeleted = false,
        };

        return await _taxRepository.CreateTaxAsync(tax);
    }

    public async Task<bool> UpdateTax(TaxViewModel viewModel)
    {
        var existingTax = await _taxRepository.GetTaxByIdAsync(viewModel.Taxid);
        if (existingTax == null)
            return false;

        existingTax.Taxname = viewModel.Taxname;
        existingTax.TaxTypeId = viewModel.TaxTypeId;
        existingTax.Taxvalue = viewModel.Taxvalue;
        existingTax.Isenabled = viewModel.Isenabled;
        existingTax.Isdefault = viewModel.Isdefault;

        return await _taxRepository.UpdateTaxAsync(existingTax);
    }

    public async Task<bool> DeleteTax(int id)
    {
        return await _taxRepository.DeleteTaxAsync(id);
    }

    public async Task<Taxis> GetTaxByName(TaxViewModel model)
        {
            string result = System.Text.RegularExpressions.Regex.Replace(model.Taxname, @"\s+", " ");
        model.Taxname = result.Trim();
            return await _taxRepository.GetTaxByName(model);
        }
}
