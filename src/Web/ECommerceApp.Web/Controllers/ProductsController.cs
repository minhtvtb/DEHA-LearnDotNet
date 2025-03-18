using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ECommerceApp.Core.DTOs;
using ECommerceApp.Core.Interfaces;
using ECommerceApp.Infrastructure.Services;

namespace ECommerceApp.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IFileService _fileService;

        public ProductsController(
            IProductService productService,
            ICategoryService categoryService,
            IFileService fileService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _fileService = fileService;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProductDto productDto, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    try
                    {
                        string imageUrl = await _fileService.UploadFileAsync(imageFile);
                        productDto.ImageUrl = imageUrl;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, $"Error uploading image: {ex.Message}");
                        ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                        return View(productDto);
                    }
                }

                await _productService.CreateProductAsync(productDto);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            return View(productDto);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, ProductDto productDto, IFormFile imageFile)
        {
            if (id != productDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _productService.GetProductByIdAsync(id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Delete old image if it exists
                        if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                        {
                            _fileService.DeleteFile(existingProduct.ImageUrl);
                        }

                        // Upload new image
                        string imageUrl = await _fileService.UploadFileAsync(imageFile);
                        productDto.ImageUrl = imageUrl;
                    }
                    else
                    {
                        // Keep existing image if no new image is uploaded
                        productDto.ImageUrl = existingProduct.ImageUrl;
                    }

                    await _productService.UpdateProductAsync(productDto);
                }
                catch (Exception)
                {
                    if (!await ProductExists(productDto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            return View(productDto);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Delete product image if it exists
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                _fileService.DeleteFile(product.ImageUrl);
            }

            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProductExists(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return product != null;
        }
    }
} 