using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Services.ViewModels
{
    public class CatalogIndexViewModel
    {
        public IEnumerable<CatalogItemViewModel> CatalogItems { get; set; }
        public IEnumerable<SelectListItem> Brands { get; set; }
        public IEnumerable<SelectListItem> Types { get; set; }
        public int? BrandId { get; set; }
        public int? TypeId { get; set; }
        public PaginationInfoViewModel PaginationInfo { get; set; }
        public string MachineName { get; set; }
    }
}
