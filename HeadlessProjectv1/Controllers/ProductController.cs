﻿using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Ucommerce.Extensions.Search.Abstractions.Models.IndexModels;
using Ucommerce.Extensions.Search.Abstractions.Models.SearchModels;
using Ucommerce.Web.Infrastructure.Core.Models;

namespace HeadlessProjectv1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IIndex _index;

        public ProductController(IIndex index)
        {
            _index = index;
        }

        //test with product A001
        [HttpGet("GetProductName")]
        public string GetProductName(string searchSku)
        {
            var language = new Language()
            {
                Name = "Danish",
                Culture = new CultureInfo("da-DK")
            };

            var indexSearch = _index.AsSearchable<ProductSearchModel>(language.Culture);

            var productResultSet = indexSearch.Where(x => x.Sku == searchSku)
                .ToResultSet(new CancellationToken()).Result;


            var result = productResultSet.FirstOrDefault()?.Name;

            return result ??= "No product found";
        }
    }
}