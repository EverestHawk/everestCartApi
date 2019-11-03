using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.ViewModels;
using WebApi.Interfaces;

namespace WebApi.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IAppLogger<CatalogService> _logger;
        private readonly IAsyncRepository<CatalogBrand> _brandRepository;
        private readonly IAsyncRepository<CatalogType> _typeRepository;
        private readonly IRepository<CatalogItem> _catalogRepository;
        private readonly IUriComposer _uriComposer;
        public CatalogService(IRepository<CatalogItem> catalogRepository,
            IAsyncRepository<CatalogBrand> brandRepository,
            IAsyncRepository<CatalogType> typeRepository,
            IAppLogger<CatalogService> logger,
            IUriComposer uriComposer)
        {
            _catalogRepository = catalogRepository;
            _brandRepository = brandRepository;
            _typeRepository = typeRepository;
            _logger = logger;
            _uriComposer = uriComposer;
        }
        public async Task<IEnumerable<SelectListItem>> GetBrands(int? brandId)
        {
            _logger.LogInfo("GetBrands called...");

            brandId = brandId ?? 0;
            var brands = await _brandRepository.ListAllAsync();

            var items = new List<SelectListItem>
            {
                new SelectListItem { Value = "All", Text = "All", Selected = brandId == 0 }
            };

            foreach (var item in brands)
            {
                items.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Brand, Selected = brandId == item.Id });
            }

            _logger.LogInfo("GetBrands completed successfully.");

            return items;
        }

        public async Task<CatalogIndexViewModel> GetCatalogs(int pageIndex, int itemsPage, int? brandId, int? typeId)
        {
            _logger.LogInfo($"GetCatalogs called for PageIndex : {pageIndex}, ItemsPage : {itemsPage}, BrandId : {brandId ?? 0}, TypeId : {typeId ?? 0}.");

            var filterSpec = new CatalogFilterSpecification(brandId, typeId);
            var catalogs = _catalogRepository.List(filterSpec);

            var totalItems = catalogs.Count();

            var itemsOnPage = catalogs
                .Skip(itemsPage * pageIndex)
                .Take(itemsPage)
                .ToList();

            itemsOnPage.ForEach(item =>
            {
                item.PictureUri = _uriComposer.ComposePictureUri(item.PictureUri);
            });

            var vm = new CatalogIndexViewModel
            {
                CatalogItems = itemsOnPage.Select(i => new CatalogItemViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Price = i.Price,
                    PictureUri = i.PictureUri
                }),
                Brands = await GetBrands(brandId),
                Types = await GetTypes(typeId),
                BrandId = brandId??0,
                TypeId = typeId??0,
                PaginationInfo = new PaginationInfoViewModel
                {
                    ActualPage = pageIndex,
                    ItemsPerPage = itemsOnPage.Count,
                    TotalItems = totalItems,
                    TotalPages = int.Parse(Math.Ceiling(((decimal)totalItems/itemsPage)).ToString())
                }
            };

            vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";

            _logger.LogInfo($"GetCatalogs for PageIndex : {pageIndex}, ItemsPage : {itemsPage}, BrandId : {brandId ?? 0}, TypeId : {typeId ?? 0} completed successfully.");

            return vm;
        }

        public async Task<IEnumerable<SelectListItem>> GetTypes(int? typeId)
        {
            _logger.LogInfo("GetTypes called...");

            typeId = typeId ?? 0;

            var types = await _typeRepository.ListAllAsync();

            var items = new List<SelectListItem>
            {
                new SelectListItem { Value = "All", Text = "All", Selected = typeId == 0 }
            };

            foreach (var type in types)
            {
                items.Add(new SelectListItem { Value = type.Id.ToString(), Text = type.Type, Selected = typeId == type.Id });
            }

            _logger.LogInfo("GetTypes completed successfully.");

            return items;
        }
    }
}
