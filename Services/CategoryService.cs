using EcommerceWepApi.Model;
using Microsoft.EntityFrameworkCore;

namespace EcommerceWepApi.Services
{
	public class CategoryService:ICategory
	{

		public CategoryService(EcommerWepApiContext myDB) {
			_myDB = myDB;
		}

		public readonly EcommerWepApiContext _myDB;
		
		public async Task<List<Category>> getAll()
		{
			return _myDB.Categories.Include(x=>x.products).ToList();
		}

		public async Task<Category?> getById(int id)
		{
			var tempCategory = _myDB.Categories.Include(x=>x.products).FirstOrDefault(x=>x.Id == id);
			if (tempCategory != null)
			{
				return tempCategory;
			}
			return null;
		}

		public async Task<bool> Delete(int id)
		{
			bool state = false;
			var tempCategory = await getById(id);
			
			if (tempCategory != null)
			{
				_myDB.Categories.Remove(tempCategory);
				_myDB.SaveChanges();
				state = true;
			}
			return state;
		}

		public async Task<Category?> Create(categoryDTo newCategory)
		{
			try
			{
				Category tempCategory = new Category { Name = newCategory.Name, Image=newCategory.Image };
				_myDB.Categories.Add(tempCategory);
				_myDB.SaveChanges();
				return tempCategory;
			}
			catch(Exception ex)
			{
				return null;
			}
		}

		public async Task<Category?> Update(int id, categoryDTo newCategory)
		{

			var tempCategory = await getById(id);
			if (tempCategory != null) 
			{
				tempCategory.Name = newCategory.Name;
				tempCategory.Image= newCategory.Image;
				_myDB.SaveChanges();
				return tempCategory;
			}
			return null;
		}
	}
}
