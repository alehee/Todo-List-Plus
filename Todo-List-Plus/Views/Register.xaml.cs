using Todo_List_Plus.Models;
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
        if (String.IsNullOrEmpty(txtUsernameRegister.Text)  || String.IsNullOrEmpty(txtPasswordRegister.Text) || String.IsNullOrEmpty(txtRetypeRegister.Text))
            ToastService.ShowShort("Fill all of the entries!");
        else if (txtRetypeRegister.Text != txtPasswordRegister.Text)
            ToastService.ShowShort("Passwords are different");
        else
        {
            // TODO
        }
    }
}