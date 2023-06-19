using Todo_List_Plus.Models;
using Todo_List_Plus.Services;

namespace Todo_List_Plus.Views;

public partial class List : ContentPage
{
	IEnumerable<Models.Task> ListOfTasks { get; set; }
	User LoggedUser { get; set; }
    Models.List LoggedList { get; set; }

	public List(User user, Models.List list)
	{
		InitializeComponent();

        NavigationPage.SetHasNavigationBar(this, false);
        NavigationPage.SetBackButtonTitle(this, null);

        LoggedUser = user;
        LoggedList = list;

        RefreshView();

        L_ListName.Text = list.Name;
    }

    async void RefreshView()
    {
        ListOfTasks = await RestService.TasksGet(LoggedList);

        if (ListOfTasks is null)
        {
            Console.WriteLine("There was an error while loading the data");
            return;
        }
        
        foreach (var task in ListOfTasks)
        {
            if (!task.IsCompleted)
            AppendTask(task);
        }
    }

    async System.Threading.Tasks.Task ReloadView()
    {
        await Navigation.PushAsync(new List(LoggedUser, LoggedList));
    }

    void AppendTask(Models.Task task)
    {
        /* Prepare gestures */
        var taskDoubleTap = new TapGestureRecognizer();
        taskDoubleTap.NumberOfTapsRequired = 2;
        taskDoubleTap.Tapped += Gesture_DoubleTap;

        /* Creating the view */
        HorizontalStackLayout hsl = new();
        hsl.ClassId = $"hsl_{task.Id}";
        hsl.WidthRequest = 300;
        hsl.Margin = new Thickness(0, 5);

        Label header = new();
        header.ClassId = $"task_{task.Id}";
        header.Text = task.Name;
        header.Margin = new Thickness(10, 20, 10, 0);
        header.WidthRequest = 250;
        header.TextColor = Color.FromRgb(255,255,255);
        header.FontSize = 15;
        header.FontAttributes = FontAttributes.Bold;
        header.GestureRecognizers.Add(taskDoubleTap);
        hsl.Add(header);

        Button check = new();
        check.ClassId = $"check_{task.Id}";
        check.Text = "✓";
        check.Clicked += Button_CompleteTask_Clicked;
        check.FontSize = 10;
        check.BackgroundColor = Color.FromRgb(138,241,115);
        hsl.Add(check);

        VSL_Main.Add(hsl);
    }

    private async void Button_Back_Clicked(object sender, EventArgs e)
    {
        Console.WriteLine("Going back");
        await Navigation.PushAsync(new Categories(LoggedUser));
    }

    private async void Button_Refresh_Clicked(object sender, EventArgs e)
    {
        Console.WriteLine("Reloading view");
        await ReloadView();
    }

    private async void Button_AddTask_Clicked(object sender, EventArgs e)
    {
        Console.WriteLine("Adding a task modal");
        string result = await DisplayPromptAsync("Add task", "Enter task name");
        if (result is not null)
        {
            Console.WriteLine($"Adding task with name {result}");
            bool apiResult = await RestService.TaskAdd(result, LoggedUser, LoggedList);
            if (apiResult)
            {
                Console.WriteLine($"API call 'TaskAdd' finished with result {apiResult}");
                ToastService.ShowShort("Successfully added new task");
                await ReloadView();
            }
            else
                ToastService.ShowShort("An error occured while adding the task");
        }
    }

    private async void Button_AddUser_Clicked(object sender, EventArgs e)
    {
        Console.WriteLine("Adding a user modal");
        string result = await DisplayPromptAsync("Add user", "Enter username");
        if (result is not null)
        {
            Console.WriteLine($"Adding user with username {result}");
            bool apiResult = await RestService.UserAdd(result, LoggedList);
            if (apiResult)
            {
                Console.WriteLine($"API call 'UserAdd' finished with result {apiResult}");
                ToastService.ShowShort("Successfully added user");
                await ReloadView();
            }
            else
                ToastService.ShowShort("An error occured while adding the user");
        }
    }

    private async void Button_CompleteTask_Clicked(object sender, EventArgs e)
    {
        Console.WriteLine("Completing a task modal");
        int taskId = Convert.ToInt32(((Button)sender).ClassId.Replace("check_", ""));
        bool apiResult = await RestService.TaskToggle(taskId);
        if (apiResult)
        {
            Console.WriteLine($"API call 'TaskToggle' finished with result {apiResult}");
            ToastService.ShowShort("Successfully completed task");
            await ReloadView();
        }
        else
            ToastService.ShowShort("An error occured while completing the task");
    }

    private async void Gesture_DoubleTap(object s, EventArgs e)
    {
        var sender = (Label)s;
        Console.WriteLine($"Double tapped task with name {sender.ClassId}");

        var task = ListOfTasks.Where(a => a.Name == sender.Text).Single();

        string action = await DisplayActionSheet("Task options", "Cancel", null, new string[] { "Edit", "Delete" });
        if (action == "Edit")
        {
            string result = await DisplayPromptAsync("Edit task", "Enter task name", initialValue: sender.Text);
            if (result is not null)
            {
                Console.WriteLine($"Changing task name from {sender.Text} to {result}");
                bool apiResult = await RestService.TaskEdit(result, task);
                if (apiResult)
                {
                    Console.WriteLine($"API call 'TaskEdit' finished with result {apiResult}");
                    ToastService.ShowShort("Successfully changed task name");
                    await ReloadView();
                }
                else
                    ToastService.ShowShort("An error occured while editing the task");
            }
        }
        else if (action == "Delete")
        {
            bool answer = await DisplayAlert("Deleting task", $"Are you sure you want to delete task '{sender.Text}'?", "Yes", "No");
            if (answer)
            {
                Console.WriteLine($"Deleting task with name {sender.Text}");
                bool apiResult = await RestService.TaskDelete(task);
                if (apiResult)
                {
                    Console.WriteLine($"API call 'TaskDelete' finished with result {apiResult}");
                    ToastService.ShowShort("Successfully deleted task");
                    await ReloadView();
                }
                else
                    ToastService.ShowShort("An error occured while deleting the task");
            }
        }
    }
}