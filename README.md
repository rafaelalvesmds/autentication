<h1 align="center">🚀 Projeto E-commerce 🚀</h1>
<br/> <br/>


- Visual Studio Community 2022
- .NET 6.0


## Iniciando Projeto (Blank Solution)
📁FrontEnd ( App Web ASP.NET Core MVC ) <br/>
[E-Commerce.Web] <br/><br/>
📁BackEnd ( API Web ASP.NET Core ) <br/>
[E-Commerce.API]

<br/>

<h1 align="center">📁 BackEnd </h1>


<h2 align="center">Adicionando Dependências</h2>

````bash
 AutoMapper
 AutoMapper.Extensions.Microsoft.DependencyInjection
 Microsoft.AspNetCore.Authentication.JwtBearer
 Microsoft.EntityFrameworkCore.Design
 Microsoft.EntityFrameworkCore.Tools
 Pomelo.EntityFrameworkCore.MySql
 Swashbuckle.AspNetCore
 Swashbuckle.AspNetCore.Annotations
 Swashbuckle.AspNetCore.SwaggerUI
````

<br/>

<h2 align="center">Criando Base de Dados</h2>

📁[Models/Base/BaseEntity.cs]

```bash
[Key]
[Column("id")]

public long Id { get; set; }
```

📁[Models/Context/MySQLContext.cs]

```bash
    public class MySQLContext : DbContext
    {
        public MySQLContext() { }
        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
    }
```

<br/>

↔️ connectionString [Program.cs]

```bash
builder.Services.AddDbContext<MySQLContext>
    (options => options.UseMySql("Server=localhost; DataBase=shopping_product_api;Uid=root;Pwd=12345",
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.27-mysql")));
```

<br/>

<h2 align="center">Criando Entidade </h2>

📁[Models/Product.cs]

```bash
{
     public class Product : BaseEntity
    {   
        [Column("name")]
        [Required]
        [StringLength(150)]
        public string? Name { get; set; }

        [Column("price")]
        [Required]
        [Range(1,10000)]
        public decimal Price { get; set; }

        [Column("description")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Column("category_name")]
        [StringLength(50)]
        public string? CategoryName { get; set;}
        
        [Column("imageUrl")]
        [StringLength(300)]
        public string? imageUrl { get; set; }
    }
}
```

<br/>
  
<h2 align="center">Executando as Migrations</h2>

````bash
add-migration [name]
````
````bash
update-database
````

<br/>

<h2 align="center">Implementando Value Object </h2>

📁[Data/ValueObjects/ProductVO.cs]
````bash
     public class ProductVO
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? CategotyName { get; set; }
        public string? imageUrl { get; set; }
    }
````
📁[Repository/IProductRepository.cs]
````bash
     public interface IProductRepository
    {
        Task<IEnumerable<ProductVO>> FindAll();
        Task<ProductVO> FindById(long id);
        Task<ProductVO> Create(ProductVO vo);
        Task<ProductVO> Update(ProductVO vo);
        Task<bool> Delete(long id);
    }
````

<br/>

<h2 align="center">Configurando AutoMapper</h2>

📁[Config/MappingConfig.cs]
````bash
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config => {
                config.CreateMap<ProductVO, Product>();
                config.CreateMap<Product, ProductVO>();
                });
            return mappingConfig;
        }
    }
````
[Program.cs]
````bash
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
````

<br/>

<h2 align="center">Implementando Repositório de Produtos</h2>

📁[Repository/ProductRepository.cs]
````bash
public class ProductRepository : IProductRepository
    {

        private readonly MySQLContext _context;
        private IMapper _mapper;

        public ProductRepository(MySQLContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductVO>> FindAll()
        {
            List<Product> products = await _context.Products.ToListAsync();
            return _mapper.Map<List<ProductVO>>(products);
        }

        public async Task<ProductVO> FindById(long id)
        {
            Product product =
                await _context.Products.Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            return _mapper.Map<ProductVO>(product);
        }
        public async Task<ProductVO> Create(ProductVO vo)
        {
            Product product = _mapper.Map<Product>(vo);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProductVO>(product);
        }
        public async Task<ProductVO> Update(ProductVO vo)
        {
            Product product = _mapper.Map<Product>(vo);
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProductVO>(product);
        }
        public async Task<bool> Delete(long id)
        {
            try
            {
                Product product =
                await _context.Products.Where(p => p.Id == id)
                    .FirstOrDefaultAsync();
                if(product == null) return false;
                _context.Products.Remove(product);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
````

[Program.cs]
````bash
builder.Services.AddScoped<IProductRepository, ProductRepository>();
````
<br/>

<h2 align="center">Criando Controller (API Controler Empty)</h2>
<h3 align="center">GET, POST, PUT, DELETE</h3>

📁[Controllers/ProductController.cs]
````bash
[Route("api/v1/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductRepository _repository;
        public ProductController(IProductRepository repository)
        {
            _repository = repository ?? throw new 
                ArgumentNullException(nameof(repository));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductVO>> FindById(long id)
        {
            var product = await _repository.FindById(id);
            if(product == null) return NotFound();
            return Ok(product);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductVO>>> FindAll(long id)
        {
            var products = await _repository.FindAll();
            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ProductVO vo)
        {
            if(vo == null) return BadRequest();
            var product = await _repository.Create(vo);
            return Ok(product);
        }

        [HttpPut]
        public async Task<ActionResult> Update(ProductVO vo)
        {
            if (vo == null) return BadRequest();
            var product = await _repository.Update(vo);
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var status = await _repository.Delete(id);
            if (!status) return BadRequest();
            return Ok(status);
        }

    }
````
![image](https://user-images.githubusercontent.com/84939473/149830110-c831b645-300e-4524-9897-7c36a1da7191.png)


<br/>

<h2 align="center">Populando o Banco de Dados</h2>
📁[Models/Context/MySQLContext.cs]

````bash
public class MySQLContext : DbContext
    {
        public MySQLContext() { }
        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             modelBuilder.Entity<Product>().HasData(new Product
             {
              Id = 1,
              Name = "Capacete Darth Vader Star Wars Black Series",
              Price = new decimal(999.99),
              Description = "It is a long established fact that a reader will.",
              imageUrl = "https://github.com/leandrocgsi/erudio-microservices-dotnet6/blob/main/ShoppingImages/3_vader.jpg?raw=true",
              CategoryName = "Action Figure"
             });
             modelBuilder.Entity<Product>().HasData(new Product
             {
              Id = 2,
              Name = "Star Wars The Black Series Hasbro - Stormtrooper Imperial",
              Price = new decimal(189.99),
              Description = "It is a long established fact that a reader will.",
              imageUrl = "https://github.com/leandrocgsi/erudio-microservices-dotnet6/blob/main/ShoppingImages/4_storm_tropper.jpg?raw=true",
              CategoryName = "Action Figure"
             });
             ...
         }
     }
````
<br/>

<h2 align="center">Executando Migration</h2>

````bash
add-migration [name]
````
````bash
update-database
````
![image](https://user-images.githubusercontent.com/84939473/149830689-13cb7f7e-27eb-4c78-b077-bb54d37a673d.png)

<br/><br/>

<h2 align="center">Integração BackEnd / FrontEnd</h2>

<h1 align="center">📁 FrontEnd </h1>

[appsettings.jon]
````bash
	"ServiceUrls": {
		"ProductAPI": "https://localhost:7077"
	}
````

<br/>

<h2 align="center">Criando Model</h2>

📁 [Models/ProductModel.cs]
````bash
    public class ProductModel
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? CategotyName { get; set; }
        public string? imageUrl { get; set; }
    }
````

<br/>

<h2 align="center">Definindo as Operações da Interface do Serviço</h2>

📁 [Services/IServices/IProductService.cs]
````bash
    public interface IProductService
    {
        Task<IEnumerable<ProductModel>> FindAllProducts();
        Task<ProductModel> FindProductById(long id);
        Task<ProductModel> CreateProduct(ProductModel model);
        Task<ProductModel> UpdateProduct(ProductModel model);
        Task<bool> DeleteProductById(long id);
    }
````

<br/>

<h2 align="center">Implementando a Classe HttpClientExtensions</h2>

📁 [Utils/HttpClientExtensions.cs]
````bash
    public static class HttpClientExtensions
    {
        private static MediaTypeHeaderValue contentType
            = new MediaTypeHeaderValue("application/json");
        public static async Task<T> ReadContentAs<T>(
            this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode) throw
                     new ApplicationException(
                         $"Something went wrong calling the API: " +
                         $"{response.ReasonPhrase}");
            var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(dataAsString,
                new JsonSerializerOptions
                {PropertyNameCaseInsensitive = true});
        }

        public static Task<HttpResponseMessage> PostAsJson<T>(
            this HttpClient httpClient,
            string url,
            T data)
        {
            var dataAsString = JsonSerializer.Serialize(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = contentType;
            return httpClient.PostAsync(url, content);
        }
        
        public static Task<HttpResponseMessage> PutAsJson<T>(
            this HttpClient httpClient,
            string url,
            T data)
        {
            var dataAsString = JsonSerializer.Serialize(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = contentType;
            return httpClient.PutAsync(url, content);
        }
            

    }
````

<br/>

<h2 align="center">Implementando a Classe ProductService</h2>

📁 [Service/ProductService.cs]
````bash
    public class ProductService : IProductService
    {
        private readonly HttpClient _client;
        public const string BasePath = "api/v1/product";

        public ProductService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IEnumerable<ProductModel>> FindAllProducts()
        {
            var response = await _client.GetAsync(BasePath);
            return await response.ReadContentAs<List<ProductModel>>();
        }

        public async Task<ProductModel> FindProductById(long id)
        {
            var response = await _client.GetAsync($"{BasePath}/{id}");
            return await response.ReadContentAs<ProductModel>();
        }

        public async Task<ProductModel> CreateProduct(ProductModel model)
        {
            var response = await _client.PostAsJson(BasePath, model);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<ProductModel>();
            else throw new Exception("Something went wrong when calling API");
        }
        public async Task<ProductModel> UpdateProduct(ProductModel model)
        {
            var response = await _client.PutAsJson(BasePath, model);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<ProductModel>();
            else throw new Exception("Something went wrong when calling API");
        }

        public async Task<bool> DeleteProductById(long id)
        {
            var response = await _client.DeleteAsync($"{BasePath}/{id}");
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<bool>();
            else throw new Exception("Something went wrong when calling API");
        }
    }
````

<br/>

<h2 align="center">Injeção de Dependência</h2>

[Program.cs]

````bash
builder.Services.AddHttpClient<IProductService, ProductService>(c =>
                    c.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"])
                );
builder.Services.AddControllersWithViews();
````

<br/>

<h2 align="center">Integrando FRONT/BACK</h2>

<br/>

📁[E-Commerce.Web/Views/Shared/_Layout.cshtml]

````bash
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC" crossorigin="anonymous">
````

````bash
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.bundle.min.js" integrity="sha384-MrcW6ZMFYlzcLA8Nl+NtUVF0sA7MsXsP1UyJoMp4YLEuNSfAP+JcXn/tWtIaxVXM" crossorigin="anonymous"></script>
````

````bash
<link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.15.4/css/all.css" integrity="sha384-DyZ88mC6Up2uqS4h/KRgHuoeGwBcD4Ng9SiP4dIRy0EXTlnuz47vAwmeGwVChigm" crossorigin="anonymous">
````

<br/>

<h2 align="center">Criando Controller</h2>

<br/>

📁 [E-Commerce.Web/Controllers/ProductController.cs]
````bash
public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> ProductIndex()
        {
            var products = await _productService.FindAllProducts();
            return View(products);
        }
        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.CreateProduct(model);
                if (response != null) return RedirectToAction(nameof(ProductIndex));
            }
            return View(model);
        }

        public async Task<IActionResult> ProductUpdate(int id)
        {
            var model = await _productService.FindProductById(id);
            if (model != null) return View(model);
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductUpdate(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.UpdateProduct(model);
                if (response != null) return RedirectToAction(nameof(ProductUpdate));
            }
            return View(model);
        }

        public async Task<IActionResult> ProductDelete(int id)
        {
            var model = await _productService.FindProductById(id);
            if (model != null) return View(model);
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductModel model)
        {
            var response = await _productService.DeleteProductById(model.Id);
            if (response) return RedirectToAction(nameof(ProductIndex));
            return View(model);
        }
    }
````

<br/>

<h2 align="center">Criando Camada de Apresemtação</h2>

<br/>
📁[E-Commerce.Web/Views/Product/ProductIndex.csproj] <br/>
📁[E-Commerce.Web/Views/Product/ProductCreate.csproj] <br/>
📁[E-Commerce.Web/Views/Product/ProductUpload.csproj] <br/>
📁[E-Commerce.Web/Views/Product/ProductDelete.csproj] <br/>
