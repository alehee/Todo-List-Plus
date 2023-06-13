using Todo_List_Plus.Models;
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

	private void LoginBtnClick(object sender, EventArgs e)
	{
		string username = txtUsername.Text;
		string password = txtPassword.Text;

        if (username == "" || password == "")
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
		Console.WriteLine($"Trying to validate u: {username}, p: {password}");
		User? user = await RestService.Login(username, password);
		if (user is null)
		{
            ToastService.ShowShort("Check your login credentials");
			return;
        }
		await Navigation.PushAsync(new Categories(user));
		
	}
}