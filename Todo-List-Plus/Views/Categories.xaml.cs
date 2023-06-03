using Todo_List_Plus.Models;
using Todo_List_Plus.Services;

namespace Todo_List_Plus.Views;

public partial class Categories : ContentPage
{
	List<Category> ListOfCategories { get; set; } = new();
    User LoggedUser { get; set; }

	public Categories()
	{
		InitializeComponent();

        ListOfCategories = _FillTestData();
        LoggedUser = new User { Id = 1, Username = "Skafander" };
		foreach (Category category in ListOfCategories)
		{
			AppendCategory(category);
		}
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
				// TODO endpoint i refresh
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
            Console.WriteLine($"API call 'CategoryAdd' finished with result {apiResult.ToString()}");
            // TODO refresh
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
        string action = await DisplayActionSheet("Opcje kategorii", "Anuluj", null, new string[] { "Edytuj", "Usuñ" });
		if (action == "Edytuj")
		{
            string result = await DisplayPromptAsync("Edytuj kategoriê", "Podaj nazwê kategorii", cancel: "Anuluj", initialValue:sender.Text);
			if ( result is not null)
			{
                Console.WriteLine($"Changing category name from {sender.Text} to {result}");
                // TODO endpoint i refresh
            }
        }
		else if (action == "Usuñ")
		{
			bool answer = await DisplayAlert("Usuwanie kategorii", $"Czy na pewno chcesz usun¹æ kategoriê '{sender.Text}'?", "Tak", "Nie");
			if (answer)
			{
                Console.WriteLine($"Deleting category with name {sender.Text}");
                // TODO endpoint i refresh
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
        string action = await DisplayActionSheet("Opcje listy", "Anuluj", null, new string[] { "Edytuj", "Usuñ" });
        if (action == "Edytuj")
        {
            string result = await DisplayPromptAsync("Edytuj listê", "Podaj nazwê listy", cancel: "Anuluj", initialValue: sender.Text);
            if (result is not null)
            {
                Console.WriteLine($"Changing list name from {sender.Text} to {result}");
                // TODO endpoint i refresh
            }
        }
        else if (action == "Usuñ")
        {
            bool answer = await DisplayAlert("Usuwanie listy", $"Czy na pewno chcesz usun¹æ listê '{sender.Text}'?", "Tak", "Nie");
            if (answer)
            {
                Console.WriteLine($"Deleting list with name {sender.Text}");
                // TODO endpoint i refresh
            }
        }
    }

    List<Category> _FillTestData()
    {
        List<Category> list = new()
        {
            new Category { Id = 1, Name = "Kategoria pierwsza", Lists = new List<Models.List> { new List { Id = 1, Name = "Zadanie pierwsze" }, new List { Id = 2, Name = "Zadanie drugie" }, new List { Id = 3, Name = "Zadanie trzecie" } } },
            new Category { Id = 2, Name = "Kategoria druga", Lists = new List<Models.List> { new List { Id = 4, Name = "Zadanie pierwsze" }, new List { Id = 5, Name = "Zadanie drugie" }, new List { Id = 6, Name = "Zadanie trzecie" } } }
        };

        return list;
    }
}