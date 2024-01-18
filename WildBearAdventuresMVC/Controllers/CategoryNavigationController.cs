﻿using Microsoft.AspNetCore.Mvc;
using WildBearAdventuresMVC.Models.WildBearCoffee;
using WildBearAdventuresMVC.WildBear.Interfaces;

namespace WildBearAdventuresMVC.Controllers
{
    public class CategoryNavigationController : Controller
    {
        private readonly IStoreApiClient _wildBearApiClient;

        public CategoryNavigationController(IStoreApiClient wildBearApiClient)
        {
            _wildBearApiClient = wildBearApiClient;
        }

        public IActionResult Index()
        {
            var categoryDtoCollection = _wildBearApiClient.GetAllCategoriesFromCatalog("MainProductCatalog", new CancellationToken());

            var categoryNames = new List<string>();

            foreach (var categoryDto in categoryDtoCollection)
            {
                categoryNames.Add(categoryDto.Name);                
            }

            var categoryNavigationViewModel = new CategoryNavigationViewModel
            {
                categoryNames = categoryNames
            };


            return View(categoryNavigationViewModel);
        }



    }
}
