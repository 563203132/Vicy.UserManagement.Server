using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using Swashbuckle.Swagger.Model;
using Vicy.UserManagement.Server.Common;
using Vicy.UserManagement.Server.DataAccess.Configurations;
using System.Data.Entity;
using Newtonsoft.Json.Serialization;
using Vicy.UserManagement.Server.DataAccess.Write;
using Mehdime.Entity;
using Vicy.UserManagement.Server.Domain.Shared;
using Vicy.UserManagement.Server.DataAccess.Read;
using Vicy.UserManagement.Server.DataAccess.Write.Repositories;
using Vicy.UserManagement.Server.Domain;

namespace Vicy.UserManagement.Server.Api
{
    public class Startup
    {
        private const string ExceptionsOnStartup = "Startup";
        private const string ExceptionsOnConfigureServices = "ConfigureServices";
        private const int InternalServerError = 500;
        private const int BadRequest = 400;
        private const string ErrorResponseType = "application/json";
        private const string CorsResponseHeader = "Access-Control-Allow-Origin";

        // Captures exceptions occur on Startup and ConfigureServices 
        private readonly Dictionary<string, List<Exception>> _exceptions;

        public Startup(IHostingEnvironment env)
        {
            _exceptions = new Dictionary<string, List<Exception>>
            {
                {ExceptionsOnStartup, new List<Exception>()},
                {ExceptionsOnConfigureServices, new List<Exception>()}
            };

            try
            {
                var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

                if (env.IsEnvironment("Development"))
                {
                    // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                    builder.AddApplicationInsightsSettings(developerMode: true);
                }

                builder.AddEnvironmentVariables();
                Configuration = builder.Build();
            }
            catch (Exception ex)
            {
                _exceptions[ExceptionsOnStartup].Add(ex);
            }
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            AddDependencies(services);

            InjectDbContextScope(services);
            InjectRepositories(services);
            InjectDomainServices(services);
            InjectDomainFactories(services);

            services.AddSingleton<IDbContextFactory, DbContextFactory>();

            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc().AddJsonOptions(options =>
            {
                // use standard name conversion of properties
                options.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();

                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            });

            AddSwaggerGen(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Add NLog to the application
            loggerFactory.AddNLog();
            env.ConfigureNLog($"nlog.{env.EnvironmentName}.config");
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            InitializeDbConfiguration(loggerFactory);

            // Check if any errors occurred on the constructor or ConfigureServices
            var logger = loggerFactory.CreateLogger<Startup>();
            if (_exceptions.Any(p => p.Value.Any()))
            {
                app.Run(context => HandleStartupErrors(context, logger, _exceptions));
                return;
            }

            try
            {
                // Add Application Insights monitoring to the request pipeline as a very first middleware.
                app.UseApplicationInsightsRequestTelemetry();

                app.UseExceptionHandler(builder =>
                {
                    // Add exception handling middleware which hanles all runtime errors. 
                    builder.Run(HandleRuntimeErrors);
                });

                app.UseMvc();

                // Enable middleware to serve generated Swagger as a JSON endpoint
                app.UseSwagger();

                // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
                app.UseSwaggerUi();
            }
            catch (Exception ex)
            {
                app.Run(async context =>
                {
                    await WriteErrorResponseAsync(context, ex.GetBaseException().Message)
                        .ConfigureAwait(false);
                });

                app.UseApplicationInsightsExceptionTelemetry();
            }
        }

        private void InitializeDbConfiguration(ILoggerFactory loggerFactory)
        {
            var dbConnectionStrings = new DbConnectionStrings();
            Configuration.GetSection("ConnectionStrings").Bind(dbConnectionStrings);
            DbConfiguration.SetConfiguration(
                new VicyDbConfiguration(
                    dbConnectionStrings,
                    new PoorPerformingSqlLogger(loggerFactory, 200)));
        }

        /// <summary>
        /// Handles the errors occurred on the startup process(constructor & ConfigureServices).
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>. </param>
        /// <param name="logger">The logger.</param>
        /// <param name="exceptions">The exceptions.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        private static async Task HandleStartupErrors(
            HttpContext context,
            ILogger logger,
            Dictionary<string, List<Exception>> exceptions)
        {
            var errorMessages = new List<string>();
            foreach (var key in exceptions.Keys)
            {
                foreach (var ex in exceptions[key])
                {
                    var errorMessage = $"{key} - {ex.GetBaseException().Message}";
                    errorMessages.Add(errorMessage);
                    logger.LogError(errorMessage);
                }
            }

            await WriteErrorResponseAsync(
                context, string.Join(Environment.NewLine, errorMessages))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Handles the runtime errors.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>. </param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        private static async Task HandleRuntimeErrors(HttpContext context)
        {
            var efh = context.Features.Get<IExceptionHandlerFeature>();
            if (efh != null)
            {
                var errorCode = ErrorCode.SystemUnhandledException;
                object[] paras = null;
                if (efh.Error is VicyServiceException)
                {
                    var vicyServiceException = efh.Error as VicyServiceException;
                    errorCode = vicyServiceException.ErrorCode;
                    paras = vicyServiceException.Args;
                }

                await WriteErrorResponseAsync(context, errorCode, efh.Error.Message, paras)
               .ConfigureAwait(false);
            }
        }

        private static async Task WriteErrorResponseAsync(HttpContext context, string errorMessage)
        {
            context.Response.StatusCode = InternalServerError;
            context.Response.ContentType = ErrorResponseType;

            // TODO: Can we use the CORS middleware to do this?
            context.Response.Headers.Add(CorsResponseHeader, "*");

            var errorResponse = new ErrorResponse(InternalServerError, errorMessage);
            await context.Response
                .WriteAsync(JsonConvert.SerializeObject(errorResponse))
                .ConfigureAwait(false);
        }

        private static async Task WriteErrorResponseAsync(
            HttpContext context,
            string errorCode,
            string errorMessage,
            params object[] args)
        {
            int statusCode = errorCode == ErrorCode.SystemUnhandledException ? InternalServerError : BadRequest;

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = ErrorResponseType;

            // TODO: Can we use the CORS middleware to do this?
            context.Response.Headers.Add(CorsResponseHeader, "*");

            var error = new ErrorResponse(statusCode, errorCode, errorMessage, args);
            var errorResponse = JsonConvert.SerializeObject(error);
            await context.Response
                .WriteAsync(errorResponse)
                .ConfigureAwait(false);
        }

        private static void AddSwaggerGen(IServiceCollection services)
        {
            // Inject an implementation of ISwaggerProvider with defaulted settings applied
            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "Vicy User Management",
                    Description = "Api for managing user.",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "Vic Li", Email = "563203132@qq.com", Url = "https://github.com/563203132/Vicy.UserManagement.Server/tree/develop" },
                });

                //Determine base path for the application.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;

                //Set the comments path for the swagger json and ui.
                var xmlPath = Path.Combine(basePath, "Vicy.UserManagement.Server.Api.xml");
                options.IncludeXmlComments(xmlPath);
            });
        }

        private void AddDependencies(IServiceCollection services)
        {
            // Add options
            services.AddOptions();
            services.Configure<DbConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
        }

        private static void InjectDbContextScope(IServiceCollection services)
        {
            services.AddSingleton<IDbContextScopeFactory, DbContextScopeFactory>();
            services.AddSingleton<IDbContextScopeFactory, DbContextScopeFactory>();
            services.AddSingleton<IAmbientDbContextLocator, AmbientDbContextLocator>();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddScoped<IReadDbFacade, ReadDbFacade>();
        }

        private static void InjectRepositories(IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
        }

        private void InjectDomainServices(IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
        }

        private void InjectDomainFactories(IServiceCollection services)
        {
            services.AddTransient<IUserFactory, UserFactory>();
        }
    }
}
