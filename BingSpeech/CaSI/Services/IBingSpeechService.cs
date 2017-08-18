using System.Threading.Tasks;

namespace CaSI
{
	public interface IBingSpeechService
	{
		Task<SpeechResult> RecognizeSpeechAsync(string filename);
        Task InitAuth();
    }
}
