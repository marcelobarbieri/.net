using Exemplo;
using Microsoft.AspNetCore.Mvc;

namespace Exemplo.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class DataController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        [HttpGet("/")]
        public IAsyncEnumerable<int> Get()
        {
            IAsyncEnumerable<int> value = GetData();
            return value;
        }

        private static async IAsyncEnumerable<int> GetData()
        {
            for (var i = 1; i <= 1000; i++)
            {
                await Task.Delay(1000);
                yield return i;
            }
        }
    }
}

