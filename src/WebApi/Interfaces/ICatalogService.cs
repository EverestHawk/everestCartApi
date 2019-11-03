using Microsoft.AspNetCore.Mvc.Rendering;
using Services.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Interfaces
{
    public interface ICatalogService
    {
        Task<CatalogIndexViewModel> GetCatalogs(int pageIndex, int itemsPage, int? brandId, int? typeId);
        Task<IEnumerable<SelectListItem>> GetBrands(int? brandId);
        Task<IEnumerable<SelectListItem>> GetTypes(int? typeId);
    }
}
