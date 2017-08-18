using System.Collections.Generic;
using System.Threading.Tasks;
using CaSI.Models;
using SQLite;

namespace CaSI.Repository
{
	public class CaSIItemRepository : ICaSIItemRepository
	{
		readonly SQLiteAsyncConnection connection;

		public CaSIItemRepository(string dbPath)
		{
			connection = new SQLiteAsyncConnection(dbPath);
			connection.CreateTableAsync<CaSIItem>().Wait();
		}

		public Task<List<CaSIItem>> GetAllItemsAsync()
		{
			return connection.Table<CaSIItem>().ToListAsync();
		}

		public Task<CaSIItem> GetItemAsync(int id)
		{
			return connection.Table<CaSIItem>().Where(i => i.ID == id).FirstOrDefaultAsync();
		}

		public Task<int> SaveItemAsync(CaSIItem item)
		{
			if (item.ID != 0)
			{
				return connection.UpdateAsync(item);
			}
			else
			{
				return connection.InsertAsync(item);
			}
		}

		public Task<int> DeleteItemAsync(CaSIItem item)
		{
			return connection.DeleteAsync(item);
		}
	}
}

