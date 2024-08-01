using EcommerceWepApi.Model;
using System.Reflection.Metadata.Ecma335;

namespace EcommerceWepApi.Services
{
	public class ContactService : IContact
	{

		public ContactService(EcommerWepApiContext myDB) 
		{
			_myDB = myDB;
		}

		private readonly EcommerWepApiContext _myDB;

		public async Task<List<Contact>> getAll()
		{
			return _myDB.Contacts.ToList();
		}

		public async Task<Contact?> Create(Contact newContact)
		{
			try
			{
				_myDB.Contacts.Add(newContact);
				_myDB.SaveChanges();
				return newContact;
			}catch (Exception ex)
			{
				return null;
			}

		}

		public async Task<bool> Delete(int id)
		{
			bool state = false;
			var tempContact = _myDB.Contacts.FirstOrDefault(x => x.Id == id);
			if (tempContact != null)
			{
				_myDB.Contacts.Remove(tempContact);
				_myDB.SaveChanges();
				state = true;
			}
			return state;
		}

		public async Task<Contact> getById(int id)
		{
			throw new NotImplementedException();
		}

		public async Task<Contact?> Update(int id, Contact newContact)
		{
			throw new NotImplementedException();
		}
	}
}
