using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NTierTodo;
using NTierTodo.Bll;
using Xunit;

namespace NTierTodoTests
{
    public class TodoControllerShould : IDisposable
    {
        private readonly HttpClient _client;
        private readonly ToDoDto _todo;
        private readonly string _root;
        private readonly TestServer _server;

        public TodoControllerShould()
        {
            var server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>());

            var client = server.CreateClient();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            _client = client;

            _server = server;

            _todo = new ToDoDto
            {
                Description = "Bla bla bla"
            };

            _root = "api/v1/todo/";
        }

        private string ToJson(ToDoDto todo)
        {
            return JsonConvert.SerializeObject(todo);
        }

        private ToDoDto FromJson(string todo)
        {
            return JsonConvert.DeserializeObject<ToDoDto>(todo);
        }

        private StringContent CreateContent(ToDoDto todo)
        {
            return new StringContent(ToJson(todo), Encoding.UTF8, "application/json");
        }

        private async Task<ToDoDto> FromContent(HttpContent content)
        {
            return FromJson(await content.ReadAsStringAsync());
        }

        private async Task<ToDoDto> Get(Guid id, HttpClient client)
        {
            var response = await client.GetAsync(_root + id);

            response.EnsureSuccessStatusCode();

            return await FromContent(response.Content);
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
        public async Task Count_Equal_1()
        {
            var todo = new ToDoDto
            {
                Description = "One"
            };

            var response = await _client.PostAsync(_root, CreateContent(todo));

            response.EnsureSuccessStatusCode();

            var created = await FromContent(response.Content);

            response = await _client.GetAsync(_root + "count");

            response.EnsureSuccessStatusCode();

            var result = int.Parse(await response.Content.ReadAsStringAsync());

            await _client.DeleteAsync(_root + created.Id);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task Not_Create_Duplicate_Description()
        {
            var todo = new ToDoDto
            {
                Description = "Ololosh"
            };

            var response = await _client.PostAsync(_root, CreateContent(todo));

            response.EnsureSuccessStatusCode();

            var result = await FromContent(response.Content);

            var duplicate = await _client.PostAsync(_root, CreateContent(todo));

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

            var response = await _client.PostAsync(_root, CreateContent(todo));

            Assert.NotEqual(true, response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Not_Create_Empty_Description()
        {
            var todo = new ToDoDto
            {
                Description = "       "
            };

            var response = await _client.PostAsync(_root, CreateContent(todo));

            Assert.NotEqual(true, response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Not_Create_Nul_Description()
        {
            var todo = new ToDoDto
            {
                Description = null
            };

            var response = await _client.PostAsync(_root, CreateContent(todo));

            Assert.NotEqual(true, response.IsSuccessStatusCode);
        }

        private async Task Create()
        {
            var response = await _client.PostAsync(_root, CreateContent(_todo));

            response.EnsureSuccessStatusCode();

            var result = await FromContent(response.Content);

            Assert.NotNull(result);
            Assert.NotEqual(default(Guid), result.Id);
            Assert.Equal(_todo.Description, result.Description);
            Assert.Equal(false, result.IsComplete);

            _todo.Id = result.Id;
        }

        private async Task ReadAll()
        {
            var result = await _client.GetAsync(_root);

            result.EnsureSuccessStatusCode();

            var array = JsonConvert.DeserializeObject<ToDoDto[]>(await result.Content.ReadAsStringAsync());

            Assert.Contains(array, x => x.Id == _todo.Id);

        }

        private async Task Read()
        {
            var result = await Get(_todo.Id);

            Assert.Equal(_todo.Id, result.Id);
            Assert.Equal(_todo.Description, result.Description);
            Assert.Equal(_todo.IsComplete, result.IsComplete);
        }

        private async Task Update()
        {
            _todo.Description = "Foo Bar Baz";
            _todo.IsComplete = true;

            var response = await _client.PutAsync(_root + _todo.Id, CreateContent(_todo));

            response.EnsureSuccessStatusCode();

            var result = await Get(_todo.Id);

            Assert.Equal(_todo.Id, result.Id);
            Assert.Equal(_todo.Description, result.Description);
            Assert.NotEqual(_todo.IsComplete, result.IsComplete);
        }


        [Fact]
        public async Task MakeComplete()
        {
            var todo = new ToDoDto
            {
                Description = "MakeComplete"
            };

            var url = "http://localhost:28888/";

            var server = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseUrls(url)
                .Build();

            Task.Run(() =>
            {
                server.Start();
            });

            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:28888/hub")
                .WithConsoleLogger()
                .Build();

            connection.On<string>("Notify", data =>
            {
                Assert.Equal(todo.Description + " is complete", data);
            });

            await connection.StartAsync();

            var client = new HttpClient();

            client.BaseAddress = new Uri(url);

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var create = await client.PostAsync(_root, CreateContent(todo));

            create.EnsureSuccessStatusCode();

            var id = (await FromContent(create.Content)).Id;

            var response = await client.PostAsync(_root + id + "/MakeComplete", new StringContent(""));

            response.EnsureSuccessStatusCode();

            var result = await Get(id, client);

            await client.DeleteAsync(_root + id);

            Assert.Equal(id, result.Id);
            Assert.Equal(todo.Description, result.Description);
            Assert.Equal(true, result.IsComplete);
        }

        private async Task Delete()
        {
            var response = await _client.DeleteAsync(_root + _todo.Id);

            response.EnsureSuccessStatusCode();

            var result = await _client.GetAsync(_root);

            result.EnsureSuccessStatusCode();

            var array = JsonConvert.DeserializeObject<ToDoDto[]>(await result.Content.ReadAsStringAsync());

            Assert.DoesNotContain(array, x => x.Id == _todo.Id);
        }

        [Fact]
        public async Task Get_Not_Accept_Default_Id()
        {
            var result = await _client.GetAsync(_root + default(Guid));

            Assert.True(!result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Delete_Accept_Default_Id()
        {
            var result = await _client.DeleteAsync(_root + default(Guid));

            result.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Put_Not_Accept_Default_Id()
        {
            var result = await _client.PutAsync(_root + default(Guid), CreateContent(_todo));

            Assert.True(!result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task MakeComplete_Not_Accept_Default_Id()
        {
            var result = await _client.PostAsync(_root + default(Guid) + "/MakeComplete", new StringContent(""));

            Assert.True(!result.IsSuccessStatusCode);
        }

        public void Dispose()
        {
            _client.Dispose();
            _server.Dispose();
        }
    }
}
