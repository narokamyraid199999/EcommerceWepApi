using EcommerceWepApi.Model;

namespace EcommerceWepApi.Services
{
	public interface ICategory
	{
		public Task<List<Category>> getAll();

		public Task<Category> getById(int id);

		public Task<bool> Delete(int id);

		public Task<Category?> Create(categoryDTo newCategory);

		public Task<Category?> Update(int id , categoryDTo newCategory);


	}
}
