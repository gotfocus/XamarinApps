using System.Collections.Generic;
using Newtonsoft.Json;

namespace CaSI
{
	[JsonObject("result")]
	public class SpeechResult
	{
		/*
		public string Scenario { get; set; }
		public string Name { get; set; }
		public string Lexical { get; set; }
		public string Confidence { get; set; }
		*/
		public string RecognitionStatus { get; set; }
		public string DisplayText { get; set; }
		public int Offset { get; set; }
		public int Duration { get; set; }

	}

	public class SpeechResults
	{
		public List<SpeechResult> results { get; set; }
	}
}
