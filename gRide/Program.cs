using gRide.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//Database initialization
var conStrBuilder = new NpgsqlConnectionStringBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));
conStrBuilder.Username = builder.Configuration["DbSettings:Username"];
conStrBuilder.Password = builder.Configuration["DbSettings:Password"];
builder.Services.AddDbContext<gRideDbContext>(options =>
    options.UseNpgsql(conStrBuilder.ConnectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    //options.SignIn.RequireConfirmedAccount = true;
})
    .AddEntityFrameworkStores<gRideDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // User settings.
    options.User.RequireUniqueEmail = true;
});

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
