using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CaSI.Repository;
using CaSI.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace CaSI
{
	public partial class App : Application
	{
		/*
		static ICaSIItemRepository CaSIItemRepository;

		public static ICaSIItemRepository CaSIManager
		{
			get
			{
				if (CaSIItemRepository == null)
				{
					CaSIItemRepository = new CaSIItemRepository(DependencyService.Get<IFileHelper>().GetLocalFilePath("CaSISQLite.db3"));
				}
				return CaSIItemRepository;
			}
		}
		*/
		public App()
		{
			InitializeComponent();

			MainPage = new NavigationPage(new HomePage());
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
