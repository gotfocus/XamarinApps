using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.TextToSpeech;
using Plugin.TextToSpeech.Abstractions;
using CaSI.Models;

namespace CaSI.Views
{
    public partial class HomePage : ContentPage
    {

        static CrossLocale? locale = null;
        public bool conversationMode = true;
		public Slider sliderPitch;
        public Slider sliderRate;
        public Slider sliderVolume;
        public ListView _listview;
        public List<SpeechItem> _speechlist;
		public int hospital_id;
        IBingSpeechService bingSpeechService;
        public SpeechResult speechResult;

        bool isRecording = false;

		public HomePage()
        {
            InitializeComponent();

            bingSpeechService = new BingSpeechService(new AuthenticationService(Constants.BingSpeechApiKey), Device.RuntimePlatform);

            _speechlist = new List<SpeechItem>
            {
                new SpeechItem
                {
                    SpeechText = "How can I help you?",
                    Speaker = "CaSI"
                }
            };

            SpeechList.SeparatorColor = Color.Transparent;
            SpeechList.ItemsSource = _speechlist;
            SpeechList.ItemSelected += SpeechList_ItemSelected;
            SpeechList.HasUnevenRows = true;

            var speakButton = new Button
            {
                Text = "Speak"
            };

            var languageButton = new Button
            {
                Text = "Default Language"
            };

            languageButton.Clicked += async (sender, args) =>
            {
                var locales = CrossTextToSpeech.Current.GetInstalledLanguages();
                var items = locales.Select(a => a.ToString()).ToArray();


                var selected = await this.DisplayActionSheet("Language", "OK", null, items);
                if (string.IsNullOrWhiteSpace(selected) || selected == "OK")
                    return;
                languageButton.Text = selected;

                if (Device.RuntimePlatform == Device.iOS)
                    locale = locales.FirstOrDefault(l => l.ToString() == selected);
                else
                    locale = new CrossLocale { Language = selected };//fine for iOS/WP

            };

            var volume = 1.0;

            if (Device.RuntimePlatform == Device.iOS)
            {
                volume = 20.0;
            }


            sliderPitch = new Slider(0, 2.0, 1.5);
            sliderRate = new Slider(0, 1.0, .35);
            sliderVolume = new Slider(0, 2.0, volume);

            var useDefaults = new Switch
            {
                IsToggled = false
            };

            speakButton.Clicked += (sender, args) =>
            {
                if (useDefaults.IsToggled)
                {
                    TextSpeaker(_speechlist[0].SpeechText);
                    return;
                }
            };

            ListView _listview = new ListView
            {
                SeparatorColor = Color.Transparent,
                // Source of data items.
                ItemsSource = _speechlist,

                // Define template for displaying each item.
                // (Argument of DataTemplate constructor is called for 
                //      each item; it must return a Cell derivative.)
                ItemTemplate = new DataTemplate(() =>
                {
                    // Create views with bindings for displaying each property.
                    Label speechLabel = new Label();
                    Label speakerLabel = new Label();
                    speechLabel.SetBinding(Label.TextProperty, "SpeechText");
                    speakerLabel.SetBinding(Label.TextProperty, "Speaker");

                    return new ViewCell
                    {
                        View = new StackLayout
                        {
                            Padding = new Thickness(0, 5),
                            Orientation = StackOrientation.Horizontal,
                            Children =
                            {
                                speechLabel,
                                speakerLabel
                            }
                        }
                    };
                })
            };

            controlslayout = new StackLayout
            {
                Padding = 10,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Children = {  new Label{ Text = "Pitch"},
                    sliderPitch,
                    new Label{ Text = "Speak Rate"},
                    sliderRate,
                    new Label{ Text = "Volume"},
                    sliderVolume,
                    new Label{ Text = "Use Defaults"},
                    useDefaults,
                    languageButton,
                    speakButton,
                    }
            };

            converseButton = new Button
            {
                Text = "Conversation"
            };

            converseButton.Clicked += (sender, args) =>
            {
                listviewlayout.IsVisible = true;
                controlslayout.IsVisible = false;

            };

            controlsButton = new Button
            {
                Text = "Controls"
            };
            controlsButton.Clicked += (sender, args) =>
            {

                listviewlayout.IsVisible = false;
                controlslayout.IsVisible = true;

            };

            controlsButton.Clicked += (sender, args) =>
            {
                listviewlayout.IsVisible = false;
                controlslayout.IsVisible = true;
            };
         }
        public void SpeechList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Check that there is a selected item
            if (e.SelectedItem == null) return;
            // DisplayAlert("Selected", e.SelectedItem.ToString(), "OK");

            SpeechItem item = e.SelectedItem as SpeechItem;

            if (item.SpeechText != "")
            {
                TextSpeaker(item.SpeechText);
            }

            // Clear the selected item in the list
            SpeechList.SelectedItem = null;
        }
        protected async override void OnAppearing()
        {
            if (_speechlist[0] != null && _speechlist[0].Speaker == "CaSI")
            {
                  TextSpeaker(_speechlist[0].SpeechText);
            }

            await bingSpeechService.InitAuth();
        }
        public void TextSpeaker(string text)
        {
            CrossTextToSpeech.Current.Speak(text,
                pitch: (float)sliderPitch.Value,
                speakRate: (float)sliderRate.Value,
                volume: (float)sliderVolume.Value,
                crossLocale: locale);
        }
		public void AddtoSpeechList(SpeechItem item)
		{
			_speechlist.Insert(0, item);
			if (_speechlist.Count() > 8)
			{
				_speechlist.RemoveAt(_speechlist.Count - 1);
			}
		}
        async void Record_OnClick()
        {
            try
            {
                var audioRecordingService = DependencyService.Get<IAudioRecorderService>();
                if (!isRecording)
                {
                    recording.IsVisible = true;
                    record.IsVisible = false;
                    audioRecordingService.StartRecording();
                    //	IsProcessing = true;
                }
                else
                {
                    recording.IsVisible = false;
                    record.IsVisible = true;
                    audioRecordingService.StopRecording();
                }

                isRecording = !isRecording;
                if (!isRecording)
                {
                    speechResult = await bingSpeechService.RecognizeSpeechAsync(Constants.AudioFilename);

                    if (!string.IsNullOrWhiteSpace(speechResult.DisplayText))
                    {
                        var result = new SpeechItem();
                        result.SpeechText = speechResult.DisplayText;
                        result.Speaker = " - ";
                        ProcessResponse(result);
                    }
                }
            }
            catch (Exception ex)
            {
                return;
                //Debug.WriteLine(ex.Message);
            }
            finally
            {
            }
        }
        public void ProcessResponse(SpeechItem item)
        {
            AddtoSpeechList(item);
            var text = item.SpeechText.ToLower();
			listviewlayout.IsVisible = false;
			controlslayout.IsVisible = false;

			if (text.Contains("say ") && text.Contains(" to "))
			{
				listviewlayout.IsVisible = true;
				var speech = new SpeechItem();
				var temp = text.Substring(text.IndexOf(" say ") + 5);
				speech.SpeechText = temp.Substring(0, temp.IndexOf(" to ")) + " ";
				temp = text.Substring(text.IndexOf(" to ") + 4);
				speech.SpeechText += temp;
				speech.Speaker = "CaSI";
				AddtoSpeechList(speech);
			}
			else if (text.Contains("show"))
			{
				if (text.Contains("conversation"))
				{
					listviewlayout.IsVisible = true;
				}
				else if (text.Contains("controls"))
				{
					controlslayout.IsVisible = true;
				}
			}
			SpeechList.ItemsSource = null;
			SpeechList.ItemsSource = _speechlist;
			if (_speechlist[0].Speaker == "CaSI")
			{
				TextSpeaker(_speechlist[0].SpeechText);
			}
		}
	}
}