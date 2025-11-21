
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using AutoPartInventorySystem.Data;
using AutoPartInventorySystem.Profiles;
using AutoPartInventorySystem.Repositories.Contracts;
using AutoPartInventorySystem.Repositories.Implementations;
using AutoPartInventorySystem.Services.Contracts;
using AutoPartInventorySystem.Services.Implementations;
using AutoPartInventorySystem.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AutoPartInventorySystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Allow any origin
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            // ---------------------------------------------------
            // Load Parameter Store *only in Production*
            // ---------------------------------------------------
            if (builder.Environment.IsProduction())
            {
                builder.Configuration.AddSystemsManager(
                    "/auto_part_inventory_api",
                    new AWSOptions
                    {
                        Region = RegionEndpoint.USEast1
                    }
                );

                Console.WriteLine(">>> Environment: Production");
                Console.WriteLine(">>> DefaultConnection: " + builder.Configuration.GetConnectionString("DefaultConnection"));
            }
            else
            {
                var awsOptions = builder.Configuration.GetAWSOptions();
                builder.Services.AddDefaultAWSOptions(awsOptions);
            }

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "AutoPartInventoryAPI", Version = "v1" });

                // Add JWT Authentication
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by your JWT token."
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // ---------------------------------------------------
            // 3. Register services
            // ---------------------------------------------------
            builder.Services.AddSingleton<IAmazonS3>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var awsSection = config.GetSection("AWS");
                var accessKey = awsSection["AccessKey"];
                var secretKey = awsSection["SecretKey"];
                var region = awsSection["Region"];

                var amazonConfig = new AmazonS3Config
                {
                    RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region)
                };

                return new AmazonS3Client(accessKey, secretKey, amazonConfig);
            });

            builder.Services.AddSingleton<IAmazonDynamoDB>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var awsSection = config.GetSection("AWS");
                var accessKey = awsSection["AccessKey"];
                var secretKey = awsSection["SecretKey"];
                var region = awsSection["Region"];

                var amazonConfig = new AmazonDynamoDBConfig
                {
                    RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region)
                };

                return new AmazonDynamoDBClient(accessKey, secretKey, amazonConfig);
            });

            // ---------------------------------------------------
            // Configure EF Core (ConnectionString will come from appsettings or Parameter Store)
            // ---------------------------------------------------

            builder.Services.AddDbContext<AutoPartInventoryDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var jwtKey = builder.Configuration["Jwt:Key"];
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            // Add AutoMapper
            builder.Services.AddAutoMapper(
                cfg => { },                  // optional, empty lambda is fine
                typeof(UserProfile)          // assembly where your profile is located
            );

            // Add services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IStorageService, S3StorageService>();
            builder.Services.AddScoped<IBrandService,  BrandService>();
            builder.Services.AddScoped<IAutoPartService, AutoPartService>();

            // Add repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IBrandRepository, BrandRepository>();
            builder.Services.AddScoped<IAutoPartRepository, AutoPartRepository>();
            builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();

            // Add Utils
            builder.Services.AddSingleton<PasswordHasher>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
