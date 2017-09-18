using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NTierTodo;
using NTierTodo.Bll.Dto;

namespace TodoControllerShould
{
    public class TodoControllerShould
    {
        private HttpClient _client;
        private ToDoDto _todo;
        private string _root;
        public TodoControllerShould()
        {
            _client = GetClient();

            _todo = new ToDoDto
            {
                Description = "Bla bla bla"
            };

            _root = "api/v1/todo/";
        }
        [Fact]
        public async Task Crud()
        {
            await Create();
            await ReadAll();
            await Read();
        }

        private async Task Create()
        {
            var result = await _client.PostAsync(_root,
                new StringContent(JsonConvert.SerializeObject(_root), Encoding.UTF8, "application/json"));

            result.EnsureSuccessStatusCode();

            var newTodo = JsonConvert.DeserializeObject<ToDoDto>(await result.Content.ReadAsStringAsync());

            Assert.NotNull(newTodo);
            Assert.NotEqual(default(Guid), newTodo.Id);
            Assert.Equal(_todo.Description, newTodo.Description);
            Assert.Equal(false, newTodo.IsComplite);

            _todo.Id = newTodo.Id;
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
            var response =await _client.GetAsync(_root + _todo.Id);
            response.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<ToDoDto>(await response.Content.ReadAsStringAsync());

            Assert.NotEqual(_todo.Id, result.Id);
            Assert.Equal(_todo.Description, result.Description);
            Assert.Equal(_todo.IsComplite, result.IsComplite);
        }

        private async Task Update()
        {
            
        }

        protected HttpClient GetClient()
        {
            var builder = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseEnvironment("Testing");

            var server = new TestServer(builder);
            var client = server.CreateClient();

            // client always expects json results
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}
