using System.Linq;
using System.Security.Principal;
using BotRules.Data;
using BotRules.Filters;
using BotRules.Services;
using BotRules.Services.Implementation;
using BotRules.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WebAppCore.Services;

namespace BotRules
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<BotRulesContext>(options=> 
            options.UseSqlServer(Configuration.GetConnectionString("BotRulesContext")));

            services.AddMvc(options =>
            {
                // обязательно требование токена на странице

                options
                .Filters.
                Add(new AutoValidateAntiforgeryTokenAttribute());
                // Добавление медиа типа для csp репорта
                options
                .InputFormatters
                .Where(item => item.GetType() == typeof(JsonInputFormatter))
                .Cast<JsonInputFormatter>()
                .Single()
                .SupportedMediaTypes
                .Add("application/csp-report");
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Аутентификация на основе куки
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                // устанавливается относительный путь, по которому будет перенаправляться анонимный пользователь при доступе к ресурсам, для которых нужна аутентификация.
                options.LoginPath = new PathString("/Account/Login");
            });

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IBotService, BotService>();
            services.AddScoped<IEditorService, EditorService>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IPrincipal>(
                provider => provider.GetService<IHttpContextAccessor>().HttpContext.User);

            // Конфигурация адреса
            AppConfig.Ip = Configuration["WebManager:Ip"];
            AppConfig.Port = Configuration["WebManager:Port"];
            AppConfig.AppUrl = AppConfig.Ip + ":" + AppConfig.Port;

            // Концигурация менеджера почты
            EmailManager.EmailSender = Configuration["EmailManager:Email"];
            EmailManager.PasswordSender = Configuration["EmailManager:Password"];

            services.AddTransient<AccountDateFilter>();           
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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            #region Заголовки для безопасности

            // Content-Security-Policy 
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Security-Policy",
                    $"default-src 'self' https://ajax.googleapis.com https://ajax.aspnetcdn.com; base-uri 'self'; report-uri /event/cspreport");
                await next();
            });

            // xss protection
            app.Use(async (context, next) =>
            {
                var headers = context.Response.Headers;
                headers.Add("x-xss-protection", "1");
                headers.Add("X-Frame-Options", "SAMEORIGIN");
                headers.Add("X-Content-Type-Options", "nosniff");
                await next();
            });

            #endregion

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
