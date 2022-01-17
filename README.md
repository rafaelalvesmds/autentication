<h1 align="center">E-commerce</h1>
<br/> <br/>


## Iniciando Projeto (Blank Solution)
- Criar FrontEnd ( App Web ASP.NET Core MVC ) <br/>
[E-Commerce.Web] 
- Criar BackEnd ( API Web ASP.NET Core ) <br/>
[E-Commerce.API]

<br/>

### E-Commerce.API
- Adicionar Dependências:
```bash
 AutoMapper
 AutoMapper.Extensions.Microsoft.DependencyInjection
 Microsoft.AspNetCore.Authentication.JwtBearer
 Microsoft.EntityFrameworkCore.Design
 Microsoft.EntityFrameworkCore.Tools
 Pomelo.EntityFrameworkCore.MySql
 Swashbuckle.AspNetCore
 Swashbuckle.AspNetCore.Annotations
 Swashbuckle.AspNetCore.SwaggerUI
```

<br/>

- Criar Base de Dados

📁[Model/Base/BaseEntity.cs]

```bash
[Key]
[Column("id")]

public long Id { get; set; }
```

📁[Model/Base/Context]

```bash
    public class MySQLContext : DbContext
    {
        public MySQLContext() { }
        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
    }
```

<br/>

- connectionString [Program.cs]

```bash
builder.Services.AddDbContext<MySQLContext>
    (options => options.UseMySql("Server=localhost; DataBase=shopping_product_api;Uid=root;Pwd=12345",
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.27-mysql")));
```

<br/>
  
- Criar Entidade 📁[Model/Product.cs]

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
  
- Executar as Migrations [Package Manager Console]

````bash
add-migration [name]
````
````bash
update-database
````

<br/>

## Organizando a Arquitetura do Microsserviço

<br/>

- Implementando Value Object 

<br/>

📁[Repository/IProductRepository.cs]
````bash
     public interface IProductRepository
    {   
    }
````

📁[Repository/IProductRepository.cs]
````bash
     public interface IProductRepository
    {   
    }
````

