namespace CaSI
{
	public static class Constants
	{
		public static readonly string AuthenticationTokenEndpoint = "https://api.cognitive.microsoft.com/sts/v1.0";
		
		public static readonly string BingSpeechApiKey = "YOUR API KEY HERE"; 
		public static readonly string BingSpeechHost = "speech.platform.bing.com";
		public static readonly string SpeechRecognitionEndpoint = "https://speech.platform.bing.com/speech/recognition/interactive/cognitiveservices/v1?language=en-US";  // https://speech.platform.bing.com/speech/recognition/interactive/cognitiveservices/v1
		public static readonly string ConversationSpeechRecognitionEndpoint = "https://speech.platform.bing.com/speech/recognition/conversation/cognitiveservices/v1?language=en-US"; 
		public static readonly string AudioContentType = @"audio/wav; codec=""audio/pcm""; samplerate=88000";

		public static readonly string AudioFilename = "CaSI.wav";
	}
}
