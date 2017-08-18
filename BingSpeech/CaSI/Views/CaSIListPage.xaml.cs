using System;
using Xamarin.Forms;
using CaSI.Models;

namespace CaSI.Views
{
	public partial class CaSIListPage : ContentPage
	{
		
		public CaSIListPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			listView.ItemsSource = await App.CaSIManager.GetAllItemsAsync();
		}

		async void OnItemAdded(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new CaSIItemPage
			{
				CaSIItem = new CaSIItem()
			});
		}

		async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			await Navigation.PushAsync(new CaSIItemPage
			{
				CaSIItem = e.SelectedItem as CaSIItem
			});
		}

		async void OnRateApplication(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new RateAppPage());
		}
	}
}
