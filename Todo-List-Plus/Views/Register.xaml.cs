using Todo_List_Plus.Services;

namespace Todo_List_Plus.Views;

public partial class Register : ContentPage
{
	public Register()
	{
		InitializeComponent();

        NavigationPage.SetHasNavigationBar(this, false);
        NavigationPage.SetBackButtonTitle(this, null);
    }

	private async void RegisterMeBtnClick(object sender, EventArgs e)
	{
        string username = txtUsernameRegister.Text;
        string password = txtPasswordRegister.Text;
        string repassword = txtRetypeRegister.Text;

        if (String.IsNullOrEmpty(username)  || String.IsNullOrEmpty(password) || String.IsNullOrEmpty(repassword))
            ToastService.ShowShort("Fill all of the entries!");
        else if (password != repassword)
            ToastService.ShowShort("Passwords are different");
        else
        {
            string result = await RestService.Register(username, password);
            if (result != "OK")
            {
                ToastService.ShowShort(result);
                return;
            }
            ToastService.ShowShort("Registered successfully!");
            await Navigation.PushAsync(new Login());
        }
    }
}