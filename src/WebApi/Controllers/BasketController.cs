using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Interfaces;

namespace WebApi.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly IBasketService _basketService;
        private readonly IBasketViewModelService _basketViewModelService;
        public BasketController()
        {

        }
    }
}
