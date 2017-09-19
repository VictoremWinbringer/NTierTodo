using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NTierTodo.Bll.Abstract;
using NTierTodo.Bll.Concrete;
using NTierTodo.Dal.Abstract;
using NTierTodo.Dal.Concrete;
using NTierTodo.SignalR;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using NTierTodo.Bll.Dto;
using NTierTodo.Bll.Validator;
using NTierTodo.Dal.Entities;

namespace NTierTodo
{

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
            services.AddMvc().AddFluentValidation();
            services.AddTransient<IToDoRepository>(s => new ToDoRepository("my.db"));
            services.AddSingleton<IMapper>(s => CreateMapper());
            services.AddTransient<IToDoManager, ToDoManager>();
            services.AddSignalR();
            services.AddTransient<IValidator<ToDoDto>, TodoValidator>();
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

        private IMapper CreateMapper()
        {
            return new MapperConfiguration(c =>
                {
                    c.CreateMap<ToDo, ToDoDto>();
                    c.CreateMap<ToDoDto, ToDo>();
                })
            .CreateMapper();
        }
    }
}
