using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using CaSI.Models;

namespace CaSI.Views
{
	public partial class CaSIItemPage : ContentPage
	{
		IBingSpeechService bingSpeechService;
		IBingSpellCheckService bingSpellCheckService;
		ITextTranslationService textTranslationService;
		bool isRecording = false;

		public static readonly BindableProperty CaSIItemProperty =
			BindableProperty.Create("CaSIItem", typeof(CaSIItem), typeof(CaSIItemPage), null);

		public CaSIItem CaSIItem
		{
			get { return (CaSIItem)GetValue(CaSIItemProperty); }
			set { SetValue(CaSIItemProperty, value); }
		}

		public static readonly BindableProperty IsProcessingProperty =
			BindableProperty.Create("IsProcessing", typeof(bool), typeof(CaSIItemPage), false);

		public bool IsProcessing
		{
			get { return (bool)GetValue(IsProcessingProperty); }
			set { SetValue(IsProcessingProperty, value); }
		}

		public CaSIItemPage()
		{
			InitializeComponent();

			bingSpeechService = new BingSpeechService(new AuthenticationService(Constants.BingSpeechApiKey), Device.RuntimePlatform.ToString());
			bingSpellCheckService = new BingSpellCheckService();
			textTranslationService = new TextTranslationService(new AuthenticationService(Constants.TextTranslatorApiKey));
		}

		async void OnRecognizeSpeechButtonClicked(object sender, EventArgs e)
		{
			try
			{
				var audioRecordingService = DependencyService.Get<IAudioRecorderService>();
				if (!isRecording)
				{
					audioRecordingService.StartRecording();

					((Button)sender).Image = "recording.png";
					IsProcessing = true;
				}
				else
				{
					audioRecordingService.StopRecording();
				}

				isRecording = !isRecording;
				if (!isRecording)
				{
					var speechResult = await bingSpeechService.RecognizeSpeechAsync(Constants.AudioFilename);
					Debug.WriteLine("Name: " + speechResult.Name);
					Debug.WriteLine("Confidence: " + speechResult.Confidence);

					if (!string.IsNullOrWhiteSpace(speechResult.Name))
					{
						CaSIItem.Name = char.ToUpper(speechResult.Name[0]) + speechResult.Name.Substring(1);
						OnPropertyChanged("CaSIItem");
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			finally
			{
				if (!isRecording)
				{
					((Button)sender).Image = "record.png";
					IsProcessing = false;
				}
			}
		}

		async void OnSpellCheckButtonClicked(object sender, EventArgs e)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(CaSIItem.Name))
				{
					IsProcessing = true;

					var spellCheckResult = await bingSpellCheckService.SpellCheckTextAsync(CaSIItem.Name);
					foreach (var flaggedToken in spellCheckResult.FlaggedTokens)
					{
						CaSIItem.Name = CaSIItem.Name.Replace(flaggedToken.Token, flaggedToken.Suggestions.FirstOrDefault().Suggestion);
					}
					OnPropertyChanged("CaSIItem");

					IsProcessing = false;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		async void OnTranslateButtonClicked(object sender, EventArgs e)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(CaSIItem.Name))
				{
					IsProcessing = true;

					CaSIItem.Name = await textTranslationService.TranslateTextAsync(CaSIItem.Name);
					OnPropertyChanged("CaSIItem");

					IsProcessing = false;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		async void OnSaveClicked(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(CaSIItem.Name))
			{
				await App.CaSIManager.SaveItemAsync(CaSIItem);
			}
			await Navigation.PopAsync();
		}

		async void OnDeleteClicked(object sender, EventArgs e)
		{
			await App.CaSIManager.DeleteItemAsync(CaSIItem);
			await Navigation.PopAsync();
		}

		async void OnCancelClicked(object sender, EventArgs e)
		{
			await Navigation.PopAsync();
		}
	}
}
