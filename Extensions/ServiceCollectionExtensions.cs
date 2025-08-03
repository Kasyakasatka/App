using UserManagementApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Npgsql;
using UserManagementApp.Models;
using UserManagementApp.DTOs;
using UserManagementApp.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using UserManagementApp.Services;

namespace UserManagementApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFluentValidators(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();

            services.AddScoped<IValidator<RefreshToken>, RefreshTokenValidator>();
            services.AddScoped<IValidator<ApplicationUser>, ApplicationUserValidator>();
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
            services.AddScoped<IValidator<LoginUserDto>, LoginUserDtoValidator>();
            services.AddScoped<IValidator<ForgotPasswordDto>, ForgotPasswordDtoValidator>();
            services.AddScoped<IValidator<ForgotPasswordWithOtpDto>, ForgotPasswordWithOtpDtoValidator>();
            services.AddScoped<IValidator<ResetPasswordDto>, ResetPasswordDtoValidator>();

            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Логгируем полученную строку подключения, чтобы проверить, что она не пустая и правильная.
            // Log the retrieved connection string to verify it's not empty and is correct.
            Console.WriteLine($"Connection string from configuration: {connectionString}");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
                }

                // Если строка подключения имеет URI-формат (начинается с "postgresql://"), преобразуем её
                // If the connection string has a URI format (starts with "postgresql://"), convert it
                if (connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
                {
                    connectionString = ConvertUriToNpgsqlString(connectionString);
                }

                // Добавляем Trust Server Certificate=true к строке подключения
                // Add Trust Server Certificate=true to the connection string
                var finalConnectionString = connectionString + ";Trust Server Certificate=true";

                options.UseNpgsql(finalConnectionString);
            });

            services.AddLogging(builder => builder.AddConsole());

            return services;
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddSingleton<IConfiguration>(configuration);

            return services;
        }

        private static string ConvertUriToNpgsqlString(string uriString)
        {
            var uri = new Uri(uriString);
            var userInfo = uri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.Port,
                Username = userInfo[0],
                Password = Uri.UnescapeDataString(userInfo[1]), // Экранируем пароль
                Database = uri.AbsolutePath.Trim('/')
            };

            return builder.ConnectionString;
        }
    }
}
