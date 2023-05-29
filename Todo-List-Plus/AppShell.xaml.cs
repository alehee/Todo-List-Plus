using Todo_List_Plus.Views;

namespace Todo_List_Plus;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute("categories", typeof(Categories));
	}
}
