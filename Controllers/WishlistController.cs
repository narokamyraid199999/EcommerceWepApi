using EcommerceWepApi.DTO;
using EcommerceWepApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceWepApi.Controllers
{
	[Route("api/wishlist")]
	[ApiController]
	public class WishlistController : ControllerBase
	{

		public WishlistController(EcommerWepApiContext myDb, UserManager<IdentityUser> userManager) 
		{
			_myDb = myDb;
			_userManager = userManager;
		}	

		private readonly EcommerWepApiContext _myDb;
		private readonly UserManager<IdentityUser> _userManager;

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> getAll()
		{

			string userId = User.Claims.FirstOrDefault(x => x.Type.Contains("userId")).Value;

			var userWishlist = _myDb.Wishlists.FirstOrDefault(x => x.UserId == userId);

			var wishlists = _myDb.WishDetails.Include(x=>x.product).Where(x=>x.wishlistId == userWishlist.Id).ToList();

			List<productDTo> wishlistProducts = new List<productDTo>();
			Category tempCategory;

			foreach(var wish in wishlists)
			{
				tempCategory = await _myDb.Categories.FirstOrDefaultAsync(x => x.Id == wish.product.CategoryId);
				wishlistProducts.Add(new productDTo { Id = wish.product.Id, Category = tempCategory.Name, CategoryId=wish.product.CategoryId, Description=wish.product.Description, Discount=wish.product.Discount, Imgs=wish.product.Imgs, Price=wish.product.Price, Quantity=wish.product.Quantity, Reviews=wish.product.Reviews, Title=wish.product.Title });
			}

			double totalPrice = 0;

			foreach(var prod in wishlistProducts)
			{
				totalPrice+= prod.Price;
			}

			WishlistDto myWishlistDto = new WishlistDto { Id=userWishlist.Id, Products = wishlistProducts, numberOfItems= wishlistProducts.Count, TotalPrice=totalPrice};

			return Ok(myWishlistDto);
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> addNewProduct(cartWithProductDto newProduct)
		{
			if (ModelState.IsValid)
			{
				string id = User.Claims.FirstOrDefault(x => x.Type == "userId").Value;

				var tempUser = await _userManager.FindByIdAsync(id);

				var tempUserProduct = _myDb.Products.FirstOrDefault(x => x.Id == newProduct.productId);

				if (tempUserProduct == null)
				{
					return BadRequest(new { error = $"Product Id {newProduct.productId} is invalid" });
				}

				if (tempUser == null)
				{
					return BadRequest(new { error = "Invalid user id" });

				}

				var userWishlist = _myDb.Wishlists.FirstOrDefault(x => x.UserId == tempUser.Id);

				if (userWishlist == null)
				{
					return BadRequest(new { error = $"User id {tempUser.Id} doesn't have any wishlist " });
				}

				_myDb.WishDetails.Add(new wishDetails {wishlistId=userWishlist.Id, productId=tempUserProduct.Id });
				_myDb.SaveChanges();

				var wishlists = _myDb.WishDetails.Include(x => x.product).Where(x => x.wishlistId == userWishlist.Id).ToList();

				List<productDTo> wishlistProducts = new List<productDTo>();
				Category tempCategory;

				foreach (var wish in wishlists)
				{
					tempCategory = await _myDb.Categories.FirstOrDefaultAsync(x => x.Id == wish.product.CategoryId);
					wishlistProducts.Add(new productDTo { Id = wish.product.Id, Category = tempCategory.Name, CategoryId = wish.product.CategoryId, Description = wish.product.Description, Discount = wish.product.Discount, Imgs = wish.product.Imgs, Price = wish.product.Price, Quantity = wish.product.Quantity, Reviews = wish.product.Reviews, Title = wish.product.Title });
				}

				double totalPrice = 0;

				foreach (var prod in wishlistProducts)
				{
					totalPrice += prod.Price;
				}

				WishlistDto myWishlistDto = new WishlistDto { Id = userWishlist.Id, Products = wishlistProducts, numberOfItems = wishlistProducts.Count, TotalPrice = totalPrice };

				return Ok(myWishlistDto);

			}

			return BadRequest(ModelState);
		}

		[HttpDelete]
		[Authorize]
		public async Task<IActionResult> Delete(cartWithProductDto newProduct)
		{
			if (ModelState.IsValid)
			{
				string id = User.Claims.FirstOrDefault(x => x.Type == "userId").Value;

				var tempUser = await _userManager.FindByIdAsync(id);

				var tempUserProduct = _myDb.Products.FirstOrDefault(x => x.Id == newProduct.productId);

				if (tempUserProduct == null)
				{
					return BadRequest(new { error = $"Product Id {newProduct.productId} is invalid" });
				}

				if (tempUser == null)
				{
					return BadRequest(new { error = "Invalid user id" });

				}

				var userWishlist = _myDb.Wishlists.FirstOrDefault(x => x.UserId == tempUser.Id);

				if (userWishlist == null)
				{
					return BadRequest(new { error = $"User id {tempUser.Id} doesn't have any wishlist " });
				}

				var myWishlistDetails = _myDb.WishDetails.FirstOrDefault(x => x.productId == tempUserProduct.Id);
				if (myWishlistDetails == null)
				{
					return BadRequest(new {error="Invalid product id"});
				}
				
				_myDb.WishDetails.Remove(myWishlistDetails);
				_myDb.SaveChanges();

				return NoContent();
			}
			return BadRequest(ModelState);
		}

		[HttpDelete("clear")]
		[Authorize]
		public async Task<IActionResult> clear()
		{
			string id = User.Claims.FirstOrDefault(x => x.Type == "userId").Value;

			var tempUser = await _userManager.FindByIdAsync(id);

			if (tempUser == null)
			{
				return BadRequest(new { error = "Invalid user id" });
			}

			var userWishlist = _myDb.Wishlists.FirstOrDefault(x => x.UserId == tempUser.Id);

			if (userWishlist == null)
			{
				return BadRequest(new { error = $"User id {tempUser.Id} doesn't have any wishlist " });
			}

			var userWishlistDetails = _myDb.WishDetails.Where(x=>x.wishlistId == userWishlist.Id).ToList();

			foreach(var item in userWishlistDetails)
			{
				_myDb.WishDetails.Remove(item);
			}

			_myDb.SaveChanges();

			return NoContent();
		}
	}
}
