using Todo_List_Plus.Models;

namespace Todo_List_Plus.Views;

public partial class Categories : ContentPage
{
	public Categories()
	{
		InitializeComponent();
		
		List<Category> categories = _FillTestData();
		foreach (Category category in categories)
		{
			AppendCategory(category);
		}
	}

	void AppendCategory(Category category)
	{
		VerticalStackLayout vsl = new();
		vsl.HorizontalOptions = LayoutOptions.Center;
		vsl.WidthRequest = 300;

		Label header = new();
		header.Text = category.Name;
		header.Margin = new Thickness(10, 0, 10, 0);
		header.TextColor = Color.FromRgb(255, 255, 255);
		header.HorizontalTextAlignment = TextAlignment.Center;
		header.FontSize = 18;
		header.FontAttributes = FontAttributes.Bold;
		vsl.Add(header);

		foreach(var l in category.Lists)
		{
            Label option = new();
            option.Text = l.Name;
			option.HorizontalTextAlignment = TextAlignment.Center;
            vsl.Add(option);
        }
		VSL_Main.Add(vsl);
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