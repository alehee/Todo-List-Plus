namespace Todo_List_Plus.Views;

public partial class Register : ContentPage
{
	public Register()
	{
		InitializeComponent();
	}

	private void RegisterMeBtnClick(object sender, EventArgs e)
	{
        if (String.IsNullOrEmpty(txtUsernameRegister.Text)  || String.IsNullOrEmpty(txtPasswordRegister.Text) || String.IsNullOrEmpty(txtRetypeRegister.Text))
        {
            DisplayAlert("Oopss...", "Something is missing...", "Ok");
        }
        else if (txtRetypeRegister.Text != txtPasswordRegister.Text)
        {
            DisplayAlert("Oopss...", "Passwords are different", "Ok");
        }
        else
        {
            Navigation.PushAsync(new Categories());
        }

    }
}