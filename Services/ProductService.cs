using EcommerceWepApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceWepApi.Services
{
	public class ProductService : IProductService
	{

		public ProductService(EcommerWepApiContext myDb) 
		{
			_myDb = myDb;
		}

		private readonly EcommerWepApiContext _myDb;

		public async Task<List<Product>> getAll()
		{
			return _myDb.Products.Include(x=>x.Category).ToList();
		}

		public async Task<bool> Create(Product newProduct)
		{
			bool state = false;

			try
			{
				_myDb.Products.Add(newProduct);
				_myDb.SaveChanges();
				state = true;
			}catch(Exception ex)
			{
				state= false;
			}

			return state;
		}

		public async Task<Product?> getById(int id)
		{
			var myProduct = _myDb.Products.Include(x=>x.Category).FirstOrDefault(x=>x.Id == id);
			if (myProduct != null)
			{
				return myProduct;
			}
			return null;
		}

		public async Task<bool> Delete(int id)
		{
			bool state = false;
			try
			{
				var myProduct = await getById(id);
				if (myProduct != null)
				{
					_myDb.Products.Remove(myProduct);
					_myDb.SaveChanges();
					state = true;
				}
			}catch(Exception ex)
			{
				state= false;
			}
			return state;
		}

		public async Task<bool> Update(int id, Product newProduct)
		{
			bool state = false;
			var myProduct = await getById(id);
			if (myProduct != null)
			{
				myProduct.Price = newProduct.Price;
				myProduct.Quantity = newProduct.Quantity;
				myProduct.Reviews = newProduct.Reviews;
				myProduct.Title = newProduct.Title;
				myProduct.Imgs = newProduct.Imgs;
				myProduct.Discount = newProduct.Discount;
				_myDb.SaveChanges();
				state=true;
			}
			return state;
		}
	}
}
