using System.Threading.Tasks;

namespace CaSI
{
	public interface IAuthenticationService
	{
		Task InitializeAsync();
		string GetAccessToken();
	}
}
