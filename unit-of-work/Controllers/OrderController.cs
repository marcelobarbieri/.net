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
        [HttpPost]
        [Route("")]
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