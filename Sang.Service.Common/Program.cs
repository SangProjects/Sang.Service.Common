using DemoCoreWebAPI.CommonService;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Sang.Service.Common.Authentication;
using Sang.Service.Common.CommonService;
using Sang.Service.Common.Models;
using Sang.Service.Common.Repositories;
using Sang.Service.Common.Services;
using Sang.Service.Common.Validators;
using Serilog;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        //var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        IConfigurationRoot configuration = new ConfigurationBuilder()
            //.SetBasePath(Directory.GetCurrentDirectory())
            //.AddJsonFile("appsettings.json")
            //.AddJsonFile($"appsettings.{environmentName}.json", true)
            .Build();

        var builder = WebApplication.CreateBuilder(args);
        var _policyName = "CorsPolicy";

        //ConfigurationManager configuration = builder.Configuration;
        var authentication = configuration.GetSection("AuthenticationSettings").Get<AuthenticationSettings>();
        var serviceOptions = configuration.GetSection("ServiceOptions").Get<ServiceOptions>();

        builder.AddCors(options =>
        {
            options.AddPolicy(_policyName,
                              builder => builder
                                  .AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader()
                                  .SetIsOriginAllowed(origin => true)
                                  .WithExposedHeaders(authentication.TokenExpiryHeader)
                              );
        });

        //NOTE:Logging to File
        var logger = new LoggerConfiguration()
                     .ReadFrom.Configuration(builder.Configuration)
                     .Enrich.FromLogContext()
                     .Enrich.WithCorrelationIdHeader("X-Correlation-Id")
                     .CreateLogger();
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);

        //NOTE: Handling Enum through Controller
        builder.AddControllers()
            .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

        builder.AddEndpointsApiExplorer();
        builder.AddSwaggerGen();

        //NOTE:Swagger XML Configuration 
        builder.AddSwaggerGen(c =>
        {
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
        });

        builder.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
       .AddJwtBearer(options =>
       {
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authentication.TokenKey)),
               ValidateIssuer = false,
               ValidateAudience = false,
               ValidateLifetime = true,
               ClockSkew = TimeSpan.Zero
           };
           options.Events = new JwtBearerEvents
           {
               OnAuthenticationFailed = context =>
               {
                   // Handle token expiration
                   if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                       context.HttpContext.Response.Headers.Append(authentication.TokenExpiryHeader, "true");

                   return Task.CompletedTask;
               }
           };
       });

        builder.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = serviceOptions?.ServiceName,
                Version = serviceOptions?.ServiceVersion
            });
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = authentication?.TokenDescription,
                Name = authentication?.TokenName,
                Type = SecuritySchemeType.Http,
                BearerFormat = authentication?.TokenFormat,
                Scheme = authentication?.TokenScheme
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                  {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = authentication?.TokenID
                            }
                        },
                        new string[]{}
                  }
            });
            opt.DocInclusionPredicate((docName, apiDescription) =>
            {
                if (!apiDescription.TryGetMethodInfo(out MethodInfo methodInfo))
                    return false;

                var isNonProdController = methodInfo.DeclaringType?.
                                            GetCustomAttributes(typeof(DisableInProductionAttribute), true)
                                            .Any() ?? false;
                return !isNonProdController;
            });
        });
        builder.AddHttpContextAccessor();

        //builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection("CacheSettings"));
        var databaseConfiguration = builder.Configuration.GetSection(nameof(DatabaseConfiguration));
        builder.Configure<DatabaseConfiguration>(databaseConfiguration);
        var authenticationSettings = builder.Configuration.GetSection(nameof(AuthenticationSettings));
        builder.Configure<AuthenticationSettings>(authenticationSettings);

        var fileUploadSettings = builder.Configuration.GetSection(nameof(FileUploadSettings));
        builder.Configure<FileUploadSettings>(fileUploadSettings);

        builder.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

        //Caches
        builder.AddScoped<IRequestCache, MemoryRequestCache>();
        builder.AddMemoryCache();

        //builder.Services.AddScoped<IAuthenticationContext, AuthenticationContext>();

        builder.AddScoped<ICommonEntityService, CommonEntityService>();
        builder.AddScoped<IDefaultEntityService, DefaultEntityService>();
        builder.AddScoped<IDbTransactionService, DbTransactionService>();
        builder.AddScoped<ITokenService, TokenService>();

        builder.AddScoped<ICachePaginator, CachePaginator>();
      

        
        builder.AddScoped<IDefaultDbRepository, DefaultDbRepository>();
       

        builder.AddTransient(typeof(ValidationMiddleware<>));       
        builder.AddTransient<IValidator<IFormFile>, FileUploadValidator>();

        var app = builder.Build();

        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors(_policyName);
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}