using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using BaseCamp_Web_API.Api.Abstractions.Services;
using BaseCamp_Web_API.Api.Configuration;
using BaseCamp_Web_API.Api.Services;
using BaseCamp_WEB_API.Core.DbContextAbstractions;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Entities.Authorization.Requirements;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using BaseCamp_WEB_API.Data.Contexts;
using BaseCamp_WEB_API.Data.Repositories;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace BaseCamp_Web_API.Api
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
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BaseCamp_Web_API", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Authorization with JWT using Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth3",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            services.AddTransient<ISneakerRepository, SneakerRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserRoleWithPrivilegesRepository, UserRoleWithPrivilegesRepository>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IOrderSneakerRepository, OrderSneakerRepository>();
            services.AddTransient<IVendorRepository, VendorRepository>();
            services.AddTransient<ISeasonRepository, SeasonRepository>();
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IPolicyAuthorizationDbContext, PolicyAuthorizationContext>();

            var mySqlConnectionString = Configuration.GetConnectionString("MySQLServer");
            services.AddDbContext<PolicyAuthorizationContext>(options =>
            {
                options.UseMySql(mySqlConnectionString,
                    MySqlServerVersion.LatestSupportedServerVersion);
            });

            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

            services.AddAutoMapper(typeof(Startup));

            services.AddTransient(_ =>
            {
                var connection = new MySqlConnection(mySqlConnectionString);
                var compiler = new MySqlCompiler();

                return new QueryFactory(connection, compiler);
            });

            var jwtSecret = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);
            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(jwtSecret),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };
            services.AddSingleton(tokenValidationParams);

            services.Configure<JwtConfig>(Configuration.GetSection(nameof(JwtConfig)));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtConfig>>().Value);
            services.Configure<RefreshTokenConfig>(Configuration.GetSection(nameof(RefreshTokenConfig)));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<RefreshTokenConfig>>().Value);

            services.AddAuthentication(options =>
            {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(jwt =>
                {
                    jwt.SaveToken = true;
                    jwt.TokenValidationParameters = tokenValidationParams;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanReadUsers", policy => policy
                    .RequireClaim("CanReadUsers")
                    .RequireRole("Admin", "Moderator"));
            });

            services.AddSingleton<IAuthorizationHandler, CanReadUsersRequirementHandler>();

            services.AddControllers()
                .AddFluentValidation(s =>
                {
                    s.RegisterValidatorsFromAssemblyContaining<Startup>();
                    s.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BaseCamp_Web_API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}