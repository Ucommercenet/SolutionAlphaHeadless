﻿namespace WildBearAdventures.MVC.Models
{
    public class ShoppingCartViewModel
    {
        public required List<OrderLineViewModel> ShoppingChartOrderLineViewModels { get; set; }

        public decimal ShoppingCartOrderTotal { get; set; }

        public Guid ShoppingCartGuid { get; set; }


    }





}
