using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcApplication.Database;

namespace MvcApplication.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    [HttpGet("")]
    public virtual ActionResult<string> ArgumentsWithoutAttributes()
    {
        return "Hello World!";
    }
    
    [HttpGet("carts")]
    public virtual async Task<ActionResult<IEnumerable<Cart>>> GetCarts([FromServices] MvcDbContext dbContext)
    {
        return await dbContext.Carts.ToListAsync();
    }
}
