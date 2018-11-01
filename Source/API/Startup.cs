using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

using AspNetCore.Sample.API.Filters;
using AspNetCore.Sample.Common;
using AspNetCore.Sample.Common.Configurations;
using AspNetCore.Sample.Common.Trace;
using AspNetCore.Sample.DataAccessor;
using AspNetCore.Sample.DataContract.Entities;
using AspNetCore.Sample.DataContract.Models;
using AspNetCore.Sample.Repository.Cosmos;
using AspNetCore.Sample.Repository.Interface;
using AspNetCore.Sample.Service.Implementation;
using AspNetCore.Sample.Service.Interface;

using AutoMapper;

using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Apex.Security;
using Microsoft.Azure.Apex.Security.Authentication;
using Microsoft.Azure.Apex.Security.Extensions;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Swashbuckle.AspNetCore.Swagger;

namespace AspNetCore.Sample.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = AddAzureKeyVaultToConfiguration(configuration);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(_configuration);

            var configProvider = services.BuildServiceProvider();
            AppSettings appSettings = configProvider.GetService<IOptions<AppSettings>>().Value;

            AddCustomServices(services, appSettings);

            AddCors(services, appSettings.AllowedCorsOrigins);
            AddApexSecurity(services);
            AddApplicationInsights(services);
            AddSwagger(services);

            services.AddMvc();
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
                app.UseMiddleware<GlobalDiagnosticsHandler>();
            }

            InitializeAutoMapper();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint(Constant.SwaggerEndpointUrl, Constant.SwaggerDescription));
            app.UseAuthentication();
            app.UseCors(Constant.DefaultCORSPolicyName);
            app.UseMvc();
        }

        private static IConfiguration AddAzureKeyVaultToConfiguration(IConfiguration configuration)
        {
            var configBuild = new ConfigurationBuilder().AddConfiguration(configuration);

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var defaultKeyVaultSecretManager = new DefaultKeyVaultSecretManager();

            configBuild.AddAzureKeyVault(configuration[Constant.KeyVaultUri], keyVaultClient, defaultKeyVaultSecretManager);

            return configBuild.Build();
        }

        private static void AddCors(IServiceCollection services, string[] allowedCorsOrigins)
        {
            // TODO: allow specific methods and headers.
            services.AddCors(options =>
            {
                options.AddPolicy(
                    Constant.DefaultCORSPolicyName,
                    builder => builder.WithOrigins(allowedCorsOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }

        private static void AddCustomServices(IServiceCollection services, AppSettings appSettings)
        {
            var documentClient = CosmosAccessor.CreateClient(appSettings.CosmosConnectionString);
            var databaseId = appSettings.CosmosDatabaseId;
            var sampleCollectionId = appSettings.CosmosSampleCollectionId;

            services.AddSingleton<ISampleService, SampleService>();
            services.AddSingleton<ISampleRepository>(new SampleRepository(documentClient, databaseId, sampleCollectionId));
        }

        private static void InitializeAutoMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<SampleEntity, GetSampleResponse>();
            });
        }

        private void AddApplicationInsights(IServiceCollection services)
        {
            services.AddSingleton<ITelemetryInitializer>(new RequestBodyTelemetryInitializer(new HttpContextAccessor()));
            services.AddApplicationInsightsTelemetryProcessor<RequestBodyTelemetryFilter>();
            services.AddApplicationInsightsTelemetryProcessor<UserTelemetryFilter>();

            var options = new ApplicationInsightsServiceOptions
            {
                EnableAdaptiveSampling = false,
                InstrumentationKey = _configuration["ApplicationInsights:InstrumentationKey"]
            };
            services.AddApplicationInsightsTelemetry(options);
        }

        private void AddApexSecurity(IServiceCollection services)
        {
            var cert = CryptoHelper.GetDefaultOptionsCertificate(_configuration[Constant.AuthCertName], StoreLocation.LocalMachine);
            services.AddApexSecurity(new TripleCrownAuthOptions
            {
                GetCertificate = () => cert,
                LogTrace = (message, requestHeaders) => Logger.TraceInfo(message, requestHeaders),
                LogException = (exception, requestHeaders) => Logger.TraceException(exception, requestHeaders)
            });
        }

        private void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(Constant.SwaggerVersion, new Info
                {
                    Title = Constant.SwaggerDescription,
                    Version = Constant.SwaggerVersion
                });

                var basePath = AppContext.BaseDirectory;
                var xmlName = GetType().GetTypeInfo().Module.Name.Replace(".dll", ".xml", StringComparison.OrdinalIgnoreCase).Replace(".exe", ".xml", StringComparison.OrdinalIgnoreCase);
                var xmlPath = Path.Combine(basePath, xmlName);
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition(Constant.SwaggerSchemeName, new ApiKeyScheme
                {
                    Description = "auth token generated by ApexSecurity",
                    Name = "DocsAuthClaims",
                    In = "cookie",
                    Type = "apiKey"
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { Constant.SwaggerSchemeName, Array.Empty<string>() } });
                c.DescribeAllEnumsAsStrings();
                c.DescribeStringEnumsInCamelCase();
            });
        }
    }
}
