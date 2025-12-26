using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.DataAccsess.Repository;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.Models;

public class SliderItemRepository : Repository<SliderItem>, ISliderItemRepository
{
    private readonly ApplicationDbContext _db;

    public SliderItemRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(SliderItem sliderItem)
    {
        var objFromDb = _db.SliderItems.FirstOrDefault(s => s.Id == sliderItem.Id);
        if (objFromDb != null)
        {
            objFromDb.Title = sliderItem.Title;
            objFromDb.Description = sliderItem.Description;
            objFromDb.DisplayOrder = sliderItem.DisplayOrder;
            if (sliderItem.ImageUrl != null)
            {
                objFromDb.ImageUrl = sliderItem.ImageUrl;
            }
        }
    }
}