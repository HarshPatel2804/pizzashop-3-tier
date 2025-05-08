
namespace pizzashop.repository.ViewModels;

public class SaveRatingViewModel
{
    public int OrderId { get; set; }
    public RatingModal Ratings { get; set; }
    public string Comment { get; set; }
}

public class RatingModal
{
    public decimal? Food { get; set; }
    public decimal? Service { get; set; }
    public decimal? Ambience { get; set; }
}

