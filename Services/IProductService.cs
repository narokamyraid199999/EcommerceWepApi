using EcommerceWepApi.Model;

namespace EcommerceWepApi.Services
{
	public interface IProductService
	{

		public Task<List<Product>> getAll();

		public Task<bool> Create(Product newProduct);

		public Task<Product?> getById(int id);

		public Task<bool> Update(int id, Product newProduct);

		public Task<bool> Delete(int id);
	}
}
