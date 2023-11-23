﻿namespace WildBearAdventuresMVC.WildBear.TransactionApi.Models
{
    public class UpdateOrderLineQuantityRequest
    {

        public required Guid ShoppingCart { get; set; }
        public required string CultureCode { get; set; }
        public required int Quantity { get; set; }
        public required string Sku { get; set; }
        public required Guid PriceGroupGuid { get; set; }
        public required Guid Catalog { get; set; }
        public string? VariantSku { get; set; }



    }
}
