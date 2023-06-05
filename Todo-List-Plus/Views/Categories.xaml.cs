using Todo_List_Plus.Models;
using Todo_List_Plus.Services;

namespace Todo_List_Plus.Views;

public partial class Categories : ContentPage
{
	IEnumerable<Category> ListOfCategories { get; set; }
    User LoggedUser { get; set; }

	public Categories(User user)
	{
		InitializeComponent();

        NavigationPage.SetHasNavigationBar(this, false);
        NavigationPage.SetBackButtonTitle(this, null);

        LoggedUser = user;

        RefreshView();
	}

    async void RefreshView()
    {
        L_Welcome.Text = $"Czeœæ, {LoggedUser.Username}!";
        ListOfCategories = await RestService.CategoriesGet(LoggedUser);
        
        if (ListOfCategories is null)
        {
            Console.WriteLine("There was an error while loading the data");
            return;
        }

        foreach (Category category in ListOfCategories)
        {
            AppendCategory(category);
        }
    }

    async Task ReloadView()
    {
        await Navigation.PushAsync(new Categories(LoggedUser));
    }

	void AppendCategory(Category category)
	{
		/* Prepare gestures */
		var headerSingleTap = new TapGestureRecognizer();
		headerSingleTap.Tapped += Gesture_Category_SingleTap;
        var headerDoubleTap = new TapGestureRecognizer();
        headerDoubleTap.NumberOfTapsRequired = 2;
        headerDoubleTap.Tapped += Gesture_Category_DoubleTap;
        var listSingleTap = new TapGestureRecognizer();
		listSingleTap.Tapped += Gesture_List_SingleTap;
        var listDoubleTap = new TapGestureRecognizer();
        listDoubleTap.NumberOfTapsRequired = 2;
        listDoubleTap.Tapped += Gesture_List_DoubleTap;

		/* Creating the view */
        VerticalStackLayout vsl = new();
		vsl.ClassId = $"vsl_{category.Id}";
		vsl.HorizontalOptions = LayoutOptions.Center;
		vsl.WidthRequest = 300;

		Label header = new();
		header.ClassId = $"category_{category.Id}";
		header.Text = category.Name;
		header.Margin = new Thickness(10, 20, 10, 0);
		header.TextColor = Color.FromRgb(255, 255, 255);
		header.HorizontalTextAlignment = TextAlignment.Center;
		header.FontSize = 18;
		header.FontAttributes = FontAttributes.Bold;
		header.GestureRecognizers.Add(headerSingleTap);
        header.GestureRecognizers.Add(headerDoubleTap);
        vsl.Add(header);

		foreach(var l in category.Lists)
		{
            Label option = new();
			option.ClassId = $"list_{category.Id}_{l.Id}";
            option.Text = l.Name;
			option.Margin = new Thickness(5);
			option.HorizontalTextAlignment = TextAlignment.Center;
			option.GestureRecognizers.Add(listSingleTap);
            option.GestureRecognizers.Add(listDoubleTap);
            vsl.Add(option);
        }
		VSL_Main.Add(vsl);
	}

	void ToggleCategoryVisibility(int categoryId)
	{
		VerticalStackLayout vsl = new();
		foreach(var view in VSL_Main)
		{
			if (view.GetType() == typeof(VerticalStackLayout) && ((VerticalStackLayout)view).ClassId == $"vsl_{categoryId}")
			{
				vsl = (VerticalStackLayout)view;
				break;
			}
		}

		foreach(var label in vsl)
		{
			if (typeof(Label) == label.GetType() && ((Label)label).ClassId.StartsWith("list"))
			{
                var l = (Label)label;
                Console.WriteLine($"Switching for {l.ClassId}");
                if (l.IsVisible)
                    l.IsVisible = false;
                else
                    l.IsVisible = true;
            }
		}
	}

    private async void Button_Logout_Clicked(object sender, EventArgs e)
    {
        Console.WriteLine("Logging out");
        await Navigation.PushAsync(new Login());
    }

    private async void Button_AddList_Clicked(object sender, EventArgs e)
    {
        Console.WriteLine("Adding a list modal");
        string[] categoriesStrings = ListOfCategories.Select(a => a.Name).ToArray();
        string action = await DisplayActionSheet("Wybierz kategoriê", "Anuluj", null, categoriesStrings);
		var categoryQuery = ListOfCategories.Where(a => a.Name == action);
        if (categoryQuery.Any())
		{
			string result = await DisplayPromptAsync("Dodaj listê", "Podaj nazwê listy", cancel: "Anuluj");
			if (result is not null)
			{
                Console.WriteLine($"Adding list with name {result} to category {action}");
                bool apiResult = await RestService.ListAdd(result, categoryQuery.Single(), LoggedUser);
                if (apiResult)
                {
                    Console.WriteLine($"API call 'ListAdd' finished with result {apiResult}");
                    ToastService.ShowShort("Poprawnie dodano now¹ listê");
                    await ReloadView();
                }
                else
                    ToastService.ShowShort("Wyst¹pi³ problem podczas dodawania listy");
            }
		}
    }

	private async void Button_AddCategory_Clicked(object sender, EventArgs e)
	{
        Console.WriteLine("Adding a category modal");
		string result = await DisplayPromptAsync("Dodaj kategoriê", "Podaj nazwê kategorii", cancel:"Anuluj");
		if (result is not null)
		{
			Console.WriteLine($"Adding category with name {result}");
            bool apiResult = await RestService.CategoryAdd(result, LoggedUser);
            if (apiResult)
            {
                Console.WriteLine($"API call 'CategoryAdd' finished with result {apiResult}");
                ToastService.ShowShort("Poprawnie dodano now¹ kategoriê");
                await ReloadView();
            }
            else
                ToastService.ShowShort("Wyst¹pi³ problem podczas dodawania kategorii");
        }
    }

	private void Gesture_Category_SingleTap(object s, EventArgs e)
	{
        var sender = (Label)s;
        Console.WriteLine($"Tapped category with name {sender.ClassId}");
		int categoryId = Convert.ToInt32(sender.ClassId.Replace("category_", ""));
		ToggleCategoryVisibility(categoryId);
    }

    private async void Gesture_Category_DoubleTap(object s, EventArgs e)
    {
        var sender = (Label)s;
        Console.WriteLine($"Double tapped category with name {sender.ClassId}");

        var category = ListOfCategories.Where(a => a.Name == sender.Text);

        string action = await DisplayActionSheet("Opcje kategorii", "Anuluj", null, new string[] { "Edytuj", "Usuñ" });
		if (action == "Edytuj")
		{
            string result = await DisplayPromptAsync("Edytuj kategoriê", "Podaj nazwê kategorii", cancel: "Anuluj", initialValue:sender.Text);
			if ( result is not null)
			{
                Console.WriteLine($"Changing category name from {sender.Text} to {result}");
                bool apiResult = await RestService.CategoryEdit(result, category.Single());
                if (apiResult)
                {
                    Console.WriteLine($"API call 'CategoryEdit' finished with result {apiResult}");
                    ToastService.ShowShort("Poprawnie zmieniono nazwê kategorii");
                    await ReloadView();
                }
                else
                    ToastService.ShowShort("Wyst¹pi³ problem podczas zmieniania nazwy kategorii");
            }
        }
		else if (action == "Usuñ")
		{
			bool answer = await DisplayAlert("Usuwanie kategorii", $"Czy na pewno chcesz usun¹æ kategoriê '{sender.Text}'?", "Tak", "Nie");
			if (answer)
			{
                Console.WriteLine($"Deleting category with name {sender.Text}");
                bool apiResult = await RestService.CategoryDelete(category.Single());
                if (apiResult)
                {
                    Console.WriteLine($"API call 'CategoryDelete' finished with result {apiResult}");
                    ToastService.ShowShort("Poprawnie usuniêto kategoriê");
                    await ReloadView();
                }
                else
                    ToastService.ShowShort("Wyst¹pi³ problem podczas usuwania kategorii");
            }
        }
    }

    private void Gesture_List_SingleTap(object s, EventArgs e)
    {
        var sender = (Label)s;
        Console.WriteLine($"Tapped list with name {sender.ClassId}");
		// TODO przejœcie do kategorii
    }

    private async void Gesture_List_DoubleTap(object s, EventArgs e)
    {
        var sender = (Label)s;
        Console.WriteLine($"Double tapped list with name {sender.ClassId}");

        var listClassIdSplit = sender.ClassId.ToString().Replace("list_", "").Split('_');
        int categoryId = Convert.ToInt32(listClassIdSplit[0]);
        int listId = Convert.ToInt32(listClassIdSplit[1]);
        var category = ListOfCategories.Where(a => a.Id == categoryId);
        var list = category.Single().Lists.Where(a => a.Id == listId);

        string action = await DisplayActionSheet("Opcje listy", "Anuluj", null, new string[] { "Edytuj", "Usuñ" });
        if (action == "Edytuj")
        {
            string result = await DisplayPromptAsync("Edytuj listê", "Podaj nazwê listy", cancel: "Anuluj", initialValue: sender.Text);
            if (result is not null)
            {
                Console.WriteLine($"Changing list name from {sender.Text} to {result}");
                bool apiResult = await RestService.ListEdit(result, category.Single(), list.Single(), LoggedUser);
                if (apiResult)
                {
                    Console.WriteLine($"API call 'ListEdit' finished with result {apiResult}");
                    ToastService.ShowShort("Poprawnie zmieniono nazwê listy");
                    await ReloadView();
                }
                else
                    ToastService.ShowShort("Wyst¹pi³ problem podczas zmieniania nazwy listy");
            }
        }
        else if (action == "Usuñ")
        {
            bool answer = await DisplayAlert("Usuwanie listy", $"Czy na pewno chcesz usun¹æ listê '{sender.Text}'?", "Tak", "Nie");
            if (answer)
            {
                Console.WriteLine($"Deleting list with name {sender.Text}");
                bool apiResult = await RestService.ListDelete(list.Single());
                if (apiResult)
                {
                    Console.WriteLine($"API call 'ListDelete' finished with result {apiResult}");
                    ToastService.ShowShort("Poprawnie usuniêto listê");
                    await ReloadView();
                }
                else
                    ToastService.ShowShort("Wyst¹pi³ problem podczas usuwania listy");
            }
        }
    }
}