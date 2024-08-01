using EcommerceWepApi.DTO;
using EcommerceWepApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EcommerceWepApi.Controllers
{

	[Route("api/cart")]
	[ApiController]
	public class CartController : ControllerBase
	{

		public CartController(EcommerWepApiContext myDB, UserManager<IdentityUser> userManager) 
		{
			_myDB = myDB;
			_userManager = userManager;
		}

		private readonly EcommerWepApiContext _myDB;
		private readonly UserManager<IdentityUser> _userManager;

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> getCart()
		{

			string id = User.Claims.FirstOrDefault(x => x.Type == "userId").Value;

			var userCart = _myDB.Carts.FirstOrDefault(x=>x.UserId.Contains(id));
			if (userCart == null)
			{
				return BadRequest(new {error=$"Invalid user id {id}"});
			}

			var userCartDetails = _myDB.cartDetailss.Include(y=>y.product).Include(m=>m.cart).Where(x=>x.cartId == userCart.Id).ToList();
			if (userCartDetails == null || userCartDetails.Count == 0)
			{
				return Ok(new {msg=$"Cart id {userCart.Id} is empty"});
			}

			List<productDTo> myProductsDto = new List<productDTo>();
			Category tempCategory;

			foreach (var item in userCartDetails)
			{
				tempCategory = _myDB.Categories.FirstOrDefault(x=>x.Id == item.product.CategoryId);

				myProductsDto.Add(new productDTo {Id=item.productId, Category= tempCategory .Name, Description=item.product.Description, Discount=item.product.Discount, Imgs=item.product.Imgs, Price=item.product.Price, Quantity=item.product.Quantity, Reviews=item.product.Reviews, Title=item.product.Title, CategoryId=item.product.CategoryId});
			}

			double totalProductPrica = 0;

			foreach(var prod in myProductsDto)
			{
				totalProductPrica += prod.Price;
			}

			CartDto myCartDto = new CartDto { Id=userCart.Id, Products= myProductsDto, TotalPrice= totalProductPrica, numberOfItems=myProductsDto.Count,  status="success"};


			return Ok(myCartDto);
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> addProductToCart(cartWithProductDto newProduct)
		{
			if (ModelState.IsValid)
			{
				string id = User.Claims.FirstOrDefault(x => x.Type == "userId").Value;

				var tempUser = await _userManager.FindByIdAsync(id);

				var tempUserProduct = _myDB.Products.FirstOrDefault(x => x.Id == newProduct.productId);

				if (tempUserProduct == null)
				{
					return BadRequest(new { error = $"Product Id {newProduct.productId} is invalid" });
				}

				if (tempUser == null)
				{
					return BadRequest(new { error = "Invalid user id" });

				}

				var userCart = _myDB.Carts.FirstOrDefault(x => x.UserId == tempUser.Id);

				if (tempUser == null)
				{
					return BadRequest(new { error = $"user id {id} doesn't have any cart!" });
				}

				cartDetails newCartDetails = new cartDetails { cartId = userCart.Id, productId = newProduct.productId };

				_myDB.cartDetailss.Add(newCartDetails);
				_myDB.SaveChanges();

				var userCartDetails = _myDB.cartDetailss.Include(y => y.product).Include(m => m.cart).Where(x => x.cartId == userCart.Id).ToList();
				if (userCartDetails == null || userCartDetails.Count == 0)
				{
					return Ok(new { msg = $"Cart id {userCart.Id} is empty" });
				}

				List<productDTo> myProductsDto = new List<productDTo>();
				Category tempCategory;

				foreach (var item in userCartDetails)
				{
					tempCategory = _myDB.Categories.FirstOrDefault(x => x.Id == item.product.CategoryId);

					myProductsDto.Add(new productDTo { Id = item.productId, Category = tempCategory.Name, Description = item.product.Description, Discount = item.product.Discount, Imgs = item.product.Imgs, Price = item.product.Price, Quantity = item.product.Quantity, Reviews = item.product.Reviews, Title = item.product.Title, CategoryId = item.product.CategoryId });
				}

				double totalProductPrica = 0;

				foreach (var prod in myProductsDto)
				{
					totalProductPrica += prod.Price;
				}

				CartDto myCartDto = new CartDto { Id = userCart.Id, Products = myProductsDto, TotalPrice= totalProductPrica, numberOfItems=myProductsDto.Count, status = "success" };

				return Ok(myCartDto);


			}

			return BadRequest(ModelState);
		}


		[HttpDelete("{id}")]
		[Authorize]
		public async Task<IActionResult> Update([FromRoute] int id)
		{

			string userId = User.Claims.FirstOrDefault(x => x.Type == "userId").Value;

			var tempUser = await _userManager.FindByIdAsync(userId);

			if (tempUser == null)
			{
				return BadRequest(new { error = "Invalid user id" });
			}

			var userCart = _myDB.Carts.FirstOrDefault(x => x.UserId == tempUser.Id);

			if (tempUser == null)
			{
				return BadRequest(new { error = $"user id {id} doesn't have any cart!" });
			}

			var tempUserProduct = _myDB.Products.FirstOrDefault(x => x.Id == int.Parse($"{id}"));

			if (tempUserProduct == null)
			{
				return BadRequest(new { error = $"Product Id {id} is invalid" });
			}

			var userCartDetails = _myDB.cartDetailss.Include(y => y.product).Include(m => m.cart).FirstOrDefault(x => x.cartId == userCart.Id && x.productId == tempUserProduct.Id);
			if (userCartDetails == null)
			{
				return Ok(new { msg = $"Cart id {userCart.Id} is empty" });
			}

			_myDB.cartDetailss.Remove(userCartDetails);
			_myDB.SaveChanges();

			return NoContent();

		}


		[HttpDelete("clear")]
		[Authorize]
		public async Task<IActionResult> Clear()
		{
			string id = User.Claims.FirstOrDefault(x => x.Type == "userId").Value;

			var userCart = _myDB.Carts.FirstOrDefault(x => x.UserId.Contains(id));

			if (userCart == null)
			{
				return BadRequest(new { error = $"Invalid cart id {id}" });
			}

			var UserCartDetails = _myDB.cartDetailss.Where(x => x.cartId == userCart.Id).ToList();

			foreach (var item in UserCartDetails)
			{
				_myDB.cartDetailss.Remove(item);
			}

			_myDB.SaveChanges();

			return NoContent();
		}
	}
}
