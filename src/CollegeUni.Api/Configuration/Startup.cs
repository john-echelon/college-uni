using AutoMapper;
using CollegeUni.Api.Utilities.Extensions;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using NLog.Web;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;
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
        readonly Container _container = new Container();
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
            // The ServiceLifetime.Scoped will insure that the context will have a scoped lifetime instead of a transient life time.
            services.AddDbContext<AuthContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);
            // More things
            IntegrateSimpleInjector(services);

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
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(130);
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
                options.Filters.Add(typeof(ValidateModelActionFilter)); // by type
            });
            #endregion

            #region Add application services
            IntegrateSimpleInjector(services);
            #endregion

            #region Add Cors Policies here
            services.AddCors(
                options => options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials()
            ));
            #endregion

            #region AutoMapper
            Mapper.Initialize(cfg => {
                cfg.CreateMap<CourseInsertCommand, CourseRequest>().ReverseMap();
                cfg.CreateMap<CourseUpdateCommand, CourseRequest>().ReverseMap();
                cfg.CreateMap<CourseInsertCommand, Course>().ReverseMap();
                cfg.CreateMap<Course, CourseRequest>().ReverseMap();
                cfg.CreateMap<Course, CourseResponse>().ReverseMap();
            });
            #endregion

        }

        private void IntegrateSimpleInjector(IServiceCollection services)
        {
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            _container.Options.AllowOverridingRegistrations = true;

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IControllerActivator>(
                new SimpleInjectorControllerActivator(_container));
            services.AddSingleton<IViewComponentActivator>(
                new SimpleInjectorViewComponentActivator(_container));
            services.EnableSimpleInjectorCrossWiring(_container);
            services.UseSimpleInjectorAspNetRequestScoping(_container);
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
                    c.SwaggerEndpoint(endpoint, "Trident API v1");
                });
            }
            ConfigureNLog(app, env, loggerFactory);
            app.UseExceptionHandler(AppMiddlewareExceptionFilter.JsonHandler());

            InitializeContainer(app);
            _container.Verify();
            app.UseMvc();
        }

        private void InitializeContainer(IApplicationBuilder app)
        {
            /*
            AsyncScopedLifestyle will scope the container to the async call and all subsequent/ child Tasks will run using this container.
            This means that when you use a controller with async Task<IActionResult> the container will be scoped to this call.
            If you make a call to 2 different controllers, a new container will be created for each.
            */
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            // Add application presentation components:
            _container.RegisterMvcControllers(app);
            _container.RegisterMvcViewComponents(app);

            // Add application services. For instance:
            //_container.Register(app.ApplicationServices.GetService<AuthContext>, Lifestyle.Scoped);
            //_container.Register<AuthContext>(() => app.GetRequiredRequestService<AuthContext>(), Lifestyle.Scoped);
            _container.Register<IAuthService, AuthService>(Lifestyle.Transient);
            _container.Register<ITokenManager, TokenManager>(Lifestyle.Transient);
            _container.Register<ICourseService, CourseService>(Lifestyle.Transient);
            _container.Register<IUnitOfWork, UnitOfWork>(Lifestyle.Scoped);
            
            // Batch registration of open generic IGenericRepo<T>.
            _container.Register(typeof(IGenericRepo<>), typeof(GenericRepo<>), Lifestyle.Scoped);
            
            // Batch registration of closed generic of IValidator<T> to a collection.
            var validatorAssemblies = new[] { typeof(IValidator<>).Assembly };
            _container.RegisterCollection(typeof(IValidator<>), validatorAssemblies);
            // Maps the open generic IValidator<T> interface to the open generic CompositeValidator<T> implementation.
            _container.Register(typeof(IValidator<>), typeof(CompositeValidator<>), Lifestyle.Singleton);

            // Go look in all assemblies and register all implementations of ICommandHandler<T> by their closed interface:
            _container.Register(typeof(ICommandHandler<>), AppDomain.CurrentDomain.GetAssemblies());
            _container.Register(typeof(ICommandHandler<,>), AppDomain.CurrentDomain.GetAssemblies());

            // Decorate each returned ICommandHandler<T> object with a CommandHandlerDecorator<T>.
            _container.RegisterDecorator(typeof(ICommandHandler<,>), typeof(OptimisticConcurrencyCommandHandlerDecorator<,>),
                context => typeof(IResolveable).IsAssignableFrom(context.ServiceType.GetGenericArguments()[0]));
            _container.RegisterDecorator(typeof(ICommandHandler<,>), typeof(NonOptimisticConcurrencyCommandHandlerDecorator<,>),
                context => !typeof(IResolveable).IsAssignableFrom(context.ServiceType.GetGenericArguments()[0]));
            _container.RegisterDecorator(typeof(ICommandHandler<,>), typeof(TransactionCommandHandlerDecorator<,>));
            // Batch registration of closed generic IQueryHandler<,>.
            _container.Register(typeof(IQueryHandler<,>), new[] { typeof(IQueryHandler<,>).Assembly }) ;

            _container.Register<IQueryProcessor, QueryProcessor>(Lifestyle.Singleton);
            // Cross-wire ASP.NET services (if any). For instance:
            _container.CrossWire<ILoggerFactory>(app);
            _container.CrossWire<UserManager<ApplicationUser>>(app);
            _container.CrossWire<SignInManager<ApplicationUser>>(app);
            _container.CrossWire<IConfiguration>(app);
            _container.CrossWire<AuthContext>(app);

            // NOTE: Do prevent cross-wired instances as much as possible.
            // See: https://simpleinjector.org/blog/2016/07/
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
