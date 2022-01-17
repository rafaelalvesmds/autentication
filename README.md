<h1 align="center">🚀 Projeto E-commerce 🚀</h1>
<br/> <br/>


## Iniciando Projeto (Blank Solution)
📁FrontEnd ( App Web ASP.NET Core MVC ) <br/>
[E-Commerce.Web] <br/><br/>
📁BackEnd ( API Web ASP.NET Core ) <br/>
[E-Commerce.API]

<br/>

<h1 align="center">📁 BackEnd </h1>


<h2 align="center">Adicionar Dependências</h2>

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

<h2 align="center">Criar Base de Dados</h2>

📁[Model/Base/BaseEntity.cs]

```bash
[Key]
[Column("id")]

public long Id { get; set; }
```

📁[Model/Context/MySQLContext.cs]

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

<h2 align="center">Criar Entidade </h2>

📁[Model/Product.cs]

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
  
<h2 align="center">Executar as Migrations [Package Manager Console]</h2>

````bash
add-migration [name]
````
````bash
update-database
````

<br/><br/>

<h2 align="center">Implementando Value Object </h2>

<br/>

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

<br/>

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

<br/>

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

<br/>

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

<br/>

<h2 align="center">Populando o Banco de Dados</h2>
📁[Model/Context/MySQLContext.cs]

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

<h2 align="center">Executar Migration</h2>

````bash
add-migration [name]
````
````bash
update-database
````
