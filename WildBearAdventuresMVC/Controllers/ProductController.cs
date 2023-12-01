﻿using Microsoft.AspNetCore.Mvc;
using WildBearAdventuresMVC.Models;
using WildBearAdventuresMVC.WildBear.Interfaces;
using WildBearAdventuresMVC.WildBear.TransactionApi;
using WildBearAdventuresMVC.WildBear.TransactionApi.Models;

namespace WildBearAdventuresMVC.Controllers
{

    public class ProductController : Controller
    {
        private readonly IStoreApiClient _wildBearApiClient;
        private readonly IContextHelper _contextHelper;
        private readonly TransactionClient _transactionClient;

        public ProductController(IStoreApiClient wildBearApiClient, IContextHelper contextHelper, TransactionClient transactionClient)
        {
            _wildBearApiClient = wildBearApiClient;
            _contextHelper = contextHelper;
            _transactionClient = transactionClient;
        }

        [HttpGet]
        public IActionResult Index(CancellationToken ct)
        {

            var ableToGetRoute = HttpContext.Request.RouteValues.TryGetValue("id", out var name);
            if (ableToGetRoute)
            { _contextHelper.SetCurrentProductByName(name.ToString()); }

            var productViewModel = CreateProductViewModel(ct);


            return View(productViewModel);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> AddToCart(Guid? productGuid, CancellationToken ct, int quantity = 1)
        {
            var currency = "EUR"; //TODO: Get dynamic
            var cultureCode = "da-DK"; //TODO: Get dynamic


            var currentCategory = _contextHelper.GetCurrentCategoryGuid() ?? throw new Exception("No Category found");
            var currentCatalog = _wildBearApiClient.GetSingleCategoryByGuid(currentCategory, ct).CatalogId;



            var basketGuid = _transactionClient.CreateBasket(currency, cultureCode, ct).Result;
            _contextHelper.SetCurrentCart(basketGuid);

            var currentProductGuid = (productGuid ?? _contextHelper.GetCurrentProductGuid()) ?? throw new Exception("No product found");
            var product = _wildBearApiClient.GetSingleProductByGuid(currentProductGuid, ct);
            var priceGroupGuid = product.PriceGroupIds.First();


            var request = new UpdateOrderLineQuantityRequest
            {
                ShoppingCart = basketGuid,
                CultureCode = cultureCode,
                Quantity = quantity,
                PriceGroupGuid = priceGroupGuid,
                Catalog = currentCatalog,
                Sku = product.Sku,
                VariantSku = product.VariantSku
            };

            await _transactionClient.UpdateOrderLineQuantity(request, ct);


            _contextHelper.UpdateCurrentShoppingCartCount(quantity);
            //After the product has been added, show the product again.
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Uses the ContextHelper to find current product
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        private ProductViewModel CreateProductViewModel(CancellationToken ct)
        {
            var currentproductGuid = _contextHelper.GetCurrentProductGuid();
            var currentprodcutDto = _wildBearApiClient.GetSingleProductByGuid((Guid)currentproductGuid, ct);

            var productViewModel = new ProductViewModel()
            {
                Name = currentprodcutDto.Name,
                ShortDescription = currentprodcutDto?.ShortDescription,
                Price = currentprodcutDto.UnitPrices.FirstOrDefault().Value,
            };
            return productViewModel;
        }



    }
}
