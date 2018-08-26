using System.Linq;
using AutoMapper;
using BlogDemo.Api.Extensions;
using BlogDemo.Core.Interfaces;
using BlogDemo.Infrastructure.Database;
using BlogDemo.Infrastructure.Repositories;
using BlogDemo.Infrastructure.Resources;
using BlogDemo.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace BlogDemo.Api
{
    public class StartupDevelopment
    {
        private static IConfiguration Configuration { get; set; }

        public StartupDevelopment(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(
                options =>
                {
                    options.ReturnHttpNotAcceptable = true;
                    // options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());

                    var intputFormatter = options.InputFormatters.OfType<JsonInputFormatter>().FirstOrDefault();
                    if (intputFormatter != null)
                    {
                        intputFormatter.SupportedMediaTypes.Add("application/vnd.cgzl.post.create+json");
                        intputFormatter.SupportedMediaTypes.Add("application/vnd.cgzl.post.update+json");
                    }

                    var outputFormatter = options.OutputFormatters.OfType<JsonOutputFormatter>().FirstOrDefault();
                    if (outputFormatter != null)
                    {
                        outputFormatter.SupportedMediaTypes.Add("application/vnd.cgzl.hateoas+json");
                    }

                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddFluentValidation();

            services.AddDbContext<MyContext>(options =>
            {
                // var connectionString = Configuration["ConnectionStrings:DefaultConnection"];
                var connectionString = Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlite(connectionString);
            });

            services.AddHttpsRedirection(options =>
                {
                    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                    options.HttpsPort = 5001;
                });
            
            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "https://localhost:5001";
                    options.ApiName = "restapi";
                });


            services.AddTransient<IValidator<PostAddResource>, PostAddOrUpdateResourceValidator<PostAddResource>>();
            services.AddTransient<IValidator<PostUpdateResource>, PostAddOrUpdateResourceValidator<PostUpdateResource>>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddAutoMapper();

            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            var propertyMappingContainer = new PropertyMappingContainer();
            propertyMappingContainer.Register<PostPropertyMapping>();
            services.AddSingleton<IPropertyMappingContainer>(propertyMappingContainer);

            services.AddTransient<ITypeHelperService, TypeHelperService>();
            
            services.Configure<MvcOptions>(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseMyExceptionHandler(loggerFactory);
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
