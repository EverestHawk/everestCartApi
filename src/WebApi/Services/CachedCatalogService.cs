using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Services.ViewModels;
using WebApi.Interfaces;

namespace WebApi.Services
{
    public class CachedCatalogService : ICatalogService
    {
        private readonly IAppLogger<CachedCatalogService> _logger;
        private readonly CatalogService _catalogService;
        private readonly IMemoryCache _cache;
        private static readonly string _brandsTemplate = "brands_{0}";
        private static readonly string _typesTemplate = "types_{0}";
        private static readonly string _catalogsTemplate = "items_{0}_{1}_{2}_{3}";
        private static readonly TimeSpan _defaultCacheDuration = TimeSpan.FromSeconds(30);
        public CachedCatalogService(IAppLogger<CachedCatalogService> logger,
            IMemoryCache cache,
            CatalogService catalogService)
        {
            _logger = logger;
            _cache = cache;
            _catalogService = catalogService;
        }
        public async Task<IEnumerable<SelectListItem>> GetBrands(int? brandId)
        {
            string cacheKey = string.Format(_brandsTemplate, brandId ?? 0);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _catalogService.GetBrands(brandId);
            });
        }

        public async Task<CatalogIndexViewModel> GetCatalogs(int pageIndex, int itemsPage, int? brandId, int? typeId)
        {
            string cacheKey = string.Format(_catalogsTemplate, pageIndex, itemsPage, brandId ?? 0, typeId ?? 0);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _catalogService.GetCatalogs(pageIndex, itemsPage, brandId, typeId);
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetTypes(int? typeId)
        {
            string cacheKey = string.Format(_typesTemplate, typeId ?? 0);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _catalogService.GetTypes(typeId);
            });
        }
    }
}
