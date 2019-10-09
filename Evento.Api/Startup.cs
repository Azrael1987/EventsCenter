using Evento.Core.Repositories;
using Evento.Core.Repository;
using Evento.Ifrastructure.Repositories;
using Evento.Infrastructure.Handlers;
using Evento.Infrastructure.Mappers;
using Evento.Infrastructure.Services;
using Evento.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace Evento.Api
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
           /// dfs
           /// services.AddAuthorization(); // można rozbudować o role lub polityki bezpieczeństwa
            /*
             services.AddAuthorization(options => 
            {
	         options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
		        .RequireAuthenticatedUser()
		        .Build();
             });
             */
             //dfs
            //services.AddIdentity<IdentityUser, IdentityRole>(
            //    option =>
            //    {
            //        option.Password.RequireDigit = true;
            //        option.Password.RequiredLength = 8;
            //        option.Password.RequireNonAlphanumeric = false;
            //        option.Password.RequireLowercase = true;
            //        option.Password.RequireUppercase = true;
            //        option.Password.RequiredUniqueChars = 0;
            //    }
            //    ).AddDefaultTokenProviders();

          
            //  services.AddAuthentication(IISDefaults.AuthenticationScheme);
            services.AddCors(); // dev z sidney
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.Configure<JwtSettings>(Configuration.GetSection("Jwt"));
            //  services.Configure<JwtSettings>(Configuration.GetSection("Jwt"));

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
                      //  IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Jwt:Key"])), //nie UTF8

                    // d f s
                    //RequireExpirationTime = true,
                    // ValidAudience = Configuration["Jwt:Issuer"],
                    // ValidIssuer = Configuration["Jwt:Issuer"],
                    // ValidateLifetime = true
                };
              //  o.Audience = Configuration["Jwt:Issuer"];
              //  o.Authority = Configuration["Jwt:Issuer"];
            });


            services.AddScoped<IUserService, UserService>();
            services.AddMvc().AddJsonOptions(x => x.SerializerSettings.Formatting = Formatting.Indented);
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton(AutoMapperConfig.Initialize());
            services.AddSingleton<IJwtHandler, JwtHandler>();
            // services.UseJwt
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //tylko gdzie tego uzyć
            var jwtSettings = app.ApplicationServices.GetServices<IOptions<JwtSettings>>();

            //  przestarzałe
            //app.UseJwtBearerAuthentication(new JwtBearerOptions
            //{
            //   AumaticAuthenticate = true,
            //TokenValidationParameters = new TokenValidationParameters
            //{
            //    ValidateIssuer = jwtSettings.Value.Issuser,
            //    ValidateAudience = false,
            //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.Value.Key))
            //}
            //});

            //dfs
            app.UseCors(c => c
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

            app.UseHttpsRedirection();


            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
