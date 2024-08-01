using EcommerceWepApi.DTO;
using EcommerceWepApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceWepApi.Controllers
{
	[Route("api/order")]
	[ApiController]
	public class OrdersController : ControllerBase
	{
		public OrdersController(EcommerWepApiContext myDb, UserManager<IdentityUser> userManager)
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

			string id = User.Claims.FirstOrDefault(x => x.Type == "userId").Value;

			var userOrders = _myDb.orders.Where(x=>x.UserId == id).ToList();

			if (userOrders == null || userOrders.Count == 0) 
			{
				return Ok(new {msg="customer doesn't have any order"});
			}

			List< OrderDto> myWishlistDtos = new List<OrderDto>();

			foreach (var userOrder in userOrders)
			{
				var orderDetais = _myDb.ordersDetails.Include(x => x.Product).Where(x => x.OrderId == userOrder.Id).ToList();

				List<productDTo> myProducts = new List<productDTo>();
				Category tempCategory;

				foreach (var wish in orderDetais)
				{
					tempCategory = await _myDb.Categories.FirstOrDefaultAsync(x => x.Id == wish.Product.CategoryId);
					myProducts.Add(new productDTo { Id = wish.Product.Id, Category = tempCategory.Name, CategoryId = wish.Product.CategoryId, Description = wish.Product.Description, Discount = wish.Product.Discount, Imgs = wish.Product.Imgs, Price = wish.Product.Price, Quantity = wish.Product.Quantity, Reviews = wish.Product.Reviews, Title = wish.Product.Title });
				}

				double totalPrice = 0;

				foreach (var prod in myProducts)
				{
					totalPrice += prod.Price;
				}

				myWishlistDtos.Add(new OrderDto { Id = userOrder.Id, status = userOrder.status, Products = myProducts, numberOfItems = myProducts.Count, TotalPrice = totalPrice });

			}

			return Ok(myWishlistDtos);

		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> create()
		{
			string id = User.Claims.FirstOrDefault(x => x.Type == "userId").Value;

			var userCart = _myDb.Carts.FirstOrDefault(x => x.UserId == id);

			if (userCart == null) 
			{
				return BadRequest(new { error = "User dosen't have any cart" });
			}

			var userCartDetails = _myDb.cartDetailss.Where(x => x.cartId == userCart.Id).ToList();

			if (userCartDetails == null || userCartDetails.Count == 0)
			{
				return BadRequest(new { error = "User cart is empty!" });
			}

			var userOrder = new Order { UserId = id };

			_myDb.orders.Add(userOrder);
			_myDb.SaveChanges();

			foreach(var cartDetails in userCartDetails)
			{
				_myDb.ordersDetails.Add(new OrderDetails { OrderId=userOrder.Id, ProductId=cartDetails.productId});
			}

			_myDb.SaveChanges();

			foreach (var cartItem in userCartDetails)
			{
				_myDb.cartDetailss.Remove(cartItem);
			}

			_myDb.SaveChanges();


			return Ok();
		}
	}
}
