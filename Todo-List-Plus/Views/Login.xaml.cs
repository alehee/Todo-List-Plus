using Todo_List_Plus.Services;

namespace Todo_List_Plus.Views;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();

        NavigationPage.SetHasNavigationBar(this, false);
        NavigationPage.SetBackButtonTitle(this, null);
    }

	private async void LoginBtnClick(object sender, EventArgs e)
	{
		string username = txtUsername.Text;
		string password = txtPassword.Text;

        if (username != "" || password != "")
		{
			ToastService.ShowShort("Fill all of the entries");
			return;
		}
		Validate(username, password);
	}

	private async void RegisterBtnClick(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new Register());
    }

	private async void Validate(string username, string password)
	{
		// TODO
	}
}