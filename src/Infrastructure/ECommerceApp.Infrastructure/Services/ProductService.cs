using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ECommerceApp.Core.DTOs;
using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public ProductService(
            IProductRepository productRepository,
            IRepository<Category> categoryRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductWithCategoryAsync(id);
            if (product == null) return null;
            
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            var result = await _productRepository.AddAsync(product);
            return _mapper.Map<ProductDto>(result);
        }

        public async Task UpdateProductAsync(ProductDto productDto)
        {
            var product = await _productRepository.GetByIdAsync(productDto.Id);
            if (product != null)
            {
                _mapper.Map(productDto, product);
                await _productRepository.UpdateAsync(product);
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product != null)
            {
                await _productRepository.DeleteAsync(product);
            }
        }

        public async Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int count)
        {
            var products = await _productRepository.GetAllAsync();
            // For now, just return the most recent products
            // In a real app, you might have a "Featured" flag or another way to determine featured products
            var featuredProducts = products.OrderByDescending(p => p.Id).Take(count);
            return _mapper.Map<IEnumerable<ProductDto>>(featuredProducts);
        }
    }
} 