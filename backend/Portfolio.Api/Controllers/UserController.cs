using Microsoft.AspNetCore.Mvc;
using Portfolio.Data;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly PortfolioContext _context;

    public UserController(PortfolioContext context)
    {
        _context = context;
    }
}
