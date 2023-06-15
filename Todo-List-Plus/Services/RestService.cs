using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using Todo_List_Plus.Models;

namespace Todo_List_Plus.Services
{

    static class RestService
    {
        #region Login
        public static async Task<User?> Login(string username, string password)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(Env.API_HOST + $"api/Todo/Login?username={HttpUtility.UrlEncodeUnicode(username)}&password={HttpUtility.UrlEncodeUnicode(password)}", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                if (jsonNode["type"].ToString() != "SUCCESS")
                    return null;

                var jsonUser = jsonNode["message"];
                return new User { Id = (int)jsonUser["id"], Username = (string)jsonUser["username"] };
            }

            return null;
        }

        public static async Task<string> Register(string username, string password)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(Env.API_HOST + $"api/Todo/Register?username={HttpUtility.UrlEncodeUnicode(username)}&password={HttpUtility.UrlEncodeUnicode(password)}", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                if (jsonNode["type"].ToString() != "SUCCESS")
                    return (string)jsonNode["message"];
                return "OK";
            }

            return "An error occured while connecting to the server";
        }
        #endregion

        #region Categories
        public static async Task<IEnumerable<Category>?> CategoriesGet(User user)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(Env.API_HOST + $"api/Todo/GetCategories?userId={user.Id}", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                if (jsonNode["type"].ToString() != "SUCCESS")
                    return null;

                List<Category> categories = new();
                JsonArray jsonArray = jsonNode["message"].AsArray();
                foreach (var jsonCategory in jsonArray)
                {
                    var category = new Category { Id = (int)jsonCategory["id"], Name = (string)jsonCategory["name"] };
                    category.Lists = (List<List>)await ListsGet(category, user);
                    categories.Add(category);
                }
                return categories;
            }

            return null;
        }

        public static async Task<bool> CategoryAdd(string name, User user)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(Env.API_HOST + $"api/Todo/CategoryAdd?name={HttpUtility.UrlEncodeUnicode(name)}&userId={user.Id}", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                return jsonNode["type"].ToString() == "SUCCESS";
            }

            return false;
        }

        public static async Task<bool> CategoryEdit(string name, Category category)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(Env.API_HOST + $"api/Todo/CategoryEdit?name={HttpUtility.UrlEncodeUnicode(name)}&categoryId={category.Id}", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                return jsonNode["type"].ToString() == "SUCCESS";
            }

            return false;
        }

        public static async Task<bool> CategoryDelete(Category category)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(Env.API_HOST + $"api/Todo/CategoryDelete?categoryId={category.Id}", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                return jsonNode["type"].ToString() == "SUCCESS";
            }

            return false;
        }
        #endregion

        #region Lists
        public static async Task<IEnumerable<List>> ListsGet(User user)
        {
            List<List> lists = new List<List>();

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(Env.API_HOST + $"api/Todo/GetLists?userId={user.Id}", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                if (jsonNode["type"].ToString() != "SUCCESS")
                    return lists;

                JsonArray jsonArray = jsonNode["message"].AsArray();
                foreach (var jsonList in jsonArray)
                {
                    var list = new List { Id = (int)jsonList["id"], Name = (string)jsonList["name"] };
                    lists.Add(list);
                }
            }

            return lists;
        }

        public static async Task<IEnumerable<List>> ListsGet(Category category, User user)
        {
            List<List> lists = new List<List>();

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(Env.API_HOST + $"api/Todo/GetLists?userId={user.Id}&categoryId={category.Id}", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                if (jsonNode["type"].ToString() != "SUCCESS")
                    return lists;

                JsonArray jsonArray = jsonNode["message"].AsArray();
                foreach (var jsonList in jsonArray)
                {
                    var list = new List { Id = (int)jsonList["id"], Name = (string)jsonList["name"] };
                    lists.Add(list);
                }
            }

            return lists;
        }

        public static async Task<bool> ListAdd(string name, Category category, User user)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(Env.API_HOST + $"api/Todo/ListAdd?name={HttpUtility.UrlEncodeUnicode(name)}&categoryId={category.Id}&userId={user.Id}", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                return jsonNode["type"].ToString() == "SUCCESS";
            }

            return false;
        }

        public static async Task<bool> ListEdit(string name, Category category, List list, User user)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(Env.API_HOST + $"api/Todo/ListEdit?name={HttpUtility.UrlEncodeUnicode(name)}&categoryId={category.Id}&listId={list.Id}&userId={user.Id}", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                return jsonNode["type"].ToString() == "SUCCESS";
            }

            return false;
        }

        public static async Task<bool> ListDelete(List list)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(Env.API_HOST + $"api/Todo/ListDelete?listId={list.Id}", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                return jsonNode["type"].ToString() == "SUCCESS";
            }

            return false;
        }

        public static async Task<IEnumerable<List>> ListsWithoutCategory(User user, IEnumerable<List> prevLists)
        {
            List<List> lists = new List<List>();

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(Env.API_HOST + $"api/Todo/GetListCategories?userId={user.Id}", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                if (jsonNode["type"].ToString() != "SUCCESS")
                    return lists;

                JsonArray jsonArray = jsonNode["message"].AsArray();
                foreach (var jsonList in jsonArray)
                {
                    if ((int?)jsonList["categoryId"] is not null)
                        continue;

                    var list = prevLists.Where(a => a.Id == (int)jsonList["listId"]).Single();
                    lists.Add(list);
                }
            }

            return lists;
        }
        #endregion

        #region Tasks
        public static async Task<IEnumerable<Models.Task>?> TasksGet(List list)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(Env.API_HOST + $"api/Todo/GetTasks?listId={list.Id}", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                if (jsonNode["type"].ToString() != "SUCCESS")
                    return null;

                List<Models.Task> tasks = new();
                JsonArray jsonArray = jsonNode["message"].AsArray();
                foreach (var jsonTask in jsonArray)
                {
                    var task = new Models.Task { Id = (int)jsonTask["id"], Name = (string)jsonTask["name"], IsCompleted = (jsonTask["completedAt"] is not null) };
                    tasks.Add(task);
                }
                return tasks;
            }

            return null;
        }

        public static async Task<bool> TaskAdd(string name, User user, List list)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(Env.API_HOST + $"api/Todo/TaskAdd?name={HttpUtility.UrlEncodeUnicode(name)}&userId={user.Id}&listId={list.Id}", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                return jsonNode["type"].ToString() == "SUCCESS";
            }

            return false;
        }

        public static async Task<bool> TaskEdit(string name, Models.Task task)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(Env.API_HOST + $"api/Todo/TaskEdit?name={HttpUtility.UrlEncodeUnicode(name)}&taskId={task.Id}", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                return jsonNode["type"].ToString() == "SUCCESS";
            }

            return false;
        }

        public static async Task<bool> TaskDelete(Models.Task task)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(Env.API_HOST + $"api/Todo/TaskDelete?taskId={task.Id}", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                return jsonNode["type"].ToString() == "SUCCESS";
            }

            return false;
        }

        public static async Task<bool> TaskToggle(int taskId)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(Env.API_HOST + $"api/Todo/TaskToggle?taskId={taskId}", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                return jsonNode["type"].ToString() == "SUCCESS";
            }

            return false;
        }
        #endregion

        #region User management
        public static async Task<bool> UserAdd(string username, List list)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(Env.API_HOST + $"api/Todo/ListAddUser?username={HttpUtility.UrlEncodeUnicode(username)}&listId={list.Id}", null);

            if (response.IsSuccessStatusCode)
            {
                var jsonNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                return jsonNode["type"].ToString() == "SUCCESS";
            }

            return false;
        }
        #endregion
    }
}
