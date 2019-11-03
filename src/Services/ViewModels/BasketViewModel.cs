using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.ViewModels
{
    public class BasketViewModel
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public List<BasketItemViewModel> Items { get; set; } = new List<BasketItemViewModel>();

        public decimal Total()
        {
            return Math.Round(Items.Sum(x => x.UnitPrice * x.Quantity), 2);
        }
    }
}
