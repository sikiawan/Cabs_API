using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CabsAPI.Auth;
using CabsAPI.Authorization;
using Model.DataContext;
using AutoMapper;
using CabsAPI.Helpers;
using Model.Entity;
using Npgsql;
using Model.Models;
using System.Security;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Implementation;
using Microsoft.Extensions.FileProviders;
using Services.Interfaces;
using Services.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

        // Add services to the container.
        #region Auto Mapper Configurations
        builder.Services.AddAutoMapper(typeof(Program));
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new AutoMapperProfile());
        });
        IMapper mapper = mapperConfig.CreateMapper();
        #endregion Auto Mapper Configurations


        builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        builder.Services.AddDbContext<RestaurantContext>();

        //Registering Identity 
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequiredLength = 8;

        }).AddEntityFrameworkStores<RestaurantContext>()
        .AddDefaultTokenProviders();

        //Registering Mail Service
        builder.Services.Configure<MailSettings>(_config.GetSection("MailSettings"));

        //Registering Interface
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddTransient<IMailService, MailService>();
        builder.Services.AddTransient<IRestaurantService, RestaurantService>();
        builder.Services.AddTransient<IClientPreferenceService, ClientPreferenceService>();
        builder.Services.AddTransient<IBookingService, BookingService>();
        builder.Services.AddTransient<ICustomerService, CustomerService>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSignalR();

        //Swagger
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = ".NET-Core-Restaurant-API-8.0",
                Version = "v1"
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
            });
        });

        //Authentication
        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
        {
            option.SaveToken = true;
            option.TokenValidationParameters = new TokenValidationParameters
            {
                SaveSigninToken = true,
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["Jwt:Issuer"],       // Jwt:Issuer - config value 
                ValidAudience = _config["Jwt:Issuer"],     // Jwt:Issuer - config value 
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Key"])) // Jwt:Key - config value 
            };
        });
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowOrigin",
                builder => builder.WithOrigins("*") // Update with your frontend origin
                                  .AllowAnyHeader()
                                  .AllowAnyMethod());
        });

        //Authorization
        using (NpgsqlConnection connection = new NpgsqlConnection(builder.Configuration.GetConnectionString("WebAPIDb")))
        {
            connection.Open();

            string sql = "SELECT \"Id\", \"Type\", \"Name\" FROM \"Identity\".permissions";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    string permissionName = "";
                    string name = "";
                    string type = "";
                    while (reader.Read())
                    {
                        name = reader.GetString(2);
                        type = reader.GetString(1);
                        permissionName = "Permissions." + type + "." + name;
                        builder.Services.AddAuthorization(options =>
                        {
                            options.AddPolicy(permissionName, builder =>
                            {
                                builder.AddRequirements(new PermissionRequirement(permissionName));
                            });
                        });
                    }
                }
            }

            connection.Close();
        }
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        var app = builder.Build();
        var env = app.Environment;
        // Seed data initialize
        try
        {

            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    var dbcontext = serviceProvider.GetRequiredService<RestaurantContext>();
                    await SeedData.Initialize(roleManager, userManager, dbcontext);
                }
                catch
                {

                }
            }
        }
        catch (Exception exp)
        {

        }
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(env.WebRootPath, "Image")),
            RequestPath = "/Image"
        });
        app.UseHttpsRedirection();
        app.UseCors("AllowOrigin");
        app.MapHub<TestHub>("/testhub");
        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}