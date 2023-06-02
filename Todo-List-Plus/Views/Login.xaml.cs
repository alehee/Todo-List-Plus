namespace Todo_List_Plus.Views;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
	}

	private void LoginBtnClick(object sender, EventArgs e)
	{
		if (txtUsername.Text == "admin" && txtPassword.Text == "admin")
		{
			Navigation.PushAsync(new Categories());
		}
		else
		{
			DisplayAlert("Oopss...", "Wrong username or password", "Ok");	
		}
	}

	private void RegisterBtnClick(object sender, EventArgs e)
	{
        Navigation.PushAsync(new Register());
    }
}