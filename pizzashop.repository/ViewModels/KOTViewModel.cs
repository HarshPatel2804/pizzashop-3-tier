using pizzashop.repository.Models;

namespace pizzashop.repository.ViewModels;

public class KOTViewModel{
    public List<OrderViewModelInKOT> orders{get;set;}
    public List<CategoryViewModel> categories{get;set;}
    
}
public class ItemViewModelInKOT{
    public string itemName{get;set;}
    public int ordertoitem{get;set;}
    public int categoryId{get;set;}
    public int totalItems{get;set;}
    public List<string> itemsModifiers{get;set;}
    public string itemInstruction{get;set;}
}

public class OrderViewModelInKOT{
    public int orderId{get;set;}
    public TimeSpan orderTime{get;set;}
    public string orderSection{get;set;}
    public string orderTable{get;set;}
    public List<ItemViewModelInKOT> items{get;set;}
    public List<int> categoryIds{get;set;}
    public DateTime Createdtime{get;set;}
    public string status{get;set;}

}