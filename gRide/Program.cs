using FluentValidation;
using gRide.Data;
using gRide.IdentityPolicy;
using gRide.Models;
using gRide.Services;
using gRide.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

    //Database
var conStrBuilder = new NpgsqlConnectionStringBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));
conStrBuilder.Username = builder.Configuration["DbSettings:Username"];
conStrBuilder.Password = builder.Configuration["DbSettings:Password"];
builder.Services.AddDbContext<gRideDbContext>(options =>
    options.UseNpgsql(conStrBuilder.ConnectionString));

//Identity
builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = null;
    options.SignIn.RequireConfirmedEmail = true;
})
    .AddEntityFrameworkStores<gRideDbContext>()
    .AddDefaultTokenProviders()
    .AddUserManager<CustomUserManager<AppUser>>();

builder.Services.ConfigureApplicationCookie(options =>
    options.ExpireTimeSpan = TimeSpan.FromDays(14));

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    options.TokenLifespan = TimeSpan.FromHours(2));

builder.Services.AddTransient<ICustomUserValidator<AppUser>, CustomUserValidator<AppUser>>();

builder.Services.AddAuthentication()
    .AddFacebook(facebookOptions =>
    {
        facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
        facebookOptions.Fields.Add("picture");
        facebookOptions.ClaimActions.MapJsonKey("image", "picture");
        facebookOptions.ClaimActions.MapCustomJson("image",
            json => json.GetProperty("picture").GetProperty("data").GetProperty("url").GetString());
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        googleOptions.ClaimActions.MapJsonKey("image", "picture");
    });

    //Mail sender
builder.Services.AddSingleton<IMailSender, MailSender>();
var mailSenderConfig = builder.Configuration.GetSection("MailSenderSettings");
mailSenderConfig["Login"] = builder.Configuration["MailSender:Login"];
mailSenderConfig["Password"] = builder.Configuration["MailSender:Password"];
builder.Services.Configure<MailSenderSettings>(mailSenderConfig);

    //User info
builder.Services.AddScoped<IUserInfo, UserInfo>();
    
    //View to string converter
builder.Services.AddSingleton<IViewConverter, ViewConverter>();

    //Validators
builder.Services.AddScoped<IValidator<NewEventViewModel>, NewEventValidator>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
