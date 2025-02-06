using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models.DB;
using SocialNetwork.Models.Entities;
using System.Reflection;
using SocialNetwork;

var builder = WebApplication.CreateBuilder(args);
var assembly = Assembly.GetAssembly(typeof(MappingProfile));

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseLoggerFactory(builder.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>()));

builder.Services.AddIdentity<User, IdentityRole>(opts =>
{
    opts.Password.RequiredLength = 5;
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequireDigit = false;
}).AddEntityFrameworkStores<ApplicationDbContext>();

//builder.Services.AddMvc()
//    .AddViewOptions(options =>
//    {
//        options.HtmlHelperOptions.ClientValidationEnabled = false;
//    });

// Подключаем автомаппинг
builder.Services.AddAutoMapper(assembly);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseAuthentication();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();