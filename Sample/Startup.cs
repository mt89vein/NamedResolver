using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NamedResolver.Abstractions;
using NamedResolver.Extensions;

namespace NamedRegistration
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<DependentClass>()
                    .AddNamed<ITest>()
                    .Add<T2>("333")
                    .Add<T2>()
                    .Add<T1>("123");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            {
                using var scope = app.ApplicationServices.CreateScope();

                var resolver = scope.ServiceProvider.GetService<INamedResolver<ITest>>();
                var instance1 = resolver.Get("123");
                var instance11 = resolver.Get("123");
                var instance2 = resolver.Get("333");
                var instance3 = resolver.Get("444");
                var instance4 = resolver.Get();

                var all = resolver.GetAll();
            }

            {
                using var scope = app.ApplicationServices.CreateScope();

                var resolver = scope.ServiceProvider.GetService<INamedResolver<ITest>>();
                var instance1 = resolver.Get("123");
                var instance11 = resolver.Get("123");
                var instance2 = resolver.Get("333");
                var instance3 = resolver.Get("444");
                var instance4 = resolver.Get();

                var all = resolver.GetAll();
            }

            {
                using var scope = app.ApplicationServices.CreateScope();

                var instance = scope.ServiceProvider.GetRequiredService<ITest>();

                var resolver = scope.ServiceProvider.GetService<INamedResolver<ITest>>();
                var instance1 = resolver.Get("123");
                var instance11 = resolver.Get("123");
                var instance2 = resolver.Get("333");
                var instance3 = resolver.Get("444");
                var instance4 = resolver.Get();

                var r = resolver["444"];
                var instance112 = resolver["123"];

                var all = resolver.GetAll();
            }


            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
