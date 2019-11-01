using Autofac;
using Autofac.Extensions.DependencyInjection;
using Evento.Core.Repositories;
using Evento.Ifrastructure.Repositories;
using Evento.Infrastructure.Handlers;
using Evento.Infrastructure.Mappers;
using Evento.Infrastructure.Services;
using Evento.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using NLog.Web;
using System;
using System.Text;
using Evento.Api.Framework;

namespace Evento.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IContainer Container { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorization(a => a.AddPolicy("HasAdminRole", p => p.RequireRole("admin")));

            //  services.AddAuthentication(IISDefaults.AuthenticationScheme);
            services.AddCors(); // dev z Sidney
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.Configure<JwtSettings>(Configuration.GetSection("Jwt"));
            services.Configure<AppSettings>(Configuration.GetSection("App"));
       //     services.Configure<AppSettings>(Configuration.GetSection("app"));

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //  o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false, // true,
                    ValidateAudience = false, // true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Jwt:Key"])), //nie UTF8

                    // d f s
                    //RequireExpirationTime = true,
                    // ValidAudience = Configuration["Jwt:Issuer"],
                    // ValidIssuer = Configuration["Jwt:Issuer"],
                    // ValidateLifetime = true
                };
                //  o.Audience = Configuration["Jwt:Issuer"];
                //  o.Authority = Configuration["Jwt:Issuer"];
            });
            services.AddMvc().AddJsonOptions(x => x.SerializerSettings.Formatting = Formatting.Indented);
            services.AddSingleton(AutoMapperConfig.Initialize());
            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
                logging.AddNLog();

            });
            services.AddMemoryCache();
            services.AddScoped<IDateInitializer, DateInitializer>();

            // IoC - autofac
            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterType<EventRepository>().As<IEventRepository>().InstancePerLifetimeScope();
            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();
            builder.RegisterType<EventService>().As<IEventService>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<TicketService>().As<ITicketService>().InstancePerLifetimeScope();
            builder.RegisterType<JwtHandler>().As<IJwtHandler>().SingleInstance();
 
            // poczytać o tym
            // builder.RegisterAssemblyTypes();

            Container = builder.Build();

            return new AutofacServiceProvider(Container);
        }

        // IoC - ninject, stractureMap, autofac

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

                app.UseExceptionHandler();
            }
            //dfs
            app.UseCors(c => c
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

            //  app.AddWebLog();
            env.ConfigureNLog("nlog.config");

            app.UseHttpsRedirection();

            SeedDate(app);
            app.UseErrorHandler();
            app.UseAuthentication();
            app.UseMvc();
            applicationLifetime.ApplicationStopped.Register(() => Container.Dispose());

        }
        private void SeedDate(Microsoft.AspNetCore.Builder.IApplicationBuilder appBuilder)
        {
            var settings = appBuilder.ApplicationServices.GetService<IOptions<AppSettings>>();
            if (settings.Value.SeedDate)
            {
                var dateInitializer = appBuilder.ApplicationServices.GetService<IDateInitializer>();
                dateInitializer.SeedAsync();
            }
        }
    }
}
