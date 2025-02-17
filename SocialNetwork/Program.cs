using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using SocialNetwork;
using SocialNetwork.DLL.DB;
using SocialNetwork.DLL.Entities;
using SocialNet.Data.UoW;
using SocialNetwork.DLL.UoW;
using SocialNet.Extentions;
using SocialNetwork.DLL.Repositories;

var builder = WebApplication.CreateBuilder(args);
var assembly = Assembly.GetAssembly(typeof(MappingProfile));

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseLoggerFactory(builder.Services.BuildServiceProvider()
           .GetRequiredService<ILoggerFactory>()))
            .AddCustomRepository<Message, MessagesRepository>()
            .AddCustomRepository<Comment, CommentsRepository>()
            .AddTransient<IUnitOfWork, UnitOfWork>(); ;


builder.Services.AddIdentity<User, IdentityRole>(opts =>
{
    opts.Password.RequiredLength = 5;
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequireDigit = false;
    opts.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" +
        "àáâãäå¸æçèéêëìíîïğñòóôõö÷øùúûüışÿ" +
        "ÀÁÂÃÄÅ¨ÆÇÈÉÊËÌÍÎÏĞÑÒÓÔÕÖ×ØÙÚÛÜİŞß" +
        "-_.";
}).AddEntityFrameworkStores<ApplicationDbContext>();

//builder.Services.AddMvc()
//    .AddViewOptions(options =>
//    {
//        options.HtmlHelperOptions.ClientValidationEnabled = false;
//    });

// Ïîäêëş÷àåì àâòî ìàïïèíã
builder.Services.AddAutoMapper(assembly);
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseStatusCodePagesWithReExecute("/Error/Error/{0}");

app.UseAuthentication();
app.UseAuthorization();


//app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/Home/Index");
        return;
    }
    await next();
});

app.Run();