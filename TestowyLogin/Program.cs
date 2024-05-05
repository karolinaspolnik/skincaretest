using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TestowyLogin;
using TestowyLogin.Database;
using NLog;
using NLog.Web;
using TestowyLogin.Middleware;
using TestowyLogin.Models;
using TestowyLogin.Models.QuizModel;
using TestowyLogin.Models.Validators;
using TestowyLogin.Services;

    var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
    logger.Debug("init main");

    var builder = WebApplication.CreateBuilder(args);


    builder.Logging.ClearProviders(); //NLog: Setup NLog for dependency injection
    builder.Host.UseNLog();

    var authenticationSettings = new AuthenticationSettings();
    builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);

    // Add services to the container.
    builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("SkinCareDataBase")); //database
    builder.Services.AddSingleton<DataInitializerService>();
    


    builder.Services.AddSingleton(authenticationSettings);
    builder.Services.AddAuthentication(option =>
    {
        option.DefaultAuthenticateScheme = "Bearer";
        option.DefaultScheme = "Bearer";
        option.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = authenticationSettings.JwtIssuer,
            ValidAudience = authenticationSettings.JwtIssuer,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
        };
    });
    builder.Services.AddControllers();

    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();
    ValidatorOptions.Global.LanguageManager.Enabled = false; //zeby nie tlumaczylo komunikatow na polski
    builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
    builder.Services.AddScoped<IValidator<UserAnswerDto>, UserAnswerDtoValidator>();////////////

    builder.Services.AddScoped<IUserService, UserService>(); //kazdy obiekt bedzie tworzony na nowo przy nowym zapytaniu wyslanym przez kleinta
    builder.Services.AddScoped<IQuizService, QuizService>();
    builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
    builder.Services.AddScoped<ErrorHandlingMiddleware>(); //middleware
    
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();



    var app = builder.Build();

    var dataInitializerService = app.Services.GetRequiredService<DataInitializerService>();
    dataInitializerService.InitializeData();

    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseAuthentication();
    app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseRouting();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();


