﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WildBearAdventuresMVC.Models;
using WildBearAdventuresMVC.Models.WildBearCoffee;
using WildBearAdventuresMVC.WildBear.Interfaces;

namespace WildBearAdventuresMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWildBearApiClient _wildBearApiClient;
        private readonly IConfiguration _configuration;


        public HomeController(ILogger<HomeController> logger, IWildBearApiClient wildBearApiClient, IConfiguration configuration)
        {
            _logger = logger;
            _wildBearApiClient = wildBearApiClient;
            _configuration = configuration;
        }
        public IActionResult Index()
        {


            var productDto = _wildBearApiClient.GetRandomProductFromCategory(new Guid("7040940e-eab1-4a72-85b5-867905b7d94a"), new CancellationToken());

            var coffeeViewModel = new CoffeeViewModel()
            {
                Name = productDto.Name,
                Price = productDto.PricesInclTax.FirstOrDefault().Value,
                Description = productDto.ShortDescription

            };



            return View(coffeeViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
