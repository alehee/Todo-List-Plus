using Todo_List_Plus.Views;

namespace Todo_List_Plus;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        MainPage = new NavigationPage(new Login());
	}
}
