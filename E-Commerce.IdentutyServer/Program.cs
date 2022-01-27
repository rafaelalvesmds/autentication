using E_Commerce.IdentityServer.Configuration;
using E_Commerce.IdentityServer.Configuration.Initializer;
using E_Commerce.IdentityServer.Model;
using E_Commerce.IdentityServer.Model.Context;
using E_Commerce.IdentuiyServer.Configuration.Initializer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MySQLContext>
    (options => options.UseMySql("Server=localhost; DataBase=shopping_product_IdentityServer;Uid=root;Pwd=admin123",
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.27-mysql")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<MySQLContext>()
    .AddDefaultTokenProviders();

var config = builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.EmitStaticAudienceClaim = true;
}).AddInMemoryIdentityResources(
                        IdentityConfiguration.IdentityResources)
                    .AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
                    .AddInMemoryClients(IdentityConfiguration.Clients)
                    .AddAspNetIdentity<ApplicationUser>();

builder.Services.AddScoped<IDBInitializer, DBInitializer>();

config.AddDeveloperSigningCredential();

var app = builder.Build();

var scope = app.Services.CreateScope();
var serviceInitialize = scope.ServiceProvider.GetService<IDBInitializer>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

serviceInitialize.Initialize(); 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
