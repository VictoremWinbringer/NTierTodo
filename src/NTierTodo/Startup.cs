using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NTierTodo.Bll.Abstract;
using NTierTodo.Bll.Concrete;
using NTierTodo.Dal.Abstract;
using NTierTodo.Dal.Concrete;
using NTierTodo.SignalR;

namespace NTierTodo
{
    using AutoMapper;

    using NTierTodo.Bll.Model;
    using NTierTodo.Dal.Entity;
    using NTierTodo.ViewModels;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddTransient<IToDoRepository>(s => new ToDoRepository("my.db"));
            services.AddTransient<IToDoManager, ToDoManager>();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSignalR(b => b.MapHub<Notifier>("hub"));
        }
    }
}
