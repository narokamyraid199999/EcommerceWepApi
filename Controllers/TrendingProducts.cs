using EcommerceWepApi.DTO;
using EcommerceWepApi.Model;
using EcommerceWepApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.Controllers
{

	public class template
	{
		public int Id { get; set; }
		public bool New { get; set; } = false;

		public bool Featured { get; set; } = false;

		public bool TopSellers { get; set; } = false;

		public productDTo product { get; set; }
	}

	public class CreateProductDto
	{
		public bool New { get; set; } = false;

		public bool Featured { get; set; } = false;

		public bool TopSellers { get; set; } = false;

		[Required]
        public int productId { get; set; }
    }

	[Route("api/trendProducts")]
	[ApiController]
	public class TrendingProducts : ControllerBase
	{

		public TrendingProducts(EcommerWepApiContext myDB, ICategory catService) 
		{
			_myDB = myDB;
			_categoryService = catService;
		}

		private EcommerWepApiContext _myDB;
		private ICategory _categoryService;

		[HttpGet]
		public async Task<IActionResult> getAll()
		{
			List<TrendingDetails> trendDetails = _myDB.TrendingDetails.Include(x=>x.Product).Include(y=>y.TrendingProduct).ToList();

			List<template> myTemplate = new List<template>();

            foreach (var trend in trendDetails)
            {
				Category myCategory = await _categoryService.getById(int.Parse($"{trend.Product.CategoryId}"));

				myTemplate.Add(new template { Id=trend.Id, New=trend.TrendingProduct.isNew, Featured=trend.TrendingProduct.isFeatured, TopSellers=trend.TrendingProduct.isTopSellers, product=new productDTo { Id=trend.Product.Id, Category= myCategory.Name, CategoryId =trend.Product.CategoryId, Description=trend.Product.Description, Discount=trend.Product.Discount, Imgs=trend.Product.Imgs, Price=trend.Product.Price, Quantity=trend.Product.Quantity, Reviews=trend.Product.Reviews, Title=trend.Product.Title} });

			}

            return Ok(myTemplate);
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete([FromRoute] int id)
		{
			var tempTrendDetails = _myDB.TrendingDetails.FirstOrDefault(t => t.Id == id);
			if (tempTrendDetails != null)
			{
				try
				{
					_myDB.TrendingDetails.Remove(tempTrendDetails);
					_myDB.SaveChanges();
					return NoContent();
				}
				catch (Exception ex)
				{
					return BadRequest(new { error = "Faild to delete product!" });
				}
			}

			return BadRequest(new {error="Invalid product id"});
		}


		[HttpPost]
		[Authorize(Roles ="Admin")]
		public async Task<IActionResult> Create([FromBody]CreateProductDto newProduct)
		{
			if (ModelState.IsValid)
			{
				TrendingProduct tempTrendProduct = new TrendingProduct { isFeatured=newProduct.Featured, isNew=newProduct.New, isTopSellers=newProduct.TopSellers};
				_myDB.TrendingProducts.Add(tempTrendProduct);
				_myDB.SaveChanges();

				var tempProduct = _myDB.Products.FirstOrDefault(x=>x.Id==newProduct.productId);
				
				if ( tempProduct == null )
				{
					return BadRequest(new {error="Invalid product id"});
				}

				TrendingDetails tempTrendDetails = new TrendingDetails {productId=tempProduct.Id, TrendingProductId=tempTrendProduct.Id };
				_myDB.TrendingDetails.Add(tempTrendDetails);
				_myDB.SaveChanges();

				return Ok(tempTrendDetails);
			}

			return BadRequest(ModelState);

		}


	}
}
