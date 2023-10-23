using CafeteriaWeb.Data;
using CafeteriaWeb.Models;
using CafeteriaWeb.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User> (options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

//builder.Services.AddIdentity<User, IdentityRole>()
//            .AddEntityFrameworkStores<ApplicationDbContext>()
//            .AddDefaultTokenProviders();

//builder.Services.AddAuthentication()
//           .AddGoogle(googleOptions =>
//           {
//               googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//               googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//           });

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);
builder.Services.Configure<PaymentConfiguration>(builder.Configuration);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped(sp => ShoppingCart.GetShoppingCart(sp));
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AdressService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<OrderService>();

builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
