using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Presentation.Api.Token;
using Core.Extensions;
using Data.Models.Identity;
using Data.Models.Mapping;
using AutoMapper;
using Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Core.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Net;
using Services;
using Providers.File;
using Providers.Sms;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.SignalR;
using Presentation.Api.Providers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json.Serialization;
using Providers.Queue;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Localization;
using System.Reflection;
using System.IO;
using Presentation.Api.Controllers;
using Providers.Mail;

namespace Presentation.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }
        private readonly TokenValidationParameters _tokenValidationParameters;

        private readonly TokenProviderOptions _tokenProviderOptions;
        private readonly ProjectConfiguration _projectConfiguration;
        private readonly IList<CultureInfo> supportedCultures;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            var signingKey = new SymmetricSecurityKey(
             Encoding.ASCII.GetBytes(Configuration.GetSection("TokenAuthentication:SecretKey").Value));

            _projectConfiguration = new ProjectConfiguration(configuration, env.EnvironmentName, env.IsEnvironment("localhost"), env.IsDevelopment(),
                env.IsStaging(), env.IsProduction(), Configuration.GetValue("UseLocalDb", false));

            _tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = Configuration.GetSection("TokenAuthentication:Issuer").Value,
                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = Configuration.GetSection("TokenAuthentication:Audience").Value,
                // Validate the token expiry
                ValidateLifetime = true,
                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero,
                TokenDecryptionKey = signingKey,
            };


            _tokenProviderOptions = new TokenProviderOptions
            {
                Path = Configuration.GetSection("TokenAuthentication:TokenPath").Value,
                Audience = Configuration.GetSection("TokenAuthentication:Audience").Value,
                Issuer = Configuration.GetSection("TokenAuthentication:Issuer").Value,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                //IdentityResolver = GetIdentity
                IsOtpTokenPathEnabled = false
            };

            supportedCultures = new List<CultureInfo>()
            {
                new CultureInfo("en-US"),
                new CultureInfo("tr-TR")
            };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowedOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
            #endregion

            #region Database
            if (_projectConfiguration.IsLocalhost)
            {
                _ = _projectConfiguration.UseLocalDb ?
                    services.AddDbContext<DataContext>(options => options.UseSqlServer(_projectConfiguration.LocalStorageConnection, opt => opt.UseNetTopologySuite())) :
                    services.AddDbContext<DataContext>(options => options.UseSqlite("Data Source=database.db", opt => opt.UseNetTopologySuite()));
            }
            else
            {
                services.AddDbContext<DataContext>(options => options.UseSqlServer(_projectConfiguration.DefaultConnection, opt => opt.UseNetTopologySuite()));
            }
            #endregion

            #region Identity
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 1;
            });

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });
            #endregion

            #region IServices
            services.AddAutoMapper(typeof(MapperProfile));
            services.AddSingleton(_projectConfiguration);
            services.AddTransient(typeof(JwtHelper));

            services.AddScoped<IFileProvider>(_ => new AzureFileProvider(_projectConfiguration.StorageConnection));
            services.AddScoped<IQueueProvider>(_ => new AzureServiceBusQueueProvider(_projectConfiguration.AzureWebJobsServiceBus));
            services.AddSingleton(_tokenProviderOptions);
            services.AddTransient<UserService, UserService>();
            services.AddTransient<KeyValuePairService, KeyValuePairService>();
            services.AddTransient<ISmsProvider, SlackSmsProvider>();
            services.AddTransient<SmsService, SmsService>();
            services.AddTransient<IMailProvider, SendGridMailProvider>(_ =>
                new SendGridMailProvider(_projectConfiguration.SendGridApiKey, "atacan@ovidos.com"));
            services.AddTransient<IUserAgreementService, UserAgreementService>();
            services.AddTransient<IAboutUsService, AboutUsService>();
            services.AddTransient<FAQService, FAQService>();
            services.AddTransient<PrivacyPolicyService, PrivacyPolicyService>();
            #endregion

            #region Authentication and Exception Handling
            services.AddAuthentication(cfg =>
              {
                  cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                  cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
              })
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = _tokenValidationParameters;

                 options.Events = new JwtBearerEvents
                 {
                     OnChallenge = (context) =>
                     {
                         var localizer = context.HttpContext.RequestServices.GetService<IStringLocalizer<Exceptions>>();
                         context.HandleResponse();
                         var response = Data.Models.Mapping.Wrappers.Response.Create(new UserUnauthorizedException());
                         response.Message = localizer["UserUnauthorizedException"].Value;
                         context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                         context.Response.WriteAsync(response.ToString()).Wait();
                         return Task.CompletedTask;
                     },
                     OnAuthenticationFailed = (context) =>
                     {
                         //TODO: We have to check this part.
                         return Task.CompletedTask;
                     },

                     //SIGNALR GETS THE ACCESSTOKEN
                     OnMessageReceived = (context) =>
                            {
                                var accessToken = context.Request.Query["access_token"];

                                // If the request is for our hub...
                                var path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(accessToken) &&
                                                       (path.StartsWithSegments("/hubs")))
                                {
                                    // Read the token out of the query string
                                    context.Token = accessToken;
                                }
                                return Task.CompletedTask;
                            }
                 };

             });

            #endregion

            #region MVC
            services.AddResponseCaching();
            services.AddMvc(options => options.EnableEndpointRouting = false)
               .AddNewtonsoftJson(options =>
               {
                   options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver()
                   {
                       IgnoreSerializableAttribute = false,
                       NamingStrategy = new CamelCaseNamingStrategy()
                       {
                           OverrideSpecifiedNames = true
                       }
                   };
                   IsoDateTimeConverter dateConverter = new IsoDateTimeConverter
                   {
                       DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz"
                   };
                   options.SerializerSettings.Converters.Add(dateConverter);
                   options.SerializerSettings.Converters.Add(new StringEnumConverter());
               }).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                // Add support for localizing strings in data annotations (e.g. validation messages) via the
                // IStringLocalizer abstractions.
                .AddDataAnnotationsLocalization();

            services.AddControllersWithViews();
            services.AddControllers();
            services.AddSingleton<IUserIdProvider, UserIdProvider>();
            #endregion

            #region Localization
            services.AddLocalization(t =>
           {
               t.ResourcesPath = "Resources";
           });

            services.Configure<RequestLocalizationOptions>(options =>
          {


              // State what the default culture for your application is. This will be used if no specific culture
              // can be determined for a given request.
              options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");

              // You must explicitly state which cultures your application supports.
              // These are the cultures the app supports for formatting numbers, dates, etc.
              options.SupportedCultures = supportedCultures;

              // These are the cultures the app supports for UI strings, i.e. we have localized resources for.
              options.SupportedUICultures = supportedCultures;

              // You can change which providers are configured to determine the culture for requests, or even add a custom
              // provider with your own logic. The providers will be asked in order to provide a culture for each request,
              // and the first to provide a non-null result that is in the configured supported cultures list will be used.
              // By default, the following built-in providers are configured:
              // - QueryStringRequestCultureProvider, sets culture via "culture" and "ui-culture" query string values, useful for testing
              // - CookieRequestCultureProvider, sets culture via "ASPNET_CULTURE" cookie
              // - AcceptLanguageHeaderRequestCultureProvider, sets culture via the "Accept-Language" request header
              options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
              {
                  foreach (var lang in context.Request.GetTypedHeaders().AcceptLanguage)
                  {
                      var globalLang = lang.Value.Value.Substring(0, 2);
                      var ui = supportedCultures.FirstOrDefault(t => t.TwoLetterISOLanguageName.StartsWith(globalLang));
                      if (ui != null)
                          return Task.FromResult(new ProviderCultureResult(ui.Name, ui.Name));
                  }
                  return Task.FromResult(new ProviderCultureResult("en-US", "en-US"));
              }));
          });
            #endregion

            #region SignalR
            services.AddSignalR(hubOptions =>
            {

                hubOptions.EnableDetailedErrors = !_projectConfiguration.IsProduction;
            }).AddNewtonsoftJsonProtocol(options =>
            {
                options.PayloadSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver()
                {
                    IgnoreSerializableAttribute = false,
                    NamingStrategy = new CamelCaseNamingStrategy()
                    {
                        OverrideSpecifiedNames = true
                    },

                };
                options.PayloadSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.PayloadSerializerSettings.Formatting = Formatting.None;

                IsoDateTimeConverter dateConverter = new IsoDateTimeConverter
                {
                    DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz"
                };
                options.PayloadSerializerSettings.Converters.Add(dateConverter);


            });
            #endregion

            #region Swagger
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Api Documentation"
                    });


                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Name = HttpRequestHeader.Authorization.ToString(),
                        Type = SecuritySchemeType.Http,
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Scheme = "Bearer".ToLowerInvariant(),
                        BearerFormat = "JWT"
                    });
                    var openApiReference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" };
                    var openApiSecurityScheme = new OpenApiSecurityScheme { Reference = openApiReference };
                    var securityRequirement = new OpenApiSecurityRequirement { { openApiSecurityScheme, new string[0] } };

                    c.AddSecurityRequirement(securityRequirement);

                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath, true);
                });
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<User> userManager,
RoleManager<Role> roleManager)
        {

            #region Database
            if (_projectConfiguration.IsLocalhost)
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<DataContext>();
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                    IdentityDataInitializer.SeedData(userManager, roleManager);
                }
            }
            else
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<DataContext>();
                    context.Database.Migrate();
                    IdentityDataInitializer.SeedData(userManager, roleManager);
                }
            }
            #endregion

            //_projectConfiguration = new ProjectConfiguration(Configuration, );
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            app.UseAuthentication();
            app.UseExceptionMiddleware();
            app.UseCors("AllowedOrigins");
            app.UseStaticFiles();
            //app.UseHttpsRedirection();
            app.UseTokenProvider(_tokenProviderOptions);
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(routes =>
            {
                routes.MapControllers();
                routes.MapControllerRoute(name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRazorPages();

            });

            #region Swagger
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            _ = app.UseMvc();
            #endregion
        }
    }
}
