using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Todo_List_Plus.Services
{
    static class ToastService
    {
        public static async void ShowShort(string message)
        {
            var toast = Toast.Make(message);
            await toast.Show();
        }
    }
}
