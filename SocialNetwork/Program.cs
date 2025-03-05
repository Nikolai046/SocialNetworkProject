using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNetwork;
using SocialNetwork.DLL.DB;
using SocialNetwork.DLL.Entities;
using SocialNetwork.DLL.Extensions;
using SocialNetwork.DLL.Repositories;
using SocialNetwork.DLL.UoW;
using SocialNetwork.Service;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var assembly = Assembly.GetAssembly(typeof(MappingProfile));

// ����������� ������������ � �������������
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            .UseLoggerFactory(builder.Services.BuildServiceProvider()
            .GetRequiredService<ILoggerFactory>()))
            .AddCustomRepository<Message, MessagesRepository>()
            .AddCustomRepository<Comment, CommentsRepository>()
            .AddCustomRepository<Friend, FriendsRepository>()
            .AddCustomRepository<ServiceData, ServiceDataRepository>()
            .AddTransient<IUnitOfWork, UnitOfWork>();

// ��������� Identity � ���������� ������������ � �������
builder.Services.AddIdentity<User, IdentityRole>(opts =>
{
    opts.Password.RequiredLength = 5;   // ����������� ����� ������
    opts.Password.RequireNonAlphanumeric = false;   // �� ��������� �� ���������-�������� ��������
    opts.Password.RequireLowercase = false; // �� ��������� �������� ����
    opts.Password.RequireUppercase = false; // �� ��������� ��������� ����
    opts.Password.RequireDigit = false; // �� ��������� ����
    opts.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" +
        "��������������������������������" +
        "�����Ũ��������������������������" +
        "-_.";
}).AddEntityFrameworkStores<ApplicationDbContext>();

// ����������� AutoMapper
builder.Services.AddAutoMapper(assembly);

// ��������� ������������� (URL � ������ ��������)
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// ����������� ���������� �������� ������
builder.Services.AddScoped<TestDataGenerator>();

var app = builder.Build();

// �������� ��������� � ����������� �� ���������
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();

    // ��������� ����������� ����������� ������ � ������ ����������
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
            ctx.Context.Response.Headers.Append("Pragma", "no-cache");
            ctx.Context.Response.Headers.Append("Expires", "0");
        }
    });
}
else
{
    // � ���������� ��������� ����������� ����������� ������
    app.UseStaticFiles();
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// ��������� ������ � ���������������� �� �������� ������
app.UseStatusCodePagesWithReExecute("/Error/Error/{0}");

app.UseAuthentication();
app.UseAuthorization();

// ���������� �������� � ��������� �������� ������
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    if (app.Environment.IsDevelopment())
    {
        var dataGen = scope.ServiceProvider.GetRequiredService<TestDataGenerator>();
        await dataGen.Generate(30);
    }
}

// ��������� �������� �� ���������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ��������������� � ��������� URL �� /Home/Index
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