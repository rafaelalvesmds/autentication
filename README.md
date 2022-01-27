# Autentication

<br><br>
# OAuth 2

- OAuth 2 é um protocolo aberto que permie autorizacao de forma simples e padronizada entre aplicações web, mobile e aplicações desktop;
- OAuth 2 utiliza um acess token e a aplicação cliente usa esse token para acessar uma API ou endpoits;
- OAuth 2 determina como os endpoits serão usados em diferentes tipos de aplicações;

<br>

# OpenID Connect

- OpenID Connect é simplesmente uma camada de identificação no topo do protocolo OAuth 2;
- Através do OpenID Connect uma aplicação pode receber um Identity Token além de um access token se for o caso;
- OpenID Connect define como os diferentes tipos de aplicações cliente podem obter de forma segura um token do Identity Server;

<br>

# Identity Server 5 - Duende Identity Server

- https://duendesoftware.com/products/identityserver
- Identity Server é uma implementação do OpenID Connect e OAuth 2 e é altamente otimizado para resolver problemas de segurança comuns em aplicações atuais, sejam elas mobile, nativas ou mesmo aplicações web.

<br>

<div align=center>
<img src="https://user-images.githubusercontent.com/84939473/151229112-5edff638-4cb4-4287-ac8a-72b2241a54c1.png"/>
</div>
  
- Client: É um componente de software que requisita um token à um Identity Server - as vezes para autenticar um usuário ou para acessar um recurso;
- API Resource: Normalmente representam uma funcionalidade que um client precisa invocar - normalmente implementados através de Web API's;
- Identity Resource (Claims): Informações relacionadas à identidade do usuário. Ex: nome, e-mail etc;

- A resposta à um processo de autenticação;
- Access Token: Possibilita o acesso do usuário a um API Resource.

<br>

# JSON Web Token (JWT)

<div align=center>
<img src="https://user-images.githubusercontent.com/84939473/151230237-a82f8f3a-6073-4224-914b-a11c5cd111ab.png"/>
<img src="https://user-images.githubusercontent.com/84939473/151230408-5ea5d9a8-2095-409f-bbe6-1fdae4e2b049.png"/>
</div>

<br>

---

Install Duende
````bash
dotnet new --install Duende.IdentityServer.Templates
````

<br>

📁[E-commerce/foo]
````bash
dotnet new isui
````

<br>

📚 Instalar Dependências [E-commerce/E-commerce.IdentityServer]
````bash
Duende.IdentityServer.AspNetIdentity
Microsoft.AspNetCore.Identity.EntityFrameworkCore
Microsoft.AspNetCore.Identity.UI
Pomelo.EntityFrameworkCore.MySql
Microsoft.EntityFrameworkCore.Design
````

<br>

📁[Model/ApplicationUser.cs]
````bash
    public class ApplicationUser : IdentityUser
    {
        public string  FirstName { get; set; }
        public string SecondName { get; set; }
    }
````

<br>


📁[Model/Context/MySQLContext]
````bash
    public class MySQLContext : IdentityDbContext<ApplicationUser>
    {
        public MySQLContext(DbContextOptions<MySQLContext> options)
            : base(options) { }
    }
````

<br>

✅ IDENTITY CUSTOM CONFIGURATION
````bash
    public static class IdentityConfiguration
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
            };
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("e_commerce", "ECommerce Server"),
                new ApiScope(name: "read", "Read Data"),
                new ApiScope(name: "write", "Write Data"),
                new ApiScope(name: "delete", "Delete Data"),
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    ClientSecrets = { new Secret("my_super_secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "read", "write", "profile" },
                }
            };
    };
````

<br>

📁[Configuration/IdentityConfiguration]
````bash
 public static class IdentityConfiguration
    {
        public const string Admin = "Admin";
        public const string Client = "Client";

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
            };
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("e_commerce", "ECommerce Server"),
                new ApiScope(name: "read", "Read Data"),
                new ApiScope(name: "write", "Write Data"),
                new ApiScope(name: "delete", "Delete Data"),
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    ClientSecrets = { new Secret("my_super_secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "read", "write", "profile" },
                },
                new Client
                {
                    ClientId = "e_commerce",
                    ClientSecrets = { new Secret("my_super_secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = {"https://localhost:4430/signin-oidc"},
                    PostLogoutRedirectUris = {"https://localhost:4430/signout-callback-oidc"},
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "e_commerce"
                    }
                }
            };
    };
````

<br>

📁[Program.cs]
````bash
using E_Commerce.IdentutyServer.Configuration;
using E_Commerce.IdentutyServer.Model;
using E_Commerce.IdentutyServer.Model.Context;
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

config.AddDeveloperSigningCredential();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
````

<br><br>

## Migrations

Install
````bash
dotnet tool install --global dotnet-ef
```` 

Update
````bash
dotnet tool update --global dotnet-ef
```` 

Add-Migration
````bash
dotnet ef migrations add [name]
```` 

Update Datebase
````bash
dotnet ef database update
```` 

<br>

<div align=center>
<img src="https://user-images.githubusercontent.com/84939473/151391808-788d8102-8e58-4cc0-9476-6dabf8de12ee.png"/>
</div>

<br><br>


📁[Configuration/Initializer/IDBInitializer]
````bash
    public interface IDBInitializer
    {
        public void Initialize();
    }
````

<br>

### POPULANDO O BANCO

<br>

📁[Configuration/Initializer/DBInitializer]
````bash
 public class DBInitializer : IDBInitializer
    {
        private readonly MySQLContext _context;
        private readonly UserManager<ApplicationUser> _user;
        private readonly RoleManager<IdentityRole> _role;

        public DBInitializer(MySQLContext context,
            UserManager<ApplicationUser> user,
            RoleManager<IdentityRole> role)
        {
            _context = context;
            _user = user;
            _role = role;
        }

        public void Initialize()
        {
            if (_role.FindByNameAsync(IdentityConfiguration.Admin).Result != null) return;
            _role.CreateAsync(new IdentityRole(
                IdentityConfiguration.Admin)).GetAwaiter().GetResult();
            _role.CreateAsync(new IdentityRole(
              IdentityConfiguration.Client)).GetAwaiter().GetResult();

            ApplicationUser admin = new ApplicationUser()
            {
                UserName = "rafael-admin",
                Email = "rafaelalvesmds@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "+55 (31) 12345-6789",
                FirstName = "Rafael",
                SecondName = "Admin"
            };

            _user.CreateAsync(admin, "Rafael@2022").GetAwaiter().GetResult();
            _user.AddToRoleAsync(admin,
                IdentityConfiguration.Admin).GetAwaiter().GetResult();

            var adminClaims = _user.AddClaimsAsync(admin, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, $"{admin.FirstName} {admin.SecondName}"),
                new Claim(JwtClaimTypes.GivenName, admin.FirstName),
                new Claim(JwtClaimTypes.FamilyName, admin.SecondName),
                new Claim(JwtClaimTypes.Role, IdentityConfiguration.Admin),

            }).Result;

            ApplicationUser client = new ApplicationUser()
            {
                UserName = "rafael-client",
                Email = "rafaelalvesmds@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "+55 (31) 12345-6789",
                FirstName = "Rafael",
                SecondName = "Client"
            };

            _user.CreateAsync(client, "Rafael@2022").GetAwaiter().GetResult();
            _user.AddToRoleAsync(client,
                IdentityConfiguration.Client).GetAwaiter().GetResult();

            var clientClaims = _user.AddClaimsAsync(client, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, $"{client.FirstName} {client.SecondName}"),
                new Claim(JwtClaimTypes.GivenName, client.FirstName),
                new Claim(JwtClaimTypes.FamilyName, client.SecondName),
                new Claim(JwtClaimTypes.Role, IdentityConfiguration.Client),

            }).Result;
        }

    }
````

<br>

### INJEÇÃO DE DEPENDÊNCIA

<br>

📁[Program.cs]

````bash
builder.Services.AddScoped<IDBInitializer, DBInitializer>();
````

````bash
var scope = app.Services.CreateScope();
var serviceInitialize = scope.ServiceProvider.GetService<IDBInitializer>();
````

````bash
serviceInitialize.Initialize(); 
````
