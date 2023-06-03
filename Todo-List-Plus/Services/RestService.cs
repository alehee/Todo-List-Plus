using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Todo_List_Plus.Models;

namespace Todo_List_Plus.Services
{
    static class RestService
    {
        public static async Task<bool> CategoryAdd(string name, User user)
        {
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(Env.API_HOST + "api/Todo/CategoryAdd");
            Console.WriteLine(response.ToString());
            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseString);
                JsonNode json = JsonNode.Parse(responseString);
                return json["type"].ToString() == "SUCCESS";
            }

            return false;
        }
    }
}
