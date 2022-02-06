# ASP.NET Core APIs - Repository Pattern e Unit of Work

> por André Baltieri #balta

[Vídeo](https://www.youtube.com/watch?v=HdsRpSK4PUg)

```ps
dotnet --list-dsks
dotnet new globaljson --sdk-version 3.1.200

dotnet new webapi
```

Criar pasta *Models* e entidades *Customer* e *Order*

```
Models/
    Customer.cs
    Order.cs
```

Customer.cs
```c#
namespace UnitOfWork.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
```

Order.cs
```c#
namespace UnitOfWork.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
```

## Repository Pattern

Criar diretório *Repositories* e um repositório para cada modelo.

```ps
Repositories/
    CustomerRepository.cs
    OrderRepository.cs
```

CustomerRepository.cs
```c#
using UnitOfWork.Models;

namespace UnitOfWork.Repositories
{
    public interface ICustomerRepository
    {
        void Save(Customer customer);
    }

    public class CustomerRepository : ICustomerRepository
    {
        public void Save(Customer customer)
        {

        }
    }
}
```

OrderRepository.cs
```c#
using UnitOfWork.Models;

namespace UnitOfWork.Repositories
{
    public interface IOrderRepository
    {
        void Save(Order order);
    }

    public class OrderRepository : IOrderRepository
    {
        public void Save(Order order)
        {

        }
    }
}
```

## Entity Framework

Instalar pacote do Microsoft EntityFramework Core InMemory
```ps
dotnet add package microsoft.entityframeworkcore.inmemory --version 3.1.3
```

Criar diretório *Data* e contexto de dados

```ps
Data/
    DataContext.cs
``` 

DataContext.cs
```c#
using Microsoft.EntityFrameworkCore;
using UnitOfWork.Models;

namespace UnitOfWork.Data
{
    public class DataContext : DbContext    
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }
            
    
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
```

Nos repositórios gerar dependência do contexto de dados através do construtor.

CustomerRepository.cs
```c#
using UnitOfWork.Data;
using UnitOfWork.Models;

namespace UnitOfWork.Repositories
{
    public interface ICustomerRepository
    {
        void Save(Customer customer);
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly DataContext _context;
        public CustomerRepository(DataContext context)
        {
            _context = context;
        }

        public void Save(Customer customer)
        {

        }
    }
}
```

Para adicionar um cliente no banco...

CustomerRepository.cs
```c#
...
        public void Save(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges(); // persiste no banco de dados
        }
... 
```

OrderRepository.cs
```c#
using UnitOfWork.Data;
using UnitOfWork.Models;

namespace UnitOfWork.Repositories
{
    public interface IOrderRepository
    {
        void Save(Order order);
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _context;
        public OrderRepository(DataContext context)
        {
            _context = context;
        }

        public void Save(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }
    }
}
```

## Resolver dependências

Registrar o contexto de dados como serviço pois foi utilizada injeção de dependência nos construtores dos repositórios criados.

Startup.cs
```c#
...
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("database"));
        }
...
```

Adicionar serviço transitório do tipo especificado com a sua implementação.

Startup.cs
```c#
...
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("database"));
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
        }
...
```

## Controladores

Criar novo controlador

Controllers/OrderController.cs

OrderController.cs
```c#
using Microsoft.AspNetCore.Mvc;
using UnitOfWork.Models;
using UnitOfWork.Repositories;

namespace UnitOfWork.Controllers
{
    [ApiController]
    [Route("v1/orders")]
    public class OrderController : ControllerBase
    {
        [HttpPost]
        [Route("")]    
        public Order Post(
            [FromServices] ICustomerRepository customerRepository,
            [FromServices] IOrderRepository orderRepository
        )
        {
            try
            {
                var customer = new Customer { Name = "André Baltieri" };
                var order = new Order { Number = "123", Customer = customer };

                customerRepository.Save(customer);
                orderRepository.Save(order);

                return order;
            }
            catch
            {
                return null;
            }
        }
    }
}
```

Neste cenário são gerados o cliente e o pedido.

Inicialmente o cliente é incluído com a chamada do *CustomerRepository* para salvar as alterações deste contexto na base de dados com a chamada do método *Save*.

O próximo passo seria salvar o pedido seguindo a mesma lógica para a inclusão do cliente. Porém, hipoteticamente, pode ocorrer uma falha ao persistir a base de dados por qualquer motivo.

Com isso, o cliente foi persistido na base de dados mas o pedido não.

O tratamento correto seria transacionar a persistência dos dados no banco de dados de ambos os repositórios para realizar o *rollback* caso ocorra algum erro, para não ocorrer o cenário acima.

Por isso, o método *SaveChanges* que persiste todas as alterações realizados no contexto de dados para a base de dados, não deve ser utilizado dentro dos repositórios.

Para resolver isso são utilizados as unidades de trabalho, ou *Unit of Work*

## Unit Of Work

Criar arquivo UnitOfWork.cs dentro do diretório Data/

```
Data/
    UnitOfWork.cs
```

A função do *Unit of Work* é externalizar (trazer para fora) o *commit* e *rollback* do método *SaveChanges* existente dentro dos repósitorios. Ou seja, a as alterações realizadas no contextos de dados serão transacionadas fora dos repositórios para então serem persistidas no banco de dados.

UnitOfWork.cs
```c#
namespace UnitOfWork.Data
{
    public interface IUnitOfWork
    {
        void Commit();
        void Rollback();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        public UnitOfWork(DataContext context)
        {
            _context = context;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Rollback()
        {
            //
        }
    }
}
```

Retirar o método *SaveChanges* dos repositórios.

CustomerRepository.cs
```c#
using UnitOfWork.Data;
using UnitOfWork.Models;

namespace UnitOfWork.Repositories
{
    public interface ICustomerRepository
    {
        void Save(Customer customer);
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly DataContext _context;
        public CustomerRepository(DataContext context)
        {
            _context = context;
        }

        public void Save(Customer customer)
        {
            _context.Customers.Add(customer);
            // _context.SaveChanges();
        }
    }
}
```

OrderRepository.cs
```c#
using UnitOfWork.Data;
using UnitOfWork.Models;

namespace UnitOfWork.Repositories
{
    public interface IOrderRepository
    {
        void Save(Order order);
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _context;
        public OrderRepository(DataContext context)
        {
            _context = context;
        }

        public void Save(Order order)
        {
            _context.Orders.Add(order);
            // _context.SaveChanges();
        }
    }
}
```

Adicionar serviço transitório do tipo especificado com a sua implementação para *UnitOfWork*

Startup.cs
```c#
...
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("database"));
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
...
```

Injetar dependência de *UnitOfWork* no controlador de pedido e realizar o *commit* para tentar salvar tudo de uma vez. Caso ocorra algum erro o *rollback* poderá ser chamado.

OrderController.cs
```c#
using Microsoft.AspNetCore.Mvc;
using UnitOfWork.Data;
using UnitOfWork.Models;
using UnitOfWork.Repositories;

namespace UnitOfWork.Controllers
{
    [ApiController]
    [Route("v1/orders")]
    public class OrderController : ControllerBase
    {
        public Order Post(
            [FromServices] ICustomerRepository customerRepository,
            [FromServices] IOrderRepository orderRepository,
            [FromServices] IUnitOfWork unitOfWork
        )
        {
            try
            {
                var customer = new Customer { Name = "André Baltieri" };
                var order = new Order { Number = "123", Customer = customer };

                customerRepository.Save(customer);
                orderRepository.Save(order);

                unitOfWork.Commit();

                return order;
            }
            catch
            {
                unitOfWork.Rollback();
                return null;
            }
        }
    }
}
```

## Executar

```ps
dotnet run

info: Microsoft.Hosting.Lifetime[0]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\DEV\.NET\unit-of-work
```

## Postman

POST    https://localhost:5001/v1/orders

```json
{
    "id": 1,
    "number": "123",
    "customerId": 1,
    "customer": {
        "id": 1,
        "name": "André Baltieri"
    }
}
```








