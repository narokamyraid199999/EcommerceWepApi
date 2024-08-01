using EcommerceWepApi.Model;
using EcommerceWepApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EcommerceWepApi.DTO;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Authorization;

namespace EcommerceWepApi.Controllers
{
	[Route("api/category")]
	[ApiController]
	public class CategoryController : ControllerBase
	{

		public CategoryController(ICategory myCat) 
		{
			_categoryService = myCat;
		}

		public readonly ICategory _categoryService;

		[HttpGet]
		public async Task<IActionResult> getAll()
		{
			var categories = await _categoryService.getAll();

			List<categoryDTo> tempCategory = new List<categoryDTo>();

			categoryDTo tempDto;

			foreach(var category in categories)
			{
				tempDto = new categoryDTo() {Id=category.Id, Name=category.Name, Image= category .Image};
				
				foreach(var product in category.products)
				{
					tempDto.Products.Add(new productDTo { Id=product.Id, Category=product.Category.Name, CategoryId=product.Category.Id, Description=product.Description, Discount=product.Discount, Imgs=product.Imgs, Price=product.Price, Quantity=product.Quantity, Reviews=product.Reviews, Title=product.Title});
				}

				tempCategory.Add(tempDto);
			}

			return Ok(tempCategory);
		}


		[HttpGet("{id}", Name ="getCategoryById")]
		public async Task<IActionResult> getBy([FromRoute] int id)
		{
			var tempCategory = await _categoryService.getById(id);
			if (tempCategory != null)
			{

				List<productDTo> tempProductDto = new List<productDTo>();

				foreach(var product in tempCategory.products)
				{
					tempProductDto.Add(new productDTo { Id=product.Id, Category=product.Category.Name, CategoryId=product.CategoryId, Description=product.Description, Discount=product.Discount, Imgs=product.Imgs, Price=product.Price, Quantity=product.Quantity, Reviews=product.Reviews, Title=product.Title});
				}

				categoryDTo categoryDTo = new categoryDTo() { Id=tempCategory.Id, Name=tempCategory.Name, Image=tempCategory.Image, Products=tempProductDto};

				return Ok(categoryDTo);

			}
			return BadRequest(new {error="Invalid category id"});
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete([FromRoute] int id)
		{
			bool delRes = await _categoryService.Delete(id);
			if (delRes)
			{
				return NoContent();
			}
			return BadRequest(new { error = "Invalid category id" });
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Create(categoryDTo newCategory)
		{
			if (ModelState.IsValid)
			{
				var tempCategory = await _categoryService.Create(newCategory);
				if (tempCategory != null)
				{
					string categuryUrl = Url.Link("getCategoryById", new {id= tempCategory.Id});

					categoryDTo categoryDTo = new categoryDTo() { Id = tempCategory.Id, Name = tempCategory.Name, Image=newCategory.Image };

					return Created(categuryUrl, categoryDTo);
				}
			}
			return BadRequest(ModelState);

		}

		[HttpPut("{id}")]
		[Authorize(Roles ="Admin")]
		public async Task<IActionResult> Update([FromRoute] int id , [FromBody] categoryDTo newCategory)
		{
			if (ModelState.IsValid)
			{
				var tempCategory = await _categoryService.Update(id, newCategory);
				if (tempCategory != null)
				{
					List<productDTo> productsDto = new List<productDTo>();

					foreach(var product in tempCategory.products)
					{
						productsDto.Add(new productDTo { Id = product.Id, Category = product.Category.Name, CategoryId = product.CategoryId, Description = product.Description, Discount = product.Discount, Imgs = product.Imgs, Price = product.Price, Quantity = product.Quantity, Reviews = product.Reviews, Title = product.Title });
					}

					categoryDTo newCategoryDto = new categoryDTo() { Id = tempCategory.Id, Name=tempCategory.Name,  Image = tempCategory.Image, Products= productsDto };

					return Ok(newCategoryDto);
				}

				return BadRequest(new {error="Invalid category id"});

			}

			return BadRequest(ModelState);

		}


	}
}
