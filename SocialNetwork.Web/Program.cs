using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Data.Entities;
using SocialNetwork.Data.Extensions;
using SocialNetwork.Data.Repositories;
using SocialNetwork.Data.Seeders;
using SocialNetwork.Data.UoW;
using SocialNetwork.Web.Infrastructure.Mapping;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var assembly = Assembly.GetAssembly(typeof(MappingProfile));

// �������� ILoggerFactory �� ���������� ��������� ������������
var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

// ����������� ������������ � �������������
builder.Services.AddControllersWithViews();

// ��������� DbContext � �������������� ILoggerFactory
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseLoggerFactory(loggerFactory)) // �������� ��� ��������� ������� �������
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


//// ��������� ������ IHttpContextAccessor � ��������� ��������� ������������
//builder.Services.AddHttpContextAccessor();

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