using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using User.Application.Services;
using User.Domain.Ports;
using User.Domain.Validators;
using User.Infraestructure.Data;
using User.Infraestructure.Persistence;

namespace User.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Configuration: inicializar conexión a BD singleton
            DatabaseConnection.Initialize(builder.Configuration);

            // Configuración de SMTP (se espera sección "Smtp" en appsettings)
            builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

            // Registraciones de DI
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserValidator, UserValidator>();
            builder.Services.AddScoped<IEncryptionService, EncryptionService>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            // registrar builder/factory/password/facade
            builder.Services.AddScoped<IUsernameGenerator, UsernameGenerator>();
            builder.Services.AddScoped<IUserBuilder, UserBuilder>();
            builder.Services.AddScoped<IUserFactory, UserFactory>();
            builder.Services.AddScoped<IPasswordService, PasswordService>();
            builder.Services.AddScoped<IUserFacade, UserFacade>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}