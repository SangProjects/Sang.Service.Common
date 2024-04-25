using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sang.Service.Common.Authentication;
using Sang.Service.Common.CommonService;
using Sang.Service.Common.Controller;
using Sang.Service.Common.Repositories;
using Sang.Service.Common.Services;
using Sang.Service.Common.Validators;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace Sang.Service.Common.Extension
{
    public class ApiExtension
    {
        private const string SwaggerNonProdKey = "non-prod";
        private const string _policyName = "CorsPolicy";

        public static void Configure
            (IHostBuilder builder,
            IApiSettings settings,
            Action<AuthorizationOptions> configureAuth = null,
            Action<IApplicationBuilder> configureApp = null)
        {
            builder.ConfigureWebHostDefaults(Configure =>
            {
                var webhostBuilder = Configure.ConfigureAppConfiguration((hostingContext, builder) =>
                {
                    builder.BuildCommonConfiguration(settings);
                });

                webhostBuilder.ConfigureServices((hostingContext, services) =>
                {
                    services.AddTransient(_ => settings);
                    services.AddTransient(typeof(ValidationMiddleware<>));
                    services.AddTransient<IValidator<IFormFile>, FileUploadValidator>();
                    var mvcBuilder = services
                       .AddRouting(options => options.LowercaseUrls = true)
                       .AddMvcCore()
                       .AddAuthorization()
                       .AddApiExplorer();

                    var xmlFilenames = new List<string>();

                    AppDomain.CurrentDomain
                        .GetAssemblies()
                        .Where(assembly => assembly
                            .GetTypes()
                            .Any(type => type.IsAssignableTo(typeof(ISangApiController))))
                        .ToList()
                        .ForEach(assembly =>
                        {
                            // Make sure controllers are enumerated for this assembly
                            mvcBuilder.AddApplicationPart(assembly);

                            // Get the XML comments file names for this assembly if they exist
                            var xmlFilename = assembly.GetName().Name + ".xml";
                            var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

                            if (File.Exists(xmlFilePath))
                            {
                                xmlFilenames.Add(xmlFilePath);
                            }
                        });
                    services
                        .AddCors(options =>
                        {
                            options.AddPolicy(_policyName,
                              builder => builder
                             .AllowAnyOrigin()
                             .AllowAnyMethod()
                             .AllowAnyHeader()
                             .SetIsOriginAllowed(origin => true)
                             .WithExposedHeaders("X-Token-Expired")
                             );
                        })
                        .AddControllers()                        
                        .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

                        //.AddNewtonsoftJson(options =>
                        //{
                        //    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                        //    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                        //});

                    // Add Swagger Document generation
                    services.AddSwaggerGen(options =>
                    {
                        var securityScheme = new OpenApiSecurityScheme()
                        {
                            In = ParameterLocation.Header,
                            Description = "Role Based Permissions Authentication using Jwt Bearer Tokens",
                            Name = "Authorization",
                            Type = SecuritySchemeType.Http,
                            BearerFormat = "JWT",
                            Scheme = "bearer"
                        };

                        var securityRequirement = new OpenApiSecurityRequirement()
                        { {
                            new OpenApiSecurityScheme()
                            {
                                Reference = new OpenApiReference()
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] { }
                        } };

                        options.AddSecurityDefinition("Bearer", securityScheme);
                        options.AddSecurityRequirement(securityRequirement);

                        options.SwaggerDoc(settings.ApiVersionString, new OpenApiInfo
                        {
                            Title = settings.ApiName,
                            Version = settings.ApiVersionString
                        });

                        options.DocInclusionPredicate((docName, apiDescription) =>
                           {
                               if (!apiDescription.TryGetMethodInfo(out MethodInfo methodInfo))
                                   return false;

                               var isNonProdController = methodInfo.DeclaringType?.
                                                           GetCustomAttributes(typeof(DisableInProductionAttribute), true)
                                                           .Any() ?? false;
                               return !isNonProdController;
                           });

                        options.DescribeAllParametersInCamelCase();//  note here this needs to check
                        options.UseInlineDefinitionsForEnums();

                        if (!settings.IsProduction)
                        {
                            EnableNonProductionApis(options, settings);
                        }

                        xmlFilenames.ForEach(xmlFilename => options.IncludeXmlComments(xmlFilename, true));
                    });
                    services.AddAuthentication(options =>
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
                              IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SymmetricSecurityKey)),
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
                                      context.HttpContext.Response.Headers.Append("X-Token-Expired", "true");

                                  return Task.CompletedTask;
                              }
                          };
                      });

                    //Caches
                    services.AddHttpContextAccessor();
                    services.AddScoped<IRequestCache, MemoryRequestCache>();
                    services.AddMemoryCache();

                    services.AddScoped<IAuthenticationContext, AuthenticationContext>();
                    services.AddScoped<ICommonEntityService, CommonEntityService>();
                    services.AddScoped<IDefaultEntityService, DefaultEntityService>();
                    services.AddScoped<IDbTransactionService, DbTransactionService>();
                    services.AddScoped<IDefaultDbService, DefaultDbService>();
                    services.AddScoped<ICachePaginator, CachePaginator>();
                    services.AddScoped<ITokenService, TokenService>();
                    services.AddScoped<IDefaultDbRepository, DefaultDbRepository>();
                });

                webhostBuilder.Configure((hostingContext, app) =>
                {
                    // Standard configuration for Self host
                    //app.UseHttpsRedirection();
                    //app.UseStaticFiles();
                    //app.UseCookiePolicy();
                    app.UseAuthentication();


                    // OpenAPI Documentation
                    app.UseSwagger();
                    app.UseCors(_policyName);
                    app.UseSwaggerUI();
                    app.UseSwaggerUI(options =>
                    {
                        options.EnableTryItOutByDefault();
                        options.EnableFilter();
                        //options.EnablePersistAuthorization();
                        options.SwaggerEndpoint(settings.DocumentUrl, settings.ApiFullName);
                    });

                    // Controllers and Routes
                    app.UseRouting();
                    app.UseAuthorization();
                    app.UseEndpoints(endpoints => endpoints.MapControllers());

                    app.UseMiddleware<RequestResponseLoggerMiddleware>();
                    app.UseMiddleware<CorrelationIdMiddleware>();
                });
            });

        }

        private static void EnableNonProductionApis(SwaggerGenOptions options, IApiSettings settings)
        {
            options.SwaggerDoc(SwaggerNonProdKey, new OpenApiInfo
            {
                Title = "Non Production",
                Version = settings.ApiVersionString
            });

            options.DocInclusionPredicate((docName, apiDescription) =>
            {
                if (!apiDescription.TryGetMethodInfo(
                    out MethodInfo methodInfo))
                    return false;

                var isNonProdController = methodInfo.DeclaringType?
                    .GetCustomAttributes(true)
                    .OfType<DisableInProductionAttribute>()
                    .Any() ?? false;

                return (docName == SwaggerNonProdKey && isNonProdController)
                    || (docName != SwaggerNonProdKey && !isNonProdController);
            });
        }
    }
}
