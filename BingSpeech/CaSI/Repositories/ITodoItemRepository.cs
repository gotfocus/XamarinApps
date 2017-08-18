using System.Collections.Generic;
using System.Threading.Tasks;

namespace CaSI
{
	public interface ICaSIItemRepository
	{
		Task<List<CaSIItem>> GetAllItemsAsync();
		Task<CaSIItem> GetItemAsync(int id);
		Task<int> SaveItemAsync(CaSIItem item);
		Task<int> DeleteItemAsync(CaSIItem item);
	}
}
