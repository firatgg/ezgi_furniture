using ezgi_mobilya.Core.Repositories;
using ezgi_mobilya.Core.UnitOfWorks;
using ezgi_mobilya.Data;
using ezgi_mobilya.Data.Repositories;
using ezgi_mobilya.Data.UnitOfWorks;
using ezgi_mobilya.Service;
using ezgi_mobilya.Web.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database connection
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly("ezgi_mobilya.Data");
    });
});

// Identity Configuration
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// DI Container (Repositories and UoW)
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Service Layer Registration (AutoMapper, FluentValidation)
builder.Services.AddServiceLayer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSerilogRequestLogging(); // Log HTTP requests

app.UseMiddleware<ExceptionMiddleware>(); // Custom Exception Middleware

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Added Identity Authentication
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
