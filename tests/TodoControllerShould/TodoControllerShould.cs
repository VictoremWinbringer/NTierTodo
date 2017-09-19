using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NTierTodo;
using NTierTodo.Bll.Dto;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Xunit;
using static Newtonsoft.Json.JsonConvert;
namespace TodoControllerShould
{
    public class TodoControllerShould
    {
        private readonly HttpClient _client;
        private readonly ToDoDto _todo;
        private readonly string _root;

        public TodoControllerShould()
        {
            _client = GetHttpClient();

            _todo = new ToDoDto
            {
                Description = "Bla bla bla"
            };

            _root = "api/v1/todo/";
        }

        private string ToJson(ToDoDto todo)
        {
            return SerializeObject(todo);
        }

        private ToDoDto FromJson(string todo)
        {
            return DeserializeObject<ToDoDto>(todo);
        }

        private StringContent CreateContent(string json)
        {
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private async Task<ToDoDto> FromContent(HttpContent content)
        {
            return FromJson(await content.ReadAsStringAsync());
        }

        private async Task<ToDoDto> Get(Guid id)
        {
            var response = await _client.GetAsync(_root + id);

            response.EnsureSuccessStatusCode();

            return await FromContent(response.Content);
        }

        [Fact]
        public async Task Crud()
        {
            await Create();
            await ReadAll();
            await Read();
            await Update();
            await Delete();
        }


        [Fact]
        public async Task Not_Create_Duplicate_Description()
        {
            var todo = new ToDoDto
            {
                Description = "Ololosh"
            };

            var response = await _client.PostAsync(_root, CreateContent(ToJson(todo)));

            response.EnsureSuccessStatusCode();

            var result = await FromContent(response.Content);

            var duplicate = await _client.PostAsync(_root, CreateContent(ToJson(todo)));

            await _client.DeleteAsync(_root + result.Id);

            Assert.NotEqual(true, duplicate.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Not_Create_Too_Short_Description()
        {
            var todo = new ToDoDto
            {
                Description = "O"
            };

            var response = await _client.PostAsync(_root, CreateContent(ToJson(todo)));

            Assert.NotEqual(true, response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Not_Create_Null_Or_Empty_Description()
        {
            var todo = new ToDoDto
            {
                Description = "       "
            };

            var response = await _client.PostAsync(_root, CreateContent(ToJson(todo)));

            Assert.NotEqual(true, response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Not_Create_Nul_Description()
        {
            var todo = new ToDoDto
            {
                Description = null
            };

            var response = await _client.PostAsync(_root, CreateContent(ToJson(todo)));

            Assert.NotEqual(true, response.IsSuccessStatusCode);
        }

        private async Task Create()
        {
            var response = await _client.PostAsync(_root, CreateContent(ToJson(_todo)));

            response.EnsureSuccessStatusCode();

            var result = await FromContent(response.Content);

            Assert.NotNull(result);
            Assert.NotEqual(default(Guid), result.Id);
            Assert.Equal(_todo.Description, result.Description);
            Assert.Equal(false, result.IsComplite);

            _todo.Id = result.Id;
        }

        private async Task ReadAll()
        {
            var result = await _client.GetAsync(_root);

            result.EnsureSuccessStatusCode();

            var array = DeserializeObject<ToDoDto[]>(await result.Content.ReadAsStringAsync());

            Assert.Contains(array, x => x.Id == _todo.Id);

        }

        private async Task Read()
        {
            var result = await Get(_todo.Id);

            Assert.Equal(_todo.Id, result.Id);
            Assert.Equal(_todo.Description, result.Description);
            Assert.Equal(_todo.IsComplite, result.IsComplite);
        }

        private async Task Update()
        {
            _todo.Description = "Foo Bar Baz";
            _todo.IsComplite = true;

            var response = await _client.PutAsync(_root + _todo.Id, CreateContent(ToJson(_todo)));

            response.EnsureSuccessStatusCode();

            var result = await Get(_todo.Id);

            Assert.Equal(_todo.Id, result.Id);
            Assert.Equal(_todo.Description, result.Description);
            Assert.NotEqual(_todo.IsComplite, result.IsComplite);
        }

        [Fact]
        private async Task MakeComplete()
        {
            var todo = new ToDoDto
            {
                Description = "MakeComplete"
            };

            var create = await _client.PostAsync(_root, CreateContent(ToJson(todo)));

            create.EnsureSuccessStatusCode();

            var id =(await FromContent(create.Content)).Id;

            var response = await _client.PostAsync(_root + id + "/MakeComplete", new StringContent(""));

            response.EnsureSuccessStatusCode();

            var result = await Get(id);

            Assert.Equal(id, result.Id);
            Assert.Equal(todo.Description, result.Description);
            Assert.Equal(true, result.IsComplite);

            await _client.DeleteAsync(_root + id);
        }

        private async Task Delete()
        {
            var response = await _client.DeleteAsync(_root + _todo.Id);

            response.EnsureSuccessStatusCode();

            var result = await _client.GetAsync(_root);

            result.EnsureSuccessStatusCode();

            var array = DeserializeObject<ToDoDto[]>(await result.Content.ReadAsStringAsync());

            Assert.DoesNotContain(array, x => x.Id == _todo.Id);
        }

        private HttpClient GetHttpClient()
        {
            var server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>());

            var client = server.CreateClient();

            // client always expects json results
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}
