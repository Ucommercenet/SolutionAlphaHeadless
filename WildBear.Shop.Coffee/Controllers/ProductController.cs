﻿using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using WildBearAdventures.MVC.ViewModels;
using WildBearAdventures.MVC.WildBear.Context;
using WildBearAdventures.MVC.WildBear.Models.Request;
using WildBearAdventures.MVC.WildBear.TransactionApi;
using WildBearAdventures.MVC.WildBear.WildBearApi;



namespace WildBearAdventures.MVC.Controllers
{

    public class ProductController : Controller
    {
        private readonly IStoreApiClient _wildBearClient;
        private readonly IContextHelper _contextHelper;
        private readonly TransactionClient _transactionClient;

        public ProductController(IStoreApiClient wildBearClient, IContextHelper contextHelper, TransactionClient transactionClient)
        {
            _wildBearClient = wildBearClient;
            _contextHelper = contextHelper;
            _transactionClient = transactionClient;
        }

        [HttpGet]
        public IActionResult Index(string productName, CancellationToken ct)
        {
            //Handout Part 3: Product details
            #region Handout
            var currentProductDto = _wildBearClient.GetSingleProductByName(productName, ct);

            var hasPrice = currentProductDto.UnitPrices.TryGetValue("EUR 15 pct", out var price);           

            var productViewModel = new ProductViewModel()
            {
                Name = currentProductDto.Name,
                ShortDescription = currentProductDto?.ShortDescription ?? "No Description",
                Price = hasPrice ? price : 0,
            };

            return View(productViewModel);
            #endregion            
        }



        [HttpPost]
        public async Task<RedirectToActionResult> AddToCart(string productName, CancellationToken ct, int quantity = 1)
        {
            //Handout Part 5 Add products to the shopping cart
            #region Handout
            //Greater information
            var currency = "EUR"; //TODO Improvement: Get dynamic
            var cultureCode = "da-DK"; //TODO Improvement: Get dynamic


            var currentCategory = _contextHelper.GetCurrentCategoryGuid() ?? throw new Exception("No Category found");
            var currentCatalog = _wildBearClient.GetSingleCategoryByGuid(currentCategory, ct).CatalogId;


            //New or current Shopping Cart
            var basketGuid = FindCurrentShoppingCartOrCreateNew(currency, cultureCode, ct);
            _contextHelper.SetCurrentCart(basketGuid);

            //Product information
            var product = _wildBearClient.GetSingleProductByName(productName, ct);
            var priceGroupGuid = product.PriceGroupIds.FirstOrDefault();

            //Ready the Request
            var request = new ShoppingCartLineUpdateRequest
            {
                ShoppingCart = basketGuid,
                CultureCode = cultureCode,
                Quantity = quantity,
                PriceGroupGuid = new Guid(priceGroupGuid),
                Catalog = new Guid(currentCatalog),
                Sku = product.Sku,
                VariantSku = product.VariantSku
            };

            //Send the Request 
            await _transactionClient.PostShoppingCartLineUpdate(request, ct);

            //Update MiniCart
            _contextHelper.UpdateMiniCartCount(quantity);


            //After the product has been added, show the product again. 
            #endregion
            return RedirectToAction(actionName: "Index", routeValues: new { productName });
        }

        private Guid FindCurrentShoppingCartOrCreateNew(string currency, string cultureCode, CancellationToken ct)
        {
            var currentBasketGuid = _contextHelper.GetCurrentCartGuid();

            //currentBasket did exists
            if (currentBasketGuid.HasValue is true)
            { return (Guid)currentBasketGuid; }

            //currentBasket did not exists
            var basketGuid = _transactionClient.PostCreateBasket(currency, cultureCode, ct).Result;
            _contextHelper.SetCurrentCart(basketGuid);
            return basketGuid;

        }
    }
}
