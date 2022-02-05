
[ASP.NET Core APIs - Repository Pattern e Unit of Work | por André Baltieri #balta](https://www.youtube.com/watch?v=HdsRpSK4PUg)

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

### Repository Pattern

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












