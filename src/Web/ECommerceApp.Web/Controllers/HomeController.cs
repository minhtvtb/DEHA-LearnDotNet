using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ECommerceApp.Core.Interfaces;
using ECommerceApp.Web.Models;

namespace ECommerceApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;

    public HomeController(
        ILogger<HomeController> logger,
        IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var featuredProducts = await _productService.GetFeaturedProductsAsync(6);
        return View(featuredProducts);
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
