using dp.api.Helpers;
using dp.business.Models;
using dp.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace dp.api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940


        public Startup(IConfiguration configuration)

        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            // needed to load configuration from appsettings.json
            services.AddOptions();

            services.AddCors();


            services.AddControllers();

            services.AddHealthChecks();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            var awsSettingsSection = Configuration.GetSection("AWS");
            services.Configure<AppSettings>(appSettingsSection);
            services.Configure<AwsSettings>(awsSettingsSection);
            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var awsSettings = awsSettingsSection.Get<AwsSettings>();

            //set ENV variables
            var connectionString = Configuration.GetConnectionString("dpDbConnectionString");

            string key1 = Configuration.GetValue<String>("Key1");

            Environment.SetEnvironmentVariable("dpDbConnectionString", connectionString);
            Environment.SetEnvironmentVariable("poolId", awsSettings.UserPoolId);
            Environment.SetEnvironmentVariable("clientId", awsSettings.UserPoolClientId);

            EmailConfig emailConfig = new EmailConfig();
            Configuration.GetSection("Email").Bind(emailConfig);
            services.Configure<EmailConfig>(Configuration.GetSection("Email"));


            //Used to sign in users or other info
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", awsSettings.AccessKey);
            Environment.SetEnvironmentVariable("AWS_SECRET_KEY", awsSettings.SecretKey);

            #region Bearer Authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(x =>
             {
                 x.SaveToken = true;
                 x.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
                     {
                             // Get JsonWebKeySet from AWS
                             var json = new WebClient().DownloadString(parameters.ValidIssuer + "/.well-known/jwks.json");
                             // Serialize the result
                             return JsonConvert.DeserializeObject<JsonWebKeySet>(json).Keys;
                     },
                     ValidIssuer = $"https://cognito-idp.{awsSettings.Region}.amazonaws.com/{awsSettings.UserPoolId}",
                     ValidateIssuer = true,
                     ValidateLifetime = true,
                     LifetimeValidator = (before, expires, token, param) => expires > DateTime.UtcNow,
                     ValidAudience = $"{awsSettings.UserPoolClientId}",
                     ValidateAudience = true
                 };
             });




            //Not sure if this is needed for a role yet, I think it automatically does it
            /*services.AddAuthorization(options =>
                      {
                          options.AddPolicy("Admin", policy => policy.RequireClaim("custom:groupId", new List<string> { "0" }));
                      });
            */

            #endregion


            //Add Services
            services.AddScoped<EmailService>(provider =>
            {
                return new EmailService(emailConfig);
            });

            // services.AddScoped<IUserService, UserService>();

            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DP API", Version = "V1" });
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "dp.api.xml");
                c.AddSecurityDefinition("[auth scheme: same name as defined for asp.net]", new OpenApiSecurityScheme()
                {
                    Name = "x-api-key",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Description = "Authorization by x-api-key inside request's header",
                    Scheme = "ApiKeyScheme"
                });
                var key = new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    },
                    In = ParameterLocation.Header
                };
                var requirement = new OpenApiSecurityRequirement
                    {
                       { key, new List<string>() }
                    };
                //c.AddSecurityRequirement(requirement);

                c.IncludeXmlComments(filePath);

                //use below to hide all swagger methods besides x controllers for production
                /*   if (isProduction)
                   {
                       c.DocInclusionPredicate((docName, apiDesc) =>
                       {

                           if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo))
                               return false;
                           if (methodInfo.DeclaringType == typeof(Controller1) || methodInfo.DeclaringType == typeof(Controller2))
                           {
                               return true;
                           }
                           return false;

                       });
                   }*/

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,

                            },
                            new List<string>()
                        }
                    });




            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseDefaultFiles();
            app.UseStaticFiles(); // to add a index.html
            app.UseRouting();

            app.UseCors(
          options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials().WithExposedHeaders("*")
      );

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHealthChecks("/health");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DP API V1");
            });

        }
    }
}
