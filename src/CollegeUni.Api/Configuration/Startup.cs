using AutoMapper;
using CollegeUni.Data.Entities;
using CollegeUni.Data.EntityFrameworkCore;
using CollegeUni.Filters;
using CollegeUni.Services.Managers;
using CollegeUni.Services.Models;
using CollegeUni.Services.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using NLog.Web;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollegeUni.Api.Configuration
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
            #region Add Framework Services
            // CORS section
            services.AddCors();

            // EF Section
            services.AddDbContext<AuthContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Identity Section
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AuthContext>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            // JWT Section
            string domain = $"https://{Configuration["Auth0:Domain"]}/";
            string apiIdentifier = Configuration["Auth0:ApiIdentifier"];
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiIdentifier)),
                    ValidAudience = domain,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = domain,
                };
            });

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "College Uni API",
                    Description = "A simple API for school course administration."
                });
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "CollegeUni.Api.xml");
                c.IncludeXmlComments(xmlPath);
                c.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
            });
            services.AddMvc(options => {
                options.Filters.Add(typeof(ApiExceptionFilterAttribute)); // by type
            });
            #endregion

            #region Add application services
            services.AddTransient<ITokenManager, TokenManager>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<ICourseService, CourseService>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IGenericRepo<Course>, GenericRepo<Course>>();
            services.AddTransient<IGenericRepo<Student>, GenericRepo<Student>>();
            services.AddTransient<IGenericRepo<Enrollment>, GenericRepo<Enrollment>>();
            #endregion
            
            #region Add Cors Policies here
            services.AddCors(
                options => options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials()
            ));
            #endregion

            #region AutoMapper
            Mapper.Initialize(cfg => {
                cfg.CreateMap<Course, CourseRequest>().ReverseMap();
                cfg.CreateMap<Course, CourseResponse>().ReverseMap();
            });
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<AuthContext>();
                    context.Database.Migrate();
                }
            }

            // Shows UseCors with CorsPolicyBuilder.
            app.UseCors("AllowAllOrigins");

            app.UseAuthentication();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            var endpoint = Configuration["Swagger:Endpoint"];
            if (!string.IsNullOrEmpty(endpoint))
            {
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint(endpoint, "college-uni API v1");
                });
            }
            ConfigureNLog(app, env, loggerFactory);
            app.UseExceptionHandler(AppMiddlewareExceptionFilter.JsonHandler());
            app.UseMvc();
        }

        protected virtual void ConfigureNLog(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //add NLog to ASP.NET Core
            env.ConfigureNLog("nlog.config");
            //add NLog.Web
            loggerFactory.AddNLog();
            app.AddNLogWeb();
        }

    }

    public class AuthorizationHeaderParameterOperationFilter: IOperationFilter
    {
        private readonly IOptions<AuthorizationOptions> authorizationOptions;

        public AuthorizationHeaderParameterOperationFilter(IOptions<AuthorizationOptions> authorizationOptions)
        {
            this.authorizationOptions = authorizationOptions;
        }
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is AuthorizeFilter);
            var allowAnonymous = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is IAllowAnonymousFilter);
            if (isAuthorized && !allowAnonymous)
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<IParameter>();
                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "Authorization",
                    In = "header",
                    Description = "access token",
                    Required = false,
                    Type = "http"
                });
            }
        }
    }
}
