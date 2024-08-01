using EcommerceWepApi.DTO;
using EcommerceWepApi.Model;
using EcommerceWepApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWepApi.Controllers
{
	[Route("api/products")]
	[ApiController]
	public class ProductsController : ControllerBase
	{

		public ProductsController(IProductService prodService) 
		{
			_productService = prodService;
		}

		private readonly IProductService _productService;

		[HttpGet]
		public async Task<IActionResult> getAll()
		{
			var products =  await _productService.getAll();

			List<productDTo> tempProducts = new List<productDTo>();

			foreach (var product in products)
			{
				tempProducts.Add(new productDTo {Id=product.Id, Category=product.Category.Name, CategoryId=product.CategoryId, Description=product.Description, Discount=product.Discount, Imgs=product.Imgs, Price=product.Price, Quantity=product.Quantity, Reviews=product.Reviews, Title=product.Title});
			}

			return Ok(tempProducts);
		}

		[HttpPost]
		[Authorize(Roles ="Admin")]
		public async Task<IActionResult> Create([FromBody] productDTo newProduct) 
		{ 
			if (ModelState.IsValid)
			{
				Product tempProduct = new Product { CategoryId=newProduct.CategoryId, Description=newProduct.Description, Discount=newProduct.Discount, Imgs=newProduct.Imgs, Price=newProduct.Price, Quantity=newProduct.Quantity, Reviews=newProduct.Reviews, Title=newProduct.Title };

				bool state = await _productService.Create(tempProduct);
				if (state)
				{

					var prodWithId = await _productService.getById(tempProduct.Id);

					newProduct.Id = prodWithId.Id;
					newProduct.Category = prodWithId.Category.Name;
					
					string newProductUrl = Url.Link("getProductById", new { id = tempProduct.Id });

					return Created(newProductUrl, newProduct);
				}
				else
				{
					return BadRequest(new { error = "Faild to create new product" });
				}
			}
			return BadRequest(ModelState);
		}



		[HttpGet]
		[Route("{id}", Name ="getProductById")]
		public async Task<IActionResult> getById([FromRoute] int id)
		{
			var newProduct = await _productService.getById(id);
            if (newProduct != null)
            {
				productDTo tempProduct = new productDTo { Id = newProduct.Id, CategoryId = newProduct.CategoryId, Description = newProduct.Description, Discount = newProduct.Discount, Imgs = newProduct.Imgs, Price = newProduct.Price, Quantity = newProduct.Quantity, Reviews = newProduct.Reviews, Title = newProduct.Title, Category=newProduct.Category.Name };
				
				return Ok(tempProduct);
			}
			return BadRequest(new {msg=$"Id {id} is invalid"});
		}


		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Update([FromRoute] int id, [FromBody] productDTo newProduct)
		{
			if (ModelState.IsValid)
			{
				Product tempProduct = new Product { CategoryId = newProduct.CategoryId, Description = newProduct.Description, Discount = newProduct.Discount, Imgs = newProduct.Imgs, Price = newProduct.Price, Quantity = newProduct.Quantity, Reviews = newProduct.Reviews, Title = newProduct.Title };

				bool state = await _productService.Update(id, tempProduct);
				if (state)
				{
					return Ok(newProduct);
				}
				return BadRequest(new {error=$"Invalid product id {id}"});
			}

			return BadRequest(ModelState);
		}

		[HttpDelete("{id}")]
		[Authorize(Roles ="Admin")]
		public async Task<IActionResult> Delete([FromRoute] int id)
		{
			var deleteResponse = await _productService.Delete(id);
			if (deleteResponse)
			{
				return NoContent();
			}
			return BadRequest(new {msg=$"Id {id} is invalid"});
		}

	}	
}
