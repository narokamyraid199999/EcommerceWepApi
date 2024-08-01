using EcommerceWepApi.Model;

namespace EcommerceWepApi.Services
{
	public interface IContact
	{

		public Task<List<Contact>> getAll();

		public Task<Contact> getById(int id);

		public Task<bool> Delete(int id);

		public Task<Contact?> Create(Contact newContact);

		public Task<Contact?> Update(int id, Contact newContact);

	}
}
