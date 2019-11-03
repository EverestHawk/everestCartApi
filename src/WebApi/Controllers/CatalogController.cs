using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApi.Interfaces;

namespace WebApi.Controllers
{
    [Authorize]
    public class CatalogController : BaseApiController
    {
        private readonly ICatalogService _catalogService;
        private readonly IAppLogger<CatalogController> _logger;

        public CatalogController(ICatalogService catalogService, 
            IAppLogger<CatalogController> logger)
        {
            _catalogService = catalogService;
            _logger = logger;
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> getCatalogItems(int? brandId, int? typeId, int? page, int? itemsPage)
        {
            _logger.LogInfo($"getCatalogItems: Called for BrandId: {brandId ?? 0}, typeId: {typeId ?? 0}, page: {page ?? 0}, itemsPage: {itemsPage ?? 0}.");

            var viewModel = await _catalogService.GetCatalogs(page ?? 0, itemsPage ?? 10, brandId, typeId);
            viewModel.MachineName = Environment.MachineName;

            _logger.LogInfo("getCatalogItems: Completed successfully for BrandId: {0}, typeId: {1}, page: {2}, itemsPage: {3}.", brandId ?? 0, typeId ?? 0, page ?? 0, itemsPage ?? 0);

            return Ok(viewModel);
        }
    }
}