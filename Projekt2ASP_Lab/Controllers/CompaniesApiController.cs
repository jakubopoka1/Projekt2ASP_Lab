using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt2ASP_Lab.Models.Movies;

namespace Projekt2ASP_Lab.Controllers;

[ApiController]
[Route("/api/companies")]
public class CompaniesApiController : ControllerBase
{
    private MoviesDbContext _context;

    public CompaniesApiController(MoviesDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetFiltered(string fragment)
    {
        return Ok(
            _context.ProductionCompanies
                .Where(c => c.CompanyName.ToLower().Contains(fragment.ToLower()))
                .AsNoTracking()
                .AsEnumerable()
            );
    }
}